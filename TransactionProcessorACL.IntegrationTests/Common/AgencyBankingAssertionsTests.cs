using NUnit.Framework;
using TestHosts.DataTransferObjects.AgencyBanking;

namespace TransactionProcessor.IntegrationTests.Common;

[TestFixture]
public class AgencyBankingAssertionsTests
{
    [Test]
    public void VerifyGlAccount_rejects_a_persisted_account_with_the_wrong_code()
    {
        CreateGlAccountRequest request = new()
        {
            GlCode = "1000",
            GlName = "Cash",
            GlType = "Asset",
            Currency = "KES"
        };

        GlAccountResponse response = new()
        {
            GlCode = "2000",
            GlName = request.GlName,
            GlType = request.GlType,
            Currency = request.Currency
        };

        Assert.Throws<Shouldly.ShouldAssertException>(() => AgencyBankingAssertions.VerifyGlAccount(response, request));
    }
}
