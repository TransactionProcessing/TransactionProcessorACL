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

        /// <summary>
        /// Converts from.
        /// </summary>
        /// <param name="processLogonTransactionResponse">The process logon transaction response.</param>
        /// <returns></returns>
        LogonTransactionResponseMessage ConvertFrom(ProcessLogonTransactionResponse processLogonTransactionResponse);

        /// <summary>
        /// Converts from.
        /// </summary>
        /// <param name="processSaleTransactionResponse">The process sale transaction response.</param>
        /// <returns></returns>
        SaleTransactionResponseMessage ConvertFrom(ProcessSaleTransactionResponse processSaleTransactionResponse);

        /// <summary>
        /// Converts from.
        /// </summary>
        /// <param name="processReconciliationResponse">The process reconciliation response.</param>
        /// <returns></returns>
        ReconciliationResponseMessage ConvertFrom(ProcessReconciliationResponse processReconciliationResponse);

        #endregion
    }
}