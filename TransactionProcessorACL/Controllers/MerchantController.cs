using Humanizer;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Shared.General;
using Shared.Results;
using SimpleResults;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Shared.Logger;
using Shared.Results.Web;
using TransactionProcessorACL.BusinessLogic.Requests;
using TransactionProcessorACL.Factories;
using TransactionProcessorACL.Models;
using static TransactionProcessorACL.BusinessLogic.Requests.VersionCheckCommands;

namespace TransactionProcessorACL.Controllers
{
    [ExcludeFromCodeCoverage]
    [Route(MerchantController.ControllerRoute)]
    [ApiController]
    [Authorize]
    public class MerchantController : ControllerBase
    {
        #region Fields

        /// <summary>
        /// The mediator
        /// </summary>
        private readonly IMediator Mediator;

        /// <summary>
        /// The model factory
        /// </summary>
        private readonly IModelFactory ModelFactory;

        #endregion
        
        public MerchantController(IMediator mediator,
                                  IModelFactory modelFactory)
        {
            this.Mediator = mediator;
            this.ModelFactory = modelFactory;
        }

        [HttpGet]
        [Route("contracts")]
        public async Task<IActionResult> GetMerchantContracts([FromQuery] String applicationVersion,
                                                              CancellationToken cancellationToken) {
            Logger.LogInformation($"Application version {applicationVersion}");
            
            if (ClaimsHelper.IsPasswordToken(this.User) == false) {
                return this.Forbid();
            }
            Logger.LogInformation($"user is nto null");
            // Do the software version check
            VersionCheckCommand versionCheckCommand = new(applicationVersion);
            Result versionCheckResult = await this.Mediator.Send(versionCheckCommand, cancellationToken);
            if (versionCheckResult.IsFailed)
                return this.StatusCode(505);

            Logger.LogInformation($"version check ok");
            Result<(Guid estateId, Guid merchantId)> claimsResult = TransactionController.GetRequiredClaims(this.User);
            if (claimsResult.IsFailed)
                return this.Forbid();

            Logger.LogInformation($"got claims ok");
            Logger.LogInformation($"estate id {claimsResult.Data.estateId}");
            Logger.LogInformation($"merchant id {claimsResult.Data.merchantId}");

            MerchantQueries.GetMerchantContractsQuery query = new(claimsResult.Data.estateId, claimsResult.Data.merchantId);
            Result<List<ContractResponse>> result = await this.Mediator.Send(query, cancellationToken);
            Logger.LogInformation($"request sent");
            if (result.IsFailed)
                return ResultHelpers.CreateFailure(result).ToActionResultX();
            Logger.LogInformation($"result was not failed");
            List<DataTransferObjects.Responses.ContractResponse> responses = new();
            // Now need to convert to the DTO type for returning to the caller
            Logger.LogInformation($"record count is {result.Data.Count}");
            foreach (ContractResponse contractModel in result.Data)
            {
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

                foreach (ContractProduct contractModelProduct in contractModel.Products)
                {
                    DataTransferObjects.Responses.ContractProduct productResponse = new() {
                        Value = contractModelProduct.Value,
                        DisplayText = contractModelProduct.DisplayText,
                        Name = contractModelProduct.Name,
                        ProductId = contractModelProduct.ProductId,
                        ProductReportingId = contractModelProduct.ProductReportingId,
                        ProductType = contractModelProduct.ProductType switch
                        {
                            ProductType.BillPayment => DataTransferObjects.Responses.ProductType.BillPayment,
                            ProductType.MobileTopup => DataTransferObjects.Responses.ProductType.MobileTopup,
                            ProductType.Voucher => DataTransferObjects.Responses.ProductType.Voucher,
                            _ => DataTransferObjects.Responses.ProductType.NotSet
                        },
                        TransactionFees = new()
                    };

                    foreach (ContractProductTransactionFee contractProductTransactionFeeModel in contractModelProduct.TransactionFees)
                    {
                        DataTransferObjects.Responses.ContractProductTransactionFee transactionFeeModel = new() {
                            Value = contractProductTransactionFeeModel.Value,
                            Description = contractProductTransactionFeeModel.Description,
                            CalculationType = contractProductTransactionFeeModel.CalculationType switch
                            {
                                CalculationType.Fixed => DataTransferObjects.Responses.CalculationType.Fixed,
                                _ => DataTransferObjects.Responses.CalculationType.Percentage,
                            },
                            FeeType = contractProductTransactionFeeModel.FeeType switch
                            {
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

            return Result.Success(responses).ToActionResultX();
        }

        [HttpGet]
        [Route("")]
        public async Task<IActionResult> GetMerchant([FromQuery] String applicationVersion,
                                                     CancellationToken cancellationToken) {
            if (ClaimsHelper.IsPasswordToken(this.User) == false) {
                return this.Forbid();
            }

            Logger.LogWarning("Here 1");
            // Do the software version check
            VersionCheckCommand versionCheckCommand = new(applicationVersion);
            Result versionCheckResult = await this.Mediator.Send(versionCheckCommand, cancellationToken);
            if (versionCheckResult.IsFailed)
                return this.StatusCode(505);
            Logger.LogWarning("Here 2");
            Result<(Guid estateId, Guid merchantId)> claimsResult = TransactionController.GetRequiredClaims(this.User);
            if (claimsResult.IsFailed)
                return this.Forbid();
            Logger.LogWarning("Here 3");
            Logger.LogWarning($"estateId is {claimsResult.Data.estateId}");
            Logger.LogWarning($"merchantId is {claimsResult.Data.merchantId}");
            MerchantQueries.GetMerchantQuery query = new(claimsResult.Data.estateId, claimsResult.Data.merchantId);
            Result<MerchantResponse> result = await this.Mediator.Send(query, cancellationToken);

            if (result.IsFailed)
                return ResultHelpers.CreateFailure(result).ToActionResultX();
            Logger.LogWarning("Here 4");
            // Now need to convert to the DTO type for returning to the caller
            DataTransferObjects.Responses.MerchantResponse response = new() {
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
            Logger.LogWarning("Here 5");
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
                response.Addresses.Add(addressResponse);
            }

            foreach (ContactResponse contactResponse in result.Data.Contacts) {
                response.Contacts.Add(new DataTransferObjects.Responses.ContactResponse { ContactId = contactResponse.ContactId, ContactName = contactResponse.ContactName, ContactPhoneNumber = contactResponse.ContactPhoneNumber, ContactEmailAddress = contactResponse.ContactEmailAddress });
            }

            foreach (MerchantContractResponse merchantContractResponse in result.Data.Contracts) {
                var contract = new DataTransferObjects.Responses.MerchantContractResponse { ContractId = merchantContractResponse.ContractId, IsDeleted = merchantContractResponse.IsDeleted, ContractProducts = new() };
                foreach (Guid contractProduct in merchantContractResponse.ContractProducts) {
                    contract.ContractProducts.Add(contractProduct);
                }

                response.Contracts.Add(contract);
            }

            foreach (KeyValuePair<Guid, string> device in result.Data.Devices) {
                response.Devices.Add(device.Key, device.Value);
            }

            foreach (MerchantOperatorResponse merchantOperatorResponse in result.Data.Operators) {
                response.Operators.Add(new DataTransferObjects.Responses.MerchantOperatorResponse {
                    OperatorId = merchantOperatorResponse.OperatorId,
                    IsDeleted = merchantOperatorResponse.IsDeleted,
                    MerchantNumber = merchantOperatorResponse.MerchantNumber,
                    Name = merchantOperatorResponse.Name,
                    TerminalNumber = merchantOperatorResponse.TerminalNumber
                });
            }

            return Result.Success(response).ToActionResultX();
        }

        #region Others

        /// <summary>
        /// The controller name
        /// </summary>
        public const String ControllerName = "merchants";

        /// <summary>
        /// The controller route
        /// </summary>
        private const String ControllerRoute = "api/" + MerchantController.ControllerName;

        #endregion
    }
}
