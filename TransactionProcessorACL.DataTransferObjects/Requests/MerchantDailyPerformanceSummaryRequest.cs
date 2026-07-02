using System;
using System.Diagnostics.CodeAnalysis;

namespace TransactionProcessorACL.DataTransferObjects.Requests;

[ExcludeFromCodeCoverage]
public class MerchantDailyPerformanceSummaryRequest
{
    public int MerchantReportingId { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }
}
