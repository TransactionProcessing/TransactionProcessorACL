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
        /// <param name="customerEmailAddress">The customer email address.</param>
        /// <param name="contractId">The contract identifier.</param>
        /// <param name="productId">The product identifier.</param>
        /// <param name="additionalRequestMetadata">The additional request metadata.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        Task<ProcessSaleTransactionResponse> ProcessSaleTransaction(Guid estateId,
                                                                      Guid merchantId,
                                                                      DateTime transactionDateTime,
                                                                      String transactionNumber,
                                                                      String deviceIdentifier,
                                                                      String operatorIdentifier,
                                                                      String customerEmailAddress,
                                                                      Guid contractId,
                                                                      Guid productId,
                                                                      Dictionary<String,String> additionalRequestMetadata,
                                                                      CancellationToken cancellationToken);

        /// <summary>
        /// Processes the reconciliation.
        /// </summary>
        /// <param name="estateId">The estate identifier.</param>
        /// <param name="merchantId">The merchant identifier.</param>
        /// <param name="transactionDateTime">The transaction date time.</param>
        /// <param name="deviceIdentifier">The device identifier.</param>
        /// <param name="transactionCount">The transaction count.</param>
        /// <param name="transactionValue">The transaction value.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        Task<ProcessReconciliationResponse> ProcessReconciliation(Guid estateId,
                                                                  Guid merchantId,
                                                                  DateTime transactionDateTime,
                                                                  String deviceIdentifier,
                                                                  Int32 transactionCount,
                                                                  Decimal transactionValue,
                                                                  CancellationToken cancellationToken);

        Task<GetVoucherResponse> GetVoucher(Guid estateId,
                                            Guid contractId,
                                            String voucherCode,
                                            CancellationToken cancellationToken);

        /// <summary>
        /// Redeems the voucher.
        /// </summary>
        /// <param name="estateId">The estate identifier.</param>
        /// <param name="contractId">The contract identifier.</param>
        /// <param name="voucherCode">The voucher code.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        Task<RedeemVoucherResponse> RedeemVoucher(Guid estateId,
                                                  Guid contractId,
                                                  String voucherCode,
                                                  CancellationToken cancellationToken);
    }
}
