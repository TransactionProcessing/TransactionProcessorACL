using System.Threading;
using System.Threading.Tasks;
using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Http;
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
                           .RequireAuthorization();

            group.MapGet("", VoucherHandlers.GetVoucher);

            group.MapPut("", VoucherHandlers.RedeemVoucher);

            return app;
        }
    }
}