using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace TransactionProcessorACL.DataTransferObjects.Responses;

public class RecentActivityReceiptSearchResponse
{
    [Description("Date searched for recent activity.")]
    public DateTime ReportDate { get; set; }

    [Description("One-based page number returned.")]
    public int PageNumber { get; set; }

    [Description("Maximum number of results per page used for this response.")]
    public int PageSize { get; set; }

    [Description("Total number of matches across all pages.")]
    public int TotalCount { get; set; }

    [Description("Transaction and receipt records on the requested page.")]
    public List<RecentActivityReceiptSearchItem> Items { get; set; } = [];
}

public class RecentActivityReceiptSearchItem
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

    [Description("Receipt reference, when a receipt is available.")]
    public string ReceiptReference { get; set; }
}
