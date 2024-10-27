using System.Text;

namespace Domain.BoulderingRoutes;

public sealed record BoulderingRouteCode(string Value)
{
    // no numbers to avoid route codes that could be confused with grades (7A, 7B, etc...)
    private const string AvailableChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
    
    // should be kept low to not having too much to write down on start markers
    private const int CharsCount = 2;

    public static double MaxUniqueCodes => Math.Pow(AvailableChars.Length, CharsCount) - 1;

    public static BoulderingRouteCode Generate(string gymCode, uint index)
    {
        if (index > MaxUniqueCodes)
        {
            throw new InvalidOperationException(
                $"Index {index} base {AvailableChars.Length} > allowed chars ({MaxUniqueCodes})");
        }

        var codeBuilder = new StringBuilder();
        var baseExp = CharsCount - 1;

        while (baseExp >= 0)
        {
            var @base = (uint)Math.Pow(AvailableChars.Length, baseExp);
            var currentIndex = (index - index % @base) / @base;
            index -= currentIndex * @base;
            baseExp--;

            codeBuilder.Append(AvailableChars[(int)currentIndex]);
        }

        return new BoulderingRouteCode($"{gymCode}-{codeBuilder}");
    }
}