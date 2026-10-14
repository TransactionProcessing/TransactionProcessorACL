using Shouldly;
using TestHosts.DataTransferObjects.AgencyBanking;

namespace TransactionProcessor.IntegrationTests.Common;

public static class AgencyBankingAssertions
{
    public static void VerifyGlAccount(GlAccountResponse response, CreateGlAccountRequest request)
    {
        response.ShouldNotBeNull();
        response.GlCode.ShouldBe(request.GlCode);
        response.GlName.ShouldBe(request.GlName);
        response.GlType.ShouldBe(request.GlType);
        response.Currency.ShouldBe(request.Currency);
    }

    public static void VerifyCustomer(CustomerResponse response, CreateCustomerRequest request)
    {
        response.ShouldNotBeNull();
        response.CustomerId.ShouldBe(request.CustomerId);
        response.FullName.ShouldBe(request.FullName);
        response.PhoneNumber.ShouldBe(request.PhoneNumber);
        response.NationalId.ShouldBe(request.NationalId);
        response.AccountNumber.ShouldBe(request.AccountNumber);
    }

    public static void VerifySettlementAccount(SettlementAccountResponse response, CreateSettlementAccountRequest request)
    {
        response.ShouldNotBeNull();
        response.AccountNumber.ShouldBe(request.AccountNumber);
        response.AccountName.ShouldBe(request.AccountName);
        response.BankCode.ShouldBe(request.BankCode);
        response.Currency.ShouldBe(request.Currency);
    }

    public static void VerifySystemConfiguration(SystemConfigurationDto response, SystemInitializationRequest request)
    {
        response.ShouldNotBeNull();
        response.InstitutionCode.ShouldBe(request.InstitutionCode);
        response.InstitutionName.ShouldBe(request.InstitutionName);
        response.DefaultCurrency.ShouldBe(request.DefaultCurrency);
        response.Timezone.ShouldBe(request.Timezone);
        response.SettlementMode.ShouldBe(request.SettlementMode);
    }
}
