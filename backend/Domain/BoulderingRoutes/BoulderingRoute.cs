using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Domain.BoulderingRoutes;

public sealed class BoulderingRoute
{
    private BoulderingRoute(Guid id, Guid gymId, BoulderingRouteCode code, uint index)
    {
        Id = id;
        GymId = gymId;
        Code = code;
        Index = index;
    }

    public Guid Id { get; init; }
    public Guid GymId { get; init; }
    public BoulderingRouteCode Code { get; init; }
    public uint Index { get; init; }

    public static BoulderingRoute Set(Guid gymId, string gymCode, uint routeIndex)
    {
        return new BoulderingRoute(
            Guid.CreateVersion7(),
            gymId,
            BoulderingRouteCode.Generate(gymCode, routeIndex),
            routeIndex);
    }
}

internal sealed class BoulderingRouteConfiguration : IEntityConfiguration<BoulderingRoute>
{
    public void Configure(EntityTypeBuilder<BoulderingRoute> builder)
    {
        builder.HasKey(route => route.Id);
        builder.HasIndex(route => new { route.GymId, route.Code }).IsUnique();
        builder.HasIndex(route => new { route.GymId, route.Index }).IsUnique();

        builder
            .Property(route => route.Code)
            .HasConversion(
                code => code.Value, 
                code => new BoulderingRouteCode(code))
            .HasMaxLength(8);
    }
}