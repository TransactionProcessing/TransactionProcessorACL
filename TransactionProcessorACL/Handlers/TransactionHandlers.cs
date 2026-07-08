using MediatR;
using Microsoft.AspNetCore.Http;
using Shared.General;
using Shared.Results.Web;
using SimpleResults;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using System.Net.Mail;
using TransactionProcessorACL.BusinessLogic.Requests;
using TransactionProcessorACL.DataTransferObjects;
using TransactionProcessorACL.DataTransferObjects.Requests;
using TransactionProcessorACL.Factories;
using TransactionProcessorACL.Models;

namespace TransactionProcessorACL.Handlers;

/// <summary>
/// Static handler methods for transaction processing; callable from minimal endpoints or controllers.
/// Returns IResult so endpoints can return directly.
/// </summary>
    public static class TransactionHandlers
{
    public static async Task<IResult> PerformSaleTransaction(IMediator mediator,
                                                             ClaimsPrincipal user,
                                                             SaleTransactionRequestMessage transactionRequest,
                                                             CancellationToken cancellationToken)
    {
        Result<(Guid estateId, Guid merchantId)> claimsResult = Helpers.GetRequiredClaims(user);
        if (claimsResult.IsFailed) {
            return ResponseFactory.FromResult(Result.Forbidden());
        }

        TransactionCommands.ProcessSaleTransactionCommand saleCommand = CreateSaleCommand(claimsResult.Data.estateId, claimsResult.Data.merchantId, transactionRequest);
        Result<ProcessSaleTransactionResponse> saleResponse = await mediator.Send(saleCommand, cancellationToken);
        return ResponseFactory.FromResult(saleResponse, ModelFactory.ConvertFrom);
    }

    public static async Task<IResult> PerformLogonTransaction(IMediator mediator,
                                                              ClaimsPrincipal user,
                                                              LogonTransactionRequestMessage transactionRequest,
                                                              CancellationToken cancellationToken)
    {
        Result<(Guid estateId, Guid merchantId)> claimsResult = Helpers.GetRequiredClaims(user);
        if (claimsResult.IsFailed) {
            return ResponseFactory.FromResult(Result.Forbidden());
        }

        TransactionCommands.ProcessLogonTransactionCommand logonCommand = CreateLogonCommand(claimsResult.Data.estateId, claimsResult.Data.merchantId, transactionRequest);
        Result<ProcessLogonTransactionResponse> logonResponse = await mediator.Send(logonCommand, cancellationToken);
        return ResponseFactory.FromResult(logonResponse, ModelFactory.ConvertFrom);
    }

    public static async Task<IResult> PerformReconciliationTransaction(IMediator mediator,
                                                                       ClaimsPrincipal user,
                                                                       ReconciliationRequestMessage transactionRequest,
                                                                       CancellationToken cancellationToken)
    {
        Result<(Guid estateId, Guid merchantId)> claimsResult = Helpers.GetRequiredClaims(user);
        if (claimsResult.IsFailed) {
            return ResponseFactory.FromResult(Result.Forbidden());
        }

        TransactionCommands.ProcessReconciliationCommand reconCommand = CreateReconciliationCommand(claimsResult.Data.estateId, claimsResult.Data.merchantId, transactionRequest);
        Result<ProcessReconciliationResponse> reconResponse = await mediator.Send(reconCommand, cancellationToken);
        return ResponseFactory.FromResult(reconResponse, ModelFactory.ConvertFrom);
    }

