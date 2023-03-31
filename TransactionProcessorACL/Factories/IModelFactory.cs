namespace TransactionProcessorACL.Factories
{
    using DataTransferObjects.Responses;
    using Models;
    using TransactionProcessor.DataTransferObjects;
    using GetVoucherResponse = Models.GetVoucherResponse;
    using RedeemVoucherResponse = Models.RedeemVoucherResponse;

    /// <summary>
    /// 
    /// </summary>
    public interface IModelFactory
    {
        #region Methods
        
        LogonTransactionResponseMessage ConvertFrom(ProcessLogonTransactionResponse processLogonTransactionResponse);
        
        SaleTransactionResponseMessage ConvertFrom(ProcessSaleTransactionResponse processSaleTransactionResponse);
        
        ReconciliationResponseMessage ConvertFrom(ProcessReconciliationResponse processReconciliationResponse);

        GetVoucherResponseMessage ConvertFrom(GetVoucherResponse model);
        RedeemVoucherResponseMessage ConvertFrom(RedeemVoucherResponse model);

        #endregion
    }
}