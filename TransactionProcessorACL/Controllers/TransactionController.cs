namespace TransactionProcessorACL.Controllers
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Threading;
    using System.Threading.Tasks;
    using BusinessLogic.Requests;
    using Common;
    using DataTransferObjects;
    using Factories;
    using MediatR;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="Microsoft.AspNetCore.Mvc.ControllerBase" />
    [ExcludeFromCodeCoverage]
    [Route(TransactionController.ControllerRoute)]
    [ApiController]
    [ApiVersion("1.0")]
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
        public async Task<IActionResult> PerformTransaction([FromBody] TransactionRequestMessage transactionRequest,
                                                            CancellationToken cancellationToken)
        {
            if (ClaimsHelper.IsPasswordToken(this.User) == false)
            {
                return this.Forbid();
            }

            dynamic request = this.CreateCommandFromRequest((dynamic)transactionRequest);
            dynamic response = await this.Mediator.Send(request, cancellationToken);

            return this.Ok(this.ModelFactory.ConvertFrom(response));
            // TODO: Populate the GET route
            //return this.Created("", transactionResponse);
        }

        /// <summary>
        /// Creates the command from request.
        /// </summary>
        /// <param name="logonTransactionRequestMessage">The logon transaction request message.</param>
        /// <returns></returns>
        private ProcessLogonTransactionRequest CreateCommandFromRequest(LogonTransactionRequestMessage logonTransactionRequestMessage)
        {
            Guid estateId = Guid.Parse(ClaimsHelper.GetUserClaim(this.User, "EstateId").Value);
            Guid merchantId = Guid.Parse(ClaimsHelper.GetUserClaim(this.User, "MerchantId").Value);

            ProcessLogonTransactionRequest request = ProcessLogonTransactionRequest.Create(estateId,
                                                                                           merchantId,
                                                                                           logonTransactionRequestMessage.TransactionDateTime,
                                                                                           logonTransactionRequestMessage.TransactionNumber,
                                                                                           logonTransactionRequestMessage.DeviceIdentifier,
                                                                                           logonTransactionRequestMessage.RequireConfigurationInResponse);

            return request;
        }

        /// <summary>
        /// Creates the command from request.
        /// </summary>
        /// <param name="saleTransactionRequestMessage">The sale transaction request message.</param>
        /// <returns></returns>
        private ProcessSaleTransactionRequest CreateCommandFromRequest(SaleTransactionRequestMessage saleTransactionRequestMessage)
        {
            Guid estateId = Guid.Parse(ClaimsHelper.GetUserClaim(this.User, "EstateId").Value);
            Guid merchantId = Guid.Parse(ClaimsHelper.GetUserClaim(this.User, "MerchantId").Value);

            ProcessSaleTransactionRequest request = ProcessSaleTransactionRequest.Create(estateId,
                                                                                         merchantId,
                                                                                         saleTransactionRequestMessage.TransactionDateTime,
                                                                                         saleTransactionRequestMessage.TransactionNumber,
                                                                                         saleTransactionRequestMessage.DeviceIdentifier,
                                                                                         saleTransactionRequestMessage.OperatorIdentifier,
                                                                                         saleTransactionRequestMessage.CustomerEmailAddress,
                                                                                         saleTransactionRequestMessage.ContractId,
                                                                                         saleTransactionRequestMessage.ProductId,
                                                                                         saleTransactionRequestMessage.AdditionalRequestMetaData);

            return request;
        }

        /// <summary>
        /// Creates the command from request.
        /// </summary>
        /// <param name="reconciliationRequestMessage">The reconciliation request message.</param>
        /// <returns></returns>
        private ProcessReconciliationRequest CreateCommandFromRequest(ReconciliationRequestMessage reconciliationRequestMessage)
        {
            Guid estateId = Guid.Parse(ClaimsHelper.GetUserClaim(this.User, "EstateId").Value);
            Guid merchantId = Guid.Parse(ClaimsHelper.GetUserClaim(this.User, "MerchantId").Value);

            ProcessReconciliationRequest request = ProcessReconciliationRequest.Create(estateId,
                                                                                       merchantId,
                                                                                       reconciliationRequestMessage.TransactionDateTime,
                                                                                       reconciliationRequestMessage.DeviceIdentifier,
                                                                                       reconciliationRequestMessage.TransactionCount,
                                                                                       reconciliationRequestMessage.TransactionValue);

            return request;
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