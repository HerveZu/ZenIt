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

    [Test]
    public void NextAvailableRouteCode__WhenGap__ShouldReturnFirstNextAvailable()
    {
        var nextAvailableIndex = BoulderingRouteCode.NextAvailableIndex([0, 1, 2, 5, 6]);
        Assert.That(nextAvailableIndex, Is.EqualTo(3));
    }

    [Test]
    public void NextAvailableRouteCode__WhenNoGap__ShouldReturnNextIndex()
    {
        var nextAvailableIndex = BoulderingRouteCode.NextAvailableIndex([0, 1, 2]);
        Assert.That(nextAvailableIndex, Is.EqualTo(3));
    }
}