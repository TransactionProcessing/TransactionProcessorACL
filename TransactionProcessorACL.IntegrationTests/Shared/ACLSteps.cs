using Shared.Serialisation;
using SimpleResults;
using TransactionProcessor.IntegrationTesting.Helpers;
using TransactionProcessorACL.DataTransferObjects.Requests;

namespace TransactionProcessorACL.IntegrationTests.Shared;

using DataTransferObjects;
using DataTransferObjects.Responses;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using TransactionProcessor.Client;
using TransactionProcessor.DataTransferObjects;
using TransactionProcessor.IntegrationTests.Common;
using ReportingDailyPerformanceSummaryResponse = TransactionProcessorACL.DataTransferObjects.Responses.MerchantDailyPerformanceSummaryResponse;
using ReportingMerchantTransactionMixSummaryResponse = TransactionProcessorACL.IntegrationTests.Shared.MerchantTransactionMixSummaryResponse;
using static TransactionProcessorACL.IntegrationTests.Shared.ReqnrollExtensions;

public class ACLSteps{
    private readonly HttpClient HttpClient;

    private readonly ITransactionProcessorClient TransactionProcessorClient;

    private EstateDetails1 LastMerchantDailyPerformanceSummaryEstateDetails;
    private Guid LastMerchantDailyPerformanceSummaryMerchantId;
    private EstateDetails1 LastMerchantTransactionMixSummaryEstateDetails;
    private Guid LastMerchantTransactionMixSummaryMerchantId;
    private EstateDetails1 LastRecentActivityReceiptSearchEstateDetails;
    private Guid LastRecentActivityReceiptSearchMerchantId;
    private readonly Dictionary<int, RecentActivityReceiptSearchResponse> RecentActivityReceiptSearchResponses;

    public ACLSteps(HttpClient httpClient, ITransactionProcessorClient transactionProcessorClient){
        this.HttpClient = httpClient;
        this.TransactionProcessorClient = transactionProcessorClient;
        this.RecentActivityReceiptSearchResponses = new Dictionary<int, RecentActivityReceiptSearchResponse>();
    }

    internal class ResponseData<T>
    {
        public T Data { get; set; }
    }

    public async Task SendAclSaleRequestMessage((EstateDetails, String, Guid, String, SaleTransactionRequestMessage) requestMessage,
                                                CancellationToken cancellationToken) {
        String uri = "api/saletransactions";
        
        await this.SendAclRequestMessage(uri, requestMessage.Item5, requestMessage.Item2, requestMessage.Item3, requestMessage.Item1, requestMessage.Item4, cancellationToken);
    }

    public async Task SendAclLogonRequestMessage((EstateDetails, String, Guid, String, LogonTransactionRequestMessage) requestMessage,
                                                CancellationToken cancellationToken)
    {
        String uri = "api/logontransactions";
        
        await this.SendAclRequestMessage(uri, requestMessage.Item5, requestMessage.Item2, requestMessage.Item3, requestMessage.Item1, requestMessage.Item4, cancellationToken);
    }

    public async Task SendAclReconciliationRequestMessage((EstateDetails, String, Guid, String, ReconciliationRequestMessage) requestMessage,
                                                 CancellationToken cancellationToken)
    {
        String uri = "api/reconciliationtransactions";
        
        await this.SendAclRequestMessage(uri, requestMessage.Item5, requestMessage.Item2, requestMessage.Item3, requestMessage.Item1, requestMessage.Item4, cancellationToken);
    }

    private async Task SendAclRequestMessage<T>(String uri, 
                                            T request,
                                            String accessToken,
                                            Guid merchantId,
                                            EstateDetails estateDetails,
                                            String transactionNumber,
                                            CancellationToken cancellationToken) {
        StringContent content = new StringContent(StringSerialiser.Serialise(request),
            Encoding.UTF8,
            "application/json");
        this.HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        HttpResponseMessage response = await this.HttpClient.PostAsync(uri, content, cancellationToken).ConfigureAwait(false);

        response.IsSuccessStatusCode.ShouldBeTrue();

        String responseContent = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);

        responseContent.ShouldNotBeNullOrEmpty("No response message received");

        TransactionResponse transactionResponse = StringSerialiser.Deserialise<TransactionResponse>(responseContent);

