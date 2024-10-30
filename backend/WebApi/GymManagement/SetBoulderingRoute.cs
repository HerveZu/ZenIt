using Domain.BoulderingRoutes;
using Domain.Gyms;
using FastEndpoints;
using FluentValidation;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using SixLabors.ImageSharp;
using WebApi.Common.Infrastructure;

namespace WebApi.GymManagement;

[PublicAPI]
internal sealed record SetBoulderingRouteRequest
{
    [PublicAPI]
    public enum RouteColor
    {
        Yellow
    }

    public required RoutePayload Route { get; init; }
    public required IFormFile RoutePicture { get; init; }

    [PublicAPI]
    public sealed record RoutePayload
    {
        public required Guid GymId { get; init; }
        public required RouteColor Color { get; init; }
        public required HoldDetection[] Holds { get; init; }
    }

    [PublicAPI]
    public sealed record HoldDetection
    {
        public required string Segmentation { get; init; }
        public required uint X { get; init; }
        public required uint Y { get; init; }
    }
}

[PublicAPI]
internal sealed record SetBoulderingRouteResponse
{
    public required string RouteCode { get; init; }
}

internal sealed class SetBoulderingRouteValidator : Validator<SetBoulderingRouteRequest>
{
    public SetBoulderingRouteValidator()
    {
        RuleFor(x => x.Route).NotNull();
        RuleFor(x => x.RoutePicture).NotNull();
    }
}

internal sealed class SetBoulderingRoute(AppDbContext dbContext)
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
        var gym = await dbContext.Set<Gym>().FindAsync([req.Route.GymId], ct);

        if (gym is null)
        {
            ThrowError($"Gym was not found with id '{req.Route.GymId}'");
            return;
        }

        var usedRoutesIndexes = await (
            from route in dbContext.Set<BoulderingRoute>()
            where route.GymId == req.Route.GymId
            select route.Index).ToArrayAsync(ct);

        if (usedRoutesIndexes.Length >= BoulderingRouteCode.MaxUniqueCodes)
        {
            ThrowError($"Max route limit for this gym is reached ({BoulderingRouteCode.MaxUniqueCodes})");
            return;
        }

        var routeColor = req.Route.Color switch
        {
            SetBoulderingRouteRequest.RouteColor.Yellow => BoulderingRouteColor.Yellow,
            _ => throw new ArgumentOutOfRangeException()
        };

        using var pictureStream = new MemoryStream();
        await req.RoutePicture.CopyToAsync(pictureStream, ct);
        pictureStream.Position = 0;

        var image = await Image.LoadAsync(pictureStream, ct);

        var boulderingRoute = BoulderingRoute.Set(
            gym.Id,
            gym.Code.Value,
            usedRoutesIndexes,
            routeColor,
            new OriginalPicture
            {
                Data = pictureStream.ToArray(),
                OriginalWidth = (uint)image.Width,
                OriginalHeight = (uint)image.Height
            });

        await Task.WhenAll(
            req.Route.Holds
                .Select(hold => AddHoldDetection(boulderingRoute, hold, ct)));

        await dbContext.Set<BoulderingRoute>().AddAsync(boulderingRoute, ct);
        await dbContext.SaveChangesAsync(ct);

        await SendAsync(
            new SetBoulderingRouteResponse
            {
                RouteCode = boulderingRoute.Code.Value
            },
            cancellation: ct);
    }

    private async Task AddHoldDetection(
        BoulderingRoute route,
        SetBoulderingRouteRequest.HoldDetection hold,
        CancellationToken ct)
    {
        using var stream = new MemoryStream();
        var segmentationFile = Files.GetFile(hold.Segmentation);

        if (segmentationFile is null)
        {
            ThrowError($"Segmentation file '{hold.Segmentation}' was not found");
            return;
        }

        await segmentationFile.CopyToAsync(stream, ct);
        route.AddHoldDetection(stream.ToArray(), hold.X, hold.Y);
    }
}