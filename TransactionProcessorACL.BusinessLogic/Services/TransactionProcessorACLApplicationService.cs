using SecurityService.DataTransferObjects;
using Shared.Exceptions;
using Shared.Results;
using SimpleResults;
using TransactionProcessor.DataTransferObjects.Responses.Contract;

namespace TransactionProcessorACL.BusinessLogic.Services
{
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using TransactionProcessorACL.BusinessLogic.BackendAPI;
    using TransactionProcessorACL.DataTransferObjects.Requests;
    using Models;
    using SecurityService.Client;
    using Shared.General;
    using Shared.Logger;
    using TransactionProcessor.Client;
    using TransactionProcessor.DataTransferObjects;
    using GetVoucherResponse = Models.GetVoucherResponse;
    using RedeemVoucherResponse = Models.RedeemVoucherResponse;

    public class TransactionProcessorACLApplicationService : ITransactionProcessorACLApplicationService
    {
        private readonly ISecurityServiceClient SecurityServiceClient;

        private readonly ITransactionProcessorClient TransactionProcessorClient;

        private readonly IEstateReportingApiClient EstateReportingApiClient;

        public TransactionProcessorACLApplicationService(ITransactionProcessorClient transactionProcessorClient,
                                                         ISecurityServiceClient securityServiceClient,
                                                         IEstateReportingApiClient estateReportingApiClient)
        {
            this.TransactionProcessorClient = transactionProcessorClient;
            this.SecurityServiceClient = securityServiceClient;
            this.EstateReportingApiClient = estateReportingApiClient;
        }
        
        public async Task<Result<ProcessLogonTransactionResponse>> ProcessLogonTransaction(Guid estateId,
                                                                                           Guid merchantId,
                                                                                           DateTime transactionDateTime,
                                                                                           String transactionNumber,
                                                                                           String deviceIdentifier,
                                                                                           CancellationToken cancellationToken)
        {
            Result<TokenResponse> accessTokenResult = await this.GetAccessToken(cancellationToken);
            if (accessTokenResult.IsFailed) {
                return ResultHelpers.CreateFailure(accessTokenResult);
            }

            TokenResponse accessToken = accessTokenResult.Data;
            LogonTransactionRequest logonTransactionRequest = CreateLogonRequestMessage(estateId, merchantId, transactionDateTime, transactionNumber, deviceIdentifier);

            try {
                Result<LogonTransactionResponse> transactionResult = await this.TransactionProcessorClient.PerformTransaction(accessToken.AccessToken, logonTransactionRequest, cancellationToken);

                if (transactionResult.IsFailed)
                    return ResultHelpers.CreateFailure(transactionResult);
                
                return Result.Success(CreateProcessLogonTransactionResponse(estateId, merchantId, transactionResult.Data));
            }
            catch (Exception ex) {
                return Result.Success(CreateLogonErrorResponse(estateId, merchantId, ex));
            }
        }

        public async Task<Result<ProcessSaleTransactionResponse>> ProcessSaleTransaction((Guid estateId, Guid merchantId) merchantData,
                                                                                         DateTime transactionDateTime,
                                                                                         String transactionNumber,
                                                                                         String deviceIdentifier,
                                                                                         String customerEmailAddress,
                                                                                         (Guid operatorId, Guid contractId, Guid productId) productData,
                                                                                         Dictionary<String, String> additionalRequestMetadata, CancellationToken cancellationToken)
        {
            Result<TokenResponse> accessTokenResult = await this.GetAccessToken(cancellationToken);
            if (accessTokenResult.IsFailed) {
                return ResultHelpers.CreateFailure(accessTokenResult);
            }
            TokenResponse accessToken = accessTokenResult.Data;

            SaleTransactionRequest saleTransactionRequest = this.BuildSaleTransactionRequest(merchantData,
                                                                                            transactionNumber,
                                                                                            deviceIdentifier,
                                                                                            transactionDateTime,
                                                                                            productData,
                                                                                            customerEmailAddress,
                                                                                            additionalRequestMetadata);

            ProcessSaleTransactionResponse response = null;

            try
            {
                Result<SaleTransactionResponse> transactionResult = 
                    await this.TransactionProcessorClient.PerformTransaction(accessToken.AccessToken, saleTransactionRequest, cancellationToken);
                if (transactionResult.IsFailed)
                    return ResultHelpers.CreateFailure(transactionResult);

                response = this.CreateSaleTransactionResponse(transactionResult.Data, merchantData.estateId, merchantData.merchantId);
            }
            catch (Exception ex)
            {
                response = this.CreateSaleTransactionErrorResponse(merchantData.estateId, merchantData.merchantId, ex);
            }

            return Result.Success(response);
        }

