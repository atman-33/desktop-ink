using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Shapes;

namespace DesktopInk;

public partial class OverlayWindow : Window
{
    private Win32.Rect _boundsPx;

    private uint _dpiX;
    private uint _dpiY;

    private IntPtr _hwnd;
    private HwndSource? _hwndSource;

    private OverlayMode _mode = OverlayMode.PassThrough;
    private bool _isDrawing;
    private Polyline? _activeStroke;

    internal OverlayWindow(Win32.Rect boundsPx, uint dpiX, uint dpiY)
    {
        _boundsPx = boundsPx;
        _dpiX = dpiX == 0 ? 96u : dpiX;
        _dpiY = dpiY == 0 ? 96u : dpiY;

        InitializeComponent();

        ApplyWpfBoundsFromPx(_boundsPx, _dpiX, _dpiY);

    AppLog.Info($"OverlayWindow ctor boundsPx=({_boundsPx.Left},{_boundsPx.Top}) {_boundsPx.Width}x{_boundsPx.Height} dpi=({_dpiX},{_dpiY})");

        SourceInitialized += OnSourceInitialized;
        Closed += OnClosed;

        MouseLeftButtonDown += OnMouseLeftButtonDown;
        MouseMove += OnMouseMove;
        MouseLeftButtonUp += OnMouseLeftButtonUp;
    }

    public void SetMode(OverlayMode mode)
    {
        _mode = mode;

        AppLog.Info($"OverlayWindow SetMode hwnd=0x{_hwnd.ToInt64():X} mode={_mode}");

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

        LogGeometry("after-set-mode");
    }

    public void ClearAll()
    {
        StrokeCanvas.Children.Clear();
    }

