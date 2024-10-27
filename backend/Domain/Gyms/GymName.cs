namespace Domain.Gyms;

public sealed record GymName
{
    private readonly Range _lengthRange = 3..30;

    public GymName(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new InvalidOperationException("Gym name should not be blank");
        }

        if (value.Length < _lengthRange.Start.Value || value.Length > _lengthRange.End.Value)
        {
            throw new InvalidOperationException($"Gym name length should within {_lengthRange}");
        }

        Value = value;
    }

    public string Value { get; }
}