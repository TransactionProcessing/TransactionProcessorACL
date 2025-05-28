﻿using System.Diagnostics;
using SimpleResults;
using TransactionProcessor.IntegrationTesting.Helpers;

namespace TransactionProcessorACL.IntegrationTests.Shared;

using DataTransferObjects;
using DataTransferObjects.Responses;
using Newtonsoft.Json;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TransactionProcessor.Client;
using TransactionProcessor.DataTransferObjects;
using TransactionProcessor.IntegrationTests.Common;
using static TransactionProcessorACL.IntegrationTests.Shared.ReqnrollExtensions;

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

                            Result<GetVoucherResponse> voucherResult = await this.TransactionProcessorClient.GetVoucherByTransactionId(accessToken,
                                                                                                      es1.EstateDetails.EstateId,
                                                                                                      transactionResponse.TransactionId,
                                                                                                      CancellationToken.None);
                            voucherResult.IsSuccess.ShouldBeTrue("Failed to retrieve voucher by transaction id");
                            voucher = voucherResult.Data;
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

    public async Task WhenIGetTheMerchantInformationForMerchantForEstateTheResponseShouldContainTheFollowingInformation(String estateName,
                                                                                                                        String merchantName, List<EstateDetails1> estateDetailsList,
                                                                                                                        ExpectedMerchantResponse expectedMerchantResponse) {
        EstateDetails1 es1 = estateDetailsList.SingleOrDefault(e => e.EstateDetails.EstateName == estateName);
        es1.ShouldNotBeNull();
        Guid merchantId = es1.EstateDetails.GetMerchantId(merchantName);

        // Build URI 
        String uri = $"api/merchants?applicationVersion=1.0.5";

        String userAccessToken = es1.GetMerchantUserToken(merchantId);
        
        this.HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", userAccessToken);

        HttpResponseMessage response = await this.HttpClient.GetAsync(uri, CancellationToken.None);

        response.IsSuccessStatusCode.ShouldBeTrue();
        String responseContent = await response.Content.ReadAsStringAsync();
        

        Result<MerchantResponse> merchantResponseResult = JsonConvert.DeserializeObject<Result<MerchantResponse>>(responseContent);
        merchantResponseResult.IsSuccess.ShouldBeTrue("Failed to retrieve merchant information");
        MerchantResponse merchantResponse = merchantResponseResult.Data;
        merchantResponse.ShouldNotBeNull();
        merchantResponse.MerchantId.ShouldBe(merchantId);
        merchantResponse.MerchantName.ShouldBe(expectedMerchantResponse.MerchantName);
        merchantResponse.Addresses.ShouldNotBeNull();
        merchantResponse.Addresses.Count.ShouldBe(1);
        merchantResponse.Addresses[0].AddressLine1.ShouldBe(expectedMerchantResponse.AddressLine1);
        merchantResponse.Addresses[0].Town.ShouldBe(expectedMerchantResponse.Town);
        merchantResponse.Addresses[0].Region.ShouldBe(expectedMerchantResponse.Region);
        merchantResponse.Addresses[0].Country.ShouldBe(expectedMerchantResponse.Country);
        merchantResponse.Contacts.ShouldNotBeNull();
        merchantResponse.Contacts.Count.ShouldBe(1);
        merchantResponse.Contacts[0].ContactName.ShouldBe(expectedMerchantResponse.ContactName);
        merchantResponse.Contacts[0].ContactEmailAddress.ShouldBe(expectedMerchantResponse.EmailAddress);


    }

    public async Task WhenIGetTheMerchantContractInformationForMerchantForEstateTheResponseShouldContainTheFollowingInformation(String estateName,
                                                                                                                                String merchantName,
                                                                                                                                List<EstateDetails1> estateDetailsList,
                                                                                                                                List<ExpectedMerchantContractResponse> expectedMerchantContractResponses) {
        EstateDetails1 es1 = estateDetailsList.SingleOrDefault(e => e.EstateDetails.EstateName == estateName);
        es1.ShouldNotBeNull();
        Guid merchantId = es1.EstateDetails.GetMerchantId(merchantName);

        // Build URI 
        String uri = $"api/merchants/contracts?applicationVersion=1.0.5";

        String userAccessToken = es1.GetMerchantUserToken(merchantId);

        this.HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", userAccessToken);

        await Retry.For(async () => {
            HttpResponseMessage response = await this.HttpClient.GetAsync(uri, CancellationToken.None);
            response.IsSuccessStatusCode.ShouldBeTrue();

            String responseContent = await response.Content.ReadAsStringAsync();

            Result<List<ContractResponse>> merchantContractResponseResult = JsonConvert.DeserializeObject<Result<List<ContractResponse>>>(responseContent);
            merchantContractResponseResult.IsSuccess.ShouldBeTrue("Failed to retrieve merchant contract information");

            foreach (ExpectedMerchantContractResponse expectedMerchantContractResponse in expectedMerchantContractResponses) {
                ContractResponse contractResponse = merchantContractResponseResult.Data.SingleOrDefault(c => c.Description == expectedMerchantContractResponse.ContractName);
                contractResponse.ShouldNotBeNull($"Failed to find contract {expectedMerchantContractResponse.ContractName} in response");
            }
        });
    }
}