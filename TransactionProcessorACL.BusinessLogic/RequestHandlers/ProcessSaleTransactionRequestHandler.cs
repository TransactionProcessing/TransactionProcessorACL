namespace TransactionProcessorACL.BusinessLogic.RequestHandlers
{
    using System.Threading;
    using System.Threading.Tasks;
    using MediatR;
    using Models;
    using Requests;
    using Services;

    public class ProcessSaleTransactionRequestHandler : IRequestHandler<ProcessSaleTransactionRequest, ProcessSaleTransactionResponse>
    {
        #region Fields

        /// <summary>
        /// The application service
        /// </summary>
        private readonly ITransactionProcessorACLApplicationService ApplicationService;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ProcessSaleTransactionRequestHandler"/> class.
        /// </summary>
        /// <param name="applicationService">The application service.</param>
        public ProcessSaleTransactionRequestHandler(ITransactionProcessorACLApplicationService applicationService)
        {
            this.ApplicationService = applicationService;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Handles the specified request.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// Response from the request
        /// </returns>
        public async Task<ProcessSaleTransactionResponse> Handle(ProcessSaleTransactionRequest request,
                                                                 CancellationToken cancellationToken)
        {
            return await this.ApplicationService.ProcessSaleTransaction(request.EstateId,
                                                                        request.MerchantId,
                                                                        request.TransactionDateTime,
                                                                        request.TransactionNumber,
                                                                        request.DeviceIdentifier,
                                                                        request.OperatorId,
                                                                        request.CustomerEmailAddress,
                                                                        request.ContractId,
                                                                        request.ProductId,
                                                                        request.AdditionalRequestMetadata,
                                                                        cancellationToken);
        }

        #endregion
    }
}