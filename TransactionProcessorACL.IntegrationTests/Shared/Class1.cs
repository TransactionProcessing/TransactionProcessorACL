using EstateManagement.IntegrationTesting.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechTalk.SpecFlow;
using TransactionProcessor.DataTransferObjects;

namespace TransactionProcessorACL.IntegrationTests.Shared
{
    using DataTransferObjects;
    using Newtonsoft.Json;
    using System.Net.Http.Headers;
    using System.Net.Http;
    using System.Threading;
    using DataTransferObjects.Responses;
    using SecurityService.DataTransferObjects;
    using Shouldly;
    using TransactionProcessor.Client;
    using TransactionProcessor.IntegrationTests.Common;
    using static TransactionProcessorACL.IntegrationTests.Shared.SpecflowExtensions;

    public static class SpecflowExtensions
    {
        public static List<CreateUserRequest> ToAclCreateUserRequests(this TableRows tableRows, List<EstateDetails1> estateDetailsList){
            List<CreateUserRequest> requests = new List<CreateUserRequest>();

            foreach (TableRow tableRow in tableRows)
            {
                Dictionary<String, String> userClaims = new Dictionary<String, String>();
                String estateName = SpecflowTableHelper.GetStringRowValue(tableRow, "EstateName");
                EstateDetails1 es1 = estateDetailsList.SingleOrDefault(e => e.EstateDetails.EstateName == estateName);
                es1.ShouldNotBeNull();
                userClaims.Add("estateId", es1.EstateDetails.EstateId.ToString());
                userClaims.Add("contractId", es1.EstateDetails.EstateId.ToString());

                String roles = SpecflowTableHelper.GetStringRowValue(tableRow, "Roles");

                CreateUserRequest createUserRequest = new CreateUserRequest
                {
                    EmailAddress = SpecflowTableHelper.GetStringRowValue(tableRow, "Email Address"),
                    FamilyName = SpecflowTableHelper.GetStringRowValue(tableRow, "Family Name"),
                    GivenName = SpecflowTableHelper.GetStringRowValue(tableRow, "Given Name"),
                    PhoneNumber = SpecflowTableHelper.GetStringRowValue(tableRow, "Phone Number"),
                    MiddleName = SpecflowTableHelper.GetStringRowValue(tableRow, "Middle name"),
                    Claims = userClaims,
                    Roles = string.IsNullOrEmpty(roles) ? null : roles.Split(",").ToList(),
                    Password = SpecflowTableHelper.GetStringRowValue(tableRow, "Password")
                };

                requests.Add(createUserRequest);
            }

            return requests;
        }

