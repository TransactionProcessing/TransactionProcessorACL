using System;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Imposter.Abstractions;
using Shouldly;
using SimpleResults;
using TransactionProcessorACL.BusinessLogic.Requests;
using TransactionProcessorACL.DataTransferObjects.Requests;
using TransactionProcessorACL.Handlers;
using Xunit;

namespace TransactionProcessorACL.Tests.Handlers;

public class ReportingHandlersTests
{
    [Fact]
    public async Task GetMerchantDailyPerformanceSummary_PassesEstateClaimAndRequestToMediator()
    {
        var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim("estateId", "1C8354B7-B97A-46EA-9AD1-C43F33F7E3C3")
        }, "Bearer"));

        var request = new MerchantDailyPerformanceSummaryRequest
        {
            MerchantReportingId = 12345,
            StartDate = new DateTime(2026, 7, 1),
            EndDate = new DateTime(2026, 7, 1)
        };

        ReportingQueries.GetMerchantDailyPerformanceSummaryQuery? capturedQuery = null;

        var mediator = new IMediatorImposter(ImposterMode.Explicit);
        mediator
            .Send<Result<TransactionProcessorACL.Models.MerchantDailyPerformanceSummaryResponse>>(Arg<MediatR.IRequest<Result<TransactionProcessorACL.Models.MerchantDailyPerformanceSummaryResponse>>>.Any(), Arg<CancellationToken>.Any())
            .ReturnsAsync(Result.Success(new TransactionProcessorACL.Models.MerchantDailyPerformanceSummaryResponse()))
            .Callback((payload, _) => { capturedQuery = (ReportingQueries.GetMerchantDailyPerformanceSummaryQuery)payload; return Task.CompletedTask; });

        var result = await ReportingHandlers.GetMerchantDailyPerformanceSummary(user, request, mediator.Instance(), CancellationToken.None);

        result.ShouldNotBeNull();
        capturedQuery.ShouldNotBeNull();
        capturedQuery!.EstateId.ShouldBe(Guid.Parse("1C8354B7-B97A-46EA-9AD1-C43F33F7E3C3"));
        capturedQuery.Request.MerchantReportingId.ShouldBe(12345);
        mediator.Send<Result<TransactionProcessorACL.Models.MerchantDailyPerformanceSummaryResponse>>(Arg<MediatR.IRequest<Result<TransactionProcessorACL.Models.MerchantDailyPerformanceSummaryResponse>>>.Any(), Arg<CancellationToken>.Any()).Called(Count.Once());
    }

    [Fact]
    public async Task GetMerchantTransactionMixSummary_PassesEstateClaimAndRequestToMediator()
    {
        var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim("estateId", "1C8354B7-B97A-46EA-9AD1-C43F33F7E3C3")
        }, "Bearer"));

        var request = new MerchantTransactionMixSummaryRequest
        {
            MerchantReportingId = 12345,
            StartDate = new DateTime(2026, 7, 1),
            EndDate = new DateTime(2026, 7, 3),
            Breakdown = TransactionMixBreakdown.Product,
            Measure = TransactionMixMeasure.Count,
            TopN = 5
        };

        ReportingQueries.GetMerchantTransactionMixSummaryQuery? capturedQuery = null;

        var mediator = new IMediatorImposter(ImposterMode.Explicit);
        mediator
            .Send<Result<TransactionProcessorACL.Models.MerchantTransactionMixSummaryResponse>>(Arg<MediatR.IRequest<Result<TransactionProcessorACL.Models.MerchantTransactionMixSummaryResponse>>>.Any(), Arg<CancellationToken>.Any())
            .ReturnsAsync(Result.Success(new TransactionProcessorACL.Models.MerchantTransactionMixSummaryResponse()))
            .Callback((payload, _) => { capturedQuery = (ReportingQueries.GetMerchantTransactionMixSummaryQuery)payload; return Task.CompletedTask; });

        var result = await ReportingHandlers.GetMerchantTransactionMixSummary(user, request, mediator.Instance(), CancellationToken.None);

        result.ShouldNotBeNull();
        capturedQuery.ShouldNotBeNull();
        capturedQuery!.EstateId.ShouldBe(Guid.Parse("1C8354B7-B97A-46EA-9AD1-C43F33F7E3C3"));
        capturedQuery.Request.MerchantReportingId.ShouldBe(12345);
        capturedQuery.Request.Breakdown.ShouldBe(TransactionMixBreakdown.Product);
        capturedQuery.Request.Measure.ShouldBe(TransactionMixMeasure.Count);
        mediator.Send<Result<TransactionProcessorACL.Models.MerchantTransactionMixSummaryResponse>>(Arg<MediatR.IRequest<Result<TransactionProcessorACL.Models.MerchantTransactionMixSummaryResponse>>>.Any(), Arg<CancellationToken>.Any()).Called(Count.Once());
    }

    [Fact]
    public async Task GetRecentActivityReceiptSearch_PassesEstateClaimAndRequestToMediator()
    {
        var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim("estateId", "1C8354B7-B97A-46EA-9AD1-C43F33F7E3C3")
        }, "Bearer"));

        var request = new TransactionProcessorACL.DataTransferObjects.Requests.RecentActivityReceiptSearchRequest
        {
            MerchantReportingId = 12345,
            ReportDate = new DateTime(2026, 7, 8),
            SearchText = "abc",
            PageNumber = 2,
            PageSize = 5
        };

        ReportingQueries.GetRecentActivityReceiptSearchQuery? capturedQuery = null;

        var mediator = new IMediatorImposter(ImposterMode.Explicit);
        mediator
            .Send<Result<TransactionProcessorACL.Models.RecentActivityReceiptSearchResponse>>(Arg<MediatR.IRequest<Result<TransactionProcessorACL.Models.RecentActivityReceiptSearchResponse>>>.Any(), Arg<CancellationToken>.Any())
            .ReturnsAsync(Result.Success(new TransactionProcessorACL.Models.RecentActivityReceiptSearchResponse()))
            .Callback((payload, _) => { capturedQuery = (ReportingQueries.GetRecentActivityReceiptSearchQuery)payload; return Task.CompletedTask; });

        var result = await ReportingHandlers.GetRecentActivityReceiptSearch(user, request, mediator.Instance(), CancellationToken.None);

        result.ShouldNotBeNull();
        capturedQuery.ShouldNotBeNull();
        capturedQuery!.EstateId.ShouldBe(Guid.Parse("1C8354B7-B97A-46EA-9AD1-C43F33F7E3C3"));
        capturedQuery.Request.MerchantReportingId.ShouldBe(12345);
        capturedQuery.Request.ReportDate.ShouldBe(new DateTime(2026, 7, 8));
        capturedQuery.Request.SearchText.ShouldBe("abc");
        capturedQuery.Request.PageNumber.ShouldBe(2);
        capturedQuery.Request.PageSize.ShouldBe(5);
        mediator.Send<Result<TransactionProcessorACL.Models.RecentActivityReceiptSearchResponse>>(Arg<MediatR.IRequest<Result<TransactionProcessorACL.Models.RecentActivityReceiptSearchResponse>>>.Any(), Arg<CancellationToken>.Any()).Called(Count.Once());
    }
}
