using System;
using System.Collections.Generic;
using System.Text;

namespace TransactionProcessorACL.BusinesssLogic.Tests
{
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using BusinessLogic.Services;
    using Microsoft.Extensions.Configuration;
    using Models;
    using Moq;
    using SecurityService.Client;
    using Shared.General;
    using Shared.Logger;
    using Shouldly;
    using Testing;
    using TransactionProcessor.Client;
    using TransactionProcessor.DataTransferObjects;
    using Xunit;
    using GetVoucherResponse = Models.GetVoucherResponse;
    using RedeemVoucherResponse = Models.RedeemVoucherResponse;

    public class TransactionProcessorACLApplicationServiceTests{
        private Mock<ITransactionProcessorClient> transactionProcessorClient;

        private Mock<ISecurityServiceClient> securityServiceClient;

        private ITransactionProcessorACLApplicationService applicationService;
        public TransactionProcessorACLApplicationServiceTests()
        {
            Logger.Initialise(new NullLogger());

            this.SetupMemoryConfiguration();

            transactionProcessorClient = new Mock<ITransactionProcessorClient>();
            securityServiceClient = new Mock<ISecurityServiceClient>();

            applicationService =
                new TransactionProcessorACLApplicationService(transactionProcessorClient.Object, securityServiceClient.Object);
        }

        private void SetupMemoryConfiguration()
        {
            if (ConfigurationReader.IsInitialised == false)
            {
                IConfigurationRoot configuration = new ConfigurationBuilder().AddInMemoryCollection(TestData.DefaultAppSettings).Build();
                ConfigurationReader.Initialise(configuration);
            }
        }

        [Fact]
        public async Task TransactionProcessorACLApplicationService_ProcessLogonTransaction_TransactionIsSuccessful()
        {
            transactionProcessorClient.Setup(t => t.PerformTransaction(It.IsAny<String>(), It.IsAny<SerialisedMessage>(), It.IsAny<CancellationToken>()))
                                      .ReturnsAsync(TestData.SerialisedMessageResponseLogon);
            
            securityServiceClient.Setup(s => s.GetToken(It.IsAny<String>(), It.IsAny<String>(), It.IsAny<CancellationToken>())).ReturnsAsync(TestData.TokenResponse);
            
            ProcessLogonTransactionResponse logonResponse = await applicationService.ProcessLogonTransaction(TestData.EstateId,
                                                                                                             TestData.MerchantId,
                                                                                                             TestData.TransactionDateTime,
                                                                                                             TestData.TransactionNumber,
                                                                                                             TestData.DeviceIdentifier,
                                                                                                             CancellationToken.None);

            logonResponse.ShouldNotBeNull();
            logonResponse.ResponseMessage.ShouldBe(TestData.ResponseMessage);
            logonResponse.ResponseCode.ShouldBe(TestData.ResponseCode);
        }

        [Fact]
        public async Task TransactionProcessorACLApplicationService_ProcessLogonTransaction_InvalidOperationExceptionErrorInLogon_TransactionIsNotSuccessful()
        {
            transactionProcessorClient.Setup(t => t.PerformTransaction(It.IsAny<String>(), It.IsAny<SerialisedMessage>(), It.IsAny<CancellationToken>()))
                                      .ThrowsAsync(new Exception("Error", new InvalidOperationException(TestData.InvalidOperationErrorResponseMessage)));
            securityServiceClient.Setup(s => s.GetToken(It.IsAny<String>(), It.IsAny<String>(), It.IsAny<CancellationToken>())).ReturnsAsync(TestData.TokenResponse);
            
            ProcessLogonTransactionResponse logonResponse = await applicationService.ProcessLogonTransaction(TestData.EstateId,
                                                                                                             TestData.MerchantId,
                                                                                                             TestData.TransactionDateTime,
                                                                                                             TestData.TransactionNumber,
                                                                                                             TestData.DeviceIdentifier,
                                                                                                             CancellationToken.None);

            logonResponse.ShouldNotBeNull();
            logonResponse.ResponseMessage.ShouldBe(TestData.InvalidOperationErrorResponseMessage);
            logonResponse.ResponseCode.ShouldBe(TestData.InvalidOperationErrorResponseCode);
        }

