using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using DesktopInk.Core;
using DesktopInk.Infrastructure;

namespace DesktopInk.Windows;

public partial class ControlWindow : Window
{
    private const int HotkeyToggleDraw = 1;
    private const int HotkeyClearAll = 2;
    private const int HotkeyQuit = 3;

    private readonly OverlayManager _overlayManager;
    private readonly KeyboardHookManager _keyboardHook;

    private HwndSource? _hwndSource;
    private IntPtr _hwnd;

    public ControlWindow(OverlayManager overlayManager)
    {
        _overlayManager = overlayManager;
        _keyboardHook = new KeyboardHookManager();

        InitializeComponent();

        // Subscribe to mode changes for visual feedback
        _overlayManager.ModeChanged += OnModeChanged;

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

        // Install keyboard hook for temporary draw mode
        try
        {
            _keyboardHook.TemporaryModeActivated += OnTemporaryModeActivated;
            _keyboardHook.TemporaryModeDeactivated += OnTemporaryModeDeactivated;
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

        _keyboardHook.TemporaryModeActivated -= OnTemporaryModeActivated;
        _keyboardHook.TemporaryModeDeactivated -= OnTemporaryModeDeactivated;
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

    private void OnModeChanged(object? sender, OverlayMode mode)
    {
        UpdateToggleButtonAppearance(mode);
    }

    private void UpdateToggleButtonAppearance(OverlayMode mode)
    {
        // Update button appearance based on current mode
        if (mode == OverlayMode.Draw)
        {
            // Active state: Windows accent blue for professional appearance
            ToggleButton.Background = new SolidColorBrush(System.Windows.Media.Color.FromArgb(220, 0, 120, 212)); // Windows Blue (#0078D4) with high opacity
        }
        else
        {
            // Inactive state: default transparent
            ToggleButton.Background = System.Windows.Media.Brushes.Transparent;
        }
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
