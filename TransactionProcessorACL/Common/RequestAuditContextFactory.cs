using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using TransactionProcessorACL.DataTransferObjects;

namespace TransactionProcessorACL.Common;

public static class RequestAuditContextFactory
{
    public static RequestAuditContext CreateForTransaction(HttpContext context,
                                                           ClaimsPrincipal user,
                                                           TransactionRequestMessage request,
                                                           string transactionType)
    {
        var requestId = RequestIdProvider.GetOrCreateRequestId(context);
        var businessContext = BuildBusinessContext(request);
        var claims = ExtractClaims(user);

        return new RequestAuditContext(
            RequestId: requestId,
            TraceId: context.TraceIdentifier,
            TimestampUtc: DateTimeOffset.UtcNow,
            Method: context.Request.Method,
            Route: context.Request.Path.Value ?? string.Empty,
            SourceIp: context.Connection.RemoteIpAddress?.ToString() ?? string.Empty,
            UserAgent: context.Request.Headers["User-Agent"].ToString(),
            EstateId: claims.EstateId,
            MerchantId: claims.MerchantId,
            TransactionType: transactionType,
            TransactionNumber: request.TransactionNumber,
            BusinessContext: businessContext);
    }

    public static RequestAuditContext CreateForRequest(HttpContext context)
    {
        return new RequestAuditContext(
            RequestId: RequestIdProvider.GetOrCreateRequestId(context),
            TraceId: context.TraceIdentifier,
            TimestampUtc: DateTimeOffset.UtcNow,
            Method: context.Request.Method,
            Route: context.Request.Path.Value ?? string.Empty,
            SourceIp: context.Connection.RemoteIpAddress?.ToString() ?? string.Empty,
            UserAgent: context.Request.Headers["User-Agent"].ToString(),
            EstateId: null,
            MerchantId: null,
            TransactionType: string.Empty,
            TransactionNumber: null,
            BusinessContext: new Dictionary<string, string>());
    }

    private static (Guid? EstateId, Guid? MerchantId) ExtractClaims(ClaimsPrincipal user)
    {
        Guid? estateId = TryGetGuidClaim(user, "estateId");
        Guid? merchantId = TryGetGuidClaim(user, "merchantId");
        return (estateId, merchantId);
    }

    private static Guid? TryGetGuidClaim(ClaimsPrincipal user, string claimType)
    {
        Claim? claim = user.Claims.FirstOrDefault(c => c.Type == claimType);
        return Guid.TryParse(claim?.Value, out Guid value) ? value : null;
    }

    private static IReadOnlyDictionary<string, string> BuildBusinessContext(TransactionRequestMessage request)
    {
        var businessContext = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["transaction_number"] = request.TransactionNumber ?? string.Empty,
            ["device_identifier"] = request.DeviceIdentifier ?? string.Empty,
            ["transaction_date_time_utc"] = request.TransactionDateTime.ToUniversalTime().ToString("O")
        };

        switch (request)
        {
            case SaleTransactionRequestMessage sale:
                businessContext["customer_email_address"] = sale.CustomerEmailAddress ?? string.Empty;
                businessContext["contract_id"] = sale.ContractId.ToString();
                businessContext["product_id"] = sale.ProductId.ToString();
                businessContext["operator_id"] = sale.OperatorId.ToString();
                AddSafeMetadata(businessContext, sale.AdditionalRequestMetadata);
                break;
            case ReconciliationRequestMessage reconciliation:
                businessContext["transaction_count"] = reconciliation.TransactionCount.ToString();
                businessContext["transaction_value"] = reconciliation.TransactionValue.ToString();
                break;
        }

        return businessContext;
    }

    private static void AddSafeMetadata(IDictionary<string, string> businessContext, IDictionary<string, string>? additionalRequestMetadata)
    {
        if (additionalRequestMetadata is null)
        {
            return;
        }

        foreach (KeyValuePair<string, string> item in additionalRequestMetadata)
        {
            if (LooksSensitive(item.Key))
            {
                continue;
            }

            businessContext[item.Key] = item.Value;
        }
    }

    private static bool LooksSensitive(string key)
    {
        return key.Contains("password", StringComparison.OrdinalIgnoreCase)
               || key.Contains("token", StringComparison.OrdinalIgnoreCase)
               || key.Contains("secret", StringComparison.OrdinalIgnoreCase)
               || key.Contains("authorization", StringComparison.OrdinalIgnoreCase);
    }
}
