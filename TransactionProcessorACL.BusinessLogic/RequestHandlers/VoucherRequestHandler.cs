using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SimpleResults;
using TransactionProcessorACL.BusinessLogic.Requests;

namespace TransactionProcessorACL.BusinessLogic.RequestHandlers
{
    using Models;
    using Services;

    public class VoucherRequestHandler : IRequestHandler<VoucherQueries.GetVoucherQuery, Result<GetVoucherResponse>>, 
                                         IRequestHandler<VoucherCommands.RedeemVoucherCommand, Result<RedeemVoucherResponse>>
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

        public async Task<Result<GetVoucherResponse>> Handle(VoucherQueries.GetVoucherQuery query,
                                                             CancellationToken cancellationToken)
        {
            return await this.ApplicationService.GetVoucher(query.EstateId, query.ContractId, query.VoucherCode, cancellationToken);
        }

        public async Task<Result<RedeemVoucherResponse>> Handle(VoucherCommands.RedeemVoucherCommand command,
                                                                CancellationToken cancellationToken){
            return await this.ApplicationService.RedeemVoucher(command.EstateId, command.ContractId, command.VoucherCode, cancellationToken);
        }

        #endregion
    }

    public class MerchantRequestHandler : IRequestHandler<MerchantQueries.GetMerchantContractsQuery, Result<List<ContractResponse>>> {
        #region Fields

        private readonly ITransactionProcessorACLApplicationService ApplicationService;

        #endregion

        public MerchantRequestHandler(ITransactionProcessorACLApplicationService applicationService) {
            this.ApplicationService = applicationService;
        }

        public async Task<Result<List<ContractResponse>>> Handle(MerchantQueries.GetMerchantContractsQuery request,
                                         CancellationToken cancellationToken) {
            return await this.ApplicationService.GetMerchantContracts(request.EstateId, request.MerchantId, cancellationToken);
        }
    }
}
