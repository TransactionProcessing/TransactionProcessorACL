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

            return ResponseFactory.FromResult(result, list => {
                List<DataTransferObjects.Responses.ContractResponse> responses = new();
                foreach (ContractResponse contractModel in result.Data) {
                    DataTransferObjects.Responses.ContractResponse contractResponse = new() {
                        ContractId = contractModel.ContractId,
                        ContractReportingId = contractModel.ContractReportingId,
                        Description = contractModel.Description,
                        EstateId = contractModel.EstateId,
                        EstateReportingId = contractModel.EstateReportingId,
                        OperatorId = contractModel.OperatorId,
                        OperatorName = contractModel.OperatorName,
                        Products = new()
                    };

                    foreach (ContractProduct contractModelProduct in contractModel.Products) {
                        DataTransferObjects.Responses.ContractProduct productResponse = new() {
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
                            TransactionFees = new()
                        };

                        foreach (ContractProductTransactionFee contractProductTransactionFeeModel in contractModelProduct.TransactionFees) {
                            DataTransferObjects.Responses.ContractProductTransactionFee transactionFeeModel = new() {
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
                            productResponse.TransactionFees.Add(transactionFeeModel);
                        }

                        contractResponse.Products.Add(productResponse);
                    }

                    responses.Add(contractResponse);
                }

                return responses;
            });
        }

        public static async Task<IResult> GetMerchant(IMediator mediator, ClaimsPrincipal user, string applicationVersion, CancellationToken cancellationToken)
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

    public class PasswordTokenRequirement : IAuthorizationRequirement { }

    public class PasswordTokenHandler : AuthorizationHandler<PasswordTokenRequirement>
    {
        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            PasswordTokenRequirement requirement)
        {
            var hasClaim = context.User.Claims
                .Any(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier");

            if (hasClaim)
                context.Succeed(requirement);

            return Task.CompletedTask;
        }
    }
}