        public static List<(EstateDetails,String, Guid, String, TransactionRequestMessage)> ToACLSerialisedMessages(String applicationVersion, List<(EstateDetails, Guid, String, SerialisedMessage)> serialisedMessages, List<EstateDetails1> estateDetailsList){
            List<(EstateDetails,String, Guid, String,TransactionRequestMessage)> aclRequestMessages = new List<(EstateDetails,String, Guid, String, TransactionRequestMessage)>();
            foreach ((EstateDetails, Guid, String, SerialisedMessage) sm in serialisedMessages){
                EstateDetails1 es1 = estateDetailsList.SingleOrDefault(e => e.EstateDetails.EstateId == sm.Item1.EstateId);
                es1.ShouldNotBeNull();
                String merchantToken = es1.GetMerchantUserToken(sm.Item2);
                
                Object x = JsonConvert.DeserializeObject(sm.Item4.SerialisedData, new JsonSerializerSettings{
                                                                                                                TypeNameHandling = TypeNameHandling.All
                                                                                                            });
                TransactionRequestMessage transactionRequest = null;
                if (x.GetType() == typeof(LogonTransactionRequest)){
                    LogonTransactionRequest logonRequest = (LogonTransactionRequest)x;
                    transactionRequest = new LogonTransactionRequestMessage{
                                                                               ApplicationVersion = applicationVersion,
                                                                               DeviceIdentifier = logonRequest.DeviceIdentifier,
                                                                               TransactionDateTime = logonRequest.TransactionDateTime,
                                                                               TransactionNumber = logonRequest.TransactionNumber
                                                                           };

                }

                if (x.GetType() == typeof(SaleTransactionRequest)){
                    SaleTransactionRequest saleRequest = (SaleTransactionRequest)x;
                    transactionRequest = new SaleTransactionRequestMessage{
                                                                               TransactionNumber = saleRequest.TransactionNumber,
                                                                               ApplicationVersion = applicationVersion,
                                                                               DeviceIdentifier = saleRequest.DeviceIdentifier,
                                                                               TransactionDateTime = saleRequest.TransactionDateTime,
                                                                               AdditionalRequestMetaData = saleRequest.AdditionalTransactionMetadata,
                                                                               ContractId = saleRequest.ContractId,
                                                                               CustomerEmailAddress = saleRequest.CustomerEmailAddress,
                                                                               OperatorIdentifier = saleRequest.OperatorIdentifier,
                                                                               ProductId = saleRequest.ProductId,
                                                                           };
                }

                if (x.GetType() == typeof(ReconciliationRequest)){
                    ReconciliationRequest reconciliation = (ReconciliationRequest)x;
                    transactionRequest = new ReconciliationRequestMessage{
                                                                             TransactionDateTime = reconciliation.TransactionDateTime,
                                                                             DeviceIdentifier = reconciliation.DeviceIdentifier,
                                                                             TransactionValue = reconciliation.TransactionValue,
                                                                             TransactionCount = reconciliation.TransactionCount,
                                                                             ApplicationVersion = applicationVersion,
                                                                             TransactionNumber = sm.Item3
                                                                         };

                    if (reconciliation.OperatorTotals != null){
                        // This is nasty so very sorry...
                        ((ReconciliationRequestMessage)transactionRequest).OperatorTotals = reconciliation.OperatorTotals.Select(o => new OperatorTotalRequest

                                                                                                                                      {
                                                                                                                                          ContractId = o.ContractId,
                                                                                                                                          OperatorIdentifier = o.OperatorIdentifier,
                                                                                                                                          TransactionCount = o.TransactionCount,
                                                                                                                                          TransactionValue = o.TransactionValue,
                                                                                                                                      }).ToList();

                    }

                }

                aclRequestMessages.Add((es1.EstateDetails, merchantToken, sm.Item2, sm.Item3, transactionRequest));

            }

            return aclRequestMessages;
        }

        public static List<ExpectedTransactionResponse> ToExpectedTransactionResponseDetails(this TableRows tableRows, List<EstateDetails1> estateDetailsList)
        {
            List<ExpectedTransactionResponse> expectedTransactionResponses = new List<ExpectedTransactionResponse>();
            List<(String responseCode, String responseMessage)> responseMessages = new List<(String responseCode, String responseMessage)>();
            foreach (TableRow tableRow in tableRows){
                ExpectedTransactionResponse expectedTransactionResponse = new ExpectedTransactionResponse{
                                                                                                             EstateName = SpecflowTableHelper.GetStringRowValue(tableRow, "EstateName"),
                                                                                                             TransactionNumber = SpecflowTableHelper.GetStringRowValue(tableRow, "TransactionNumber"),
                                                                                                             MerchantName = SpecflowTableHelper.GetStringRowValue(tableRow, "MerchantName"),
                                                                                                             ResponseCode = SpecflowTableHelper.GetStringRowValue(tableRow, "ResponseCode"),
                                                                                                             ResponseMessage = SpecflowTableHelper.GetStringRowValue(tableRow, "ResponseMessage"),
                                                                                                             TransactionType = SpecflowTableHelper.GetStringRowValue(tableRow, "TransactionType")
                                                                                                         };
                EstateDetails1 es1 = estateDetailsList.SingleOrDefault(e => e.EstateDetails.EstateName == expectedTransactionResponse.EstateName);
                es1.ShouldNotBeNull();
                expectedTransactionResponse.MerchantId = es1.EstateDetails.GetMerchantId(expectedTransactionResponse.MerchantName);
                expectedTransactionResponses.Add(expectedTransactionResponse);
            }
            return expectedTransactionResponses;
        }

