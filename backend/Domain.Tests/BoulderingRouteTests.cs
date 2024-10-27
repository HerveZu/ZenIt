using Domain.BoulderingRoutes;

namespace Domain.Tests;

internal sealed class BoulderingRouteTests
{
    [TestCase("MGYM", 0, "MGYM-AA")]
    [TestCase("MGYM", 25, "MGYM-AZ")]
    [TestCase("MGYM", 25 + 26, "MGYM-BZ")]
    [TestCase("MGYM", 26 * 26 - 1, "MGYM-ZZ")]
    public void GenerateBoulderingRouteCode__ShouldConvertIndexToBase26(string gymCode, int index, string expectedCode)
    {
        var generatedCode = BoulderingRouteCode.Generate(gymCode, (uint)index);
        
        Assert.That(generatedCode.Value, Is.EqualTo(expectedCode));
    }
    
    [TestCase(26 * 26)]
    public void GenerateBoulderingRouteCode__WhenIndexOutsideBounds__ShouldThrow(int index)
    {
        Assert.Throws<InvalidOperationException>(
            () => BoulderingRouteCode.Generate("MGYM", (uint)index));
    }
}