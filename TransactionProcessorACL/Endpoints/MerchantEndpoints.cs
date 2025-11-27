using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Shared.Authorisation;
using Shared.Logger;
using TransactionProcessorACL.Handlers;

namespace TransactionProcessorACL.Endpoints
{
    public static class MerchantEndpoints
    {
        private const string BaseRoute = "/api/merchants";

        public static IEndpointRouteBuilder MapMerchantEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup(BaseRoute)
                           .RequireAuthorization()
                           .RequireAuthorization(AuthorizationExtensions.PolicyNames.PasswordTokenOnlyPolicy);
            
            group.MapGet("", MerchantHandlers.GetMerchant).WithName("GetMerchant");

            group.MapGet("contracts",MerchantHandlers.GetMerchantContracts).WithName("GetMerchantContracts");

            return app;
        }
    }
}