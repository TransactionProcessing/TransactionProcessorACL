using System;
using Microsoft.AspNetCore.Http;

namespace TransactionProcessorACL.Common;

public static class RequestIdProvider
{
    private static readonly object RequestIdItemKey = new();

    public static string GetOrCreateRequestId(HttpContext context)
    {
        if (context.Items.TryGetValue(RequestIdItemKey, out object? value) && value is string requestId && !string.IsNullOrWhiteSpace(requestId)) {
            return requestId;
        }

        string generatedRequestId = Guid.NewGuid().ToString("D");
        context.Items[RequestIdItemKey] = generatedRequestId;
        return generatedRequestId;
    }
}
