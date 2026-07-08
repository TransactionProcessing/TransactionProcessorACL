using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SimpleResults;
using TransactionProcessorACL.BusinessLogic.Requests;
using TransactionProcessorACL.BusinessLogic.Services;
using TransactionProcessorACL.Models;
using TransactionProcessorACL.DataTransferObjects.Requests;

namespace TransactionProcessorACL.BusinessLogic.RequestHandlers;

public class ReportingRequestHandler :
    IRequestHandler<ReportingQueries.GetMerchantDailyPerformanceSummaryQuery, Result<MerchantDailyPerformanceSummaryResponse>>,
    IRequestHandler<ReportingQueries.GetMerchantTransactionMixSummaryQuery, Result<MerchantTransactionMixSummaryResponse>>,
    IRequestHandler<ReportingQueries.GetRecentActivityReceiptSearchQuery, Result<RecentActivityReceiptSearchResponse>>
{
    private readonly ITransactionProcessorACLApplicationService ApplicationService;

    public ReportingRequestHandler(ITransactionProcessorACLApplicationService applicationService)
    {
        this.ApplicationService = applicationService;
    }

    public async Task<Result<MerchantDailyPerformanceSummaryResponse>> Handle(ReportingQueries.GetMerchantDailyPerformanceSummaryQuery request,
                                                                             CancellationToken cancellationToken)
    {
        return await this.ApplicationService.GetMerchantDailyPerformanceSummary(request.EstateId, request.Request, cancellationToken);
    }

    public async Task<Result<MerchantTransactionMixSummaryResponse>> Handle(ReportingQueries.GetMerchantTransactionMixSummaryQuery request,
                                                                           CancellationToken cancellationToken)
    {
        return await this.ApplicationService.GetMerchantTransactionMixSummary(request.EstateId, request.Request, cancellationToken);
    }

    public async Task<Result<RecentActivityReceiptSearchResponse>> Handle(ReportingQueries.GetRecentActivityReceiptSearchQuery request,
                                                                          CancellationToken cancellationToken)
    {
        return await this.ApplicationService.GetRecentActivityReceiptSearch(request.EstateId, request.Request, cancellationToken);
    }
}
