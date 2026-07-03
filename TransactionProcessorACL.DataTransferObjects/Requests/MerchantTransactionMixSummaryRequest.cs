using System;
using System.Diagnostics.CodeAnalysis;

namespace TransactionProcessorACL.DataTransferObjects.Requests;

[ExcludeFromCodeCoverage]
public class MerchantTransactionMixSummaryRequest
{
    public int MerchantReportingId { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public TransactionMixBreakdown Breakdown { get; set; }

    public TransactionMixMeasure Measure { get; set; }

    public int TopN { get; set; } = 5;
}

public enum TransactionMixBreakdown
{
    NotSet = 0,
    TransactionType = 1,
    Product = 2,
    Operator = 3,
    Status = 4
}

public enum TransactionMixMeasure
{
    NotSet = 0,
    Count = 1,
    Value = 2
}
