using System;
using System.Collections.Generic;
using System.Text;

namespace TransactionProcessorACL.BusinessLogic.Services
{
    using System.Threading;
    using System.Threading.Tasks;
    using Shared.General;
    using TransactionProcessor.DataTransferObjects;
    using TransactionProcessorACL.Models;
    using GetVoucherResponse = Models.GetVoucherResponse;
    using RedeemVoucherResponse = Models.RedeemVoucherResponse;

    public interface ITransactionProcessorACLApplicationService
    {
        Task<ProcessLogonTransactionResponse> ProcessLogonTransaction(Guid estateId,
                                                                      Guid merchantId,
                                                                      DateTime transactionDateTime,
                                                                      String transactionNumber,
                                                                      String deviceIdentifier,
                                                                      CancellationToken cancellationToken);
        
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

       Task<RedeemVoucherResponse> RedeemVoucher(Guid estateId,
                                                 Guid contractId,
                                                 String voucherCode,
                                                 CancellationToken cancellationToken);
    }
}
