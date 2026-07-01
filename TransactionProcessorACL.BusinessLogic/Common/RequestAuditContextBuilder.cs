using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using System.Text;
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
        if (request is null)
        {
            return businessContext;
        }

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
                continue;
            }

            if (TryAddDictionaryBusinessContext(businessContext, value))
            {
                continue;
            }

            if (TryFormatValue(value, out String formattedValue))
            {
                businessContext[GetBusinessContextKey(property.Name, value)] = formattedValue;
            }
        }

        return businessContext;
    }

    private static String GetBusinessContextKey(String propertyName, object value)
    {
        String key = ToSnakeCase(propertyName);
        return value is DateTime or DateTimeOffset ? $"{key}_utc" : key;
    }

    private static bool TryAddDictionaryBusinessContext(IDictionary<String, String> businessContext, object value)
    {
        if (value is not IEnumerable<KeyValuePair<String, String>> entries)
        {
            return false;
        }

        foreach (KeyValuePair<String, String> item in entries)
        {
            if (LooksSensitive(item.Key))
            {
                continue;
            }

            businessContext[item.Key] = item.Value;
        }

        return true;
    }

    private static String? TryGetTransactionNumber<TRequest>(TRequest request)
    {
        if (request is null)
        {
            return null;
        }

        PropertyInfo? transactionNumberProperty = request.GetType().GetProperty(
            "TransactionNumber",
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase);

        if (transactionNumberProperty is null || !transactionNumberProperty.CanRead)
        {
            return null;
        }

        object? value = transactionNumberProperty.GetValue(request);
        return value switch
        {
            null => null,
            String stringValue => stringValue,
            _ => value.ToString()
        };
    }

    private static bool LooksSensitive(String key)
    {
        return key.Contains("password", StringComparison.OrdinalIgnoreCase)
               || key.Contains("token", StringComparison.OrdinalIgnoreCase)
               || key.Contains("secret", StringComparison.OrdinalIgnoreCase)
               || key.Contains("authorization", StringComparison.OrdinalIgnoreCase);
    }

    private static bool TryFormatValue(object value, out String formattedValue)
    {
        switch (value)
        {
            case String stringValue:
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
            case Boolean booleanValue:
                formattedValue = booleanValue.ToString();
                return true;
            case IFormattable formattableValue:
                formattedValue = formattableValue.ToString(null, CultureInfo.InvariantCulture) ?? String.Empty;
                return true;
            default:
                formattedValue = String.Empty;
                return false;
        }
    }

    private static String ToSnakeCase(String value)
    {
        if (String.IsNullOrWhiteSpace(value))
        {
            return value;
        }

        StringBuilder builder = new();
        for (Int32 index = 0; index < value.Length; index++)
        {
            Char character = value[index];
            if (Char.IsUpper(character) && builder.Length > 0)
            {
                builder.Append('_');
            }

            builder.Append(Char.ToLowerInvariant(character));
        }

        return builder.ToString();
    }
}
