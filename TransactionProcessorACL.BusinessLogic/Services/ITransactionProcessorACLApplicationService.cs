using System;
using System.Collections.Generic;
using System.Text;

namespace TransactionProcessorACL.BusinessLogic.Services
{
    using System.Threading;
    using System.Threading.Tasks;
    using Shared.General;
    using TransactionProcessorACL.Models;

    public interface ITransactionProcessorACLApplicationService
    {
        /// <summary>
        /// Processes the logon transaction.
        /// </summary>
        /// <param name="estateId">The estate identifier.</param>
        /// <param name="merchantId">The merchant identifier.</param>
        /// <param name="transactionDateTime">The transaction date time.</param>
        /// <param name="transactionNumber">The transaction number.</param>
        /// <param name="deviceIdentifier">The device identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        Task<ProcessLogonTransactionResponse> ProcessLogonTransaction(Guid estateId,
                                                                      Guid merchantId,
                                                                      DateTime transactionDateTime,
                                                                      String transactionNumber,
                                                                      String deviceIdentifier,
                                                                      CancellationToken cancellationToken);

        /// <summary>
        /// Processes the sale transaction.
        /// </summary>
        /// <param name="estateId">The estate identifier.</param>
        /// <param name="merchantId">The merchant identifier.</param>
        /// <param name="transactionDateTime">The transaction date time.</param>
        /// <param name="transactionNumber">The transaction number.</param>
        /// <param name="deviceIdentifier">The device identifier.</param>
        /// <param name="operatorIdentifier">The operator identifier.</param>
        /// <param name="amount">The amount.</param>
        /// <param name="customerAccountNumber">The customer account number.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        Task<ProcessSaleTransactionResponse> ProcessSaleTransaction(Guid estateId,
                                                                      Guid merchantId,
                                                                      DateTime transactionDateTime,
                                                                      String transactionNumber,
                                                                      String deviceIdentifier,
                                                                      String operatorIdentifier,
                                                                      Decimal amount,
                                                                      String customerAccountNumber,
                                                                      CancellationToken cancellationToken);
    }
}
