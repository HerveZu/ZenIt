using System.Net;
using System.Net.Http.Json;
using Domain.BoulderingRoutes;
using NUnit.Framework;
using WebApi.GymManagement;
using WebApi.GymManagement.Contracts;

namespace IntegrationTests;

internal sealed class SetBoulderingRouteTests : IntegrationTestsBase
{
    private byte[] _routePicture;
    private byte[] _yellowHoldPicture;

    [SetUp]
    public async Task LoadRoutePicture()
    {
        _routePicture = await File.ReadAllBytesAsync("./Resources/routePicture.jpg");
        _yellowHoldPicture = await File.ReadAllBytesAsync("./Resources/yellowHold.png");
    }

    [CancelAfter(10_000)]
    [Test]
    public async Task SetBoulderingRoute__WhenNoRoutePicture__ShouldBadRequest(CancellationToken cancellationToken)
    {
        var client = ApplicationFactory.CreateClient();

        var form = new MultipartFormDataContent();
        form.Add(new ByteArrayContent(_routePicture), "RoutePicture");

        var response = await client.PostAsync("/routes/set", form, cancellationToken);

        await response.AssertIs(HttpStatusCode.BadRequest);
    }

    [CancelAfter(10_000)]
    [Test]
    public async Task SetBoulderingRoute__WhenNoRoutePayload__ShouldBadRequest(CancellationToken cancellationToken)
    {
        var client = ApplicationFactory.CreateClient();

        var form = new MultipartFormDataContent();
        form.Add(
            JsonContent.Create(
                new SetBoulderingRouteRequest.RoutePayload
                {
                    GymId = Guid.NewGuid(),
                    Color = SetBoulderingRouteRequest.RouteColor.Yellow,
                    Holds = []
                }),
            "Route");

        var response = await client.PostAsync("/routes/set", form, cancellationToken);

        await response.AssertIs(HttpStatusCode.BadRequest);
    }

    [CancelAfter(10_000)]
    [Test]
    public async Task SetBoulderingRoute__WhenValid__ShouldSucceed(CancellationToken cancellationToken)
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

        const string holdName = "hold.png";
        var form = new MultipartFormDataContent();
        form.Add(new ByteArrayContent(_routePicture), "routePicture", "routePicture.jpg");
        form.Add(new ByteArrayContent(_yellowHoldPicture), holdName, holdName);
        form.Add(
            JsonContent.Create(
                new SetBoulderingRouteRequest.RoutePayload
                {
                    GymId = gym!.Id,
                    Color = SetBoulderingRouteRequest.RouteColor.Yellow,
                    Holds =
                    [
                        new SetBoulderingRouteRequest.HoldDetection
                        {
                            Segmentation = holdName,
                            X = 100,
                            Y = 100
                        }
                    ]
                }),
            "Route");

        var response = await client.PostAsync("/routes/set", form, cancellationToken);
        await response.AssertIsSuccessful();
    }

    [CancelAfter(10_000)]
    [Test]
    public async Task SetDifferentBoulderingRoutes__WhenSameData__ShouldReturnDifferentCodes(
        CancellationToken cancellationToken)
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

        const string holdName = "hold.png";
        var form = new MultipartFormDataContent();
        form.Add(new ByteArrayContent(_routePicture), "routePicture", "routePicture.jpg");
        form.Add(new ByteArrayContent(_yellowHoldPicture), holdName, holdName);
        form.Add(
            JsonContent.Create(
                new SetBoulderingRouteRequest.RoutePayload
                {
                    GymId = gym!.Id,
                    Color = SetBoulderingRouteRequest.RouteColor.Yellow,
                    Holds =
                    [
                        new SetBoulderingRouteRequest.HoldDetection
                        {
                            Segmentation = holdName,
                            X = 100,
                            Y = 100
                        }
                    ]
                }),
            "Route");

        var setRoute1Response = await client.PostAsync("/routes/set", form, cancellationToken);
        await setRoute1Response.AssertIsSuccessful();
        var route1 = await setRoute1Response.Content.ReadFromJsonAsync<SetBoulderingRouteResponse>(cancellationToken);

        var setRoute2Response = await client.PostAsync("/routes/set", form, cancellationToken);
        await setRoute2Response.AssertIsSuccessful();
        var route2 = await setRoute2Response.Content.ReadFromJsonAsync<SetBoulderingRouteResponse>(cancellationToken);

        Assert.That(route1!.RouteCode, Is.Not.EqualTo(route2!.RouteCode));
    }

    [TestCase(500, 700, 50, 70, .1, .1)]
    [TestCase(1000, 100, 50, 0, .05, 0)]
    public void AddRouteHoldDetections__ShouldNormalizeHoldPositionRelativeToOriginalPictureDimensions(
        int pictureWidth,
        int pictureHeight,
        int holdX,
        int holdY,
        double expectedX,
        double expectedY)
    {
        var route = BoulderingRoute.Set(
            Guid.NewGuid(),
            "MGYM",
            [],
            BoulderingRouteColor.Yellow,
            new OriginalPicture
            {
                Data = [],
                OriginalWidth = (uint)pictureWidth,
                OriginalHeight = (uint)pictureHeight
            });

        route.AddHoldDetection([], (uint)holdX, (uint)holdY);
        var detectedHold = route.DetectedHolds.Single();

        Assert.Multiple(
            () =>
            {
                Assert.That(detectedHold.X.Value, Is.EqualTo(expectedX));
                Assert.That(detectedHold.Y.Value, Is.EqualTo(expectedY));
            });
    }
}