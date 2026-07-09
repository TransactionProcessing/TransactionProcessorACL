using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Shared.Authorisation;
using TransactionProcessorACL.Handlers;

namespace TransactionProcessorACL.Endpoints;

public static class ReportingEndpoints
{
    private const string BaseRoute = "/api/reporting";

    public static IEndpointRouteBuilder MapReportingEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup(BaseRoute).RequireAuthorization().RequireAuthorization(AuthorizationExtensions.PolicyNames.PasswordTokenOnlyPolicy);

        // POST /api/reporting/dailymerchantperformancesummary
        group.MapPost("dailymerchantperformancesummary", ReportingHandlers.GetMerchantDailyPerformanceSummary);
        // POST /api/reporting/transactionmixsummary
        group.MapPost("transactionmixsummary", ReportingHandlers.GetMerchantTransactionMixSummary);
        // POST /api/reporting/recentactivityreceiptsearch
        group.MapPost("recentactivityreceiptsearch", ReportingHandlers.GetRecentActivityReceiptSearch);

        return app;
    }
}
