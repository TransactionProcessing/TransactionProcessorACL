namespace TransactionProcessorACL.DataTransferObjects
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="TransactionProcessorACL.DataTransferObjects.TransactionRequestMessage" />
    [ExcludeFromCodeCoverage]
    public class SaleTransactionRequestMessage : TransactionRequestMessage
    {
        #region Properties

        /// <summary>
        /// Gets or sets the amount.
        /// </summary>
        /// <value>
        /// The amount.
        /// </value>
        public Decimal Amount { get; set; }

        #endregion
    }
}