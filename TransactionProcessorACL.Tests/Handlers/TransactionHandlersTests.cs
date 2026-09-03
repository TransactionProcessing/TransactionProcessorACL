using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Imposter.Abstractions;
using Shouldly;
using SimpleResults;
using TransactionProcessorACL.BusinessLogic.Requests;
using TransactionProcessorACL.DataTransferObjects;
using TransactionProcessorACL.DataTransferObjects.Requests;
using TransactionProcessorACL.Handlers;
using Xunit;

namespace TransactionProcessorACL.Tests.Handlers;

public class TransactionHandlersTests
{
    [Fact]
    public async Task PerformSaleTransaction_PassesBusinessFieldsIntoCommand()
    {
        var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim("estateId", "1C8354B7-B97A-46EA-9AD1-C43F33F7E3C3"),
            new Claim("merchantId", "2C8354B7-B97A-46EA-9AD1-C43F33F7E3C4"),
        }, "Bearer"));

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

        TransactionCommands.ProcessSaleTransactionCommand? capturedCommand = null;

        var mediator = new IMediatorImposter(ImposterMode.Explicit);
        mediator
            .Send<Result<TransactionProcessorACL.Models.ProcessSaleTransactionResponse>>(Arg<MediatR.IRequest<Result<TransactionProcessorACL.Models.ProcessSaleTransactionResponse>>>.Any(), Arg<CancellationToken>.Any())
            .ReturnsAsync(Result.Success(new TransactionProcessorACL.Models.ProcessSaleTransactionResponse()))
            .Callback((command, _) => { capturedCommand = (TransactionCommands.ProcessSaleTransactionCommand)command; return Task.CompletedTask; });

        await TransactionHandlers.PerformSaleTransaction(mediator.Instance(), user, request, CancellationToken.None);

        capturedCommand.ShouldNotBeNull();
        capturedCommand!.EstateId.ShouldBe(System.Guid.Parse("1C8354B7-B97A-46EA-9AD1-C43F33F7E3C3"));
        capturedCommand.MerchantId.ShouldBe(System.Guid.Parse("2C8354B7-B97A-46EA-9AD1-C43F33F7E3C4"));
        capturedCommand.TransactionNumber.ShouldBe("TX-0001");
        capturedCommand.DeviceIdentifier.ShouldBe("device-01");
        capturedCommand.CustomerEmailAddress.ShouldBe("customer@example.com");
        capturedCommand.AdditionalRequestMetadata["amount"].ShouldBe("1000.00");
        mediator.Send<Result<TransactionProcessorACL.Models.ProcessSaleTransactionResponse>>(Arg<MediatR.IRequest<Result<TransactionProcessorACL.Models.ProcessSaleTransactionResponse>>>.Any(), Arg<CancellationToken>.Any()).Called(Count.Once());
    }

    [Fact]
    public async Task ResendReceipt_PassesClaimsAndRequestIntoCommand()
    {
        var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim("estateId", "1C8354B7-B97A-46EA-9AD1-C43F33F7E3C3"),
            new Claim("merchantId", "2C8354B7-B97A-46EA-9AD1-C43F33F7E3C4"),
        }, "Bearer"));

        var request = new ResendReceiptRequestMessage
        {
            Reference = "RCPT-0001",
            RecipientEmailAddress = "recipient@example.com"
        };

        TransactionCommands.ResendReceiptCommand? capturedCommand = null;

        var mediator = new IMediatorImposter(ImposterMode.Explicit);
        mediator
            .Send<Result<TransactionProcessorACL.Models.ResendReceiptResponse>>(Arg<MediatR.IRequest<Result<TransactionProcessorACL.Models.ResendReceiptResponse>>>.Any(), Arg<CancellationToken>.Any())
            .ReturnsAsync(Result.Success(new TransactionProcessorACL.Models.ResendReceiptResponse { Success = true, Message = "Receipt resend requested." }))
            .Callback((command, _) => { capturedCommand = (TransactionCommands.ResendReceiptCommand)command; return Task.CompletedTask; });

        await TransactionHandlers.ResendReceipt(mediator.Instance(), user, request, CancellationToken.None);

        capturedCommand.ShouldNotBeNull();
        capturedCommand!.EstateId.ShouldBe(System.Guid.Parse("1C8354B7-B97A-46EA-9AD1-C43F33F7E3C3"));
        capturedCommand.MerchantId.ShouldBe(System.Guid.Parse("2C8354B7-B97A-46EA-9AD1-C43F33F7E3C4"));
        capturedCommand.Reference.ShouldBe("RCPT-0001");
        capturedCommand.RecipientEmailAddress.ShouldBe("recipient@example.com");
        mediator.Send<Result<TransactionProcessorACL.Models.ResendReceiptResponse>>(Arg<MediatR.IRequest<Result<TransactionProcessorACL.Models.ResendReceiptResponse>>>.Any(), Arg<CancellationToken>.Any()).Called(Count.Once());
    }

    [Fact]
    public async Task ResendReceipt_InvalidEmail_IsRejected()
    {
        var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim("estateId", "1C8354B7-B97A-46EA-9AD1-C43F33F7E3C3"),
            new Claim("merchantId", "2C8354B7-B97A-46EA-9AD1-C43F33F7E3C4"),
        }, "Bearer"));

        var request = new ResendReceiptRequestMessage
        {
            Reference = "RCPT-0001",
            RecipientEmailAddress = "not-an-email"
        };

        var mediator = new IMediatorImposter(ImposterMode.Explicit);

        var result = await TransactionHandlers.ResendReceipt(mediator.Instance(), user, request, CancellationToken.None);

        result.ShouldNotBeNull();
    }

}
