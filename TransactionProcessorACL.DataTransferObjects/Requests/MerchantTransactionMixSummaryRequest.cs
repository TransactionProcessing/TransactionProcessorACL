using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace TransactionProcessorACL.DataTransferObjects.Requests;

[ExcludeFromCodeCoverage]
public class MerchantTransactionMixSummaryRequest
{
    [Description("Reporting identifier of the merchant to summarize.")]
    public int MerchantReportingId { get; set; }

    [Description("Inclusive start of the reporting date/time range.")]
    public DateTime StartDate { get; set; }

    [Description("Inclusive end of the reporting date/time range.")]
    public DateTime EndDate { get; set; }

    [Description("Numeric grouping dimension: 1=TransactionType, 2=Product, 3=Operator, 4=Status.")]
    public TransactionMixBreakdown Breakdown { get; set; }

    [Description("Numeric measure: 1=Count, 2=Value.")]
    public TransactionMixMeasure Measure { get; set; }

    [Description("Client application version used by the version check when supplied in the JSON request body.")]
    public String ApplicationVersion { get; set; }

    [Description("Maximum number of ranked groups to return. Defaults to 5.")]
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
