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

            // Now do the transaction
            TransactionResponseMessage dto = null;
            switch (transactionRequest){
                case SaleTransactionRequestMessage msg:
                    TransactionCommands.ProcessSaleTransactionCommand saleCommand = this.CreateCommandFromRequest(msg);
                    Result<ProcessSaleTransactionResponse> saleResponse = await this.Mediator.Send(saleCommand, cancellationToken);
                    // TODO: Handle the result
                    if (saleResponse.IsFailed)
                        return saleResponse.ToActionResultX();
                    dto = this.ModelFactory.ConvertFrom(saleResponse);
                    break;
                case LogonTransactionRequestMessage msg:
                    TransactionCommands.ProcessLogonTransactionCommand logonCommand= this.CreateCommandFromRequest(msg);
                    Result<ProcessLogonTransactionResponse> logonResponse = await this.Mediator.Send(logonCommand, cancellationToken);
                    if (logonResponse.IsFailed)
                        return logonResponse.ToActionResultX();
                    dto = this.ModelFactory.ConvertFrom(logonResponse);
                    break;
                case ReconciliationRequestMessage msg:
                    TransactionCommands.ProcessReconciliationCommand reconciliationCommand = this.CreateCommandFromRequest(msg);
                    Result<ProcessReconciliationResponse> reconciliationResponse = await this.Mediator.Send(reconciliationCommand, cancellationToken);
                    if (reconciliationResponse.IsFailed)
                        return reconciliationResponse.ToActionResultX();
                    dto = this.ModelFactory.ConvertFrom(reconciliationResponse);
                    break;
            }


            return Result.Success(dto).ToActionResultX();
        }

        /// <summary>
        /// Creates the command from request.
        /// </summary>
        /// <param name="logonTransactionRequestMessage">The logon transaction request message.</param>
        /// <returns></returns>
        private TransactionCommands.ProcessLogonTransactionCommand  CreateCommandFromRequest(LogonTransactionRequestMessage logonTransactionRequestMessage)
        {
            Guid estateId = Guid.Parse(ClaimsHelper.GetUserClaim(this.User, "estateId").Value);
            Guid merchantId = Guid.Parse(ClaimsHelper.GetUserClaim(this.User, "merchantId").Value);

            TransactionCommands.ProcessLogonTransactionCommand command = new(estateId,
                                                                                           merchantId,
                                                                                           logonTransactionRequestMessage.TransactionDateTime,
                                                                                           logonTransactionRequestMessage.TransactionNumber,
                                                                                           logonTransactionRequestMessage.DeviceIdentifier);

            return command;
        }

        /// <summary>
        /// Creates the command from request.
        /// </summary>
        /// <param name="saleTransactionRequestMessage">The sale transaction request message.</param>
        /// <returns></returns>
        private TransactionCommands.ProcessSaleTransactionCommand CreateCommandFromRequest(SaleTransactionRequestMessage saleTransactionRequestMessage)
        {
            Guid estateId = Guid.Parse(ClaimsHelper.GetUserClaim(this.User, "estateId").Value);
            Guid merchantId = Guid.Parse(ClaimsHelper.GetUserClaim(this.User, "merchantId").Value);

            TransactionCommands.ProcessSaleTransactionCommand command = new(estateId,
                                                                                         merchantId,
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
        private TransactionCommands.ProcessReconciliationCommand CreateCommandFromRequest(ReconciliationRequestMessage reconciliationRequestMessage)
        {
            Guid estateId = Guid.Parse(ClaimsHelper.GetUserClaim(this.User, "estateId").Value);
            Guid merchantId = Guid.Parse(ClaimsHelper.GetUserClaim(this.User, "merchantId").Value);

            TransactionCommands.ProcessReconciliationCommand command = new(estateId,
                                                                                       merchantId,
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