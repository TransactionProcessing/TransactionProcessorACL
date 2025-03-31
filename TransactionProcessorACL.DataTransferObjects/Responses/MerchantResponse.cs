using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace TransactionProcessorACL.DataTransferObjects.Responses {

    [ExcludeFromCodeCoverage]
    public class MerchantResponse
    {
        [JsonProperty("addresses")]
        public List<AddressResponse> Addresses { get; set; }

        [JsonProperty("contacts")]
        public List<ContactResponse> Contacts { get; set; }

        [JsonProperty("devices")]
        public Dictionary<Guid, string> Devices { get; set; }

        [JsonProperty("estate_id")]
        public Guid EstateId { get; set; }

        [JsonProperty("estate_reporting_id")]
        public int EstateReportingId { get; set; }

        [JsonProperty("merchant_id")]
        public Guid MerchantId { get; set; }

        [JsonProperty("merchant_reporting_id")]
        public int MerchantReportingId { get; set; }

        [JsonProperty("merchant_name")]
        public string MerchantName { get; set; }

        [JsonProperty("merchant_reference")]
        public string MerchantReference { get; set; }

        [JsonProperty("next_statement_date")]
        public DateTime NextStatementDate { get; set; }

        [JsonProperty("operators")]
        public List<MerchantOperatorResponse> Operators { get; set; }

        [JsonProperty("settlement_schedule")]
        public SettlementSchedule SettlementSchedule { get; set; }

        [JsonProperty("contracts")]
        public List<MerchantContractResponse> Contracts { get; set; }
    }

    [ExcludeFromCodeCoverage]
    public class AddressResponse
    {
        [JsonProperty("address_id")]
        public Guid AddressId { get; set; }

        [JsonProperty("address_line_1")]
        public string AddressLine1 { get; set; }

        [JsonProperty("address_line_2")]
        public string AddressLine2 { get; set; }

        [JsonProperty("address_line_3")]
        public string AddressLine3 { get; set; }

        [JsonProperty("address_line_4")]
        public string AddressLine4 { get; set; }

        [JsonProperty("country")]
        public string Country { get; set; }

        [JsonProperty("postal_code")]
        public string PostalCode { get; set; }

        [JsonProperty("region")]
        public string Region { get; set; }

        [JsonProperty("town")]
        public string Town { get; set; }
    }

    [ExcludeFromCodeCoverage]
    public class ContactResponse
    {
        [JsonProperty("contact_email_address")]
        public string ContactEmailAddress { get; set; }

        [JsonProperty("contact_id")]
        public Guid ContactId { get; set; }

        [JsonProperty("contact_name")]
        public string ContactName { get; set; }

        [JsonProperty("contact_phone_number")]
        public string ContactPhoneNumber { get; set; }
    }

    [ExcludeFromCodeCoverage]
    public class MerchantOperatorResponse
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("operator_id")]
        public Guid OperatorId { get; set; }

        [JsonProperty("merchant_number")]
        public string MerchantNumber { get; set; }

        [JsonProperty("terminal_number")]
        public string TerminalNumber { get; set; }

        [JsonProperty("is_deleted")]
        public bool IsDeleted { get; set; }
    }

    [ExcludeFromCodeCoverage]
    public class MerchantContractResponse
    {
        [JsonProperty("contract_id")]
        public Guid ContractId { get; set; }

        [JsonProperty("is_deleted")]
        public bool IsDeleted { get; set; }

        [JsonProperty("contract_products")]
        public List<Guid> ContractProducts { get; set; }

        public MerchantContractResponse() => this.ContractProducts = new List<Guid>();
    }

    public enum SettlementSchedule
    {
        NotSet,
        Immediate,
        Weekly,
        Monthly,
    }
}