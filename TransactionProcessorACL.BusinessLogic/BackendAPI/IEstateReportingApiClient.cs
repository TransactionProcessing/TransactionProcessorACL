using System;
using System.Threading;
using System.Threading.Tasks;
using SimpleResults;
using TransactionProcessorACL.BusinessLogic.BackendAPI.DataTransferObjects;


namespace TransactionProcessorACL.BusinessLogic.BackendAPI;

public interface IEstateReportingApiClient
{
    Task<Result<MerchantDailyPerformanceSummaryResponse>> GetMerchantDailyPerformanceSummary(String accessToken,
                                                                                              Guid estateId,
                                                                                              MerchantDailyPerformanceSummaryRequest request,
                                                                                              CancellationToken cancellationToken);

    Task<Result<TransactionMixSummaryResponse>> GetMerchantTransactionMixSummary(String accessToken,
                                                                                 Guid estateId,
                                                                                 TransactionMixSummaryRequest request,
                                                                                 CancellationToken cancellationToken);

    Task<Result<RecentActivityReceiptSearchResponse>> GetRecentActivityReceiptSearch(String accessToken,
                                                                                      Guid estateId,
                                                                                      RecentActivityReceiptSearchRequest request,
                                                                                      CancellationToken cancellationToken);
}
