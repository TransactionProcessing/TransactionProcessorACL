namespace TransactionProcessorACL.BusinessLogic.RequestHandlers
{
    using System.Threading;
    using System.Threading.Tasks;
    using MediatR;
    using Models;
    using Requests;
    using Services;

    public class ProcessReconciliationRequestHandler : IRequestHandler<ProcessReconciliationRequest, ProcessReconciliationResponse>
    {
        #region Fields

        /// <summary>
        /// The application service
        /// </summary>
        private readonly ITransactionProcessorACLApplicationService ApplicationService;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ProcessLogonTransactionRequestHandler"/> class.
        /// </summary>
        /// <param name="applicationService">The application service.</param>
        public ProcessReconciliationRequestHandler(ITransactionProcessorACLApplicationService applicationService)
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
        public async Task<ProcessReconciliationResponse> Handle(ProcessReconciliationRequest request,
                                                                CancellationToken cancellationToken)
        {
            return await this.ApplicationService.ProcessReconciliation(request.EstateId,
                                                                       request.MerchantId,
                                                                       request.TransactionDateTime,
                                                                       request.DeviceIdentifier,
                                                                       request.TransactionCount,
                                                                       request.TransactionValue,
                                                                       cancellationToken);
        }

        #endregion
    }
}