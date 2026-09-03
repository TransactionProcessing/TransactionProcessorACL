using System;
using SimpleResults;

namespace TransactionProcessorACL.BusinesssLogic.Tests
{
    using System.Collections.Generic;
    using System.Globalization;
    using System.Threading;
    using System.Threading.Tasks;
    using BusinessLogic.Services;
    using TransactionProcessorACL.BusinessLogic.BackendAPI;
    using Microsoft.Extensions.Configuration;
    using Models;
    using Imposter.Abstractions;
    using SecurityService.Client;
    using Shared.General;
    using Shared.Logger;
    using Shouldly;
    using Testing;
    using TransactionProcessorACL.DataTransferObjects.Requests;
    using TransactionProcessor.Client;
    using TransactionProcessor.DataTransferObjects;
    using Xunit;
    using GetVoucherResponse = Models.GetVoucherResponse;
    using RedeemVoucherResponse = Models.RedeemVoucherResponse;
    using RequestTransactionMixBreakdown = TransactionProcessorACL.DataTransferObjects.Requests.TransactionMixBreakdown;
    using RequestTransactionMixMeasure = TransactionProcessorACL.DataTransferObjects.Requests.TransactionMixMeasure;

    public class TransactionProcessorACLApplicationServiceTests
    {
        public TransactionProcessorACLApplicationServiceTests()
        {
            Logger.Initialise(new NullLogger());

            this.SetupMemoryConfiguration();

            transactionProcessorClient = new ITransactionProcessorClientImposter();
            securityServiceClient = new ISecurityServiceClientImposter();
            estateReportingApiClient = new IEstateReportingApiClientImposter();
            applicationService =
                new TransactionProcessorACLApplicationService(transactionProcessorClient.Instance(), securityServiceClient.Instance(), estateReportingApiClient.Instance());
        }

        private void SetupMemoryConfiguration()
        {
            this.InitialiseConfiguration();
        }

        private void InitialiseConfiguration(int? securityServiceTokenRetryCount = null)
        {
            Dictionary<String, String> settings = new(TestData.DefaultAppSettings);
            if (securityServiceTokenRetryCount.HasValue) {
                settings["AppSettings:SecurityServiceTokenRetryCount"] = securityServiceTokenRetryCount.Value.ToString(CultureInfo.InvariantCulture);
            }

            IConfigurationRoot configuration = new ConfigurationBuilder().AddInMemoryCollection(settings).Build();
            ConfigurationReader.Initialise(configuration);
        }

        private ITransactionProcessorClientImposter transactionProcessorClient;

        private ISecurityServiceClientImposter securityServiceClient;

        private IEstateReportingApiClientImposter estateReportingApiClient;

        private ITransactionProcessorACLApplicationService applicationService;
        [Fact]
        public async Task TransactionProcessorACLApplicationService_ProcessLogonTransaction_TransactionIsSuccessful()
        {
            transactionProcessorClient.PerformTransaction(Arg<String>.Any(), Arg<LogonTransactionRequest>.Any(), Arg<CancellationToken>.Any())
                                      .ReturnsAsync(TestData.ClientLogonTransactionResponse);
            securityServiceClient.GetToken(Arg<String>.Any(), Arg<String>.Any(), Arg<CancellationToken>.Any()).ReturnsAsync( Result.Success(TestData.TokenResponse));

            Result<ProcessLogonTransactionResponse> result = await applicationService.ProcessLogonTransaction(TestData.EstateId,
                                                                                                             TestData.MerchantId,
                                                                                                             TestData.TransactionDateTime,
                                                                                                             TestData.TransactionNumber,
                                                                                                             TestData.DeviceIdentifier,
                                                                                                             CancellationToken.None);
            result.IsSuccess.ShouldBeTrue();
            ProcessLogonTransactionResponse logonResponse = result.Data;
            logonResponse.ShouldNotBeNull();
            logonResponse.ResponseMessage.ShouldBe(TestData.ResponseMessage);
            logonResponse.ResponseCode.ShouldBe(TestData.ResponseCode);
        }

        [Fact]
        public async Task TransactionProcessorACLApplicationService_ProcessLogonTransaction_RequestIsSerialisedCorrectly()
        {
            LogonTransactionRequest capturedMessage = null;

            transactionProcessorClient.PerformTransaction(Arg<String>.Any(), Arg<LogonTransactionRequest>.Any(), Arg<CancellationToken>.Any())
                                      .ReturnsAsync(TestData.ClientLogonTransactionResponse).Callback((_, message, _) => { capturedMessage = message; return Task.CompletedTask; });
            securityServiceClient.GetToken(Arg<String>.Any(), Arg<String>.Any(), Arg<CancellationToken>.Any())
                                 .ReturnsAsync(Result.Success(TestData.TokenResponse));

            await applicationService.ProcessLogonTransaction(TestData.EstateId,
                                                             TestData.MerchantId,
                                                             TestData.TransactionDateTime,
                                                             TestData.TransactionNumber,
                                                             TestData.DeviceIdentifier,
                                                             CancellationToken.None);

            capturedMessage.ShouldNotBeNull();
            capturedMessage.EstateId.ShouldBe(TestData.EstateId);
            capturedMessage.MerchantId.ShouldBe(TestData.MerchantId);
            capturedMessage.TransactionNumber.ShouldBe(TestData.TransactionNumber);
            capturedMessage.DeviceIdentifier.ShouldBe(TestData.DeviceIdentifier);
            capturedMessage.TransactionDateTime.ShouldBe(TestData.TransactionDateTime);
            capturedMessage.TransactionType.ShouldBe("LOGON");
        }

        [Fact]
        public async Task TransactionProcessorACLApplicationService_ProcessLogonTransaction_GetTokenFailed_ResultFailed()
        {
            securityServiceClient.GetToken(Arg<String>.Any(), Arg<String>.Any(), Arg<CancellationToken>.Any()).ReturnsAsync(Result.Failure());

            Result<ProcessLogonTransactionResponse> result = await applicationService.ProcessLogonTransaction(TestData.EstateId,
                TestData.MerchantId,
                TestData.TransactionDateTime,
                TestData.TransactionNumber,
                TestData.DeviceIdentifier,
                CancellationToken.None);

            result.IsFailed.ShouldBeTrue();
        }

