namespace TransactionProcessorACL.DataTransferObjects
{
    using System;

    public class TransactionRequestMessage
    {
        public String IMEINumber { get; set; }
        public DateTime TransactionDateTime { get; set; }
        public String TransactionNumber { get; set; }
    }
}