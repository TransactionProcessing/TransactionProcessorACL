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
    /// <seealso cref="MediatR.IRequestHandler{TransactionProcessorACL.BusinessLogic.Requests.ProcessLogonTransactionRequest, TransactionProcessorACL.Models.ProcessLogonTransactionResponse}" />
    /// <seealso cref="ProcessLogonTransactionResponse" />
    public class ProcessLogonTransactionRequestHandler : IRequestHandler<ProcessLogonTransactionRequest, ProcessLogonTransactionResponse>
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
        public ProcessLogonTransactionRequestHandler(ITransactionProcessorACLApplicationService applicationService)
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