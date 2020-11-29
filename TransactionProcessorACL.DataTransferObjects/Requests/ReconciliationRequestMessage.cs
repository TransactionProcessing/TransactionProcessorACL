namespace TransactionProcessorACL.DataTransferObjects
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;

    [ExcludeFromCodeCoverage]
    public class ReconciliationRequestMessage : TransactionRequestMessage
    {
        /// <summary>
        /// Gets or sets the operator totals.
        /// </summary>
        /// <value>
        /// The operator totals.
        /// </value>
        public List<OperatorTotalRequest> OperatorTotals { get; set; }

        /// <summary>
        /// Gets or sets the transaction count.
        /// </summary>
        /// <value>
        /// The transaction count.
        /// </value>
        public Int32 TransactionCount { get; set; }

        /// <summary>
        /// Gets or sets the transaction value.
        /// </summary>
        /// <value>
        /// The transaction value.
        /// </value>
        public Decimal TransactionValue { get; set; }
    }
}