        public static List<ExpectedReconciliationResponse> ToExpectedReconciliationResponseDetails(this TableRows tableRows, List<EstateDetails1> estateDetailsList)
        {
            List<ExpectedReconciliationResponse> expectedReconciliationResponses = new List<ExpectedReconciliationResponse>();
            List<(String responseCode, String responseMessage)> responseMessages = new List<(String responseCode, String responseMessage)>();
            foreach (TableRow tableRow in tableRows)
            {
                ExpectedReconciliationResponse expectedTransactionResponse = new ExpectedReconciliationResponse
                {
                                                                                 EstateName = SpecflowTableHelper.GetStringRowValue(tableRow, "EstateName"),
                                                                                 MerchantName = SpecflowTableHelper.GetStringRowValue(tableRow, "MerchantName"),
                                                                                 ResponseCode = SpecflowTableHelper.GetStringRowValue(tableRow, "ResponseCode"),
                                                                                 ResponseMessage = SpecflowTableHelper.GetStringRowValue(tableRow, "ResponseMessage"),
                                                                                 TransactionNumber = SpecflowTableHelper.GetStringRowValue(tableRow, "TransactionNumber"),
                };
                EstateDetails1 es1 = estateDetailsList.SingleOrDefault(e => e.EstateDetails.EstateName == expectedTransactionResponse.EstateName);
                es1.ShouldNotBeNull();
                expectedTransactionResponse.MerchantId = es1.EstateDetails.GetMerchantId(expectedTransactionResponse.MerchantName);
                expectedReconciliationResponses.Add(expectedTransactionResponse);
            }
            return expectedReconciliationResponses;
        }

        public class ExpectedTransactionResponse{
            public String ResponseCode{ get; set; }
            public String ResponseMessage { get; set; }
            public Guid MerchantId { get; set; }
            public String EstateName { get; set; }
            public String MerchantName { get; set; }
            public String TransactionNumber { get; set; }
            public String TransactionType { get; set; }
        }

        public class ExpectedReconciliationResponse
        {
            public String ResponseCode { get; set; }
            public String ResponseMessage { get; set; }
            public Guid MerchantId { get; set; }
            public String EstateName { get; set; }
            public String MerchantName { get; set; }
            public String TransactionNumber { get; set; }
        }
    }

    public class ACLSteps{
        private readonly HttpClient HttpClient;

        private readonly ITransactionProcessorClient TransactionProcessorClient;

        public ACLSteps(HttpClient httpClient, ITransactionProcessorClient transactionProcessorClient){
            this.HttpClient = httpClient;
            this.TransactionProcessorClient = transactionProcessorClient;
        }

        public async Task SendAclRequestMessage((EstateDetails, String, Guid, String, TransactionRequestMessage) requestMessage, CancellationToken cancellationToken){
            String uri = "api/transactions";

            StringContent content = new StringContent(JsonConvert.SerializeObject(requestMessage.Item5,
                                                                                  new JsonSerializerSettings
                                                                                  {
                                                                                      TypeNameHandling = TypeNameHandling.All
                                                                                  }),
                                                      Encoding.UTF8,
                                                      "application/json");

            this.HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", requestMessage.Item2);

            HttpResponseMessage response = await this.HttpClient.PostAsync(uri, content, cancellationToken).ConfigureAwait(false);

            response.IsSuccessStatusCode.ShouldBeTrue();

            String responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            responseContent.ShouldNotBeNullOrEmpty("No response message received");

            requestMessage.Item1.AddTransactionResponse(requestMessage.Item3, requestMessage.Item4, responseContent);
        }

        public void ThenTheLogonTransactionResponseShouldContainTheFollowingInformation(List<SpecflowExtensions.ExpectedTransactionResponse> expectedResponses, List<EstateDetails1> estateDetailsList)
        {
            foreach (ExpectedTransactionResponse expectedTransactionResponse in expectedResponses){
                EstateDetails1 es1 = estateDetailsList.SingleOrDefault(e => e.EstateDetails.EstateName == expectedTransactionResponse.EstateName);
                es1.ShouldNotBeNull();
                String responseMessage = es1.EstateDetails.GetTransactionResponse(expectedTransactionResponse.MerchantId, expectedTransactionResponse.TransactionNumber);

                LogonTransactionResponseMessage transactionResponse = JsonConvert.DeserializeObject<LogonTransactionResponseMessage>(responseMessage);
                transactionResponse.ResponseCode.ShouldBe(expectedTransactionResponse.ResponseCode);
                transactionResponse.ResponseMessage.ShouldBe(expectedTransactionResponse.ResponseMessage);
            }
        }

