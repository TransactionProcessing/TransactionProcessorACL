using System;
using System.Collections.Generic;
using TransactionProcessorACL.DataTransferObjects.Requests;

namespace TransactionProcessorACL.IntegrationTests.Shared;

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
