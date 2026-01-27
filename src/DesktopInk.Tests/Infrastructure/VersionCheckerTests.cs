using System.Net;
using System.Net.Http.Headers;
using System.Text;
using DesktopInk.Infrastructure;
using FluentAssertions;
using Xunit;

namespace DesktopInk.Tests.Infrastructure;

public class VersionCheckerTests
{
    [Fact]
    public void IsNewerVersion_ShouldHandlePrefixesAndPrerelease()
    {
        VersionChecker.IsNewerVersion("1.3.0", "v1.4.0").Should().BeTrue();
        VersionChecker.IsNewerVersion("1.4.0", "1.4.0-beta").Should().BeFalse();
        VersionChecker.IsNewerVersion("1.4.0", "1.4.0").Should().BeFalse();
    }

    [Fact]
    public void IsNewerVersion_ShouldReturnFalseForInvalidVersions()
    {
        VersionChecker.IsNewerVersion("1.3.0", "invalid").Should().BeFalse();
        VersionChecker.IsNewerVersion("invalid", "1.4.0").Should().BeFalse();
    }

    [Fact]
    public async Task CheckForUpdatesAsync_ShouldReturnUpdateWhenNewerVersionAvailable()
    {
        var responseJson = "{" +
                           "\"tag_name\": \"v1.4.0\"," +
                           "\"html_url\": \"https://github.com/atman-33/desktop-ink/releases/tag/v1.4.0\"," +
                           "\"body\": \"- Added feature X\"" +
                           "}";

        var handler = new StubHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        });

        var httpClient = new HttpClient(handler);
        var checker = new VersionChecker(httpClient);
        var settings = new VersionCheckSettings
        {
            Enabled = true,
            LastChecked = DateTime.UtcNow.AddDays(-2)
        };

        var result = await checker.CheckForUpdatesAsync("1.3.0", settings, CancellationToken.None);

        result.IsNewVersionAvailable.Should().BeTrue();
        result.LatestVersion.Should().Be("1.4.0");
        result.ReleaseNotesUrl.Should().Be("https://github.com/atman-33/desktop-ink/releases/tag/v1.4.0");
        result.ReleaseNotes.Should().Contain("Added feature X");
        settings.LastChecked.Should().NotBeNull();

        handler.CallCount.Should().Be(1);
        handler.LastRequest.Should().NotBeNull();
        handler.LastRequest!.Headers.UserAgent.ToString().Should().Contain("DesktopInk");
        handler.LastRequest!.Headers.Accept.Should().Contain(h => h.MediaType == "application/json");
    }

    [Fact]
    public async Task CheckForUpdatesAsync_ShouldSkipOnFirstLaunchAndSetBaseline()
    {
        var handler = new StubHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.OK));
        var httpClient = new HttpClient(handler);
        var checker = new VersionChecker(httpClient);
        var settings = new VersionCheckSettings { Enabled = true };
        var now = DateTime.UtcNow;

        var result = await checker.CheckForUpdatesAsync("1.5.0", settings, CancellationToken.None);

        result.IsNewVersionAvailable.Should().BeFalse();
        result.LatestVersion.Should().BeNull();
        settings.SkippedVersion.Should().Be("1.5.0");
        settings.LastChecked.Should().NotBeNull();
        settings.LastChecked.Should().BeOnOrAfter(now);
        settings.LastChecked.Should().BeOnOrBefore(DateTime.UtcNow);
        handler.CallCount.Should().Be(0);
    }

    [Fact]
    public async Task CheckForUpdatesAsync_ShouldOverrideBaselineWhenLastCheckedIsNull()
    {
        var handler = new StubHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.OK));
        var httpClient = new HttpClient(handler);
        var checker = new VersionChecker(httpClient);
        var settings = new VersionCheckSettings
        {
            Enabled = true,
            SkippedVersion = "1.2.0",
            LastChecked = null
        };

        var result = await checker.CheckForUpdatesAsync("1.5.0", settings, CancellationToken.None);

        result.IsNewVersionAvailable.Should().BeFalse();
        settings.SkippedVersion.Should().Be("1.5.0");
        settings.LastChecked.Should().NotBeNull();
        handler.CallCount.Should().Be(0);
    }

    [Fact]
    public async Task CheckForUpdatesAsync_ShouldPerformNormalCheckAfterFirstLaunch()
    {
        var responseJson = "{" +
                           "\"tag_name\": \"v1.6.0\"," +
                           "\"html_url\": \"https://github.com/atman-33/desktop-ink/releases/tag/v1.6.0\"," +
                           "\"body\": \"- Added feature Y\"" +
                           "}";

        var handler = new StubHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        });

        var httpClient = new HttpClient(handler);
        var checker = new VersionChecker(httpClient);
        var settings = new VersionCheckSettings
        {
            Enabled = true,
            SkippedVersion = "1.5.0",
            LastChecked = DateTime.UtcNow.AddDays(-2)
        };

        var result = await checker.CheckForUpdatesAsync("1.5.0", settings, CancellationToken.None);

        result.IsNewVersionAvailable.Should().BeTrue();
        result.LatestVersion.Should().Be("1.6.0");
        handler.CallCount.Should().Be(1);
    }

    [Fact]
    public async Task CheckForUpdatesAsync_ShouldSkipWhenCheckedRecently()
    {
        var handler = new StubHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.OK));
        var httpClient = new HttpClient(handler);
        var checker = new VersionChecker(httpClient);
        var settings = new VersionCheckSettings
        {
            Enabled = true,
            LastChecked = DateTime.UtcNow.AddHours(-2)
        };

        var result = await checker.CheckForUpdatesAsync("1.3.0", settings, CancellationToken.None);

        result.IsNewVersionAvailable.Should().BeFalse();
        handler.CallCount.Should().Be(0);
    }

    private sealed class StubHttpMessageHandler : HttpMessageHandler
    {
        private readonly Func<HttpRequestMessage, HttpResponseMessage> _handler;

        public StubHttpMessageHandler(Func<HttpRequestMessage, HttpResponseMessage> handler)
        {
            _handler = handler;
        }

        public int CallCount { get; private set; }

        public HttpRequestMessage? LastRequest { get; private set; }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            CallCount++;
            LastRequest = request;
            return Task.FromResult(_handler(request));
        }
    }
}