        private async Task<Result<TokenResponse>> GetAccessToken(CancellationToken cancellationToken)
        {
            String clientId = ConfigurationReader.GetValue("AppSettings", "ClientId");
            String clientSecret = ConfigurationReader.GetValue("AppSettings", "ClientSecret");

            return await this.SecurityServiceClient.GetToken(clientId, clientSecret, cancellationToken);
        }

        private SaleTransactionRequest BuildSaleTransactionRequest((Guid estateId, Guid merchantId) merchantData,
                                                                   String transactionNumber,
                                                                   String deviceIdentifier, 
                                                                   DateTime transactionDateTime,
                                                                   (Guid operatorId, Guid contractId, Guid productId) productData,
                                                                   String customerEmailAddress,
                                                                   Dictionary<String, String> additionalRequestMetadata)
        {
            return new SaleTransactionRequest
            {
                TransactionNumber = transactionNumber,
                DeviceIdentifier = deviceIdentifier,
                TransactionDateTime = transactionDateTime,
                TransactionType = "SALE",
                OperatorId = productData.operatorId,
                CustomerEmailAddress = customerEmailAddress,
                TransactionSource = 1,
                ContractId = productData.contractId,
                ProductId = productData.productId,
                AdditionalTransactionMetadata = additionalRequestMetadata,
                EstateId = merchantData.estateId,
                MerchantId = merchantData.merchantId,
            };
        }


        private ProcessSaleTransactionResponse CreateSaleTransactionResponse(SaleTransactionResponse saleTransactionResponse,
                                                                             Guid estateId,
                                                                             Guid merchantId)
        {
            return new ProcessSaleTransactionResponse
            {
                ResponseCode = saleTransactionResponse.ResponseCode,
                ResponseMessage = saleTransactionResponse.ResponseMessage,
                EstateId = estateId,
                MerchantId = merchantId,
                AdditionalTransactionMetadata = saleTransactionResponse.AdditionalTransactionMetadata,
                TransactionId = saleTransactionResponse.TransactionId,
            };
        }

        private ProcessSaleTransactionResponse CreateSaleTransactionErrorResponse(Guid estateId,
                                                                                  Guid merchantId,
                                                                                  Exception ex)
        {
            return new ProcessSaleTransactionResponse
            {
                ResponseCode = "0001", // Request Message error
                ResponseMessage = "Process Sale Failed",
                EstateId = estateId,
                MerchantId = merchantId,
                ErrorMessages = ex.GetExceptionMessages()
            };
        }

        public async Task<Result<ProcessReconciliationResponse>> ProcessReconciliation(Guid estateId,
                                                                                       Guid merchantId,
                                                                                       DateTime transactionDateTime,
                                                                                       String deviceIdentifier,
                                                                                       Int32 transactionCount,
                                                                                       Decimal transactionValue,
                                                                                       CancellationToken cancellationToken)
        {
            Result<TokenResponse> accessTokenResult = await this.GetAccessToken(cancellationToken);
            if (accessTokenResult.IsFailed) {
                return ResultHelpers.CreateFailure(accessTokenResult);
            }

            TokenResponse accessToken = accessTokenResult.Data;
            ReconciliationRequest reconciliationRequest = CreateReconciliationRequestMessage(estateId,
                                                                                            merchantId,
                                                                                            transactionDateTime,
                                                                                            deviceIdentifier,
                                                                                            transactionCount,
                                                                                            transactionValue);

            try
            {
                Result<ReconciliationResponse> transactionResult = 
                    await this.TransactionProcessorClient.PerformTransaction(accessToken.AccessToken, reconciliationRequest, cancellationToken);
                if (transactionResult.IsFailed)
                    return ResultHelpers.CreateFailure(transactionResult);
                return Result.Success(CreateProcessReconciliationResponse(transactionResult.Data));
            }
            catch (Exception ex)
            {
                return Result.Success(CreateReconciliationErrorResponse(estateId, merchantId, ex));
            }
        }

