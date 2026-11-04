using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace TransactionProcessorACL.BusinessLogic.BackendAPI.DataTransferObjects
{
    /*public class TransactionMixSummaryResponse
    {
        public DateOnly FromDate { get; set; }
        public DateOnly ToDate { get; set; }
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
        public DateTimeOffset DateTime { get; set; }
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
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
        public TransactionMixBreakdown Breakdown { get; set; }
        public TransactionMixMeasure Measure { get; set; }
        public int TopN { get; set; } = 5;
    }

    public class MerchantDailyPerformanceSummaryRequest
    {
        public Int32 MerchantReportingId { get; set; }
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
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
*/

public sealed class MerchantDailyPerformanceSummaryResponse {
    /// <summary>The metrics value.</summary>
    public List<MetricItem> Metrics { get; set; } = [];

    /// <summary>The drill down transactions value.</summary>

    public List<DrillDownTransaction> DrillDownTransactions { get; set; } = [];
}

public sealed class MetricItem {
    /// <summary>The title value.</summary>
    public string Title { get; set; }

    /// <summary>The value value.</summary>

    public Decimal Value { get; set; }

    /// <summary>The description value.</summary>

    public string Description { get; set; }

    /// <summary>The category value.</summary>

    public int Category { get; set; }
    /// <summary>The type value.</summary>
    public Int32 Type { get; set; }
}

public sealed class DrillDownTransaction {
    /// <summary>The reference value.</summary>
    public string Reference { get; set; }

    /// <summary>The product value.</summary>

    public string Product { get; set; }

    /// <summary>The status value.</summary>

    public string Status { get; set; }

    /// <summary>The amount value.</summary>

    public Decimal Amount { get; set; }

    /// <summary>Date and time of the transaction.</summary>

    public DateTimeOffset TransactionDateTime { get; set; }
}

public class MerchantDailyPerformanceSummaryRequest {
    /// <summary>Numeric reporting identifier used to filter or identify a merchant.</summary>
    public Int32 MerchantReportingId { get; set; }
    /// <summary>Inclusive start date of the report period.</summary>
    public DateOnly StartDate { get; set; }
    /// <summary>Inclusive end date of the report period.</summary>
    public DateOnly EndDate { get; set; }
}

public class TransactionMixSummaryResponse {
    /// <summary>The from date value.</summary>
    public DateOnly FromDate { get; set; }
    /// <summary>The to date value.</summary>
    public DateOnly ToDate { get; set; }
    /// <summary>Dimension used to group transactions.</summary>
    public TransactionMixBreakdown Breakdown { get; set; }
    /// <summary>Metric used to rank or compare transaction groups.</summary>
    public TransactionMixMeasure Measure { get; set; }
    /// <summary>The total count value.</summary>
    public int TotalCount { get; set; }
    /// <summary>Total monetary value represented by this result.</summary>
    public decimal TotalValue { get; set; }
    /// <summary>The groups value.</summary>
    public List<TransactionMixSummaryGroup> Groups { get; set; } = [];
    /// <summary>The transactions value.</summary>
    public List<TransactionMixSummaryTransaction> Transactions { get; set; } = [];
    /// <summary>The pagination value.</summary>
    public PaginationMetadata Pagination { get; set; } = new();
}

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum TransactionMixBreakdown {
    /// <summary>Group transactions by product.</summary>
    Product,
    /// <summary>Group transactions by transaction type.</summary>
    TransactionType,
    /// <summary>Group transactions by operator.</summary>
    Operator,
    /// <summary>Group transactions by transaction status.</summary>
    Status
}

/// <summary>API data model describing transaction mix measure information.</summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum TransactionMixMeasure {
    /// <summary>Compare groups by transaction count.</summary>
    Count,
    /// <summary>Compare groups by transaction value.</summary>
    Value
}

public class TransactionMixSummaryGroup {
    /// <summary>The group key value.</summary>
    public string? GroupKey { get; set; }
    /// <summary>The group name value.</summary>
    public string? GroupName { get; set; }
    /// <summary>Number of transactions represented by this result.</summary>
    public int TransactionCount { get; set; }
    /// <summary>Total monetary value of the transactions represented by this result.</summary>
    public decimal TransactionValue { get; set; }
}

public class TransactionMixSummaryTransaction {
    /// <summary>The id value.</summary>
    public Guid Id { get; set; }
    /// <summary>Date and time of the transaction.</summary>
    public DateTimeOffset DateTime { get; set; }
    /// <summary>The merchant value.</summary>
    public string? Merchant { get; set; }
    /// <summary>Unique identifier of the merchant.</summary>
    public Guid MerchantId { get; set; }
    /// <summary>Numeric reporting identifier used to filter or identify a merchant.</summary>
    public int MerchantReportingId { get; set; }
    /// <summary>The operator value.</summary>
    public string? Operator { get; set; }
    /// <summary>Unique identifier of the operator.</summary>
    public Guid OperatorId { get; set; }
    /// <summary>The operator reporting id value.</summary>
    public int OperatorReportingId { get; set; }
    /// <summary>The product value.</summary>
    public string? Product { get; set; }
    /// <summary>Unique identifier of the product.</summary>
    public Guid ProductId { get; set; }
    /// <summary>The product reporting id value.</summary>
    public int ProductReportingId { get; set; }
    /// <summary>The type value.</summary>
    public string? Type { get; set; }
    /// <summary>The status value.</summary>
    public string? Status { get; set; }
    /// <summary>The value value.</summary>
    public decimal Value { get; set; }
    /// <summary>The total fees value.</summary>
    public decimal TotalFees { get; set; }
    /// <summary>The settlement reference value.</summary>
    public string? SettlementReference { get; set; }
    /// <summary>The transaction number value.</summary>
    public int TransactionNumber { get; set; }
}

/// <summary>Request criteria for transaction mix summary.</summary>

public class TransactionMixSummaryRequest {
    /// <summary>Numeric reporting identifier used to filter or identify a merchant.</summary>
    public int? MerchantReportingId { get; set; }
    /// <summary>Inclusive start date of the report period.</summary>
    public DateOnly StartDate { get; set; }
    /// <summary>Inclusive end date of the report period.</summary>
    public DateOnly EndDate { get; set; }
    /// <summary>Dimension used to group transactions.</summary>
    public TransactionMixBreakdown Breakdown { get; set; }
    /// <summary>Metric used to rank or compare transaction groups.</summary>
    public TransactionMixMeasure Measure { get; set; }
    /// <summary>Maximum number of groups selected before pagination.</summary>
    public int TopN { get; set; } = 5;
    /// <summary>One-based page number to return.</summary>
    public int PageNumber { get; set; } = 1;
    /// <summary>Maximum number of items to include on a page.</summary>
    public int PageSize { get; set; } = 50;
}
}