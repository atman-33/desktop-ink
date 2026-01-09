using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;

namespace DesktopInk;

public partial class ControlWindow : Window
{
    private const int HotkeyToggleDraw = 1;
    private const int HotkeyClearAll = 2;
    private const int HotkeyQuit = 3;

    private readonly OverlayManager _overlayManager;

    private HwndSource? _hwndSource;
    private IntPtr _hwnd;

    public ControlWindow(OverlayManager overlayManager)
    {
        _overlayManager = overlayManager;

        InitializeComponent();

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
        PositionNearPrimaryRightEdge();

        if (!TryRegisterHotkeys())
        {
            System.Windows.MessageBox.Show(
                "Failed to register one or more global hotkeys. Another application may already be using them.",
                "DesktopInk",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
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
        var x = workingArea.Right - (int)Width - margin;
        var y = workingArea.Top + margin;

        Win32.SetWindowPos(
            _hwnd,
            Win32.HwndTopmost,
            x,
            y,
            (int)Width,
            (int)Height,
            Win32.SwpNoActivate);
    }

    private bool TryRegisterHotkeys()
    {
        if (_hwnd == IntPtr.Zero)
        {
            return false;
        }

        var ok = true;
        ok &= Win32.RegisterHotKey(_hwnd, HotkeyToggleDraw, Win32.ModControl | Win32.ModAlt, (uint)KeyInterop.VirtualKeyFromKey(Key.D));
        ok &= Win32.RegisterHotKey(_hwnd, HotkeyClearAll, Win32.ModControl | Win32.ModAlt, (uint)KeyInterop.VirtualKeyFromKey(Key.C));
        ok &= Win32.RegisterHotKey(_hwnd, HotkeyQuit, Win32.ModControl | Win32.ModAlt, (uint)KeyInterop.VirtualKeyFromKey(Key.Q));
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

        if (_hwndSource is not null)
        {
            _hwndSource.RemoveHook(WndProc);
            _hwndSource = null;
        }
    }

    private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
    {
        if (msg != Win32.WmHotkey)
        {
            return IntPtr.Zero;
        }

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

    private void OnToggleClick(object sender, RoutedEventArgs e)
    {
        _overlayManager.ToggleMode();
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
