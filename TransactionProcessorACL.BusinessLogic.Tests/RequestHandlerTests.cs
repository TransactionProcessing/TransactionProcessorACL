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
    using Imposter.Abstractions;
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
            ITransactionProcessorACLApplicationServiceImposter applicationService = new ITransactionProcessorACLApplicationServiceImposter();
            applicationService
                .ProcessLogonTransaction(Arg<Guid>.Any(),
                                                      Arg<Guid>.Any(),
                                                      Arg<DateTime>.Any(),
                                                      Arg<String>.Any(),
                                                      Arg<String>.Any(),
                                                      Arg<CancellationToken>.Any()).ReturnsAsync(TestData.ProcessLogonTransactionResponse);
            TransactionRequestHandler requestHandler = new TransactionRequestHandler(applicationService.Instance());

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
            ITransactionProcessorACLApplicationServiceImposter applicationService = new ITransactionProcessorACLApplicationServiceImposter();
            applicationService
                .ProcessSaleTransaction(Arg<(Guid, Guid)>.Any(),
                                                      Arg<DateTime>.Any(),
                                                      Arg<String>.Any(),
                                                      Arg<String>.Any(),
                                                      Arg<String>.Any(),
                                                      Arg<(Guid, Guid, Guid)>.Any(),
                                                      Arg<Dictionary<String,String>>.Any(),
                                                      Arg<CancellationToken>.Any()).ReturnsAsync(TestData.ProcessSaleTransactionResponse);

            TransactionRequestHandler requestHandler = new TransactionRequestHandler(applicationService.Instance());

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
            ITransactionProcessorACLApplicationServiceImposter applicationService = new ITransactionProcessorACLApplicationServiceImposter();
            applicationService
                .ProcessReconciliation(Arg<Guid>.Any(),
                                                      Arg<Guid>.Any(),
                                                      Arg<DateTime>.Any(),
                                                      Arg<String>.Any(),
                                                      Arg<Int32>.Any(),
                                                      Arg<Decimal>.Any(),
                                                      Arg<CancellationToken>.Any()).ReturnsAsync(TestData.ProcessReconciliationResponse);
            TransactionRequestHandler requestHandler = new TransactionRequestHandler(applicationService.Instance());

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
            ITransactionProcessorACLApplicationServiceImposter applicationService = new ITransactionProcessorACLApplicationServiceImposter();
            VoucherRequestHandler requestHandler = new VoucherRequestHandler(applicationService.Instance());

            Should.NotThrow(async () =>
                            {
                                await requestHandler.Handle(TestData.GetVoucherQuery, CancellationToken.None);
                            });
        }

        [Fact]
        public async Task VoucherRequestHandler_RedeemVoucherRequest_Handle_RequestIsHandled()
        {
            ITransactionProcessorACLApplicationServiceImposter applicationService = new ITransactionProcessorACLApplicationServiceImposter();
            VoucherRequestHandler requestHandler = new VoucherRequestHandler(applicationService.Instance());

            Should.NotThrow(async () =>
                            {
                                await requestHandler.Handle(TestData.RedeemVoucherCommand, CancellationToken.None);
                            });
        }

        [Fact]
        public async Task ReportingRequestHandler_GetMerchantTransactionMixSummaryQuery_Handle_RequestIsHandled()
        {
            ITransactionProcessorACLApplicationServiceImposter applicationService = new ITransactionProcessorACLApplicationServiceImposter();
            applicationService
                .GetMerchantTransactionMixSummary(Arg<Guid>.Any(),
                                                               Arg<MerchantTransactionMixSummaryRequest>.Any(),
                                                               Arg<CancellationToken>.Any())
                .ReturnsAsync(new MerchantTransactionMixSummaryResponse());

            ReportingRequestHandler requestHandler = new ReportingRequestHandler(applicationService.Instance());

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
            ITransactionProcessorACLApplicationServiceImposter applicationService = new ITransactionProcessorACLApplicationServiceImposter();
            applicationService
                .GetMerchantDailyPerformanceSummary(Arg<Guid>.Any(),
                                                                 Arg<MerchantDailyPerformanceSummaryRequest>.Any(),
                                                                 Arg<CancellationToken>.Any())
                .ReturnsAsync(new MerchantDailyPerformanceSummaryResponse());

            ReportingRequestHandler requestHandler = new ReportingRequestHandler(applicationService.Instance());

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

        [Fact]
        public async Task ReportingRequestHandler_GetRecentActivityReceiptSearchQuery_Handle_RequestIsHandled()
        {
            ITransactionProcessorACLApplicationServiceImposter applicationService = new ITransactionProcessorACLApplicationServiceImposter();
            applicationService
                .GetRecentActivityReceiptSearch(Arg<Guid>.Any(),
                                                             Arg<RecentActivityReceiptSearchRequest>.Any(),
                                                             Arg<CancellationToken>.Any())
                .ReturnsAsync(new RecentActivityReceiptSearchResponse());

            ReportingRequestHandler requestHandler = new ReportingRequestHandler(applicationService.Instance());

            ReportingQueries.GetRecentActivityReceiptSearchQuery query = new(
                TestData.EstateId,
                new RecentActivityReceiptSearchRequest
                {
                    MerchantReportingId = 12345,
                    ReportDate = new DateTime(2026, 7, 8),
                    SearchText = "abc",
                    PageNumber = 2,
                    PageSize = 5
                });

            Result<RecentActivityReceiptSearchResponse> result = await requestHandler.Handle(query, CancellationToken.None);

            result.IsSuccess.ShouldBeTrue();
            result.Data.ShouldNotBeNull();
        }

        [Fact]
        public async Task TransactionRequestHandler_ResendReceiptCommand_Handle_RequestIsHandled()
        {
            ITransactionProcessorACLApplicationServiceImposter applicationService = new ITransactionProcessorACLApplicationServiceImposter();
            applicationService
                .ResendReceipt(Arg<Guid>.Any(),
                                            Arg<Guid>.Any(),
                                            Arg<String>.Any(),
                                            Arg<String>.Any(),
                                            Arg<CancellationToken>.Any())
                .ReturnsAsync(new ResendReceiptResponse { Success = true, Message = "Receipt resend requested." });

            TransactionRequestHandler requestHandler = new TransactionRequestHandler(applicationService.Instance());

            TransactionCommands.ResendReceiptCommand command = new(
                TestData.EstateId,
                TestData.MerchantId,
                "RCPT-0001",
                "recipient@example.com");

            Result<ResendReceiptResponse> result = await requestHandler.Handle(command, CancellationToken.None);

            result.IsSuccess.ShouldBeTrue();
            result.Data.ShouldNotBeNull();
            result.Data.Success.ShouldBeTrue();
        }

        #endregion
    }
}
