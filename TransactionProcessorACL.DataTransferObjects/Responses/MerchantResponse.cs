using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.ComponentModel;

namespace TransactionProcessorACL.DataTransferObjects.Responses {

    [ExcludeFromCodeCoverage]
    public class MerchantResponse
    {
        [Description("Merchant postal addresses.")]
        public List<AddressResponse> Addresses { get; set; }
        [Description("Merchant contacts.")]
        public List<ContactResponse> Contacts { get; set; }

        [Description("Devices associated with the merchant, keyed by device identifier with a display name as the value.")]
        public Dictionary<Guid, string> Devices { get; set; }

        [Description("Estate that owns the merchant.")]
        public Guid EstateId { get; set; }
 
        [Description("Estate reporting identifier.")]
        public int EstateReportingId { get; set; }

        [Description("Unique merchant identifier.")]
        public Guid MerchantId { get; set; }

        [Description("Merchant reporting identifier.")]
        public int MerchantReportingId { get; set; }

        [Description("Merchant display name.")]
        public string MerchantName { get; set; }

        [Description("Merchant reference used by clients or reporting.")]
        public string MerchantReference { get; set; }

        [Description("Date of the merchant's next statement.")]
        public DateTime NextStatementDate { get; set; }

        [Description("Operators registered for the merchant.")]
        public List<MerchantOperatorResponse> Operators { get; set; }

        [Description("Merchant settlement frequency.")]
        public SettlementSchedule SettlementSchedule { get; set; }

        [Description("Contracts associated with the merchant and their product identifiers.")]
        public List<MerchantContractResponse> Contracts { get; set; }

        [Description("Opening hours keyed by day of the week.")]
        public Dictionary<DayOfWeek, OpeningHoursResponse> OpeningHours { get; set; }
    }

    public class OpeningHoursResponse
    {
        [Description("Local opening time for the day, in the format supplied by the merchant data.")]
        public string Opening { get; set; }

        [Description("Local closing time for the day, in the format supplied by the merchant data.")]
        public string Closing { get; set; }
    }

    [ExcludeFromCodeCoverage]
    public class AddressResponse
    {
        [Description("Unique address identifier.")]
        public Guid AddressId { get; set; }

        [Description("First address line.")]
        public string AddressLine1 { get; set; }

        [Description("Second address line, when present.")]
        public string AddressLine2 { get; set; }

        [Description("Third address line, when present.")]
        public string AddressLine3 { get; set; }

        [Description("Fourth address line, when present.")]
        public string AddressLine4 { get; set; }

        [Description("Country name or code.")]
        public string Country { get; set; }

        [Description("Postal or ZIP code.")]
        public string PostalCode { get; set; }

        [Description("Region, county, or state.")]
        public string Region { get; set; }

        [Description("Town or city.")]
        public string Town { get; set; }
    }

    [ExcludeFromCodeCoverage]
    public class ContactResponse
    {
        [Description("Contact email address.")]
        public string ContactEmailAddress { get; set; }

        [Description("Unique contact identifier.")]
        public Guid ContactId { get; set; }

        [Description("Contact's name.")]
        public string ContactName { get; set; }

        [Description("Contact telephone number.")]
        public string ContactPhoneNumber { get; set; }
    }

    [ExcludeFromCodeCoverage]
    public class MerchantOperatorResponse
    {
        [Description("Operator display name.")]
        public string Name { get; set; }

        [Description("Unique operator identifier.")]
        public Guid OperatorId { get; set; }

        [Description("Merchant number assigned to this operator.")]
        public string MerchantNumber { get; set; }

        [Description("Terminal number assigned to this operator.")]
        public string TerminalNumber { get; set; }

        [Description("Indicates whether the operator has been deleted.")]
        public bool IsDeleted { get; set; }
    }

    [ExcludeFromCodeCoverage]
    public class MerchantContractResponse
    {
        [Description("Unique contract identifier.")]
        public Guid ContractId { get; set; }

        [Description("Indicates whether the merchant's contract is deleted.")]
        public bool IsDeleted { get; set; }

        [Description("Product identifiers available on this merchant contract.")]
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
