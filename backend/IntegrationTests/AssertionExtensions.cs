using NUnit.Framework;

namespace IntegrationTests;

internal static class AssertionExtensions
{
    public static async Task AssertIsSuccessful(this HttpResponseMessage message, CancellationToken cancellationToken)
    {
        if (message.IsSuccessStatusCode)
        {
            return;
        }

        await TestContext.Error.WriteLineAsync(await message.Content.ReadAsStringAsync(cancellationToken));
        message.EnsureSuccessStatusCode();
    }
}