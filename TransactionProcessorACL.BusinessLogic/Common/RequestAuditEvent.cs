using System;
using System.Collections.Generic;

namespace TransactionProcessorACL.Common;

public sealed record RequestAuditEvent(
    RequestAuditContext Context,
    String RequestType,
    String Outcome,
    String? ErrorMessage,
    IReadOnlyDictionary<String, String> BusinessContext);