        [Fact]
        public async Task TransactionProcessorACLApplicationService_ProcessLogonTransaction_HttpRequestExceptionErrorInLogon_TransactionIsNotSuccessful()
        {
            transactionProcessorClient.Setup(t => t.PerformTransaction(It.IsAny<String>(), It.IsAny<SerialisedMessage>(), It.IsAny<CancellationToken>()))
                                      .ThrowsAsync(new Exception("Error", new HttpRequestException(TestData.HttpRequestErrorResponseMessage)));
            securityServiceClient.Setup(s => s.GetToken(It.IsAny<String>(), It.IsAny<String>(), It.IsAny<CancellationToken>())).ReturnsAsync(TestData.TokenResponse);
            
            ProcessLogonTransactionResponse logonResponse = await applicationService.ProcessLogonTransaction(TestData.EstateId,
                                                                                                             TestData.MerchantId,
                                                                                                             TestData.TransactionDateTime,
                                                                                                             TestData.TransactionNumber,
                                                                                                             TestData.DeviceIdentifier,
                                                                                                             CancellationToken.None);

            logonResponse.ShouldNotBeNull();
            logonResponse.ResponseMessage.ShouldContain(TestData.HttpRequestErrorResponseMessage);
            logonResponse.ResponseCode.ShouldBe(TestData.HttpRequestErrorResponseCode);
        }

        [Fact]
        public async Task TransactionProcessorACLApplicationService_ProcessLogonTransaction_OtherExceptionErrorInLogon_TransactionIsNotSuccessful()
        {
            transactionProcessorClient.Setup(t => t.PerformTransaction(It.IsAny<String>(), It.IsAny<SerialisedMessage>(), It.IsAny<CancellationToken>()))
                                      .ThrowsAsync(new Exception("Error", new Exception(TestData.GeneralErrorResponseMessage)));
            securityServiceClient.Setup(s => s.GetToken(It.IsAny<String>(), It.IsAny<String>(), It.IsAny<CancellationToken>())).ReturnsAsync(TestData.TokenResponse);

            ProcessLogonTransactionResponse logonResponse = await applicationService.ProcessLogonTransaction(TestData.EstateId,
                                                                                                             TestData.MerchantId,
                                                                                                             TestData.TransactionDateTime,
                                                                                                             TestData.TransactionNumber,
                                                                                                             TestData.DeviceIdentifier,
                                                                                                             CancellationToken.None);

            logonResponse.ShouldNotBeNull();
            logonResponse.ResponseMessage.ShouldBe(TestData.GeneralErrorResponseMessage);
            logonResponse.ResponseCode.ShouldBe(TestData.GeneralErrorResponseCode);
        }

        [Fact]
        public async Task TransactionProcessorACLApplicationService_ProcessSaleTransaction_TransactionIsSuccessful()
        {
            transactionProcessorClient.Setup(t => t.PerformTransaction(It.IsAny<String>(), It.IsAny<SerialisedMessage>(), It.IsAny<CancellationToken>()))
                                      .ReturnsAsync(TestData.SerialisedMessageResponseSale);
            securityServiceClient.Setup(s => s.GetToken(It.IsAny<String>(), It.IsAny<String>(), It.IsAny<CancellationToken>())).ReturnsAsync(TestData.TokenResponse);
            
            ProcessSaleTransactionResponse saleResponse = await applicationService.ProcessSaleTransaction(TestData.EstateId,
                                                                                                             TestData.MerchantId,
                                                                                                             TestData.TransactionDateTime,
                                                                                                             TestData.TransactionNumber,
                                                                                                             TestData.DeviceIdentifier,
                                                                                                             TestData.OperatorIdentifier,
                                                                                                             TestData.CustomerEmailAddress,
                                                                                                             TestData.ContractId,
                                                                                                             TestData.ProductId,
                                                                                                             TestData.AdditionalRequestMetadata,
                                                                                                             CancellationToken.None);

            saleResponse.ShouldNotBeNull();
            saleResponse.ResponseMessage.ShouldBe(TestData.ResponseMessage);
            saleResponse.ResponseCode.ShouldBe(TestData.ResponseCode);
        }

