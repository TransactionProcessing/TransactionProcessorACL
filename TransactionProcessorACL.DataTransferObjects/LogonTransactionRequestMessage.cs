namespace TransactionProcessorACL.DataTransferObjects
{
    using System;

    public class LogonTransactionRequestMessage : TransactionRequestMessage
    {
        public Boolean RequireConfigurationInResponse { get; set; }
    }
}