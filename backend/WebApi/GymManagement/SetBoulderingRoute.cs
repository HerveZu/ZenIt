using Domain.BoulderingRoutes;
using Domain.Gyms;
using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using WebApi.Common.Infrastructure;

namespace WebApi.GymManagement;

[Serializable]
internal sealed record SetBoulderingRouteRequest
{
    [Serializable]
    public enum RouteColor
    {
        Yellow
    }

    public required PayloadRequest Payload { get; init; }
    public required IFormFile? RoutePicture { get; init; }

    [Serializable]
    public sealed record PayloadRequest
    {
        public required Guid GymId { get; init; }
        public required RouteColor Color { get; init; }
    }
}

[Serializable]
internal sealed record SetBoulderingRouteResponse
{
    public required string RouteCode { get; init; }
}

internal sealed class SetBoulderingRoute(AppDbContext dbContext, IRouteAnalyser routeAnalyser)
    : Endpoint<SetBoulderingRouteRequest, SetBoulderingRouteResponse>
{
    public override void Configure()
    {
        Post("/routes/set");
        AllowAnonymous();
        AllowFileUploads();
    }

    public override async Task HandleAsync(SetBoulderingRouteRequest req, CancellationToken ct)
    {
        var gym = await dbContext.Set<Gym>().FindAsync([req.Payload.GymId], ct);

        if (gym is null)
        {
            ThrowError($"Gym was not found with id '{req.Payload.GymId}'");
            return;
        }

        var usedRoutesIndexes = await (
            from route in dbContext.Set<BoulderingRoute>()
            where route.GymId == req.Payload.GymId
            select route.Index).ToArrayAsync(ct);

        if (usedRoutesIndexes.Length >= BoulderingRouteCode.MaxUniqueCodes)
        {
            ThrowError($"Max route limit for this gym is reached ({BoulderingRouteCode.MaxUniqueCodes})");
            return;
        }

        var routeColor = req.Payload.Color switch
        {
            SetBoulderingRouteRequest.RouteColor.Yellow => BoulderingRouteColor.Yellow,
            _ => throw new ArgumentOutOfRangeException()
        };

        var boulderingRoute = BoulderingRoute.Set(
            gym.Id,
            gym.Code.Value,
            usedRoutesIndexes,
            routeColor);

        if (req.RoutePicture is not null)
        {
            using var pictureStream = new MemoryStream();
            await req.RoutePicture.CopyToAsync(pictureStream, ct);

            boulderingRoute.UsePicture(routeAnalyser, pictureStream.ToArray());
        }

        await dbContext.Set<BoulderingRoute>().AddAsync(boulderingRoute, ct);
        await dbContext.SaveChangesAsync(ct);

        await SendAsync(
            new SetBoulderingRouteResponse
            {
                RouteCode = boulderingRoute.Code.Value
            },
            cancellation: ct);
    }
}