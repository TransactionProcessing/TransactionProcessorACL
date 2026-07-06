using System;
using System.Collections.Generic;

namespace TransactionProcessorACL.Models;

public class MerchantTransactionMixSummaryResponse
{
    public DateTime FromDate { get; set; }

    public DateTime ToDate { get; set; }

    public TransactionMixBreakdown Breakdown { get; set; }

    public TransactionMixMeasure Measure { get; set; }

    public decimal TotalCount { get; set; }

    public decimal TotalValue { get; set; }

    public List<TransactionMixSummaryItem> Items { get; set; } = [];

    public List<TransactionMixDrillDownTransaction> DrillDownTransactions { get; set; } = [];
}

public class TransactionMixSummaryItem
{
    public string Key { get; set; }

    public string Label { get; set; }

    public decimal Count { get; set; }

    public decimal Value { get; set; }
}

public class TransactionMixDrillDownTransaction
{
    public string Reference { get; set; }

    public string TransactionType { get; set; }

    public string Product { get; set; }

    public string Operator { get; set; }

    public string Status { get; set; }

    public decimal Amount { get; set; }

    public DateTime TransactionDateTime { get; set; }
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
