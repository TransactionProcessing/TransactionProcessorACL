namespace TransactionProcessorACL.BusinesssLogic.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using BusinessLogic.RequestHandlers;
    using BusinessLogic.Requests;
    using BusinessLogic.Services;
    using Microsoft.Extensions.Configuration;
    using Models;
    using Moq;
    using Shared.General;
    using Shouldly;
    using Testing;
    using Xunit;

    /// <summary>
    /// 
    /// </summary>
    public class RequestHandlerTests
    {
        #region Methods

        /// <summary>
        /// Processes the logon transaction request handler handle request is handled.
        /// </summary>
        [Fact]
        public async Task ProcessLogonTransactionRequestHandler_Handle_RequestIsHandled()
        {
            Mock<ITransactionProcessorACLApplicationService> applicationService = new Mock<ITransactionProcessorACLApplicationService>();
            applicationService
                .Setup(a => a.ProcessLogonTransaction(It.IsAny<Guid>(),
                                                      It.IsAny<Guid>(),
                                                      It.IsAny<DateTime>(),
                                                      It.IsAny<String>(),
                                                      It.IsAny<String>(),
                                                      It.IsAny<CancellationToken>())).ReturnsAsync(TestData.ProcessLogonTransactionResponse);
            ProcessLogonTransactionRequestHandler requestHandler = new ProcessLogonTransactionRequestHandler(applicationService.Object);

            ProcessLogonTransactionRequest request = TestData.ProcessLogonTransactionRequest;
            ProcessLogonTransactionResponse response = await requestHandler.Handle(request, CancellationToken.None);

            response.ShouldNotBeNull();
            response.ResponseCode.ShouldBe(TestData.ResponseCode);
            response.ResponseMessage.ShouldBe(TestData.ResponseMessage);
            response.EstateId.ShouldBe(TestData.EstateId);
            response.MerchantId.ShouldBe(TestData.MerchantId);
        }

        [Fact]
        public async Task ProcessSaleTransactionRequestHandler_Handle_RequestIsHandled()
        {
            Mock<ITransactionProcessorACLApplicationService> applicationService = new Mock<ITransactionProcessorACLApplicationService>();
            applicationService
                .Setup(a => a.ProcessSaleTransaction(It.IsAny<Guid>(),
                                                      It.IsAny<Guid>(),
                                                      It.IsAny<DateTime>(),
                                                      It.IsAny<String>(),
                                                      It.IsAny<String>(),
                                                      It.IsAny<String>(),
                                                      It.IsAny<String>(),
                                                      It.IsAny<Guid>(),
                                                      It.IsAny<Guid>(),
                                                      It.IsAny<Dictionary<String,String>>(),
                                                      It.IsAny<CancellationToken>())).ReturnsAsync(TestData.ProcessSaleTransactionResponse);

            ProcessSaleTransactionRequestHandler requestHandler = new ProcessSaleTransactionRequestHandler(applicationService.Object);

            ProcessSaleTransactionRequest request = TestData.ProcessSaleTransactionRequest;
            ProcessSaleTransactionResponse response = await requestHandler.Handle(request, CancellationToken.None);

            response.ShouldNotBeNull();
            response.ResponseCode.ShouldBe(TestData.ResponseCode);
            response.ResponseMessage.ShouldBe(TestData.ResponseMessage);
        }

        [Fact]
        public async Task ProcessReconciliationRequestHandler_Handle_RequestIsHandled()
        {
            Mock<ITransactionProcessorACLApplicationService> applicationService = new Mock<ITransactionProcessorACLApplicationService>();
            applicationService
                .Setup(a => a.ProcessReconciliation(It.IsAny<Guid>(),
                                                      It.IsAny<Guid>(),
                                                      It.IsAny<DateTime>(),
                                                      It.IsAny<String>(),
                                                      It.IsAny<Int32>(),
                                                      It.IsAny<Decimal>(),
                                                      It.IsAny<CancellationToken>())).ReturnsAsync(TestData.ProcessReconciliationResponse);
            ProcessReconciliationRequestHandler requestHandler = new ProcessReconciliationRequestHandler(applicationService.Object);

            ProcessReconciliationRequest request = TestData.ProcessReconciliationRequest;
            ProcessReconciliationResponse response = await requestHandler.Handle(request, CancellationToken.None);

            response.ShouldNotBeNull();
            response.ResponseCode.ShouldBe(TestData.ResponseCode);
            response.ResponseMessage.ShouldBe(TestData.ResponseMessage);
            response.EstateId.ShouldBe(TestData.EstateId);
            response.MerchantId.ShouldBe(TestData.MerchantId);
        }

        [Fact]
        public async Task VersionCheckRequestHandler_Handle_RequestIsHandled()
        {
            VersionCheckRequestHandler requestHandler = new VersionCheckRequestHandler();
            IConfigurationRoot configurationRoot = new ConfigurationBuilder().AddInMemoryCollection(TestData.DefaultAppSettings).Build();
            ConfigurationReader.Initialise(configurationRoot);
            VersionCheckRequest request = TestData.VersionCheckRequest;
            Should.NotThrow(async () =>
                            {
                                await requestHandler.Handle(request, CancellationToken.None);
                            });
        }

        [Fact]
        public async Task VersionCheckRequestHandler_Handle_OldVersion_ErrorThrown()
        {
            VersionCheckRequestHandler requestHandler = new VersionCheckRequestHandler();
            IConfigurationRoot configurationRoot = new ConfigurationBuilder().AddInMemoryCollection(TestData.DefaultAppSettings).Build();
            ConfigurationReader.Initialise(configurationRoot);
            VersionCheckRequest request = VersionCheckRequest.Create(TestData.OldApplicationVersion);
            Should.Throw<NotSupportedException>(async () =>
                            {
                                await requestHandler.Handle(request, CancellationToken.None);
                            });
        }

        [Fact]
        public async Task VersionCheckRequestHandler_Handle_NewerVersionBuildNumber_RequestIsHandled()
        {
            VersionCheckRequestHandler requestHandler = new VersionCheckRequestHandler();
            IConfigurationRoot configurationRoot = new ConfigurationBuilder().AddInMemoryCollection(TestData.DefaultAppSettings).Build();
            ConfigurationReader.Initialise(configurationRoot);
            VersionCheckRequest request = VersionCheckRequest.Create(TestData.NewerApplicationVersion);
            Should.NotThrow(async () =>
                            {
                                await requestHandler.Handle(request, CancellationToken.None);
                            });
        }

        #endregion
    }
}