namespace TransactionProcessorACL.DataTransferObjects
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    
    [ExcludeFromCodeCoverage]
    public class ReconciliationRequestMessage : TransactionRequestMessage
    {
        public List<OperatorTotalRequest> OperatorTotals { get; set; }

        public Int32 TransactionCount { get; set; }

        public Decimal TransactionValue { get; set; }
    }
}