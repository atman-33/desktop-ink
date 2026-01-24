using DesktopInk.Infrastructure;
using FluentAssertions;
using Xunit;

namespace DesktopInk.Tests.Infrastructure;

public class AppSettingsTests
{
    [Fact]
    public void Load_ShouldCreateDefaults_WhenFileMissing()
    {
        var tempDir = CreateTempDirectory();
        var path = Path.Combine(tempDir, "settings.json");

        var settings = AppSettings.Load(path);

        settings.VersionCheck.Enabled.Should().BeTrue();
        settings.VersionCheck.SkippedVersion.Should().BeNull();
        settings.VersionCheck.LastChecked.Should().BeNull();
        File.Exists(path).Should().BeTrue();
    }

    [Fact]
    public void Save_ShouldPersistValues()
    {
        var tempDir = CreateTempDirectory();
        var path = Path.Combine(tempDir, "settings.json");
        var timestamp = new DateTime(2026, 1, 24, 10, 0, 0, DateTimeKind.Utc);

        var settings = new AppSettings
        {
            VersionCheck = new VersionCheckSettings
            {
                Enabled = false,
                SkippedVersion = "1.4.0",
                LastChecked = timestamp
            }
        };

        settings.Save(path);

        var loaded = AppSettings.Load(path);
        loaded.VersionCheck.Enabled.Should().BeFalse();
        loaded.VersionCheck.SkippedVersion.Should().Be("1.4.0");
        loaded.VersionCheck.LastChecked.Should().Be(timestamp);
    }

    private static string CreateTempDirectory()
    {
        var root = Path.Combine(Path.GetTempPath(), "DesktopInkTests", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(root);
        return root;
    }
}
