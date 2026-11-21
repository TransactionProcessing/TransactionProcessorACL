using System;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using TransactionProcessing.Shared.General;
using TransactionProcessing.UnionResults;
using ApiResponses = TransactionProcessorACL.DataTransferObjects.Responses;
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
        public static async Task<Result<ApiResponses.GetVoucherResponseMessage>> GetVoucher(IMediator mediator,
                                                          ClaimsPrincipal user,
                                                          string voucherCode,
                                                          string applicationVersion,
                                                          CancellationToken cancellationToken)
        {

            // NOTE: original controller had the version check commented out � preserving that behaviour.
            Result<Claim> estateClaim = ClaimsHelper.GetUserClaim(user, "estateId");
            if (estateClaim.Value is not Success<Claim> estate)
                return new Error(ErrorType.Failure, "No Claim found for Estate Id");

            Result<Claim> contractClaim = ClaimsHelper.GetUserClaim(user, "contractId");
            if (contractClaim.Value is not Success<Claim> contract)
                return new Error(ErrorType.Failure, "No Claim found for Contract Id");

            Guid estateId = Guid.Parse(estate.Value.Value);
            Guid contractId = Guid.Parse(contract.Value.Value);

            var query = new VoucherQueries.GetVoucherQuery(estateId, contractId, voucherCode);
            var result = await mediator.Send(query, cancellationToken);

            return ResponseFactory.FromResult(result, ModelFactory.ConvertFrom);
        }

        public static async Task<Result<ApiResponses.RedeemVoucherResponseMessage>> RedeemVoucher(IMediator mediator,
                                                             ClaimsPrincipal user,
                                                             string voucherCode,
                                                             string applicationVersion,
                                                             CancellationToken cancellationToken)
        {
            // NOTE: original controller had authentication/version checks commented out for redeem � preserve behaviour.
            Result<Claim> estateClaim = ClaimsHelper.GetUserClaim(user, "estateId");
            if (estateClaim.Value is not Success<Claim> estate)
                return new Error(ErrorType.Failure, "No Claim found for Estate Id");

            Result<Claim> contractClaim = ClaimsHelper.GetUserClaim(user, "contractId");
            if (contractClaim.Value is not Success<Claim> contract)
                return new Error(ErrorType.Failure, "No Claim found for Contract Id");

            Guid estateId = Guid.Parse(estate.Value.Value);
            Guid contractId = Guid.Parse(contract.Value.Value);

            var command = new VoucherCommands.RedeemVoucherCommand(estateId, contractId, voucherCode);
            var result = await mediator.Send(command, cancellationToken);

            return ResponseFactory.FromResult(result, ModelFactory.ConvertFrom);
        }
    }
}