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
                                                                  ClaimsPrincipal user,
                                                                  SaleTransactionRequestMessage transactionRequest,
                                                                  CancellationToken cancellationToken)
        {
            Result<(Guid estateId, Guid merchantId)> claimsResult = Helpers.GetRequiredClaims(user);
            if (claimsResult.IsFailed)
                return ResponseFactory.FromResult(Result.Forbidden());

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
            if (claimsResult.IsFailed)
                return ResponseFactory.FromResult(Result.Forbidden());

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
            if (claimsResult.IsFailed)
                return ResponseFactory.FromResult(Result.Forbidden());

            TransactionCommands.ProcessReconciliationCommand reconCommand = CreateReconciliationCommand(claimsResult.Data.estateId, claimsResult.Data.merchantId, transactionRequest);
            Result<ProcessReconciliationResponse> reconResponse = await mediator.Send(reconCommand, cancellationToken);
            return ResponseFactory.FromResult(reconResponse, ModelFactory.ConvertFrom);
        
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

    public static class Helpers {
        public static Result<(Guid estateId, Guid merchantId)> GetRequiredClaims(ClaimsPrincipal user)
        {
            Result<Claim> estateIdResult = ClaimsHelper2.GetUserClaim(user, "estateId");
            if (estateIdResult.IsFailed)
                return Result.Failure("No Claim found for Estate Id");

            Result<Claim> merchantIdResult = ClaimsHelper2.GetUserClaim(user, "merchantId");
            if (merchantIdResult.IsFailed)
                return Result.Failure("No Claim found for Merchant Id");

            // Ok we have the required claims
            Guid estateId = Guid.Parse(estateIdResult.Data.Value);
            Guid merchantId = Guid.Parse(merchantIdResult.Data.Value);
            return Result.Success((estateId, merchantId));
        }
    }
}

public static class ClaimsHelper2
{
    #region Methods

    public static Result<Claim> GetUserClaim(ClaimsPrincipal user,
                                             String customClaimType) =>
        GetUserClaim(user, customClaimType, String.Empty);

    /// <summary>
    /// Gets the user claims.
    /// </summary>
    /// <param name="user">The user.</param>
    /// <param name="customClaimType">Type of the custom claim.</param>
    /// <param name="defaultValue">The default value.</param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException">No claim [{customClaimType}] found for user id [{userIdClaim.Value}</exception>
    public static Result<Claim> GetUserClaim(ClaimsPrincipal user,
                                             String customClaimType,
                                             String defaultValue)
    {
        Claim userClaim = null;

        if (ClaimsHelper2.IsPasswordToken(user))
        {
            // Get the claim from the token
            userClaim = user.Claims.SingleOrDefault(c => c.Type.Equals(customClaimType, StringComparison.CurrentCultureIgnoreCase));

            if (userClaim == null)
            {
                return Result.NotFound($"Claim type [{customClaimType}] not found");
            }
        }
        else
        {
            userClaim = new Claim(customClaimType, defaultValue);
        }

        return Result.Success(userClaim);
    }

    /// <summary>
    /// Determines whether [is client token] [the specified user].
    /// </summary>
    /// <param name="user">The user.</param>
    /// <returns>
    ///   <c>true</c> if [is client token] [the specified user]; otherwise, <c>false</c>.
    /// </returns>
    public static Boolean IsPasswordToken(ClaimsPrincipal user)
    {
        Boolean result = false;

        Claim userIdClaim = user.Claims.SingleOrDefault(c => c.Type == "name");

        if (userIdClaim != null)
        {
            result = true;
        }

        return result;
    }

    /// <summary>
    /// Determines whether [is user roles valid] [the specified user].
    /// </summary>
    /// <param name="user">The user.</param>
    /// <param name="allowedRoles">The allowed roles.</param>
    /// <returns>
    ///   <c>true</c> if [is user roles valid] [the specified user]; otherwise, <c>false</c>.
    /// </returns>
    public static Boolean IsUserRolesValid(ClaimsPrincipal user,
                                           String[] allowedRoles)
    {
        if (!ClaimsHelper.IsPasswordToken(user))
        {
            return true;
        }

        return allowedRoles.Any(r => user.IsInRole(r));
    }

    /// <summary>
    /// Validates the route parameter.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="routeParameter">The route parameter.</param>
    /// <param name="userClaim">The user claim.</param>
    public static Boolean ValidateRouteParameter<T>(T routeParameter,
                                                    Claim userClaim)
    {
        if (userClaim != null && userClaim.Value != String.Empty)
        {
            if (routeParameter.ToString() != userClaim.Value)
            {
                return false;
            }
        }

        return true;
    }

    #endregion
}