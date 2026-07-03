using System;
using System.Collections.Generic;
using System.Text;
using SimpleResults;

namespace TransactionProcessorACL.BusinessLogic.Services
{
    using System.Threading;
    using System.Threading.Tasks;
    using Shared.General;
    using TransactionProcessorACL.DataTransferObjects.Requests;
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
        
        Task<Result<ProcessSaleTransactionResponse>> ProcessSaleTransaction((Guid estateId, Guid merchantId) merchantData,
                                                                            DateTime transactionDateTime,
                                                                            String transactionNumber,
                                                                            String deviceIdentifier,
                                                                            String customerEmailAddress,
                                                                            (Guid operatorId, Guid contractId, Guid productId) productData,
                                                                            Dictionary<String, String> additionalRequestMetadata, CancellationToken cancellationToken);

       Task<Result<ProcessReconciliationResponse>> ProcessReconciliation(Guid estateId,
                                                                  Guid merchantId,
                                                                  DateTime transactionDateTime,
                                                                  String deviceIdentifier,
                                                                  Int32 transactionCount,
                                                                  Decimal transactionValue,
                                                                  CancellationToken cancellationToken);

       Task<Result<GetVoucherResponse>> GetVoucher(Guid estateId,
                                           Guid contractId,
                                           String voucherCode,
                                           CancellationToken cancellationToken);

       Task<Result<RedeemVoucherResponse>> RedeemVoucher(Guid estateId,
                                                 Guid contractId,
                                                 String voucherCode,
                                                 CancellationToken cancellationToken);

       Task<Result<List<Models.ContractResponse>>> GetMerchantContracts(Guid estateId,
                                                                        Guid merchantId,
                                                                        CancellationToken cancellationToken);

       Task<Result<Models.MerchantResponse>> GetMerchant(Guid estateId,
                                                                        Guid merchantId,
                                                                        CancellationToken cancellationToken);

       Task<Result<MerchantScheduleResponse>> GetMerchantSchedule(Guid estateId,
                                                                  Guid merchantId,
                                                                  Int32 year,
                                                                  CancellationToken cancellationToken);

        Task<Result<MerchantDailyPerformanceSummaryResponse>> GetMerchantDailyPerformanceSummary(Guid estateId,
                                                                                                MerchantDailyPerformanceSummaryRequest request,
                                                                                                CancellationToken cancellationToken);

        Task<Result<MerchantTransactionMixSummaryResponse>> GetMerchantTransactionMixSummary(Guid estateId,
                                                                                             MerchantTransactionMixSummaryRequest request,
                                                                                             CancellationToken cancellationToken);
    }
}
