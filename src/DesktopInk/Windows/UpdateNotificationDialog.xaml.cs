using System.Diagnostics;
using System.Windows;
using DesktopInk.Infrastructure;

namespace DesktopInk.Windows;

public partial class UpdateNotificationDialog : Window
{
    private readonly string? _releaseNotesUrl;

    public UpdateNotificationDialog(string currentVersion, VersionCheckResult result)
    {
        InitializeComponent();

        _releaseNotesUrl = result.ReleaseNotesUrl;

        CurrentVersionText.Text = string.IsNullOrWhiteSpace(currentVersion) ? "Unknown" : currentVersion;
        LatestVersionText.Text = result.LatestVersion ?? "Unknown";

        ReleaseNotesText.Text = string.IsNullOrWhiteSpace(result.ReleaseNotes)
            ? "Release notes are not available."
            : result.ReleaseNotes.Trim();
    }

    public UserAction? UserChoice { get; private set; }

    private void OnOpenDownloadClick(object sender, RoutedEventArgs e)
    {
        if (!string.IsNullOrWhiteSpace(_releaseNotesUrl))
        {
            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = _releaseNotesUrl,
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                AppLog.Error("Failed to open release page.", ex);
            }
        }

        UserChoice = UserAction.OpenDownloadPage;
        Close();
    }

    private void OnRemindLaterClick(object sender, RoutedEventArgs e)
    {
        UserChoice = UserAction.RemindLater;
        Close();
    }

    private void OnSkipVersionClick(object sender, RoutedEventArgs e)
    {
        UserChoice = UserAction.SkipVersion;
        Close();
    }
}

public enum UserAction
{
    OpenDownloadPage,
    RemindLater,
    SkipVersion
}
