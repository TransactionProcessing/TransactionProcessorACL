using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace TransactionProcessorACL.DataTransferObjects.Responses {
    public class ContractResponse
    {
        [JsonProperty("contract_id")]
        public Guid ContractId { get; set; }

        [JsonProperty("contract_reporting_id")]
        public int ContractReportingId { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("estate_id")]
        public Guid EstateId { get; set; }

        [JsonProperty("estate_reporting_id")]
        public int EstateReportingId { get; set; }

        [JsonProperty("operator_id")]
        public Guid OperatorId { get; set; }

        [JsonProperty("operator_name")]
        public string OperatorName { get; set; }

        [JsonProperty("products")]
        public List<ContractProduct> Products { get; set; }
    }

    public class ContractProduct
    {
        [JsonProperty("display_text")]
        public string DisplayText { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("product_id")]
        public Guid ProductId { get; set; }
        [JsonProperty("product_reporting_id")]
        public int ProductReportingId { get; set; }
        [JsonProperty("transaction_fees")]
        public List<ContractProductTransactionFee> TransactionFees { get; set; }
        [JsonProperty("value")]
        public Decimal? Value { get; set; }
        [JsonProperty("product_type")]
        public ProductType ProductType { get; set; }
    }

    public class ContractProductTransactionFee
    {
        [JsonProperty("calculation_type")]
        public CalculationType CalculationType { get; set; }
        [JsonProperty("fee_type")]
        public FeeType FeeType { get; set; }
        [JsonProperty("description")]
        public string Description { get; set; }
        [JsonProperty("transaction_fee_id")]
        public Guid TransactionFeeId { get; set; }
        [JsonProperty("transaction_fee_reporting_id")]
        public int TransactionFeeReportingId { get; set; }
        [JsonProperty("value")]
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