using Domain.Common;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Domain.BoulderingRoutes;

public sealed class BoulderingRoute
{
    private BoulderingRoute(
        Guid id,
        Guid gymId,
        BoulderingRouteCode code,
        uint index,
        BoulderingRouteColor color)
    {
        Id = id;
        GymId = gymId;
        Code = code;
        Index = index;
        Color = color;
    }

    public Guid Id { get; init; }
    public Guid GymId { get; init; }
    public BoulderingRouteCode Code { get; init; }
    public uint Index { get; init; }
    public BoulderingRouteColor Color { get; init; }
    public required OriginalPicture OriginalPicture { get; init; }
    public IReadOnlyCollection<RouteHold> DetectedHolds { get; private set; } = [];

    public static BoulderingRoute Set(
        Guid gymId,
        string gymCode,
        uint[] usedRoutesIndexes,
        BoulderingRouteColor color,
        OriginalPicture originalPicture)
    {
        var routeIndex = BoulderingRouteCode.NextAvailableIndex(usedRoutesIndexes);

        return new BoulderingRoute(
            Guid.CreateVersion7(),
            gymId,
            BoulderingRouteCode.Generate(gymCode, routeIndex),
            routeIndex,
            color)
        {
            OriginalPicture = originalPicture
        };
    }

    public void AddHoldDetection(byte[] segmentedPicture, uint x, uint y)
    {
        var hold = new RouteHold
        {
            SegmentedPicture = segmentedPicture,
            X = new Percent(x / (double)OriginalPicture.OriginalWidth),
            Y = new Percent(y / (double)OriginalPicture.OriginalHeight)
        };

        DetectedHolds = DetectedHolds.Append(hold).ToArray();
    }
}

public sealed record OriginalPicture
{
    public required byte[] Data { get; init; }
    public required uint OriginalWidth { get; init; }
    public required uint OriginalHeight { get; init; }
}

public sealed record RouteHold
{
    public required byte[] SegmentedPicture { get; init; }
    public required Percent X { get; init; }
    public required Percent Y { get; init; }
}

internal sealed class BoulderingRouteConfiguration : IEntityConfiguration<BoulderingRoute>
{
    public void Configure(EntityTypeBuilder<BoulderingRoute> builder)
    {
        builder.HasKey(route => route.Id);
        builder.HasIndex(route => new { route.GymId, route.Code }).IsUnique();
        builder.HasIndex(route => new { route.GymId, route.Index }).IsUnique();

        builder.Property(route => route.Color).HasConversion<string>();
        builder.ComplexProperty(route => route.OriginalPicture);

        var routeHoldBuilder = builder.OwnsMany<RouteHold>(route => route.DetectedHolds);

        routeHoldBuilder
            .Property(hold => hold.X)
            .HasConversion(percent => percent.Value, percent => new Percent(percent));

        routeHoldBuilder
            .Property(hold => hold.Y)
            .HasConversion(percent => percent.Value, percent => new Percent(percent));

        builder
            .Property(route => route.Code)
            .HasConversion(
                code => code.Value,
                code => new BoulderingRouteCode(code))
            .HasMaxLength(8);
    }
}