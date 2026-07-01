using System;
using Shouldly;
using TransactionProcessorACL.Common;
using Xunit;

namespace TransactionProcessorACL.Tests.Audit;

public class RequestAuditStreamOptionsTests
{
    [Fact]
    public void DefaultLifetime_IsSevenDays()
    {
        var options = new RequestAuditStreamOptions();

        options.RequestAuditStreamLifetimeDays.ShouldBe(7);
        options.RequestAuditStreamLifetime.ShouldBe(TimeSpan.FromDays(7));
    }

    [Fact]
    public void CustomLifetime_ConvertsToTimeSpan()
    {
        var options = new RequestAuditStreamOptions
        {
            RequestAuditStreamLifetimeDays = 14
        };

        options.RequestAuditStreamLifetimeDays.ShouldBe(14);
        options.RequestAuditStreamLifetime.ShouldBe(TimeSpan.FromDays(14));
    }
}
