using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Domain.Gyms;

public sealed class Gym
{
    private Gym(Guid id, GymCode code, GymName name)
    {
        Id = id;
        Code = code;
        Name = name;
    }

    public Guid Id { get; }
    public GymCode Code { get; }
    public GymName Name { get; }

    public static Gym EnrollNew(string code, string name)
    {
        return new Gym(Guid.CreateVersion7(), new GymCode(code), new GymName(name));
    }
}

internal sealed class GymConfiguration : IEntityConfiguration<Gym>
{
    public void Configure(EntityTypeBuilder<Gym> builder)
    {
        builder.HasKey(gym => gym.Id);
        builder.HasIndex(gym => gym.Code).IsUnique();

        builder.Property(gym => gym.Code)
            .HasConversion(
                code => code.Value,
                code => new GymCode(code))
            .HasMaxLength(4);

        builder.Property(gym => gym.Name)
            .HasConversion(
                name => name.Value,
                code => new GymName(code))
            .HasMaxLength(30);
    }
}