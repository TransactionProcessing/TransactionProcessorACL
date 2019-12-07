namespace TransactionProcessorACL.Tests.ControllerTests
{
    using System;
    using System.Net;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using Common;
    using DataTransferObjects;
    using Shouldly;
    using Testing;
    using Xunit;

    [Collection("TestCollection")]
    public class TransactionControllerTests : IClassFixture<TransactionProcessorACLWebFactory<Startup>>
    {
        #region Fields

        /// <summary>
        /// The web application factory
        /// </summary>
        private readonly TransactionProcessorACLWebFactory<Startup> WebApplicationFactory;

        #endregion

        #region Constructors

        public TransactionControllerTests(TransactionProcessorACLWebFactory<Startup> webApplicationFactory)
        {
            this.WebApplicationFactory = webApplicationFactory;
        }

        #endregion

        #region Methods

        [Fact]
        public async Task TransactionController_POST_LogonTransaction_LogonTransactionResponseIsReturned()
        {
            HttpClient client = this.WebApplicationFactory.CreateClient();

            LogonTransactionRequestMessage logonTransactionRequestMessage = TestData.LogonTransactionRequestMessage;
            String uri = "api/transactions";
            StringContent content = Helpers.CreateStringContent(logonTransactionRequestMessage);
            client.DefaultRequestHeaders.Add("api-version", "1.0");

            HttpResponseMessage response = await client.PostAsync(uri, content, CancellationToken.None);

            response.StatusCode.ShouldBe(HttpStatusCode.OK);

            // TODO: Response message
        }

        #endregion
    }
}