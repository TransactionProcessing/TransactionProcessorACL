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

    public class TransactionProcessorACLApplicationServiceTests
    {
        public TransactionProcessorACLApplicationServiceTests()
        {
            this.SetupMemoryConfiguration();
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
            Mock<ITransactionProcessorClient> transactionProcessorClient = new Mock<ITransactionProcessorClient>();
            transactionProcessorClient.Setup(t => t.PerformTransaction(It.IsAny<String>(), It.IsAny<SerialisedMessage>(), It.IsAny<CancellationToken>()))
                                      .ReturnsAsync(TestData.SerialisedMessageResponseLogon);
            Mock<ISecurityServiceClient> securityServiceClient = new Mock<ISecurityServiceClient>();
            securityServiceClient.Setup(s => s.GetToken(It.IsAny<String>(), It.IsAny<String>(), It.IsAny<CancellationToken>())).ReturnsAsync(TestData.TokenResponse);

            ITransactionProcessorACLApplicationService applicationService =
                new TransactionProcessorACLApplicationService(transactionProcessorClient.Object, securityServiceClient.Object);

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
            Mock<ITransactionProcessorClient> transactionProcessorClient = new Mock<ITransactionProcessorClient>();
            transactionProcessorClient.Setup(t => t.PerformTransaction(It.IsAny<String>(), It.IsAny<SerialisedMessage>(), It.IsAny<CancellationToken>()))
                                      .ThrowsAsync(new Exception("Error", new InvalidOperationException(TestData.InvalidOperationErrorResponseMessage)));
            Mock<ISecurityServiceClient> securityServiceClient = new Mock<ISecurityServiceClient>();
            securityServiceClient.Setup(s => s.GetToken(It.IsAny<String>(), It.IsAny<String>(), It.IsAny<CancellationToken>())).ReturnsAsync(TestData.TokenResponse);

            ITransactionProcessorACLApplicationService applicationService =
                new TransactionProcessorACLApplicationService(transactionProcessorClient.Object, securityServiceClient.Object);

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
            Logger.Initialise(new NlogLogger());

            Mock<ITransactionProcessorClient> transactionProcessorClient = new Mock<ITransactionProcessorClient>();
            transactionProcessorClient.Setup(t => t.PerformTransaction(It.IsAny<String>(), It.IsAny<SerialisedMessage>(), It.IsAny<CancellationToken>()))
                                      .ThrowsAsync(new Exception("Error", new HttpRequestException(TestData.HttpRequestErrorResponseMessage)));
            Mock<ISecurityServiceClient> securityServiceClient = new Mock<ISecurityServiceClient>();
            securityServiceClient.Setup(s => s.GetToken(It.IsAny<String>(), It.IsAny<String>(), It.IsAny<CancellationToken>())).ReturnsAsync(TestData.TokenResponse);

            ITransactionProcessorACLApplicationService applicationService =
                new TransactionProcessorACLApplicationService(transactionProcessorClient.Object, securityServiceClient.Object);

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
            Mock<ITransactionProcessorClient> transactionProcessorClient = new Mock<ITransactionProcessorClient>();
            transactionProcessorClient.Setup(t => t.PerformTransaction(It.IsAny<String>(), It.IsAny<SerialisedMessage>(), It.IsAny<CancellationToken>()))
                                      .ThrowsAsync(new Exception("Error", new Exception(TestData.GeneralErrorResponseMessage)));
            Mock<ISecurityServiceClient> securityServiceClient = new Mock<ISecurityServiceClient>();
            securityServiceClient.Setup(s => s.GetToken(It.IsAny<String>(), It.IsAny<String>(), It.IsAny<CancellationToken>())).ReturnsAsync(TestData.TokenResponse);

            ITransactionProcessorACLApplicationService applicationService =
                new TransactionProcessorACLApplicationService(transactionProcessorClient.Object, securityServiceClient.Object);

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
            Mock<ITransactionProcessorClient> transactionProcessorClient = new Mock<ITransactionProcessorClient>();
            transactionProcessorClient.Setup(t => t.PerformTransaction(It.IsAny<String>(), It.IsAny<SerialisedMessage>(), It.IsAny<CancellationToken>()))
                                      .ReturnsAsync(TestData.SerialisedMessageResponseSale);
            Mock<ISecurityServiceClient> securityServiceClient = new Mock<ISecurityServiceClient>();
            securityServiceClient.Setup(s => s.GetToken(It.IsAny<String>(), It.IsAny<String>(), It.IsAny<CancellationToken>())).ReturnsAsync(TestData.TokenResponse);

            ITransactionProcessorACLApplicationService applicationService =
                new TransactionProcessorACLApplicationService(transactionProcessorClient.Object, securityServiceClient.Object);

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
            Mock<ITransactionProcessorClient> transactionProcessorClient = new Mock<ITransactionProcessorClient>();
            transactionProcessorClient.Setup(t => t.PerformTransaction(It.IsAny<String>(), It.IsAny<SerialisedMessage>(), It.IsAny<CancellationToken>()))
                                      .ThrowsAsync(new Exception("Error", new InvalidOperationException(TestData.InvalidOperationErrorResponseMessage)));
            Mock<ISecurityServiceClient> securityServiceClient = new Mock<ISecurityServiceClient>();
            securityServiceClient.Setup(s => s.GetToken(It.IsAny<String>(), It.IsAny<String>(), It.IsAny<CancellationToken>())).ReturnsAsync(TestData.TokenResponse);

            ITransactionProcessorACLApplicationService applicationService =
                new TransactionProcessorACLApplicationService(transactionProcessorClient.Object, securityServiceClient.Object);

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
            Mock<ITransactionProcessorClient> transactionProcessorClient = new Mock<ITransactionProcessorClient>();
            transactionProcessorClient.Setup(t => t.PerformTransaction(It.IsAny<String>(), It.IsAny<SerialisedMessage>(), It.IsAny<CancellationToken>()))
                                      .ThrowsAsync(new Exception("Error", new HttpRequestException(TestData.HttpRequestErrorResponseMessage)));
            Mock<ISecurityServiceClient> securityServiceClient = new Mock<ISecurityServiceClient>();
            securityServiceClient.Setup(s => s.GetToken(It.IsAny<String>(), It.IsAny<String>(), It.IsAny<CancellationToken>())).ReturnsAsync(TestData.TokenResponse);

            ITransactionProcessorACLApplicationService applicationService =
                new TransactionProcessorACLApplicationService(transactionProcessorClient.Object, securityServiceClient.Object);

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
            Mock<ITransactionProcessorClient> transactionProcessorClient = new Mock<ITransactionProcessorClient>();
            transactionProcessorClient.Setup(t => t.PerformTransaction(It.IsAny<String>(), It.IsAny<SerialisedMessage>(), It.IsAny<CancellationToken>()))
                                      .ThrowsAsync(new Exception("Error", new Exception(TestData.GeneralErrorResponseMessage)));
            Mock<ISecurityServiceClient> securityServiceClient = new Mock<ISecurityServiceClient>();
            securityServiceClient.Setup(s => s.GetToken(It.IsAny<String>(), It.IsAny<String>(), It.IsAny<CancellationToken>())).ReturnsAsync(TestData.TokenResponse);

            ITransactionProcessorACLApplicationService applicationService =
                new TransactionProcessorACLApplicationService(transactionProcessorClient.Object, securityServiceClient.Object);

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
            Mock<ITransactionProcessorClient> transactionProcessorClient = new Mock<ITransactionProcessorClient>();
            transactionProcessorClient.Setup(t => t.PerformTransaction(It.IsAny<String>(), It.IsAny<SerialisedMessage>(), It.IsAny<CancellationToken>()))
                                      .ReturnsAsync(TestData.SerialisedMessageResponseReconciliation);
            Mock<ISecurityServiceClient> securityServiceClient = new Mock<ISecurityServiceClient>();
            securityServiceClient.Setup(s => s.GetToken(It.IsAny<String>(), It.IsAny<String>(), It.IsAny<CancellationToken>())).ReturnsAsync(TestData.TokenResponse);

            ITransactionProcessorACLApplicationService applicationService =
                new TransactionProcessorACLApplicationService(transactionProcessorClient.Object, securityServiceClient.Object);

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
            Mock<ITransactionProcessorClient> transactionProcessorClient = new Mock<ITransactionProcessorClient>();
            transactionProcessorClient.Setup(t => t.PerformTransaction(It.IsAny<String>(), It.IsAny<SerialisedMessage>(), It.IsAny<CancellationToken>()))
                                      .ThrowsAsync(new Exception("Error", new InvalidOperationException(TestData.InvalidOperationErrorResponseMessage)));
            Mock<ISecurityServiceClient> securityServiceClient = new Mock<ISecurityServiceClient>();
            securityServiceClient.Setup(s => s.GetToken(It.IsAny<String>(), It.IsAny<String>(), It.IsAny<CancellationToken>())).ReturnsAsync(TestData.TokenResponse);

            ITransactionProcessorACLApplicationService applicationService =
                new TransactionProcessorACLApplicationService(transactionProcessorClient.Object, securityServiceClient.Object);

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
            Mock<ITransactionProcessorClient> transactionProcessorClient = new Mock<ITransactionProcessorClient>();
            transactionProcessorClient.Setup(t => t.PerformTransaction(It.IsAny<String>(), It.IsAny<SerialisedMessage>(), It.IsAny<CancellationToken>()))
                                      .ThrowsAsync(new Exception("Error", new HttpRequestException(TestData.HttpRequestErrorResponseMessage)));
            Mock<ISecurityServiceClient> securityServiceClient = new Mock<ISecurityServiceClient>();
            securityServiceClient.Setup(s => s.GetToken(It.IsAny<String>(), It.IsAny<String>(), It.IsAny<CancellationToken>())).ReturnsAsync(TestData.TokenResponse);

            ITransactionProcessorACLApplicationService applicationService =
                new TransactionProcessorACLApplicationService(transactionProcessorClient.Object, securityServiceClient.Object);

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
            Mock<ITransactionProcessorClient> transactionProcessorClient = new Mock<ITransactionProcessorClient>();
            transactionProcessorClient.Setup(t => t.PerformTransaction(It.IsAny<String>(), It.IsAny<SerialisedMessage>(), It.IsAny<CancellationToken>()))
                                      .ThrowsAsync(new Exception("Error", new Exception(TestData.GeneralErrorResponseMessage)));
            Mock<ISecurityServiceClient> securityServiceClient = new Mock<ISecurityServiceClient>();
            securityServiceClient.Setup(s => s.GetToken(It.IsAny<String>(), It.IsAny<String>(), It.IsAny<CancellationToken>())).ReturnsAsync(TestData.TokenResponse);

            ITransactionProcessorACLApplicationService applicationService =
                new TransactionProcessorACLApplicationService(transactionProcessorClient.Object, securityServiceClient.Object);

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
    }
}
