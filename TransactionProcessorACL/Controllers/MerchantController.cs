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
            if (ClaimsHelper.IsPasswordToken(this.User) == false) {
                return this.Forbid();
            }

            // Do the software version check
            VersionCheckCommand versionCheckCommand = new(applicationVersion);
            Result versionCheckResult = await this.Mediator.Send(versionCheckCommand, cancellationToken);
            if (versionCheckResult.IsFailed)
                return this.StatusCode(505);

            Result<(Guid estateId, Guid merchantId)> claimsResult = TransactionController.GetRequiredClaims(this.User);
            if (claimsResult.IsFailed)
                return this.Forbid();

            MerchantQueries.GetMerchantContractsQuery query = new(claimsResult.Data.estateId, claimsResult.Data.merchantId);
            Result<List<ContractResponse>> result = await this.Mediator.Send(query, cancellationToken);

            if (result.IsFailed)
                return ResultHelpers.CreateFailure(result).ToActionResultX();

            List<DataTransferObjects.Responses.ContractResponse> responses = new();
            // Now need to convert to the DTO type for returning to the caller
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
