using System;
using System.Collections.Generic;
using System.Text;
using SimpleResults;

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
        Task<Result<ProcessLogonTransactionResponse>> ProcessLogonTransaction(Guid estateId,
                                                                      Guid merchantId,
                                                                      DateTime transactionDateTime,
                                                                      String transactionNumber,
                                                                      String deviceIdentifier,
                                                                      CancellationToken cancellationToken);
        
        Task<Result<ProcessSaleTransactionResponse>> ProcessSaleTransaction(Guid estateId,
                                                                      Guid merchantId,
                                                                      DateTime transactionDateTime,
                                                                      String transactionNumber,
                                                                      String deviceIdentifier,
                                                                      Guid operatorId,
                                                                      String customerEmailAddress,
                                                                      Guid contractId,
                                                                      Guid productId,
                                                                      Dictionary<String,String> additionalRequestMetadata,
                                                                      CancellationToken cancellationToken);

       Task<Result<ProcessReconciliationResponse>> ProcessReconciliation(Guid estateId,
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