        public async Task<Result<GetVoucherResponse>> GetVoucher(Guid estateId,
                                                                 Guid contractId,
                                                                 String voucherCode,
                                                                 CancellationToken cancellationToken)
        {
            Result<TokenResponse> accessTokenResult = await this.GetAccessToken(cancellationToken);
            if (accessTokenResult.IsFailed) {
                return ResultHelpers.CreateFailure(accessTokenResult);
            }

            TokenResponse accessToken = accessTokenResult.Data;

            try {
                Result<TransactionProcessor.DataTransferObjects.GetVoucherResponse> getVoucherResult = await this.TransactionProcessorClient.GetVoucherByCode(accessToken.AccessToken, estateId, voucherCode, cancellationToken);
                if (getVoucherResult.IsFailed) {
                    return ResultHelpers.CreateFailure(getVoucherResult);
                }

                return Result.Success(CreateGetVoucherSuccessResponse(estateId, contractId, getVoucherResult.Data));
            }
            catch (Exception ex) {
                return Result.Failure(ex.GetExceptionMessages());
            }
        }

        public async Task<Result<RedeemVoucherResponse>> RedeemVoucher(Guid estateId,
                                                                       Guid contractId,
                                                                       String voucherCode,
                                                                       CancellationToken cancellationToken)
        {
            Result<TokenResponse> accessTokenResult = await this.GetAccessToken(cancellationToken);
            if (accessTokenResult.IsFailed) {
                return ResultHelpers.CreateFailure(accessTokenResult);
            }
            
            TokenResponse accessToken = accessTokenResult.Data;

            try {
                RedeemVoucherRequest redeemVoucherRequest = new() { EstateId = estateId, VoucherCode = voucherCode };

                Result redeemVoucherResponseResult = await this.TransactionProcessorClient.RedeemVoucher(accessToken.AccessToken, redeemVoucherRequest, cancellationToken);

                if (redeemVoucherResponseResult.IsFailed) {
                    return ResultHelpers.CreateFailure(redeemVoucherResponseResult);
                }

                return Result.Success(new RedeemVoucherResponse {
                    ResponseCode = "0000", // Success
                    ResponseMessage = "SUCCESS",
                    ContractId = contractId,
                    EstateId = estateId
                });
            }
            catch (Exception ex) {
                return Result.Failure(ex.GetExceptionMessages());
            }
        }

        public async Task<Result<List<Models.ContractResponse>>> GetMerchantContracts(Guid estateId,
                                                                                      Guid merchantId,
                                                                                      CancellationToken cancellationToken) {
            Logger.LogInformation($"GetMerchantContracts: {estateId} {merchantId}");

            Result<TokenResponse> accessTokenResult = await this.GetAccessToken(cancellationToken);
            if (accessTokenResult.IsFailed) {
                return ResultHelpers.CreateFailure(accessTokenResult);
            }

            TokenResponse accessToken = accessTokenResult.Data;

            Result<List<TransactionProcessor.DataTransferObjects.Responses.Contract.ContractResponse>> result = await this.TransactionProcessorClient.GetMerchantContracts(accessToken.AccessToken, estateId, merchantId, cancellationToken);

            if (result.IsFailed)
                return Result.Failure($"Error getting merchant contracts {result.Message}");

            Logger.LogInformation($"Got the merchant contracts {result.Data.Count}");
            List<Models.ContractResponse> models = new();

            foreach (TransactionProcessor.DataTransferObjects.Responses.Contract.ContractResponse contractResponse in result.Data) {
                Logger.LogInformation($"Processing contract {contractResponse.OperatorName}");

                ContractResponse contractModel = new ContractResponse {
                    ContractId = contractResponse.ContractId,
                    ContractReportingId = contractResponse.ContractReportingId,
                    Description = contractResponse.Description,
                    EstateId = contractResponse.EstateId,
                    EstateReportingId = contractResponse.EstateReportingId,
                    OperatorId = contractResponse.OperatorId,
                    OperatorName = contractResponse.OperatorName,
                    Products = new()
                };

                foreach (TransactionProcessor.DataTransferObjects.Responses.Contract.ContractProduct contractResponseProduct in contractResponse.Products) {
                    Logger.LogInformation($"Processing contract product {contractResponseProduct.DisplayText}");
                    ContractProduct productModel = new ContractProduct {
                        Value = contractResponseProduct.Value,
                        DisplayText = contractResponseProduct.DisplayText,
                        Name = contractResponseProduct.Name,
                        ProductId = contractResponseProduct.ProductId,
                        ProductReportingId = contractResponseProduct.ProductReportingId,
                        ProductType = contractResponseProduct.ProductType switch {
                            TransactionProcessor.DataTransferObjects.Responses.Contract.ProductType.BillPayment => ProductType.BillPayment,
                            TransactionProcessor.DataTransferObjects.Responses.Contract.ProductType.MobileTopup => ProductType.MobileTopup,
                            TransactionProcessor.DataTransferObjects.Responses.Contract.ProductType.Voucher => ProductType.Voucher,
                            _ => ProductType.NotSet
                        },
                        TransactionFees = new()
                    };
                    
                    contractModel.Products.Add(productModel);
                }

                models.Add(contractModel);
            }

            return Result.Success(models);
        }

