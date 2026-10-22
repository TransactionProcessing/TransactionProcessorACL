using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace TransactionProcessorACL.DataTransferObjects.Responses {
    public class ContractResponse
    {
        [Description("Unique contract identifier.")]
        public Guid ContractId { get; set; }

        [Description("Contract reporting identifier.")]
        public int ContractReportingId { get; set; }

        [Description("Display description of the contract.")]
        public string Description { get; set; }

        [Description("Estate that owns the contract.")]
        public Guid EstateId { get; set; }

        [Description("Estate reporting identifier.")]
        public int EstateReportingId { get; set; }

        [Description("Operator associated with the contract.")]
        public Guid OperatorId { get; set; }

        [Description("Display name of the associated operator.")]
        public string OperatorName { get; set; }

        [Description("Products and fees available under this contract.")]
        public List<ContractProduct> Products { get; set; }
    }

    public class ContractProduct
    {
        [Description("Text intended for display to the client.")]
        public string DisplayText { get; set; }
        [Description("Product name.")]
        public string Name { get; set; }
        [Description("Unique product identifier.")]
        public Guid ProductId { get; set; }
        [Description("Product reporting identifier.")]
        public int ProductReportingId { get; set; }
        [Description("Transaction fees applicable to the product.")]
        public List<ContractProductTransactionFee> TransactionFees { get; set; }
        [Description("Product value, when the product has a fixed value.")]
        public Decimal? Value { get; set; }
        [Description("Product category, such as MobileTopup, Voucher, or BillPayment.")]
        public ProductType ProductType { get; set; }
    }

    public class ContractProductTransactionFee
    {
        [Description("How the fee is calculated: Fixed or Percentage.")]
        public CalculationType CalculationType { get; set; }
        [Description("Party to whom the fee applies: Merchant or ServiceProvider.")]
        public FeeType FeeType { get; set; }
        [Description("Description of the transaction fee.")]
        public string Description { get; set; }
        [Description("Unique transaction fee identifier.")]
        public Guid TransactionFeeId { get; set; }
        [Description("Transaction fee reporting identifier.")]
        public int TransactionFeeReportingId { get; set; }
        [Description("Fee amount or percentage, interpreted according to CalculationType.")]
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
