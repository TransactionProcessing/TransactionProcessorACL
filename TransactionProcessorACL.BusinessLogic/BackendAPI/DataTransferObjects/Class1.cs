using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace TransactionProcessorACL.BusinessLogic.BackendAPI.DataTransferObjects
{
    public class TransactionMixSummaryResponse
    {
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public TransactionMixBreakdown Breakdown { get; set; }
        public TransactionMixMeasure Measure { get; set; }
        public int TotalCount { get; set; }
        public decimal TotalValue { get; set; }
        public List<TransactionMixSummaryGroup> Groups { get; set; } = [];
        public List<TransactionMixSummaryTransaction> Transactions { get; set; } = [];
    }

    public class TransactionMixSummaryGroup
    {
        public string? GroupKey { get; set; }
        public string? GroupName { get; set; }
        public int TransactionCount { get; set; }
        public decimal TransactionValue { get; set; }
    }

    public class TransactionMixSummaryTransaction
    {
        public Guid Id { get; set; }
        public DateTime DateTime { get; set; }
        public string? Merchant { get; set; }
        public Guid MerchantId { get; set; }
        public int MerchantReportingId { get; set; }
        public string? Operator { get; set; }
        public Guid OperatorId { get; set; }
        public int OperatorReportingId { get; set; }
        public string? Product { get; set; }
        public Guid ProductId { get; set; }
        public int ProductReportingId { get; set; }
        public string? Type { get; set; }
        public string? Status { get; set; }
        public decimal Value { get; set; }
        public decimal TotalFees { get; set; }
        public string? SettlementReference { get; set; }
        public int TransactionNumber { get; set; }
    }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum TransactionMixBreakdown
    {
        Product,
        TransactionType,
        Operator,
        Status
    }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum TransactionMixMeasure
    {
        Count,
        Value
    }

    public class TransactionMixSummaryRequest
    {
        public int? MerchantReportingId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public TransactionMixBreakdown Breakdown { get; set; }
        public TransactionMixMeasure Measure { get; set; }
        public int TopN { get; set; } = 5;
    }

    public class MerchantDailyPerformanceSummaryRequest
    {
        public Int32 MerchantReportingId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }

    public sealed class MerchantDailyPerformanceSummaryResponse
    {
        public List<MetricItem> Metrics { get; set; } = [];

        public List<DrillDownTransaction> DrillDownTransactions { get; set; } = [];
    }

    public sealed class MetricItem
    {
        public string Title { get; set; }

        public Decimal Value { get; set; }

        public string Description { get; set; }

        public int Category { get; set; }
    }

    public sealed class DrillDownTransaction
    {
        public string Reference { get; set; }

        public string Product { get; set; }

        public string Status { get; set; }

        public Decimal Amount { get; set; }

        public DateTime TransactionDateTime { get; set; }
    }
}
