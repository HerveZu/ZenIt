using System.Text.RegularExpressions;

namespace Domain.Gyms;

public sealed partial record GymCode
{
    public GymCode(string code)
    {
        Value = code.ToUpper();
        
        if (!Regex().IsMatch(Value))
        {
            throw new InvalidOperationException($"Invalid gym code, was '{code}'");
        }
    }

    public string Value { get; }

    public static implicit operator string(GymCode code)
    {
        return code.Value;
    }
    
    [GeneratedRegex("^[A-Z]{4}$")]
    private static partial Regex Regex();
}