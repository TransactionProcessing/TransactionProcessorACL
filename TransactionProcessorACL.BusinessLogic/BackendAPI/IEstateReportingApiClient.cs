using System;
using System.Threading;
using System.Threading.Tasks;
using SimpleResults;
using TransactionProcessorACL.DataTransferObjects.Requests;
using TransactionProcessorACL.Models;

namespace TransactionProcessorACL.BusinessLogic.BackendAPI;

public interface IEstateReportingApiClient
{
    Task<Result<MerchantDailyPerformanceSummaryResponse>> GetMerchantDailyPerformanceSummary(String accessToken,
                                                                                              Guid estateId,
                                                                                              MerchantDailyPerformanceSummaryRequest request,
                                                                                              CancellationToken cancellationToken);

    Task<Result<MerchantTransactionMixSummaryResponse>> GetMerchantTransactionMixSummary(String accessToken,
                                                                                         Guid estateId,
                                                                                         MerchantTransactionMixSummaryRequest request,
                                                                                         CancellationToken cancellationToken);
}
