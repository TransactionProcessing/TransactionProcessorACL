namespace TransactionProcessorACL.BusinesssLogic.Tests
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using BusinessLogic.RequestHandlers;
    using BusinessLogic.Requests;
    using BusinessLogic.Services;
    using Models;
    using Moq;
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
                                                      It.IsAny<Decimal>(),
                                                      It.IsAny<String>(),
                                                      It.IsAny<CancellationToken>())).ReturnsAsync(TestData.ProcessSaleTransactionResponse);

            ProcessSaleTransactionRequestHandler requestHandler = new ProcessSaleTransactionRequestHandler(applicationService.Object);

            ProcessSaleTransactionRequest request = TestData.ProcessSaleTransactionRequest;
            ProcessSaleTransactionResponse response = await requestHandler.Handle(request, CancellationToken.None);

            response.ShouldNotBeNull();
            response.ResponseCode.ShouldBe(TestData.ResponseCode);
            response.ResponseMessage.ShouldBe(TestData.ResponseMessage);
        }

        #endregion
    }
}