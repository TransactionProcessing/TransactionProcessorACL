using System;
using System.Collections.Generic;
using TransactionProcessing.UnionResults;
using TransactionProcessorACL.BusinessLogic.Requests;

namespace TransactionProcessorACL.BusinessLogic.Services
{
    using System.Threading;
    using System.Threading.Tasks;
    using TransactionProcessorACL.DataTransferObjects.Requests;
    using TransactionProcessorACL.Models;
    using GetVoucherResponse = Models.GetVoucherResponse;
    using RedeemVoucherResponse = Models.RedeemVoucherResponse;

    public interface ITransactionProcessorACLApplicationService
    {
        Task<Result<ProcessLogonTransactionResponse>> ProcessLogonTransaction(TransactionCommands.ProcessLogonTransactionCommand command,
                                                                              CancellationToken cancellationToken);
        
        Task<Result<ProcessSaleTransactionResponse>> ProcessSaleTransaction(TransactionCommands.ProcessSaleTransactionCommand command, CancellationToken cancellationToken);

       Task<Result<ProcessReconciliationResponse>> ProcessReconciliation(TransactionCommands.ProcessReconciliationCommand command,
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

        Task<Result<RecentActivityReceiptSearchResponse>> GetRecentActivityReceiptSearch(Guid estateId,
                                                                                          RecentActivityReceiptSearchRequest request,
                                                                                          CancellationToken cancellationToken);

        Task<Result<ResendReceiptResponse>> ResendReceipt(Guid estateId,
                                                          Guid merchantId,
                                                          String reference,
                                                          String recipientEmailAddress,
                                                          CancellationToken cancellationToken);
    }
}
