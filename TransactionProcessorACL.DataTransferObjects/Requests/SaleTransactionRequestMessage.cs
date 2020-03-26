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

        /// <summary>
        /// Gets or sets the customer account number.
        /// </summary>
        /// <value>
        /// The customer account number.
        /// </value>
        public String CustomerAccountNumber { get; set; }

        /// <summary>
        /// Gets or sets the operator identifier.
        /// </summary>
        /// <value>
        /// The operator identifier.
        /// </value>
        public String OperatorIdentifier { get; set; }

        #endregion
    }
}