        [Fact]
        public async Task TransactionProcessorACLApplicationService_ProcessLogonTransaction_PerformTransactionFailed_ResultFailed()
        {
            securityServiceClient.GetToken(Arg<String>.Any(), Arg<String>.Any(), Arg<CancellationToken>.Any()).ReturnsAsync(Result.Success(TestData.TokenResponse));
            transactionProcessorClient.PerformTransaction(Arg<String>.Any(), Arg<LogonTransactionRequest>.Any(), Arg<CancellationToken>.Any())
                .ReturnsAsync(Result.Failure());
            Result<ProcessLogonTransactionResponse> result = await applicationService.ProcessLogonTransaction(TestData.EstateId,
                TestData.MerchantId,
                TestData.TransactionDateTime,
                TestData.TransactionNumber,
                TestData.DeviceIdentifier,
                CancellationToken.None);

            result.IsFailed.ShouldBeTrue();
        }

        [Fact]
        public async Task TransactionProcessorACLApplicationService_ProcessLogonTransaction_ExceptionErrorInLogon_TransactionIsNotSuccessful()
        {
            transactionProcessorClient.PerformTransaction(Arg<String>.Any(), Arg<LogonTransactionRequest>.Any(), Arg<CancellationToken>.Any())
                                      .Throws(new Exception("Error"));
            securityServiceClient.GetToken(Arg<String>.Any(), Arg<String>.Any(), Arg<CancellationToken>.Any()).ReturnsAsync(Result.Success(TestData.TokenResponse));

            Result<ProcessLogonTransactionResponse> result = await applicationService.ProcessLogonTransaction(TestData.EstateId,
                                                                                                             TestData.MerchantId,
                                                                                                             TestData.TransactionDateTime,
                                                                                                             TestData.TransactionNumber,
                                                                                                             TestData.DeviceIdentifier,
                                                                                                             CancellationToken.None);
            result.IsSuccess.ShouldBeTrue();
            ProcessLogonTransactionResponse logonResponse =result.Data;
            logonResponse.ShouldNotBeNull();
            logonResponse.ResponseMessage.ShouldContain(TestData.LogonExceptionResponseMessage);
            logonResponse.ResponseCode.ShouldBe(TestData.ExceptionErrorResponseCode);
        }
        
        [Fact]
        public async Task TransactionProcessorACLApplicationService_ProcessSaleTransaction_TransactionIsSuccessful()
        {
            transactionProcessorClient.PerformTransaction(Arg<String>.Any(), Arg<SaleTransactionRequest>.Any(), Arg<CancellationToken>.Any())
                                      .ReturnsAsync(TestData.ClientSaleTransactionResponse);
            securityServiceClient.GetToken(Arg<String>.Any(), Arg<String>.Any(), Arg<CancellationToken>.Any()).ReturnsAsync(Result.Success(TestData.TokenResponse));

            Result<ProcessSaleTransactionResponse> result = await applicationService.ProcessSaleTransaction((TestData.EstateId, TestData.MerchantId),
            TestData.TransactionDateTime,
                                                                                                             TestData.TransactionNumber,
                                                                                                             TestData.DeviceIdentifier,
                                                                                                             TestData.CustomerEmailAddress,
                                                                                                             (TestData.OperatorId, TestData.ContractId, TestData.ProductId),
                                                                                                             TestData.AdditionalRequestMetadata,
                                                                                                             CancellationToken.None);

            result.IsSuccess.ShouldBeTrue();
            ProcessSaleTransactionResponse saleResponse = result.Data;
            saleResponse.ShouldNotBeNull();
            saleResponse.ResponseMessage.ShouldBe(TestData.ResponseMessage);
            saleResponse.ResponseCode.ShouldBe(TestData.ResponseCode);
        }

        [Fact]
        public async Task TransactionProcessorACLApplicationService_ProcessSaleTransaction_RequestContainsExpectedSaleData()
        {
            SaleTransactionRequest capturedRequest = null;
            transactionProcessorClient.PerformTransaction(Arg<String>.Any(), Arg<SaleTransactionRequest>.Any(), Arg<CancellationToken>.Any())
                                      .ReturnsAsync(TestData.ClientSaleTransactionResponse).Callback((accessToken, request, cancellationToken) => { capturedRequest = request; return Task.CompletedTask; });
            securityServiceClient.GetToken(Arg<String>.Any(), Arg<String>.Any(), Arg<CancellationToken>.Any()).ReturnsAsync(Result.Success(TestData.TokenResponse));

            Result<ProcessSaleTransactionResponse> result = await applicationService.ProcessSaleTransaction((TestData.EstateId, TestData.MerchantId),
                                                                                                             TestData.TransactionDateTime,
                                                                                                             TestData.TransactionNumber,
                                                                                                             TestData.DeviceIdentifier,
                                                                                                             TestData.CustomerEmailAddress,
                                                                                                             (TestData.OperatorId, TestData.ContractId, TestData.ProductId),
                                                                                                             TestData.AdditionalRequestMetadata,
                                                                                                             CancellationToken.None);

            result.IsSuccess.ShouldBeTrue();
            capturedRequest.ShouldNotBeNull();
            capturedRequest.EstateId.ShouldBe(TestData.EstateId);
            capturedRequest.MerchantId.ShouldBe(TestData.MerchantId);
            
            capturedRequest.ShouldNotBeNull();
            capturedRequest.TransactionNumber.ShouldBe(TestData.TransactionNumber);
            capturedRequest.DeviceIdentifier.ShouldBe(TestData.DeviceIdentifier);
            capturedRequest.TransactionDateTime.ShouldBe(TestData.TransactionDateTime);
            capturedRequest.TransactionType.ShouldBe("SALE");
            capturedRequest.OperatorId.ShouldBe(TestData.OperatorId);
            capturedRequest.CustomerEmailAddress.ShouldBe(TestData.CustomerEmailAddress);
            capturedRequest.TransactionSource.ShouldBe(1);
            capturedRequest.ContractId.ShouldBe(TestData.ContractId);
            capturedRequest.ProductId.ShouldBe(TestData.ProductId);
            capturedRequest.AdditionalTransactionMetadata["Amount"].ShouldBe(TestData.AdditionalRequestMetadata["Amount"]);
            capturedRequest.AdditionalTransactionMetadata["CustomerAccountNumber"].ShouldBe(TestData.AdditionalRequestMetadata["CustomerAccountNumber"]);
        }