        [Fact]
        public async Task TransactionProcessorACLApplicationService_ProcessSaleTransaction_InvalidOperationExceptionErrorInSale_TransactionIsNotSuccessful()
        {
            transactionProcessorClient.Setup(t => t.PerformTransaction(It.IsAny<String>(), It.IsAny<SerialisedMessage>(), It.IsAny<CancellationToken>()))
                                      .ThrowsAsync(new Exception("Error", new InvalidOperationException(TestData.InvalidOperationErrorResponseMessage)));
            securityServiceClient.Setup(s => s.GetToken(It.IsAny<String>(), It.IsAny<String>(), It.IsAny<CancellationToken>())).ReturnsAsync(TestData.TokenResponse);
            
            ProcessSaleTransactionResponse saleResponse = await applicationService.ProcessSaleTransaction(TestData.EstateId,
                                                                                                          TestData.MerchantId,
                                                                                                          TestData.TransactionDateTime,
                                                                                                          TestData.TransactionNumber,
                                                                                                          TestData.DeviceIdentifier,
                                                                                                          TestData.OperatorIdentifier,
                                                                                                          TestData.CustomerEmailAddress,
                                                                                                          TestData.ContractId,
                                                                                                          TestData.ProductId,
                                                                                                          TestData.AdditionalRequestMetadata,
                                                                                                          CancellationToken.None);

            saleResponse.ShouldNotBeNull();
            saleResponse.ResponseMessage.ShouldBe(TestData.InvalidOperationErrorResponseMessage);
            saleResponse.ResponseCode.ShouldBe(TestData.InvalidOperationErrorResponseCode);
        }

        [Fact]
        public async Task TransactionProcessorACLApplicationService_ProcessSaleTransaction_HttpRequestExceptionErrorInSale_TransactionIsNotSuccessful()
        {
            transactionProcessorClient.Setup(t => t.PerformTransaction(It.IsAny<String>(), It.IsAny<SerialisedMessage>(), It.IsAny<CancellationToken>()))
                                      .ThrowsAsync(new Exception("Error", new HttpRequestException(TestData.HttpRequestErrorResponseMessage)));
            securityServiceClient.Setup(s => s.GetToken(It.IsAny<String>(), It.IsAny<String>(), It.IsAny<CancellationToken>())).ReturnsAsync(TestData.TokenResponse);
            
            ProcessSaleTransactionResponse saleResponse = await applicationService.ProcessSaleTransaction(TestData.EstateId,
                                                                                                          TestData.MerchantId,
                                                                                                          TestData.TransactionDateTime,
                                                                                                          TestData.TransactionNumber,
                                                                                                          TestData.DeviceIdentifier,
                                                                                                          TestData.OperatorIdentifier,
                                                                                                          TestData.CustomerEmailAddress,
                                                                                                          TestData.ContractId,
                                                                                                          TestData.ProductId,
                                                                                                          TestData.AdditionalRequestMetadata,
                                                                                                          CancellationToken.None);

            saleResponse.ShouldNotBeNull();
            saleResponse.ResponseMessage.ShouldContain(TestData.HttpRequestErrorResponseMessage);
            saleResponse.ResponseCode.ShouldBe(TestData.HttpRequestErrorResponseCode);
        }

