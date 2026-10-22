using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace TransactionProcessorACL.DataTransferObjects.Requests;

[ExcludeFromCodeCoverage]
public class RecentActivityReceiptSearchRequest
{
    [Description("Client application version used by the version check when supplied in the JSON request body.")]
    public string ApplicationVersion { get; set; }

    [Description("Reporting identifier of the merchant to search.")]
    public int MerchantReportingId { get; set; }

    [Description("Date whose transaction activity should be searched.")]
    public DateTime ReportDate { get; set; }

    [Description("Optional text used to filter matching transactions or receipts.")]
    public string? SearchText { get; set; }

    [Description("One-based page number. Defaults to 1.")]
    public int PageNumber { get; set; } = 1;

    [Description("Maximum number of results per page. Defaults to 5.")]
    public int PageSize { get; set; } = 5;
}
