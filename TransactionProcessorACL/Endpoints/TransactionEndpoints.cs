using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Http;
using TransactionProcessorACL.Factories;
using System.Security.Claims;
using Shared.Authorisation;
using TransactionProcessorACL.DataTransferObjects;
using TransactionProcessorACL.Handlers;

namespace TransactionProcessorACL.Endpoints
{
    public static class TransactionEndpoints
    {
        private const string SaleBaseRoute = "/api/saletransactions";
        private const string LogonBaseRoute = "/api/logontransactions";
        private const string ReconciliationBaseRoute = "/api/reconciliationtransactions";

        public static IEndpointRouteBuilder MapTransactionEndpoints(this IEndpointRouteBuilder app)
        {
            var saleGroup = app.MapGroup(SaleBaseRoute).RequireAuthorization().RequireAuthorization(AuthorizationExtensions.PolicyNames.PasswordTokenOnlyPolicy);

            // POST /api/saletransactions
            saleGroup.MapPost("", TransactionHandlers.PerformSaleTransaction);

            var logonGroup = app.MapGroup(LogonBaseRoute).RequireAuthorization().RequireAuthorization(AuthorizationExtensions.PolicyNames.PasswordTokenOnlyPolicy);

            // POST /api/logontransactions
            logonGroup.MapPost("", TransactionHandlers.PerformLogonTransaction);

            var reconciliationGroup = app.MapGroup(ReconciliationBaseRoute).RequireAuthorization().RequireAuthorization(AuthorizationExtensions.PolicyNames.PasswordTokenOnlyPolicy);

            // POST /api/reconciliationtransactions
            reconciliationGroup.MapPost("", TransactionHandlers.PerformReconciliationTransaction);

            return app;
        }
    }
}