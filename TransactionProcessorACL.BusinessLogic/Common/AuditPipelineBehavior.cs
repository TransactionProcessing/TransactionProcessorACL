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

        Boolean failed = false;
        String? errorMessage = null;

        try
        {
            TResponse? response = await next(cancellationToken).ConfigureAwait(false);
            return response;
        }
        catch (Exception ex)
        {
            failed = true;
            errorMessage = ex.Message;
            throw;
        }
        finally
        {
            await RecordAuditAsync(httpContext, request, failed, errorMessage, cancellationToken).ConfigureAwait(false);
        }
    }

    private async Task RecordAuditAsync(HttpContext httpContext,
                                        TRequest request,
                                        Boolean failed,
                                        String? errorMessage,
                                        CancellationToken cancellationToken)
    {
        RequestAuditEvent auditEvent = RequestAuditContextBuilder.Build(
            httpContext,
            request,
            failed ? "Failed" : "Succeeded",
            errorMessage);

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
