using System;
using SimpleResults;

namespace TransactionProcessorACL.BusinesssLogic.Tests
{
    using System.Threading;
    using System.Threading.Tasks;
    using BusinessLogic.Services;
    using TransactionProcessorACL.BusinessLogic.BackendAPI;
    using Microsoft.Extensions.Configuration;
    using Models;
    using Moq;
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

    public class TransactionProcessorACLApplicationServiceTests
    {
        public TransactionProcessorACLApplicationServiceTests()
        {
            Logger.Initialise(new NullLogger());

            this.SetupMemoryConfiguration();

            transactionProcessorClient = new Mock<ITransactionProcessorClient>();
            securityServiceClient = new Mock<ISecurityServiceClient>();
            estateReportingApiClient = new Mock<IEstateReportingApiClient>();
            applicationService =
                new TransactionProcessorACLApplicationService(transactionProcessorClient.Object, securityServiceClient.Object, estateReportingApiClient.Object);
        }

        private void SetupMemoryConfiguration()
        {
            if (ConfigurationReader.IsInitialised == false)
            {
                IConfigurationRoot configuration = new ConfigurationBuilder().AddInMemoryCollection(TestData.DefaultAppSettings).Build();
                ConfigurationReader.Initialise(configuration);
            }
        }

        private Mock<ITransactionProcessorClient> transactionProcessorClient;

        private Mock<ISecurityServiceClient> securityServiceClient;

        private Mock<IEstateReportingApiClient> estateReportingApiClient;