    private void OnSourceInitialized(object? sender, System.EventArgs e)
    {
        _hwnd = new WindowInteropHelper(this).Handle;

        _hwndSource = HwndSource.FromHwnd(_hwnd);
        _hwndSource.AddHook(WndProc);

        ApplyToolWindowStyle();
        // Ensure WPF logical bounds match the actual monitor bounds.
        // Use the window DPI as ground truth if available.
        var dpi = Win32.GetDpiForWindow(_hwnd);
        if (dpi != 0)
        {
            _dpiX = dpi;
            _dpiY = dpi;
        }

        ApplyWpfBoundsFromPx(_boundsPx, _dpiX, _dpiY);

        SetMode(_mode);

        LogGeometry("after-source-init");
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

    private void ApplyBoundsPxToHwnd(Win32.Rect boundsPx)
    {
        if (_hwnd == IntPtr.Zero)
        {
            return;
        }

        Win32.SetWindowPos(
            _hwnd,
            Win32.HwndTopmost,
            boundsPx.Left,
            boundsPx.Top,
            boundsPx.Width,
            boundsPx.Height,
            Win32.SwpNoActivate);
    }

    private void OnClosed(object? sender, System.EventArgs e)
    {
        if (_hwndSource is not null)
        {
            _hwndSource.RemoveHook(WndProc);
            _hwndSource = null;
        }
    }

    private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
    {
        if (msg == Win32.WmMouseActivate)
        {
            // In draw mode we want to receive pointer input, but we do not want to steal activation
            // from the control palette or underlying apps.
            if (_mode == OverlayMode.Draw)
            {
                handled = true;
                return new IntPtr(Win32.MaNoActivate);
            }

            return IntPtr.Zero;
        }

        if (msg != Win32.WmDpichanged)
        {
            return IntPtr.Zero;
        }

        AppLog.Info($"OverlayWindow WM_DPICHANGED hwnd=0x{_hwnd.ToInt64():X} wParam=0x{wParam.ToInt64():X} lParam=0x{lParam.ToInt64():X}");

        // New DPI is in wParam (LOWORD=x, HIWORD=y).
        var newDpiX = (uint)(wParam.ToInt32() & 0xFFFF);
        var newDpiY = (uint)((wParam.ToInt32() >> 16) & 0xFFFF);
        if (newDpiX == 0) newDpiX = 96;
        if (newDpiY == 0) newDpiY = 96;

        _dpiX = newDpiX;
        _dpiY = newDpiY;

        // lParam points to a recommended RECT in physical pixels.
        var rect = Marshal.PtrToStructure<Win32.Rect>(lParam);
        _boundsPx = rect;

        ApplyBoundsPxToHwnd(rect);
        ApplyWpfBoundsFromPx(rect, _dpiX, _dpiY);

        LogGeometry("wm-dpichanged");

        handled = true;
        return IntPtr.Zero;
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

        AppLog.Info($"OverlayWindow ToolWindowStyle hwnd=0x{_hwnd.ToInt64():X} exStyle=0x{exStyle:X}");
    }

    private void SetClickThrough(bool enabled)
    {
        if (_hwnd == IntPtr.Zero)
        {
            return;
        }

        var exStyle = Win32.GetWindowLongPtr(_hwnd, Win32.GwlExStyle).ToInt64();
        var before = exStyle;

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

        _ = Win32.SetWindowPos(
            _hwnd,
            IntPtr.Zero,
            0,
            0,
            0,
            0,
            Win32.SwpNoActivate | Win32.SwpNoMove | Win32.SwpNoSize | Win32.SwpNoZOrder | Win32.SwpFrameChanged);

        AppLog.Info($"OverlayWindow ClickThrough hwnd=0x{_hwnd.ToInt64():X} enabled={enabled} exStyleBefore=0x{before:X} exStyleAfter=0x{exStyle:X} hitTest={Root.IsHitTestVisible}");
    }

    private void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (_mode != OverlayMode.Draw)
        {
            return;
        }

        try
        {
            var p = e.GetPosition(StrokeCanvas);
            AppLog.Info($"OverlayWindow MouseDown hwnd=0x{_hwnd.ToInt64():X} p=({p.X:0.##},{p.Y:0.##})");
        }
        catch { }

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

        if ((_activeStroke.Points.Count % 50) == 0)
        {
            try
            {
                AppLog.Info($"OverlayWindow MouseMove hwnd=0x{_hwnd.ToInt64():X} points={_activeStroke.Points.Count} last=({point.X:0.##},{point.Y:0.##})");
            }
            catch { }
        }

        e.Handled = true;
    }

    private void OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
        if (_mode != OverlayMode.Draw)
        {
            return;
        }

        try
        {
            var p = e.GetPosition(StrokeCanvas);
            AppLog.Info($"OverlayWindow MouseUp hwnd=0x{_hwnd.ToInt64():X} p=({p.X:0.##},{p.Y:0.##})");
        }
        catch { }

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

    private void LogGeometry(string tag)
    {
        try
        {
            var dpiWin = _hwnd != IntPtr.Zero ? Win32.GetDpiForWindow(_hwnd) : 0;
            var wpf = $"wpf L/T=({Left:0.##},{Top:0.##}) W/H=({Width:0.##},{Height:0.##}) Actual=({ActualWidth:0.##},{ActualHeight:0.##})";

            var hwndRect = "hwndRect=(n/a)";
            if (_hwnd != IntPtr.Zero && Win32.GetWindowRect(_hwnd, out var r))
            {
                hwndRect = $"hwndRect=({r.Left},{r.Top}) {r.Width}x{r.Height}";
            }

            AppLog.Info($"OverlayWindow Geometry {tag} hwnd=0x{_hwnd.ToInt64():X} boundsPx=({_boundsPx.Left},{_boundsPx.Top}) {_boundsPx.Width}x{_boundsPx.Height} dpi=({_dpiX},{_dpiY}) dpiWin={dpiWin} {hwndRect} {wpf}");
        }
        catch (Exception ex)
        {
            AppLog.Error("OverlayWindow LogGeometry failed", ex);
        }
    }
}
