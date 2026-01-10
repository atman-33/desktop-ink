using System.Text;
using DesktopInk.Infrastructure;
using FluentAssertions;
using Xunit;

namespace DesktopInk.Tests.Infrastructure;

public class AppLogTests
{
    [Fact]
    public void LogPath_ShouldReturnValidPath()
    {
        // Act
        var logPath = AppLog.LogPath;

        // Assert
        logPath.Should().NotBeNullOrEmpty();
        logPath.Should().EndWith(Path.Combine(".tmp", "desktopink", "desktopink.log"));
    }

    [Fact]
    public void LogPath_ShouldBeInCurrentDirectory()
    {
        // Act
        var logPath = AppLog.LogPath;

        // Assert
        logPath.Should().StartWith(Environment.CurrentDirectory);
    }

#if DEBUG
    [Fact]
    public void Info_ShouldWriteToLogFile()
    {
        // Arrange
        var testMessage = $"Test message at {DateTime.Now.Ticks}";
        var logPath = AppLog.LogPath;

        // Ensure directory exists
        if (File.Exists(logPath))
        {
            File.Delete(logPath);
        }

        // Act
        AppLog.Info(testMessage);

        // Wait a bit for file write
        Thread.Sleep(100);

        // Assert
        File.Exists(logPath).Should().BeTrue();
        var logContent = File.ReadAllText(logPath, Encoding.UTF8);
        logContent.Should().Contain("INFO");
        logContent.Should().Contain(testMessage);
    }

    [Fact]
    public void Error_ShouldWriteErrorMessageToLogFile()
    {
        // Arrange
        var testMessage = $"Error message at {DateTime.Now.Ticks}";
        var logPath = AppLog.LogPath;

        // Ensure directory exists
        if (File.Exists(logPath))
        {
            File.Delete(logPath);
        }

        // Act
        AppLog.Error(testMessage);

        // Wait a bit for file write
        Thread.Sleep(100);

        // Assert
        File.Exists(logPath).Should().BeTrue();
        var logContent = File.ReadAllText(logPath, Encoding.UTF8);
        logContent.Should().Contain("ERROR");
        logContent.Should().Contain(testMessage);
    }

    [Fact]
    public void Error_WithException_ShouldWriteExceptionDetails()
    {
        // Arrange
        var testMessage = $"Error with exception at {DateTime.Now.Ticks}";
        var exception = new InvalidOperationException("Test exception");
        var logPath = AppLog.LogPath;

        // Ensure directory exists
        if (File.Exists(logPath))
        {
            File.Delete(logPath);
        }

        // Act
        AppLog.Error(testMessage, exception);

        // Wait a bit for file write
        Thread.Sleep(100);

        // Assert
        File.Exists(logPath).Should().BeTrue();
        var logContent = File.ReadAllText(logPath, Encoding.UTF8);
        logContent.Should().Contain("ERROR");
        logContent.Should().Contain(testMessage);
        logContent.Should().Contain("InvalidOperationException");
        logContent.Should().Contain("Test exception");
    }
#endif

#if RELEASE
    [Fact]
    public void Info_InReleaseMode_ShouldNotWriteToLogFile()
    {
        // Arrange
        var testMessage = $"Release test at {DateTime.Now.Ticks}";
        var logPath = AppLog.LogPath;
        var logDir = Path.GetDirectoryName(logPath)!;

        // Clean up if exists
        if (Directory.Exists(logDir))
        {
            Directory.Delete(logDir, true);
        }

        // Act
        AppLog.Info(testMessage);

        // Wait a bit
        Thread.Sleep(100);

        // Assert - In release mode, logging is disabled
        Directory.Exists(logDir).Should().BeFalse();
    }
#endif
}