        [Fact]
        public async Task TransactionProcessorACLApplicationService_ProcessSaleTransaction_GetTokenFailed_ResultFailed()
        {
            securityServiceClient.GetToken(Arg<String>.Any(), Arg<String>.Any(), Arg<CancellationToken>.Any()).ReturnsAsync(Result.Failure());

            Result<ProcessSaleTransactionResponse> result = await applicationService.ProcessSaleTransaction((TestData.EstateId, TestData.MerchantId),
                                                                                                             TestData.TransactionDateTime,
                                                                                                             TestData.TransactionNumber,
                                                                                                             TestData.DeviceIdentifier,
                                                                                                             TestData.CustomerEmailAddress,
                                                                                                             (TestData.OperatorId, TestData.ContractId, TestData.ProductId),
                                                                                                             TestData.AdditionalRequestMetadata,
                                                                                                             CancellationToken.None);

            result.IsFailed.ShouldBeTrue();
        }

        [Fact]
        public async Task TransactionProcessorACLApplicationService_ProcessSaleTransaction_PerformTransactionFailed_ResultFailed()
        {
            transactionProcessorClient.PerformTransaction(Arg<String>.Any(), Arg<SaleTransactionRequest>.Any(), Arg<CancellationToken>.Any())
                                      .ReturnsAsync(Result.Failure());
            securityServiceClient.GetToken(Arg<String>.Any(), Arg<String>.Any(), Arg<CancellationToken>.Any()).ReturnsAsync(Result.Success(TestData.TokenResponse));

            Result<ProcessSaleTransactionResponse> result = await applicationService.ProcessSaleTransaction((TestData.EstateId, TestData.MerchantId),
                TestData.TransactionDateTime,
                TestData.TransactionNumber,
                TestData.DeviceIdentifier,
                TestData.CustomerEmailAddress,
                (TestData.OperatorId, TestData.ContractId, TestData.ProductId),
                TestData.AdditionalRequestMetadata,
                CancellationToken.None);

            result.IsFailed.ShouldBeTrue();
        }

        [Fact]
        public async Task TransactionProcessorACLApplicationService_ProcessSaleTransaction_ExceptionErrorInSale_TransactionIsNotSuccessful()
        {
            transactionProcessorClient.PerformTransaction(Arg<String>.Any(), Arg<SaleTransactionRequest>.Any(), Arg<CancellationToken>.Any())
                                      .Throws(new Exception("Error"));
            securityServiceClient.GetToken(Arg<String>.Any(), Arg<String>.Any(), Arg<CancellationToken>.Any()).ReturnsAsync(Result.Success(TestData.TokenResponse));

            Result<ProcessSaleTransactionResponse> result = await applicationService.ProcessSaleTransaction((TestData.EstateId, TestData.MerchantId),
                                                                                                          TestData.TransactionDateTime,
                                                                                                          TestData.TransactionNumber,
                                                                                                          TestData.DeviceIdentifier,
                                                                                                          TestData.CustomerEmailAddress,
                                                                                                          (TestData.OperatorId, TestData.ContractId, TestData.ProductId),
                                                                                                          TestData.AdditionalRequestMetadata,
                                                                                                          CancellationToken.None);
            result.IsSuccess.ShouldBeTrue();
            ProcessSaleTransactionResponse saleResponse = result.Data;
            saleResponse.ShouldNotBeNull();
            saleResponse.ResponseMessage.ShouldBe(TestData.SaleExceptionResponseMessage);
            saleResponse.ResponseCode.ShouldBe(TestData.ExceptionErrorResponseCode);
        }

        [Fact]
        public async Task TransactionProcessorACLApplicationService_ProcessReconciliation_TransactionIsSuccessful()
        {
            transactionProcessorClient.PerformTransaction(Arg<String>.Any(), Arg<ReconciliationRequest>.Any(), Arg<CancellationToken>.Any())
                                      .ReturnsAsync(TestData.ClientReconciliationResponse);
            securityServiceClient.GetToken(Arg<String>.Any(), Arg<String>.Any(), Arg<CancellationToken>.Any()).ReturnsAsync(Result.Success(TestData.TokenResponse));

            Result<ProcessReconciliationResponse> result = await applicationService.ProcessReconciliation(TestData.EstateId,
                TestData.MerchantId,
                TestData.TransactionDateTime,
                TestData.DeviceIdentifier,
                TestData.ReconciliationTransactionCount,
                TestData.ReconciliationTransactionValue,
                CancellationToken.None);

            result.IsSuccess.ShouldBeTrue();
            ProcessReconciliationResponse reconciliationResponse = result.Data;
            reconciliationResponse.ShouldNotBeNull();
            reconciliationResponse.ResponseMessage.ShouldBe(TestData.ResponseMessage);
            reconciliationResponse.ResponseCode.ShouldBe(TestData.ResponseCode);
        }

        [Fact]
        public async Task TransactionProcessorACLApplicationService_ProcessReconciliation_RequestIsSerialisedCorrectly()
        {
            ReconciliationRequest capturedMessage = null;

            transactionProcessorClient.PerformTransaction(Arg<String>.Any(), Arg<ReconciliationRequest>.Any(), Arg<CancellationToken>.Any())
                                      .ReturnsAsync(TestData.ClientReconciliationResponse).Callback((_, message, _) => { capturedMessage = message; return Task.CompletedTask; });
            securityServiceClient.GetToken(Arg<String>.Any(), Arg<String>.Any(), Arg<CancellationToken>.Any()).ReturnsAsync(Result.Success(TestData.TokenResponse));

            await applicationService.ProcessReconciliation(TestData.EstateId,
                                                           TestData.MerchantId,
                                                           TestData.TransactionDateTime,
                                                           TestData.DeviceIdentifier,
                                                           TestData.ReconciliationTransactionCount,
                                                           TestData.ReconciliationTransactionValue,
                                                           CancellationToken.None);

            capturedMessage.ShouldNotBeNull();
            capturedMessage.ShouldNotBeNull();
            capturedMessage.EstateId.ShouldBe(TestData.EstateId);
            capturedMessage.MerchantId.ShouldBe(TestData.MerchantId);
            capturedMessage.DeviceIdentifier.ShouldBe(TestData.DeviceIdentifier);
            capturedMessage.TransactionDateTime.ShouldBe(TestData.TransactionDateTime);
            capturedMessage.TransactionCount.ShouldBe(TestData.ReconciliationTransactionCount);
            capturedMessage.TransactionValue.ShouldBe(TestData.ReconciliationTransactionValue);
        }

        [Fact]
        public async Task TransactionProcessorACLApplicationService_ProcessReconciliation_GetTokenFailed_ResultFailed()
        {
            securityServiceClient.GetToken(Arg<String>.Any(), Arg<String>.Any(), Arg<CancellationToken>.Any()).ReturnsAsync(Result.Failure());

            Result<ProcessReconciliationResponse> result = await applicationService.ProcessReconciliation(TestData.EstateId,
                TestData.MerchantId,
                TestData.TransactionDateTime,
                TestData.DeviceIdentifier,
                TestData.ReconciliationTransactionCount,
                TestData.ReconciliationTransactionValue,
                CancellationToken.None);

            result.IsFailed.ShouldBeTrue();
        }

