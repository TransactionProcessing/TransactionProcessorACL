using System;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Shouldly;
using SimpleResults;
using TransactionProcessorACL.BusinessLogic.Services;
using TransactionProcessorACL.DataTransferObjects.Requests;
using TransactionProcessorACL.Handlers;
using Xunit;

namespace TransactionProcessorACL.Tests.Handlers;

public class ReportingHandlersTests
{
    [Fact]
    public async Task GetMerchantDailyPerformanceSummary_PassesEstateClaimAndRequestToApplicationService()
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

        MerchantDailyPerformanceSummaryRequest? capturedRequest = null;
        Guid capturedEstateId = Guid.Empty;

        var applicationService = new Mock<ITransactionProcessorACLApplicationService>(MockBehavior.Strict);
        applicationService
            .Setup(a => a.GetMerchantDailyPerformanceSummary(It.IsAny<Guid>(), It.IsAny<MerchantDailyPerformanceSummaryRequest>(), It.IsAny<CancellationToken>()))
            .Callback<Guid, MerchantDailyPerformanceSummaryRequest, CancellationToken>((estateId, payload, _) =>
            {
                capturedEstateId = estateId;
                capturedRequest = payload;
            })
            .ReturnsAsync(Result.Success(new TransactionProcessorACL.Models.MerchantDailyPerformanceSummaryResponse()));

        var result = await ReportingHandlers.GetMerchantDailyPerformanceSummary(user, request, applicationService.Object, CancellationToken.None);

        result.ShouldNotBeNull();
        capturedEstateId.ShouldBe(Guid.Parse("1C8354B7-B97A-46EA-9AD1-C43F33F7E3C3"));
        capturedRequest.ShouldNotBeNull();
        capturedRequest!.MerchantReportingId.ShouldBe(12345);
        applicationService.Verify(a => a.GetMerchantDailyPerformanceSummary(It.IsAny<Guid>(), It.IsAny<MerchantDailyPerformanceSummaryRequest>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}
