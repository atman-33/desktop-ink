using System.IO;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace DesktopInk.Infrastructure;

public sealed class AppSettings
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        WriteIndented = true
    };

    [JsonPropertyName("versionCheck")]
    public VersionCheckSettings VersionCheck { get; set; } = new();

    public static AppSettings Load(string? pathOverride = null)
    {
        var path = pathOverride ?? ResolveSettingsPath();

        try
        {
            if (!File.Exists(path))
            {
                var settings = new AppSettings();
                settings.Save(path);
                return settings;
            }

            var json = File.ReadAllText(path, Encoding.UTF8);
            var settingsFromFile = JsonSerializer.Deserialize<AppSettings>(json, JsonOptions) ?? new AppSettings();
            settingsFromFile.VersionCheck ??= new VersionCheckSettings();
            return settingsFromFile;
        }
        catch (Exception ex)
        {
            AppLog.Error("Failed to load settings. Falling back to defaults.", ex);
            var settings = new AppSettings();
            settings.Save(path);
            return settings;
        }
    }

    public void Save(string? pathOverride = null)
    {
        var path = pathOverride ?? ResolveSettingsPath();

        try
        {
            var directory = Path.GetDirectoryName(path);
            if (!string.IsNullOrWhiteSpace(directory))
            {
                Directory.CreateDirectory(directory);
            }

            var json = JsonSerializer.Serialize(this, JsonOptions);
            File.WriteAllText(path, json, Encoding.UTF8);
        }
        catch (Exception ex)
        {
            AppLog.Error("Failed to save settings.", ex);
        }
    }

    internal static string ResolveSettingsPath()
    {
        var root = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        return Path.Combine(root, "DesktopInk", "settings.json");
    }
}

public sealed class VersionCheckSettings
{
    public bool Enabled { get; set; } = true;

    public string? SkippedVersion { get; set; }

    public DateTime? LastChecked { get; set; }
}
