using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Shared.Authorisation;
using TransactionProcessorACL.Handlers;

namespace TransactionProcessorACL.Endpoints
{
    public static class MerchantEndpoints
    {
        private const string BaseRoute = "/api/merchants";

        public static IEndpointRouteBuilder MapMerchantEndpoints(this IEndpointRouteBuilder app) {
            var group = app.MapGroup(BaseRoute)
                .WithTags("Merchants")
                .RequireAuthorization()
                .RequireAuthorization(AuthorizationExtensions.PolicyNames.PasswordTokenOnlyPolicy);

            group.MapGet("", MerchantHandlers.GetMerchant)
                .WithName("GetMerchant")
                .WithSummary("Get the authenticated merchant")
                .WithDescription("Returns merchant details, including contacts, addresses, devices, operators, contracts, opening hours, and settlement information. The estate and merchant are resolved from the authenticated token. Supply the client applicationVersion as a query parameter; it is checked against the configured minimum supported version.")
                .DocumentQueryParameter("applicationVersion", "Required client version. Use a version meeting the configured minimum; requests below it return HTTP 505.", exampleValue: "1.0.5")
                .Produces<TransactionProcessorACL.DataTransferObjects.Responses.MerchantResponse>(StatusCodes.Status200OK)
                .DocumentJsonResponseExample(StatusCodes.Status200OK, """{"addresses":[{"addressId":"aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa","addressLine1":"10 High Street","addressLine2":"Unit 2","addressLine3":"","addressLine4":"","country":"United Kingdom","postalCode":"AB1 2CD","region":"Example County","town":"Exampleton"}],"contacts":[{"contactEmailAddress":"ops@example.com","contactId":"bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb","contactName":"Operations","contactPhoneNumber":"+441234567890"}],"devices":{"cccccccc-cccc-cccc-cccc-cccccccccccc":"Main terminal"},"estateId":"11111111-1111-1111-1111-111111111111","estateReportingId":100,"merchantId":"22222222-2222-2222-2222-222222222222","merchantReportingId":1024,"merchantName":"Example Merchant","merchantReference":"MERCHANT-1024","nextStatementDate":"2026-11-01T00:00:00Z","operators":[{"name":"Operator One","operatorId":"44444444-4444-4444-4444-444444444444","merchantNumber":"M-1024","terminalNumber":"T-01","isDeleted":false}],"settlementSchedule":2,"contracts":[{"contractId":"33333333-3333-3333-3333-333333333333","isDeleted":false,"contractProducts":["55555555-5555-5555-5555-555555555555"]}],"openingHours":{"Monday":{"opening":"09:00","closing":"17:00"}}}""")
                .Produces(StatusCodes.Status401Unauthorized)
                .Produces(StatusCodes.Status403Forbidden)
                .Produces(505);

            group.MapGet("contracts", MerchantHandlers.GetMerchantContracts)
                .WithName("GetMerchantContracts")
                .WithSummary("List contracts for the authenticated merchant")
                .WithDescription("Returns the contracts available to the authenticated merchant, including products and transaction fees. The estate and merchant are resolved from token claims. Supply the client applicationVersion as a query parameter.")
                .DocumentQueryParameter("applicationVersion", "Required client version. Use a version meeting the configured minimum; requests below it return HTTP 505.", exampleValue: "1.0.5")
                .Produces<List<TransactionProcessorACL.DataTransferObjects.Responses.ContractResponse>>(StatusCodes.Status200OK)
                .DocumentJsonResponseExample(StatusCodes.Status200OK, """[{"contractId":"33333333-3333-3333-3333-333333333333","contractReportingId":3001,"description":"Standard retail contract","estateId":"11111111-1111-1111-1111-111111111111","estateReportingId":100,"operatorId":"44444444-4444-4444-4444-444444444444","operatorName":"Example Operator","products":[{"displayText":"Mobile top-up","name":"Mobile Top-up","productId":"55555555-5555-5555-5555-555555555555","productReportingId":5001,"transactionFees":[{"calculationType":0,"feeType":0,"description":"Merchant fee","transactionFeeId":"66666666-6666-6666-6666-666666666666","transactionFeeReportingId":6001,"value":0.25}],"value":10.0,"productType":1}]}]""")
                .Produces(StatusCodes.Status401Unauthorized)
                .Produces(StatusCodes.Status403Forbidden)
                .Produces(505);

            group.MapGet("schedules", MerchantHandlers.GetMerchantSchedule)
                .WithName("GetMerchantSchedule")
                .WithSummary("Get a merchant's trading schedule")
                .WithDescription("Returns the merchant's schedule for the requested calendar year, including the days on which the merchant is closed. The estate and merchant are resolved from token claims. Provide year and applicationVersion as query parameters.")
                .DocumentQueryParameter("year", "Calendar year for which to return the merchant schedule.", Microsoft.OpenApi.JsonSchemaType.Integer, 2026)
                .DocumentQueryParameter("applicationVersion", "Required client version. Use a version meeting the configured minimum; requests below it return HTTP 505.", exampleValue: "1.0.5")
                .Produces<TransactionProcessorACL.DataTransferObjects.Responses.MerchantScheduleResponse>(StatusCodes.Status200OK)
                .DocumentJsonResponseExample(StatusCodes.Status200OK, """{"year":2026,"months":[{"month":1,"closedDays":[1,2]},{"month":2,"closedDays":[15]}]}""")
                .Produces(StatusCodes.Status401Unauthorized)
                .Produces(StatusCodes.Status403Forbidden)
                .Produces(505);

            return app;
        }
    }
}
