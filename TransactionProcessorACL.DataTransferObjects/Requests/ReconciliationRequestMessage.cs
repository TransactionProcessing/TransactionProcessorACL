namespace TransactionProcessorACL.DataTransferObjects
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.ComponentModel;
    
    [ExcludeFromCodeCoverage]
    public class ReconciliationRequestMessage : TransactionRequestMessage
    {
        [Description("Optional reconciliation totals broken down by operator and contract.")]
        public List<OperatorTotalRequest> OperatorTotals { get; set; }

        [Description("Total number of transactions in the reconciliation period.")]
        public Int32 TransactionCount { get; set; }

        [Description("Total monetary value of transactions in the reconciliation period.")]
        public Decimal TransactionValue { get; set; }
    }
}
