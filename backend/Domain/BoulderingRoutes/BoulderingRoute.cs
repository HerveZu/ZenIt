using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Domain.BoulderingRoutes;

public sealed class BoulderingRoute
{
    private BoulderingRoute(
        Guid id,
        Guid gymId,
        BoulderingRouteCode code,
        uint index,
        BoulderingRouteColor color,
        byte[]? originalPicture,
        byte[]? maskedPicture)
    {
        Id = id;
        GymId = gymId;
        Code = code;
        Index = index;
        Color = color;
        OriginalPicture = originalPicture;
        MaskedPicture = maskedPicture;
    }

    public Guid Id { get; init; }
    public Guid GymId { get; init; }
    public BoulderingRouteCode Code { get; init; }
    public uint Index { get; init; }
    public BoulderingRouteColor Color { get; init; }
    public byte[]? OriginalPicture { get; private set; }
    public byte[]? MaskedPicture { get; private set; }
    public IReadOnlyCollection<HoldDetection> DetectedHolds { get; private set; } = [];

    public static BoulderingRoute Set(
        Guid gymId,
        string gymCode,
        uint[] usedRoutesIndexes,
        BoulderingRouteColor color)
    {
        var routeIndex = BoulderingRouteCode.NextAvailableIndex(usedRoutesIndexes);

        return new BoulderingRoute(
            Guid.CreateVersion7(),
            gymId,
            BoulderingRouteCode.Generate(gymCode, routeIndex),
            routeIndex,
            color,
            null,
            null);
    }

    public void UsePicture(IRouteAnalyser analyser, byte[] picture)
    {
        OriginalPicture = picture;
        (MaskedPicture, DetectedHolds) = analyser.DetectHolds(OriginalPicture, Color);
    }
}

internal sealed class BoulderingRouteConfiguration : IEntityConfiguration<BoulderingRoute>
{
    public void Configure(EntityTypeBuilder<BoulderingRoute> builder)
    {
        builder.HasKey(route => route.Id);
        builder.HasIndex(route => new { route.GymId, route.Code }).IsUnique();
        builder.HasIndex(route => new { route.GymId, route.Index }).IsUnique();

        builder.Property(route => route.Color).HasConversion<string>();
        builder.OwnsMany<HoldDetection>(route => route.DetectedHolds);

        builder
            .Property(route => route.Code)
            .HasConversion(
                code => code.Value,
                code => new BoulderingRouteCode(code))
            .HasMaxLength(8);
    }
}