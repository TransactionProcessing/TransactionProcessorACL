using System;
using System.Collections.Generic;
using System.ComponentModel;
using TransactionProcessorACL.DataTransferObjects.Requests;

namespace TransactionProcessorACL.DataTransferObjects.Responses;

public class MerchantTransactionMixSummaryResponse
{
    [Description("Start of the reporting period represented by the result.")]
    public DateTime FromDate { get; set; }

    [Description("End of the reporting period represented by the result.")]
    public DateTime ToDate { get; set; }

    [Description("Dimension used to group the results.")]
    public TransactionMixBreakdown Breakdown { get; set; }

    [Description("Measure returned for each group: Count or Value.")]
    public TransactionMixMeasure Measure { get; set; }

    [Description("Total transaction count across the requested period.")]
    public decimal TotalCount { get; set; }

    [Description("Total transaction value across the requested period.")]
    public decimal TotalValue { get; set; }

    [Description("Ranked transaction groups, limited by the request's TopN value.")]
    public List<TransactionMixSummaryItem> Items { get; set; } = [];

    [Description("Transaction rows contributing to the summary.")]
    public List<TransactionMixDrillDownTransaction> DrillDownTransactions { get; set; } = [];
}

public class TransactionMixSummaryItem
{
    [Description("Stable grouping key for this item.")]
    public string Key { get; set; }

    [Description("Display label for the group.")]
    public string Label { get; set; }

    [Description("Transaction count for this group.")]
    public decimal Count { get; set; }

    [Description("Transaction value for this group.")]
    public decimal Value { get; set; }
}

public class TransactionMixDrillDownTransaction
{
    [Description("Transaction reference.")]
    public string Reference { get; set; }

    [Description("Transaction type.")]
    public string TransactionType { get; set; }

    [Description("Product associated with the transaction.")]
    public string Product { get; set; }

    [Description("Operator associated with the transaction.")]
    public string Operator { get; set; }

    [Description("Transaction status.")]
    public string Status { get; set; }

    [Description("Transaction amount.")]
    public decimal Amount { get; set; }

    [Description("Date and time when the transaction occurred.")]
    public DateTime TransactionDateTime { get; set; }
}
