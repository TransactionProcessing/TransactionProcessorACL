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
                                                                                           TestData.DeviceIdentifier);

            request.EstateId.ShouldBe(TestData.EstateId);
            request.MerchantId.ShouldBe(TestData.MerchantId);
            request.TransactionDateTime.ShouldBe(TestData.TransactionDateTime);
            request.TransactionNumber.ShouldBe(TestData.TransactionNumber);
            request.DeviceIdentifier.ShouldBe(TestData.DeviceIdentifier);
        }

        [Fact]
        public void ProcessSaleTransactionRequest_CanBeCreated_IsCreated()
        {
            ProcessSaleTransactionRequest request = ProcessSaleTransactionRequest.Create(TestData.EstateId,
                                                                                         TestData.MerchantId,
                                                                                         TestData.TransactionDateTime,
                                                                                         TestData.TransactionNumber,
                                                                                         TestData.DeviceIdentifier,
                                                                                         TestData.OperatorId,
                                                                                         TestData.CustomerEmailAddress,
                                                                                         TestData.ContractId,
                                                                                         TestData.ProductId,
                                                                                         TestData.AdditionalRequestMetadata);

            request.EstateId.ShouldBe(TestData.EstateId);
            request.MerchantId.ShouldBe(TestData.MerchantId);
            request.TransactionDateTime.ShouldBe(TestData.TransactionDateTime);
            request.TransactionNumber.ShouldBe(TestData.TransactionNumber);
            request.DeviceIdentifier.ShouldBe(TestData.DeviceIdentifier);
            request.OperatorId.ShouldBe(TestData.OperatorId);
            request.CustomerEmailAddress.ShouldBe(TestData.CustomerEmailAddress);
            request.ContractId.ShouldBe(TestData.ContractId);
            request.ProductId.ShouldBe(TestData.ProductId);
            request.AdditionalRequestMetadata.ShouldBe(TestData.AdditionalRequestMetadata);
        }

        [Fact]
        public void ProcessReconciliationRequest_CanBeCreated_IsCreated()
        {
            ProcessReconciliationRequest request = ProcessReconciliationRequest.Create(TestData.EstateId,
                                                                                       TestData.MerchantId,
                                                                                       TestData.TransactionDateTime,
                                                                                       TestData.DeviceIdentifier,
                                                                                       TestData.ReconciliationTransactionCount,
                                                                                       TestData.ReconciliationTransactionValue);

            request.EstateId.ShouldBe(TestData.EstateId);
            request.MerchantId.ShouldBe(TestData.MerchantId);
            request.TransactionDateTime.ShouldBe(TestData.TransactionDateTime);
            request.DeviceIdentifier.ShouldBe(TestData.DeviceIdentifier);
            request.TransactionCount.ShouldBe(TestData.ReconciliationTransactionCount);
            request.TransactionValue.ShouldBe(TestData.ReconciliationTransactionValue);
        }

        [Fact]
        public void VersionCheckRequest_CanBeCreated_IsCreated()
        {
            VersionCheckRequest request = VersionCheckRequest.Create(TestData.ApplicationVersion);

            request.VersionNumber.ShouldBe(TestData.ApplicationVersion);
        }

        /// <summary>
        /// Processes the logon transaction request can be created is created.
        /// </summary>
        [Fact]
        public void GetVoucherRequest_CanBeCreated_IsCreated()
        {
            GetVoucherRequest request = GetVoucherRequest.Create(TestData.EstateId, TestData.ContractId, TestData.VoucherCode);

            request.EstateId.ShouldBe(TestData.EstateId);
            request.ContractId.ShouldBe(TestData.ContractId);
            request.VoucherCode.ShouldBe(TestData.VoucherCode);
        }

        [Fact]
        public void RedeemVoucherRequest_CanBeCreated_IsCreated()
        {
            RedeemVoucherRequest request = RedeemVoucherRequest.Create(TestData.EstateId, TestData.ContractId, TestData.VoucherCode);

            request.EstateId.ShouldBe(TestData.EstateId);
            request.ContractId.ShouldBe(TestData.ContractId);
            request.VoucherCode.ShouldBe(TestData.VoucherCode);
        }

        #endregion
    }
}