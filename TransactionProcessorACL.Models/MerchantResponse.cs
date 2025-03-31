using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TransactionProcessorACL.Models
{
    public class MerchantResponse
    {
        public List<AddressResponse> Addresses { get; set; }

        public List<ContactResponse> Contacts { get; set; }

        public Dictionary<Guid, string> Devices { get; set; }

        public Guid EstateId { get; set; }

        public int EstateReportingId { get; set; }

        public Guid MerchantId { get; set; }

        public int MerchantReportingId { get; set; }

        public string MerchantName { get; set; }

        public string MerchantReference { get; set; }

        public DateTime NextStatementDate { get; set; }

        public List<MerchantOperatorResponse> Operators { get; set; }

        public SettlementSchedule SettlementSchedule { get; set; }

        public List<MerchantContractResponse> Contracts { get; set; }
    }

    [ExcludeFromCodeCoverage]
    public class AddressResponse
    {
        public Guid AddressId { get; set; }

        public string AddressLine1 { get; set; }

        public string AddressLine2 { get; set; }

        public string AddressLine3 { get; set; }

        public string AddressLine4 { get; set; }

        public string Country { get; set; }

        public string PostalCode { get; set; }

        public string Region { get; set; }

        public string Town { get; set; }
    }

    [ExcludeFromCodeCoverage]
    public class ContactResponse
    {
        public string ContactEmailAddress { get; set; }

        public Guid ContactId { get; set; }

        public string ContactName { get; set; }

        public string ContactPhoneNumber { get; set; }
    }

    [ExcludeFromCodeCoverage]
    public class MerchantOperatorResponse
    {
        public string Name { get; set; }

        public Guid OperatorId { get; set; }

        public string MerchantNumber { get; set; }

        public string TerminalNumber { get; set; }
        public bool IsDeleted { get; set; }
    }

    [ExcludeFromCodeCoverage]
    public class MerchantContractResponse
    {
        public Guid ContractId { get; set; }

        public bool IsDeleted { get; set; }

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
