using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Shouldly;
using TransactionProcessorACL.Common;
using Xunit;

namespace TransactionProcessorACL.Tests.Audit;

public class CorrelationHeaderDelegatingHandlerTests
{
    [Fact]
    public async Task SendAsync_WhenRequestContextExists_AddsRequestIdHeader()
    {
        var accessor = new RequestAuditContextAccessor();
        accessor.Current = new RequestAuditContext(
            RequestId: "req-123",
            TraceId: "trace-123",
            TimestampUtc: System.DateTimeOffset.UtcNow,
            Method: "POST",
            Route: "/api/saletransactions",
            SourceIp: "203.0.113.9",
            UserAgent: "TestClient/1.0",
            EstateId: null,
            MerchantId: null,
            TransactionType: "SALE",
            TransactionNumber: "TX-0001",
            BusinessContext: new System.Collections.Generic.Dictionary<string, string>());

        HttpRequestMessage? capturedRequest = null;
        var innerHandler = new CapturingHandler(request =>
        {
            capturedRequest = request;
            return new HttpResponseMessage(HttpStatusCode.OK);
        });

        var handler = new CorrelationHeaderDelegatingHandler(accessor)
        {
            InnerHandler = innerHandler
        };

        using var invoker = new HttpMessageInvoker(handler);

        HttpResponseMessage response = await invoker.SendAsync(new HttpRequestMessage(HttpMethod.Get, "https://example.test"), CancellationToken.None);

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        capturedRequest.ShouldNotBeNull();
        capturedRequest!.Headers.Contains("X-Request-Id").ShouldBeTrue();
        capturedRequest.Headers.GetValues("X-Request-Id").ShouldContain("req-123");
    }

    private sealed class CapturingHandler : HttpMessageHandler
    {
        private readonly System.Func<HttpRequestMessage, HttpResponseMessage> _callback;

        public CapturingHandler(System.Func<HttpRequestMessage, HttpResponseMessage> callback)
        {
            _callback = callback;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return Task.FromResult(_callback(request));
        }
    }
}
