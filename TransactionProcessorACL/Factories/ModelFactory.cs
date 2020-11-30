namespace TransactionProcessorACL.Factories
{
    using DataTransferObjects.Responses;
    using Models;

    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="TransactionProcessorACL.Factories.IModelFactory" />
    public class ModelFactory : IModelFactory
    {
        #region Methods

        /// <summary>
        /// Converts from.
        /// </summary>
        /// <param name="processLogonTransactionResponse">The process logon transaction response.</param>
        /// <returns></returns>
        public LogonTransactionResponseMessage ConvertFrom(ProcessLogonTransactionResponse processLogonTransactionResponse)
        {
            if (processLogonTransactionResponse == null)
            {
                return null;
            }

            LogonTransactionResponseMessage logonTransactionResponseMessage = new LogonTransactionResponseMessage();

            logonTransactionResponseMessage.ResponseMessage = processLogonTransactionResponse.ResponseMessage;
            logonTransactionResponseMessage.ResponseCode = processLogonTransactionResponse.ResponseCode;
            logonTransactionResponseMessage.MerchantId = processLogonTransactionResponse.MerchantId;
            logonTransactionResponseMessage.EstateId = processLogonTransactionResponse.EstateId;

            return logonTransactionResponseMessage;
        }

        /// <summary>
        /// Converts from.
        /// </summary>
        /// <param name="processSaleTransactionResponse">The process sale transaction response.</param>
        /// <returns></returns>
        public SaleTransactionResponseMessage ConvertFrom(ProcessSaleTransactionResponse processSaleTransactionResponse)
        {
            if (processSaleTransactionResponse == null)
            {
                return null;
            }

            SaleTransactionResponseMessage saleTransactionResponseMessage = new SaleTransactionResponseMessage();

            saleTransactionResponseMessage.ResponseMessage = processSaleTransactionResponse.ResponseMessage;
            saleTransactionResponseMessage.ResponseCode = processSaleTransactionResponse.ResponseCode;

            return saleTransactionResponseMessage;
        }

        /// <summary>
        /// Converts from.
        /// </summary>
        /// <param name="processReconciliationResponse">The process reconciliation response.</param>
        /// <returns></returns>
        public ReconciliationResponseMessage ConvertFrom(ProcessReconciliationResponse processReconciliationResponse)
        {
            if (processReconciliationResponse == null)
            {
                return null;
            }

            ReconciliationResponseMessage reconciliationResponseMessage = new ReconciliationResponseMessage();

            reconciliationResponseMessage.ResponseMessage = processReconciliationResponse.ResponseMessage;
            reconciliationResponseMessage.ResponseCode = processReconciliationResponse.ResponseCode;

            return reconciliationResponseMessage;
        }

        #endregion
    }
}