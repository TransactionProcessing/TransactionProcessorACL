using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Shared.Logger;
using Shared.Results.Web;
using SimpleResults;
using TransactionProcessorACL.BusinessLogic.Requests;
using TransactionProcessorACL.Factories;
using TransactionProcessorACL.Models;

namespace TransactionProcessorACL.Handlers
{
    public static class MerchantHandlers
    {
        public static async Task<IResult> GetMerchantContracts(IMediator mediator,
                                                               ClaimsPrincipal user,
                                                               string applicationVersion,
                                                               CancellationToken cancellationToken)
        {
            Logger.LogInformation($"Application version {applicationVersion}");

            Result<(Guid estateId, Guid merchantId)> claimsResult = Helpers.GetRequiredClaims(user);
            if (claimsResult.IsFailed)
                return ResponseFactory.FromResult(Result.Forbidden());

            MerchantQueries.GetMerchantContractsQuery query = new(claimsResult.Data.estateId, claimsResult.Data.merchantId);
            Result<List<ContractResponse>> result = await mediator.Send(query, cancellationToken);

            return ResponseFactory.FromResult(result, ModelFactory.ConvertFrom);
        }

        public static async Task<IResult> GetMerchant(IMediator mediator,
                                                      ClaimsPrincipal user,
                                                      string applicationVersion,
                                                      CancellationToken cancellationToken)
        {
            Logger.LogWarning("In GetMerchant Handler");

            Result<(Guid estateId, Guid merchantId)> claimsResult = Helpers.GetRequiredClaims(user);
            if (claimsResult.IsFailed)
                return ResponseFactory.FromResult(Result.Forbidden());

            MerchantQueries.GetMerchantQuery query = new(claimsResult.Data.estateId, claimsResult.Data.merchantId);
            Result<MerchantResponse> result = await mediator.Send(query, cancellationToken);

            return ResponseFactory.FromResult(result, ModelFactory.ConvertFrom);
        }
    }

}
