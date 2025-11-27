using System;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Shared.General;
using Shared.Results.Web;
using SimpleResults;
using TransactionProcessorACL.BusinessLogic.Requests;
using TransactionProcessorACL.DataTransferObjects;
using TransactionProcessorACL.Factories;
using TransactionProcessorACL.Models;

namespace TransactionProcessorACL.Handlers
{
    /// <summary>
    /// Static handler methods for transaction processing; callable from minimal endpoints or controllers.
    /// Returns IResult so endpoints can return directly.
    /// </summary>
    public static class TransactionHandlers
    {
        public static async Task<IResult> PerformSaleTransaction(IMediator mediator,
                                                                  IModelFactory modelFactory,
                                                                  ClaimsPrincipal user,
                                                                  SaleTransactionRequestMessage transactionRequest,
                                                                  CancellationToken cancellationToken)
        {
            Result<(Guid estateId, Guid merchantId)> claimsResult = Helpers.GetRequiredClaims(user);
            if (claimsResult.IsFailed)
                return ResponseFactory.FromResult(Result.Forbidden());

            TransactionCommands.ProcessSaleTransactionCommand saleCommand = CreateSaleCommand(claimsResult.Data.estateId, claimsResult.Data.merchantId, transactionRequest);
            Result<ProcessSaleTransactionResponse> saleResponse = await mediator.Send(saleCommand, cancellationToken);
            return ResponseFactory.FromResult(saleResponse, modelFactory.ConvertFrom);
        }

        public static async Task<IResult> PerformLogonTransaction(IMediator mediator,
                                                                      IModelFactory modelFactory,
                                                                      ClaimsPrincipal user,
                                                                      LogonTransactionRequestMessage transactionRequest,
                                                                      CancellationToken cancellationToken)
        {
            Result<(Guid estateId, Guid merchantId)> claimsResult = Helpers.GetRequiredClaims(user);
            if (claimsResult.IsFailed)
                return ResponseFactory.FromResult(Result.Forbidden());

            TransactionCommands.ProcessLogonTransactionCommand logonCommand = CreateLogonCommand(claimsResult.Data.estateId, claimsResult.Data.merchantId, transactionRequest);
            Result<ProcessLogonTransactionResponse> logonResponse = await mediator.Send(logonCommand, cancellationToken);
            return ResponseFactory.FromResult(logonResponse, modelFactory.ConvertFrom);
        }

        public static async Task<IResult> PerformReconciliationTransaction(IMediator mediator,
                                                                       IModelFactory modelFactory,
                                                                       ClaimsPrincipal user,
                                                                       ReconciliationRequestMessage transactionRequest,
                                                                       CancellationToken cancellationToken)
        {
            Result<(Guid estateId, Guid merchantId)> claimsResult = Helpers.GetRequiredClaims(user);
            if (claimsResult.IsFailed)
                return ResponseFactory.FromResult(Result.Forbidden());

            TransactionCommands.ProcessReconciliationCommand reconCommand = CreateReconciliationCommand(claimsResult.Data.estateId, claimsResult.Data.merchantId, transactionRequest);
            Result<ProcessReconciliationResponse> reconResponse = await mediator.Send(reconCommand, cancellationToken);
            return ResponseFactory.FromResult(reconResponse, modelFactory.ConvertFrom);
        
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

    public class Helpers {
        public static Result<(Guid estateId, Guid merchantId)> GetRequiredClaims(ClaimsPrincipal user)
        {
            Result<Claim> estateIdResult = ClaimsHelper.GetUserClaim(user, "estateId");
            if (estateIdResult.IsFailed)
                return Result.Failure("No Claim found for Estate Id");

            Result<Claim> merchantIdResult = ClaimsHelper.GetUserClaim(user, "merchantId");
            if (merchantIdResult.IsFailed)
                return Result.Failure("No Claim found for Merchant Id");

            // Ok we have the required claims
            Guid estateId = Guid.Parse(estateIdResult.Data.Value);
            Guid merchantId = Guid.Parse(merchantIdResult.Data.Value);
            return Result.Success((estateId, merchantId));
        }
    }
}