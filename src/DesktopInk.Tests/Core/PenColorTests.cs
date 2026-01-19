using DesktopInk.Core;
using FluentAssertions;
using Xunit;

namespace DesktopInk.Tests.Core;

public class PenColorTests
{
    [Fact]
    public void PenColor_ShouldHaveExpectedValues()
    {
        var values = Enum.GetValues<PenColor>();

        values.Should().HaveCount(3);
        values.Should().Contain(PenColor.Red);
        values.Should().Contain(PenColor.Blue);
        values.Should().Contain(PenColor.Green);
    }

    [Theory]
    [InlineData(PenColor.Red, 0)]
    [InlineData(PenColor.Blue, 1)]
    [InlineData(PenColor.Green, 2)]
    public void PenColor_ShouldHaveStableOrder(PenColor color, int expectedValue)
    {
        ((int)color).Should().Be(expectedValue);
    }
}
