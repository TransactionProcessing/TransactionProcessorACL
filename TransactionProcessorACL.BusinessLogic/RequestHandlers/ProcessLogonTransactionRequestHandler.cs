using SimpleResults;

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
    /// <seealso cref="ProcessLogonTransactionResponse" />
    public class TransactionRequestHandler : IRequestHandler<TransactionCommands.ProcessLogonTransactionCommand, Result<ProcessLogonTransactionResponse>>,
        IRequestHandler<TransactionCommands.ProcessReconciliationCommand, Result<ProcessReconciliationResponse>>,
        IRequestHandler<TransactionCommands.ProcessSaleTransactionCommand, Result<ProcessSaleTransactionResponse>>
    {
        #region Fields

        /// <summary>
        /// The application service
        /// </summary>
        private readonly ITransactionProcessorACLApplicationService ApplicationService;

        #endregion

        #region Constructors

        public TransactionRequestHandler(ITransactionProcessorACLApplicationService applicationService)
        {
            this.ApplicationService = applicationService;
        }

        #endregion

        #region Methods

        public async Task<Result<ProcessLogonTransactionResponse>> Handle(TransactionCommands.ProcessLogonTransactionCommand command,
                                                                          CancellationToken cancellationToken)
        {
            return await this.ApplicationService.ProcessLogonTransaction(command.EstateId,
                command.MerchantId,
                command.TransactionDateTime,
                command.TransactionNumber,
                command.DeviceIdentifier,
                                                                         cancellationToken);
        }

        public async Task<Result<ProcessReconciliationResponse>> Handle(TransactionCommands.ProcessReconciliationCommand command,
                                                                        CancellationToken cancellationToken)
        {
            return await this.ApplicationService.ProcessReconciliation(command.EstateId,
                command.MerchantId,
                command.TransactionDateTime,
                command.DeviceIdentifier,
                command.TransactionCount,
                command.TransactionValue,
                cancellationToken);
        }

        public async Task<Result<ProcessSaleTransactionResponse>> Handle(TransactionCommands.ProcessSaleTransactionCommand command,
                                                                         CancellationToken cancellationToken)
        {
            return await this.ApplicationService.ProcessSaleTransaction(command.EstateId,
                command.MerchantId,
                command.TransactionDateTime,
                command.TransactionNumber,
                command.DeviceIdentifier,
                command.OperatorId,
                command.CustomerEmailAddress,
                command.ContractId,
                command.ProductId,
                command.AdditionalRequestMetadata,
                cancellationToken);
        }

        #endregion
    }
}