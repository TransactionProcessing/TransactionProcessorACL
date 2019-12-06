namespace TransactionProcessorACL.BusinessLogic.RequestHandlers
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using MediatR;
    using Models;
    using Requests;

    public class ProcessLogonTransactionRequestHandler : IRequestHandler<ProcessLogonTransactionRequest, ProcessLogonTransactionResponse>
    {
        public async Task<ProcessLogonTransactionResponse> Handle(ProcessLogonTransactionRequest request, CancellationToken cancellationToken)
        {
            return new ProcessLogonTransactionResponse
                   {
                       ResponseCode = "1234"
                   };
        }
    }
}
