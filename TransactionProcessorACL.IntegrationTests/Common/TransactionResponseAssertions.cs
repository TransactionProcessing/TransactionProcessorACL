using Shouldly;
using System;
using TransactionProcessor.DataTransferObjects;

namespace TransactionProcessor.IntegrationTests.Common;

public static class TransactionResponseAssertions
{
    public static void Verify(TransactionResponse response,
                              Guid expectedEstateId,
                              Guid expectedMerchantId)
    {
        response.ShouldNotBeNull();
        response.TransactionId.ShouldNotBe(Guid.Empty);
        response.EstateId.ShouldBe(expectedEstateId);
        response.MerchantId.ShouldBe(expectedMerchantId);
    }
}
