using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Runtime.InteropServices;
using DesktopInk.Core;
using DesktopInk.Infrastructure;

namespace DesktopInk.Windows;

public partial class ControlWindow : Window
{
    private const int HotkeyToggleDraw = 1;
    private const int HotkeyClearAll = 2;
    private const int HotkeyQuit = 3;
    private const double LogicalWidth = 56.0;
    private const double LogicalHeight = 204.0;

    private readonly OverlayManager _overlayManager;
    private readonly KeyboardHookManager _keyboardHook;

    private HwndSource? _hwndSource;
    private IntPtr _hwnd;
    private uint _dpiX = 96;
    private uint _dpiY = 96;

    public ControlWindow(OverlayManager overlayManager)
    {
        _overlayManager = overlayManager;
        _keyboardHook = new KeyboardHookManager();

        InitializeComponent();

        // Subscribe to mode changes for visual feedback
        _overlayManager.ModeChanged += OnModeChanged;
        _overlayManager.PenColorChanged += OnPenColorChanged;

        UpdateColorCycleButton(_overlayManager.CurrentPenColor);

        SourceInitialized += OnSourceInitialized;
        Closed += OnClosed;

        MouseLeftButtonDown += (_, e) =>
        {
            if (e.ButtonState == MouseButtonState.Pressed)
            {
                DragMove();
            }
        };
    }

