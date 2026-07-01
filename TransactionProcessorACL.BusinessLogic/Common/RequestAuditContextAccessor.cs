using System;
using System.Threading;

namespace TransactionProcessorACL.Common;

public sealed class RequestAuditContextAccessor
{
    private static readonly AsyncLocal<RequestAuditContext?> CurrentContext = new();

    public RequestAuditContext? Current
    {
        get => CurrentContext.Value;
        set => CurrentContext.Value = value;
    }
}
