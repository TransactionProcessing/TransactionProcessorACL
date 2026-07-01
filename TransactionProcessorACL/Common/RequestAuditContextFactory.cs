using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using TransactionProcessorACL.DataTransferObjects;
using System.Text;

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
        var businessContext = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        foreach (PropertyInfo property in request.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public))
        {
            if (!property.CanRead || property.GetIndexParameters().Length > 0)
            {
                continue;
            }

            if (LooksSensitive(property.Name))
            {
                continue;
            }

            object? value = property.GetValue(request);
            if (value is null)
            {
                if (property.PropertyType == typeof(string))
                {
                    businessContext[ToSnakeCase(property.Name)] = string.Empty;
                }

                continue;
            }

            if (TryAddDictionaryBusinessContext(businessContext, value))
            {
                continue;
            }

            if (TryFormatValue(value, out string formattedValue))
            {
                businessContext[GetBusinessContextKey(property.Name, value)] = formattedValue;
            }
        }

        return businessContext;
    }

    private static string GetBusinessContextKey(string propertyName, object value)
    {
        return value is DateTime or DateTimeOffset
            ? $"{ToSnakeCase(propertyName)}_utc"
            : ToSnakeCase(propertyName);
    }

    private static bool TryAddDictionaryBusinessContext(IDictionary<string, string> businessContext, object value)
    {
        if (value is not IEnumerable<KeyValuePair<string, string>> entries)
        {
            return false;
        }

        foreach (KeyValuePair<string, string> item in entries)
        {
            if (LooksSensitive(item.Key))
            {
                continue;
            }

            businessContext[item.Key] = item.Value;
        }

        return true;
    }

    private static bool LooksSensitive(string key)
    {
        return key.Contains("password", StringComparison.OrdinalIgnoreCase)
               || key.Contains("token", StringComparison.OrdinalIgnoreCase)
               || key.Contains("secret", StringComparison.OrdinalIgnoreCase)
               || key.Contains("authorization", StringComparison.OrdinalIgnoreCase);
    }

    private static bool TryFormatValue(object value, out string formattedValue)
    {
        switch (value)
        {
            case string stringValue:
                formattedValue = stringValue;
                return true;
            case Guid guidValue:
                formattedValue = guidValue.ToString();
                return true;
            case DateTime dateTimeValue:
                formattedValue = dateTimeValue.ToUniversalTime().ToString("O");
                return true;
            case DateTimeOffset dateTimeOffsetValue:
                formattedValue = dateTimeOffsetValue.ToUniversalTime().ToString("O");
                return true;
            case Enum enumValue:
                formattedValue = enumValue.ToString();
                return true;
            case bool boolValue:
                formattedValue = boolValue.ToString();
                return true;
            case IFormattable formattableValue:
                formattedValue = formattableValue.ToString(null, CultureInfo.InvariantCulture) ?? string.Empty;
                return true;
            default:
                formattedValue = string.Empty;
                return false;
        }
    }

    private static string ToSnakeCase(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return value;
        }

        StringBuilder builder = new();

        for (int index = 0; index < value.Length; index++)
        {
            char character = value[index];
            if (char.IsUpper(character) && builder.Length > 0)
            {
                builder.Append('_');
            }

            builder.Append(char.ToLowerInvariant(character));
        }

        return builder.ToString();
    }
}
