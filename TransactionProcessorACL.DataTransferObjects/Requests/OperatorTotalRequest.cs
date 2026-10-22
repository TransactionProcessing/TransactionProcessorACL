namespace TransactionProcessorACL.DataTransferObjects
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.ComponentModel;

    [ExcludeFromCodeCoverage]
    public class OperatorTotalRequest
    {
        #region Properties

        [Description("Contract for this operator's totals.")]
        public Guid ContractId { get; set; }

        [Description("Operator whose transactions are included in these totals.")]
        public Guid OperatorId { get; set; }

        [Description("Transaction count for this operator and contract.")]
        public Int32 TransactionCount { get; set; }

        [Description("Transaction value for this operator and contract.")]
        public Decimal TransactionValue { get; set; }

        #endregion
    }
}
