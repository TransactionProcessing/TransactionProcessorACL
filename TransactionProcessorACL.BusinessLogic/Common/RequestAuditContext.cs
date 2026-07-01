using System;
using System.Collections.Generic;

namespace TransactionProcessorACL.Common;

public sealed record RequestAuditContext(
    string RequestId,
    string TraceId,
    DateTimeOffset TimestampUtc,
    string Method,
    string Route,
    string SourceIp,
    string UserAgent,
    Guid? EstateId,
    Guid? MerchantId,
    string TransactionType,
    string? TransactionNumber,
    IReadOnlyDictionary<string, string> BusinessContext);
