using System;
using System.Diagnostics.CodeAnalysis;

namespace TransactionProcessorACL.DataTransferObjects.Requests;

[ExcludeFromCodeCoverage]
public class RecentActivityReceiptSearchRequest
{
    public string ApplicationVersion { get; set; }

    public int MerchantReportingId { get; set; }

    public DateTime ReportDate { get; set; }

    public string? SearchText { get; set; }

    public int PageNumber { get; set; } = 1;

    public int PageSize { get; set; } = 5;
}
