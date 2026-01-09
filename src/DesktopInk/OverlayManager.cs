using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace DesktopInk;

public sealed class OverlayManager : IDisposable
{
    private readonly List<OverlayWindow> _overlays = new();
    private OverlayMode _mode = OverlayMode.PassThrough;

    public void ShowOverlays()
    {
        if (_overlays.Count > 0)
        {
            return;
        }

        // Use WinForms Screen enumeration for multi-monitor bounds.
        foreach (var screen in System.Windows.Forms.Screen.AllScreens)
        {
            var bounds = screen.Bounds;
            var overlay = new OverlayWindow(bounds.Left, bounds.Top, bounds.Width, bounds.Height);
            overlay.SetMode(_mode);
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
            overlay.SetMode(_mode);
        }
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
        foreach (var overlay in _overlays.ToList())
        {
            overlay.Close();
        }

        _overlays.Clear();
    }
}