        [Fact]
        public async Task TransactionProcessorACLApplicationService_ProcessReconciliation_PerformTransactionFailed_ResultFailed()
        {
            securityServiceClient.GetToken(Arg<String>.Any(), Arg<String>.Any(), Arg<CancellationToken>.Any()).ReturnsAsync(Result.Success(TestData.TokenResponse));
            transactionProcessorClient.PerformTransaction(Arg<String>.Any(), Arg<ReconciliationRequest>.Any(), Arg<CancellationToken>.Any())
                .ReturnsAsync(Result.Failure());

            Result<ProcessReconciliationResponse> result = await applicationService.ProcessReconciliation(TestData.EstateId,
                TestData.MerchantId,
                TestData.TransactionDateTime,
                TestData.DeviceIdentifier,
                TestData.ReconciliationTransactionCount,
                TestData.ReconciliationTransactionValue,
                CancellationToken.None);

            result.IsFailed.ShouldBeTrue();
        }

        [Fact]
        public async Task TransactionProcessorACLApplicationService_ProcessReconciliation_ExceptionErrorInReconciliation_TransactionIsNotSuccessful()
        {
            transactionProcessorClient.PerformTransaction(Arg<String>.Any(), Arg<ReconciliationRequest>.Any(), Arg<CancellationToken>.Any())
                                      .Throws(new Exception("Error"));
            securityServiceClient.GetToken(Arg<String>.Any(), Arg<String>.Any(), Arg<CancellationToken>.Any()).ReturnsAsync(Result.Success(TestData.TokenResponse));

            Result<ProcessReconciliationResponse> result= await applicationService.ProcessReconciliation(TestData.EstateId,
                TestData.MerchantId,
                TestData.TransactionDateTime,
                TestData.DeviceIdentifier,
                TestData.ReconciliationTransactionCount,
                TestData.ReconciliationTransactionValue,
                CancellationToken.None);

            result.IsSuccess.ShouldBeTrue();
            ProcessReconciliationResponse reconciliationResponse = result.Data;
            reconciliationResponse.ShouldNotBeNull();
            reconciliationResponse.ResponseMessage.ShouldBe(TestData.ReconciliationExceptionResponseMessage);
            reconciliationResponse.ResponseCode.ShouldBe(TestData.ExceptionErrorResponseCode);
        }

        [Fact]
        public async Task TransactionProcessorACLApplicationService_GetVoucher_VoucherRetrieved()
        {
            this.transactionProcessorClient.GetVoucherByCode(Arg<String>.Any(), Arg<Guid>.Any(), Arg<String>.Any(), Arg<CancellationToken>.Any())
                .ReturnsAsync(TestData.GetVoucherResponse);
            securityServiceClient.GetToken(Arg<String>.Any(), Arg<String>.Any(), Arg<CancellationToken>.Any()).ReturnsAsync(Result.Success(TestData.TokenResponse));

            Result<GetVoucherResponse> voucherResponse = await applicationService.GetVoucher(TestData.EstateId, TestData.ContractId, TestData.VoucherCode, CancellationToken.None);

            voucherResponse.IsSuccess.ShouldBeTrue();
            voucherResponse.Data.ShouldNotBeNull();
            voucherResponse.Data.VoucherCode.ShouldBe(TestData.GetVoucherResponse.VoucherCode);
            voucherResponse.Data.ContractId.ShouldBe(TestData.ContractId);
            voucherResponse.Data.EstateId.ShouldBe(TestData.EstateId);
            voucherResponse.Data.ExpiryDate.ShouldBe(TestData.GetVoucherResponse.ExpiryDate);
            voucherResponse.Data.Value.ShouldBe(TestData.GetVoucherResponse.Value);
            voucherResponse.Data.VoucherId.ShouldBe(TestData.GetVoucherResponse.VoucherId);
        }

        [Fact]
        public async Task TransactionProcessorACLApplicationService_GetVoucher_GetTokenFailed_ResultIsFailed()
        {
            securityServiceClient.GetToken(Arg<String>.Any(), Arg<String>.Any(), Arg<CancellationToken>.Any()).ReturnsAsync(Result.Failure());

            Result<GetVoucherResponse> voucherResponse = await applicationService.GetVoucher(TestData.EstateId, TestData.ContractId, TestData.VoucherCode, CancellationToken.None);

            voucherResponse.IsFailed.ShouldBeTrue();
        }

        [Fact]
        public async Task TransactionProcessorACLApplicationService_GetVoucher_GetVoucherFailed_ResultIsFailed()
        {
            this.transactionProcessorClient.GetVoucherByCode(Arg<String>.Any(), Arg<Guid>.Any(), Arg<String>.Any(), Arg<CancellationToken>.Any())
                .ReturnsAsync(Result.Failure());
            securityServiceClient.GetToken(Arg<String>.Any(), Arg<String>.Any(), Arg<CancellationToken>.Any()).ReturnsAsync(Result.Success(TestData.TokenResponse));

            Result<GetVoucherResponse> voucherResponse = await applicationService.GetVoucher(TestData.EstateId, TestData.ContractId, TestData.VoucherCode, CancellationToken.None);

            voucherResponse.IsFailed.ShouldBeTrue();
        }

        [Fact]
        public async Task TransactionProcessorACLApplicationService_GetVoucher_ExceptionErrorInGetVoucher_GetVoucherIsNotSuccessful()
        {
            transactionProcessorClient.GetVoucherByCode(Arg<String>.Any(), Arg<Guid>.Any(), Arg<String>.Any(), Arg<CancellationToken>.Any())
                                      .Throws(new Exception("Error"));
            securityServiceClient.GetToken(Arg<String>.Any(), Arg<String>.Any(), Arg<CancellationToken>.Any()).ReturnsAsync(Result.Success(TestData.TokenResponse));

            Result<GetVoucherResponse> voucherResponse = await applicationService.GetVoucher(TestData.EstateId, TestData.ContractId, TestData.VoucherCode, CancellationToken.None);

            voucherResponse.IsFailed.ShouldBeTrue();
        }