        [Fact]
        public async Task TransactionProcessorACLApplicationService_ProcessSaleTransaction_OtherExceptionErrorInSale_TransactionIsNotSuccessful()
        {
            transactionProcessorClient.Setup(t => t.PerformTransaction(It.IsAny<String>(), It.IsAny<SerialisedMessage>(), It.IsAny<CancellationToken>()))
                                      .ThrowsAsync(new Exception("Error", new Exception(TestData.GeneralErrorResponseMessage)));
            securityServiceClient.Setup(s => s.GetToken(It.IsAny<String>(), It.IsAny<String>(), It.IsAny<CancellationToken>())).ReturnsAsync(TestData.TokenResponse);
            
            ProcessSaleTransactionResponse saleResponse = await applicationService.ProcessSaleTransaction(TestData.EstateId,
                                                                                                          TestData.MerchantId,
                                                                                                          TestData.TransactionDateTime,
                                                                                                          TestData.TransactionNumber,
                                                                                                          TestData.DeviceIdentifier,
                                                                                                          TestData.OperatorIdentifier,
                                                                                                          TestData.CustomerEmailAddress,
                                                                                                          TestData.ContractId,
                                                                                                          TestData.ProductId,
                                                                                                          TestData.AdditionalRequestMetadata,
                                                                                                          CancellationToken.None);

            saleResponse.ShouldNotBeNull();
            saleResponse.ResponseMessage.ShouldBe(TestData.GeneralErrorResponseMessage);
            saleResponse.ResponseCode.ShouldBe(TestData.GeneralErrorResponseCode);
        }

        [Fact]
        public async Task TransactionProcessorACLApplicationService_ProcessReconciliation_TransactionIsSuccessful()
        {
            transactionProcessorClient.Setup(t => t.PerformTransaction(It.IsAny<String>(), It.IsAny<SerialisedMessage>(), It.IsAny<CancellationToken>()))
                                      .ReturnsAsync(TestData.SerialisedMessageResponseReconciliation);
            securityServiceClient.Setup(s => s.GetToken(It.IsAny<String>(), It.IsAny<String>(), It.IsAny<CancellationToken>())).ReturnsAsync(TestData.TokenResponse);

            ProcessReconciliationResponse reconciliationResponse = await applicationService.ProcessReconciliation(TestData.EstateId,
                TestData.MerchantId,
                TestData.TransactionDateTime,
                TestData.DeviceIdentifier,
                TestData.ReconciliationTransactionCount,
                TestData.ReconciliationTransactionValue,
                CancellationToken.None);

            reconciliationResponse.ShouldNotBeNull();
            reconciliationResponse.ResponseMessage.ShouldBe(TestData.ResponseMessage);
            reconciliationResponse.ResponseCode.ShouldBe(TestData.ResponseCode);
        }

        [Fact]
        public async Task TransactionProcessorACLApplicationService_ProcessReconciliation_InvalidOperationExceptionErrorInReconciliation_TransactionIsNotSuccessful()
        {
            transactionProcessorClient.Setup(t => t.PerformTransaction(It.IsAny<String>(), It.IsAny<SerialisedMessage>(), It.IsAny<CancellationToken>()))
                                      .ThrowsAsync(new Exception("Error", new InvalidOperationException(TestData.InvalidOperationErrorResponseMessage)));
            securityServiceClient.Setup(s => s.GetToken(It.IsAny<String>(), It.IsAny<String>(), It.IsAny<CancellationToken>())).ReturnsAsync(TestData.TokenResponse);
            
            ProcessReconciliationResponse reconciliationResponse = await applicationService.ProcessReconciliation(TestData.EstateId,
                TestData.MerchantId,
                TestData.TransactionDateTime,
                TestData.DeviceIdentifier,
                TestData.ReconciliationTransactionCount,
                TestData.ReconciliationTransactionValue,
                CancellationToken.None);

            reconciliationResponse.ShouldNotBeNull();
            reconciliationResponse.ResponseMessage.ShouldBe(TestData.InvalidOperationErrorResponseMessage);
            reconciliationResponse.ResponseCode.ShouldBe(TestData.InvalidOperationErrorResponseCode);
        }

