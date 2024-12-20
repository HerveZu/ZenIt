using Domain.Gyms;
using JetBrains.Annotations;

namespace WebApi.GymManagement.Contracts;

[PublicAPI]
internal sealed record GymDto
{
    public required Guid Id { get; init; }
    public required string Code { get; init; }
    public required string Name { get; init; }
}

internal static class GymDtoMapping
{
    public static GymDto ToDto(this Gym gym)
    {
        return new GymDto
        {
            Id = gym.Id,
            Code = gym.Code.Value,
            Name = gym.Name.Value
        };
    }
}