        [Fact]
        public async Task TransactionProcessorACLApplicationService_RedeemVoucher_VoucherRedeemed()
        {
            transactionProcessorClient.RedeemVoucher(Arg<String>.Any(), Arg<RedeemVoucherRequest>.Any(), Arg<CancellationToken>.Any())
                                      .ReturnsAsync(Result.Success());
            securityServiceClient.GetToken(Arg<String>.Any(), Arg<String>.Any(), Arg<CancellationToken>.Any()).ReturnsAsync(Result.Success(TestData.TokenResponse));

            Result<RedeemVoucherResponse> voucherResponse = await applicationService.RedeemVoucher(TestData.EstateId, TestData.ContractId, TestData.VoucherCode, CancellationToken.None);

            voucherResponse.IsSuccess.ShouldBeTrue();
            voucherResponse.Data.ContractId.ShouldBe(TestData.ContractId);
            voucherResponse.Data.EstateId.ShouldBe(TestData.EstateId);
        }

        [Fact]
        public async Task TransactionProcessorACLApplicationService_RedeemVoucher_GetTokenFailed_ResultIsFailed()
        {
            transactionProcessorClient.RedeemVoucher(Arg<String>.Any(), Arg<RedeemVoucherRequest>.Any(), Arg<CancellationToken>.Any())
                .ReturnsAsync(Result.Failure());
            securityServiceClient.GetToken(Arg<String>.Any(), Arg<String>.Any(), Arg<CancellationToken>.Any()).ReturnsAsync(Result.Failure());

            Result<RedeemVoucherResponse> voucherResponse = await applicationService.RedeemVoucher(TestData.EstateId, TestData.ContractId, TestData.VoucherCode, CancellationToken.None);

            voucherResponse.IsFailed.ShouldBeTrue();
        }

        [Fact]
        public async Task TransactionProcessorACLApplicationService_RedeemVoucher_RedeemVoucherFailed_ResultIsFailed()
        {
            transactionProcessorClient.RedeemVoucher(Arg<String>.Any(), Arg<RedeemVoucherRequest>.Any(), Arg<CancellationToken>.Any())
                .ReturnsAsync(Result.Failure());
            securityServiceClient.GetToken(Arg<String>.Any(), Arg<String>.Any(), Arg<CancellationToken>.Any()).ReturnsAsync(Result.Success(TestData.TokenResponse));

            Result<RedeemVoucherResponse> voucherResponse = await applicationService.RedeemVoucher(TestData.EstateId, TestData.ContractId, TestData.VoucherCode, CancellationToken.None);

            voucherResponse.IsFailed.ShouldBeTrue();
        }

        [Fact]
        public async Task TransactionProcessorACLApplicationService_RedeemVoucher_ExceptionErrorInGetVoucher_GetVoucherIsNotSuccessful()
        {
             transactionProcessorClient.RedeemVoucher(Arg<String>.Any(), Arg<RedeemVoucherRequest>.Any(), Arg<CancellationToken>.Any())
                                      .Throws(new Exception("Error"));
            
            securityServiceClient.GetToken(Arg<String>.Any(), Arg<String>.Any(), Arg<CancellationToken>.Any()).ReturnsAsync(Result.Success(TestData.TokenResponse));

            Result<RedeemVoucherResponse> voucherResponse = await applicationService.RedeemVoucher(TestData.EstateId, TestData.ContractId, TestData.VoucherCode, CancellationToken.None);

            voucherResponse.IsFailed.ShouldBeTrue();
        }

        [Theory]
        [InlineData(TransactionProcessor.DataTransferObjects.Responses.Merchant.SettlementSchedule.Immediate)]
        [InlineData(TransactionProcessor.DataTransferObjects.Responses.Merchant.SettlementSchedule.Weekly)]
        [InlineData(TransactionProcessor.DataTransferObjects.Responses.Merchant.SettlementSchedule.Monthly)]
        public async Task TransactionProcessorACLApplicationService_GetMerchant_MerchantReturned(TransactionProcessor.DataTransferObjects.Responses.Merchant.SettlementSchedule settlementSchedule)
        {
            transactionProcessorClient.GetMerchant(Arg<String>.Any(), Arg<Guid>.Any(), Arg<Guid>.Any(), Arg<CancellationToken>.Any())
                                      .ReturnsAsync(Result.Success(TestData.MerchantResponse(settlementSchedule)));
            securityServiceClient.GetToken(Arg<String>.Any(), Arg<String>.Any(), Arg<CancellationToken>.Any()).ReturnsAsync(Result.Success(TestData.TokenResponse));

            Result<MerchantResponse> merchantResponse = await applicationService.GetMerchant(TestData.EstateId, TestData.MerchantId, CancellationToken.None);
            merchantResponse.IsSuccess.ShouldBeTrue();
            merchantResponse.Data.ShouldNotBeNull();
            merchantResponse.Data.MerchantId.ShouldBe(TestData.MerchantId);
            merchantResponse.Data.MerchantReportingId.ShouldBe(TestData.MerchantReportingId);
            merchantResponse.Data.Addresses.Count.ShouldBe(1);
            merchantResponse.Data.Addresses[0].AddressLine1.ShouldBe(TestData.AddressLine1);
            merchantResponse.Data.Addresses[0].Town.ShouldBe(TestData.Town);
            merchantResponse.Data.Contacts.Count.ShouldBe(1);
            merchantResponse.Data.Contacts[0].ContactName.ShouldBe(TestData.ContactName);
            merchantResponse.Data.Contacts[0].ContactEmailAddress.ShouldBe(TestData.ContactEmail);
            merchantResponse.Data.Contracts.Count.ShouldBe(1);
            merchantResponse.Data.Contracts[0].ContractId.ShouldBe(TestData.ContractId);
            merchantResponse.Data.Contracts[0].ContractProducts.ShouldContain(TestData.ProductId);
            merchantResponse.Data.Devices[TestData.DeviceId].ShouldBe(TestData.DeviceIdentifier);
            merchantResponse.Data.Operators.Count.ShouldBe(1);
            merchantResponse.Data.Operators[0].MerchantNumber.ShouldBe(TestData.MerchantNumber);
            merchantResponse.Data.Operators[0].TerminalNumber.ShouldBe(TestData.TerminalNumber);
        }

        [Fact]
        public async Task TransactionProcessorACLApplicationService_GetMerchant_GetTokenFailed_ResultIsFailed()
        {
            securityServiceClient.GetToken(Arg<String>.Any(), Arg<String>.Any(), Arg<CancellationToken>.Any()).ReturnsAsync(Result.Failure());

            Result<MerchantResponse> merchantResponse = await applicationService.GetMerchant(TestData.EstateId, TestData.MerchantId, CancellationToken.None);
            merchantResponse.IsFailed.ShouldBeTrue();
            
        }

