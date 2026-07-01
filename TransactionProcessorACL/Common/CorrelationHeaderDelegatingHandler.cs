using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace TransactionProcessorACL.Common;

public sealed class CorrelationHeaderDelegatingHandler : DelegatingHandler
{
    private readonly RequestAuditContextAccessor _accessor;

    public CorrelationHeaderDelegatingHandler(RequestAuditContextAccessor accessor)
    {
        _accessor = accessor;
    }

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        RequestAuditContext? context = _accessor.Current;
        if (context is not null && !request.Headers.Contains("X-Request-Id"))
        {
            request.Headers.TryAddWithoutValidation("X-Request-Id", context.RequestId);
        }

        return base.SendAsync(request, cancellationToken);
    }
}