        [Fact]
        public async Task TransactionProcessorACLApplicationService_ProcessReconciliation_HttpRequestExceptionErrorInReconciliation_TransactionIsNotSuccessful()
        {
            transactionProcessorClient.Setup(t => t.PerformTransaction(It.IsAny<String>(), It.IsAny<SerialisedMessage>(), It.IsAny<CancellationToken>()))
                                      .ThrowsAsync(new Exception("Error", new HttpRequestException(TestData.HttpRequestErrorResponseMessage)));
            securityServiceClient.Setup(s => s.GetToken(It.IsAny<String>(), It.IsAny<String>(), It.IsAny<CancellationToken>())).ReturnsAsync(TestData.TokenResponse);
            
            ProcessReconciliationResponse reconciliationResponse = await applicationService.ProcessReconciliation(TestData.EstateId,
                TestData.MerchantId,
                TestData.TransactionDateTime,
                TestData.DeviceIdentifier,
                TestData.ReconciliationTransactionCount,
                TestData.ReconciliationTransactionValue,
                CancellationToken.None);

            reconciliationResponse.ShouldNotBeNull();
            reconciliationResponse.ResponseMessage.ShouldContain(TestData.HttpRequestErrorResponseMessage);
            reconciliationResponse.ResponseCode.ShouldBe(TestData.HttpRequestErrorResponseCode);
        }

        [Fact]
        public async Task TransactionProcessorACLApplicationService_ProcessReconciliation_OtherExceptionErrorInReconciliation_TransactionIsNotSuccessful()
        {
            transactionProcessorClient.Setup(t => t.PerformTransaction(It.IsAny<String>(), It.IsAny<SerialisedMessage>(), It.IsAny<CancellationToken>()))
                                      .ThrowsAsync(new Exception("Error", new Exception(TestData.GeneralErrorResponseMessage)));
            securityServiceClient.Setup(s => s.GetToken(It.IsAny<String>(), It.IsAny<String>(), It.IsAny<CancellationToken>())).ReturnsAsync(TestData.TokenResponse);
            
            ProcessReconciliationResponse reconciliationResponse = await applicationService.ProcessReconciliation(TestData.EstateId,
                                                                                                           TestData.MerchantId,
                                                                                                           TestData.TransactionDateTime,
                                                                                                           TestData.DeviceIdentifier,
                                                                                                           TestData.ReconciliationTransactionCount,
                                                                                                           TestData.ReconciliationTransactionValue,
                                                                                                           CancellationToken.None);

            reconciliationResponse.ShouldNotBeNull();
            reconciliationResponse.ResponseMessage.ShouldBe(TestData.GeneralErrorResponseMessage);
            reconciliationResponse.ResponseCode.ShouldBe(TestData.GeneralErrorResponseCode);
        }

