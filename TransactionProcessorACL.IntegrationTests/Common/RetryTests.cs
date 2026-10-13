using NUnit.Framework;
using System;
using System.Threading.Tasks;

namespace TransactionProcessor.IntegrationTests.Common;

[TestFixture]
public class RetryTests
{
    [Test]
    public async Task ForEventually_retries_a_transient_failure()
    {
        Int32 attempts = 0;

        await Retry.ForEventually(() =>
        {
            attempts++;

            if (attempts == 1)
            {
                throw new InvalidOperationException("transient failure");
            }

            return Task.CompletedTask;
        }, TimeSpan.FromSeconds(1), TimeSpan.Zero);

        Assert.That(attempts, Is.EqualTo(2));
    }
}
