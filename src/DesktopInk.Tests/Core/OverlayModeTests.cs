using DesktopInk.Core;
using FluentAssertions;
using Xunit;

namespace DesktopInk.Tests.Core;

public class OverlayModeTests
{
    [Fact]
    public void OverlayMode_ShouldHavePassThroughValue()
    {
        // Arrange & Act
        var mode = OverlayMode.PassThrough;

        // Assert
        mode.Should().Be(OverlayMode.PassThrough);
        ((int)mode).Should().Be(0);
    }

    [Fact]
    public void OverlayMode_ShouldHaveDrawValue()
    {
        // Arrange & Act
        var mode = OverlayMode.Draw;

        // Assert
        mode.Should().Be(OverlayMode.Draw);
        ((int)mode).Should().Be(1);
    }

    [Fact]
    public void OverlayMode_ShouldHaveOnlyTwoValues()
    {
        // Arrange & Act
        var values = Enum.GetValues<OverlayMode>();

        // Assert
        values.Should().HaveCount(2);
        values.Should().Contain(OverlayMode.PassThrough);
        values.Should().Contain(OverlayMode.Draw);
    }

    [Theory]
    [InlineData(OverlayMode.PassThrough, "PassThrough")]
    [InlineData(OverlayMode.Draw, "Draw")]
    public void OverlayMode_ToString_ShouldReturnCorrectName(OverlayMode mode, string expectedName)
    {
        // Act
        var result = mode.ToString();

        // Assert
        result.Should().Be(expectedName);
    }

    [Theory]
    [InlineData("PassThrough", OverlayMode.PassThrough)]
    [InlineData("Draw", OverlayMode.Draw)]
    public void OverlayMode_Parse_ShouldReturnCorrectValue(string input, OverlayMode expected)
    {
        // Act
        var result = Enum.Parse<OverlayMode>(input);

        // Assert
        result.Should().Be(expected);
    }
}
