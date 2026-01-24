using System.Diagnostics;
using System.Windows;
using DesktopInk.Core;
using DesktopInk.Infrastructure;
using DesktopInk.Windows;

namespace DesktopInk;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : System.Windows.Application
{
	private OverlayManager? _overlayManager;
	private ControlWindow? _controlWindow;
	private AppSettings? _appSettings;
	private readonly object _updateDialogGate = new();
	private bool _updateDialogShown;

	protected override void OnStartup(StartupEventArgs e)
	{
		base.OnStartup(e);

		ShutdownMode = ShutdownMode.OnExplicitShutdown;

		AppLog.Info($"Startup cwd='{Environment.CurrentDirectory}' args='{string.Join(' ', e.Args)}'");

		_overlayManager = new OverlayManager();
		_overlayManager.ShowOverlays();

		_controlWindow = new ControlWindow(_overlayManager);
		_controlWindow.Show();

		_appSettings = AppSettings.Load();

		_ = Task.Run(async () =>
		{
			try
			{
				await Task.Delay(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
				await CheckForUpdatesAsync().ConfigureAwait(false);
			}
			catch (Exception ex)
			{
				AppLog.Error("Update check task failed.", ex);
			}
		});
	}

	protected override void OnExit(ExitEventArgs e)
	{
		AppLog.Info("Exit");
		_controlWindow?.Close();
		_controlWindow = null;

		_overlayManager?.Dispose();
		_overlayManager = null;

		base.OnExit(e);
	}

	private async Task CheckForUpdatesAsync()
	{
		if (_appSettings is null)
		{
			return;
		}

		var versionSettings = _appSettings.VersionCheck;
		if (!versionSettings.Enabled)
		{
			AppLog.Info("Version check disabled.");
			return;
		}

		var currentVersion = GetCurrentVersion();
		using var checker = new VersionChecker();
		var result = await checker.CheckForUpdatesAsync(currentVersion, versionSettings, CancellationToken.None)
			.ConfigureAwait(false);

		_appSettings.Save();

		if (!result.IsNewVersionAvailable || string.IsNullOrWhiteSpace(result.LatestVersion))
		{
			return;
		}

		if (string.Equals(versionSettings.SkippedVersion, result.LatestVersion, StringComparison.OrdinalIgnoreCase))
		{
			AppLog.Info($"Update skipped for version '{result.LatestVersion}'.");
			return;
		}

		if (!string.IsNullOrWhiteSpace(versionSettings.SkippedVersion) &&
			!string.Equals(versionSettings.SkippedVersion, result.LatestVersion, StringComparison.OrdinalIgnoreCase))
		{
			versionSettings.SkippedVersion = null;
			_appSettings.Save();
		}

		lock (_updateDialogGate)
		{
			if (_updateDialogShown)
			{
				return;
			}

			_updateDialogShown = true;
		}

		await Dispatcher.InvokeAsync(() =>
		{
			var dialog = new UpdateNotificationDialog(currentVersion, result)
			{
				Owner = _controlWindow
			};

			dialog.ShowDialog();

			if (dialog.UserChoice is null)
			{
				return;
			}

			AppLog.Info($"Update dialog action: {dialog.UserChoice}.");

			if (dialog.UserChoice == UserAction.SkipVersion)
			{
				versionSettings.SkippedVersion = result.LatestVersion;
				versionSettings.LastChecked = DateTime.UtcNow;
				_appSettings.Save();
			}
		});
	}

	private static string GetCurrentVersion()
	{
		// Environment variable override for debugging/testing
		var overrideVersion = Environment.GetEnvironmentVariable("DESKTOPINK_DEBUG_VERSION");
		if (!string.IsNullOrWhiteSpace(overrideVersion))
		{
			AppLog.Info($"Using debug version from environment: {overrideVersion}");
			return overrideVersion;
		}

		try
		{
			var assemblyLocation = System.Reflection.Assembly.GetExecutingAssembly().Location;
			var info = FileVersionInfo.GetVersionInfo(assemblyLocation);
			return info.FileVersion ?? info.ProductVersion ?? "0.0.0";
		}
		catch (Exception ex)
		{
			AppLog.Error("Failed to read current version.", ex);
			return "0.0.0";
		}
	}
}

