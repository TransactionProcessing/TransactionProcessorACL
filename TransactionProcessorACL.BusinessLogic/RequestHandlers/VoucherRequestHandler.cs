using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TransactionProcessorACL.BusinessLogic.Requests;

namespace TransactionProcessorACL.BusinessLogic.RequestHandlers
{
    using Models;
    using Services;
    using RedeemVoucherRequest = Requests.RedeemVoucherRequest;

    public class VoucherRequestHandler : IRequestHandler<GetVoucherRequest, GetVoucherResponse>, IRequestHandler<RedeemVoucherRequest, RedeemVoucherResponse>
    {
        #region Fields
        
        private readonly ITransactionProcessorACLApplicationService ApplicationService;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="VoucherRequestHandler" /> class.
        /// </summary>
        /// <param name="applicationService">The application service.</param>
        public VoucherRequestHandler(ITransactionProcessorACLApplicationService applicationService)
        {
            this.ApplicationService = applicationService;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Handles a request
        /// </summary>
        /// <param name="request">The request</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>
        /// Response from the request
        /// </returns>
        public async Task<GetVoucherResponse> Handle(GetVoucherRequest request,
                                                     CancellationToken cancellationToken)
        {
            return await this.ApplicationService.GetVoucher(request.EstateId, request.ContractId, request.VoucherCode, cancellationToken);
        }

        /// <summary>
        /// Handles a request
        /// </summary>
        /// <param name="request">The request</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>
        /// Response from the request
        /// </returns>
        public async Task<RedeemVoucherResponse> Handle(RedeemVoucherRequest request,
                                                        CancellationToken cancellationToken){
            return await this.ApplicationService.RedeemVoucher(request.EstateId, request.ContractId, request.VoucherCode, cancellationToken);
        }

        #endregion
    }
}
