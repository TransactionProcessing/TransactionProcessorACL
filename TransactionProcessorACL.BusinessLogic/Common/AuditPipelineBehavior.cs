using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace TransactionProcessorACL.Common;

public sealed class AuditPipelineBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IRequestAuditRecorder _requestAuditRecorder;

    public AuditPipelineBehavior(IHttpContextAccessor httpContextAccessor,
                                 IRequestAuditRecorder requestAuditRecorder)
    {
        _httpContextAccessor = httpContextAccessor;
        _requestAuditRecorder = requestAuditRecorder;
    }

    public async Task<TResponse> Handle(TRequest request,
                                        RequestHandlerDelegate<TResponse> next,
                                        CancellationToken cancellationToken)
    {
        HttpContext? httpContext = _httpContextAccessor.HttpContext;
        if (httpContext is null)
        {
            return await next().ConfigureAwait(false);
        }

        TResponse? response = default;
        Exception? exception = null;

        try
        {
            response = await next(cancellationToken).ConfigureAwait(false);
            return response;
        }
        catch (Exception ex)
        {
            exception = ex;
            throw;
        }
        finally
        {
            RequestAuditEvent auditEvent = RequestAuditContextBuilder.Build(
                httpContext,
                request,
                exception is null ? "Succeeded" : "Failed",
                exception?.Message);

            try
            {
                await _requestAuditRecorder.RecordAsync(auditEvent, cancellationToken).ConfigureAwait(false);
            }
            catch
            {
                // Audit must not interfere with the request path.
            }
        }
    }
}
