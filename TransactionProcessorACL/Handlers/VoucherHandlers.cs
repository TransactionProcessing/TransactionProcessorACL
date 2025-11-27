using System;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Shared.General;
using Shared.Results.Web;
using SimpleResults;
using TransactionProcessorACL.BusinessLogic.Requests;
using TransactionProcessorACL.Factories;

namespace TransactionProcessorACL.Handlers
{
    /// <summary>
    /// Static handlers for voucher operations, suitable for minimal API endpoints.
    /// Returns IResult so endpoints can proxy the result directly.
    /// </summary>
    public static class VoucherHandlers
    {
        public static async Task<IResult> GetVoucherAsync(IMediator mediator,
                                                          IModelFactory modelFactory,
                                                          ClaimsPrincipal user,
                                                          string voucherCode,
                                                          string applicationVersion,
                                                          CancellationToken cancellationToken)
        {

            // NOTE: original controller had the version check commented out — preserving that behaviour.
            Result<Claim> estateClaim = ClaimsHelper.GetUserClaim(user, "estateId");
            if (estateClaim.IsFailed)
                return ResponseFactory.FromResult(Result.Failure("No Claim found for Estate Id"));

            Result<Claim> contractClaim = ClaimsHelper.GetUserClaim(user, "contractId");
            if (contractClaim.IsFailed)
                return ResponseFactory.FromResult(Result.Failure("No Claim found for Contract Id"));

            Guid estateId = Guid.Parse(estateClaim.Data.Value);
            Guid contractId = Guid.Parse(contractClaim.Data.Value);

            var query = new VoucherQueries.GetVoucherQuery(estateId, contractId, voucherCode);
            var result = await mediator.Send(query, cancellationToken);

            return ResponseFactory.FromResult(result, modelFactory.ConvertFrom);
        }

        public static async Task<IResult> RedeemVoucherAsync(IMediator mediator,
                                                             IModelFactory modelFactory,
                                                             ClaimsPrincipal user,
                                                             string voucherCode,
                                                             string applicationVersion,
                                                             CancellationToken cancellationToken)
        {
            // NOTE: original controller had authentication/version checks commented out for redeem — preserve behaviour.
            Result<Claim> estateClaim = ClaimsHelper.GetUserClaim(user, "estateId");
            if (estateClaim.IsFailed)
                return ResponseFactory.FromResult(Result.Failure("No Claim found for Estate Id"));

            Result<Claim> contractClaim = ClaimsHelper.GetUserClaim(user, "contractId");
            if (contractClaim.IsFailed)
                return ResponseFactory.FromResult(Result.Failure("No Claim found for Contract Id"));

            Guid estateId = Guid.Parse(estateClaim.Data.Value);
            Guid contractId = Guid.Parse(contractClaim.Data.Value);

            var command = new VoucherCommands.RedeemVoucherCommand(estateId, contractId, voucherCode);
            var result = await mediator.Send(command, cancellationToken);

            return ResponseFactory.FromResult(result, modelFactory.ConvertFrom);
        }
    }
}