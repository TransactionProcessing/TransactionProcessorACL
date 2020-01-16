namespace TransactionProcessorACL.BusinessLogic.Services
{
    using System;
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

    public class TransactionProcessorACLApplicationService : ITransactionProcessorACLApplicationService
    {
        private readonly ITransactionProcessorClient TransactionProcessorClient;

        private readonly ISecurityServiceClient SecurityServiceClient;

        public TransactionProcessorACLApplicationService(ITransactionProcessorClient transactionProcessorClient, ISecurityServiceClient securityServiceClient)
        {
            this.TransactionProcessorClient = transactionProcessorClient;
            this.SecurityServiceClient = securityServiceClient;
        }

        public async Task<ProcessLogonTransactionResponse> ProcessLogonTransaction(Guid estateId,
                                                                                   Guid merchantId,
                                                                                   DateTime transactionDateTime,
                                                                                   String transactionNumber,
                                                                                   String imeiNumber,
                                                                                   CancellationToken cancellationToken)
        {
            // Get a client token to call the Transaction Processor
            String clientId = ConfigurationReader.GetValue("AppSettings", "ClientId");
            String clientSecret = ConfigurationReader.GetValue("AppSettings", "ClientSecret");

            TokenResponse accessToken = await this.SecurityServiceClient.GetToken(clientId, clientSecret, cancellationToken);

            LogonTransactionRequest logonTransactionRequest = new LogonTransactionRequest();
            logonTransactionRequest.TransactionNumber = transactionNumber;
            logonTransactionRequest.IMEINumber = imeiNumber;
            logonTransactionRequest.TransactionDateTime = transactionDateTime;
            logonTransactionRequest.TransactionType = "LOGON";

            SerialisedMessage requestSerialisedMessage = new SerialisedMessage();
            requestSerialisedMessage.Metadata.Add("EstateId", estateId.ToString());
            requestSerialisedMessage.Metadata.Add("MerchantId", merchantId.ToString());
            requestSerialisedMessage.SerialisedData = JsonConvert.SerializeObject(logonTransactionRequest, new JsonSerializerSettings
                                                                                                           {
                                                                                                               TypeNameHandling = TypeNameHandling.All
                                                                                                           });
            ProcessLogonTransactionResponse response = new ProcessLogonTransactionResponse();

            try
            {
                SerialisedMessage responseSerialisedMessage =
                    await this.TransactionProcessorClient.PerformTransaction(accessToken.AccessToken, requestSerialisedMessage, cancellationToken);

                LogonTransactionResponse logonTransactionResponse = JsonConvert.DeserializeObject<LogonTransactionResponse>(responseSerialisedMessage.SerialisedData);

                response = new ProcessLogonTransactionResponse
                           {
                               ResponseCode = logonTransactionResponse.ResponseCode,
                               ResponseMessage = logonTransactionResponse.ResponseMessage
                           };
            }
            catch(Exception ex)
            {
                if (ex.InnerException is InvalidOperationException)
                {
                    // This means there is an error in the request
                    response.ResponseCode = "0001"; // Request Message error
                    response.ResponseMessage = ex.InnerException.Message;
                }
            }

            return response;
        }
    }
}