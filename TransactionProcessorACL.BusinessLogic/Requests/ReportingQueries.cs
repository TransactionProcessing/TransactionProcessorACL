using System;
using System.Diagnostics.CodeAnalysis;
using MediatR;
using SimpleResults;
using TransactionProcessorACL.DataTransferObjects.Requests;
using TransactionProcessorACL.Models;

namespace TransactionProcessorACL.BusinessLogic.Requests;

[ExcludeFromCodeCoverage]
public record ReportingQueries
{
    public record GetMerchantDailyPerformanceSummaryQuery(Guid EstateId,
                                                          MerchantDailyPerformanceSummaryRequest Request)
        : IRequest<Result<MerchantDailyPerformanceSummaryResponse>>;

    public record GetMerchantTransactionMixSummaryQuery(Guid EstateId,
                                                        MerchantTransactionMixSummaryRequest Request)
        : IRequest<Result<MerchantTransactionMixSummaryResponse>>;
}
