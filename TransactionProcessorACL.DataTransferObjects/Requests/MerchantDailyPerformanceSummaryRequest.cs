using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace TransactionProcessorACL.DataTransferObjects.Requests;

[ExcludeFromCodeCoverage]
public class MerchantDailyPerformanceSummaryRequest
{
    [Description("Reporting identifier of the merchant to summarize.")]
    public int MerchantReportingId { get; set; }

    [Description("Inclusive start of the reporting date/time range.")]
    public DateTime StartDate { get; set; }

    [Description("Inclusive end of the reporting date/time range.")]
    public DateTime EndDate { get; set; }

    [Description("Client application version used by the version check when supplied in the JSON request body.")]
    public String ApplicationVersion { get; set; }
}
