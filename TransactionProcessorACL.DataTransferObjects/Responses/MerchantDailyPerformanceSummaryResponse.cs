using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace TransactionProcessorACL.DataTransferObjects.Responses
{
    public class MerchantDailyPerformanceSummaryResponse
    {
        [Description("Summary metrics for the requested period.")]
        public List<MetricItem> Metrics { get; set; } = [];

        [Description("Transaction rows for drilling into the summary.")]
        public List<DrillDownTransaction> DrillDownTransactions { get; set; } = [];
    }

    public class MetricItem
    {
        [Description("Metric title.")]
        public string Title { get; set; }

        [Description("Numeric value for the metric.")]
        public decimal Value { get; set; }

        [Description("Human-readable explanation of the metric.")]
        public string Description { get; set; }

        [Description("Category identifier used by the reporting client.")]
        public int Category { get; set; }
    }

    public class DrillDownTransaction
    {
        [Description("Transaction reference.")]
        public string Reference { get; set; }

        [Description("Product associated with the transaction.")]
        public string Product { get; set; }

        [Description("Transaction status.")]
        public string Status { get; set; }

        [Description("Transaction amount.")]
        public decimal Amount { get; set; }

        [Description("Date and time when the transaction occurred.")]
        public DateTime TransactionDateTime { get; set; }
    }
}
