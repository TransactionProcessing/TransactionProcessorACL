using System;
using System.Collections.Generic;
using System.Text;

namespace TransactionProcessorACL.BusinesssLogic.Tests
{
    using System.Threading;
    using System.Threading.Tasks;
    using BusinessLogic.Services;
    using Microsoft.Extensions.Configuration;
    using Models;
    using Moq;
    using SecurityService.Client;
    using Shared.General;
    using Shouldly;
    using Testing;
    using TransactionProcessor.Client;
    using TransactionProcessor.DataTransferObjects;
    using Xunit;

    public class TransactionProcessorACLApplicationServiceTests
    {
        private IConfigurationRoot SetupMemoryConfiguration()
        {
            Dictionary<String, String> configuration = new Dictionary<String, String>();

            IConfigurationBuilder builder = new ConfigurationBuilder();

            configuration.Add("AppSettings:SecurityService", "http://192.168.1.133:5001");
            configuration.Add("AppSettings:TransactionProcessorApi", "http://192.168.1.133:5002");
            configuration.Add("AppSettings:ClientId", "ClientId");
            configuration.Add("AppSettings:ClientSecret", "secret");

            builder.AddInMemoryCollection(configuration);

            return builder.Build();
        }

        [Fact]
        public async Task TransactionProcessorACLApplicationService_ProcessLogonTransaction_TransactionIsSuccessful()
        {
            IConfigurationRoot configuration = this.SetupMemoryConfiguration();
            ConfigurationReader.Initialise(configuration);

            Mock<ITransactionProcessorClient> transactionProcessorClient = new Mock<ITransactionProcessorClient>();
            transactionProcessorClient.Setup(t => t.PerformTransaction(It.IsAny<String>(), It.IsAny<SerialisedMessage>(), It.IsAny<CancellationToken>()))
                                      .ReturnsAsync(TestData.SerialisedMessageResponse);
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
        public async Task TransactionProcessorACLApplicationService_ProcessLogonTransaction_ErrorInLogon_TransactionIsNotSuccessful()
        {
            IConfigurationRoot configuration = this.SetupMemoryConfiguration();
            ConfigurationReader.Initialise(configuration);

            Mock<ITransactionProcessorClient> transactionProcessorClient = new Mock<ITransactionProcessorClient>();
            transactionProcessorClient.Setup(t => t.PerformTransaction(It.IsAny<String>(), It.IsAny<SerialisedMessage>(), It.IsAny<CancellationToken>()))
                                      .ThrowsAsync(new Exception("Error", new InvalidOperationException(TestData.ErrorResponseMessage)));
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
            logonResponse.ResponseMessage.ShouldBe(TestData.ErrorResponseMessage);
            logonResponse.ResponseCode.ShouldBe(TestData.ErrorResponseCode);
        }

        [Fact]
        public async Task TransactionProcessorACLApplicationService_ProcessSaleTransaction_TransactionIsSuccessful()
        {
            IConfigurationRoot configuration = this.SetupMemoryConfiguration();
            ConfigurationReader.Initialise(configuration);

            Mock<ITransactionProcessorClient> transactionProcessorClient = new Mock<ITransactionProcessorClient>();
            transactionProcessorClient.Setup(t => t.PerformTransaction(It.IsAny<String>(), It.IsAny<SerialisedMessage>(), It.IsAny<CancellationToken>()))
                                      .ReturnsAsync(TestData.SerialisedMessageResponse);
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
                                                                                                             TestData.SaleAmount,
                                                                                                             TestData.CustomerAccountNumber,
                                                                                                             TestData.CustomerEmailAddress,
                                                                                                             TestData.ContractId,
                                                                                                             TestData.ProductId,
                                                                                                             CancellationToken.None);

            saleResponse.ShouldNotBeNull();
            saleResponse.ResponseMessage.ShouldBe(TestData.ResponseMessage);
            saleResponse.ResponseCode.ShouldBe(TestData.ResponseCode);
        }

        [Fact]
        public async Task TransactionProcessorACLApplicationService_ProcessSaleTransaction_ErrorInSale_TransactionIsNotSuccessful()
        {
            IConfigurationRoot configuration = this.SetupMemoryConfiguration();
            ConfigurationReader.Initialise(configuration);

            Mock<ITransactionProcessorClient> transactionProcessorClient = new Mock<ITransactionProcessorClient>();
            transactionProcessorClient.Setup(t => t.PerformTransaction(It.IsAny<String>(), It.IsAny<SerialisedMessage>(), It.IsAny<CancellationToken>()))
                                      .ThrowsAsync(new Exception("Error", new InvalidOperationException(TestData.ErrorResponseMessage)));
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
                                                                                                          TestData.SaleAmount,
                                                                                                          TestData.CustomerAccountNumber,
                                                                                                          TestData.CustomerEmailAddress,
                                                                                                          TestData.ContractId,
                                                                                                          TestData.ProductId,
                                                                                                          CancellationToken.None);

            saleResponse.ShouldNotBeNull();
            saleResponse.ResponseMessage.ShouldBe(TestData.ErrorResponseMessage);
            saleResponse.ResponseCode.ShouldBe(TestData.ErrorResponseCode);
        }
    }
}
