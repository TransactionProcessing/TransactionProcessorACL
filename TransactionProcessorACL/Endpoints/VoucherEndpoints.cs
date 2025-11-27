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

            // GET /api/vouchers?voucherCode=...&applicationVersion=...
            group.MapGet("", async (IMediator mediator,
                                     IModelFactory modelFactory,
                                     ClaimsPrincipal user,
                                     [FromQuery] string voucherCode,
                                     [FromQuery] string applicationVersion,
                                     CancellationToken cancellationToken) =>
            {
                return await VoucherHandlers.GetVoucherAsync(mediator, modelFactory, user, voucherCode, applicationVersion, cancellationToken);
            });

            // PUT /api/vouchers?voucherCode=...&applicationVersion=...
            group.MapPut("", async (IMediator mediator,
                                     IModelFactory modelFactory,
                                     ClaimsPrincipal user,
                                     [FromQuery] string voucherCode,
                                     [FromQuery] string applicationVersion,
                                     CancellationToken cancellationToken) =>
            {
                return await VoucherHandlers.RedeemVoucherAsync(mediator, modelFactory, user, voucherCode, applicationVersion, cancellationToken);
            });

            return app;
        }
    }
}