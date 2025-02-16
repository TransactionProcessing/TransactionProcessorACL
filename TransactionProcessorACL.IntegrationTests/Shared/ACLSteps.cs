using TransactionProcessor.IntegrationTesting.Helpers;

namespace TransactionProcessorACL.IntegrationTests.Shared;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DataTransferObjects;
using DataTransferObjects.Responses;
using Newtonsoft.Json;
using Shouldly;
using TransactionProcessor.Client;
using TransactionProcessor.DataTransferObjects;
using TransactionProcessor.IntegrationTests.Common;

public class ACLSteps{
    private readonly HttpClient HttpClient;

    private readonly ITransactionProcessorClient TransactionProcessorClient;

    public ACLSteps(HttpClient httpClient, ITransactionProcessorClient transactionProcessorClient){
        this.HttpClient = httpClient;
        this.TransactionProcessorClient = transactionProcessorClient;
    }

    internal class ResponseData<T>
    {
        public T Data { get; set; }
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
        
        ResponseData<TransactionResponseMessage> responseData =
            JsonConvert.DeserializeObject<ResponseData<TransactionResponseMessage>>(responseContent);

        String responseMessage = JsonConvert.SerializeObject(responseData.Data);

        requestMessage.Item1.AddTransactionResponse(requestMessage.Item3, requestMessage.Item4, responseMessage);
    }

    public void ThenTheLogonTransactionResponseShouldContainTheFollowingInformation(List<ReqnrollExtensions.ExpectedTransactionResponse> expectedResponses, List<EstateDetails1> estateDetailsList)
    {
        foreach (ReqnrollExtensions.ExpectedTransactionResponse expectedTransactionResponse in expectedResponses){
            EstateDetails1 es1 = estateDetailsList.SingleOrDefault(e => e.EstateDetails.EstateName == expectedTransactionResponse.EstateName);
            es1.ShouldNotBeNull();
            String responseMessage = es1.EstateDetails.GetTransactionResponse(expectedTransactionResponse.MerchantId, expectedTransactionResponse.TransactionNumber);

            LogonTransactionResponseMessage transactionResponse = JsonConvert.DeserializeObject<LogonTransactionResponseMessage>(responseMessage);
            transactionResponse.ResponseCode.ShouldBe(expectedTransactionResponse.ResponseCode);
            transactionResponse.ResponseMessage.ShouldBe(expectedTransactionResponse.ResponseMessage);
        }
    }

    public void ThenTheSaleTransactionResponseShouldContainTheFollowingInformation(List<ReqnrollExtensions.ExpectedTransactionResponse> expectedResponses, List<EstateDetails1> estateDetailsList)
    {
        foreach (ReqnrollExtensions.ExpectedTransactionResponse expectedTransactionResponse in expectedResponses)
        {
            EstateDetails1 es1 = estateDetailsList.SingleOrDefault(e => e.EstateDetails.EstateName == expectedTransactionResponse.EstateName);
            es1.ShouldNotBeNull();
            String responseMessage = es1.EstateDetails.GetTransactionResponse(expectedTransactionResponse.MerchantId, expectedTransactionResponse.TransactionNumber);

            SaleTransactionResponseMessage transactionResponse = JsonConvert.DeserializeObject<SaleTransactionResponseMessage>(responseMessage);
            transactionResponse.ResponseCode.ShouldBe(expectedTransactionResponse.ResponseCode);
            transactionResponse.ResponseMessage.ShouldBe(expectedTransactionResponse.ResponseMessage);
        }
    }

    public void ThenReconciliationResponseShouldContainTheFollowingInformation(List<ReqnrollExtensions.ExpectedReconciliationResponse> expectedResponses, List<EstateDetails1> estateDetailsList){
        foreach (ReqnrollExtensions.ExpectedReconciliationResponse expectedReconciliationResponse  in expectedResponses)
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