using System.Threading;
using System.Threading.Tasks;
using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using TransactionProcessorACL.Factories;
using Microsoft.AspNetCore.Mvc;
using TransactionProcessorACL.Handlers;

namespace TransactionProcessorACL.Endpoints
{
    public static class VoucherEndpoints
    {
        private const string BaseRoute = "/api/vouchers";

        public static IEndpointRouteBuilder MapVoucherEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup(BaseRoute)
                .WithTags("Vouchers")
                .RequireAuthorization();

            group.MapGet("", VoucherHandlers.GetVoucher)
                .WithName("GetVoucher")
                .WithSummary("Get voucher details")
                .WithDescription("Looks up a voucher by voucherCode for the estate and contract identified by the authenticated token. Provide voucherCode and applicationVersion as query parameters. The application version is checked against the configured minimum.")
                .DocumentQueryParameter("voucherCode", "Voucher code to look up.", exampleValue: "VOUCHER-EXAMPLE-001")
                .DocumentQueryParameter("applicationVersion", "Required client version. Use a version meeting the configured minimum; requests below it return HTTP 505.", exampleValue: "1.0.5")
                .Produces<TransactionProcessorACL.DataTransferObjects.Responses.GetVoucherResponseMessage>(StatusCodes.Status200OK)
                .DocumentJsonResponseExample(StatusCodes.Status200OK, """{"responseCode":"00","responseMessage":"Voucher found","contractId":"33333333-3333-3333-3333-333333333333","estateId":"11111111-1111-1111-1111-111111111111","expiryDate":"2027-10-21T23:59:59Z","value":50.0,"balance":35.0,"voucherCode":"VOUCHER-EXAMPLE-001","voucherId":"aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"}""")
                .Produces(StatusCodes.Status401Unauthorized)
                .Produces(505);

            group.MapPut("", VoucherHandlers.RedeemVoucher)
                .WithName("RedeemVoucher")
                .WithSummary("Redeem a voucher")
                .WithDescription("Redeems the voucher identified by voucherCode for the estate and contract identified by the authenticated token. Provide voucherCode and applicationVersion as query parameters. The application version is checked against the configured minimum.")
                .DocumentQueryParameter("voucherCode", "Voucher code to redeem.", exampleValue: "VOUCHER-EXAMPLE-001")
                .DocumentQueryParameter("applicationVersion", "Required client version. Use a version meeting the configured minimum; requests below it return HTTP 505.", exampleValue: "1.0.5")
                .Produces<TransactionProcessorACL.DataTransferObjects.Responses.RedeemVoucherResponseMessage>(StatusCodes.Status200OK)
                .DocumentJsonResponseExample(StatusCodes.Status200OK, """{"responseCode":"00","responseMessage":"Voucher redeemed","contractId":"33333333-3333-3333-3333-333333333333","estateId":"11111111-1111-1111-1111-111111111111","expiryDate":"2027-10-21T23:59:59Z","balance":0.0,"voucherCode":"VOUCHER-EXAMPLE-001"}""")
                .Produces(StatusCodes.Status401Unauthorized)
                .Produces(505);

            return app;
        }
    }
}