    public static async Task<IResult> ResendReceipt(IMediator mediator,
                                                    ClaimsPrincipal user,
                                                    ResendReceiptRequestMessage transactionRequest,
                                                    CancellationToken cancellationToken)
    {
        Result<(Guid estateId, Guid merchantId)> claimsResult = Helpers.GetRequiredClaims(user);
        if (claimsResult.IsFailed) {
            return ResponseFactory.FromResult(Result.Forbidden());
        }

        if (string.IsNullOrWhiteSpace(transactionRequest.Reference)) {
            return Results.BadRequest(new ResendReceiptResponse { Success = false, Message = "A transaction reference or receipt reference is required." });
        }

        if (string.IsNullOrWhiteSpace(transactionRequest.RecipientEmailAddress)) {
            return Results.BadRequest(new ResendReceiptResponse { Success = false, Message = "Recipient email address is required." });
        }

        try {
            _ = new MailAddress(transactionRequest.RecipientEmailAddress);
        }
        catch (FormatException) {
            return Results.BadRequest(new ResendReceiptResponse { Success = false, Message = "Recipient email address is not valid." });
        }

        TransactionCommands.ResendReceiptCommand resendCommand = new(claimsResult.Data.estateId,
                                                                     claimsResult.Data.merchantId,
                                                                     transactionRequest.Reference,
                                                                     transactionRequest.RecipientEmailAddress);
        Result<ResendReceiptResponse> resendResponse = await mediator.Send(resendCommand, cancellationToken);

        if (resendResponse.IsFailed && resendResponse.Status == ResultStatus.NotFound) {
            return Results.NotFound(new ResendReceiptResponse { Success = false, Message = resendResponse.Message });
        }

        return ResponseFactory.FromResult(resendResponse, response => response);
    }

        private static TransactionCommands.ProcessLogonTransactionCommand CreateLogonCommand(Guid estateId, Guid merchantId, LogonTransactionRequestMessage msg)
        {
            return new TransactionCommands.ProcessLogonTransactionCommand(estateId, merchantId, msg.TransactionDateTime, msg.TransactionNumber, msg.DeviceIdentifier);
        }

    private static TransactionCommands.ProcessSaleTransactionCommand CreateSaleCommand(Guid estateId, Guid merchantId, SaleTransactionRequestMessage msg)
    {
        return new TransactionCommands.ProcessSaleTransactionCommand(
            estateId,
            merchantId,
            msg.TransactionDateTime,
            msg.TransactionNumber,
            msg.DeviceIdentifier,
            msg.OperatorId,
            msg.CustomerEmailAddress,
            msg.ContractId,
            msg.ProductId,
            msg.AdditionalRequestMetadata);
    }

    private static TransactionCommands.ProcessReconciliationCommand CreateReconciliationCommand(Guid estateId, Guid merchantId, ReconciliationRequestMessage msg)
    {
        return new TransactionCommands.ProcessReconciliationCommand(
            estateId,
            merchantId,
            msg.TransactionDateTime,
            msg.DeviceIdentifier,
            msg.TransactionCount,
            msg.TransactionValue);
    }
}

public static class Helpers
{
    public static Result<(Guid estateId, Guid merchantId)> GetRequiredClaims(ClaimsPrincipal user)
    {
        Claim? estateClaim = user.Claims.FirstOrDefault(c => string.Equals(c.Type, "estateId", StringComparison.OrdinalIgnoreCase));
        if (!Guid.TryParse(estateClaim?.Value, out Guid estateId))
            return Result.Failure("No Claim found for Estate Id");

        Claim? merchantClaim = user.Claims.FirstOrDefault(c => string.Equals(c.Type, "merchantId", StringComparison.OrdinalIgnoreCase));
        if (!Guid.TryParse(merchantClaim?.Value, out Guid merchantId))
            return Result.Failure("No Claim found for Merchant Id");

        return Result.Success((estateId, merchantId));
    }

    public static Result<Guid> GetRequiredEstateClaim(ClaimsPrincipal user)
    {
        Claim? estateClaim = user.Claims.FirstOrDefault(c => string.Equals(c.Type, "estateId", StringComparison.OrdinalIgnoreCase));
        if (!Guid.TryParse(estateClaim?.Value, out Guid estateId))
            return Result.Failure("No Claim found for Estate Id");

        return Result.Success(estateId);
    }
}
