using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Shared.General;
using Shared.Results.Web;
using SimpleResults;
using TransactionProcessorACL.BusinessLogic.Requests;
using TransactionProcessorACL.DataTransferObjects.Requests;

namespace TransactionProcessorACL.Handlers;

public static class ReportingHandlers
{
    public static async Task<IResult> GetMerchantDailyPerformanceSummary(ClaimsPrincipal user,
                                                                         MerchantDailyPerformanceSummaryRequest request,
                                                                         IMediator mediator,
                                                                         CancellationToken cancellationToken)
    {
        Result<System.Guid> estateIdResult = Helpers.GetRequiredEstateClaim(user);
        if (estateIdResult.IsFailed)
            return ResponseFactory.FromResult(Result.Forbidden());

        ReportingQueries.GetMerchantDailyPerformanceSummaryQuery query = new(estateIdResult.Data, request);
        Result<TransactionProcessorACL.Models.MerchantDailyPerformanceSummaryResponse> result =
            await mediator.Send(query, cancellationToken);
        return ResponseFactory.FromResult(result, response => response);
    }

    public static async Task<IResult> GetMerchantTransactionMixSummary(ClaimsPrincipal user,
                                                                       MerchantTransactionMixSummaryRequest request,
                                                                       IMediator mediator,
                                                                       CancellationToken cancellationToken)
    {
        Result<System.Guid> estateIdResult = Helpers.GetRequiredEstateClaim(user);
        if (estateIdResult.IsFailed)
            return ResponseFactory.FromResult(Result.Forbidden());

        ReportingQueries.GetMerchantTransactionMixSummaryQuery query = new(estateIdResult.Data, request);
        Result<TransactionProcessorACL.Models.MerchantTransactionMixSummaryResponse> result = await mediator.Send(query, cancellationToken);
        return ResponseFactory.FromResult(result, response => response);
    }

    public static async Task<IResult> GetRecentActivityReceiptSearch(ClaimsPrincipal user,
                                                                     RecentActivityReceiptSearchRequest request,
                                                                     IMediator mediator,
                                                                     CancellationToken cancellationToken)
    {
        Result<System.Guid> estateIdResult = Helpers.GetRequiredEstateClaim(user);
        if (estateIdResult.IsFailed)
            return ResponseFactory.FromResult(Result.Forbidden());

        ReportingQueries.GetRecentActivityReceiptSearchQuery query = new(estateIdResult.Data, request);
        Result<TransactionProcessorACL.Models.RecentActivityReceiptSearchResponse> result =
            await mediator.Send(query, cancellationToken);
        return ResponseFactory.FromResult(result, response => response);
    }
}
