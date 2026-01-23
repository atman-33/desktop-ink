using System;
using System.Collections.Generic;
using System.Linq;
using DesktopInk.Core;
using DesktopInk.Infrastructure;
using FluentAssertions;
using Xunit;

namespace DesktopInk.Tests.Core;

public class OverlayManagerPerMonitorTests
{
    [Fact]
    public void ToggleMode_ShouldEnableDrawOnlyOnPaletteMonitor()
    {
        var boundsA = new Win32.Rect { Left = 0, Top = 0, Right = 1920, Bottom = 1080 };
        var boundsB = new Win32.Rect { Left = 1920, Top = 0, Right = 3840, Bottom = 1080 };
        var monitors = CreateMonitors(boundsA, boundsB);
        var overlays = new List<TestOverlayWindow>();

        using var manager = CreateManager(monitors, overlays);
        manager.ShowOverlays();

        UpdatePaletteMonitor(manager, boundsA);
        SetMode(manager, OverlayMode.Draw, boundsA);

        overlays.Single(o => o.MonitorBoundsPx.Equals(boundsA)).Mode.Should().Be(OverlayMode.Draw);
        overlays.Single(o => o.MonitorBoundsPx.Equals(boundsB)).Mode.Should().Be(OverlayMode.PassThrough);
    }

    [Fact]
    public void PaletteMove_ShouldMoveDrawModeToNewMonitor()
    {
        var boundsA = new Win32.Rect { Left = 0, Top = 0, Right = 1920, Bottom = 1080 };
        var boundsB = new Win32.Rect { Left = 1920, Top = 0, Right = 3840, Bottom = 1080 };
        var monitors = CreateMonitors(boundsA, boundsB);
        var overlays = new List<TestOverlayWindow>();

        using var manager = CreateManager(monitors, overlays);
        manager.ShowOverlays();

        UpdatePaletteMonitor(manager, boundsA);
        SetMode(manager, OverlayMode.Draw, boundsA);

        UpdatePaletteMonitor(manager, boundsB);

        overlays.Single(o => o.MonitorBoundsPx.Equals(boundsA)).Mode.Should().Be(OverlayMode.PassThrough);
        overlays.Single(o => o.MonitorBoundsPx.Equals(boundsB)).Mode.Should().Be(OverlayMode.Draw);
    }

    [Fact]
    public void TemporaryMode_ShouldOnlyAffectPaletteMonitor_AndClearOnExit()
    {
        var boundsA = new Win32.Rect { Left = 0, Top = 0, Right = 1920, Bottom = 1080 };
        var boundsB = new Win32.Rect { Left = 1920, Top = 0, Right = 3840, Bottom = 1080 };
        var monitors = CreateMonitors(boundsA, boundsB);
        var overlays = new List<TestOverlayWindow>();

        using var manager = CreateManager(monitors, overlays);
        manager.ShowOverlays();

        UpdatePaletteMonitor(manager, boundsA);
        ActivateTemporaryDrawMode(manager, boundsA);

        overlays.Single(o => o.MonitorBoundsPx.Equals(boundsA)).Mode.Should().Be(OverlayMode.Draw);
        overlays.Single(o => o.MonitorBoundsPx.Equals(boundsA)).IsTemporary.Should().BeTrue();
        overlays.Single(o => o.MonitorBoundsPx.Equals(boundsB)).Mode.Should().Be(OverlayMode.PassThrough);

        DeactivateTemporaryDrawMode(manager, boundsA);

        overlays.Single(o => o.MonitorBoundsPx.Equals(boundsA)).Mode.Should().Be(OverlayMode.PassThrough);
        overlays.Single(o => o.MonitorBoundsPx.Equals(boundsB)).Mode.Should().Be(OverlayMode.PassThrough);
        overlays.Single(o => o.MonitorBoundsPx.Equals(boundsA)).ClearAllCallCount.Should().Be(1);
        overlays.Single(o => o.MonitorBoundsPx.Equals(boundsB)).ClearAllCallCount.Should().Be(0);
    }

    private static OverlayManager CreateManager(
        IReadOnlyList<MonitorEnumerator.MonitorInfo> monitors,
        List<TestOverlayWindow> overlays)
    {
        var monitorProvider = new Func<IReadOnlyList<MonitorEnumerator.MonitorInfo>>(() => monitors);
        var overlayFactory = new Func<MonitorEnumerator.MonitorInfo, IOverlayWindow>(monitor =>
        {
            var overlay = new TestOverlayWindow(monitor.BoundsPx);
            overlays.Add(overlay);
            return overlay;
        });

        return (OverlayManager)Activator.CreateInstance(
            typeof(OverlayManager),
            System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public,
            null,
            new object[] { monitorProvider, overlayFactory },
            null)!;
    }

    private static IReadOnlyList<MonitorEnumerator.MonitorInfo> CreateMonitors(params Win32.Rect[] bounds)
    {
        return bounds
            .Select((rect, index) => new MonitorEnumerator.MonitorInfo(new IntPtr(index + 1), rect, 96, 96))
            .ToList();
    }

    private sealed class TestOverlayWindow : IOverlayWindow
    {
        public TestOverlayWindow(Win32.Rect boundsPx)
        {
            MonitorBoundsPx = boundsPx;
            Mode = OverlayMode.PassThrough;
        }

        public Win32.Rect MonitorBoundsPx { get; }

        public OverlayMode Mode { get; private set; }

        public bool IsTemporary { get; private set; }

        public int ClearAllCallCount { get; private set; }

        public void SetMode(OverlayMode mode, bool isTemporary = false)
        {
            Mode = mode;
            IsTemporary = isTemporary;
        }

        public void ClearAll()
        {
            ClearAllCallCount++;
        }

        public void SetPenColor(PenColor color)
        {
        }

        public void Show()
        {
        }

        public void Close()
        {
        }
    }

    private static void UpdatePaletteMonitor(OverlayManager manager, Win32.Rect boundsPx)
    {
        InvokeManager(manager, nameof(UpdatePaletteMonitor), boundsPx);
    }

    private static void SetMode(OverlayManager manager, OverlayMode mode, Win32.Rect boundsPx)
    {
        InvokeManager(manager, nameof(SetMode), mode, (Win32.Rect?)boundsPx);
    }

    private static void ActivateTemporaryDrawMode(OverlayManager manager, Win32.Rect boundsPx)
    {
        InvokeManager(manager, nameof(ActivateTemporaryDrawMode), (Win32.Rect?)boundsPx);
    }

    private static void DeactivateTemporaryDrawMode(OverlayManager manager, Win32.Rect boundsPx)
    {
        InvokeManager(manager, nameof(DeactivateTemporaryDrawMode), (Win32.Rect?)boundsPx);
    }

    private static void InvokeManager(OverlayManager manager, string methodName, params object?[] args)
    {
        var method = typeof(OverlayManager).GetMethod(methodName);
        if (method is null)
        {
            throw new InvalidOperationException($"OverlayManager method '{methodName}' was not found.");
        }

        method.Invoke(manager, args);
    }
}
