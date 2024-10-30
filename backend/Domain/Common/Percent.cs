namespace Domain.Common;

public record struct Percent
{
    public Percent(double value)
    {
        if (value is < 0 or > 1)
        {
            throw new InvalidOperationException("Percent value must be between 0 and 100.");
        }

        Value = value;
    }

    public double Value { get; }
}