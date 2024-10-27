using System.Net;
using NUnit.Framework;

namespace IntegrationTests;

internal static class HttpAssertions
{
    public static async Task AssertIsSuccessful(this HttpResponseMessage message)
    {
        if (message.IsSuccessStatusCode)
        {
            return;
        }

        await TestContext.Error.WriteLineAsync(await message.Content.ReadAsStringAsync());
        Assert.That(
            () => message.IsSuccessStatusCode,
            $"Expected successful HTTP status code, was {message.StatusCode}");
    }

    public static async Task AssertIs(this HttpResponseMessage message, HttpStatusCode expectedStatusCode)
    {
        if (message.StatusCode == expectedStatusCode)
        {
            return;
        }

        await TestContext.Error.WriteLineAsync(await message.Content.ReadAsStringAsync());
        Assert.Fail($"Expected HTTP status code {expectedStatusCode}, was {message.StatusCode}");
    }
}