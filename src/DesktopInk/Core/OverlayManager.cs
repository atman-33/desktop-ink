using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using DesktopInk.Infrastructure;
using DesktopInk.Windows;
using Microsoft.Win32;

namespace DesktopInk.Core;

public sealed class OverlayManager : IDisposable
{
    private readonly List<IOverlayWindow> _overlays = new();
    private readonly Func<IReadOnlyList<MonitorEnumerator.MonitorInfo>> _monitorProvider;
    private readonly Func<MonitorEnumerator.MonitorInfo, IOverlayWindow> _overlayFactory;
    private OverlayMode _mode = OverlayMode.PassThrough;
    private bool _isTemporaryDrawMode;
    private bool _isDisposed;
    private PenColor _penColor = PenColor.Red;
    private Win32.Rect? _paletteMonitorBoundsPx;

    public event EventHandler<OverlayMode>? ModeChanged;
    public event EventHandler<PenColor>? PenColorChanged;

    public PenColor CurrentPenColor => _penColor;

    public OverlayManager()
        : this(MonitorEnumerator.GetMonitors, monitor => new OverlayWindow(monitor.BoundsPx, monitor.DpiX, monitor.DpiY))
    {
    }

    internal OverlayManager(
        Func<IReadOnlyList<MonitorEnumerator.MonitorInfo>> monitorProvider,
        Func<MonitorEnumerator.MonitorInfo, IOverlayWindow> overlayFactory)
    {
        _monitorProvider = monitorProvider;
        _overlayFactory = overlayFactory;
    }

    public void ShowOverlays()
    {
        if (_overlays.Count == 0)
        {
            SystemEvents.DisplaySettingsChanged += OnDisplaySettingsChanged;
        }

        RefreshOverlays();
    }

    private void OnDisplaySettingsChanged(object? sender, EventArgs e)
    {
        if (_isDisposed)
        {
            return;
        }

        _ = System.Windows.Application.Current.Dispatcher.BeginInvoke(RefreshOverlays);
    }

    private void RefreshOverlays()
    {
        if (_isDisposed)
        {
            return;
        }

        IReadOnlyList<MonitorEnumerator.MonitorInfo> monitors;

        try
        {
            monitors = _monitorProvider();
            AppLog.Info($"RefreshOverlays monitors={monitors.Count} mode={_mode}");
            foreach (var m in monitors)
            {
                AppLog.Info($"Monitor hmon=0x{m.HMonitor.ToInt64():X} boundsPx=({m.BoundsPx.Left},{m.BoundsPx.Top}) {m.BoundsPx.Width}x{m.BoundsPx.Height} dpi=({m.DpiX},{m.DpiY})");
            }
        }
        catch (Exception ex)
        {
            AppLog.Error("RefreshOverlays monitor enumeration failed", ex);
            return;
        }

        foreach (var overlay in _overlays.ToList())
        {
            overlay.Close();
        }

        _overlays.Clear();

        foreach (var monitor in monitors)
        {
            var overlay = _overlayFactory(monitor);
            overlay.SetPenColor(_penColor);
            overlay.Show();
            _overlays.Add(overlay);
        }

        ApplyModeToOverlays();
    }

    public void ToggleMode(Win32.Rect? paletteMonitorBoundsPx = null)
    {
        SetMode(_mode == OverlayMode.PassThrough ? OverlayMode.Draw : OverlayMode.PassThrough, paletteMonitorBoundsPx);
    }

    public void SetMode(OverlayMode mode, Win32.Rect? paletteMonitorBoundsPx = null)
    {
        _mode = mode;

        if (paletteMonitorBoundsPx.HasValue)
        {
            _paletteMonitorBoundsPx = paletteMonitorBoundsPx.Value;
        }

        ApplyModeToOverlays();

        ModeChanged?.Invoke(this, GetEffectiveMode());
    }

    public void ActivateTemporaryDrawMode(Win32.Rect? paletteMonitorBoundsPx = null)
    {
        if (_isTemporaryDrawMode)
        {
            return;
        }

        _isTemporaryDrawMode = true;

        if (paletteMonitorBoundsPx.HasValue)
        {
            _paletteMonitorBoundsPx = paletteMonitorBoundsPx.Value;
        }

        AppLog.Info("OverlayManager: Temporary draw mode activated.");

        ApplyModeToOverlays();

        ModeChanged?.Invoke(this, OverlayMode.Draw);
    }

