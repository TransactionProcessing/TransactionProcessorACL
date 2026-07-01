using System;
using System.Collections.Generic;
using System.Net;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Shouldly;
using SimpleResults;
using TransactionProcessorACL.BusinessLogic.Requests;
using TransactionProcessorACL.Common;
using TransactionProcessorACL.Models;
using Xunit;

namespace TransactionProcessorACL.Tests.Audit;

public class AuditPipelineBehaviorTests
{
    [Fact]
    public async Task Handle_WhenPipelineRuns_RecordsSingleAuditEventWithBusinessContext()
    {
        var httpContext = new DefaultHttpContext();
        httpContext.TraceIdentifier = "trace-123";
        httpContext.Request.Method = HttpMethods.Post;
        httpContext.Request.Path = "/api/saletransactions";
        httpContext.Request.Headers["User-Agent"] = "TestClient/1.0";
        httpContext.Connection.RemoteIpAddress = IPAddress.Parse("203.0.113.9");
        httpContext.User = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim("estateId", "1C8354B7-B97A-46EA-9AD1-C43F33F7E3C3"),
            new Claim("merchantId", "2C8354B7-B97A-46EA-9AD1-C43F33F7E3C4"),
        }, "Bearer"));

        var accessor = new HttpContextAccessor { HttpContext = httpContext };
        var recorder = new CapturingAuditRecorder();
        var behavior = new AuditPipelineBehavior<TransactionCommands.ProcessSaleTransactionCommand, Result<ProcessSaleTransactionResponse>>(accessor, recorder);
        var request = new TransactionCommands.ProcessSaleTransactionCommand(
            Guid.Parse("1C8354B7-B97A-46EA-9AD1-C43F33F7E3C3"),
            Guid.Parse("2C8354B7-B97A-46EA-9AD1-C43F33F7E3C4"),
            new System.DateTime(2026, 6, 30, 10, 15, 0),
            "TX-0001",
            "device-01",
            Guid.Parse("5C8354B7-B97A-46EA-9AD1-C43F33F7E3C7"),
            "customer@example.com",
            Guid.Parse("3C8354B7-B97A-46EA-9AD1-C43F33F7E3C5"),
            Guid.Parse("4C8354B7-B97A-46EA-9AD1-C43F33F7E3C6"),
            new Dictionary<string, string> { ["amount"] = "1000.00" });

        Result<ProcessSaleTransactionResponse> response = await behavior.Handle(request, cancellationToken => Task.FromResult(Result.Success(new ProcessSaleTransactionResponse())), CancellationToken.None);

        response.IsSuccess.ShouldBeTrue();
        recorder.CallCount.ShouldBe(1);
        recorder.LastEvent.ShouldNotBeNull();
        Guid.TryParse(recorder.LastEvent!.Context.RequestId, out _).ShouldBeTrue();
        recorder.LastEvent.Context.EstateId.ShouldBe(Guid.Parse("1C8354B7-B97A-46EA-9AD1-C43F33F7E3C3"));
        recorder.LastEvent.Context.MerchantId.ShouldBe(Guid.Parse("2C8354B7-B97A-46EA-9AD1-C43F33F7E3C4"));
        recorder.LastEvent.RequestType.ShouldBe(nameof(TransactionCommands.ProcessSaleTransactionCommand));
        recorder.LastEvent.Context.BusinessContext["amount"].ShouldBe("1000.00");
    }

    [Fact]
    public async Task Handle_WhenVersionCheckRuns_WithClaims_RecordsEstateAndMerchantIds()
    {
        var httpContext = new DefaultHttpContext();
        httpContext.TraceIdentifier = "trace-456";
        httpContext.Request.Method = HttpMethods.Post;
        httpContext.Request.Path = "/api/versioncheck";
        httpContext.User = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim("estateId", "1C8354B7-B97A-46EA-9AD1-C43F33F7E3C3"),
            new Claim("merchantId", "2C8354B7-B97A-46EA-9AD1-C43F33F7E3C4"),
        }, "Bearer"));

        var accessor = new HttpContextAccessor { HttpContext = httpContext };
        var recorder = new CapturingAuditRecorder();
        var behavior = new AuditPipelineBehavior<VersionCheckCommands.VersionCheckCommand, Result>(accessor, recorder);
        var request = new VersionCheckCommands.VersionCheckCommand("9.9.9");

        Result response = await behavior.Handle(request, cancellationToken => Task.FromResult(Result.Success()), CancellationToken.None);

        response.IsSuccess.ShouldBeTrue();
        recorder.CallCount.ShouldBe(1);
        recorder.LastEvent.ShouldNotBeNull();
        recorder.LastEvent!.Context.EstateId.ShouldBe(Guid.Parse("1C8354B7-B97A-46EA-9AD1-C43F33F7E3C3"));
        recorder.LastEvent.Context.MerchantId.ShouldBe(Guid.Parse("2C8354B7-B97A-46EA-9AD1-C43F33F7E3C4"));
        recorder.LastEvent.RequestType.ShouldBe(nameof(VersionCheckCommands.VersionCheckCommand));
        recorder.LastEvent.Context.BusinessContext["application_version"].ShouldBe("9.9.9");
    }

    private sealed class CapturingAuditRecorder : IRequestAuditRecorder
    {
        public int CallCount { get; private set; }

        public RequestAuditEvent? LastEvent { get; private set; }

        public Task RecordAsync(RequestAuditEvent auditEvent, CancellationToken cancellationToken)
        {
            CallCount++;
            LastEvent = auditEvent;
            return Task.CompletedTask;
        }
    }
}
