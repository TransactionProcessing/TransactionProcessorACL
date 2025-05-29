using System;
using System.Collections.Generic;
using System.Text;
using SimpleResults;

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

    public class TransactionProcessorACLApplicationServiceTests
    {
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

        private Mock<ITransactionProcessorClient> transactionProcessorClient;

        private Mock<ISecurityServiceClient> securityServiceClient;

        private ITransactionProcessorACLApplicationService applicationService;
        [Fact]
        public async Task TransactionProcessorACLApplicationService_ProcessLogonTransaction_TransactionIsSuccessful()
        {
            transactionProcessorClient.Setup(t => t.PerformTransaction(It.IsAny<String>(), It.IsAny<SerialisedMessage>(), It.IsAny<CancellationToken>()))
                                      .ReturnsAsync(TestData.SerialisedMessageResponseLogon);
            securityServiceClient.Setup(s => s.GetToken(It.IsAny<String>(), It.IsAny<String>(), It.IsAny<CancellationToken>())).ReturnsAsync( Result.Success(TestData.TokenResponse));

            Result<ProcessLogonTransactionResponse> result = await applicationService.ProcessLogonTransaction(TestData.EstateId,
                                                                                                             TestData.MerchantId,
                                                                                                             TestData.TransactionDateTime,
                                                                                                             TestData.TransactionNumber,
                                                                                                             TestData.DeviceIdentifier,
                                                                                                             CancellationToken.None);
            result.IsSuccess.ShouldBeTrue();
            ProcessLogonTransactionResponse logonResponse = result.Data;
            logonResponse.ShouldNotBeNull();
            logonResponse.ResponseMessage.ShouldBe(TestData.ResponseMessage);
            logonResponse.ResponseCode.ShouldBe(TestData.ResponseCode);
        }

        [Fact]
        public async Task TransactionProcessorACLApplicationService_ProcessLogonTransaction_GetTokenFailed_ResultFailed()
        {
            securityServiceClient.Setup(s => s.GetToken(It.IsAny<String>(), It.IsAny<String>(), It.IsAny<CancellationToken>())).ReturnsAsync(Result.Failure());

            Result<ProcessLogonTransactionResponse> result = await applicationService.ProcessLogonTransaction(TestData.EstateId,
                TestData.MerchantId,
                TestData.TransactionDateTime,
                TestData.TransactionNumber,
                TestData.DeviceIdentifier,
                CancellationToken.None);

            result.IsFailed.ShouldBeTrue();
        }

        [Fact]
        public async Task TransactionProcessorACLApplicationService_ProcessLogonTransaction_PerformTransactionFailed_ResultFailed()
        {
            securityServiceClient.Setup(s => s.GetToken(It.IsAny<String>(), It.IsAny<String>(), It.IsAny<CancellationToken>())).ReturnsAsync(Result.Success(TestData.TokenResponse));
            transactionProcessorClient.Setup(t => t.PerformTransaction(It.IsAny<String>(), It.IsAny<SerialisedMessage>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result.Failure());
            Result<ProcessLogonTransactionResponse> result = await applicationService.ProcessLogonTransaction(TestData.EstateId,
                TestData.MerchantId,
                TestData.TransactionDateTime,
                TestData.TransactionNumber,
                TestData.DeviceIdentifier,
                CancellationToken.None);

            result.IsFailed.ShouldBeTrue();
        }

        [Fact]
        public async Task TransactionProcessorACLApplicationService_ProcessLogonTransaction_ExceptionErrorInLogon_TransactionIsNotSuccessful()
        {
            transactionProcessorClient.Setup(t => t.PerformTransaction(It.IsAny<String>(), It.IsAny<SerialisedMessage>(), It.IsAny<CancellationToken>()))
                                      .ThrowsAsync(new Exception("Error"));
            securityServiceClient.Setup(s => s.GetToken(It.IsAny<String>(), It.IsAny<String>(), It.IsAny<CancellationToken>())).ReturnsAsync(Result.Success(TestData.TokenResponse));

            Result<ProcessLogonTransactionResponse> result = await applicationService.ProcessLogonTransaction(TestData.EstateId,
                                                                                                             TestData.MerchantId,
                                                                                                             TestData.TransactionDateTime,
                                                                                                             TestData.TransactionNumber,
                                                                                                             TestData.DeviceIdentifier,
                                                                                                             CancellationToken.None);
            result.IsSuccess.ShouldBeTrue();
            ProcessLogonTransactionResponse logonResponse =result.Data;
            logonResponse.ShouldNotBeNull();
            logonResponse.ResponseMessage.ShouldContain(TestData.LogonExceptionResponseMessage);
            logonResponse.ResponseCode.ShouldBe(TestData.ExceptionErrorResponseCode);
        }
        
        [Fact]
        public async Task TransactionProcessorACLApplicationService_ProcessSaleTransaction_TransactionIsSuccessful()
        {
            transactionProcessorClient.Setup(t => t.PerformTransaction(It.IsAny<String>(), It.IsAny<SerialisedMessage>(), It.IsAny<CancellationToken>()))
                                      .ReturnsAsync(TestData.SerialisedMessageResponseSale);
            securityServiceClient.Setup(s => s.GetToken(It.IsAny<String>(), It.IsAny<String>(), It.IsAny<CancellationToken>())).ReturnsAsync(Result.Success(TestData.TokenResponse));

            Result<ProcessSaleTransactionResponse> result = await applicationService.ProcessSaleTransaction(TestData.EstateId,
                                                                                                             TestData.MerchantId,
                                                                                                             TestData.TransactionDateTime,
                                                                                                             TestData.TransactionNumber,
                                                                                                             TestData.DeviceIdentifier,
                                                                                                             TestData.OperatorId,
                                                                                                             TestData.CustomerEmailAddress,
                                                                                                             TestData.ContractId,
                                                                                                             TestData.ProductId,
                                                                                                             TestData.AdditionalRequestMetadata,
                                                                                                             CancellationToken.None);

            result.IsSuccess.ShouldBeTrue();
            ProcessSaleTransactionResponse saleResponse = result.Data;
            saleResponse.ShouldNotBeNull();
            saleResponse.ResponseMessage.ShouldBe(TestData.ResponseMessage);
            saleResponse.ResponseCode.ShouldBe(TestData.ResponseCode);
        }

        [Fact]
        public async Task TransactionProcessorACLApplicationService_ProcessSaleTransaction_GetTokenFailed_ResultFailed()
        {
            securityServiceClient.Setup(s => s.GetToken(It.IsAny<String>(), It.IsAny<String>(), It.IsAny<CancellationToken>())).ReturnsAsync(Result.Failure());

            Result<ProcessSaleTransactionResponse> result = await applicationService.ProcessSaleTransaction(TestData.EstateId,
                                                                                                             TestData.MerchantId,
                                                                                                             TestData.TransactionDateTime,
                                                                                                             TestData.TransactionNumber,
                                                                                                             TestData.DeviceIdentifier,
                                                                                                             TestData.OperatorId,
                                                                                                             TestData.CustomerEmailAddress,
                                                                                                             TestData.ContractId,
                                                                                                             TestData.ProductId,
                                                                                                             TestData.AdditionalRequestMetadata,
                                                                                                             CancellationToken.None);

            result.IsFailed.ShouldBeTrue();
        }

        [Fact]
        public async Task TransactionProcessorACLApplicationService_ProcessSaleTransaction_PerformTransactionFailed_ResultFailed()
        {
            transactionProcessorClient.Setup(t => t.PerformTransaction(It.IsAny<String>(), It.IsAny<SerialisedMessage>(), It.IsAny<CancellationToken>()))
                                      .ReturnsAsync(Result.Failure());
            securityServiceClient.Setup(s => s.GetToken(It.IsAny<String>(), It.IsAny<String>(), It.IsAny<CancellationToken>())).ReturnsAsync(Result.Success(TestData.TokenResponse));

            Result<ProcessSaleTransactionResponse> result = await applicationService.ProcessSaleTransaction(TestData.EstateId,
                TestData.MerchantId,
                TestData.TransactionDateTime,
                TestData.TransactionNumber,
                TestData.DeviceIdentifier,
                TestData.OperatorId,
                TestData.CustomerEmailAddress,
                TestData.ContractId,
                TestData.ProductId,
                TestData.AdditionalRequestMetadata,
                CancellationToken.None);

            result.IsFailed.ShouldBeTrue();
        }

        [Fact]
        public async Task TransactionProcessorACLApplicationService_ProcessSaleTransaction_ExceptionErrorInSale_TransactionIsNotSuccessful()
        {
            transactionProcessorClient.Setup(t => t.PerformTransaction(It.IsAny<String>(), It.IsAny<SerialisedMessage>(), It.IsAny<CancellationToken>()))
                                      .ThrowsAsync(new Exception("Error"));
            securityServiceClient.Setup(s => s.GetToken(It.IsAny<String>(), It.IsAny<String>(), It.IsAny<CancellationToken>())).ReturnsAsync(Result.Success(TestData.TokenResponse));

            Result<ProcessSaleTransactionResponse> result = await applicationService.ProcessSaleTransaction(TestData.EstateId,
                                                                                                          TestData.MerchantId,
                                                                                                          TestData.TransactionDateTime,
                                                                                                          TestData.TransactionNumber,
                                                                                                          TestData.DeviceIdentifier,
                                                                                                          TestData.OperatorId,
                                                                                                          TestData.CustomerEmailAddress,
                                                                                                          TestData.ContractId,
                                                                                                          TestData.ProductId,
                                                                                                          TestData.AdditionalRequestMetadata,
                                                                                                          CancellationToken.None);
            result.IsSuccess.ShouldBeTrue();
            ProcessSaleTransactionResponse saleResponse = result.Data;
            saleResponse.ShouldNotBeNull();
            saleResponse.ResponseMessage.ShouldBe(TestData.SaleExceptionResponseMessage);
            saleResponse.ResponseCode.ShouldBe(TestData.ExceptionErrorResponseCode);
        }

        [Fact]
        public async Task TransactionProcessorACLApplicationService_ProcessReconciliation_TransactionIsSuccessful()
        {
            transactionProcessorClient.Setup(t => t.PerformTransaction(It.IsAny<String>(), It.IsAny<SerialisedMessage>(), It.IsAny<CancellationToken>()))
                                      .ReturnsAsync(TestData.SerialisedMessageResponseReconciliation);
            securityServiceClient.Setup(s => s.GetToken(It.IsAny<String>(), It.IsAny<String>(), It.IsAny<CancellationToken>())).ReturnsAsync(Result.Success(TestData.TokenResponse));

            Result<ProcessReconciliationResponse> result = await applicationService.ProcessReconciliation(TestData.EstateId,
                TestData.MerchantId,
                TestData.TransactionDateTime,
                TestData.DeviceIdentifier,
                TestData.ReconciliationTransactionCount,
                TestData.ReconciliationTransactionValue,
                CancellationToken.None);

            result.IsSuccess.ShouldBeTrue();
            ProcessReconciliationResponse reconciliationResponse = result.Data;
            reconciliationResponse.ShouldNotBeNull();
            reconciliationResponse.ResponseMessage.ShouldBe(TestData.ResponseMessage);
            reconciliationResponse.ResponseCode.ShouldBe(TestData.ResponseCode);
        }

        [Fact]
        public async Task TransactionProcessorACLApplicationService_ProcessReconciliation_GetTokenFailed_ResultFailed()
        {
            securityServiceClient.Setup(s => s.GetToken(It.IsAny<String>(), It.IsAny<String>(), It.IsAny<CancellationToken>())).ReturnsAsync(Result.Failure());

            Result<ProcessReconciliationResponse> result = await applicationService.ProcessReconciliation(TestData.EstateId,
                TestData.MerchantId,
                TestData.TransactionDateTime,
                TestData.DeviceIdentifier,
                TestData.ReconciliationTransactionCount,
                TestData.ReconciliationTransactionValue,
                CancellationToken.None);

            result.IsFailed.ShouldBeTrue();
        }

        [Fact]
        public async Task TransactionProcessorACLApplicationService_ProcessReconciliation_PerformTransactionFailed_ResultFailed()
        {
            securityServiceClient.Setup(s => s.GetToken(It.IsAny<String>(), It.IsAny<String>(), It.IsAny<CancellationToken>())).ReturnsAsync(Result.Success(TestData.TokenResponse));
            transactionProcessorClient.Setup(t => t.PerformTransaction(It.IsAny<String>(), It.IsAny<SerialisedMessage>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result.Failure());

            Result<ProcessReconciliationResponse> result = await applicationService.ProcessReconciliation(TestData.EstateId,
                TestData.MerchantId,
                TestData.TransactionDateTime,
                TestData.DeviceIdentifier,
                TestData.ReconciliationTransactionCount,
                TestData.ReconciliationTransactionValue,
                CancellationToken.None);

            result.IsFailed.ShouldBeTrue();
        }

        [Fact]
        public async Task TransactionProcessorACLApplicationService_ProcessReconciliation_ExceptionErrorInReconciliation_TransactionIsNotSuccessful()
        {
            transactionProcessorClient.Setup(t => t.PerformTransaction(It.IsAny<String>(), It.IsAny<SerialisedMessage>(), It.IsAny<CancellationToken>()))
                                      .ThrowsAsync(new Exception("Error"));
            securityServiceClient.Setup(s => s.GetToken(It.IsAny<String>(), It.IsAny<String>(), It.IsAny<CancellationToken>())).ReturnsAsync(Result.Success(TestData.TokenResponse));

            Result<ProcessReconciliationResponse> result= await applicationService.ProcessReconciliation(TestData.EstateId,
                TestData.MerchantId,
                TestData.TransactionDateTime,
                TestData.DeviceIdentifier,
                TestData.ReconciliationTransactionCount,
                TestData.ReconciliationTransactionValue,
                CancellationToken.None);

            result.IsSuccess.ShouldBeTrue();
            ProcessReconciliationResponse reconciliationResponse = result.Data;
            reconciliationResponse.ShouldNotBeNull();
            reconciliationResponse.ResponseMessage.ShouldBe(TestData.ReconciliationExceptionResponseMessage);
            reconciliationResponse.ResponseCode.ShouldBe(TestData.ExceptionErrorResponseCode);
        }

        [Fact]
        public async Task VoucherManagementACLApplicationService_GetVoucher_VoucherRetrieved()
        {
            this.transactionProcessorClient.Setup(v => v.GetVoucherByCode(It.IsAny<String>(), It.IsAny<Guid>(), It.IsAny<String>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(TestData.GetVoucherResponse);
            securityServiceClient.Setup(s => s.GetToken(It.IsAny<String>(), It.IsAny<String>(), It.IsAny<CancellationToken>())).ReturnsAsync(Result.Success(TestData.TokenResponse));

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
        public async Task VoucherManagementACLApplicationService_GetVoucher_GetTokenFailed_ResultIsFailed()
        {
            securityServiceClient.Setup(s => s.GetToken(It.IsAny<String>(), It.IsAny<String>(), It.IsAny<CancellationToken>())).ReturnsAsync(Result.Failure());

            GetVoucherResponse voucherResponse = await applicationService.GetVoucher(TestData.EstateId, TestData.ContractId, TestData.VoucherCode, CancellationToken.None);

            voucherResponse.ShouldNotBeNull();
            voucherResponse.ResponseCode.ShouldBe("0004");
        }

        [Fact]
        public async Task VoucherManagementACLApplicationService_GetVoucher_GetVoucherFailed_ResultIsFailed()
        {
            this.transactionProcessorClient.Setup(v => v.GetVoucherByCode(It.IsAny<String>(), It.IsAny<Guid>(), It.IsAny<String>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result.Failure());
            securityServiceClient.Setup(s => s.GetToken(It.IsAny<String>(), It.IsAny<String>(), It.IsAny<CancellationToken>())).ReturnsAsync(Result.Success(TestData.TokenResponse));

            GetVoucherResponse voucherResponse = await applicationService.GetVoucher(TestData.EstateId, TestData.ContractId, TestData.VoucherCode, CancellationToken.None);

            voucherResponse.ShouldNotBeNull();
            voucherResponse.ResponseCode.ShouldBe("0005");
        }

        [Fact]
        public async Task VoucherManagementACLApplicationService_GetVoucher_ExceptionErrorInGetVoucher_GetVoucherIsNotSuccessful()
        {
            transactionProcessorClient.Setup(v => v.GetVoucherByCode(It.IsAny<String>(), It.IsAny<Guid>(), It.IsAny<String>(), It.IsAny<CancellationToken>()))
                                      .ThrowsAsync(new Exception("Error"));
            securityServiceClient.Setup(s => s.GetToken(It.IsAny<String>(), It.IsAny<String>(), It.IsAny<CancellationToken>())).ReturnsAsync(Result.Success(TestData.TokenResponse));

            GetVoucherResponse voucherResponse = await applicationService.GetVoucher(TestData.EstateId, TestData.ContractId, TestData.VoucherCode, CancellationToken.None);

            voucherResponse.ShouldNotBeNull();
            voucherResponse.ResponseMessage.ShouldBe(TestData.VoucherExceptionResponseMessage);
            voucherResponse.ResponseCode.ShouldBe(TestData.ExceptionErrorResponseCode);
        }

        [Fact]
        public async Task VoucherManagementACLApplicationService_RedeemVoucher_VoucherRedeemed()
        {
            transactionProcessorClient.Setup(v => v.RedeemVoucher(It.IsAny<String>(), It.IsAny<RedeemVoucherRequest>(), It.IsAny<CancellationToken>()))
                                      .ReturnsAsync(TestData.RedeemVoucherResponse);
            securityServiceClient.Setup(s => s.GetToken(It.IsAny<String>(), It.IsAny<String>(), It.IsAny<CancellationToken>())).ReturnsAsync(Result.Success(TestData.TokenResponse));

            RedeemVoucherResponse voucherResponse = await applicationService.RedeemVoucher(TestData.EstateId, TestData.ContractId, TestData.VoucherCode, CancellationToken.None);

            voucherResponse.ShouldNotBeNull();
            voucherResponse.VoucherCode.ShouldBe(TestData.RedeemVoucherResponse.VoucherCode);
            voucherResponse.ContractId.ShouldBe(TestData.ContractId);
            voucherResponse.EstateId.ShouldBe(TestData.EstateId);
            voucherResponse.ExpiryDate.ShouldBe(TestData.RedeemVoucherResponse.ExpiryDate);
            voucherResponse.Balance.ShouldBe(TestData.RedeemVoucherResponse.RemainingBalance);
        }

        [Fact]
        public async Task VoucherManagementACLApplicationService_RedeemVoucher_GetTokenFailed_ResultIsFailed()
        {
            transactionProcessorClient.Setup(v => v.RedeemVoucher(It.IsAny<String>(), It.IsAny<RedeemVoucherRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(TestData.RedeemVoucherResponse);
            securityServiceClient.Setup(s => s.GetToken(It.IsAny<String>(), It.IsAny<String>(), It.IsAny<CancellationToken>())).ReturnsAsync(Result.Failure());

            RedeemVoucherResponse voucherResponse = await applicationService.RedeemVoucher(TestData.EstateId, TestData.ContractId, TestData.VoucherCode, CancellationToken.None);

            voucherResponse.ShouldNotBeNull();
            voucherResponse.ResponseCode.ShouldBe("0004");
        }

        [Fact]
        public async Task VoucherManagementACLApplicationService_RedeemVoucher_RedeemVoucherFailed_ResultIsFailed()
        {
            transactionProcessorClient.Setup(v => v.RedeemVoucher(It.IsAny<String>(), It.IsAny<RedeemVoucherRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result.Failure());
            securityServiceClient.Setup(s => s.GetToken(It.IsAny<String>(), It.IsAny<String>(), It.IsAny<CancellationToken>())).ReturnsAsync(Result.Success(TestData.TokenResponse));

            RedeemVoucherResponse voucherResponse = await applicationService.RedeemVoucher(TestData.EstateId, TestData.ContractId, TestData.VoucherCode, CancellationToken.None);

            voucherResponse.ShouldNotBeNull();
            voucherResponse.ResponseCode.ShouldBe("0005");
        }

        [Fact]
        public async Task VoucherManagementACLApplicationService_RedeemVoucher_ExceptionErrorInGetVoucher_GetVoucherIsNotSuccessful()
        {
             transactionProcessorClient.Setup(v => v.RedeemVoucher(It.IsAny<String>(), It.IsAny<RedeemVoucherRequest>(), It.IsAny<CancellationToken>()))
                                      .ThrowsAsync(new Exception("Error"));
            
            securityServiceClient.Setup(s => s.GetToken(It.IsAny<String>(), It.IsAny<String>(), It.IsAny<CancellationToken>())).ReturnsAsync(Result.Success(TestData.TokenResponse));

            RedeemVoucherResponse voucherResponse = await applicationService.RedeemVoucher(TestData.EstateId, TestData.ContractId, TestData.VoucherCode, CancellationToken.None);

            voucherResponse.ShouldNotBeNull();
            voucherResponse.ResponseMessage.ShouldBe(TestData.RedeemVoucherExceptionResponseMessage);
            voucherResponse.ResponseCode.ShouldBe(TestData.ExceptionErrorResponseCode);
        }

        [Theory]
        [InlineData(TransactionProcessor.DataTransferObjects.Responses.Merchant.SettlementSchedule.Immediate)]
        [InlineData(TransactionProcessor.DataTransferObjects.Responses.Merchant.SettlementSchedule.Weekly)]
        [InlineData(TransactionProcessor.DataTransferObjects.Responses.Merchant.SettlementSchedule.Monthly)]
        public async Task VoucherManagementACLApplicationService_GetMerchant_MerchantReturned(TransactionProcessor.DataTransferObjects.Responses.Merchant.SettlementSchedule settlementSchedule)
        {
            transactionProcessorClient.Setup(v => v.GetMerchant(It.IsAny<String>(), It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                                      .ReturnsAsync(Result.Success(TestData.MerchantResponse(settlementSchedule)));
            securityServiceClient.Setup(s => s.GetToken(It.IsAny<String>(), It.IsAny<String>(), It.IsAny<CancellationToken>())).ReturnsAsync(Result.Success(TestData.TokenResponse));

            Result<MerchantResponse> merchantResponse = await applicationService.GetMerchant(TestData.EstateId, TestData.MerchantId, CancellationToken.None);
            merchantResponse.IsSuccess.ShouldBeTrue();
            merchantResponse.Data.ShouldNotBeNull();
            merchantResponse.Data.MerchantId.ShouldBe(TestData.MerchantId);
        }

        [Fact]
        public async Task VoucherManagementACLApplicationService_GetMerchant_GetTokenFailed_ResultIsFailed()
        {
            securityServiceClient.Setup(s => s.GetToken(It.IsAny<String>(), It.IsAny<String>(), It.IsAny<CancellationToken>())).ReturnsAsync(Result.Failure());

            Result<MerchantResponse> merchantResponse = await applicationService.GetMerchant(TestData.EstateId, TestData.MerchantId, CancellationToken.None);
            merchantResponse.IsFailed.ShouldBeTrue();
            
        }

        [Fact]
        public async Task VoucherManagementACLApplicationService_GetMerchant_GetMerchantFailed_ResultIsFailed()
        {
            transactionProcessorClient.Setup(v => v.GetMerchant(It.IsAny<String>(), It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result.Failure());
            securityServiceClient.Setup(s => s.GetToken(It.IsAny<String>(), It.IsAny<String>(), It.IsAny<CancellationToken>())).ReturnsAsync(Result.Success(TestData.TokenResponse));

            Result<MerchantResponse> merchantResponse = await applicationService.GetMerchant(TestData.EstateId, TestData.MerchantId, CancellationToken.None);
            merchantResponse.IsFailed.ShouldBeTrue();
        }

        [Theory]
        [InlineData(TransactionProcessor.DataTransferObjects.Responses.Contract.ProductType.Voucher)]
        [InlineData(TransactionProcessor.DataTransferObjects.Responses.Contract.ProductType.BillPayment)]
        [InlineData(TransactionProcessor.DataTransferObjects.Responses.Contract.ProductType.MobileTopup)]
        public async Task VoucherManagementACLApplicationService_GetMerchantContracts_MerchantContractsReturned(TransactionProcessor.DataTransferObjects.Responses.Contract.ProductType productType) {
            transactionProcessorClient.Setup(v => v.GetMerchantContracts(It.IsAny<String>(), It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result.Success(TestData.MerchantContractResponses(productType)));
            securityServiceClient.Setup(s => s.GetToken(It.IsAny<String>(), It.IsAny<String>(), It.IsAny<CancellationToken>())).ReturnsAsync(Result.Success(TestData.TokenResponse));

            var merchantContractsResponse = await applicationService.GetMerchantContracts(TestData.EstateId, TestData.MerchantId, CancellationToken.None);
            merchantContractsResponse.IsSuccess.ShouldBeTrue();
        }

        [Fact]
        public async Task VoucherManagementACLApplicationService_GetMerchantContracts_GetTokenFailed_ResultIsFailed()
        {
            securityServiceClient.Setup(s => s.GetToken(It.IsAny<String>(), It.IsAny<String>(), It.IsAny<CancellationToken>())).ReturnsAsync(Result.Failure());

            var merchantContractsResponse = await applicationService.GetMerchantContracts(TestData.EstateId, TestData.MerchantId, CancellationToken.None);
            merchantContractsResponse.IsFailed.ShouldBeTrue();
        }

        [Fact]
        public async Task VoucherManagementACLApplicationService_GetMerchantContracts_GetMerchantContractsFailed_ResultIsFailed()
        {
            transactionProcessorClient.Setup(v => v.GetMerchantContracts(It.IsAny<String>(), It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result.Failure());
            securityServiceClient.Setup(s => s.GetToken(It.IsAny<String>(), It.IsAny<String>(), It.IsAny<CancellationToken>())).ReturnsAsync(Result.Success(TestData.TokenResponse));

            var merchantContractsResponse = await applicationService.GetMerchantContracts(TestData.EstateId, TestData.MerchantId, CancellationToken.None);
            merchantContractsResponse.IsFailed.ShouldBeTrue();
        }
    }
}
