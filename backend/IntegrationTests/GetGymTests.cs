using System.Net;
using NUnit.Framework;

namespace IntegrationTests;

internal sealed class GetGymTests : IntegrationTestsBase
{
    [CancelAfter(10_000)]
    [Test]
    public async Task GetGym__WhenIdDoesntMatch__ShouldNotFound(CancellationToken cancellationToken)
    {
        var client = ApplicationFactory.CreateClient();
        
        var apiResponse = await client.GetAsync($"/gyms/{Guid.NewGuid()}", cancellationToken);

        await apiResponse.AssertIs(HttpStatusCode.NotFound);
    }
}