        private ITransactionProcessorACLApplicationService applicationService;
        [Fact]
        public async Task TransactionProcessorACLApplicationService_ProcessLogonTransaction_TransactionIsSuccessful()
        {
            transactionProcessorClient.Setup(t => t.PerformTransaction(It.IsAny<String>(), It.IsAny<LogonTransactionRequest>(), It.IsAny<CancellationToken>()))
                                      .ReturnsAsync(TestData.ClientLogonTransactionResponse);
            securityServiceClient.Setup(s => s.GetToken(It.IsAny<String>(), It.IsAny<String>(), It.IsAny<CancellationToken>())).ReturnsAsync( Result.Success(TestData.TokenResponse));

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

            transactionProcessorClient.Setup(t => t.PerformTransaction(It.IsAny<String>(), It.IsAny<LogonTransactionRequest>(), It.IsAny<CancellationToken>()))
                                      .Callback<String, LogonTransactionRequest, CancellationToken>((_, message, _) => capturedMessage = message)
                                      .ReturnsAsync(TestData.ClientLogonTransactionResponse);
            securityServiceClient.Setup(s => s.GetToken(It.IsAny<String>(), It.IsAny<String>(), It.IsAny<CancellationToken>()))
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
            securityServiceClient.Setup(s => s.GetToken(It.IsAny<String>(), It.IsAny<String>(), It.IsAny<CancellationToken>())).ReturnsAsync(Result.Failure());

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
            securityServiceClient.Setup(s => s.GetToken(It.IsAny<String>(), It.IsAny<String>(), It.IsAny<CancellationToken>())).ReturnsAsync(Result.Success(TestData.TokenResponse));
            transactionProcessorClient.Setup(t => t.PerformTransaction(It.IsAny<String>(), It.IsAny<LogonTransactionRequest>(), It.IsAny<CancellationToken>()))
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
            transactionProcessorClient.Setup(t => t.PerformTransaction(It.IsAny<String>(), It.IsAny<LogonTransactionRequest>(), It.IsAny<CancellationToken>()))
                                      .ThrowsAsync(new Exception("Error"));
            securityServiceClient.Setup(s => s.GetToken(It.IsAny<String>(), It.IsAny<String>(), It.IsAny<CancellationToken>())).ReturnsAsync(Result.Success(TestData.TokenResponse));

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
            transactionProcessorClient.Setup(t => t.PerformTransaction(It.IsAny<String>(), It.IsAny<SaleTransactionRequest>(), It.IsAny<CancellationToken>()))
                                      .ReturnsAsync(TestData.ClientSaleTransactionResponse);
            securityServiceClient.Setup(s => s.GetToken(It.IsAny<String>(), It.IsAny<String>(), It.IsAny<CancellationToken>())).ReturnsAsync(Result.Success(TestData.TokenResponse));

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
            transactionProcessorClient.Setup(t => t.PerformTransaction(It.IsAny<String>(), It.IsAny<SaleTransactionRequest>(), It.IsAny<CancellationToken>()))
                                      .Callback<String, SaleTransactionRequest, CancellationToken>((accessToken, request, cancellationToken) => capturedRequest = request)
                                      .ReturnsAsync(TestData.ClientSaleTransactionResponse);
            securityServiceClient.Setup(s => s.GetToken(It.IsAny<String>(), It.IsAny<String>(), It.IsAny<CancellationToken>())).ReturnsAsync(Result.Success(TestData.TokenResponse));

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
            securityServiceClient.Setup(s => s.GetToken(It.IsAny<String>(), It.IsAny<String>(), It.IsAny<CancellationToken>())).ReturnsAsync(Result.Failure());

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
            transactionProcessorClient.Setup(t => t.PerformTransaction(It.IsAny<String>(), It.IsAny<SaleTransactionRequest>(), It.IsAny<CancellationToken>()))
                                      .ReturnsAsync(Result.Failure());
            securityServiceClient.Setup(s => s.GetToken(It.IsAny<String>(), It.IsAny<String>(), It.IsAny<CancellationToken>())).ReturnsAsync(Result.Success(TestData.TokenResponse));

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
            transactionProcessorClient.Setup(t => t.PerformTransaction(It.IsAny<String>(), It.IsAny<SaleTransactionRequest>(), It.IsAny<CancellationToken>()))
                                      .ThrowsAsync(new Exception("Error"));
            securityServiceClient.Setup(s => s.GetToken(It.IsAny<String>(), It.IsAny<String>(), It.IsAny<CancellationToken>())).ReturnsAsync(Result.Success(TestData.TokenResponse));

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
            transactionProcessorClient.Setup(t => t.PerformTransaction(It.IsAny<String>(), It.IsAny<ReconciliationRequest>(), It.IsAny<CancellationToken>()))
                                      .ReturnsAsync(TestData.ClientReconciliationResponse);
            securityServiceClient.Setup(s => s.GetToken(It.IsAny<String>(), It.IsAny<String>(), It.IsAny<CancellationToken>())).ReturnsAsync(Result.Success(TestData.TokenResponse));

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

            transactionProcessorClient.Setup(t => t.PerformTransaction(It.IsAny<String>(), It.IsAny<ReconciliationRequest>(), It.IsAny<CancellationToken>()))
                                      .Callback<String, ReconciliationRequest, CancellationToken>((_, message, _) => capturedMessage = message)
                                      .ReturnsAsync(TestData.ClientReconciliationResponse);
            securityServiceClient.Setup(s => s.GetToken(It.IsAny<String>(), It.IsAny<String>(), It.IsAny<CancellationToken>())).ReturnsAsync(Result.Success(TestData.TokenResponse));

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
            securityServiceClient.Setup(s => s.GetToken(It.IsAny<String>(), It.IsAny<String>(), It.IsAny<CancellationToken>())).ReturnsAsync(Result.Failure());

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
            securityServiceClient.Setup(s => s.GetToken(It.IsAny<String>(), It.IsAny<String>(), It.IsAny<CancellationToken>())).ReturnsAsync(Result.Success(TestData.TokenResponse));
            transactionProcessorClient.Setup(t => t.PerformTransaction(It.IsAny<String>(), It.IsAny<ReconciliationRequest>(), It.IsAny<CancellationToken>()))
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
            transactionProcessorClient.Setup(t => t.PerformTransaction(It.IsAny<String>(), It.IsAny<ReconciliationRequest>(), It.IsAny<CancellationToken>()))
                                      .ThrowsAsync(new Exception("Error"));
            securityServiceClient.Setup(s => s.GetToken(It.IsAny<String>(), It.IsAny<String>(), It.IsAny<CancellationToken>())).ReturnsAsync(Result.Success(TestData.TokenResponse));

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
            this.transactionProcessorClient.Setup(v => v.GetVoucherByCode(It.IsAny<String>(), It.IsAny<Guid>(), It.IsAny<String>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(TestData.GetVoucherResponse);
            securityServiceClient.Setup(s => s.GetToken(It.IsAny<String>(), It.IsAny<String>(), It.IsAny<CancellationToken>())).ReturnsAsync(Result.Success(TestData.TokenResponse));

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
            securityServiceClient.Setup(s => s.GetToken(It.IsAny<String>(), It.IsAny<String>(), It.IsAny<CancellationToken>())).ReturnsAsync(Result.Failure());

            Result<GetVoucherResponse> voucherResponse = await applicationService.GetVoucher(TestData.EstateId, TestData.ContractId, TestData.VoucherCode, CancellationToken.None);

            voucherResponse.IsFailed.ShouldBeTrue();
        }

        [Fact]
        public async Task TransactionProcessorACLApplicationService_GetVoucher_GetVoucherFailed_ResultIsFailed()
        {
            this.transactionProcessorClient.Setup(v => v.GetVoucherByCode(It.IsAny<String>(), It.IsAny<Guid>(), It.IsAny<String>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result.Failure());
            securityServiceClient.Setup(s => s.GetToken(It.IsAny<String>(), It.IsAny<String>(), It.IsAny<CancellationToken>())).ReturnsAsync(Result.Success(TestData.TokenResponse));

            Result<GetVoucherResponse> voucherResponse = await applicationService.GetVoucher(TestData.EstateId, TestData.ContractId, TestData.VoucherCode, CancellationToken.None);

            voucherResponse.IsFailed.ShouldBeTrue();
        }

        [Fact]
        public async Task TransactionProcessorACLApplicationService_GetVoucher_ExceptionErrorInGetVoucher_GetVoucherIsNotSuccessful()
        {
            transactionProcessorClient.Setup(v => v.GetVoucherByCode(It.IsAny<String>(), It.IsAny<Guid>(), It.IsAny<String>(), It.IsAny<CancellationToken>()))
                                      .ThrowsAsync(new Exception("Error"));
            securityServiceClient.Setup(s => s.GetToken(It.IsAny<String>(), It.IsAny<String>(), It.IsAny<CancellationToken>())).ReturnsAsync(Result.Success(TestData.TokenResponse));

            Result<GetVoucherResponse> voucherResponse = await applicationService.GetVoucher(TestData.EstateId, TestData.ContractId, TestData.VoucherCode, CancellationToken.None);

            voucherResponse.IsFailed.ShouldBeTrue();
        }

        [Fact]
        public async Task TransactionProcessorACLApplicationService_RedeemVoucher_VoucherRedeemed()
        {
            transactionProcessorClient.Setup(v => v.RedeemVoucher(It.IsAny<String>(), It.IsAny<RedeemVoucherRequest>(), It.IsAny<CancellationToken>()))
                                      .ReturnsAsync(Result.Success);
            securityServiceClient.Setup(s => s.GetToken(It.IsAny<String>(), It.IsAny<String>(), It.IsAny<CancellationToken>())).ReturnsAsync(Result.Success(TestData.TokenResponse));

            Result<RedeemVoucherResponse> voucherResponse = await applicationService.RedeemVoucher(TestData.EstateId, TestData.ContractId, TestData.VoucherCode, CancellationToken.None);

            voucherResponse.IsSuccess.ShouldBeTrue();
            voucherResponse.Data.ContractId.ShouldBe(TestData.ContractId);
            voucherResponse.Data.EstateId.ShouldBe(TestData.EstateId);
        }

        [Fact]
        public async Task TransactionProcessorACLApplicationService_RedeemVoucher_GetTokenFailed_ResultIsFailed()
        {
            transactionProcessorClient.Setup(v => v.RedeemVoucher(It.IsAny<String>(), It.IsAny<RedeemVoucherRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result.Failure);
            securityServiceClient.Setup(s => s.GetToken(It.IsAny<String>(), It.IsAny<String>(), It.IsAny<CancellationToken>())).ReturnsAsync(Result.Failure());

            Result<RedeemVoucherResponse> voucherResponse = await applicationService.RedeemVoucher(TestData.EstateId, TestData.ContractId, TestData.VoucherCode, CancellationToken.None);

            voucherResponse.IsFailed.ShouldBeTrue();
        }

        [Fact]
        public async Task TransactionProcessorACLApplicationService_RedeemVoucher_RedeemVoucherFailed_ResultIsFailed()
        {
            transactionProcessorClient.Setup(v => v.RedeemVoucher(It.IsAny<String>(), It.IsAny<RedeemVoucherRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result.Failure());
            securityServiceClient.Setup(s => s.GetToken(It.IsAny<String>(), It.IsAny<String>(), It.IsAny<CancellationToken>())).ReturnsAsync(Result.Success(TestData.TokenResponse));

            Result<RedeemVoucherResponse> voucherResponse = await applicationService.RedeemVoucher(TestData.EstateId, TestData.ContractId, TestData.VoucherCode, CancellationToken.None);

            voucherResponse.IsFailed.ShouldBeTrue();
        }

        [Fact]
        public async Task TransactionProcessorACLApplicationService_RedeemVoucher_ExceptionErrorInGetVoucher_GetVoucherIsNotSuccessful()
        {
             transactionProcessorClient.Setup(v => v.RedeemVoucher(It.IsAny<String>(), It.IsAny<RedeemVoucherRequest>(), It.IsAny<CancellationToken>()))
                                      .ThrowsAsync(new Exception("Error"));
            
            securityServiceClient.Setup(s => s.GetToken(It.IsAny<String>(), It.IsAny<String>(), It.IsAny<CancellationToken>())).ReturnsAsync(Result.Success(TestData.TokenResponse));

            Result<RedeemVoucherResponse> voucherResponse = await applicationService.RedeemVoucher(TestData.EstateId, TestData.ContractId, TestData.VoucherCode, CancellationToken.None);

            voucherResponse.IsFailed.ShouldBeTrue();
        }

        [Theory]
        [InlineData(TransactionProcessor.DataTransferObjects.Responses.Merchant.SettlementSchedule.Immediate)]
        [InlineData(TransactionProcessor.DataTransferObjects.Responses.Merchant.SettlementSchedule.Weekly)]
        [InlineData(TransactionProcessor.DataTransferObjects.Responses.Merchant.SettlementSchedule.Monthly)]
        public async Task TransactionProcessorACLApplicationService_GetMerchant_MerchantReturned(TransactionProcessor.DataTransferObjects.Responses.Merchant.SettlementSchedule settlementSchedule)
        {
            transactionProcessorClient.Setup(v => v.GetMerchant(It.IsAny<String>(), It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                                      .ReturnsAsync(Result.Success(TestData.MerchantResponse(settlementSchedule)));
            securityServiceClient.Setup(s => s.GetToken(It.IsAny<String>(), It.IsAny<String>(), It.IsAny<CancellationToken>())).ReturnsAsync(Result.Success(TestData.TokenResponse));

            Result<MerchantResponse> merchantResponse = await applicationService.GetMerchant(TestData.EstateId, TestData.MerchantId, CancellationToken.None);
            merchantResponse.IsSuccess.ShouldBeTrue();
            merchantResponse.Data.ShouldNotBeNull();
            merchantResponse.Data.MerchantId.ShouldBe(TestData.MerchantId);
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
            securityServiceClient.Setup(s => s.GetToken(It.IsAny<String>(), It.IsAny<String>(), It.IsAny<CancellationToken>())).ReturnsAsync(Result.Failure());

            Result<MerchantResponse> merchantResponse = await applicationService.GetMerchant(TestData.EstateId, TestData.MerchantId, CancellationToken.None);
            merchantResponse.IsFailed.ShouldBeTrue();
            
        }

        [Fact]
        public async Task TransactionProcessorACLApplicationService_GetMerchant_GetMerchantFailed_ResultIsFailed()
        {
            transactionProcessorClient.Setup(v => v.GetMerchant(It.IsAny<String>(), It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result.Failure());
            securityServiceClient.Setup(s => s.GetToken(It.IsAny<String>(), It.IsAny<String>(), It.IsAny<CancellationToken>())).ReturnsAsync(Result.Success(TestData.TokenResponse));

            Result<MerchantResponse> merchantResponse = await applicationService.GetMerchant(TestData.EstateId, TestData.MerchantId, CancellationToken.None);
            merchantResponse.IsFailed.ShouldBeTrue();
        }

        [Theory]
        [InlineData(TransactionProcessor.DataTransferObjects.Responses.Contract.ProductType.Voucher)]
        [InlineData(TransactionProcessor.DataTransferObjects.Responses.Contract.ProductType.BillPayment)]
        [InlineData(TransactionProcessor.DataTransferObjects.Responses.Contract.ProductType.MobileTopup)]
        public async Task TransactionProcessorACLApplicationService_GetMerchantContracts_MerchantContractsReturned(TransactionProcessor.DataTransferObjects.Responses.Contract.ProductType productType) {
            transactionProcessorClient.Setup(v => v.GetMerchantContracts(It.IsAny<String>(), It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result.Success(TestData.MerchantContractResponses(productType)));
            securityServiceClient.Setup(s => s.GetToken(It.IsAny<String>(), It.IsAny<String>(), It.IsAny<CancellationToken>())).ReturnsAsync(Result.Success(TestData.TokenResponse));

            var merchantContractsResponse = await applicationService.GetMerchantContracts(TestData.EstateId, TestData.MerchantId, CancellationToken.None);
            merchantContractsResponse.IsSuccess.ShouldBeTrue();
        }

        [Fact]
        public async Task TransactionProcessorACLApplicationService_GetMerchantContracts_GetTokenFailed_ResultIsFailed()
        {
            securityServiceClient.Setup(s => s.GetToken(It.IsAny<String>(), It.IsAny<String>(), It.IsAny<CancellationToken>())).ReturnsAsync(Result.Failure());

            var merchantContractsResponse = await applicationService.GetMerchantContracts(TestData.EstateId, TestData.MerchantId, CancellationToken.None);
            merchantContractsResponse.IsFailed.ShouldBeTrue();
        }

        [Fact]
        public async Task TransactionProcessorACLApplicationService_GetMerchantContracts_GetMerchantContractsFailed_ResultIsFailed()
        {
            transactionProcessorClient.Setup(v => v.GetMerchantContracts(It.IsAny<String>(), It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result.Failure());
            securityServiceClient.Setup(s => s.GetToken(It.IsAny<String>(), It.IsAny<String>(), It.IsAny<CancellationToken>())).ReturnsAsync(Result.Success(TestData.TokenResponse));

            var merchantContractsResponse = await applicationService.GetMerchantContracts(TestData.EstateId, TestData.MerchantId, CancellationToken.None);
            merchantContractsResponse.IsFailed.ShouldBeTrue();
        }

        [Fact]
        public async Task TransactionProcessorACLApplicationService_GetMerchantSchedule_MerchantScheduleReturned()
        {
            transactionProcessorClient.Setup(v => v.GetMerchantSchedule(It.IsAny<String>(), It.IsAny<Guid>(), It.IsAny<Guid>(),It.IsAny<Int32>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result.Success(TestData.MerchantScheduleResponse()));
            securityServiceClient.Setup(s => s.GetToken(It.IsAny<String>(), It.IsAny<String>(), It.IsAny<CancellationToken>())).ReturnsAsync(Result.Success(TestData.TokenResponse));

            var merchantScheduleResponse = await applicationService.GetMerchantSchedule(TestData.EstateId, TestData.MerchantId, TestData.ScheduleYear, CancellationToken.None);
            merchantScheduleResponse.IsSuccess.ShouldBeTrue();
        }

        [Fact]
        public async Task TransactionProcessorACLApplicationService_GetMerchantSchedule_GetTokenFailed_ResultIsFailed()
        {
            securityServiceClient.Setup(s => s.GetToken(It.IsAny<String>(), It.IsAny<String>(), It.IsAny<CancellationToken>())).ReturnsAsync(Result.Failure());

            var merchantScheduleResponse = await applicationService.GetMerchantSchedule(TestData.EstateId, TestData.MerchantId, TestData.ScheduleYear, CancellationToken.None);
            merchantScheduleResponse.IsFailed.ShouldBeTrue();
        }

        [Fact]
        public async Task TransactionProcessorACLApplicationService_GetMerchantSchedule_GetMerchantScheduleFailed_ResultIsFailed()
        {
            transactionProcessorClient.Setup(v => v.GetMerchantSchedule(It.IsAny<String>(), It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<Int32>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result.Failure());
            securityServiceClient.Setup(s => s.GetToken(It.IsAny<String>(), It.IsAny<String>(), It.IsAny<CancellationToken>())).ReturnsAsync(Result.Success(TestData.TokenResponse));

            var merchantScheduleResponse = await applicationService.GetMerchantSchedule(TestData.EstateId, TestData.MerchantId, TestData.ScheduleYear, CancellationToken.None);
            merchantScheduleResponse.IsFailed.ShouldBeTrue();
        }

        [Fact]
        public async Task TransactionProcessorACLApplicationService_GetMerchantDailyPerformanceSummary_ReturnedFromEstateReportingClient()
        {
            MerchantDailyPerformanceSummaryRequest capturedRequest = null;
            estateReportingApiClient
                .Setup(v => v.GetMerchantDailyPerformanceSummary(It.IsAny<String>(), It.IsAny<Guid>(), It.IsAny<MerchantDailyPerformanceSummaryRequest>(), It.IsAny<CancellationToken>()))
                .Callback<String, Guid, MerchantDailyPerformanceSummaryRequest, CancellationToken>((_, _, request, _) => capturedRequest = request)
                .ReturnsAsync(Result.Success(new MerchantDailyPerformanceSummaryResponse
                {
                    Metrics =
                    [
                        new MetricItem
                        {
                            Title = "Total Sales Count",
                            Value = 6,
                            Description = "All sales transactions in the range",
                            Category = 0
                        }
                    ]
                }));
            securityServiceClient.Setup(s => s.GetToken(It.IsAny<String>(), It.IsAny<String>(), It.IsAny<CancellationToken>())).ReturnsAsync(Result.Success(TestData.TokenResponse));

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
            securityServiceClient.Setup(s => s.GetToken(It.IsAny<String>(), It.IsAny<String>(), It.IsAny<CancellationToken>())).ReturnsAsync(Result.Failure());

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
    }
}
