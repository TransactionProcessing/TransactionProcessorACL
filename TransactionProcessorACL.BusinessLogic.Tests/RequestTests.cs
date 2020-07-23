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
                                                                                           TestData.DeviceIdentifier,
                                                                                           TestData.RequireConfigurationInResponseTrue);

            request.EstateId.ShouldBe(TestData.EstateId);
            request.MerchantId.ShouldBe(TestData.MerchantId);
            request.TransactionDateTime.ShouldBe(TestData.TransactionDateTime);
            request.TransactionNumber.ShouldBe(TestData.TransactionNumber);
            request.DeviceIdentifier.ShouldBe(TestData.DeviceIdentifier);
            request.RequireConfigurationInResponse.ShouldBe(TestData.RequireConfigurationInResponseTrue);
        }

        [Fact]
        public void ProcessSaleTransactionRequest_CanBeCreated_IsCreated()
        {
            ProcessSaleTransactionRequest request = ProcessSaleTransactionRequest.Create(TestData.EstateId,
                                                                                         TestData.MerchantId,
                                                                                         TestData.TransactionDateTime,
                                                                                         TestData.TransactionNumber,
                                                                                         TestData.DeviceIdentifier,
                                                                                         TestData.OperatorIdentifier,
                                                                                         TestData.SaleAmount,
                                                                                         TestData.CustomerAccountNumber,
                                                                                         TestData.CustomerEmailAddress,
                                                                                         TestData.ContractId,
                                                                                         TestData.ProductId);

            request.EstateId.ShouldBe(TestData.EstateId);
            request.MerchantId.ShouldBe(TestData.MerchantId);
            request.TransactionDateTime.ShouldBe(TestData.TransactionDateTime);
            request.TransactionNumber.ShouldBe(TestData.TransactionNumber);
            request.DeviceIdentifier.ShouldBe(TestData.DeviceIdentifier);
            request.OperatorIdentifier.ShouldBe(TestData.OperatorIdentifier);
            request.Amount.ShouldBe(TestData.SaleAmount);
            request.CustomerAccountNumber.ShouldBe(TestData.CustomerAccountNumber);
            request.CustomerEmailAddress.ShouldBe(TestData.CustomerEmailAddress);
            request.ContractId.ShouldBe(TestData.ContractId);
            request.ProductId.ShouldBe(TestData.ProductId);
        }

        #endregion
    }
}