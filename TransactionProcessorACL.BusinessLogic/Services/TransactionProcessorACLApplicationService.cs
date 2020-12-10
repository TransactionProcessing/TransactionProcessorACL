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
    using TransactionProcessor.Client;
    using TransactionProcessor.DataTransferObjects;

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
        public async Task<ProcessLogonTransactionResponse> ProcessLogonTransaction(Guid estateId,
                                                                                   Guid merchantId,
                                                                                   DateTime transactionDateTime,
                                                                                   String transactionNumber,
                                                                                   String deviceIdentifier,
                                                                                   CancellationToken cancellationToken)
        {
            // Get a client token to call the Transaction Processor
            String clientId = ConfigurationReader.GetValue("AppSettings", "ClientId");
            String clientSecret = ConfigurationReader.GetValue("AppSettings", "ClientSecret");

            TokenResponse accessToken = await this.SecurityServiceClient.GetToken(clientId, clientSecret, cancellationToken);

            LogonTransactionRequest logonTransactionRequest = new LogonTransactionRequest();
            logonTransactionRequest.TransactionNumber = transactionNumber;
            logonTransactionRequest.DeviceIdentifier = deviceIdentifier;
            logonTransactionRequest.TransactionDateTime = transactionDateTime;
            logonTransactionRequest.TransactionType = "LOGON";

            SerialisedMessage requestSerialisedMessage = new SerialisedMessage();
            requestSerialisedMessage.Metadata.Add("EstateId", estateId.ToString());
            requestSerialisedMessage.Metadata.Add("MerchantId", merchantId.ToString());
            requestSerialisedMessage.SerialisedData = JsonConvert.SerializeObject(logonTransactionRequest,
                                                                                  new JsonSerializerSettings
                                                                                  {
                                                                                      TypeNameHandling = TypeNameHandling.All
                                                                                  });

            ProcessLogonTransactionResponse response = null;

            try
            {
                SerialisedMessage responseSerialisedMessage =
                    await this.TransactionProcessorClient.PerformTransaction(accessToken.AccessToken, requestSerialisedMessage, cancellationToken);

                LogonTransactionResponse logonTransactionResponse = JsonConvert.DeserializeObject<LogonTransactionResponse>(responseSerialisedMessage.SerialisedData);

                response = new ProcessLogonTransactionResponse
                           {
                               ResponseCode = logonTransactionResponse.ResponseCode,
                               ResponseMessage = logonTransactionResponse.ResponseMessage,
                               EstateId = estateId,
                               MerchantId = merchantId
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
                    // Request Send Exception
                    response = new ProcessLogonTransactionResponse
                    {
                                   ResponseCode = "0002", // Request Message error
                                   ResponseMessage = "Error Sending Request Message",
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

            return response;
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
        public async Task<ProcessSaleTransactionResponse> ProcessSaleTransaction(Guid estateId,
                                                                                   Guid merchantId,
                                                                                   DateTime transactionDateTime,
                                                                                   String transactionNumber,
                                                                                   String deviceIdentifier,
                                                                                   String operatorIdentifier,
                                                                                   String customerEmailAddress,
                                                                                   Guid contractId,
                                                                                   Guid productId,
                                                                                   Dictionary<String, String> additionalRequestMetadata,
                                                                                   CancellationToken cancellationToken)
        {
            // Get a client token to call the Transaction Processor
            String clientId = ConfigurationReader.GetValue("AppSettings", "ClientId");
            String clientSecret = ConfigurationReader.GetValue("AppSettings", "ClientSecret");

            TokenResponse accessToken = await this.SecurityServiceClient.GetToken(clientId, clientSecret, cancellationToken);

            SaleTransactionRequest saleTransactionRequest = new SaleTransactionRequest();
            saleTransactionRequest.TransactionNumber = transactionNumber;
            saleTransactionRequest.DeviceIdentifier = deviceIdentifier;
            saleTransactionRequest.TransactionDateTime = transactionDateTime;
            saleTransactionRequest.TransactionType = "SALE";
            saleTransactionRequest.OperatorIdentifier = operatorIdentifier;
            saleTransactionRequest.CustomerEmailAddress = customerEmailAddress;

            // Set the product information
            saleTransactionRequest.ContractId = contractId;
            saleTransactionRequest.ProductId = productId;

            // Build up the metadata
            saleTransactionRequest.AdditionalTransactionMetadata = additionalRequestMetadata;
            
            SerialisedMessage requestSerialisedMessage = new SerialisedMessage();
            requestSerialisedMessage.Metadata.Add("EstateId", estateId.ToString());
            requestSerialisedMessage.Metadata.Add("MerchantId", merchantId.ToString());
            requestSerialisedMessage.SerialisedData = JsonConvert.SerializeObject(saleTransactionRequest,
                                                                                  new JsonSerializerSettings
                                                                                  {
                                                                                      TypeNameHandling = TypeNameHandling.All
                                                                                  });

            ProcessSaleTransactionResponse response = null;

            try
            {
                SerialisedMessage responseSerialisedMessage =
                    await this.TransactionProcessorClient.PerformTransaction(accessToken.AccessToken, requestSerialisedMessage, cancellationToken);

                SaleTransactionResponse saleTransactionResponse = JsonConvert.DeserializeObject<SaleTransactionResponse>(responseSerialisedMessage.SerialisedData);

                response = new ProcessSaleTransactionResponse
                {
                    ResponseCode = saleTransactionResponse.ResponseCode,
                    ResponseMessage = saleTransactionResponse.ResponseMessage,
                    EstateId = estateId,
                    MerchantId = merchantId,
                    AdditionalTransactionMetadata = saleTransactionResponse.AdditionalTransactionMetadata
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

            return response;
        }

        public async Task<ProcessReconciliationResponse> ProcessReconciliation(Guid estateId,
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

            TokenResponse accessToken = await this.SecurityServiceClient.GetToken(clientId, clientSecret, cancellationToken);

            ReconciliationRequest reconciliationRequest = new ReconciliationRequest();
            reconciliationRequest.DeviceIdentifier = deviceIdentifier;
            reconciliationRequest.TransactionDateTime = transactionDateTime;
            reconciliationRequest.TransactionCount = transactionCount;
            reconciliationRequest.TransactionValue = transactionValue;

            SerialisedMessage requestSerialisedMessage = new SerialisedMessage();
            requestSerialisedMessage.Metadata.Add("EstateId", estateId.ToString());
            requestSerialisedMessage.Metadata.Add("MerchantId", merchantId.ToString());
            requestSerialisedMessage.SerialisedData = JsonConvert.SerializeObject(reconciliationRequest,
                                                                                  new JsonSerializerSettings
                                                                                  {
                                                                                      TypeNameHandling = TypeNameHandling.All
                                                                                  });

            ProcessReconciliationResponse response = null;

            try
            {
                SerialisedMessage responseSerialisedMessage =
                    await this.TransactionProcessorClient.PerformTransaction(accessToken.AccessToken, requestSerialisedMessage, cancellationToken);

                ReconciliationResponse reconciliationResponse = JsonConvert.DeserializeObject<ReconciliationResponse>(responseSerialisedMessage.SerialisedData);

                response = new ProcessReconciliationResponse
                {
                    ResponseCode = reconciliationResponse.ResponseCode,
                    ResponseMessage = reconciliationResponse.ResponseMessage
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

            return response;
        }

        #endregion
    }
}