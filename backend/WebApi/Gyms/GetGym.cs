using Domain.Gyms;
using FastEndpoints;
using WebApi.Common.Infrastructure;
using WebApi.Gyms.Contracts;

namespace WebApi.Gyms;

[Serializable]
internal sealed record GetGymRequest
{
    public required Guid GymId { get; init; }
}

internal sealed class GetGym(AppDbContext dbContext) : Endpoint<GetGymRequest, GymDto>
{
    public override void Configure()
    {
        Get("/gyms/{GymId:guid}");
        AllowAnonymous();
    }

    public override async Task HandleAsync(GetGymRequest req, CancellationToken ct)
    {
        var gym = await dbContext.Set<Gym>().FindAsync([req.GymId], ct);

        if (gym is null)
        {
            await SendNotFoundAsync(ct);
            return;
        }
        
        await SendAsync(gym.ToDto(), cancellation: ct);
    }
}