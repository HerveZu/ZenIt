using Domain.Common;

namespace Domain.Tests;

internal sealed class PercentTests
{
    [TestCase(0)]
    [TestCase(1)]
    [TestCase(0.3)]
    public void Percent__WhenValid__ShouldNotThrow(double value)
    {
        _ = new Percent(value);
    }

    [TestCase(-0.1)]
    [TestCase(1.1)]
    [TestCase(42)]
    public void Percent__WhenInvalid__ShouldThrow(double value)
    {
        Assert.Throws<InvalidOperationException>(() => _ = new Percent(value));
    }
}