        [Fact]
        public async Task TransactionProcessorACLApplicationService_GetMerchant_GetTokenThrowsOnceThenSucceeds_RetriesAndReturnsSuccess()
        {
            this.InitialiseConfiguration(1);

            transactionProcessorClient.GetMerchant(Arg<String>.Any(), Arg<Guid>.Any(), Arg<Guid>.Any(), Arg<CancellationToken>.Any())
                                      .ReturnsAsync(Result.Success(TestData.MerchantResponse(TransactionProcessor.DataTransferObjects.Responses.Merchant.SettlementSchedule.Monthly)));
            securityServiceClient.GetToken(Arg<String>.Any(), Arg<String>.Any(), Arg<CancellationToken>.Any())
                                 .Throws(new TaskCanceledException("Transient token failure")).Then()
                                 .ReturnsAsync(Result.Success(TestData.TokenResponse));

            Result<MerchantResponse> merchantResponse = await applicationService.GetMerchant(TestData.EstateId, TestData.MerchantId, CancellationToken.None);

            merchantResponse.IsSuccess.ShouldBeTrue();
            securityServiceClient.GetToken(Arg<String>.Any(), Arg<String>.Any(), Arg<CancellationToken>.Any()).Called(Count.Exactly(2));
        }

        [Fact]
        public async Task TransactionProcessorACLApplicationService_GetMerchant_GetTokenKeepsFailing_StopsAfterConfiguredRetries()
        {
            this.InitialiseConfiguration(2);

            transactionProcessorClient.GetMerchant(Arg<String>.Any(), Arg<Guid>.Any(), Arg<Guid>.Any(), Arg<CancellationToken>.Any())
                                      .ReturnsAsync(Result.Success(TestData.MerchantResponse(TransactionProcessor.DataTransferObjects.Responses.Merchant.SettlementSchedule.Monthly)));
            securityServiceClient.GetToken(Arg<String>.Any(), Arg<String>.Any(), Arg<CancellationToken>.Any())
                                 .Throws(new TaskCanceledException("Transient token failure")).Then()
                                 .Throws(new TaskCanceledException("Transient token failure"))
                                 .Then().Throws(new TaskCanceledException("Transient token failure"));

            Result<MerchantResponse> merchantResponse = await applicationService.GetMerchant(TestData.EstateId, TestData.MerchantId, CancellationToken.None);

            merchantResponse.IsFailed.ShouldBeTrue();
            securityServiceClient.GetToken(Arg<String>.Any(), Arg<String>.Any(), Arg<CancellationToken>.Any()).Called(Count.Exactly(3));
        }

        [Fact]
        public async Task TransactionProcessorACLApplicationService_GetMerchant_GetTokenCanceled_DoesNotRetry()
        {
            this.InitialiseConfiguration(2);

            using CancellationTokenSource cancellationTokenSource = new();
            cancellationTokenSource.Cancel();

            securityServiceClient.GetToken(Arg<String>.Any(), Arg<String>.Any(), Arg<CancellationToken>.Any())
                                 .Throws(new Exception("GetToken should not be called when the request is already canceled"));

            await Should.ThrowAsync<OperationCanceledException>(async () =>
            {
                await applicationService.GetMerchant(TestData.EstateId, TestData.MerchantId, cancellationTokenSource.Token);
            });

            securityServiceClient.GetToken(Arg<String>.Any(), Arg<String>.Any(), Arg<CancellationToken>.Any()).Called(Count.Never());
        }

        [Fact]
        public async Task TransactionProcessorACLApplicationService_GetMerchant_GetMerchantFailed_ResultIsFailed()
        {
            transactionProcessorClient.GetMerchant(Arg<String>.Any(), Arg<Guid>.Any(), Arg<Guid>.Any(), Arg<CancellationToken>.Any())
                .ReturnsAsync(Result.Failure());
            securityServiceClient.GetToken(Arg<String>.Any(), Arg<String>.Any(), Arg<CancellationToken>.Any()).ReturnsAsync(Result.Success(TestData.TokenResponse));

            Result<MerchantResponse> merchantResponse = await applicationService.GetMerchant(TestData.EstateId, TestData.MerchantId, CancellationToken.None);
            merchantResponse.IsFailed.ShouldBeTrue();
        }

        [Theory]
        [InlineData(TransactionProcessor.DataTransferObjects.Responses.Contract.ProductType.Voucher)]
        [InlineData(TransactionProcessor.DataTransferObjects.Responses.Contract.ProductType.BillPayment)]
        [InlineData(TransactionProcessor.DataTransferObjects.Responses.Contract.ProductType.MobileTopup)]
        public async Task TransactionProcessorACLApplicationService_GetMerchantContracts_MerchantContractsReturned(TransactionProcessor.DataTransferObjects.Responses.Contract.ProductType productType) {
            transactionProcessorClient.GetMerchantContracts(Arg<String>.Any(), Arg<Guid>.Any(), Arg<Guid>.Any(), Arg<CancellationToken>.Any())
                .ReturnsAsync(Result.Success(TestData.MerchantContractResponses(productType)));
            securityServiceClient.GetToken(Arg<String>.Any(), Arg<String>.Any(), Arg<CancellationToken>.Any()).ReturnsAsync(Result.Success(TestData.TokenResponse));

            var merchantContractsResponse = await applicationService.GetMerchantContracts(TestData.EstateId, TestData.MerchantId, CancellationToken.None);
            merchantContractsResponse.IsSuccess.ShouldBeTrue();
        }

        [Fact]
        public async Task TransactionProcessorACLApplicationService_GetMerchantContracts_GetTokenFailed_ResultIsFailed()
        {
            securityServiceClient.GetToken(Arg<String>.Any(), Arg<String>.Any(), Arg<CancellationToken>.Any()).ReturnsAsync(Result.Failure());

            var merchantContractsResponse = await applicationService.GetMerchantContracts(TestData.EstateId, TestData.MerchantId, CancellationToken.None);
            merchantContractsResponse.IsFailed.ShouldBeTrue();
        }