    private void OnSourceInitialized(object? sender, EventArgs e)
    {
        _hwnd = new WindowInteropHelper(this).Handle;

        _hwndSource = HwndSource.FromHwnd(_hwnd);
        _hwndSource.AddHook(WndProc);

        ApplyToolWindowStyle();
        var dpi = Win32.GetDpiForWindow(_hwnd);
        if (dpi != 0)
        {
            _dpiX = dpi;
            _dpiY = dpi;
        }
        PositionNearPrimaryRightEdge();

        if (!TryRegisterHotkeys())
        {
            System.Windows.MessageBox.Show(
                "Failed to register one or more global hotkeys. Another application may already be using them.",
                "DesktopInk",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }

        // Install keyboard hook for temporary draw mode
        try
        {
            _keyboardHook.TemporaryModeActivated += OnTemporaryModeActivated;
            _keyboardHook.TemporaryModeDeactivated += OnTemporaryModeDeactivated;
            _keyboardHook.ColorCycleRequested += OnColorCycleRequested;
            _keyboardHook.Install();
        }
        catch (Exception ex)
        {
            AppLog.Error("Failed to install keyboard hook for temporary draw mode", ex);
        }
    }

    private void ApplyToolWindowStyle()
    {
        if (_hwnd == IntPtr.Zero)
        {
            return;
        }

        var exStyle = Win32.GetWindowLongPtr(_hwnd, Win32.GwlExStyle).ToInt64();
        exStyle |= Win32.WsExToolWindow;
        exStyle |= Win32.WsExLayered;
        Win32.SetWindowLongPtr(_hwnd, Win32.GwlExStyle, new IntPtr(exStyle));
    }

    private void PositionNearPrimaryRightEdge()
    {
        if (_hwnd == IntPtr.Zero)
        {
            return;
        }

        var primary = System.Windows.Forms.Screen.PrimaryScreen;
        if (primary is null)
        {
            return;
        }

        var workingArea = primary.WorkingArea;
        const int margin = 24;

        // Note: we position in physical pixels for predictable placement.
        var widthPx = ScaleToPixels(Width, _dpiX);
        var heightPx = ScaleToPixels(Height, _dpiY);
        var x = workingArea.Right - widthPx - margin;
        var y = workingArea.Top + margin;

        var boundsPx = new Win32.Rect
        {
            Left = x,
            Top = y,
            Right = x + widthPx,
            Bottom = y + heightPx,
        };

        ApplyBoundsPxToHwnd(boundsPx);
        ApplyWpfBoundsFromPx(boundsPx, _dpiX, _dpiY);
    }

    private bool TryRegisterHotkeys()
    {
        if (_hwnd == IntPtr.Zero)
        {
            return false;
        }

        var ok = true;
        ok &= Win32.RegisterHotKey(_hwnd, HotkeyToggleDraw, Win32.ModWin | Win32.ModShift, (uint)KeyInterop.VirtualKeyFromKey(Key.D));
        ok &= Win32.RegisterHotKey(_hwnd, HotkeyClearAll, Win32.ModWin | Win32.ModShift, (uint)KeyInterop.VirtualKeyFromKey(Key.C));
        ok &= Win32.RegisterHotKey(_hwnd, HotkeyQuit, Win32.ModWin | Win32.ModShift, (uint)KeyInterop.VirtualKeyFromKey(Key.Q));
        return ok;
    }

    private void UnregisterHotkeys()
    {
        if (_hwnd == IntPtr.Zero)
        {
            return;
        }

        Win32.UnregisterHotKey(_hwnd, HotkeyToggleDraw);
        Win32.UnregisterHotKey(_hwnd, HotkeyClearAll);
        Win32.UnregisterHotKey(_hwnd, HotkeyQuit);
    }

    private void OnClosed(object? sender, EventArgs e)
    {
        UnregisterHotkeys();

        _overlayManager.ModeChanged -= OnModeChanged;
        _overlayManager.PenColorChanged -= OnPenColorChanged;

        _keyboardHook.TemporaryModeActivated -= OnTemporaryModeActivated;
        _keyboardHook.TemporaryModeDeactivated -= OnTemporaryModeDeactivated;
        _keyboardHook.ColorCycleRequested -= OnColorCycleRequested;
        _keyboardHook.Dispose();

        if (_hwndSource is not null)
        {
            _hwndSource.RemoveHook(WndProc);
            _hwndSource = null;
        }
    }

    private void OnTemporaryModeActivated(object? sender, EventArgs e)
    {
        _overlayManager.ActivateTemporaryDrawMode();
    }

    private void OnTemporaryModeDeactivated(object? sender, EventArgs e)
    {
        _overlayManager.DeactivateTemporaryDrawMode();
    }

    private void OnColorCycleRequested(object? sender, EventArgs e)
    {
        _overlayManager.CycleColor();
    }

    private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
    {
        if (msg == Win32.WmHotkey)
        {
            handled = true;
            var id = wParam.ToInt32();

            switch (id)
            {
                case HotkeyToggleDraw:
                    _overlayManager.ToggleMode();
                    break;
                case HotkeyClearAll:
                    _overlayManager.ClearAll();
                    break;
                case HotkeyQuit:
                    _overlayManager.Quit();
                    break;
            }

            return IntPtr.Zero;
        }

        if (msg != Win32.WmDpichanged)
        {
            return IntPtr.Zero;
        }

        AppLog.Info($"ControlWindow WM_DPICHANGED hwnd=0x{_hwnd.ToInt64():X} wParam=0x{wParam.ToInt64():X} lParam=0x{lParam.ToInt64():X}");

        // New DPI is in wParam (LOWORD=x, HIWORD=y).
        var newDpiX = (uint)(wParam.ToInt32() & 0xFFFF);
        var newDpiY = (uint)((wParam.ToInt32() >> 16) & 0xFFFF);
        if (newDpiX == 0) newDpiX = 96;
        if (newDpiY == 0) newDpiY = 96;

        _dpiX = newDpiX;
        _dpiY = newDpiY;

        // lParam points to a recommended RECT in physical pixels (position only).
        var rect = Marshal.PtrToStructure<Win32.Rect>(lParam);

        var newWidthPx = ScaleToPixels(LogicalWidth, newDpiX);
        var newHeightPx = ScaleToPixels(LogicalHeight, newDpiY);
        var adjustedRect = new Win32.Rect
        {
            Left = rect.Left,
            Top = rect.Top,
            Right = rect.Left + newWidthPx,
            Bottom = rect.Top + newHeightPx,
        };

        ApplyBoundsPxToHwnd(adjustedRect);
        ApplyWpfBoundsFromPx(adjustedRect, _dpiX, _dpiY);

        handled = true;
        return IntPtr.Zero;
    }

    private void ApplyBoundsPxToHwnd(Win32.Rect boundsPx)
    {
        if (_hwnd != IntPtr.Zero)
        {
            Win32.SetWindowPos(
                _hwnd,
                Win32.HwndTopmost,
                boundsPx.Left,
                boundsPx.Top,
                boundsPx.Width,
                boundsPx.Height,
                Win32.SwpNoActivate);
        }
    }

    private void ApplyWpfBoundsFromPx(Win32.Rect boundsPx, uint dpiX, uint dpiY)
    {
        var dx = dpiX == 0 ? 96u : dpiX;
        var dy = dpiY == 0 ? 96u : dpiY;

        Left = boundsPx.Left * 96.0 / dx;
        Top = boundsPx.Top * 96.0 / dy;
        Width = boundsPx.Width * 96.0 / dx;
        Height = boundsPx.Height * 96.0 / dy;
    }

    private static int ScaleToPixels(double logicalSize, uint dpi)
    {
        var effectiveDpi = dpi == 0 ? 96u : dpi;
        return (int)Math.Round(logicalSize * effectiveDpi / 96.0);
    }

    private void OnToggleClick(object sender, RoutedEventArgs e)
    {
        _overlayManager.ToggleMode();
    }

    private void OnColorCycleClick(object sender, RoutedEventArgs e)
    {
        _overlayManager.CycleColor();
    }

    private void OnModeChanged(object? sender, OverlayMode mode)
    {
        UpdateToggleButtonAppearance(mode);
    }

    private void OnPenColorChanged(object? sender, PenColor color)
    {
        UpdateColorCycleButton(color);
    }

    private void UpdateToggleButtonAppearance(OverlayMode mode)
    {
        // Update button appearance based on current mode using Tag property
        // This allows XAML style triggers to work properly for hover effects
        ToggleButton.Tag = mode.ToString();
    }

    private void UpdateColorCycleButton(PenColor color)
    {
        if (ColorCycleSwatch is null)
        {
            return;
        }

        ColorCycleSwatch.Fill = CreateBrush(color);
    }

    private static System.Windows.Media.Brush CreateBrush(PenColor color)
    {
        return color switch
        {
            PenColor.Red => new SolidColorBrush(System.Windows.Media.Color.FromRgb(0xFF, 0x00, 0x00)),
            PenColor.Blue => new SolidColorBrush(System.Windows.Media.Color.FromRgb(0x00, 0x00, 0xFF)),
            PenColor.Green => new SolidColorBrush(System.Windows.Media.Color.FromRgb(0x00, 0xFF, 0x00)),
            _ => new SolidColorBrush(System.Windows.Media.Color.FromRgb(0xFF, 0x00, 0x00)),
        };
    }

    private void OnClearClick(object sender, RoutedEventArgs e)
    {
        _overlayManager.ClearAll();
    }

    private void OnQuitClick(object sender, RoutedEventArgs e)
    {
        _overlayManager.Quit();
    }
}