        [Fact]
        public async Task VoucherManagementACLApplicationService_GetVoucher_VoucherRetrieved()
        {
            this.transactionProcessorClient.Setup(v => v.GetVoucherByCode(It.IsAny<String>(), It.IsAny<Guid>(), It.IsAny<String>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(TestData.GetVoucherResponse);
            securityServiceClient.Setup(s => s.GetToken(It.IsAny<String>(), It.IsAny<String>(), It.IsAny<CancellationToken>())).ReturnsAsync(TestData.TokenResponse);
            
            GetVoucherResponse voucherResponse = await applicationService.GetVoucher(TestData.EstateId, TestData.ContractId, TestData.VoucherCode, CancellationToken.None);

            voucherResponse.ShouldNotBeNull();
            voucherResponse.VoucherCode.ShouldBe(TestData.GetVoucherResponse.VoucherCode);
            voucherResponse.ContractId.ShouldBe(TestData.ContractId);
            voucherResponse.EstateId.ShouldBe(TestData.EstateId);
            voucherResponse.ExpiryDate.ShouldBe(TestData.GetVoucherResponse.ExpiryDate);
            voucherResponse.Value.ShouldBe(TestData.GetVoucherResponse.Value);
            voucherResponse.VoucherId.ShouldBe(TestData.GetVoucherResponse.VoucherId);
        }

        [Fact]
        public async Task VoucherManagementACLApplicationService_GetVoucher_InvalidOperationExceptionErrorInGetVoucher_GetVoucherIsNotSuccessful()
        {
            transactionProcessorClient.Setup(v => v.GetVoucherByCode(It.IsAny<String>(), It.IsAny<Guid>(), It.IsAny<String>(), It.IsAny<CancellationToken>()))
                                      .ThrowsAsync(new Exception("Error", new InvalidOperationException(TestData.InvalidOperationErrorResponseMessage)));
            securityServiceClient.Setup(s => s.GetToken(It.IsAny<String>(), It.IsAny<String>(), It.IsAny<CancellationToken>())).ReturnsAsync(TestData.TokenResponse);
            
            GetVoucherResponse voucherResponse = await applicationService.GetVoucher(TestData.EstateId, TestData.ContractId, TestData.VoucherCode, CancellationToken.None);

            voucherResponse.ShouldNotBeNull();
            voucherResponse.ResponseMessage.ShouldBe(TestData.InvalidOperationErrorResponseMessage);
            voucherResponse.ResponseCode.ShouldBe(TestData.InvalidOperationErrorResponseCode);
        }

        [Fact]
        public async Task VoucherManagementACLApplicationService_GetVoucher_HttpRequestExceptionErrorInGetVoucher_GetVoucherNotSuccessful()
        {
            transactionProcessorClient.Setup(v => v.GetVoucherByCode(It.IsAny<String>(), It.IsAny<Guid>(), It.IsAny<String>(), It.IsAny<CancellationToken>()))
                                      .ThrowsAsync(new Exception("Error", new HttpRequestException(TestData.HttpRequestErrorResponseMessage)));
            securityServiceClient.Setup(s => s.GetToken(It.IsAny<String>(), It.IsAny<String>(), It.IsAny<CancellationToken>())).ReturnsAsync(TestData.TokenResponse);

            GetVoucherResponse voucherResponse = await applicationService.GetVoucher(TestData.EstateId, TestData.ContractId, TestData.VoucherCode, CancellationToken.None);

            voucherResponse.ShouldNotBeNull();
            voucherResponse.ResponseMessage.ShouldBe(TestData.HttpRequestErrorResponseMessage);
            voucherResponse.ResponseCode.ShouldBe(TestData.HttpRequestErrorResponseCode);
        }

        [Fact]
        public async Task VoucherManagementACLApplicationService_GetVoucher_OtherExceptionErrorInInGetVoucher_GetVoucherNotSuccessful()
        {
            transactionProcessorClient.Setup(v => v.GetVoucherByCode(It.IsAny<String>(), It.IsAny<Guid>(), It.IsAny<String>(), It.IsAny<CancellationToken>()))
                                      .ThrowsAsync(new Exception("Error", new Exception(TestData.GeneralErrorResponseMessage)));
            securityServiceClient.Setup(s => s.GetToken(It.IsAny<String>(), It.IsAny<String>(), It.IsAny<CancellationToken>())).ReturnsAsync(TestData.TokenResponse);
            
            GetVoucherResponse voucherResponse = await applicationService.GetVoucher(TestData.EstateId, TestData.ContractId, TestData.VoucherCode, CancellationToken.None);

            voucherResponse.ShouldNotBeNull();
            voucherResponse.ResponseMessage.ShouldBe(TestData.GeneralErrorResponseMessage);
            voucherResponse.ResponseCode.ShouldBe(TestData.GeneralErrorResponseCode);
        }

        [Fact]
        public async Task VoucherManagementACLApplicationService_RedeemVoucher_VoucherRedeemed()
        {
            transactionProcessorClient.Setup(v => v.RedeemVoucher(It.IsAny<String>(), It.IsAny<RedeemVoucherRequest>(), It.IsAny<CancellationToken>()))
                                      .ReturnsAsync(TestData.RedeemVoucherResponse);
            securityServiceClient.Setup(s => s.GetToken(It.IsAny<String>(), It.IsAny<String>(), It.IsAny<CancellationToken>())).ReturnsAsync(TestData.TokenResponse);
            
            RedeemVoucherResponse voucherResponse = await applicationService.RedeemVoucher(TestData.EstateId, TestData.ContractId, TestData.VoucherCode, CancellationToken.None);

            voucherResponse.ShouldNotBeNull();
            voucherResponse.VoucherCode.ShouldBe(TestData.RedeemVoucherResponse.VoucherCode);
            voucherResponse.ContractId.ShouldBe(TestData.ContractId);
            voucherResponse.EstateId.ShouldBe(TestData.EstateId);
            voucherResponse.ExpiryDate.ShouldBe(TestData.RedeemVoucherResponse.ExpiryDate);
            voucherResponse.Balance.ShouldBe(TestData.RedeemVoucherResponse.RemainingBalance);
        }

        [Fact]
        public async Task VoucherManagementACLApplicationService_RedeemVoucher_InvalidOperationExceptionErrorInGetVoucher_GetVoucherIsNotSuccessful()
        {
            transactionProcessorClient.Setup(v => v.RedeemVoucher(It.IsAny<String>(), It.IsAny<RedeemVoucherRequest>(), It.IsAny<CancellationToken>()))
                                      .ThrowsAsync(new Exception("Error", new InvalidOperationException(TestData.InvalidOperationErrorResponseMessage)));
            
            securityServiceClient.Setup(s => s.GetToken(It.IsAny<String>(), It.IsAny<String>(), It.IsAny<CancellationToken>())).ReturnsAsync(TestData.TokenResponse);
            
            RedeemVoucherResponse voucherResponse = await applicationService.RedeemVoucher(TestData.EstateId, TestData.ContractId, TestData.VoucherCode, CancellationToken.None);

            voucherResponse.ShouldNotBeNull();
            voucherResponse.ResponseMessage.ShouldBe(TestData.InvalidOperationErrorResponseMessage);
            voucherResponse.ResponseCode.ShouldBe(TestData.InvalidOperationErrorResponseCode);
        }

        [Fact]
        public async Task VoucherManagementACLApplicationService_RedeemVoucher_HttpRequestExceptionErrorInGetVoucher_GetVoucherNotSuccessful()
        {
            transactionProcessorClient.Setup(v => v.RedeemVoucher(It.IsAny<String>(), It.IsAny<RedeemVoucherRequest>(), It.IsAny<CancellationToken>()))
                                      .ThrowsAsync(new Exception("Error", new HttpRequestException(TestData.HttpRequestErrorResponseMessage)));
            securityServiceClient.Setup(s => s.GetToken(It.IsAny<String>(), It.IsAny<String>(), It.IsAny<CancellationToken>())).ReturnsAsync(TestData.TokenResponse);
            
            RedeemVoucherResponse voucherResponse = await applicationService.RedeemVoucher(TestData.EstateId, TestData.ContractId, TestData.VoucherCode, CancellationToken.None);

            voucherResponse.ShouldNotBeNull();
            voucherResponse.ResponseMessage.ShouldBe(TestData.HttpRequestErrorResponseMessage);
            voucherResponse.ResponseCode.ShouldBe(TestData.HttpRequestErrorResponseCode);
        }

        [Fact]
        public async Task VoucherManagementACLApplicationService_RedeemVoucher_OtherExceptionErrorInInGetVoucher_GetVoucherNotSuccessful()
        {
            transactionProcessorClient.Setup(v => v.RedeemVoucher(It.IsAny<String>(), It.IsAny<RedeemVoucherRequest>(), It.IsAny<CancellationToken>()))
                                      .ThrowsAsync(new Exception("Error", new Exception(TestData.GeneralErrorResponseMessage)));
            
            securityServiceClient.Setup(s => s.GetToken(It.IsAny<String>(), It.IsAny<String>(), It.IsAny<CancellationToken>())).ReturnsAsync(TestData.TokenResponse);
            
            RedeemVoucherResponse voucherResponse = await applicationService.RedeemVoucher(TestData.EstateId, TestData.ContractId, TestData.VoucherCode, CancellationToken.None);

            voucherResponse.ShouldNotBeNull();
            voucherResponse.ResponseMessage.ShouldBe(TestData.GeneralErrorResponseMessage);
            voucherResponse.ResponseCode.ShouldBe(TestData.GeneralErrorResponseCode);
        }
    }
}