        public void ThenTheSaleTransactionResponseShouldContainTheFollowingInformation(List<ExpectedTransactionResponse> expectedResponses, List<EstateDetails1> estateDetailsList)
        {
            foreach (ExpectedTransactionResponse expectedTransactionResponse in expectedResponses)
            {
                EstateDetails1 es1 = estateDetailsList.SingleOrDefault(e => e.EstateDetails.EstateName == expectedTransactionResponse.EstateName);
                es1.ShouldNotBeNull();
                String responseMessage = es1.EstateDetails.GetTransactionResponse(expectedTransactionResponse.MerchantId, expectedTransactionResponse.TransactionNumber);

                SaleTransactionResponseMessage transactionResponse = JsonConvert.DeserializeObject<SaleTransactionResponseMessage>(responseMessage);
                transactionResponse.ResponseCode.ShouldBe(expectedTransactionResponse.ResponseCode);
                transactionResponse.ResponseMessage.ShouldBe(expectedTransactionResponse.ResponseMessage);
            }
        }

        public void ThenReconciliationResponseShouldContainTheFollowingInformation(List<ExpectedReconciliationResponse> expectedResponses, List<EstateDetails1> estateDetailsList){
            foreach (ExpectedReconciliationResponse expectedReconciliationResponse  in expectedResponses)
            {
                EstateDetails1 es1 = estateDetailsList.SingleOrDefault(e => e.EstateDetails.EstateName == expectedReconciliationResponse.EstateName);
                es1.ShouldNotBeNull();
                String responseMessage = es1.EstateDetails.GetTransactionResponse(expectedReconciliationResponse.MerchantId, expectedReconciliationResponse.TransactionNumber);

                ReconciliationResponseMessage reconciliationResponse= JsonConvert.DeserializeObject<ReconciliationResponseMessage>(responseMessage);
                reconciliationResponse.ResponseCode.ShouldBe(expectedReconciliationResponse.ResponseCode);
                reconciliationResponse.ResponseMessage.ShouldBe(expectedReconciliationResponse.ResponseMessage);
            }
        }

        public async Task WhenIRedeemTheVoucherForEstateAndMerchantTransactionNumberTheVoucherBalanceWillBe(String accessToken, String estateName, String merchantName, Int32 transactionNumber, Int32 balance, List<EstateDetails1> estateDetailsList){
            EstateDetails1 es1 = estateDetailsList.SingleOrDefault(e => e.EstateDetails.EstateName == estateName);
             es1.ShouldNotBeNull();
                Guid merchantId = es1.EstateDetails.GetMerchantId(merchantName);
                String serialisedMessage = es1.EstateDetails.GetTransactionResponse(merchantId, transactionNumber.ToString());
                SaleTransactionResponse transactionResponse = JsonConvert.DeserializeObject<SaleTransactionResponse>(serialisedMessage,
                                                                                                                     new JsonSerializerSettings
                                                                                                                     {
                                                                                                                         TypeNameHandling = TypeNameHandling.All
                                                                                                                     });
                GetVoucherResponse voucher = null;
                await Retry.For(async () => {

                                    voucher = await this.TransactionProcessorClient.GetVoucherByTransactionId(accessToken,
                                                                                                                                 es1.EstateDetails.EstateId,
                                                                                                                                 transactionResponse.TransactionId,
                                                                                                                                 CancellationToken.None);
                                    voucher.ShouldNotBeNull();
                                });
                
                // Build URI 
                String uri = $"api/vouchers?applicationVersion=1.0.5&voucherCode={voucher.VoucherCode}";

                String voucherAccessToken = es1.GetVoucherRedemptionUserToken("Voucher");
                
                StringContent content = new StringContent(String.Empty);

                this.HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", voucherAccessToken);

                HttpResponseMessage response = await this.HttpClient.PutAsync(uri, content, CancellationToken.None).ConfigureAwait(false);

                response.IsSuccessStatusCode.ShouldBeTrue();

                RedeemVoucherResponseMessage redeemVoucherResponse = JsonConvert.DeserializeObject<RedeemVoucherResponseMessage>(await response.Content.ReadAsStringAsync().ConfigureAwait(false));

                redeemVoucherResponse.Balance.ShouldBe(balance);
        }
    }
}
