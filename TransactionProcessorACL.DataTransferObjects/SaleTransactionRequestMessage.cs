namespace TransactionProcessorACL.DataTransferObjects
{
    using System;

    public class SaleTransactionRequestMessage : TransactionRequestMessage
    {
        public Decimal Amount { get; set; }
    }
}