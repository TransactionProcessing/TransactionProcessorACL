using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TransactionProcessing.Shared.Logger;
using TransactionProcessing.UnionResults;
using ApiResponses = TransactionProcessorACL.DataTransferObjects.Responses;
using TransactionProcessorACL.BusinessLogic.Requests;
using TransactionProcessorACL.Factories;
using TransactionProcessorACL.Models;

namespace TransactionProcessorACL.Handlers
{
    public static class MerchantHandlers
    {
        public static async Task<Result<List<ApiResponses.ContractResponse>>> GetMerchantContracts(IMediator mediator,
                                                               ClaimsPrincipal user,
                                                               string applicationVersion,
                                                               CancellationToken cancellationToken)
        {
            Logger.LogInformation("Application version {ApplicationVersion}", applicationVersion);

            Result<(Guid estateId, Guid merchantId)> claimsResult = Helpers.GetRequiredClaims(user);
            if (claimsResult.Value is not Success<(Guid estateId, Guid merchantId)> claims)
                return new Error(ErrorType.Forbidden, "The request is forbidden.");

            MerchantQueries.GetMerchantContractsQuery query = new(claims.Value.estateId, claims.Value.merchantId);
            Result<List<ContractResponse>> result = await mediator.Send(query, cancellationToken);

            return ResponseFactory.FromResult(result, ModelFactory.ConvertFrom);
        }

        public static async Task<Result<ApiResponses.MerchantResponse>> GetMerchant(IMediator mediator,
                                                      ClaimsPrincipal user,
                                                      string applicationVersion,
                                                      CancellationToken cancellationToken)
        {
            Logger.LogInformation("Application version {ApplicationVersion}", applicationVersion);
            Logger.LogWarning("In GetMerchant Handler");

            Result<(Guid estateId, Guid merchantId)> claimsResult = Helpers.GetRequiredClaims(user);
            if (claimsResult.Value is not Success<(Guid estateId, Guid merchantId)> claims)
                return new Error(ErrorType.Forbidden, "The request is forbidden.");

            MerchantQueries.GetMerchantQuery query = new(claims.Value.estateId, claims.Value.merchantId);
            Result<MerchantResponse> result = await mediator.Send(query, cancellationToken);

            return ResponseFactory.FromResult(result, ModelFactory.ConvertFrom);
        }

        public static async Task<Result<ApiResponses.MerchantScheduleResponse>> GetMerchantSchedule(IMediator mediator,
                                                      ClaimsPrincipal user,
                                                      string applicationVersion,
                                                      [FromQuery] Int32 year,
                                                      CancellationToken cancellationToken)
        {
            Logger.LogInformation("Application version {ApplicationVersion}", applicationVersion);
            Logger.LogWarning("In GetMerchantSchedule Handler");

            Result<(Guid estateId, Guid merchantId)> claimsResult = Helpers.GetRequiredClaims(user);
            if (claimsResult.Value is not Success<(Guid estateId, Guid merchantId)> claims)
                return new Error(ErrorType.Forbidden, "The request is forbidden.");

            MerchantQueries.GetMerchantScheduleQuery query = new(claims.Value.estateId, claims.Value.merchantId, year);
            Result<MerchantScheduleResponse> result = await mediator.Send(query, cancellationToken);

            return ResponseFactory.FromResult(result, ModelFactory.ConvertFrom);
        }
    }

}
