using MediatR;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.Filters;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using System.Threading;
using System;
using JasperFx.Core;
using Shared.Results;
using Shared.Results.Web;
using SimpleResults;
using TransactionProcessorACL.BusinessLogic.Common;
using TransactionProcessorACL.BusinessLogic.Requests;
using TransactionProcessorACL.Factories;

namespace TransactionProcessorACL.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Models;
    using Shared.General;
    using Shared.Logger;
    using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
    using static TransactionProcessorACL.BusinessLogic.Requests.VersionCheckCommands;

    [ExcludeFromCodeCoverage]
    [Route(VoucherController.ControllerRoute)]
    [ApiController]
    [Authorize]
    public class VoucherController : ControllerBase
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
        /// Initializes a new instance of the <see cref="VoucherController"/> class.
        /// </summary>
        /// <param name="mediator">The mediator.</param>
        /// <param name="modelFactory">The model factory.</param>
        public VoucherController(IMediator mediator,
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
        /// <param name="voucherCode">The voucher code.</param>
        /// <param name="applicationVersion">The application version.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        [HttpGet]
        [Route("")]
        //[SwaggerResponse(200, type: typeof(GetVoucherResponseMessage))]
        //[SwaggerResponseExample(200, typeof(GetVoucherResponseMessageExample))]
        public async Task<IActionResult> GetVoucher([FromQuery] String voucherCode,
                                                    [FromQuery] String applicationVersion,
                                                    CancellationToken cancellationToken)
        {
            if (ClaimsHelper.IsPasswordToken(this.User) == false)
            {
                return this.Forbid();
            }

            // Do the software version check
            VersionCheckCommand versionCheckCommand = new(applicationVersion);
            var versionCheckResult = await this.Mediator.Send(versionCheckCommand, cancellationToken);
            if (versionCheckResult.IsFailed)
                return this.StatusCode(505);

            Guid estateId = Guid.Parse(ClaimsHelper.GetUserClaim(this.User, "estateId").Data.Value);
            Guid contractId = Guid.Parse(ClaimsHelper.GetUserClaim(this.User, "contractId").Data.Value);

            // Now do the GET
            VoucherQueries.GetVoucherQuery query = new(estateId, contractId, voucherCode);

            var result = await this.Mediator.Send(query, cancellationToken);
            if (result.IsFailed)
                return ResultHelpers.CreateFailure(result).ToActionResultX();

            return Result.Success(this.ModelFactory.ConvertFrom(result.Data)).ToActionResultX();
        }

        /// <summary>
        /// Redeems the voucher.
        /// </summary>
        /// <param name="voucherCode">The voucher code.</param>
        /// <param name="applicationVersion">The application version.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        [HttpPut]
        [Route("")]
        //[SwaggerResponse(200, type: typeof(RedeemVoucherResponseMessage))]
        //[SwaggerResponseExample(200, typeof(RedeemVoucherResponseMessageExample))]
        public async Task<IActionResult> RedeemVoucher([FromQuery] String voucherCode,
                                                    [FromQuery] String applicationVersion,
                                                    CancellationToken cancellationToken)
        {
            if (ClaimsHelper.IsPasswordToken(this.User) == false)
            {
                return this.Forbid();
            }

            // Do the software version check
            VersionCheckCommand versionCheckCommand = new(applicationVersion);
            var versionCheckResult = await this.Mediator.Send(versionCheckCommand, cancellationToken);
            if (versionCheckResult.IsFailed)
                return this.StatusCode(505);

            Guid estateId = Guid.Parse(ClaimsHelper.GetUserClaim(this.User, "estateId").Data.Value);
            Guid contractId = Guid.Parse(ClaimsHelper.GetUserClaim(this.User, "contractId").Data.Value);

            // Now do the GET
            VoucherCommands.RedeemVoucherCommand comamnd = new(estateId, contractId, voucherCode);

            var result = await this.Mediator.Send(comamnd, cancellationToken);
            if (result.IsFailed)
                return ResultHelpers.CreateFailure(result).ToActionResultX();

            return Result.Success(this.ModelFactory.ConvertFrom(result.Data)).ToActionResultX();
        }


        #endregion

        #region Others

        /// <summary>
        /// The controller name
        /// </summary>
        private const String ControllerName = "vouchers";

        /// <summary>
        /// The controller route
        /// </summary>
        private const String ControllerRoute = "api/" + VoucherController.ControllerName;

        #endregion
    }
}
