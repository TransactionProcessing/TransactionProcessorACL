namespace TransactionProcessorACL.Factories
{
    using System.Collections.Generic;
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

        DataTransferObjects.Responses.MerchantResponse ConvertFrom(Models.MerchantResponse model);

        List<DataTransferObjects.Responses.ContractResponse> ConvertFrom(List<Models.ContractResponse> model);

        #endregion
    }
}
