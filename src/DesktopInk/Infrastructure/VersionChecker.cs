using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace DesktopInk.Infrastructure;

public sealed class VersionChecker : IDisposable
{
    private static readonly Uri LatestReleaseEndpoint = new("https://api.github.com/repos/atman-33/desktop-ink/releases/latest");
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private readonly HttpClient _httpClient;
    private readonly bool _disposeHttpClient;

    public VersionChecker(HttpClient? httpClient = null)
    {
        if (httpClient is null)
        {
            _httpClient = new HttpClient
            {
                Timeout = TimeSpan.FromSeconds(5)
            };
            _disposeHttpClient = true;
        }
        else
        {
            _httpClient = httpClient;
            _disposeHttpClient = false;
        }
    }

    public void Dispose()
    {
        if (_disposeHttpClient)
        {
            _httpClient.Dispose();
        }
    }

    /// <summary>
    /// Checks for updates against the GitHub API.
    /// <para>
    /// On first launch (when LastChecked is null), the check is skipped and the current version
    /// is set as the baseline SkippedVersion to prevent immediate update notifications.
    /// </para>
    /// </summary>

    public async Task<VersionCheckResult> CheckForUpdatesAsync(string currentVersion, VersionCheckSettings settings, CancellationToken ct)
    {
        if (!settings.Enabled)
        {
            AppLog.Info("Version check disabled via settings.");
            return VersionCheckResult.Disabled();
        }

        if (!settings.LastChecked.HasValue)
        {
            AppLog.Info($"First launch detected. Setting baseline version to '{currentVersion}'.");
            settings.SkippedVersion = currentVersion;
            settings.LastChecked = DateTime.UtcNow;
            return VersionCheckResult.Skipped();
        }

        if (DateTime.UtcNow - settings.LastChecked.Value < TimeSpan.FromDays(1))
        {
            AppLog.Info("Version check skipped due to daily cache.");
            return VersionCheckResult.Skipped();
        }

        settings.LastChecked = DateTime.UtcNow;

        try
        {
            using var request = new HttpRequestMessage(HttpMethod.Get, LatestReleaseEndpoint);
            request.Headers.UserAgent.ParseAdd("DesktopInk");
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            using var response = await _httpClient.SendAsync(request, ct).ConfigureAwait(false);
            if (!response.IsSuccessStatusCode)
            {
                AppLog.Error($"Version check failed: HTTP {(int)response.StatusCode} {response.ReasonPhrase}.");
                return VersionCheckResult.Failed();
            }

            var payload = await response.Content.ReadAsStringAsync(ct).ConfigureAwait(false);
            var release = JsonSerializer.Deserialize<GitHubRelease>(payload, JsonOptions);
            if (release is null || string.IsNullOrWhiteSpace(release.TagName))
            {
                AppLog.Error("Version check failed: missing release tag.");
                return VersionCheckResult.Failed();
            }

            var latestVersion = NormalizeVersion(release.TagName);
            if (!TryCompareVersions(currentVersion, latestVersion, out var comparison))
            {
                AppLog.Error($"Version check failed: invalid version format current='{currentVersion}', latest='{latestVersion}'.");
                return VersionCheckResult.Failed();
            }

            var isNewVersion = comparison < 0;
            if (isNewVersion)
            {
                AppLog.Info($"Update available. Current='{currentVersion}', Latest='{latestVersion}'.");
            }
            else
            {
                AppLog.Info($"No update available. Current='{currentVersion}', Latest='{latestVersion}'.");
            }

            return new VersionCheckResult(
                isNewVersion,
                latestVersion,
                release.HtmlUrl,
                release.Body);
        }
        catch (Exception ex)
        {
            AppLog.Error("Version check failed due to exception.", ex);
            return VersionCheckResult.Failed();
        }
    }

    public static bool IsNewerVersion(string currentVersion, string latestVersion)
    {
        return TryCompareVersions(currentVersion, latestVersion, out var comparison) && comparison < 0;
    }

    internal static bool TryCompareVersions(string currentVersion, string latestVersion, out int comparison)
    {
        comparison = 0;

        if (!TryParseSemanticVersion(currentVersion, out var current) ||
            !TryParseSemanticVersion(latestVersion, out var latest))
        {
            return false;
        }

        comparison = CompareSemanticVersions(current, latest);
        return true;
    }

    private static int CompareSemanticVersions(SemanticVersion current, SemanticVersion latest)
    {
        var major = current.Major.CompareTo(latest.Major);
        if (major != 0)
        {
            return major;
        }

        var minor = current.Minor.CompareTo(latest.Minor);
        if (minor != 0)
        {
            return minor;
        }

        var patch = current.Patch.CompareTo(latest.Patch);
        if (patch != 0)
        {
            return patch;
        }

        if (current.HasPreRelease != latest.HasPreRelease)
        {
            return current.HasPreRelease ? -1 : 1;
        }

        if (current.HasPreRelease && latest.HasPreRelease)
        {
            return string.CompareOrdinal(current.PreRelease, latest.PreRelease);
        }

        return 0;
    }

    private static bool TryParseSemanticVersion(string input, out SemanticVersion version)
    {
        version = default;

        if (string.IsNullOrWhiteSpace(input))
        {
            return false;
        }

        var trimmed = input.Trim();
        if (trimmed.StartsWith('v') || trimmed.StartsWith('V'))
        {
            trimmed = trimmed[1..];
        }

        var parts = trimmed.Split('-', 2, StringSplitOptions.RemoveEmptyEntries);
        var core = parts[0];
        var preRelease = parts.Length > 1 ? parts[1] : null;

        var numbers = core.Split('.', StringSplitOptions.RemoveEmptyEntries);
        if (numbers.Length < 3)
        {
            return false;
        }

        if (!int.TryParse(numbers[0], out var major) ||
            !int.TryParse(numbers[1], out var minor) ||
            !int.TryParse(numbers[2], out var patch))
        {
            return false;
        }

        version = new SemanticVersion(major, minor, patch, preRelease);
        return true;
    }

    private static string NormalizeVersion(string version)
    {
        if (string.IsNullOrWhiteSpace(version))
        {
            return version;
        }

        var trimmed = version.Trim();
        return trimmed.StartsWith('v') || trimmed.StartsWith('V')
            ? trimmed[1..]
            : trimmed;
    }

    private readonly record struct SemanticVersion(int Major, int Minor, int Patch, string? PreRelease)
    {
        public bool HasPreRelease => !string.IsNullOrWhiteSpace(PreRelease);
    }

    private sealed class GitHubRelease
    {
        [JsonPropertyName("tag_name")]
        public string? TagName { get; set; }

        [JsonPropertyName("html_url")]
        public string? HtmlUrl { get; set; }

        [JsonPropertyName("body")]
        public string? Body { get; set; }
    }
}

public sealed record VersionCheckResult(
    bool IsNewVersionAvailable,
    string? LatestVersion,
    string? ReleaseNotesUrl,
    string? ReleaseNotes)
{
    public static VersionCheckResult Disabled() => new(false, null, null, null);

    public static VersionCheckResult Skipped() => new(false, null, null, null);

    public static VersionCheckResult Failed() => new(false, null, null, null);
}
