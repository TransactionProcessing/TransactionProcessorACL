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
                                                               IModelFactory modelFactory,
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

            return ResponseFactory.FromResult(result, modelFactory.ConvertFrom);
        }

        public static async Task<IResult> GetMerchant(IMediator mediator,
                                                      IModelFactory modelFactory,
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

            return ResponseFactory.FromResult(result, MapMerchantResponse);
        }

        private static DataTransferObjects.Responses.MerchantResponse MapMerchantResponse(MerchantResponse merchantResponse) => new() {
            EstateId = merchantResponse.EstateId,
            MerchantId = merchantResponse.MerchantId,
            EstateReportingId = merchantResponse.EstateReportingId,
            MerchantName = merchantResponse.MerchantName,
            MerchantReference = merchantResponse.MerchantReference,
            MerchantReportingId = merchantResponse.MerchantReportingId,
            NextStatementDate = merchantResponse.NextStatementDate,
            SettlementSchedule = merchantResponse.SettlementSchedule switch {
                SettlementSchedule.Weekly => DataTransferObjects.Responses.SettlementSchedule.Weekly,
                SettlementSchedule.Monthly => DataTransferObjects.Responses.SettlementSchedule.Monthly,
                _ => DataTransferObjects.Responses.SettlementSchedule.NotSet
            },
            Addresses = merchantResponse.Addresses.Select(MapAddress).ToList(),
            Contacts = merchantResponse.Contacts.Select(MapContact).ToList(),
            Contracts = merchantResponse.Contracts.Select(MapContract).ToList(),
            Devices = new Dictionary<Guid, string>(merchantResponse.Devices),
            Operators = merchantResponse.Operators.Select(MapOperator).ToList()
        };

        private static DataTransferObjects.Responses.AddressResponse MapAddress(AddressResponse addressResponse) => new() {
            AddressId = addressResponse.AddressId,
            AddressLine1 = addressResponse.AddressLine1,
            AddressLine2 = addressResponse.AddressLine2,
            AddressLine3 = addressResponse.AddressLine3,
            AddressLine4 = addressResponse.AddressLine4,
            Country = addressResponse.Country,
            PostalCode = addressResponse.PostalCode,
            Region = addressResponse.Region,
            Town = addressResponse.Town
        };

        private static DataTransferObjects.Responses.ContactResponse MapContact(ContactResponse contactResponse) => new() {
            ContactId = contactResponse.ContactId,
            ContactName = contactResponse.ContactName,
            ContactPhoneNumber = contactResponse.ContactPhoneNumber,
            ContactEmailAddress = contactResponse.ContactEmailAddress
        };

        private static DataTransferObjects.Responses.MerchantContractResponse MapContract(MerchantContractResponse contractResponse) => new() {
            ContractId = contractResponse.ContractId,
            IsDeleted = contractResponse.IsDeleted,
            ContractProducts = contractResponse.ContractProducts.ToList()
        };

        private static DataTransferObjects.Responses.MerchantOperatorResponse MapOperator(MerchantOperatorResponse operatorResponse) => new() {
            OperatorId = operatorResponse.OperatorId,
            IsDeleted = operatorResponse.IsDeleted,
            MerchantNumber = operatorResponse.MerchantNumber,
            Name = operatorResponse.Name,
            TerminalNumber = operatorResponse.TerminalNumber
        };
    }

    public class ClaimsIdentityRequirement : IAuthorizationRequirement { }

    public class ClaimsIdentityHandler : AuthorizationHandler<ClaimsIdentityRequirement>
    {
        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            ClaimsIdentityRequirement requirement)
        {
            var hasClaim = context.User.Claims
                .Any(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier");

            if (hasClaim)
                context.Succeed(requirement);

            return Task.CompletedTask;
        }
    }
}
