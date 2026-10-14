using NUnit.Framework;
using TransactionProcessor.DataTransferObjects;
using System;

namespace TransactionProcessor.IntegrationTests.Common;

[TestFixture]
public class TransactionResponseAssertionsTests
{
    [Test]
    public void Verify_rejects_a_response_for_the_wrong_merchant()
    {
        Guid estateId = Guid.NewGuid();
        Guid requestedMerchantId = Guid.NewGuid();
        TransactionResponse response = new()
        {
            EstateId = estateId,
            MerchantId = Guid.NewGuid(),
            TransactionId = Guid.NewGuid()
        };

        Assert.Throws<Shouldly.ShouldAssertException>(() =>
            TransactionResponseAssertions.Verify(response, estateId, requestedMerchantId));
    }
}
