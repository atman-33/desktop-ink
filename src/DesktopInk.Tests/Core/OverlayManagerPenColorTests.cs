using DesktopInk.Core;
using FluentAssertions;
using Xunit;

namespace DesktopInk.Tests.Core;

public class OverlayManagerPenColorTests
{
    [Fact]
    public void OverlayManager_ShouldDefaultToRed()
    {
        using var manager = new OverlayManager();

        manager.CurrentPenColor.Should().Be(PenColor.Red);
    }

    [Fact]
    public void OverlayManager_ShouldCycleColorsInOrder()
    {
        using var manager = new OverlayManager();

        manager.CycleColor();
        manager.CurrentPenColor.Should().Be(PenColor.Blue);

        manager.CycleColor();
        manager.CurrentPenColor.Should().Be(PenColor.Green);

        manager.CycleColor();
        manager.CurrentPenColor.Should().Be(PenColor.Red);
    }
}
