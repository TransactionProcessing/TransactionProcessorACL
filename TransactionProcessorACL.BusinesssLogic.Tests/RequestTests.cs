namespace TransactionProcessorACL.BusinesssLogic.Tests
{
    using BusinessLogic.Requests;
    using Shouldly;
    using Testing;
    using Xunit;

    /// <summary>
    /// 
    /// </summary>
    public class RequestTests
    {
        #region Methods

        /// <summary>
        /// Processes the logon transaction request can be created is created.
        /// </summary>
        [Fact]
        public void ProcessLogonTransactionRequest_CanBeCreated_IsCreated()
        {
            ProcessLogonTransactionRequest request = ProcessLogonTransactionRequest.Create(TestData.EstateId,
                                                                                           TestData.MerchantId,
                                                                                           TestData.TransactionDateTime,
                                                                                           TestData.TransactionNumber,
                                                                                           TestData.IMEINumber,
                                                                                           TestData.RequireConfigurationInResponseTrue);

            request.EstateId.ShouldBe(TestData.EstateId);
            request.MerchantId.ShouldBe(TestData.MerchantId);
            request.TransactionDateTime.ShouldBe(TestData.TransactionDateTime);
            request.TransactionNumber.ShouldBe(TestData.TransactionNumber);
            request.ImeiNumber.ShouldBe(TestData.IMEINumber);
            request.RequireConfigurationInResponse.ShouldBe(TestData.RequireConfigurationInResponseTrue);
        }

        #endregion
    }
}