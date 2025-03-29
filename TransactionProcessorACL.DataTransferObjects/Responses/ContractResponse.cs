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