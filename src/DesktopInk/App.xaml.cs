using System.Windows;

namespace DesktopInk;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : System.Windows.Application
{
	private OverlayManager? _overlayManager;
	private ControlWindow? _controlWindow;

	protected override void OnStartup(StartupEventArgs e)
	{
		base.OnStartup(e);

		ShutdownMode = ShutdownMode.OnExplicitShutdown;

		_overlayManager = new OverlayManager();
		_overlayManager.ShowOverlays();

		_controlWindow = new ControlWindow(_overlayManager);
		_controlWindow.Show();
	}

	protected override void OnExit(ExitEventArgs e)
	{
		_controlWindow?.Close();
		_controlWindow = null;

		_overlayManager?.Dispose();
		_overlayManager = null;

		base.OnExit(e);
	}
}

