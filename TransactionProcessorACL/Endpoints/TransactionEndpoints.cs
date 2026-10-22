using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Shared.Authorisation;
using TransactionProcessorACL.Handlers;

namespace TransactionProcessorACL.Endpoints
{
    public static class TransactionEndpoints
    {
        private const string SaleBaseRoute = "/api/saletransactions";
        private const string LogonBaseRoute = "/api/logontransactions";
        private const string ReconciliationBaseRoute = "/api/reconciliationtransactions";
        private const string TransactionsBaseRoute = "/api/transactions";

        public static IEndpointRouteBuilder MapTransactionEndpoints(this IEndpointRouteBuilder app)
        {
            var saleGroup = app.MapGroup(SaleBaseRoute)
                .WithTags("Transactions")
                .RequireAuthorization()
                .RequireAuthorization(AuthorizationExtensions.PolicyNames.PasswordTokenOnlyPolicy);

            // POST /api/saletransactions
            saleGroup.MapPost("", TransactionHandlers.PerformSaleTransaction)
                .WithName("ProcessSaleTransaction")
                .WithSummary("Process a sale transaction")
                .WithDescription("Submits a sale for the authenticated merchant. Estate and merchant identifiers are resolved from token claims. The request includes transaction details, contract, operator, product, and optional customer and metadata fields. Provide applicationVersion as a query parameter for minimum-version validation; the middleware also reads application_version from the JSON body.")
                .DocumentQueryParameter("applicationVersion", "Required client version. Use a version meeting the configured minimum; requests below it return HTTP 505.", exampleValue: "1.0.5")
                .Accepts<TransactionProcessorACL.DataTransferObjects.SaleTransactionRequestMessage>("application/json")
                .DocumentJsonRequestExample("""{"estateId":"11111111-1111-1111-1111-111111111111","merchantId":"22222222-2222-2222-2222-222222222222","applicationVersion":"1.0.5","deviceIdentifier":"terminal-01","transactionDateTime":"2026-10-21T14:30:00Z","transactionNumber":"SALE-20261021-0001","additionalRequestMetadata":{"channel":"terminal"},"contractId":"33333333-3333-3333-3333-333333333333","operatorId":"44444444-4444-4444-4444-444444444444","customerEmailAddress":"customer@example.com","productId":"55555555-5555-5555-5555-555555555555"}""")
                .Produces<TransactionProcessorACL.DataTransferObjects.Responses.SaleTransactionResponseMessage>(StatusCodes.Status200OK)
                .DocumentJsonResponseExample(StatusCodes.Status200OK, """{"responseCode":"00","responseMessage":"Approved","additionalResponseMetadata":{"authorizationCode":"AUTH-12345"},"estateId":"11111111-1111-1111-1111-111111111111","merchantId":"22222222-2222-2222-2222-222222222222","requiresApplicationUpdate":false,"transactionId":"77777777-7777-7777-7777-777777777777"}""")
                .Produces(StatusCodes.Status401Unauthorized)
                .Produces(StatusCodes.Status403Forbidden)
                .Produces(505);

            var logonGroup = app.MapGroup(LogonBaseRoute)
                .WithTags("Transactions")
                .RequireAuthorization()
                .RequireAuthorization(AuthorizationExtensions.PolicyNames.PasswordTokenOnlyPolicy);

            // POST /api/logontransactions
            logonGroup.MapPost("", TransactionHandlers.PerformLogonTransaction)
                .WithName("ProcessLogonTransaction")
                .WithSummary("Process a logon transaction")
                .WithDescription("Submits a logon transaction for the authenticated merchant. Estate and merchant identifiers are resolved from token claims. The request includes transaction date, transaction number, device identifier, application version, and optional metadata. Provide applicationVersion as a query parameter for minimum-version validation; the middleware also reads application_version from the JSON body.")
                .DocumentQueryParameter("applicationVersion", "Required client version. Use a version meeting the configured minimum; requests below it return HTTP 505.", exampleValue: "1.0.5")
                .Accepts<TransactionProcessorACL.DataTransferObjects.LogonTransactionRequestMessage>("application/json")
                .DocumentJsonRequestExample("""{"estateId":"11111111-1111-1111-1111-111111111111","merchantId":"22222222-2222-2222-2222-222222222222","applicationVersion":"1.0.5","deviceIdentifier":"terminal-01","transactionDateTime":"2026-10-21T08:00:00Z","transactionNumber":"LOGON-20261021-0001","additionalRequestMetadata":{"terminalModel":"countertop"}}""")
                .Produces<TransactionProcessorACL.DataTransferObjects.Responses.LogonTransactionResponseMessage>(StatusCodes.Status200OK)
                .DocumentJsonResponseExample(StatusCodes.Status200OK, """{"responseCode":"00","responseMessage":"Logon accepted","additionalResponseMetadata":{},"estateId":"11111111-1111-1111-1111-111111111111","merchantId":"22222222-2222-2222-2222-222222222222","requiresApplicationUpdate":false,"transactionId":"88888888-8888-8888-8888-888888888888"}""")
                .Produces(StatusCodes.Status401Unauthorized)
                .Produces(StatusCodes.Status403Forbidden)
                .Produces(505);

            var reconciliationGroup = app.MapGroup(ReconciliationBaseRoute)
                .WithTags("Transactions")
                .RequireAuthorization()
                .RequireAuthorization(AuthorizationExtensions.PolicyNames.PasswordTokenOnlyPolicy);

            // POST /api/reconciliationtransactions
            reconciliationGroup.MapPost("", TransactionHandlers.PerformReconciliationTransaction)
                .WithName("ProcessReconciliation")
                .WithSummary("Submit transaction reconciliation")
                .WithDescription("Submits reconciliation totals for the authenticated merchant, including total transaction count, transaction value, and optional totals by operator and contract. Estate and merchant identifiers are resolved from token claims. Provide applicationVersion as a query parameter for minimum-version validation; the middleware also reads application_version from the JSON body.")
                .DocumentQueryParameter("applicationVersion", "Required client version. Use a version meeting the configured minimum; requests below it return HTTP 505.", exampleValue: "1.0.5")
                .Accepts<TransactionProcessorACL.DataTransferObjects.ReconciliationRequestMessage>("application/json")
                .DocumentJsonRequestExample("""{"estateId":"11111111-1111-1111-1111-111111111111","merchantId":"22222222-2222-2222-2222-222222222222","applicationVersion":"1.0.5","deviceIdentifier":"terminal-01","transactionDateTime":"2026-10-21T23:59:59Z","transactionNumber":"RECON-20261021-0001","additionalRequestMetadata":{"batch":"daily"},"operatorTotals":[{"contractId":"33333333-3333-3333-3333-333333333333","operatorId":"44444444-4444-4444-4444-444444444444","transactionCount":42,"transactionValue":1250.75}],"transactionCount":42,"transactionValue":1250.75}""")
                .Produces<TransactionProcessorACL.DataTransferObjects.Responses.ReconciliationResponseMessage>(StatusCodes.Status200OK)
                .DocumentJsonResponseExample(StatusCodes.Status200OK, """{"responseCode":"00","responseMessage":"Reconciliation accepted","additionalResponseMetadata":{},"estateId":"11111111-1111-1111-1111-111111111111","merchantId":"22222222-2222-2222-2222-222222222222","requiresApplicationUpdate":false,"transactionId":"99999999-9999-9999-9999-999999999999"}""")
                .Produces(StatusCodes.Status401Unauthorized)
                .Produces(StatusCodes.Status403Forbidden)
                .Produces(505);

            var transactionGroup = app.MapGroup(TransactionsBaseRoute)
                .WithTags("Transactions")
                .RequireAuthorization()
                .RequireAuthorization(AuthorizationExtensions.PolicyNames.PasswordTokenOnlyPolicy);

            // POST /api/transactions/resendreceipt
            transactionGroup.MapPost("resendreceipt", TransactionHandlers.ResendReceipt)
                .WithName("ResendReceipt")
                .WithSummary("Resend a transaction receipt")
                .WithDescription("Sends a receipt to the requested email address for a transaction or receipt reference belonging to the authenticated merchant. Estate and merchant identifiers are resolved from token claims. The reference and recipientEmailAddress fields are required; the email address must be valid. Provide applicationVersion as a query parameter for minimum-version validation.")
                .DocumentQueryParameter("applicationVersion", "Required client version. Use a version meeting the configured minimum; requests below it return HTTP 505.", exampleValue: "1.0.5")
                .Accepts<TransactionProcessorACL.DataTransferObjects.Requests.ResendReceiptRequestMessage>("application/json")
                .DocumentJsonRequestExample("""{"reference":"SALE-20261021-0001","recipientEmailAddress":"customer@example.com"}""")
                .Produces<TransactionProcessorACL.DataTransferObjects.Responses.ResendReceiptResponse>(StatusCodes.Status200OK)
                .DocumentJsonResponseExample(StatusCodes.Status200OK, """{"success":true,"message":"Receipt sent","reference":"SALE-20261021-0001","receiptReference":"RCPT-20261021-0001","transactionReference":"SALE-20261021-0001"}""")
                .Produces<TransactionProcessorACL.DataTransferObjects.Responses.ResendReceiptResponse>(StatusCodes.Status400BadRequest)
                .DocumentJsonResponseExample(StatusCodes.Status400BadRequest, """{"success":false,"message":"Recipient email address is not valid.","reference":"SALE-20261021-0001","receiptReference":"","transactionReference":""}""")
                .Produces<TransactionProcessorACL.DataTransferObjects.Responses.ResendReceiptResponse>(StatusCodes.Status404NotFound)
                .DocumentJsonResponseExample(StatusCodes.Status404NotFound, """{"success":false,"message":"The transaction or receipt reference was not found.","reference":"SALE-20261021-0001","receiptReference":"","transactionReference":""}""")
                .Produces(StatusCodes.Status401Unauthorized)
                .Produces(StatusCodes.Status403Forbidden)
                .Produces(505);

            return app;
        }
    }
}