    public void DeactivateTemporaryDrawMode(Win32.Rect? paletteMonitorBoundsPx = null)
    {
        if (!_isTemporaryDrawMode)
        {
            return;
        }

        _isTemporaryDrawMode = false;

        if (paletteMonitorBoundsPx.HasValue)
        {
            _paletteMonitorBoundsPx = paletteMonitorBoundsPx.Value;
        }

        AppLog.Info("OverlayManager: Temporary draw mode deactivated.");

        ClearPaletteMonitor();
        ApplyModeToOverlays();

        ModeChanged?.Invoke(this, _mode);
    }

    public void UpdatePaletteMonitor(Win32.Rect boundsPx)
    {
        if (_paletteMonitorBoundsPx.HasValue && AreBoundsEqual(_paletteMonitorBoundsPx.Value, boundsPx))
        {
            return;
        }

        _paletteMonitorBoundsPx = boundsPx;

        if (_mode == OverlayMode.Draw || _isTemporaryDrawMode)
        {
            ApplyModeToOverlays();
        }
    }

    private void ApplyModeToOverlays()
    {
        if (_isTemporaryDrawMode)
        {
            ApplyDrawModeToPaletteOverlay(isTemporary: true);
            return;
        }

        if (_mode == OverlayMode.Draw)
        {
            ApplyDrawModeToPaletteOverlay(isTemporary: false);
            return;
        }

        SetAllPassThrough();
    }

    private void ApplyDrawModeToPaletteOverlay(bool isTemporary)
    {
        if (!_paletteMonitorBoundsPx.HasValue)
        {
            AppLog.Info("OverlayManager: Palette monitor unknown; forcing pass-through on all overlays.");
            SetAllPassThrough();
            return;
        }

        var paletteBounds = _paletteMonitorBoundsPx.Value;
        var matchedOverlay = false;

        foreach (var overlay in _overlays.ToList())
        {
            if (AreBoundsEqual(overlay.MonitorBoundsPx, paletteBounds))
            {
                overlay.SetMode(OverlayMode.Draw, isTemporary);
                matchedOverlay = true;
            }
            else
            {
                overlay.SetMode(OverlayMode.PassThrough, isTemporary: false);
            }
        }

        if (!matchedOverlay)
        {
            AppLog.Info(
                $"OverlayManager: No overlay matched palette bounds ({paletteBounds.Left},{paletteBounds.Top}) {paletteBounds.Width}x{paletteBounds.Height}.");
        }
    }

    private void SetAllPassThrough()
    {
        foreach (var overlay in _overlays.ToList())
        {
            overlay.SetMode(OverlayMode.PassThrough, isTemporary: false);
        }
    }

    private void ClearPaletteMonitor()
    {
        if (!_paletteMonitorBoundsPx.HasValue)
        {
            return;
        }

        var paletteBounds = _paletteMonitorBoundsPx.Value;

        foreach (var overlay in _overlays.ToList().Where(overlay => AreBoundsEqual(overlay.MonitorBoundsPx, paletteBounds)))
        {
            overlay.ClearAll();
        }
    }

    private static bool AreBoundsEqual(Win32.Rect left, Win32.Rect right)
    {
        return left.Left == right.Left
            && left.Top == right.Top
            && left.Right == right.Right
            && left.Bottom == right.Bottom;
    }

    private OverlayMode GetEffectiveMode()
    {
        return _isTemporaryDrawMode ? OverlayMode.Draw : _mode;
    }

    public void ClearAll()
    {
        foreach (var overlay in _overlays.ToList())
        {
            overlay.ClearAll();
        }
    }

    public void CycleColor()
    {
        var nextColor = _penColor switch
        {
            PenColor.Red => PenColor.Blue,
            PenColor.Blue => PenColor.Green,
            PenColor.Green => PenColor.Red,
            _ => PenColor.Red,
        };

        SetPenColor(nextColor);
    }

    private void SetPenColor(PenColor color)
    {
        if (_penColor == color)
        {
            return;
        }

        _penColor = color;

        foreach (var overlay in _overlays.ToList())
        {
            overlay.SetPenColor(_penColor);
        }

        PenColorChanged?.Invoke(this, _penColor);
    }

    public void Quit()
    {
        Dispose();
        System.Windows.Application.Current.Shutdown();
    }

    public void Dispose()
    {
        if (_isDisposed)
        {
            return;
        }

        _isDisposed = true;

        SystemEvents.DisplaySettingsChanged -= OnDisplaySettingsChanged;

        foreach (var overlay in _overlays.ToList())
        {
            overlay.Close();
        }

        _overlays.Clear();
    }
}
