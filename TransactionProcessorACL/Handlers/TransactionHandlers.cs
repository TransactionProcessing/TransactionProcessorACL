using MediatR;
using Microsoft.AspNetCore.Http;
using TransactionProcessing.Shared.General;
using TransactionProcessing.Shared.Logger;
using TransactionProcessing.UnionResults;
using Results = TransactionProcessing.UnionResults.Results;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using System.Net.Mail;
using TransactionProcessorACL.BusinessLogic.Requests;
using TransactionProcessorACL.DataTransferObjects;
using TransactionProcessorACL.DataTransferObjects.Requests;
using ApiResponses = TransactionProcessorACL.DataTransferObjects.Responses;
using TransactionProcessorACL.Factories;
using TransactionProcessorACL.Models;

namespace TransactionProcessorACL.Handlers;

/// <summary>
/// Static handler methods for transaction processing; callable from minimal endpoints or controllers.
/// Returns UnionResults outcomes for endpoint filter mapping.
/// </summary>
    public static class TransactionHandlers
{
    private const String ForbiddenRequestMessage = "The request is forbidden.";

    public static async Task<Result<ApiResponses.SaleTransactionResponseMessage>> PerformSaleTransaction(IMediator mediator,
                                                             ClaimsPrincipal user,
                                                             SaleTransactionRequestMessage transactionRequest,
                                                             CancellationToken cancellationToken)
    {
        Logger.LogInformation("Application version {ApplicationVersion}", transactionRequest.ApplicationVersion);

        Result<(Guid estateId, Guid merchantId)> claimsResult = Helpers.GetRequiredClaims(user);
        if (claimsResult.Value is not Success<(Guid estateId, Guid merchantId)> claims) {
            return new Error(ErrorType.Forbidden, ForbiddenRequestMessage);
        }

        TransactionCommands.ProcessSaleTransactionCommand saleCommand = CreateSaleCommand(claims.Value.estateId, claims.Value.merchantId, transactionRequest);
        Result<ProcessSaleTransactionResponse> saleResponse = await mediator.Send(saleCommand, cancellationToken);
        return ResponseFactory.FromResult(saleResponse, ModelFactory.ConvertFrom);
    }

    public static async Task<Result<ApiResponses.LogonTransactionResponseMessage>> PerformLogonTransaction(IMediator mediator,
                                                              ClaimsPrincipal user,
                                                              LogonTransactionRequestMessage transactionRequest,
                                                              CancellationToken cancellationToken)
    {
        Logger.LogInformation("Application version {ApplicationVersion}", transactionRequest.ApplicationVersion);

        Result<(Guid estateId, Guid merchantId)> claimsResult = Helpers.GetRequiredClaims(user);
        if (claimsResult.Value is not Success<(Guid estateId, Guid merchantId)> claims) {
            return new Error(ErrorType.Forbidden, ForbiddenRequestMessage);
        }

        TransactionCommands.ProcessLogonTransactionCommand logonCommand = CreateLogonCommand(claims.Value.estateId, claims.Value.merchantId, transactionRequest);
        Result<ProcessLogonTransactionResponse> logonResponse = await mediator.Send(logonCommand, cancellationToken);
        return ResponseFactory.FromResult(logonResponse, ModelFactory.ConvertFrom);
    }

    public static async Task<Result<ApiResponses.ReconciliationResponseMessage>> PerformReconciliationTransaction(IMediator mediator,
                                                                       ClaimsPrincipal user,
                                                                       ReconciliationRequestMessage transactionRequest,
                                                                       CancellationToken cancellationToken)
    {
        Logger.LogInformation("Application version {ApplicationVersion}", transactionRequest.ApplicationVersion);

        Result<(Guid estateId, Guid merchantId)> claimsResult = Helpers.GetRequiredClaims(user);
        if (claimsResult.Value is not Success<(Guid estateId, Guid merchantId)> claims) {
            return new Error(ErrorType.Forbidden, ForbiddenRequestMessage);
        }

        TransactionCommands.ProcessReconciliationCommand reconCommand = CreateReconciliationCommand(claims.Value.estateId, claims.Value.merchantId, transactionRequest);
        Result<ProcessReconciliationResponse> reconResponse = await mediator.Send(reconCommand, cancellationToken);
        return ResponseFactory.FromResult(reconResponse, ModelFactory.ConvertFrom);
    }

