using System.Security.Claims;
using Shared.General;
using Shared.Results;
using Shared.Results.Web;
using SimpleResults;

namespace TransactionProcessorACL.Controllers
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Threading;
    using System.Threading.Tasks;
    using BusinessLogic.Common;
    using BusinessLogic.RequestHandlers;
    using BusinessLogic.Requests;
    using Common;
    using Common.Examples;
    using DataTransferObjects;
    using DataTransferObjects.Responses;
    using Factories;
    using MediatR;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Models;
    using Shared.Logger;
    using Swashbuckle.AspNetCore.Annotations;
    using Swashbuckle.AspNetCore.Filters;
    using static TransactionProcessorACL.BusinessLogic.Requests.VersionCheckCommands;

    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="Microsoft.AspNetCore.Mvc.ControllerBase" />
    [ExcludeFromCodeCoverage]
    [Route(TransactionController.ControllerRoute)]
    [ApiController]
    [Authorize]
    public class TransactionController : ControllerBase
    {
        #region Fields

        /// <summary>
        /// The mediator
        /// </summary>
        private readonly IMediator Mediator;

        /// <summary>
        /// The model factory
        /// </summary>
        private readonly IModelFactory ModelFactory;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="TransactionController"/> class.
        /// </summary>
        /// <param name="mediator">The mediator.</param>
        /// <param name="modelFactory">The model factory.</param>
        public TransactionController(IMediator mediator,
                                     IModelFactory modelFactory)
        {
            this.Mediator = mediator;
            this.ModelFactory = modelFactory;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Performs the transaction.
        /// </summary>
        /// <param name="transactionRequest">The transaction request.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        [HttpPost]
        [Route("")]
        [SwaggerRequestExample(typeof(TransactionRequestMessage), typeof(TransactionRequestMessageExample))]
        [SwaggerResponse(200,"OK", typeof(TransactionResponseMessage))]
        [SwaggerResponseExample(200, typeof(TransactionResponseMessageExample))]
        
        public async Task<IActionResult> PerformTransaction([FromBody] TransactionRequestMessage transactionRequest,
                                                            CancellationToken cancellationToken)
        {
            if (ClaimsHelper.IsPasswordToken(this.User) == false)
            {
                return this.Forbid();
            }

            // Do the software version check
            VersionCheckCommand versionCheckCommand = new(transactionRequest.ApplicationVersion);
            var versionCheckResult = await this.Mediator.Send(versionCheckCommand, cancellationToken);
            if (versionCheckResult.IsFailed)
                return this.StatusCode(505);

            Result<(Guid estateId, Guid merchantId)> claimsResult = TransactionController.GetRequiredClaims(this.User);
            if (claimsResult.IsFailed)
                return this.Forbid();

            TransactionResponseMessage dto = new TransactionResponseMessage();
            switch (transactionRequest)
            {
                case SaleTransactionRequestMessage msg:
                    TransactionCommands.ProcessSaleTransactionCommand saleCommand = this.CreateCommandFromRequest(claimsResult.Data.estateId, claimsResult.Data.merchantId, msg);
                    Result<ProcessSaleTransactionResponse> saleResponse = await this.Mediator.Send(saleCommand, cancellationToken);
                    // TODO: Handle the result
                    if (saleResponse.IsFailed)
                        return saleResponse.ToActionResultX();
                    dto = this.ModelFactory.ConvertFrom(saleResponse.Data);
                    break;
                case LogonTransactionRequestMessage msg:
                    TransactionCommands.ProcessLogonTransactionCommand logonCommand= this.CreateCommandFromRequest(claimsResult.Data.estateId, claimsResult.Data.merchantId, msg);
                    Result<ProcessLogonTransactionResponse> logonResponse = await this.Mediator.Send(logonCommand, cancellationToken);
                    if (logonResponse.IsFailed)
                        return logonResponse.ToActionResultX();
                    dto = this.ModelFactory.ConvertFrom(logonResponse.Data);
                    break;
                case ReconciliationRequestMessage msg: 
                    TransactionCommands.ProcessReconciliationCommand reconciliationCommand = this.CreateCommandFromRequest(claimsResult.Data.estateId, claimsResult.Data.merchantId, msg);
                    Result<ProcessReconciliationResponse> reconciliationResponse = await this.Mediator.Send(reconciliationCommand, cancellationToken);
                    if (reconciliationResponse.IsFailed)
                        return reconciliationResponse.ToActionResultX();
                    dto = this.ModelFactory.ConvertFrom(reconciliationResponse.Data);
                    break;
            }


            return Result.Success(dto).ToActionResultX();
        }

        /// <summary>
        /// Creates the command from request.
        /// </summary>
        /// <param name="logonTransactionRequestMessage">The logon transaction request message.</param>
        /// <returns></returns>
        private TransactionCommands.ProcessLogonTransactionCommand  CreateCommandFromRequest(Guid estateId, Guid merchantId, LogonTransactionRequestMessage logonTransactionRequestMessage)
        {
            TransactionCommands.ProcessLogonTransactionCommand command = new(estateId, merchantId, logonTransactionRequestMessage.TransactionDateTime, logonTransactionRequestMessage.TransactionNumber, logonTransactionRequestMessage.DeviceIdentifier);

            return command;
        }

        public static Result<(Guid estateId, Guid merchantId)> GetRequiredClaims(ClaimsPrincipal user) {
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

        /// <summary>
        /// Creates the command from request.
        /// </summary>
        /// <param name="saleTransactionRequestMessage">The sale transaction request message.</param>
        /// <returns></returns>
        private TransactionCommands.ProcessSaleTransactionCommand CreateCommandFromRequest(Guid estateId, Guid merchantId, SaleTransactionRequestMessage saleTransactionRequestMessage)
        {

            TransactionCommands.ProcessSaleTransactionCommand command = new(estateId, merchantId,
                                                                                         saleTransactionRequestMessage.TransactionDateTime,
                                                                                         saleTransactionRequestMessage.TransactionNumber,
                                                                                         saleTransactionRequestMessage.DeviceIdentifier,
                                                                                         saleTransactionRequestMessage.OperatorId,
                                                                                         saleTransactionRequestMessage.CustomerEmailAddress,
                                                                                         saleTransactionRequestMessage.ContractId,
                                                                                         saleTransactionRequestMessage.ProductId,
                                                                                         saleTransactionRequestMessage.AdditionalRequestMetaData);

            return command;
        }

        /// <summary>
        /// Creates the command from request.
        /// </summary>
        /// <param name="reconciliationRequestMessage">The reconciliation request message.</param>
        /// <returns></returns>
        private TransactionCommands.ProcessReconciliationCommand CreateCommandFromRequest(Guid estateId, Guid merchantId, ReconciliationRequestMessage reconciliationRequestMessage)
        {
            TransactionCommands.ProcessReconciliationCommand command = new(estateId, merchantId,
                                                                                       reconciliationRequestMessage.TransactionDateTime,
                                                                                       reconciliationRequestMessage.DeviceIdentifier,
                                                                                       reconciliationRequestMessage.TransactionCount,
                                                                                       reconciliationRequestMessage.TransactionValue);

            return command;
        }

        #endregion

        #region Others

        /// <summary>
        /// The controller name
        /// </summary>
        public const String ControllerName = "transactions";

        /// <summary>
        /// The controller route
        /// </summary>
        private const String ControllerRoute = "api/" + TransactionController.ControllerName;

        #endregion
    }
}