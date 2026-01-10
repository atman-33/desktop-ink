using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Interop;
using DesktopInk.Infrastructure;

namespace DesktopInk.Windows;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private enum OverlayMode
    {
        PassThrough,
        Draw,
    }

    private const int HotkeyToggleDraw = 1;
    private const int HotkeyClearAll = 2;
    private const int HotkeyQuit = 3;

    private HwndSource? _hwndSource;
    private IntPtr _hwnd;

    private OverlayMode _mode = OverlayMode.PassThrough;
    private bool _isDrawing;
    private Polyline? _activeStroke;

    public MainWindow()
    {
        InitializeComponent();

        SourceInitialized += OnSourceInitialized;
        Closed += OnClosed;

        MouseLeftButtonDown += OnMouseLeftButtonDown;
        MouseMove += OnMouseMove;
        MouseLeftButtonUp += OnMouseLeftButtonUp;
    }

    private void OnSourceInitialized(object? sender, EventArgs e)
    {
        var helper = new WindowInteropHelper(this);
        _hwnd = helper.Handle;

        _hwndSource = HwndSource.FromHwnd(_hwnd);
        _hwndSource.AddHook(WndProc);

        ConfigureOverlayBounds();
        ApplyToolWindowStyle();

        if (!TryRegisterHotkeys())
        {
            System.Windows.MessageBox.Show(
                "Failed to register one or more global hotkeys. Another application may already be using them.\n\nThe app will now exit.",
                "DesktopInk",
                MessageBoxButton.OK,
                MessageBoxImage.Error);

            Close();
            return;
        }

        SetMode(OverlayMode.PassThrough);
    }

    private void ConfigureOverlayBounds()
    {
        WindowStartupLocation = WindowStartupLocation.Manual;
        Left = 0;
        Top = 0;
        Width = SystemParameters.PrimaryScreenWidth;
        Height = SystemParameters.PrimaryScreenHeight;
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
                ToggleMode();
                break;
            case HotkeyClearAll:
                ClearAll();
                break;
            case HotkeyQuit:
                Close();
                break;
        }

        return IntPtr.Zero;
    }

    private void ToggleMode()
    {
        SetMode(_mode == OverlayMode.PassThrough ? OverlayMode.Draw : OverlayMode.PassThrough);
    }

    private void SetMode(OverlayMode mode)
    {
        _mode = mode;

        if (_mode == OverlayMode.Draw)
        {
            SetClickThrough(false);
            IndicatorHost.Visibility = Visibility.Visible;
        }
        else
        {
            CancelActiveStroke();
            SetClickThrough(true);
            IndicatorHost.Visibility = Visibility.Collapsed;
        }
    }

    private void SetClickThrough(bool enabled)
    {
        if (_hwnd == IntPtr.Zero)
        {
            return;
        }

        var exStyle = Win32.GetWindowLongPtr(_hwnd, Win32.GwlExStyle).ToInt64();

        exStyle |= Win32.WsExLayered;
        exStyle |= Win32.WsExToolWindow;

        if (enabled)
        {
            exStyle |= Win32.WsExTransparent;
            Root.IsHitTestVisible = false;
        }
        else
        {
            exStyle &= ~Win32.WsExTransparent;
            Root.IsHitTestVisible = true;
        }

        Win32.SetWindowLongPtr(_hwnd, Win32.GwlExStyle, new IntPtr(exStyle));
    }

    private void ClearAll()
    {
        StrokeCanvas.Children.Clear();
    }

    private void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (_mode != OverlayMode.Draw)
        {
            return;
        }

        _isDrawing = true;
        _activeStroke = CreateStroke();
        var point = e.GetPosition(StrokeCanvas);
        _activeStroke.Points.Add(point);
        StrokeCanvas.Children.Add(_activeStroke);

        CaptureMouse();
        e.Handled = true;
    }

    private void OnMouseMove(object sender, System.Windows.Input.MouseEventArgs e)
    {
        if (_mode != OverlayMode.Draw || !_isDrawing || _activeStroke is null)
        {
            return;
        }

        if (e.LeftButton != MouseButtonState.Pressed)
        {
            CancelActiveStroke();
            return;
        }

        var point = e.GetPosition(StrokeCanvas);
        _activeStroke.Points.Add(point);
        e.Handled = true;
    }

    private void OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
        if (_mode != OverlayMode.Draw)
        {
            return;
        }

        if (_isDrawing)
        {
            _isDrawing = false;
            _activeStroke = null;
            ReleaseMouseCapture();
            e.Handled = true;
        }
    }

    private void CancelActiveStroke()
    {
        _isDrawing = false;
        _activeStroke = null;
        ReleaseMouseCapture();
    }

    private static Polyline CreateStroke()
    {
        return new Polyline
        {
            Stroke = System.Windows.Media.Brushes.Red,
            StrokeThickness = 3,
            StrokeStartLineCap = PenLineCap.Round,
            StrokeEndLineCap = PenLineCap.Round,
            StrokeLineJoin = PenLineJoin.Round,
            SnapsToDevicePixels = true,
        };
    }
}