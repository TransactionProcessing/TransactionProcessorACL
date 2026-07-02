using System;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Shared.General;
using Shared.Results.Web;
using SimpleResults;
using TransactionProcessorACL.DataTransferObjects.Requests;
using TransactionProcessorACL.Models;

namespace TransactionProcessorACL.Handlers;

public static class ReportingHandlers
{
    public static async Task<IResult> GetMerchantDailyPerformanceSummary(ClaimsPrincipal user,
                                                                         MerchantDailyPerformanceSummaryRequest request,
                                                                         TransactionProcessorACL.BusinessLogic.Services.ITransactionProcessorACLApplicationService applicationService,
                                                                         CancellationToken cancellationToken)
    {
        Result<Guid> estateIdResult = Helpers.GetRequiredEstateClaim(user);
        if (estateIdResult.IsFailed)
            return ResponseFactory.FromResult(Result.Forbidden());

        Result<MerchantDailyPerformanceSummaryResponse> result = await applicationService.GetMerchantDailyPerformanceSummary(estateIdResult.Data, request, cancellationToken);
        return ResponseFactory.FromResult(result, response => response);
    }
}
