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
        public static async Task<IResult> GetMerchantContracts(IMediator mediator, ClaimsPrincipal user, string applicationVersion, CancellationToken cancellationToken)
        {
            Logger.LogInformation($"Application version {applicationVersion}");

            Result<(Guid estateId, Guid merchantId)> claimsResult = Helpers.GetRequiredClaims(user);
            if (claimsResult.IsFailed)
                return ResponseFactory.FromResult(Result.Forbidden());

            MerchantQueries.GetMerchantContractsQuery query = new(claimsResult.Data.estateId, claimsResult.Data.merchantId);
            Result<List<ContractResponse>> result = await mediator.Send(query, cancellationToken);

            return ResponseFactory.FromResult(result, list => MapContractResponses(list));
        }

        private static List<DataTransferObjects.Responses.ContractResponse> MapContractResponses(List<ContractResponse> contractResponses) =>
            contractResponses.Select(MapContractResponse).ToList();

        private static DataTransferObjects.Responses.ContractResponse MapContractResponse(ContractResponse contractModel) =>
            new() {
                ContractId = contractModel.ContractId,
                ContractReportingId = contractModel.ContractReportingId,
                Description = contractModel.Description,
                EstateId = contractModel.EstateId,
                EstateReportingId = contractModel.EstateReportingId,
                OperatorId = contractModel.OperatorId,
                OperatorName = contractModel.OperatorName,
                Products = contractModel.Products.Select(MapContractProductResponse).ToList()
            };

        private static DataTransferObjects.Responses.ContractProduct MapContractProductResponse(ContractProduct contractModelProduct) =>
            new() {
                Value = contractModelProduct.Value,
                DisplayText = contractModelProduct.DisplayText,
                Name = contractModelProduct.Name,
                ProductId = contractModelProduct.ProductId,
                ProductReportingId = contractModelProduct.ProductReportingId,
                ProductType = contractModelProduct.ProductType switch {
                    ProductType.BillPayment => DataTransferObjects.Responses.ProductType.BillPayment,
                    ProductType.MobileTopup => DataTransferObjects.Responses.ProductType.MobileTopup,
                    ProductType.Voucher => DataTransferObjects.Responses.ProductType.Voucher,
                    _ => DataTransferObjects.Responses.ProductType.NotSet
                },
                TransactionFees = contractModelProduct.TransactionFees.Select(MapContractProductTransactionFeeResponse).ToList()
            };

        private static DataTransferObjects.Responses.ContractProductTransactionFee MapContractProductTransactionFeeResponse(ContractProductTransactionFee contractProductTransactionFeeModel) =>
            new() {
                Value = contractProductTransactionFeeModel.Value,
                Description = contractProductTransactionFeeModel.Description,
                CalculationType = contractProductTransactionFeeModel.CalculationType switch {
                    CalculationType.Fixed => DataTransferObjects.Responses.CalculationType.Fixed,
                    _ => DataTransferObjects.Responses.CalculationType.Percentage,
                },
                FeeType = contractProductTransactionFeeModel.FeeType switch {
                    FeeType.Merchant => DataTransferObjects.Responses.FeeType.Merchant,
                    _ => DataTransferObjects.Responses.FeeType.ServiceProvider,
                },
                TransactionFeeId = contractProductTransactionFeeModel.TransactionFeeId,
                TransactionFeeReportingId = contractProductTransactionFeeModel.TransactionFeeReportingId
            };

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

            return ResponseFactory.FromResult(result, modelFactory.ConvertFrom);
        }
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
