using System.Net;
using System.Net.Http.Json;
using NUnit.Framework;
using WebApi.GymManagement;
using WebApi.GymManagement.Contracts;

namespace IntegrationTests;

internal sealed class SetBoulderingRouteTests : IntegrationTestsBase
{
    private byte[] _routePicture;

    [SetUp]
    public async Task LoadRoutePicture()
    {
        _routePicture = await File.ReadAllBytesAsync("./Resources/routePicture.jpg");
    }

    [CancelAfter(10_000)]
    [Test]
    public async Task SetBoulderingRoute__WhenNoPayload__ShouldBadRequest(CancellationToken cancellationToken)
    {
        var client = ApplicationFactory.CreateClient();

        var form = new MultipartFormDataContent();
        form.Add(new ByteArrayContent(_routePicture));

        var response = await client.PostAsync("/routes/set", form, cancellationToken);

        await response.AssertIs(HttpStatusCode.BadRequest);
    }

    [CancelAfter(10_000)]
    [Test]
    public async Task SetDifferentBoulderingRoute__ShouldReturnDifferentRouteCodes(CancellationToken cancellationToken)
    {
        var client = ApplicationFactory.CreateClient();

        var enrollmentResponse = await client.PostAsync(
            "/gyms/enroll",
            JsonContent.Create(
                new EnrollNewGymRequest
                {
                    Code = "MGYM",
                    Name = "My gym"
                }),
            cancellationToken);

        await enrollmentResponse.AssertIsSuccessful();
        var gym = await enrollmentResponse.Content.ReadFromJsonAsync<GymDto>(cancellationToken: cancellationToken);

        var form = new MultipartFormDataContent();
        form.Add(new ByteArrayContent(_routePicture), "RoutePicture");
        form.Add(
            JsonContent.Create(
                new SetBoulderingRouteRequest.PayloadRequest
                {
                    GymId = gym!.Id,
                    Color = SetBoulderingRouteRequest.RouteColor.Yellow
                }),
            "Payload");

        var setRoute1Response = await client.PostAsync("/routes/set", form, cancellationToken);
        await setRoute1Response.AssertIsSuccessful();
        var route1 = await setRoute1Response.Content.ReadFromJsonAsync<SetBoulderingRouteResponse>(cancellationToken);

        var setRoute2Response = await client.PostAsync("/routes/set", form, cancellationToken);
        await setRoute2Response.AssertIsSuccessful();
        var route2 = await setRoute2Response.Content.ReadFromJsonAsync<SetBoulderingRouteResponse>(cancellationToken);

        Assert.That(route1!.RouteCode, Is.Not.EqualTo(route2!.RouteCode));
    }
}