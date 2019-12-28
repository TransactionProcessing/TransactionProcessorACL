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

            return logonTransactionResponseMessage;
        }

        #endregion
    }
}