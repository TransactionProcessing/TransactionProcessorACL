namespace TransactionProcessorACL.BusinessLogic.RequestHandlers
{
    using System.Threading;
    using System.Threading.Tasks;
    using MediatR;
    using Models;
    using Requests;

    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="MediatR.IRequestHandler{TransactionProcessorACL.BusinessLogic.Requests.ProcessLogonTransactionRequest, TransactionProcessorACL.Models.ProcessLogonTransactionResponse}" />
    public class ProcessLogonTransactionRequestHandler : IRequestHandler<ProcessLogonTransactionRequest, ProcessLogonTransactionResponse>
    {
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
            return new ProcessLogonTransactionResponse
                   {
                       ResponseCode = "0000",
                       ResponseMessage = "SUCCESS"
                   };
        }

        #endregion
    }
}