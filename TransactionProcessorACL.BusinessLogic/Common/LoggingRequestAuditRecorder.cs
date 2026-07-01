using KurrentDB.Client;
using Microsoft.AspNetCore.Http;
using Shared.Logger;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace TransactionProcessorACL.Common;

public sealed class LoggingRequestAuditRecorder : IRequestAuditRecorder
{
    public Task RecordAsync(RequestAuditEvent auditEvent, CancellationToken cancellationToken)
    {
        string payload = JsonSerializer.Serialize(auditEvent);
        Logger.LogWarning($"AUDIT {payload}");
        return Task.CompletedTask;
    }

    public class KurrentDbRequestAuditRecorder : IRequestAuditRecorder
    {
        private readonly KurrentDBClient KurrentDbClient;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly RequestAuditStreamOptions _streamOptions;

        public KurrentDbRequestAuditRecorder(KurrentDBClient KurrentDBClient,
                                            IHttpContextAccessor httpContextAccessor,
                                            RequestAuditStreamOptions streamOptions) {
            this.KurrentDbClient = KurrentDBClient;
            _httpContextAccessor = httpContextAccessor;
            _streamOptions = streamOptions;
        }

        public async Task RecordAsync(RequestAuditEvent auditEvent, CancellationToken cancellationToken) {
            String requestId = GetRequestId(auditEvent);
            String streamName = $"RequestAudit-{requestId}";

            StreamMetadata streamMetadata = new(
                maxCount: null,
                maxAge: _streamOptions.RequestAuditStreamLifetime,
                truncateBefore: null,
                cacheControl: null,
                acl: null,
                customMetadata: null);

            await this.KurrentDbClient.SetStreamMetadataAsync(
                streamName,
                StreamState.Any,
                streamMetadata,
                cancellationToken: cancellationToken);

            List<EventData> events = [CreateEventData(Guid.NewGuid(), "RequestAuditEvent", JsonSerializer.Serialize(auditEvent), "")];

            await this.KurrentDbClient.AppendToStreamAsync(streamName, StreamState.Any, events, cancellationToken: cancellationToken);
        }

        private String GetRequestId(RequestAuditEvent auditEvent)
        {
            HttpContext? httpContext = _httpContextAccessor.HttpContext;
            if (httpContext is not null)
            {
                return RequestIdProvider.GetOrCreateRequestId(httpContext);
            }

            return auditEvent.Context.RequestId;
        }

        private static EventData CreateEventData(Guid eventId,
                                                 String eventType,
                                                 String data,
                                                 String metadata)
        {
            Byte[] eventData = Encoding.Default.GetBytes(data);
            Byte[] eventMetadata = Encoding.Default.GetBytes(metadata);

            EventData @event = new(Uuid.FromGuid(eventId), eventType, eventData, eventMetadata);

            return @event;
        }
    }
}