        estateDetails.AddTransactionResponse(merchantId, transactionNumber, transactionResponse);
    }
    
    public void ThenTheLogonTransactionResponseShouldContainTheFollowingInformation(List<ReqnrollExtensions.ExpectedTransactionResponse> expectedResponses, List<EstateDetails1> estateDetailsList)
    {
        foreach (ReqnrollExtensions.ExpectedTransactionResponse expectedTransactionResponse in expectedResponses){
            EstateDetails1 es1 = estateDetailsList.SingleOrDefault(e => e.EstateDetails.EstateName == expectedTransactionResponse.EstateName);
            es1.ShouldNotBeNull();
            TransactionResponse transactionResponse = es1.EstateDetails.GetTransactionResponse(expectedTransactionResponse.MerchantId, expectedTransactionResponse.TransactionNumber);

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
            TransactionResponse transactionResponse = es1.EstateDetails.GetTransactionResponse(expectedTransactionResponse.MerchantId, expectedTransactionResponse.TransactionNumber);

            transactionResponse.ResponseCode.ShouldBe(expectedTransactionResponse.ResponseCode);
            transactionResponse.ResponseMessage.ShouldBe(expectedTransactionResponse.ResponseMessage);
        }
    }

    public void ThenReconciliationResponseShouldContainTheFollowingInformation(List<ReqnrollExtensions.ExpectedReconciliationResponse> expectedResponses, List<EstateDetails1> estateDetailsList){
        foreach (ReqnrollExtensions.ExpectedReconciliationResponse expectedReconciliationResponse  in expectedResponses)
        {
            EstateDetails1 es1 = estateDetailsList.SingleOrDefault(e => e.EstateDetails.EstateName == expectedReconciliationResponse.EstateName);
            es1.ShouldNotBeNull();
            TransactionResponse transactionResponse = es1.EstateDetails.GetTransactionResponse(expectedReconciliationResponse.MerchantId, expectedReconciliationResponse.TransactionNumber);

            transactionResponse.ResponseCode.ShouldBe(expectedReconciliationResponse.ResponseCode);
            transactionResponse.ResponseMessage.ShouldBe(expectedReconciliationResponse.ResponseMessage);
        }
    }

    public async Task WhenIRedeemTheVoucherForEstateAndMerchantTransactionNumberTheVoucherBalanceWillBe(String accessToken, String estateName, String merchantName, Int32 transactionNumber, Int32 balance, List<EstateDetails1> estateDetailsList){
        EstateDetails1 es1 = estateDetailsList.SingleOrDefault(e => e.EstateDetails.EstateName == estateName);
        es1.ShouldNotBeNull();
        Guid merchantId = es1.EstateDetails.GetMerchantId(merchantName);
        TransactionResponse transactionResponse = es1.EstateDetails.GetTransactionResponse(merchantId, transactionNumber.ToString());

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

        RedeemVoucherResponseMessage redeemVoucherResponse = StringSerialiser.DeserializeObject<RedeemVoucherResponseMessage>(await response.Content.ReadAsStringAsync().ConfigureAwait(false), typeof(RedeemVoucherResponseMessage), new SerialiserOptions(SerialiserPropertyFormat.SnakeCase));

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
        

        MerchantResponse merchantResponse = StringSerialiser.Deserialise<MerchantResponse>(responseContent, new SerialiserOptions(SerialiserPropertyFormat.SnakeCase));
        merchantResponse.ShouldNotBeNull();
        merchantResponse.MerchantId.ShouldBe(merchantId);
        merchantResponse.MerchantReportingId.ShouldNotBe(0);
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

            List<ContractResponse> merchantContractResponseResult = StringSerialiser.Deserialise<List<ContractResponse>>(responseContent);

            foreach (ExpectedMerchantContractResponse expectedMerchantContractResponse in expectedMerchantContractResponses) {
                ContractResponse contractResponse = merchantContractResponseResult.SingleOrDefault(c => c.Description == expectedMerchantContractResponse.ContractName);
                contractResponse.ShouldNotBeNull($"Failed to find contract {expectedMerchantContractResponse.ContractName} in response");
            }
        });
    }

    public async Task WhenIGetTheMerchantDailyPerformanceSummaryForMerchantForEstate(String estateName,
                                                                                     String merchantName,
                                                                                     List<EstateDetails1> estateDetailsList,
                                                                                     CancellationToken cancellationToken)
    {
        EstateDetails1 es1 = estateDetailsList.SingleOrDefault(e => e.EstateDetails.EstateName == estateName);
        es1.ShouldNotBeNull();
        Guid merchantId = es1.EstateDetails.GetMerchantId(merchantName);
        Int32 merchantReportingId = es1.GetMerchantReportingId(merchantId);

        String uri = "api/reporting/dailymerchantprformancesummary";
        String userAccessToken = es1.GetMerchantUserToken(merchantId);

        this.HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", userAccessToken);

        MerchantDailyPerformanceSummaryRequest request = new()
        {
            EndDate = DateTime.Today,
            StartDate = DateTime.Today,
            ApplicationVersion = "1.0.5",
            MerchantReportingId = merchantReportingId
        };
        

        StringContent content = new StringContent(StringSerialiser.Serialise(request), Encoding.UTF8, "application/json");

        HttpResponseMessage response = await this.HttpClient.PostAsync(uri, content, cancellationToken).ConfigureAwait(false);

        response.IsSuccessStatusCode.ShouldBeTrue();

        String responseContent = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
        responseContent.ShouldNotBeNullOrEmpty("No response message received");

        ReportingDailyPerformanceSummaryResponse dailyPerformanceSummaryResponse =
            StringSerialiser.Deserialise<ReportingDailyPerformanceSummaryResponse>(responseContent,
                                                                                   new SerialiserOptions(SerialiserPropertyFormat.SnakeCase));

        es1.AddMerchantDailyPerformanceSummaryResponse(merchantId, dailyPerformanceSummaryResponse);
        this.LastMerchantDailyPerformanceSummaryEstateDetails = es1;
        this.LastMerchantDailyPerformanceSummaryMerchantId = merchantId;
    }

    public void ThenTheMerchantDailyPerformanceSummaryResponseShouldContainAtLeastOneMetricAndTheSaleAmount(Decimal saleAmount)
    {
        this.LastMerchantDailyPerformanceSummaryEstateDetails.ShouldNotBeNull();

        ReportingDailyPerformanceSummaryResponse merchantDailyPerformanceSummaryResponse =
            this.LastMerchantDailyPerformanceSummaryEstateDetails.GetMerchantDailyPerformanceSummaryResponse(this.LastMerchantDailyPerformanceSummaryMerchantId);

        merchantDailyPerformanceSummaryResponse.ShouldNotBeNull();
        merchantDailyPerformanceSummaryResponse!.Metrics.ShouldNotBeEmpty();
        merchantDailyPerformanceSummaryResponse.DrillDownTransactions.ShouldNotBeEmpty();
        merchantDailyPerformanceSummaryResponse.DrillDownTransactions.Any(t => t.Amount == saleAmount)
            .ShouldBeTrue($"Expected a drill down transaction with amount {saleAmount}");
    }

    public async Task WhenIGetTheMerchantTransactionMixSummaryForMerchantForEstate(String estateName,
                                                                                   String merchantName,
                                                                                   List<EstateDetails1> estateDetailsList,
                                                                                   CancellationToken cancellationToken)
    {
        EstateDetails1 es1 = estateDetailsList.SingleOrDefault(e => e.EstateDetails.EstateName == estateName);
        es1.ShouldNotBeNull();
        Guid merchantId = es1.EstateDetails.GetMerchantId(merchantName);
        Int32 merchantReportingId = es1.GetMerchantReportingId(merchantId);

        String uri = "api/reporting/transactionmixsummary";
        String userAccessToken = es1.GetMerchantUserToken(merchantId);

        this.HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", userAccessToken);

        MerchantTransactionMixSummaryRequest request= new()
        {
            ApplicationVersion = "1.0.5",
            MerchantReportingId = merchantReportingId,
            StartDate = DateTime.Today,
            EndDate = DateTime.Today,
            Breakdown = TransactionMixBreakdown.Product,
            Measure = TransactionMixMeasure.Count,
            TopN = 5,
        };

        StringContent content = new StringContent(StringSerialiser.Serialise(request), Encoding.UTF8, "application/json");

        HttpResponseMessage response = await this.HttpClient.PostAsync(uri, content, cancellationToken).ConfigureAwait(false);

        response.IsSuccessStatusCode.ShouldBeTrue();

        String responseContent = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
        responseContent.ShouldNotBeNullOrEmpty("No response message received");

        ReportingMerchantTransactionMixSummaryResponse transactionMixSummaryResponse =
            StringSerialiser.Deserialise<ReportingMerchantTransactionMixSummaryResponse>(responseContent,
                                                                                        new SerialiserOptions(SerialiserPropertyFormat.SnakeCase));

        es1.AddMerchantTransactionMixSummaryResponse(merchantId, transactionMixSummaryResponse);
        this.LastMerchantTransactionMixSummaryEstateDetails = es1;
        this.LastMerchantTransactionMixSummaryMerchantId = merchantId;
    }

    public async Task WhenIGetTheRecentActivityReceiptSearchForMerchantForEstate(String estateName,
                                                                                 String merchantName,
                                                                                 Int32 pageNumber,
                                                                                 Int32 pageSize,
                                                                                 List<EstateDetails1> estateDetailsList,
                                                                                 CancellationToken cancellationToken)
    {
        EstateDetails1 es1 = estateDetailsList.SingleOrDefault(e => e.EstateDetails.EstateName == estateName);
        es1.ShouldNotBeNull();
        Guid merchantId = es1.EstateDetails.GetMerchantId(merchantName);
        Int32 merchantReportingId = es1.GetMerchantReportingId(merchantId);

        String uri = "api/reporting/recentactivityreceiptsearch";
        String userAccessToken = es1.GetMerchantUserToken(merchantId);

        this.HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", userAccessToken);

        RecentActivityReceiptSearchRequest request = new()
        {
            ApplicationVersion = "1.0.5",
            MerchantReportingId = merchantReportingId,
            ReportDate = DateTime.Today,
            PageNumber = pageNumber,
            PageSize = pageSize
        };

        StringContent content = new StringContent(StringSerialiser.Serialise(request), Encoding.UTF8, "application/json");

        HttpResponseMessage response = await this.HttpClient.PostAsync(uri, content, cancellationToken).ConfigureAwait(false);

        response.IsSuccessStatusCode.ShouldBeTrue();

        String responseContent = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
        responseContent.ShouldNotBeNullOrEmpty("No response message received");

        RecentActivityReceiptSearchResponse searchResponse =
            StringSerialiser.Deserialise<RecentActivityReceiptSearchResponse>(responseContent,
                                                                             new SerialiserOptions(SerialiserPropertyFormat.SnakeCase));

        this.RecentActivityReceiptSearchResponses[pageNumber] = searchResponse;
        this.LastRecentActivityReceiptSearchEstateDetails = es1;
        this.LastRecentActivityReceiptSearchMerchantId = merchantId;
    }

    public void ThenTheMerchantTransactionMixSummaryResponseShouldContainAtLeastOneItemAndTheSaleAmount(Decimal saleAmount)
    {
        this.LastMerchantTransactionMixSummaryEstateDetails.ShouldNotBeNull();

        ReportingMerchantTransactionMixSummaryResponse merchantTransactionMixSummaryResponse =
            this.LastMerchantTransactionMixSummaryEstateDetails.GetMerchantTransactionMixSummaryResponse(this.LastMerchantTransactionMixSummaryMerchantId);

        merchantTransactionMixSummaryResponse.ShouldNotBeNull();
        merchantTransactionMixSummaryResponse!.Items.ShouldNotBeEmpty();
        merchantTransactionMixSummaryResponse.TotalCount.ShouldBeGreaterThan(0);
        merchantTransactionMixSummaryResponse.DrillDownTransactions.ShouldNotBeEmpty();
        merchantTransactionMixSummaryResponse.DrillDownTransactions.Any(t => t.Amount == saleAmount)
            .ShouldBeTrue($"Expected a drill down transaction with amount {saleAmount}");
    }

    public void ThenTheRecentActivityReceiptSearchResponseShouldContainPageWithCountAndDescendingDates(Int32 pageNumber,
                                                                                                       Int32 expectedCount,
                                                                                                       Int32 expectedTotalCount)
    {
        this.LastRecentActivityReceiptSearchEstateDetails.ShouldNotBeNull();

        RecentActivityReceiptSearchResponse response = this.RecentActivityReceiptSearchResponses.SingleOrDefault(x => x.Key == pageNumber).Value;
        response.ShouldNotBeNull();
        response.Items.Count.ShouldBe(expectedCount);
        response.TotalCount.ShouldBe(expectedTotalCount);
        response.PageNumber.ShouldBe(pageNumber);
        response.PageSize.ShouldBe(expectedCount);
        response.Items.Select(i => i.TransactionDateTime).Zip(response.Items.Select(i => i.TransactionDateTime).Skip(1), (first, second) => first >= second).All(x => x).ShouldBeTrue();
    }
}
