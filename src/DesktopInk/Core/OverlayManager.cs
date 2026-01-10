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
    private readonly List<OverlayWindow> _overlays = new();
    private OverlayMode _mode = OverlayMode.PassThrough;
    private bool _isTemporaryDrawMode;
    private bool _isDisposed;

    public event EventHandler<OverlayMode>? ModeChanged;

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

        // Ensure we refresh on the UI thread.
        _ = System.Windows.Application.Current.Dispatcher.BeginInvoke(RefreshOverlays);
    }

    private void RefreshOverlays()
    {
        if (_isDisposed)
        {
            return;
        }

        try
        {
            var monitors = MonitorEnumerator.GetMonitors();
            AppLog.Info($"RefreshOverlays monitors={monitors.Count} mode={_mode}");
            foreach (var m in monitors)
            {
                AppLog.Info($"Monitor hmon=0x{m.HMonitor.ToInt64():X} boundsPx=({m.BoundsPx.Left},{m.BoundsPx.Top}) {m.BoundsPx.Width}x{m.BoundsPx.Height} dpi=({m.DpiX},{m.DpiY})");
            }
        }
        catch (Exception ex)
        {
            AppLog.Error("RefreshOverlays monitor enumeration failed", ex);
        }

        // Simple, robust approach: rebuild overlays from current OS monitor topology.
        foreach (var overlay in _overlays.ToList())
        {
            overlay.Close();
        }

        _overlays.Clear();

        foreach (var monitor in MonitorEnumerator.GetMonitors())
        {
            var overlay = new OverlayWindow(monitor.BoundsPx, monitor.DpiX, monitor.DpiY);
            overlay.SetMode(GetEffectiveMode(), _isTemporaryDrawMode);
            overlay.Show();
            _overlays.Add(overlay);
        }
    }

    public void ToggleMode()
    {
        SetMode(_mode == OverlayMode.PassThrough ? OverlayMode.Draw : OverlayMode.PassThrough);
    }

    public void SetMode(OverlayMode mode)
    {
        _mode = mode;

        foreach (var overlay in _overlays.ToList())
        {
            overlay.SetMode(GetEffectiveMode(), _isTemporaryDrawMode);
        }

        ModeChanged?.Invoke(this, GetEffectiveMode());
    }

    public void ActivateTemporaryDrawMode()
    {
        if (_isTemporaryDrawMode)
        {
            return;
        }

        _isTemporaryDrawMode = true;
        AppLog.Info("OverlayManager: Temporary draw mode activated.");

        foreach (var overlay in _overlays.ToList())
        {
            overlay.SetMode(OverlayMode.Draw, isTemporary: true);
        }

        ModeChanged?.Invoke(this, OverlayMode.Draw);
    }

    public void DeactivateTemporaryDrawMode()
    {
        if (!_isTemporaryDrawMode)
        {
            return;
        }

        _isTemporaryDrawMode = false;
        AppLog.Info("OverlayManager: Temporary draw mode deactivated.");

        // Clear all strokes when exiting temporary mode
        ClearAll();

        // Return to the permanent mode state
        foreach (var overlay in _overlays.ToList())
        {
            overlay.SetMode(_mode, isTemporary: false);
        }

        ModeChanged?.Invoke(this, _mode);
    }

    private OverlayMode GetEffectiveMode()
    {
        // Temporary mode takes precedence
        return _isTemporaryDrawMode ? OverlayMode.Draw : _mode;
    }

    public void ClearAll()
    {
        foreach (var overlay in _overlays.ToList())
        {
            overlay.ClearAll();
        }
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