    public static async Task<Result<ApiResponses.ResendReceiptResponse>> ResendReceipt(IMediator mediator,
                                                    ClaimsPrincipal user,
                                                    ResendReceiptRequestMessage transactionRequest,
                                                    CancellationToken cancellationToken)
    {
        Result<(Guid estateId, Guid merchantId)> claimsResult = Helpers.GetRequiredClaims(user);
        if (claimsResult.Value is not Success<(Guid estateId, Guid merchantId)> claims) {
            return new Error(ErrorType.Forbidden, ForbiddenRequestMessage);
        }

        if (string.IsNullOrWhiteSpace(transactionRequest.Reference)) {
            return new Error(ErrorType.Validation, "A transaction reference or receipt reference is required.");
        }

        if (string.IsNullOrWhiteSpace(transactionRequest.RecipientEmailAddress)) {
            return new Error(ErrorType.Validation, "Recipient email address is required.");
        }

        try {
            _ = new MailAddress(transactionRequest.RecipientEmailAddress);
        }
        catch (FormatException) {
            return new Error(ErrorType.Validation, "Recipient email address is not valid.");
        }

        TransactionCommands.ResendReceiptCommand resendCommand = new(claims.Value.estateId,
                                                                     claims.Value.merchantId,
                                                                     transactionRequest.Reference,
                                                                     transactionRequest.RecipientEmailAddress);
        Result<ResendReceiptResponse> resendResponse = await mediator.Send(resendCommand, cancellationToken);

        return ResponseFactory.FromResult(resendResponse, response => new ApiResponses.ResendReceiptResponse
        {
            Success = response.Success,
            Message = response.Message,
            Reference = response.Reference,
            ReceiptReference = response.ReceiptReference,
            TransactionReference = response.TransactionReference
        });
    }

        private static TransactionCommands.ProcessLogonTransactionCommand CreateLogonCommand(Guid estateId, Guid merchantId, LogonTransactionRequestMessage msg)
        {
            return new TransactionCommands.ProcessLogonTransactionCommand(estateId, merchantId, msg.TransactionDateTime, DateTimeOffset.UtcNow, msg.TransactionNumber, msg.DeviceIdentifier);
        }

    private static TransactionCommands.ProcessSaleTransactionCommand CreateSaleCommand(Guid estateId, Guid merchantId, SaleTransactionRequestMessage msg)
    {
        return new TransactionCommands.ProcessSaleTransactionCommand(
            estateId,
            merchantId,
            msg.TransactionDateTime,
            DateTimeOffset.UtcNow,
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
            DateTimeOffset.UtcNow,
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
            return new Error(ErrorType.Failure, "No Claim found for Estate Id");

        Claim? merchantClaim = user.Claims.FirstOrDefault(c => string.Equals(c.Type, "merchantId", StringComparison.OrdinalIgnoreCase));
        if (!Guid.TryParse(merchantClaim?.Value, out Guid merchantId))
            return new Error(ErrorType.Failure, "No Claim found for Merchant Id");

        return Results.Ok((estateId, merchantId));
    }

    public static Result<Guid> GetRequiredEstateClaim(ClaimsPrincipal user)
    {
        Claim? estateClaim = user.Claims.FirstOrDefault(c => string.Equals(c.Type, "estateId", StringComparison.OrdinalIgnoreCase));
        if (!Guid.TryParse(estateClaim?.Value, out Guid estateId))
            return new Error(ErrorType.Failure, "No Claim found for Estate Id");

        return Results.Ok(estateId);
    }
}
