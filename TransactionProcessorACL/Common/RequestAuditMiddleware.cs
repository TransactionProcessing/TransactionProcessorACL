using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using NLog;

namespace TransactionProcessorACL.Common;

public sealed class RequestAuditMiddleware
{
    private readonly RequestDelegate _next;
    private readonly RequestAuditContextAccessor _accessor;

    public RequestAuditMiddleware(RequestDelegate next, RequestAuditContextAccessor accessor)
    {
        _next = next;
        _accessor = accessor;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        RequestAuditContext requestAuditContext = RequestAuditContextFactory.CreateForRequest(context);
        _accessor.Current = requestAuditContext;
        MappedDiagnosticsLogicalContext.Set("CorrelationId", requestAuditContext.RequestId);
        try
        {
            await _next(context).ConfigureAwait(false);
        }
        finally
        {
            _accessor.Current = null;
            MappedDiagnosticsLogicalContext.Remove("CorrelationId");
        }
    }
}
