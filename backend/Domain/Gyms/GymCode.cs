using System.Text.RegularExpressions;

namespace Domain.Gyms;

public sealed record GymCode
{
    private readonly Regex _regex = new("^[A-Z]{4}$");

    public GymCode(string code)
    {
        Value = code.ToUpper();

        if (!_regex.IsMatch(Value))
        {
            throw new InvalidOperationException($"Invalid gym code, was '{code}'");
        }
    }

    public string Value { get; }
}