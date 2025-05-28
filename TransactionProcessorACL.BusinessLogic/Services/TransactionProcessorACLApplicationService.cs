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
    using Models;
    using Newtonsoft.Json;
    using SecurityService.Client;
    using SecurityService.DataTransferObjects.Responses;
    using Shared.General;
    using Shared.Logger;
    using TransactionProcessor.Client;
    using TransactionProcessor.DataTransferObjects;
    using GetVoucherResponse = Models.GetVoucherResponse;
    using RedeemVoucherResponse = Models.RedeemVoucherResponse;

    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="TransactionProcessorACL.BusinessLogic.Services.ITransactionProcessorACLApplicationService" />
    public class TransactionProcessorACLApplicationService : ITransactionProcessorACLApplicationService
    {
        #region Fields

        /// <summary>
        /// The security service client
        /// </summary>
        private readonly ISecurityServiceClient SecurityServiceClient;

        /// <summary>
        /// The transaction processor client
        /// </summary>
        private readonly ITransactionProcessorClient TransactionProcessorClient;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="TransactionProcessorACLApplicationService"/> class.
        /// </summary>
        /// <param name="transactionProcessorClient">The transaction processor client.</param>
        /// <param name="securityServiceClient">The security service client.</param>
        public TransactionProcessorACLApplicationService(ITransactionProcessorClient transactionProcessorClient,
                                                         ISecurityServiceClient securityServiceClient)
        {
            this.TransactionProcessorClient = transactionProcessorClient;
            this.SecurityServiceClient = securityServiceClient;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Processes the logon transaction.
        /// </summary>
        /// <param name="estateId">The estate identifier.</param>
        /// <param name="merchantId">The merchant identifier.</param>
        /// <param name="transactionDateTime">The transaction date time.</param>
        /// <param name="transactionNumber">The transaction number.</param>
        /// <param name="deviceIdentifier">The device identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        public async Task<Result<ProcessLogonTransactionResponse>> ProcessLogonTransaction(Guid estateId,
                                                                                           Guid merchantId,
                                                                                           DateTime transactionDateTime,
                                                                                           String transactionNumber,
                                                                                           String deviceIdentifier,
                                                                                           CancellationToken cancellationToken)
        {
            // Get a client token to call the Transaction Processor
            String clientId = ConfigurationReader.GetValue("AppSettings", "ClientId");
            String clientSecret = ConfigurationReader.GetValue("AppSettings", "ClientSecret");

            Result<TokenResponse> accessTokenResult = await this.SecurityServiceClient.GetToken(clientId, clientSecret, cancellationToken);
            if (accessTokenResult.IsFailed) {
                return ResultHelpers.CreateFailure(accessTokenResult);
            }
            TokenResponse accessToken = accessTokenResult.Data;
            LogonTransactionRequest logonTransactionRequest = new LogonTransactionRequest();
            logonTransactionRequest.TransactionNumber = transactionNumber;
            logonTransactionRequest.DeviceIdentifier = deviceIdentifier;
            logonTransactionRequest.TransactionDateTime = transactionDateTime;
            logonTransactionRequest.TransactionType = "LOGON";

            SerialisedMessage requestSerialisedMessage = new SerialisedMessage();
            requestSerialisedMessage.Metadata.Add(MetadataContants.KeyNameEstateId, estateId.ToString());
            requestSerialisedMessage.Metadata.Add(MetadataContants.KeyNameMerchantId, merchantId.ToString());
            requestSerialisedMessage.SerialisedData = JsonConvert.SerializeObject(logonTransactionRequest,
                                                                                  new JsonSerializerSettings
                                                                                  {
                                                                                      TypeNameHandling = TypeNameHandling.All
                                                                                  });

            ProcessLogonTransactionResponse response = null;

            try
            {
                Result<SerialisedMessage> transactionResult = 
                    await this.TransactionProcessorClient.PerformTransaction(accessToken.AccessToken, requestSerialisedMessage, cancellationToken);

                if (transactionResult.IsFailed)
                    return ResultHelpers.CreateFailure(transactionResult);
                SerialisedMessage responseSerialisedMessage = transactionResult.Data;

                LogonTransactionResponse logonTransactionResponse = JsonConvert.DeserializeObject<LogonTransactionResponse>(responseSerialisedMessage.SerialisedData);

                response = new ProcessLogonTransactionResponse
                           {
                               ResponseCode = logonTransactionResponse.ResponseCode,
                               ResponseMessage = logonTransactionResponse.ResponseMessage,
                               EstateId = estateId,
                               MerchantId = merchantId,
                               TransactionId = logonTransactionResponse.TransactionId,
                };
            }
            catch(Exception ex)
            {
                if (ex.InnerException is InvalidOperationException)
                {
                    // This means there is an error in the request
                    response = new ProcessLogonTransactionResponse
                               {
                                   ResponseCode = "0001", // Request Message error
                                   ResponseMessage = ex.InnerException.Message,
                                   EstateId = estateId,
                                   MerchantId = merchantId
                    };
                }
                else if (ex.InnerException is HttpRequestException)
                {
                    Logger.LogError(ex.InnerException);

                    // Request Send Exception
                    response = new ProcessLogonTransactionResponse
                    {
                                   ResponseCode = "0002", // Request Message error
                                   ResponseMessage = $"Error Sending Request Message [{ex.InnerException.Message}]",
                                   EstateId = estateId,
                                   MerchantId = merchantId
                    };
                }
                else
                {
                    response = new ProcessLogonTransactionResponse
                    {
                                   ResponseCode = "0003", // General error
                                   ResponseMessage = "General Error",
                                   EstateId = estateId,
                                   MerchantId = merchantId
                    };
                }
            }

            return Result.Success(response);
        }

        /// <summary>
        /// Processes the sale transaction.
        /// </summary>
        /// <param name="estateId">The estate identifier.</param>
        /// <param name="merchantId">The merchant identifier.</param>
        /// <param name="transactionDateTime">The transaction date time.</param>
        /// <param name="transactionNumber">The transaction number.</param>
        /// <param name="deviceIdentifier">The device identifier.</param>
        /// <param name="operatorIdentifier">The operator identifier.</param>
        /// <param name="customerEmailAddress">The customer email address.</param>
        /// <param name="contractId">The contract identifier.</param>
        /// <param name="productId">The product identifier.</param>
        /// <param name="additionalRequestMetadata">The additional request metadata.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        public async Task<Result<ProcessSaleTransactionResponse>> ProcessSaleTransaction(Guid estateId,
                                                                                         Guid merchantId,
                                                                                         DateTime transactionDateTime,
                                                                                         String transactionNumber,
                                                                                         String deviceIdentifier,
                                                                                         Guid operatorId,
                                                                                         String customerEmailAddress,
                                                                                         Guid contractId,
                                                                                         Guid productId,
                                                                                         Dictionary<String, String> additionalRequestMetadata,
                                                                                         CancellationToken cancellationToken)
        {
            // Get a client token to call the Transaction Processor
            String clientId = ConfigurationReader.GetValue("AppSettings", "ClientId");
            String clientSecret = ConfigurationReader.GetValue("AppSettings", "ClientSecret");

            Result<TokenResponse> accessTokenResult = await this.SecurityServiceClient.GetToken(clientId, clientSecret, cancellationToken);
            if (accessTokenResult.IsFailed)
            {
                return ResultHelpers.CreateFailure(accessTokenResult);
            }
            TokenResponse accessToken = accessTokenResult.Data;

            SaleTransactionRequest saleTransactionRequest = new SaleTransactionRequest();
            saleTransactionRequest.TransactionNumber = transactionNumber;
            saleTransactionRequest.DeviceIdentifier = deviceIdentifier;
            saleTransactionRequest.TransactionDateTime = transactionDateTime;
            saleTransactionRequest.TransactionType = "SALE";
            saleTransactionRequest.OperatorId = operatorId;
            saleTransactionRequest.CustomerEmailAddress = customerEmailAddress;
            saleTransactionRequest.TransactionSource = 1; // Online sale

            // Set the product information
            saleTransactionRequest.ContractId = contractId;
            saleTransactionRequest.ProductId = productId;

            // Build up the metadata
            saleTransactionRequest.AdditionalTransactionMetadata = additionalRequestMetadata;
            
            SerialisedMessage requestSerialisedMessage = new SerialisedMessage();
            requestSerialisedMessage.Metadata.Add(MetadataContants.KeyNameEstateId, estateId.ToString());
            requestSerialisedMessage.Metadata.Add(MetadataContants.KeyNameMerchantId, merchantId.ToString());
            requestSerialisedMessage.SerialisedData = JsonConvert.SerializeObject(saleTransactionRequest,
                                                                                  new JsonSerializerSettings
                                                                                  {
                                                                                      TypeNameHandling = TypeNameHandling.All
                                                                                  });

            ProcessSaleTransactionResponse response = null;

            try
            {
                Result<SerialisedMessage> transactionResult = 
                    await this.TransactionProcessorClient.PerformTransaction(accessToken.AccessToken, requestSerialisedMessage, cancellationToken);
                if (transactionResult.IsFailed)
                    return ResultHelpers.CreateFailure(transactionResult);
                SerialisedMessage responseSerialisedMessage = transactionResult.Data;

                SaleTransactionResponse saleTransactionResponse = JsonConvert.DeserializeObject<SaleTransactionResponse>(responseSerialisedMessage.SerialisedData);

                response = new ProcessSaleTransactionResponse
                {
                    ResponseCode = saleTransactionResponse.ResponseCode,
                    ResponseMessage = saleTransactionResponse.ResponseMessage,
                    EstateId = estateId,
                    MerchantId = merchantId,
                    AdditionalTransactionMetadata = saleTransactionResponse.AdditionalTransactionMetadata,
                    TransactionId = saleTransactionResponse.TransactionId,
                };
            }
            catch (Exception ex)
            {
                if (ex.InnerException is InvalidOperationException)
                {
                    // This means there is an error in the request
                    response = new ProcessSaleTransactionResponse
                    {
                        ResponseCode = "0001", // Request Message error
                        ResponseMessage = ex.InnerException.Message,
                        EstateId = estateId,
                        MerchantId = merchantId
                    };
                }
                else if (ex.InnerException is HttpRequestException) 
                {
                    // Request Send Exception
                    response = new ProcessSaleTransactionResponse
                               {
                                   ResponseCode = "0002", // Request Message error
                                   ResponseMessage = "Error Sending Request Message",
                                   EstateId = estateId,
                                   MerchantId = merchantId
                    };
                }
                else
                {
                    response = new ProcessSaleTransactionResponse
                               {
                                   ResponseCode = "0003", // General error
                                   ResponseMessage = "General Error",
                                   EstateId = estateId,
                                   MerchantId = merchantId
                    };
                }
            }

            return Result.Success(response);
        }

        public async Task<Result<ProcessReconciliationResponse>> ProcessReconciliation(Guid estateId,
                                                                                       Guid merchantId,
                                                                                       DateTime transactionDateTime,
                                                                                       String deviceIdentifier,
                                                                                       Int32 transactionCount,
                                                                                       Decimal transactionValue,
                                                                                       CancellationToken cancellationToken)
        {
            // Get a client token to call the Transaction Processor
            String clientId = ConfigurationReader.GetValue("AppSettings", "ClientId");
            String clientSecret = ConfigurationReader.GetValue("AppSettings", "ClientSecret");

            Result<TokenResponse> accessTokenResult = await this.SecurityServiceClient.GetToken(clientId, clientSecret, cancellationToken);
            if (accessTokenResult.IsFailed) {
                return ResultHelpers.CreateFailure(accessTokenResult);
            }

            TokenResponse accessToken = accessTokenResult.Data;
            ReconciliationRequest reconciliationRequest = new ReconciliationRequest();
            reconciliationRequest.DeviceIdentifier = deviceIdentifier;
            reconciliationRequest.TransactionDateTime = transactionDateTime;
            reconciliationRequest.TransactionCount = transactionCount;
            reconciliationRequest.TransactionValue = transactionValue;

            SerialisedMessage requestSerialisedMessage = new SerialisedMessage();
            requestSerialisedMessage.Metadata.Add(MetadataContants.KeyNameEstateId, estateId.ToString());
            requestSerialisedMessage.Metadata.Add(MetadataContants.KeyNameMerchantId, merchantId.ToString());
            requestSerialisedMessage.SerialisedData = JsonConvert.SerializeObject(reconciliationRequest,
                                                                                  new JsonSerializerSettings
                                                                                  {
                                                                                      TypeNameHandling = TypeNameHandling.All
                                                                                  });

            ProcessReconciliationResponse response = null;

            try
            {
                Result<SerialisedMessage> transactionResult = 
                    await this.TransactionProcessorClient.PerformTransaction(accessToken.AccessToken, requestSerialisedMessage, cancellationToken);
                if (transactionResult.IsFailed)
                    return ResultHelpers.CreateFailure(transactionResult);
                SerialisedMessage responseSerialisedMessage = transactionResult.Data;
                ReconciliationResponse reconciliationResponse = JsonConvert.DeserializeObject<ReconciliationResponse>(responseSerialisedMessage.SerialisedData);

                response = new ProcessReconciliationResponse
                {
                    ResponseCode = reconciliationResponse.ResponseCode,
                    ResponseMessage = reconciliationResponse.ResponseMessage,
                    TransactionId = reconciliationResponse.TransactionId,
                };
            }
            catch (Exception ex)
            {
                if (ex.InnerException is InvalidOperationException)
                {
                    // This means there is an error in the request
                    response = new ProcessReconciliationResponse
                    {
                        ResponseCode = "0001", // Request Message error
                        ResponseMessage = ex.InnerException.Message
                    };
                }
                else if (ex.InnerException is HttpRequestException)
                {
                    // Request Send Exception
                    response = new ProcessReconciliationResponse
                    {
                        ResponseCode = "0002", // Request Message error
                        ResponseMessage = "Error Sending Request Message"
                    };
                }
                else
                {
                    response = new ProcessReconciliationResponse
                    {
                        ResponseCode = "0003", // General error
                        ResponseMessage = "General Error"
                    };
                }
            }

            return Result.Success(response);
        }

        public async Task<GetVoucherResponse> GetVoucher(Guid estateId,
                                                         Guid contractId,
                                                         String voucherCode,
                                                         CancellationToken cancellationToken)
        {
            // Get a client token to call the Voucher Management
            String clientId = ConfigurationReader.GetValue("AppSettings", "ClientId");
            String clientSecret = ConfigurationReader.GetValue("AppSettings", "ClientSecret");

            Result<TokenResponse> accessTokenResult = await this.SecurityServiceClient.GetToken(clientId, clientSecret, cancellationToken);
            if (accessTokenResult.IsFailed) {
                return new GetVoucherResponse {
                    ResponseCode = "0004", // Token error
                    ResponseMessage = "Token Error",
                };
            }
            TokenResponse accessToken = accessTokenResult.Data;

            GetVoucherResponse response = null;

            try
            {
                Result<TransactionProcessor.DataTransferObjects.GetVoucherResponse> getVoucherResult = await this.TransactionProcessorClient.GetVoucherByCode(accessToken.AccessToken, estateId, voucherCode, cancellationToken);
                if (getVoucherResult.IsFailed)
                {
                    return new GetVoucherResponse
                    {
                        ResponseCode = "0005", // Voucher not found
                        ResponseMessage = getVoucherResult.Message,
                    };
                }
                TransactionProcessor.DataTransferObjects.GetVoucherResponse getVoucherResponse = getVoucherResult.Data;
                response = new GetVoucherResponse
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
            catch (Exception ex)
            {
                if (ex.InnerException is InvalidOperationException)
                {
                    // This means there is an error in the request
                    response = new GetVoucherResponse
                    {
                        ResponseCode = "0001", // Request Message error
                        ResponseMessage = ex.InnerException.Message,
                    };
                }
                else if (ex.InnerException is HttpRequestException)
                {
                    // Request Send Exception
                    response = new GetVoucherResponse
                    {
                        ResponseCode = "0002", // Request Message error
                        ResponseMessage = "Error Sending Request Message",
                    };
                }
                else
                {
                    response = new GetVoucherResponse
                    {
                        ResponseCode = "0003", // General error
                        ResponseMessage = "General Error",
                    };
                }
            }

            return response;
        }

        public async Task<RedeemVoucherResponse> RedeemVoucher(Guid estateId,
                                                               Guid contractId,
                                                               String voucherCode,
                                                               CancellationToken cancellationToken)
        {
            // Get a client token to call the Voucher Management
            String clientId = ConfigurationReader.GetValue("AppSettings", "ClientId");
            String clientSecret = ConfigurationReader.GetValue("AppSettings", "ClientSecret");

            Result<TokenResponse> accessTokenResult = await this.SecurityServiceClient.GetToken(clientId, clientSecret, cancellationToken);
            if (accessTokenResult.IsFailed) {
                return new RedeemVoucherResponse {
                    ResponseCode = "0004", // Token error
                    ResponseMessage = "Token Error",
                };
            }
            TokenResponse accessToken = accessTokenResult.Data;

            RedeemVoucherResponse response = null;

            try
            {
                RedeemVoucherRequest redeemVoucherRequest = new RedeemVoucherRequest
                {
                    EstateId = estateId,
                    VoucherCode = voucherCode
                };

                Result<TransactionProcessor.DataTransferObjects.RedeemVoucherResponse> redeemVoucherResponseResult = await this.TransactionProcessorClient.RedeemVoucher(accessToken.AccessToken, redeemVoucherRequest, cancellationToken);

                if (redeemVoucherResponseResult.IsFailed)
                {
                    return new RedeemVoucherResponse
                    {
                        ResponseCode = "0005", // Voucher not found or already redeemed
                        ResponseMessage = redeemVoucherResponseResult.Message,
                    };
                }
                TransactionProcessor.DataTransferObjects.RedeemVoucherResponse redeemVoucherResponse = redeemVoucherResponseResult.Data;
                response = new RedeemVoucherResponse
                {
                    ResponseCode = "0000", // Success
                    ResponseMessage = "SUCCESS",
                    ContractId = contractId,
                    EstateId = estateId,
                    ExpiryDate = redeemVoucherResponse.ExpiryDate,
                    Balance = redeemVoucherResponse.RemainingBalance,
                    VoucherCode = redeemVoucherResponse.VoucherCode
                };
            }
            catch (Exception ex)
            {
                if (ex.InnerException is InvalidOperationException)
                {
                    // This means there is an error in the request
                    response = new RedeemVoucherResponse
                    {
                        ResponseCode = "0001", // Request Message error
                        ResponseMessage = ex.InnerException.Message,
                    };
                }
                else if (ex.InnerException is HttpRequestException)
                {
                    // Request Send Exception
                    response = new RedeemVoucherResponse
                    {
                        ResponseCode = "0002", // Request Message error
                        ResponseMessage = "Error Sending Request Message",
                    };
                }
                else
                {
                    response = new RedeemVoucherResponse
                    {
                        ResponseCode = "0003", // General error
                        ResponseMessage = "General Error",
                    };
                }
            }

            return response;
        }

        public async Task<Result<List<Models.ContractResponse>>> GetMerchantContracts(Guid estateId,
                                                                                      Guid merchantId,
                                                                                      CancellationToken cancellationToken) {
            Logger.LogInformation($"GetMerchantContracts: {estateId} {merchantId}");
            // Get a client token to call the Transaction Processor
            String clientId = ConfigurationReader.GetValue("AppSettings", "ClientId");
            String clientSecret = ConfigurationReader.GetValue("AppSettings", "ClientSecret");

            Result<TokenResponse> accessTokenResult = await this.SecurityServiceClient.GetToken(clientId, clientSecret, cancellationToken);
            if (accessTokenResult.IsFailed) {
                ResultHelpers.CreateFailure(accessTokenResult);
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

                    // Leave here but not used atm
                    //foreach (TransactionProcessor.DataTransferObjects.Responses.Contract.ContractProductTransactionFee contractProductTransactionFee in contractResponseProduct.TransactionFees) {
                    //    Logger.LogInformation($"Processing contract product fee {contractProductTransactionFee.Description}");
                    //    ContractProductTransactionFee transactionFeeModel = new ContractProductTransactionFee {
                    //        Value = contractProductTransactionFee.Value,
                    //        Description = contractProductTransactionFee.Description,
                    //        CalculationType = contractProductTransactionFee.CalculationType switch {
                    //            TransactionProcessor.DataTransferObjects.Responses.Contract.CalculationType.Fixed => CalculationType.Fixed,
                    //            _ => CalculationType.Percentage,
                    //        },
                    //        FeeType = contractProductTransactionFee.FeeType switch {
                    //            TransactionProcessor.DataTransferObjects.Responses.Contract.FeeType.Merchant => FeeType.Merchant,
                    //            _ => FeeType.ServiceProvider,
                    //        },
                    //        TransactionFeeId = contractProductTransactionFee.TransactionFeeId,
                    //        TransactionFeeReportingId = contractProductTransactionFee.TransactionFeeReportingId
                    //    };
                    //    productModel.TransactionFees.Add(transactionFeeModel);
                    //}

                    contractModel.Products.Add(productModel);
                }

                models.Add(contractModel);
            }

            return Result.Success(models);
        }

        public async Task<Result<MerchantResponse>> GetMerchant(Guid estateId,
                                                                Guid merchantId,
                                                                CancellationToken cancellationToken) {
            Logger.LogWarning("in GetMerchant");
            // Get a client token to call the Transaction Processor
            String clientId = ConfigurationReader.GetValue("AppSettings", "ClientId");
            String clientSecret = ConfigurationReader.GetValue("AppSettings", "ClientSecret");

            Result<TokenResponse> accessTokenResult = await this.SecurityServiceClient.GetToken(clientId, clientSecret, cancellationToken);
            if (accessTokenResult.IsFailed) {
                return ResultHelpers.CreateFailure(accessTokenResult);
            }
            Logger.LogWarning("in GetMerchant - Got Token");
            TokenResponse accessToken = accessTokenResult.Data;

            ProcessLogonTransactionResponse response = null;

            Result<TransactionProcessor.DataTransferObjects.Responses.Merchant.MerchantResponse> result = await this.TransactionProcessorClient.GetMerchant(accessToken.AccessToken, estateId, merchantId, cancellationToken);

            if (result.IsFailed)
                return Result.Failure($"Error getting merchant {result.Message}");
            Logger.LogWarning("in GetMerchant - Got Merchant");

            Logger.LogWarning($"in GetMerchant - {JsonConvert.SerializeObject(result.Data)}");

            MerchantResponse merchantResponse = new();
            merchantResponse.MerchantId = result.Data.MerchantId;
            merchantResponse.EstateId = result.Data.EstateId;
            merchantResponse.MerchantName = result.Data.MerchantName;
            merchantResponse.EstateReportingId = result.Data.EstateReportingId;
            merchantResponse.MerchantReference = result.Data.MerchantReference;
            merchantResponse.NextStatementDate = result.Data.NextStatementDate;
            merchantResponse.SettlementSchedule = result.Data.SettlementSchedule switch {
                TransactionProcessor.DataTransferObjects.Responses.Merchant.SettlementSchedule.Immediate => Models.SettlementSchedule.Immediate,
                TransactionProcessor.DataTransferObjects.Responses.Merchant.SettlementSchedule.Weekly => SettlementSchedule.Weekly,
                TransactionProcessor.DataTransferObjects.Responses.Merchant.SettlementSchedule.Monthly => SettlementSchedule.Monthly,
                _ => SettlementSchedule.NotSet
            };
            merchantResponse.Contracts = new();
            merchantResponse.Contacts = new();
            merchantResponse.Addresses = new();
            merchantResponse.Devices = new();
            merchantResponse.Operators = new();

            if (result.Data.Addresses != null) {
                foreach (TransactionProcessor.DataTransferObjects.Responses.Merchant.AddressResponse address in result.Data.Addresses) {
                    AddressResponse addressResponse = new() {
                        AddressId = address.AddressId,
                        AddressLine1 = address.AddressLine1,
                        AddressLine2 = address.AddressLine2,
                        AddressLine3 = address.AddressLine3,
                        AddressLine4 = address.AddressLine4,
                        Country = address.Country,
                        PostalCode = address.PostalCode,
                        Region = address.Region,
                        Town = address.Town
                    };
                    merchantResponse.Addresses.Add(addressResponse);
                }
            }

            if (result.Data.Contacts != null) {
                foreach (TransactionProcessor.DataTransferObjects.Responses.Contract.ContactResponse contact in result.Data.Contacts) {
                    merchantResponse.Contacts.Add(new ContactResponse { ContactId = contact.ContactId, ContactName = contact.ContactName, ContactPhoneNumber = contact.ContactPhoneNumber, ContactEmailAddress = contact.ContactEmailAddress });
                }
            }

            if (result.Data.Contracts != null) {

                foreach (var merchantContract in result.Data.Contracts) {
                    var contract = new MerchantContractResponse { ContractId = merchantContract.ContractId, IsDeleted = merchantContract.IsDeleted, ContractProducts = new() };
                    foreach (Guid contractProduct in merchantContract.ContractProducts) {
                        contract.ContractProducts.Add(contractProduct);
                    }

                    merchantResponse.Contracts.Add(contract);
                }
            }

            if (result.Data.Contracts != null) {
                foreach (KeyValuePair<Guid, string> device in result.Data.Devices) {
                    merchantResponse.Devices.Add(device.Key, device.Value);
                }
            }

            if (result.Data.Operators != null) {

                foreach (var merchantOperator in result.Data.Operators) {
                    merchantResponse.Operators.Add(new MerchantOperatorResponse {
                        OperatorId = merchantOperator.OperatorId,
                        IsDeleted = merchantOperator.IsDeleted,
                        MerchantNumber = merchantOperator.MerchantNumber,
                        Name = merchantOperator.Name,
                        TerminalNumber = merchantOperator.TerminalNumber
                    });
                }
            }

            return merchantResponse;
        }

        #endregion
    }
}