        [Fact]
        public async Task TransactionProcessorACLApplicationService_GetMerchantContracts_GetMerchantContractsFailed_ResultIsFailed()
        {
            transactionProcessorClient.GetMerchantContracts(Arg<String>.Any(), Arg<Guid>.Any(), Arg<Guid>.Any(), Arg<CancellationToken>.Any())
                .ReturnsAsync(Result.Failure());
            securityServiceClient.GetToken(Arg<String>.Any(), Arg<String>.Any(), Arg<CancellationToken>.Any()).ReturnsAsync(Result.Success(TestData.TokenResponse));

            var merchantContractsResponse = await applicationService.GetMerchantContracts(TestData.EstateId, TestData.MerchantId, CancellationToken.None);
            merchantContractsResponse.IsFailed.ShouldBeTrue();
        }

        [Fact]
        public async Task TransactionProcessorACLApplicationService_GetMerchantSchedule_MerchantScheduleReturned()
        {
            transactionProcessorClient.GetMerchantSchedule(Arg<String>.Any(), Arg<Guid>.Any(), Arg<Guid>.Any(),Arg<Int32>.Any(), Arg<CancellationToken>.Any())
                .ReturnsAsync(Result.Success(TestData.MerchantScheduleResponse()));
            securityServiceClient.GetToken(Arg<String>.Any(), Arg<String>.Any(), Arg<CancellationToken>.Any()).ReturnsAsync(Result.Success(TestData.TokenResponse));

            var merchantScheduleResponse = await applicationService.GetMerchantSchedule(TestData.EstateId, TestData.MerchantId, TestData.ScheduleYear, CancellationToken.None);
            merchantScheduleResponse.IsSuccess.ShouldBeTrue();
        }

        [Fact]
        public async Task TransactionProcessorACLApplicationService_GetMerchantSchedule_GetTokenFailed_ResultIsFailed()
        {
            securityServiceClient.GetToken(Arg<String>.Any(), Arg<String>.Any(), Arg<CancellationToken>.Any()).ReturnsAsync(Result.Failure());

            var merchantScheduleResponse = await applicationService.GetMerchantSchedule(TestData.EstateId, TestData.MerchantId, TestData.ScheduleYear, CancellationToken.None);
            merchantScheduleResponse.IsFailed.ShouldBeTrue();
        }

        [Fact]
        public async Task TransactionProcessorACLApplicationService_GetMerchantSchedule_GetMerchantScheduleFailed_ResultIsFailed()
        {
            transactionProcessorClient.GetMerchantSchedule(Arg<String>.Any(), Arg<Guid>.Any(), Arg<Guid>.Any(), Arg<Int32>.Any(), Arg<CancellationToken>.Any())
                .ReturnsAsync(Result.Failure());
            securityServiceClient.GetToken(Arg<String>.Any(), Arg<String>.Any(), Arg<CancellationToken>.Any()).ReturnsAsync(Result.Success(TestData.TokenResponse));

            var merchantScheduleResponse = await applicationService.GetMerchantSchedule(TestData.EstateId, TestData.MerchantId, TestData.ScheduleYear, CancellationToken.None);
            merchantScheduleResponse.IsFailed.ShouldBeTrue();
        }

        [Fact]
        public async Task TransactionProcessorACLApplicationService_GetMerchantDailyPerformanceSummary_ReturnedFromEstateReportingClient()
        {
            TransactionProcessorACL.BusinessLogic.BackendAPI.DataTransferObjects.MerchantDailyPerformanceSummaryRequest capturedRequest = null;
            estateReportingApiClient
                .GetMerchantDailyPerformanceSummary(Arg<String>.Any(), Arg<Guid>.Any(), Arg<TransactionProcessorACL.BusinessLogic.BackendAPI.DataTransferObjects.MerchantDailyPerformanceSummaryRequest>.Any(), Arg<CancellationToken>.Any())
                .ReturnsAsync(Result.Success(new TransactionProcessorACL.BusinessLogic.BackendAPI.DataTransferObjects.MerchantDailyPerformanceSummaryResponse
                {
                    Metrics =
                    [
                        new TransactionProcessorACL.BusinessLogic.BackendAPI.DataTransferObjects.MetricItem
                        {
                            Title = "Total Sales Count",
                            Value = 6,
                            Description = "All sales transactions in the range",
                            Category = 0
                        }
                    ]
                })).Callback((_, _, request, _) => { capturedRequest = request; return Task.CompletedTask; });
            securityServiceClient.GetToken(Arg<String>.Any(), Arg<String>.Any(), Arg<CancellationToken>.Any()).ReturnsAsync(Result.Success(TestData.TokenResponse));

            Result<MerchantDailyPerformanceSummaryResponse> result = await applicationService.GetMerchantDailyPerformanceSummary(
                TestData.EstateId,
                new MerchantDailyPerformanceSummaryRequest
                {
                    MerchantReportingId = 12345,
                    StartDate = new DateTime(2026, 7, 1),
                    EndDate = new DateTime(2026, 7, 1)
                },
                CancellationToken.None);

            result.IsSuccess.ShouldBeTrue();
            result.Data.ShouldNotBeNull();
            result.Data.Metrics.Count.ShouldBe(1);
            capturedRequest.ShouldNotBeNull();
            capturedRequest.MerchantReportingId.ShouldBe(12345);
        }

        [Fact]
        public async Task TransactionProcessorACLApplicationService_GetMerchantDailyPerformanceSummary_GetTokenFailed_ResultIsFailed()
        {
            securityServiceClient.GetToken(Arg<String>.Any(), Arg<String>.Any(), Arg<CancellationToken>.Any()).ReturnsAsync(Result.Failure());

            Result<MerchantDailyPerformanceSummaryResponse> result = await applicationService.GetMerchantDailyPerformanceSummary(
                TestData.EstateId,
                new MerchantDailyPerformanceSummaryRequest
                {
                    MerchantReportingId = 12345,
                    StartDate = new DateTime(2026, 7, 1),
                    EndDate = new DateTime(2026, 7, 1)
                },
                CancellationToken.None);

            result.IsFailed.ShouldBeTrue();
        }

