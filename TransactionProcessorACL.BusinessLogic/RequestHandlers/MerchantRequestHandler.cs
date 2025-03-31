using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SimpleResults;
using TransactionProcessorACL.BusinessLogic.Requests;
using TransactionProcessorACL.BusinessLogic.Services;
using TransactionProcessorACL.Models;

namespace TransactionProcessorACL.BusinessLogic.RequestHandlers;

public class MerchantRequestHandler : IRequestHandler<MerchantQueries.GetMerchantContractsQuery, Result<List<ContractResponse>>>,
IRequestHandler<MerchantQueries.GetMerchantQuery, Result<MerchantResponse>> {
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

    public async Task<Result<MerchantResponse>> Handle(MerchantQueries.GetMerchantQuery request,
                                                       CancellationToken cancellationToken) {
        return await this.ApplicationService.GetMerchant(request.EstateId, request.MerchantId, cancellationToken);
    }
}