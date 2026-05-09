namespace TransactionProcessorACL.DataTransferObjects
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    
    [ExcludeFromCodeCoverage]
    public class SaleTransactionRequestMessage : TransactionRequestMessage
    {
        #region Properties

        public Guid ContractId { get; set; }

        public Guid OperatorId { get; set; }

        public String CustomerEmailAddress { get; set; }

        public Guid ProductId { get; set; }

        #endregion
    }
}