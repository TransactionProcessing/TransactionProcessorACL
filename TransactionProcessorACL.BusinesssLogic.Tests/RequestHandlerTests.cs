namespace TransactionProcessorACL.BusinesssLogic.Tests
{
    using System.Threading;
    using System.Threading.Tasks;
    using BusinessLogic.RequestHandlers;
    using BusinessLogic.Requests;
    using Models;
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
            ProcessLogonTransactionRequestHandler requestHandler = new ProcessLogonTransactionRequestHandler();

            ProcessLogonTransactionRequest request = TestData.ProcessLogonTransactionRequest;
            ProcessLogonTransactionResponse response = await requestHandler.Handle(request, CancellationToken.None);

            response.ShouldNotBeNull();
            response.ResponseCode.ShouldBe("0000");
            response.ResponseMessage.ShouldBe("SUCCESS");
        }

        #endregion
    }
}