namespace TransactionProcessorACL.BusinessLogic.RequestHandlers
{
    using System.Threading;
    using System.Threading.Tasks;
    using MediatR;
    using Models;
    using Requests;
    using Services;

    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="ProcessLogonTransactionResponse" />
    public class ProcessLogonTransactionRequestHandler : IRequestHandler<ProcessLogonTransactionRequest, ProcessLogonTransactionResponse>
    {
        private readonly ITransactionProcessorACLApplicationService ApplicationService;

        public ProcessLogonTransactionRequestHandler(ITransactionProcessorACLApplicationService applicationService)
        {
            this.ApplicationService = applicationService;
        }

        #region Methods

        /// <summary>
        /// Handles the specified request.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        public async Task<ProcessLogonTransactionResponse> Handle(ProcessLogonTransactionRequest request,
                                                                  CancellationToken cancellationToken)
        {
            return await this.ApplicationService.ProcessLogonTransaction(request.EstateId,
                                                                         request.MerchantId,
                                                                         request.TransactionDateTime,
                                                                         request.TransactionNumber,
                                                                         request.DeviceIdentifier,
                                                                         cancellationToken);
        }

        #endregion
    }
}