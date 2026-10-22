namespace TransactionProcessorACL.DataTransferObjects
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.ComponentModel;
    
    [ExcludeFromCodeCoverage]
    public class SaleTransactionRequestMessage : TransactionRequestMessage
    {
        #region Properties

        [Description("Contract under which the sale is processed.")]
        public Guid ContractId { get; set; }

        [Description("Operator who performed the sale.")]
        public Guid OperatorId { get; set; }

        [Description("Optional email address for the customer receipt.")]
        public String CustomerEmailAddress { get; set; }

        [Description("Product being sold under the selected contract.")]
        public Guid ProductId { get; set; }

        #endregion
    }
}
