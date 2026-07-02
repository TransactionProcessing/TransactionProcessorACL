using System;
using System.Collections.Generic;

namespace TransactionProcessorACL.Models;

public class MerchantDailyPerformanceSummaryResponse
{
    public List<MetricItem> Metrics { get; set; } = [];

    public List<DrillDownTransaction> DrillDownTransactions { get; set; } = [];
}

public class MetricItem
{
    public string Title { get; set; }

    public decimal Value { get; set; }

    public string Description { get; set; }

    public int Category { get; set; }
}

public class DrillDownTransaction
{
    public string Reference { get; set; }

    public string Product { get; set; }

    public string Status { get; set; }

    public decimal Amount { get; set; }

    public DateTime TransactionDateTime { get; set; }
}
