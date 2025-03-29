using System;
using System.Collections.Generic;

namespace TransactionProcessorACL.Models {
    public class ContractResponse
    {
        public Guid ContractId { get; set; }
        public int ContractReportingId { get; set; }

        public string Description { get; set; }

        public Guid EstateId { get; set; }

        public int EstateReportingId { get; set; }

        public Guid OperatorId { get; set; }

        public string OperatorName { get; set; }

        public List<ContractProduct> Products { get; set; }
    }

    public class ContractProduct
    {
        public string DisplayText { get; set; }

        public string Name { get; set; }

        public Guid ProductId { get; set; }

        public int ProductReportingId { get; set; }

        public List<ContractProductTransactionFee> TransactionFees { get; set; }

        public Decimal? Value { get; set; }

        public ProductType ProductType { get; set; }
    }

    public class ContractProductTransactionFee
    {
        public CalculationType CalculationType { get; set; }

        public FeeType FeeType { get; set; }

        public string Description { get; set; }

        public Guid TransactionFeeId { get; set; }

        public int TransactionFeeReportingId { get; set; }

        public Decimal Value { get; set; }
    }

    public enum FeeType
    {
        Merchant,
        ServiceProvider,
    }

    public enum CalculationType
    {
        Fixed,
        Percentage,
    }

    public enum ProductType
    {
        NotSet,
        MobileTopup,
        Voucher,
        BillPayment,
    }
}