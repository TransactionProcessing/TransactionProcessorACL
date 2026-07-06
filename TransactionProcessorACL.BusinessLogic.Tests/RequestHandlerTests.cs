using SimpleResults;

namespace TransactionProcessorACL.BusinesssLogic.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using BusinessLogic.Common;
    using BusinessLogic.RequestHandlers;
    using BusinessLogic.Requests;
    using BusinessLogic.Services;
    using Microsoft.Extensions.Configuration;
    using Models;
    using Moq;
    using Shared.General;
    using Shouldly;
    using Testing;
    using TransactionProcessorACL.DataTransferObjects.Requests;
    using Xunit;
    using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
    using RequestTransactionMixBreakdown = TransactionProcessorACL.DataTransferObjects.Requests.TransactionMixBreakdown;
    using RequestTransactionMixMeasure = TransactionProcessorACL.DataTransferObjects.Requests.TransactionMixMeasure;

    /// <summary>
    /// 
    /// </summary>
    public class RequestHandlerTests
    {
        #region Methods

        public RequestHandlerTests()
        {
            this.SetupMemoryConfiguration();
        }

        private void SetupMemoryConfiguration()
        {
            //if (ConfigurationReader.IsInitialised == false)
            //{
                IConfigurationRoot configuration = new ConfigurationBuilder().AddInMemoryCollection(TestData.DefaultAppSettings).Build();
                ConfigurationReader.Initialise(configuration);
            //}
        }

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
            TransactionRequestHandler requestHandler = new TransactionRequestHandler(applicationService.Object);

            TransactionCommands.ProcessLogonTransactionCommand command = TestData.ProcessLogonTransactionCommand;
            Result<ProcessLogonTransactionResponse> result = await requestHandler.Handle(command, CancellationToken.None);

            result.IsSuccess.ShouldBeTrue();
            result.Data.ShouldNotBeNull();
            result.Data.ResponseCode.ShouldBe(TestData.ResponseCode);
            result.Data.ResponseMessage.ShouldBe(TestData.ResponseMessage);
            result.Data.EstateId.ShouldBe(TestData.EstateId);
            result.Data.MerchantId.ShouldBe(TestData.MerchantId);
        }

        [Fact]
        public async Task ProcessSaleTransactionRequestHandler_Handle_RequestIsHandled()
        {
            Mock<ITransactionProcessorACLApplicationService> applicationService = new Mock<ITransactionProcessorACLApplicationService>();
            applicationService
                .Setup(a => a.ProcessSaleTransaction(It.IsAny<(Guid, Guid)>(),
                                                      It.IsAny<DateTime>(),
                                                      It.IsAny<String>(),
                                                      It.IsAny<String>(),
                                                      It.IsAny<String>(),
                                                      It.IsAny<(Guid, Guid, Guid)>(),
                                                      It.IsAny<Dictionary<String,String>>(),
                                                      It.IsAny<CancellationToken>())).ReturnsAsync(TestData.ProcessSaleTransactionResponse);

            TransactionRequestHandler requestHandler = new TransactionRequestHandler(applicationService.Object);

            TransactionCommands.ProcessSaleTransactionCommand command = TestData.ProcessSaleTransactionCommand;
            Result<ProcessSaleTransactionResponse> result = await requestHandler.Handle(command, CancellationToken.None);

            result.IsSuccess.ShouldBeTrue();
            result.Data.ShouldNotBeNull();
            result.Data.ResponseCode.ShouldBe(TestData.ResponseCode);
            result.Data.ResponseMessage.ShouldBe(TestData.ResponseMessage);
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
            TransactionRequestHandler requestHandler = new TransactionRequestHandler(applicationService.Object);

            TransactionCommands.ProcessReconciliationCommand command = TestData.ProcessReconciliationCommand;
            Result<ProcessReconciliationResponse> result = await requestHandler.Handle(command, CancellationToken.None);

            result.IsSuccess.ShouldBeTrue();
            result.Data.ShouldNotBeNull();
            result.Data.ResponseCode.ShouldBe(TestData.ResponseCode);
            result.Data.ResponseMessage.ShouldBe(TestData.ResponseMessage);
            result.Data.EstateId.ShouldBe(TestData.EstateId);
            result.Data.MerchantId.ShouldBe(TestData.MerchantId);
        }

        [Fact]
        public async Task VersionCheckRequestHandler_Handle_RequestIsHandled()
        {
            VersionCheckRequestHandler requestHandler = new VersionCheckRequestHandler();
            
            VersionCheckCommands.VersionCheckCommand command = TestData.VersionCheckCommand;
            var result = await requestHandler.Handle(command, CancellationToken.None);
            result.IsSuccess.ShouldBeTrue();
        }

        [Fact]
        public async Task VersionCheckRequestHandler_Handle_OldVersion_ErrorThrown()
        {
            VersionCheckRequestHandler requestHandler = new VersionCheckRequestHandler();

            VersionCheckCommands.VersionCheckCommand command = new(TestData.OldApplicationVersion);
            var result = await requestHandler.Handle(command, CancellationToken.None);
            result.IsFailed.ShouldBeTrue();
            result.Status.ShouldBe(ResultStatus.Conflict);
        }

        [Fact]
        public async Task VersionCheckRequestHandler_Handle_NewerVersionBuildNumber_RequestIsHandled()
        {
            VersionCheckRequestHandler requestHandler = new VersionCheckRequestHandler();

            VersionCheckCommands.VersionCheckCommand command = new(TestData.NewerApplicationVersion);
            var result = await requestHandler.Handle(command, CancellationToken.None);
            result.IsSuccess.ShouldBeTrue();
        }

        [Fact]
        public async Task VersionCheckRequestHandler_Handle_SkipVersionCheck_RequestIsHandled()
        {
            IConfigurationRoot configuration = new ConfigurationBuilder().AddInMemoryCollection(TestData.DefaultAppSettingsSkipVersionCheck).Build();
            ConfigurationReader.Initialise(configuration);

            VersionCheckRequestHandler requestHandler = new VersionCheckRequestHandler();

            VersionCheckCommands.VersionCheckCommand command = new(TestData.NewerApplicationVersion);
            var result = await requestHandler.Handle(command, CancellationToken.None);
            result.IsSuccess.ShouldBeTrue(); ;
        }

        [Fact]
        public async Task VersionCheckRequestHandler_Handle_NullVersionInRequest_RequestIsHandled()
        {
            VersionCheckRequestHandler requestHandler = new VersionCheckRequestHandler();

            VersionCheckCommands.VersionCheckCommand command = new(null);
            var result = await requestHandler.Handle(command, CancellationToken.None);
            result.IsFailed.ShouldBeTrue();
            result.Status.ShouldBe(ResultStatus.Conflict);
        }

        [Fact]
        public async Task VoucherRequestHandler_GetVoucherRequest_Handle_RequestIsHandled()
        {
            Mock<ITransactionProcessorACLApplicationService> applicationService = new Mock<ITransactionProcessorACLApplicationService>();
            VoucherRequestHandler requestHandler = new VoucherRequestHandler(applicationService.Object);

            Should.NotThrow(async () =>
                            {
                                await requestHandler.Handle(TestData.GetVoucherQuery, CancellationToken.None);
                            });
        }

        [Fact]
        public async Task VoucherRequestHandler_RedeemVoucherRequest_Handle_RequestIsHandled()
        {
            Mock<ITransactionProcessorACLApplicationService> applicationService = new Mock<ITransactionProcessorACLApplicationService>();
            VoucherRequestHandler requestHandler = new VoucherRequestHandler(applicationService.Object);

            Should.NotThrow(async () =>
                            {
                                await requestHandler.Handle(TestData.RedeemVoucherCommand, CancellationToken.None);
                            });
        }

        [Fact]
        public async Task ReportingRequestHandler_GetMerchantTransactionMixSummaryQuery_Handle_RequestIsHandled()
        {
            Mock<ITransactionProcessorACLApplicationService> applicationService = new Mock<ITransactionProcessorACLApplicationService>();
            applicationService
                .Setup(a => a.GetMerchantTransactionMixSummary(It.IsAny<Guid>(),
                                                               It.IsAny<MerchantTransactionMixSummaryRequest>(),
                                                               It.IsAny<CancellationToken>()))
                .ReturnsAsync(new MerchantTransactionMixSummaryResponse());

            ReportingRequestHandler requestHandler = new ReportingRequestHandler(applicationService.Object);

            ReportingQueries.GetMerchantTransactionMixSummaryQuery query = new(
                TestData.EstateId,
                new MerchantTransactionMixSummaryRequest
                {
                    MerchantReportingId = 12345,
                    StartDate = new DateTime(2026, 7, 1),
                    EndDate = new DateTime(2026, 7, 3),
                    Breakdown = RequestTransactionMixBreakdown.Product,
                    Measure = RequestTransactionMixMeasure.Count,
                    TopN = 5
                });

            Result<MerchantTransactionMixSummaryResponse> result = await requestHandler.Handle(query, CancellationToken.None);

            result.IsSuccess.ShouldBeTrue();
            result.Data.ShouldNotBeNull();
        }

        [Fact]
        public async Task ReportingRequestHandler_GetMerchantDailyPerformanceSummaryQuery_Handle_RequestIsHandled()
        {
            Mock<ITransactionProcessorACLApplicationService> applicationService = new Mock<ITransactionProcessorACLApplicationService>();
            applicationService
                .Setup(a => a.GetMerchantDailyPerformanceSummary(It.IsAny<Guid>(),
                                                                 It.IsAny<MerchantDailyPerformanceSummaryRequest>(),
                                                                 It.IsAny<CancellationToken>()))
                .ReturnsAsync(new MerchantDailyPerformanceSummaryResponse());

            ReportingRequestHandler requestHandler = new ReportingRequestHandler(applicationService.Object);

            ReportingQueries.GetMerchantDailyPerformanceSummaryQuery query = new(
                TestData.EstateId,
                new MerchantDailyPerformanceSummaryRequest
                {
                    MerchantReportingId = 12345,
                    StartDate = new DateTime(2026, 7, 1),
                    EndDate = new DateTime(2026, 7, 1)
                });

            Result<MerchantDailyPerformanceSummaryResponse> result = await requestHandler.Handle(query, CancellationToken.None);

            result.IsSuccess.ShouldBeTrue();
            result.Data.ShouldNotBeNull();
        }

        #endregion
    }
}
