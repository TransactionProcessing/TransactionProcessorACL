using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Http;
using TransactionProcessorACL.BusinessLogic.Requests;

namespace TransactionProcessorACL.Common;

public static class RequestAuditContextBuilder
{
    public static RequestAuditEvent Build<TRequest>(HttpContext httpContext, TRequest request, String outcome, String? errorMessage)
    {
        IReadOnlyDictionary<String, String> businessContext = BuildBusinessContext(request);
        (Guid? estateId, Guid? merchantId) = ExtractClaims(httpContext.User);
        String requestType = request?.GetType().Name ?? typeof(TRequest).Name;

        RequestAuditContext context = new(
            RequestId: RequestIdProvider.GetOrCreateRequestId(httpContext),
            TraceId: httpContext.TraceIdentifier,
            TimestampUtc: DateTimeOffset.UtcNow,
            Method: httpContext.Request.Method,
            Route: httpContext.Request.Path.Value ?? String.Empty,
            SourceIp: httpContext.Connection.RemoteIpAddress?.ToString() ?? String.Empty,
            UserAgent: httpContext.Request.Headers["User-Agent"].ToString(),
            EstateId: estateId,
            MerchantId: merchantId,
            TransactionType: requestType,
            TransactionNumber: TryGetTransactionNumber(request),
            BusinessContext: businessContext);

        return new RequestAuditEvent(context, requestType, outcome, errorMessage, businessContext);
    }

    private static (Guid? EstateId, Guid? MerchantId) ExtractClaims(ClaimsPrincipal principal)
    {
        return (TryGetGuidClaim(principal, "estateId"), TryGetGuidClaim(principal, "merchantId"));
    }

    private static Guid? TryGetGuidClaim(ClaimsPrincipal principal, String claimType)
    {
        Claim? claim = principal.Claims.FirstOrDefault(c => String.Equals(c.Type, claimType, StringComparison.OrdinalIgnoreCase));
        return Guid.TryParse(claim?.Value, out Guid value) ? value : null;
    }

    private static IReadOnlyDictionary<String, String> BuildBusinessContext<TRequest>(TRequest request)
    {
        Dictionary<String, String> businessContext = new(StringComparer.OrdinalIgnoreCase);

        switch (request)
        {
            case TransactionCommands.ProcessLogonTransactionCommand logon:
                businessContext["transaction_number"] = logon.TransactionNumber;
                businessContext["device_identifier"] = logon.DeviceIdentifier;
                businessContext["transaction_date_time_utc"] = logon.TransactionDateTime.ToUniversalTime().ToString("O");
                businessContext["transaction_kind"] = "LOGON";
                break;
            case TransactionCommands.ProcessSaleTransactionCommand sale:
                businessContext["transaction_number"] = sale.TransactionNumber;
                businessContext["device_identifier"] = sale.DeviceIdentifier;
                businessContext["transaction_date_time_utc"] = sale.TransactionDateTime.ToUniversalTime().ToString("O");
                businessContext["transaction_kind"] = "SALE";
                businessContext["customer_email_address"] = sale.CustomerEmailAddress ?? String.Empty;
                businessContext["contract_id"] = sale.ContractId.ToString();
                businessContext["product_id"] = sale.ProductId.ToString();
                businessContext["operator_id"] = sale.OperatorId.ToString();
                AddSafeMetadata(businessContext, sale.AdditionalRequestMetadata);
                break;
            case TransactionCommands.ProcessReconciliationCommand reconciliation:
                businessContext["device_identifier"] = reconciliation.DeviceIdentifier;
                businessContext["transaction_date_time_utc"] = reconciliation.TransactionDateTime.ToUniversalTime().ToString("O");
                businessContext["transaction_kind"] = "RECONCILIATION";
                businessContext["transaction_count"] = reconciliation.TransactionCount.ToString();
                businessContext["transaction_value"] = reconciliation.TransactionValue.ToString();
                break;
            case VersionCheckCommands.VersionCheckCommand versionCheck:
                businessContext["application_version"] = versionCheck.VersionNumber ?? String.Empty;
                break;
            case MerchantQueries.GetMerchantContractsQuery merchantContracts:
                businessContext["estate_id"] = merchantContracts.EstateId.ToString();
                businessContext["merchant_id"] = merchantContracts.MerchantId.ToString();
                break;
            case MerchantQueries.GetMerchantQuery merchant:
                businessContext["estate_id"] = merchant.EstateId.ToString();
                businessContext["merchant_id"] = merchant.MerchantId.ToString();
                break;
            case MerchantQueries.GetMerchantScheduleQuery merchantSchedule:
                businessContext["estate_id"] = merchantSchedule.EstateId.ToString();
                businessContext["merchant_id"] = merchantSchedule.MerchantId.ToString();
                businessContext["year"] = merchantSchedule.Year.ToString();
                break;
            case VoucherQueries.GetVoucherQuery voucherQuery:
                businessContext["estate_id"] = voucherQuery.EstateId.ToString();
                businessContext["contract_id"] = voucherQuery.ContractId.ToString();
                businessContext["voucher_code"] = voucherQuery.VoucherCode ?? String.Empty;
                break;
            case VoucherCommands.RedeemVoucherCommand redeemVoucher:
                businessContext["estate_id"] = redeemVoucher.EstateId.ToString();
                businessContext["contract_id"] = redeemVoucher.ContractId.ToString();
                businessContext["voucher_code"] = redeemVoucher.VoucherCode ?? String.Empty;
                break;
        }

        return businessContext;
    }

    private static void AddSafeMetadata(IDictionary<String, String> businessContext, IDictionary<String, String>? additionalRequestMetadata)
    {
        if (additionalRequestMetadata is null)
        {
            return;
        }

        foreach (KeyValuePair<String, String> item in additionalRequestMetadata)
        {
            if (LooksSensitive(item.Key))
            {
                continue;
            }

            businessContext[item.Key] = item.Value;
        }
    }

    private static bool LooksSensitive(String key)
    {
        return key.Contains("password", StringComparison.OrdinalIgnoreCase)
               || key.Contains("token", StringComparison.OrdinalIgnoreCase)
               || key.Contains("secret", StringComparison.OrdinalIgnoreCase)
               || key.Contains("authorization", StringComparison.OrdinalIgnoreCase);
    }

    private static String? TryGetTransactionNumber<TRequest>(TRequest request)
    {
        return request switch
        {
            TransactionCommands.ProcessLogonTransactionCommand logon => logon.TransactionNumber,
            TransactionCommands.ProcessSaleTransactionCommand sale => sale.TransactionNumber,
            _ => null
        };
    }
}
