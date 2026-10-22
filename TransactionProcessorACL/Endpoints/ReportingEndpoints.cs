using Microsoft.AspNetCore.Http;
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
        var group = app.MapGroup(BaseRoute)
            .WithTags("Reporting")
            .RequireAuthorization()
            .RequireAuthorization(AuthorizationExtensions.PolicyNames.PasswordTokenOnlyPolicy);

        // POST /api/reporting/dailymerchantperformancesummary
        group.MapPost("dailymerchantperformancesummary", ReportingHandlers.GetMerchantDailyPerformanceSummary)
            .WithName("GetMerchantDailyPerformanceSummary")
            .WithSummary("Get daily merchant performance")
            .WithDescription("Returns performance metrics and transaction drill-down rows for the requested merchant and date range. The estate is taken from the authenticated token. Provide applicationVersion as a query parameter for minimum-version validation; the middleware also reads application_version from the JSON body.")
            .DocumentQueryParameter("applicationVersion", "Required client version. Use a version meeting the configured minimum; requests below it return HTTP 505.", exampleValue: "1.0.5")
            .Accepts<TransactionProcessorACL.DataTransferObjects.Requests.MerchantDailyPerformanceSummaryRequest>("application/json")
            .DocumentJsonRequestExample("""{"merchantReportingId":1024,"startDate":"2026-10-01T00:00:00Z","endDate":"2026-10-21T23:59:59Z","applicationVersion":"1.0.5"}""")
            .Produces<TransactionProcessorACL.DataTransferObjects.Responses.MerchantDailyPerformanceSummaryResponse>(StatusCodes.Status200OK)
            .DocumentJsonResponseExample(StatusCodes.Status200OK, """{"metrics":[{"title":"Transaction count","value":42,"description":"Total completed transactions","category":1},{"title":"Transaction value","value":1250.75,"description":"Total value of completed transactions","category":2}],"drillDownTransactions":[{"reference":"SALE-20261021-0001","product":"Mobile Top-up","status":"Completed","amount":25.0,"transactionDateTime":"2026-10-21T14:30:00Z"}]}""")
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(505);
        // POST /api/reporting/transactionmixsummary
        group.MapPost("transactionmixsummary", ReportingHandlers.GetMerchantTransactionMixSummary)
            .WithName("GetMerchantTransactionMixSummary")
            .WithSummary("Get a transaction mix summary")
            .WithDescription("Summarizes transactions for a merchant and date range. Choose a breakdown (transaction type, product, operator, or status) and a measure (count or value); TopN limits the ranked items. The estate is taken from the authenticated token. Provide applicationVersion as a query parameter for minimum-version validation; the middleware also reads application_version from the JSON body.")
            .DocumentQueryParameter("applicationVersion", "Required client version. Use a version meeting the configured minimum; requests below it return HTTP 505.", exampleValue: "1.0.5")
            .Accepts<TransactionProcessorACL.DataTransferObjects.Requests.MerchantTransactionMixSummaryRequest>("application/json")
            .DocumentJsonRequestExample("""{"merchantReportingId":1024,"startDate":"2026-10-01T00:00:00Z","endDate":"2026-10-21T23:59:59Z","breakdown":2,"measure":2,"applicationVersion":"1.0.5","topN":5}""")
            .Produces<TransactionProcessorACL.DataTransferObjects.Responses.MerchantTransactionMixSummaryResponse>(StatusCodes.Status200OK)
            .DocumentJsonResponseExample(StatusCodes.Status200OK, """{"fromDate":"2026-10-01T00:00:00Z","toDate":"2026-10-21T23:59:59Z","breakdown":2,"measure":2,"totalCount":42,"totalValue":1250.75,"items":[{"key":"55555555-5555-5555-5555-555555555555","label":"Mobile Top-up","count":30,"value":900.0}],"drillDownTransactions":[{"reference":"SALE-20261021-0001","transactionType":"Sale","product":"Mobile Top-up","operator":"Operator One","status":"Completed","amount":25.0,"transactionDateTime":"2026-10-21T14:30:00Z"}]}""")
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(505);
        // POST /api/reporting/recentactivityreceiptsearch
        group.MapPost("recentactivityreceiptsearch", ReportingHandlers.GetRecentActivityReceiptSearch)
            .WithName("GetRecentActivityReceiptSearch")
            .WithSummary("Search recent merchant activity and receipts")
            .WithDescription("Searches a merchant's transactions for a report date, with optional search text and pagination. The estate is taken from the authenticated token. Provide applicationVersion as a query parameter for minimum-version validation; the middleware also reads application_version from the JSON body.")
            .DocumentQueryParameter("applicationVersion", "Required client version. Use a version meeting the configured minimum; requests below it return HTTP 505.", exampleValue: "1.0.5")
            .Accepts<TransactionProcessorACL.DataTransferObjects.Requests.RecentActivityReceiptSearchRequest>("application/json")
            .DocumentJsonRequestExample("""{"applicationVersion":"1.0.5","merchantReportingId":1024,"reportDate":"2026-10-21T00:00:00Z","searchText":"coffee","pageNumber":1,"pageSize":10}""")
            .Produces<TransactionProcessorACL.DataTransferObjects.Responses.RecentActivityReceiptSearchResponse>(StatusCodes.Status200OK)
            .DocumentJsonResponseExample(StatusCodes.Status200OK, """{"reportDate":"2026-10-21T00:00:00Z","pageNumber":1,"pageSize":10,"totalCount":1,"items":[{"reference":"SALE-20261021-0001","transactionType":"Sale","product":"Coffee","operator":"Operator One","status":"Completed","amount":4.5,"transactionDateTime":"2026-10-21T14:30:00Z","receiptReference":"RCPT-20261021-0001"}]}""")
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(505);

        return app;
    }
}
