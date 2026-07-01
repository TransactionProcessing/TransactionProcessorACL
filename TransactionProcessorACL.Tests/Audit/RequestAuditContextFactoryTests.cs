using System;
using System.Collections.Generic;
using System.Net;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Shouldly;
using TransactionProcessorACL.DataTransferObjects;
using TransactionProcessorACL.Common;
using Xunit;

namespace TransactionProcessorACL.Tests.Audit;

public class RequestAuditContextFactoryTests
{
    [Fact]
    public void CreateForTransaction_WhenRequestHasCorrelationHeader_CapturesRequestAndBusinessContext()
    {
        var context = new DefaultHttpContext();
        context.TraceIdentifier = "trace-123";
        context.Request.Method = HttpMethods.Post;
        context.Request.Path = "/api/saletransactions";
        context.Request.Headers["User-Agent"] = "TestClient/1.0";
        context.Connection.RemoteIpAddress = IPAddress.Parse("203.0.113.9");

        var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim("estateId", "1C8354B7-B97A-46EA-9AD1-C43F33F7E3C3"),
            new Claim("merchantId", "2C8354B7-B97A-46EA-9AD1-C43F33F7E3C4"),
        }));

        var request = new SaleTransactionRequestMessage
        {
            TransactionNumber = "TX-0001",
            DeviceIdentifier = "device-01",
            TransactionDateTime = new System.DateTime(2026, 6, 30, 10, 15, 0),
            CustomerEmailAddress = "customer@example.com",
            ContractId = System.Guid.Parse("3C8354B7-B97A-46EA-9AD1-C43F33F7E3C5"),
            ProductId = System.Guid.Parse("4C8354B7-B97A-46EA-9AD1-C43F33F7E3C6"),
            OperatorId = System.Guid.Parse("5C8354B7-B97A-46EA-9AD1-C43F33F7E3C7"),
            AdditionalRequestMetadata = new Dictionary<string, string>
            {
                ["amount"] = "1000.00"
            }
        };

        RequestAuditContext auditContext = RequestAuditContextFactory.CreateForTransaction(context, user, request, "SALE");
        RequestAuditContext secondAuditContext = RequestAuditContextFactory.CreateForRequest(context);

        Guid.TryParse(auditContext.RequestId, out _).ShouldBeTrue();
        secondAuditContext.RequestId.ShouldBe(auditContext.RequestId);
        auditContext.TraceId.ShouldBe("trace-123");
        auditContext.Method.ShouldBe("POST");
        auditContext.Route.ShouldBe("/api/saletransactions");
        auditContext.SourceIp.ShouldBe("203.0.113.9");
        auditContext.UserAgent.ShouldBe("TestClient/1.0");
        auditContext.EstateId.ShouldBe(System.Guid.Parse("1C8354B7-B97A-46EA-9AD1-C43F33F7E3C3"));
        auditContext.MerchantId.ShouldBe(System.Guid.Parse("2C8354B7-B97A-46EA-9AD1-C43F33F7E3C4"));
        auditContext.TransactionType.ShouldBe("SALE");
        auditContext.TransactionNumber.ShouldBe("TX-0001");
        auditContext.BusinessContext["customer_email_address"].ShouldBe("customer@example.com");
        auditContext.BusinessContext["contract_id"].ToUpperInvariant().ShouldBe("3C8354B7-B97A-46EA-9AD1-C43F33F7E3C5");
        auditContext.BusinessContext["product_id"].ToUpperInvariant().ShouldBe("4C8354B7-B97A-46EA-9AD1-C43F33F7E3C6");
        auditContext.BusinessContext["operator_id"].ToUpperInvariant().ShouldBe("5C8354B7-B97A-46EA-9AD1-C43F33F7E3C7");
        auditContext.BusinessContext["amount"].ShouldBe("1000.00");
    }
}
