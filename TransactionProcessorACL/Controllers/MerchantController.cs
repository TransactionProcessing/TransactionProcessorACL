using MediatR;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Shared.General;
using System;
using System.Threading;
using System.Threading.Tasks;
using SimpleResults;
using TransactionProcessorACL.Factories;
using static TransactionProcessorACL.BusinessLogic.Requests.VersionCheckCommands;

namespace TransactionProcessorACL.Controllers
{
    public class MerchantController : ControllerBase
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
        
        public MerchantController(IMediator mediator,
                                  IModelFactory modelFactory)
        {
            this.Mediator = mediator;
            this.ModelFactory = modelFactory;
        }

        [HttpGet]
        [Route("")]
        public async Task<IActionResult> GetMerchantContracts([FromQuery] String applicationVersion,
                                                              [FromQuery] String deviceIdentifier,
                                                              CancellationToken cancellationToken) {
            if (ClaimsHelper.IsPasswordToken(this.User) == false) {
                return this.Forbid();
            }

            // Do the software version check
            VersionCheckCommand versionCheckCommand = new(applicationVersion);
            Result versionCheckResult = await this.Mediator.Send(versionCheckCommand, cancellationToken);
            if (versionCheckResult.IsFailed)
                return this.StatusCode(505);

            //Guid estateId = Guid.Parse(ClaimsHelper.GetUserClaim(this.User, "estateId").Value);
            //Guid merchantId = Guid.Parse(ClaimsHelper.GetUserClaim(this.User, "merchantId").Value);

            return this.Ok();
        }

        #region Others

        /// <summary>
        /// The controller name
        /// </summary>
        public const String ControllerName = "merchants";

        /// <summary>
        /// The controller route
        /// </summary>
        private const String ControllerRoute = "api/" + MerchantController.ControllerName;

        #endregion
    }
}
