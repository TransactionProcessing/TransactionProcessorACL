using System.Collections.Generic;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Moq;
using Shouldly;
using SimpleResults;
using TransactionProcessorACL.BusinessLogic.Requests;
using TransactionProcessorACL.DataTransferObjects;
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

        var mediator = new Mock<IMediator>(MockBehavior.Strict);
        mediator
            .Setup(m => m.Send(It.IsAny<TransactionCommands.ProcessSaleTransactionCommand>(), It.IsAny<CancellationToken>()))
            .Callback<object, CancellationToken>((command, _) => capturedCommand = (TransactionCommands.ProcessSaleTransactionCommand)command)
            .ReturnsAsync(Result.Success(new TransactionProcessorACL.Models.ProcessSaleTransactionResponse()));

        await TransactionHandlers.PerformSaleTransaction(mediator.Object, user, request, CancellationToken.None);

        capturedCommand.ShouldNotBeNull();
        capturedCommand!.EstateId.ShouldBe(System.Guid.Parse("1C8354B7-B97A-46EA-9AD1-C43F33F7E3C3"));
        capturedCommand.MerchantId.ShouldBe(System.Guid.Parse("2C8354B7-B97A-46EA-9AD1-C43F33F7E3C4"));
        capturedCommand.TransactionNumber.ShouldBe("TX-0001");
        capturedCommand.DeviceIdentifier.ShouldBe("device-01");
        capturedCommand.CustomerEmailAddress.ShouldBe("customer@example.com");
        capturedCommand.AdditionalRequestMetadata["amount"].ShouldBe("1000.00");
        mediator.Verify(m => m.Send(It.IsAny<TransactionCommands.ProcessSaleTransactionCommand>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}
