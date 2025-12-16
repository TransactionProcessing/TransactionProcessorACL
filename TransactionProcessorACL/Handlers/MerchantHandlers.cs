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

            return ResponseFactory.FromResult(result, response => {

                DataTransferObjects.Responses.MerchantResponse merchantResponse = new() {
                    EstateId = result.Data.EstateId,
                    MerchantId = result.Data.MerchantId,
                    EstateReportingId = result.Data.EstateReportingId,
                    MerchantName = result.Data.MerchantName,
                    MerchantReference = result.Data.MerchantReference,
                    MerchantReportingId = result.Data.MerchantReportingId,
                    NextStatementDate = result.Data.NextStatementDate,
                    SettlementSchedule = result.Data.SettlementSchedule switch {
                        SettlementSchedule.Weekly => DataTransferObjects.Responses.SettlementSchedule.Weekly,
                        SettlementSchedule.Monthly => DataTransferObjects.Responses.SettlementSchedule.Monthly,
                        _ => DataTransferObjects.Responses.SettlementSchedule.NotSet
                    },
                    Addresses = new(),
                    Contacts = new(),
                    Contracts = new(),
                    Devices = new(),
                    Operators = new()
                };

                foreach (AddressResponse addressModel in result.Data.Addresses) {
                    DataTransferObjects.Responses.AddressResponse addressResponse = new() {
                        AddressId = addressModel.AddressId,
                        AddressLine1 = addressModel.AddressLine1,
                        AddressLine2 = addressModel.AddressLine2,
                        AddressLine3 = addressModel.AddressLine3,
                        AddressLine4 = addressModel.AddressLine4,
                        Country = addressModel.Country,
                        PostalCode = addressModel.PostalCode,
                        Region = addressModel.Region,
                        Town = addressModel.Town
                    };
                    merchantResponse.Addresses.Add(addressResponse);
                }

                foreach (ContactResponse contactResponse in result.Data.Contacts) {
                    merchantResponse.Contacts.Add(new DataTransferObjects.Responses.ContactResponse { ContactId = contactResponse.ContactId, ContactName = contactResponse.ContactName, ContactPhoneNumber = contactResponse.ContactPhoneNumber, ContactEmailAddress = contactResponse.ContactEmailAddress });
                }

                foreach (MerchantContractResponse merchantContractResponse in result.Data.Contracts) {
                    DataTransferObjects.Responses.MerchantContractResponse contract = new() { ContractId = merchantContractResponse.ContractId, IsDeleted = merchantContractResponse.IsDeleted, ContractProducts = new() };
                    foreach (Guid contractProduct in merchantContractResponse.ContractProducts) {
                        contract.ContractProducts.Add(contractProduct);
                    }

                    merchantResponse.Contracts.Add(contract);
                }
                
                foreach (KeyValuePair<Guid, string> device in result.Data.Devices) {
                    response.Devices.Add(device.Key, device.Value);
                }

                foreach (MerchantOperatorResponse merchantOperatorResponse in result.Data.Operators) {
                    merchantResponse.Operators.Add(new DataTransferObjects.Responses.MerchantOperatorResponse {
                        OperatorId = merchantOperatorResponse.OperatorId,
                        IsDeleted = merchantOperatorResponse.IsDeleted,
                        MerchantNumber = merchantOperatorResponse.MerchantNumber,
                        Name = merchantOperatorResponse.Name,
                        TerminalNumber = merchantOperatorResponse.TerminalNumber
                    });
                }

                return merchantResponse;
            });
        }
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