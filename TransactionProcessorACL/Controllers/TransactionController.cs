using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TransactionProcessorACL.Controllers
{
    using System.Diagnostics.CodeAnalysis;
    using System.Threading;
    using BusinessLogic.Requests;
    using Common;
    using DataTransferObjects;
    using Factories;
    using MediatR;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Newtonsoft.Json;

    [ExcludeFromCodeCoverage]
    [Route(TransactionController.ControllerRoute)]
    [ApiController]
    [ApiVersion("1.0")]
    [Authorize]
    public class TransactionController : ControllerBase
    {
        private readonly IMediator Mediator;

        private readonly IModelFactory ModelFactory;

        public TransactionController(IMediator mediator, IModelFactory modelFactory)
        {
            this.Mediator = mediator;
            this.ModelFactory = modelFactory;
        }

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

        private ProcessLogonTransactionRequest CreateCommandFromRequest(LogonTransactionRequestMessage logonTransactionRequestMessage)
        {
            Guid estateId = Guid.Parse(ClaimsHelper.GetUserClaim(this.User, "EstateId").Value);
            Guid merchantId = Guid.Parse(ClaimsHelper.GetUserClaim(this.User, "MerchantId").Value);

            ProcessLogonTransactionRequest request = ProcessLogonTransactionRequest.Create(estateId,
                                                                                           merchantId,
                                                                                           logonTransactionRequestMessage.TransactionDateTime,
                                                                                           logonTransactionRequestMessage.TransactionNumber,
                                                                                           logonTransactionRequestMessage.IMEINumber,
                                                                                           logonTransactionRequestMessage.RequireConfigurationInResponse);

            return request;
        }


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