        public async Task<Result<MerchantResponse>> GetMerchant(Guid estateId,
                                                                Guid merchantId,
                                                                CancellationToken cancellationToken) {
            Result<TokenResponse> accessTokenResult = await this.GetAccessToken(cancellationToken);
            if (accessTokenResult.IsFailed) {
                return ResultHelpers.CreateFailure(accessTokenResult);
            }

            TokenResponse accessToken = accessTokenResult.Data;
            
            Result<TransactionProcessor.DataTransferObjects.Responses.Merchant.MerchantResponse> result = await this.TransactionProcessorClient.GetMerchant(accessToken.AccessToken, estateId, merchantId, cancellationToken);

            if (result.IsFailed)
                return Result.Failure($"Error getting merchant {result.Message}");
            
            MerchantResponse merchantResponse = ResponseFactory.Build(result.Data);

            return Result.Success(merchantResponse);
        }

        public async Task<Result<MerchantScheduleResponse>> GetMerchantSchedule(Guid estateId,
                                                                                Guid merchantId,
                                                                                Int32 year,
                                                                                CancellationToken cancellationToken) {
            Result<TokenResponse> accessTokenResult = await this.GetAccessToken(cancellationToken);
            if (accessTokenResult.IsFailed)
            {
                return ResultHelpers.CreateFailure(accessTokenResult);
            }

            TokenResponse accessToken = accessTokenResult.Data;

            Result<TransactionProcessor.DataTransferObjects.Responses.Merchant.MerchantScheduleResponse> result = await this.TransactionProcessorClient.GetMerchantSchedule(accessToken.AccessToken, estateId, merchantId, year, cancellationToken);

            if (result.IsFailed)
                return Result.Failure($"Error getting merchant schedule {result.Message}");

            MerchantScheduleResponse merchantScheduleResponse = ResponseFactory.Build(result.Data);

            return Result.Success(merchantScheduleResponse);
        }

        public async Task<Result<MerchantDailyPerformanceSummaryResponse>> GetMerchantDailyPerformanceSummary(Guid estateId,
                                                                                                               MerchantDailyPerformanceSummaryRequest request,
                                                                                                               CancellationToken cancellationToken)
        {
            Result<TokenResponse> accessTokenResult = await this.GetAccessToken(cancellationToken);
            if (accessTokenResult.IsFailed) {
                return ResultHelpers.CreateFailure(accessTokenResult);
            }

            TokenResponse accessToken = accessTokenResult.Data;
            return await this.EstateReportingApiClient.GetMerchantDailyPerformanceSummary(accessToken.AccessToken, estateId, request, cancellationToken);
        }

