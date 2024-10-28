using Domain.Gyms;
using FastEndpoints;
using WebApi.Common.Infrastructure;
using WebApi.GymManagement.Contracts;

namespace WebApi.GymManagement;

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
            ThrowError($"Gym was not found with id '{req.GymId}'");
            return;
        }

        await SendAsync(gym.ToDto(), cancellation: ct);
    }
}