namespace TransactionProcessorACL.Factories
{
    using DataTransferObjects.Responses;
    using Models;

    /// <summary>
    /// 
    /// </summary>
    public interface IModelFactory
    {
        #region Methods

        LogonTransactionResponseMessage ConvertFrom(ProcessLogonTransactionResponse processLogonTransactionResponse);

        SaleTransactionResponseMessage ConvertFrom(ProcessSaleTransactionResponse processSaleTransactionResponse);
        
        ReconciliationResponseMessage ConvertFrom(ProcessReconciliationResponse processReconciliationResponse);

        RedeemVoucherResponseMessage ConvertFrom(RedeemVoucherResponse model);

        GetVoucherResponseMessage ConvertFrom(GetVoucherResponse model);

        #endregion
    }
}