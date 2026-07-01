using System;

namespace TransactionProcessorACL.Common;

public sealed record RequestAuditStreamOptions
{
    public Int32 RequestAuditStreamLifetimeDays { get; init; } = 7;

    public TimeSpan RequestAuditStreamLifetime => TimeSpan.FromDays(RequestAuditStreamLifetimeDays);
}
