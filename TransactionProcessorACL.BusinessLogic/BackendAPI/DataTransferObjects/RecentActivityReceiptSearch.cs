using System;
using System.Collections.Generic;

namespace TransactionProcessorACL.BusinessLogic.BackendAPI.DataTransferObjects;

public class RecentActivityReceiptSearchRequest
{
    public string? ApplicationVersion { get; set; }

    public int? MerchantReportingId { get; set; }

    public DateTime ReportDate { get; set; }

    public string? SearchText { get; set; }

    public int PageNumber { get; set; } = 1;

    public int PageSize { get; set; } = 5;
}

public class RecentActivityReceiptSearchResponse
{
    public DateTime ReportDate { get; set; }

    public int PageNumber { get; set; }

    public int PageSize { get; set; }

    public int TotalCount { get; set; }

    public List<RecentActivityReceiptSearchItem> Items { get; set; } = [];
}

public class RecentActivityReceiptSearchItem
{
    public string? Reference { get; set; }

    public string? TransactionType { get; set; }

    public string? Product { get; set; }

    public string? Operator { get; set; }

    public string? Status { get; set; }

    public decimal Amount { get; set; }

    public DateTime TransactionDateTime { get; set; }

    public string? ReceiptReference { get; set; }
}