        public async Task<Result<MerchantTransactionMixSummaryResponse>> GetMerchantTransactionMixSummary(Guid estateId,
                                                                                                          MerchantTransactionMixSummaryRequest request,
                                                                                                          CancellationToken cancellationToken)
        {
            Result<TokenResponse> accessTokenResult = await this.GetAccessToken(cancellationToken);
            if (accessTokenResult.IsFailed) {
                return ResultHelpers.CreateFailure(accessTokenResult);
            }

            TokenResponse accessToken = accessTokenResult.Data;
            return await this.EstateReportingApiClient.GetMerchantTransactionMixSummary(accessToken.AccessToken, estateId, request, cancellationToken);
        }

        private static ProcessReconciliationResponse CreateProcessReconciliationResponse(ReconciliationResponse reconciliationResponse)
        {
            return new ProcessReconciliationResponse
            {
                ResponseCode = reconciliationResponse.ResponseCode,
                ResponseMessage = reconciliationResponse.ResponseMessage,
                TransactionId = reconciliationResponse.TransactionId,
            };
        }

        private static GetVoucherResponse CreateGetVoucherSuccessResponse(Guid estateId,
                                                                          Guid contractId,
                                                                          TransactionProcessor.DataTransferObjects.GetVoucherResponse getVoucherResponse)
        {
            return new GetVoucherResponse
            {
                ResponseCode = "0000", // Success
                ContractId = contractId,
                EstateId = estateId,
                ExpiryDate = getVoucherResponse.ExpiryDate,
                Value = getVoucherResponse.Value,
                VoucherCode = getVoucherResponse.VoucherCode,
                VoucherId = getVoucherResponse.VoucherId,
                Balance = getVoucherResponse.Balance
            };
        }
        
        private static ProcessReconciliationResponse CreateReconciliationErrorResponse(Guid estateId,
                                                                                       Guid merchantId,
                                                                                       Exception ex)
        {
            return new ProcessReconciliationResponse
            {
                ResponseCode = "0001", // Request Message error
                ResponseMessage = "Process Reconciliation Failed",
                EstateId = estateId,
                MerchantId = merchantId,
                ErrorMessages = ex.GetExceptionMessages()
            };
        }

        private static ReconciliationRequest CreateReconciliationRequestMessage(Guid estateId,
                                                                                Guid merchantId,
                                                                                DateTime transactionDateTime,
                                                                                String deviceIdentifier,
                                                                                Int32 transactionCount,
                                                                                Decimal transactionValue)
        {
            ReconciliationRequest reconciliationRequest = new ReconciliationRequest
            {
                DeviceIdentifier = deviceIdentifier,
                TransactionDateTime = transactionDateTime,
                TransactionCount = transactionCount,
                TransactionValue = transactionValue,
                EstateId = estateId,
                MerchantId = merchantId
            };

            return reconciliationRequest;
        }

        private static ProcessLogonTransactionResponse CreateProcessLogonTransactionResponse(Guid estateId,
                                                                                             Guid merchantId,
                                                                                             LogonTransactionResponse logonTransactionResponse)
        {
            return new ProcessLogonTransactionResponse
            {
                ResponseCode = logonTransactionResponse.ResponseCode,
                ResponseMessage = logonTransactionResponse.ResponseMessage,
                EstateId = estateId,
                MerchantId = merchantId,
                TransactionId = logonTransactionResponse.TransactionId,
            };
        }

        private static ProcessLogonTransactionResponse CreateLogonErrorResponse(Guid estateId,
                                                                                Guid merchantId,
                                                                                Exception ex)
        {
            return new ProcessLogonTransactionResponse
            {
                ResponseCode = "0001", // Request Message error
                ResponseMessage = "Process Logon Failed",
                EstateId = estateId,
                MerchantId = merchantId,
                ErrorMessages = ex.GetExceptionMessages()
            };
        }

        private static LogonTransactionRequest CreateLogonRequestMessage(Guid estateId,
                                                                         Guid merchantId,
                                                                         DateTime transactionDateTime,
                                                                         String transactionNumber,
                                                                         String deviceIdentifier)
        {
            LogonTransactionRequest logonTransactionRequest = new LogonTransactionRequest
            {
                TransactionNumber = transactionNumber,
                DeviceIdentifier = deviceIdentifier,
                TransactionDateTime = transactionDateTime,
                TransactionType = "LOGON",
                EstateId = estateId,
                MerchantId = merchantId
            };
            
            return logonTransactionRequest;
        }
    }
}
