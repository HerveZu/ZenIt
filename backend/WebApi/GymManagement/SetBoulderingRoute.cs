using Domain.BoulderingRoutes;
using Domain.Gyms;
using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using WebApi.Common.Infrastructure;

namespace WebApi.GymManagement;

[Serializable]
internal sealed record SetBoulderingRouteRequest
{
    public required Guid GymId { get; init; }
}

[Serializable]
internal sealed record SetBoulderingRouteResponse
{
    public required string RouteCode { get; init; }
}

internal sealed class SetBoulderingRoute(AppDbContext dbContext)
    : Endpoint<SetBoulderingRouteRequest, SetBoulderingRouteResponse>
{
    public override void Configure()
    {
        Post("/routes/set");
        AllowAnonymous();
    }

    public override async Task HandleAsync(SetBoulderingRouteRequest req, CancellationToken ct)
    {
        var gym = await dbContext.Set<Gym>().FindAsync([req.GymId], ct);

        if (gym is null)
        {
            await SendNotFoundAsync(ct);
            return;
        }

        var usedRoutesIndexes = await (
            from route in dbContext.Set<BoulderingRoute>()
            where route.GymId == req.GymId
            select route.Index).ToArrayAsync(ct);

        if (usedRoutesIndexes.Length >= BoulderingRouteCode.MaxUniqueCodes)
        {
            ThrowError($"Max route limit for this gym is reached ({BoulderingRouteCode.MaxUniqueCodes})");
            return;
        }

        var boulderingRoute = BoulderingRoute.Set(
            gym.Id,
            gym.Code.Value,
            usedRoutesIndexes);

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