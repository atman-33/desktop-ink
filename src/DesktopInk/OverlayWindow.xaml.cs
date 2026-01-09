using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Shapes;

namespace DesktopInk;

public partial class OverlayWindow : Window
{
    private readonly int _x;
    private readonly int _y;
    private readonly int _width;
    private readonly int _height;

    private IntPtr _hwnd;

    private OverlayMode _mode = OverlayMode.PassThrough;
    private bool _isDrawing;
    private Polyline? _activeStroke;

    public OverlayWindow(int x, int y, int width, int height)
    {
        _x = x;
        _y = y;
        _width = width;
        _height = height;

        InitializeComponent();

        SourceInitialized += OnSourceInitialized;

        MouseLeftButtonDown += OnMouseLeftButtonDown;
        MouseMove += OnMouseMove;
        MouseLeftButtonUp += OnMouseLeftButtonUp;
    }

    public void SetMode(OverlayMode mode)
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

    public void ClearAll()
    {
        StrokeCanvas.Children.Clear();
    }

    private void OnSourceInitialized(object? sender, System.EventArgs e)
    {
        _hwnd = new WindowInteropHelper(this).Handle;

        ApplyToolWindowStyle();
        PositionToBounds();

        SetMode(_mode);
    }

    private void PositionToBounds()
    {
        if (_hwnd == IntPtr.Zero)
        {
            return;
        }

        Win32.SetWindowPos(
            _hwnd,
            Win32.HwndTopmost,
            _x,
            _y,
            _width,
            _height,
            Win32.SwpNoActivate);
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

        _ = Win32.SetWindowPos(
            _hwnd,
            IntPtr.Zero,
            0,
            0,
            0,
            0,
            Win32.SwpNoActivate | Win32.SwpNoMove | Win32.SwpNoSize | Win32.SwpNoZOrder | Win32.SwpFrameChanged);
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