        [Fact]
        public async Task TransactionProcessorACLApplicationService_GetMerchantTransactionMixSummary_ReturnedFromEstateReportingClient()
        {
            TransactionProcessorACL.BusinessLogic.BackendAPI.DataTransferObjects.TransactionMixSummaryRequest capturedRequest = null;
            estateReportingApiClient
                .GetMerchantTransactionMixSummary(Arg<String>.Any(), Arg<Guid>.Any(), Arg<TransactionProcessorACL.BusinessLogic.BackendAPI.DataTransferObjects.TransactionMixSummaryRequest>.Any(), Arg<CancellationToken>.Any())
                .ReturnsAsync(Result.Success(new TransactionProcessorACL.BusinessLogic.BackendAPI.DataTransferObjects.TransactionMixSummaryResponse
                {
                    FromDate = new DateTime(2026, 7, 1),
                    ToDate = new DateTime(2026, 7, 3),
                    Breakdown = TransactionProcessorACL.BusinessLogic.BackendAPI.DataTransferObjects.TransactionMixBreakdown.Product,
                    Measure = TransactionProcessorACL.BusinessLogic.BackendAPI.DataTransferObjects.TransactionMixMeasure.Count,
                    Groups = 
                    [
                        new TransactionProcessorACL.BusinessLogic.BackendAPI.DataTransferObjects.TransactionMixSummaryGroup()
                        {
                            GroupKey = "product-1",
                            GroupName = "Product 1",
                            TransactionCount = 6,
                            TransactionValue = 42.75M
                        }
                    ]
                })).Callback((_, _, request, _) => { capturedRequest = request; return Task.CompletedTask; });
            securityServiceClient.GetToken(Arg<String>.Any(), Arg<String>.Any(), Arg<CancellationToken>.Any()).ReturnsAsync(Result.Success(TestData.TokenResponse));

            Result<MerchantTransactionMixSummaryResponse> result = await applicationService.GetMerchantTransactionMixSummary(
                TestData.EstateId,
                new MerchantTransactionMixSummaryRequest
                {
                    MerchantReportingId = 12345,
                    StartDate = new DateTime(2026, 7, 1),
                    EndDate = new DateTime(2026, 7, 3),
                    Breakdown = RequestTransactionMixBreakdown.Product,
                    Measure = RequestTransactionMixMeasure.Count,
                    TopN = 5
                },
                CancellationToken.None);

            result.IsSuccess.ShouldBeTrue();
            result.Data.ShouldNotBeNull();
            result.Data.Items.Count.ShouldBe(1);
            capturedRequest.ShouldNotBeNull();
            capturedRequest.MerchantReportingId.ShouldBe(12345);
            //capturedRequest.Breakdown.ShouldBe(RequestTransactionMixBreakdown.Product);
        }

        [Fact]
        public async Task TransactionProcessorACLApplicationService_GetRecentActivityReceiptSearch_ReturnedFromEstateReportingClient()
        {
            TransactionProcessorACL.BusinessLogic.BackendAPI.DataTransferObjects.RecentActivityReceiptSearchRequest capturedRequest = null;
            estateReportingApiClient
                .GetRecentActivityReceiptSearch(Arg<String>.Any(), Arg<Guid>.Any(), Arg<TransactionProcessorACL.BusinessLogic.BackendAPI.DataTransferObjects.RecentActivityReceiptSearchRequest>.Any(), Arg<CancellationToken>.Any())
                .ReturnsAsync(Result.Success(new TransactionProcessorACL.BusinessLogic.BackendAPI.DataTransferObjects.RecentActivityReceiptSearchResponse
                {
                    ReportDate = new DateTime(2026, 7, 8),
                    PageNumber = 2,
                    PageSize = 5,
                    TotalCount = 1,
                    Items =
                    [
                        new TransactionProcessorACL.BusinessLogic.BackendAPI.DataTransferObjects.RecentActivityReceiptSearchItem
                        {
                            Reference = "REF-1",
                            TransactionType = "SALE",
                            Product = "Product 1",
                            Operator = "Operator 1",
                            Status = "Completed",
                            Amount = 42.75M,
                            TransactionDateTime = new DateTime(2026, 7, 8, 10, 30, 0),
                            ReceiptReference = "RCPT-1"
                        }
                    ]
                })).Callback((_, _, request, _) => { capturedRequest = request; return Task.CompletedTask; });
            securityServiceClient.GetToken(Arg<String>.Any(), Arg<String>.Any(), Arg<CancellationToken>.Any()).ReturnsAsync(Result.Success(TestData.TokenResponse));

            Result<RecentActivityReceiptSearchResponse> result = await applicationService.GetRecentActivityReceiptSearch(
                TestData.EstateId,
                new RecentActivityReceiptSearchRequest
                {
                    MerchantReportingId = 12345,
                    ReportDate = new DateTime(2026, 7, 8),
                    SearchText = "abc",
                    PageNumber = 2,
                    PageSize = 5
                },
                CancellationToken.None);

            result.IsSuccess.ShouldBeTrue();
            result.Data.ShouldNotBeNull();
            result.Data.Items.Count.ShouldBe(1);
            result.Data.Items[0].ReceiptReference.ShouldBe("RCPT-1");
            capturedRequest.ShouldNotBeNull();
            capturedRequest.MerchantReportingId.ShouldBe(12345);
            capturedRequest.ReportDate.ShouldBe(new DateTime(2026, 7, 8));
            capturedRequest.SearchText.ShouldBe("abc");
            capturedRequest.PageNumber.ShouldBe(2);
            capturedRequest.PageSize.ShouldBe(5);
        }

        [Fact]
        public async Task TransactionProcessorACLApplicationService_GetRecentActivityReceiptSearch_BlankSearchTextIsNotForwarded()
        {
            TransactionProcessorACL.BusinessLogic.BackendAPI.DataTransferObjects.RecentActivityReceiptSearchRequest capturedRequest = null;
            estateReportingApiClient
                .GetRecentActivityReceiptSearch(Arg<String>.Any(), Arg<Guid>.Any(), Arg<TransactionProcessorACL.BusinessLogic.BackendAPI.DataTransferObjects.RecentActivityReceiptSearchRequest>.Any(), Arg<CancellationToken>.Any())
                .ReturnsAsync(Result.Success(new TransactionProcessorACL.BusinessLogic.BackendAPI.DataTransferObjects.RecentActivityReceiptSearchResponse())).Callback((_, _, request, _) => { capturedRequest = request; return Task.CompletedTask; });
            securityServiceClient.GetToken(Arg<String>.Any(), Arg<String>.Any(), Arg<CancellationToken>.Any()).ReturnsAsync(Result.Success(TestData.TokenResponse));

            Result<RecentActivityReceiptSearchResponse> result = await applicationService.GetRecentActivityReceiptSearch(
                TestData.EstateId,
                new RecentActivityReceiptSearchRequest
                {
                    MerchantReportingId = 12345,
                    ReportDate = new DateTime(2026, 7, 8),
                    SearchText = "   ",
                    PageNumber = 1,
                    PageSize = 5
                },
                CancellationToken.None);

            result.IsSuccess.ShouldBeTrue();
            capturedRequest.ShouldNotBeNull();
            capturedRequest.SearchText.ShouldBeNull();
        }
    }
}
