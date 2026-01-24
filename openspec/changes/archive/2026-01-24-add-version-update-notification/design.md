# Design for add-version-update-notification

## Architecture Overview

The version update notification system consists of four main components:

```
┌─────────────────────────────────────────────────────────────┐
│                        App.xaml.cs                           │
│  - OnStartup: Trigger version check after 5s delay          │
│  - Load AppSettings                                          │
└──────────────────────┬──────────────────────────────────────┘
                       │
                       ├──> AppSettings.cs
                       │    - Load/Save settings from JSON
                       │    - Settings: enabled, skippedVersion, lastChecked
                       │
                       ├──> VersionChecker.cs
                       │    - CheckForUpdatesAsync()
                       │    - GitHub API client (releases/latest)
                       │    - Version comparison (SemVer)
                       │    - Result: { IsNewVersion, LatestVersion, ReleaseUrl }
                       │
                       └──> UpdateNotificationDialog.xaml
                            - Modal dialog with version info
                            - Buttons: Download / Remind Later / Skip
                            - Updates AppSettings on user choice
```

## Key Design Decisions

### 1. **Async Startup with Delay**

**Decision**: Trigger version check 5 seconds after `OnStartup` completes.

**Rationale**:
- Avoids blocking application initialization
- Allows main window and overlays to appear immediately
- Gives time for network stack to initialize
- User sees the app as responsive

**Implementation**:
```csharp
protected override void OnStartup(StartupEventArgs e)
{
    base.OnStartup(e);
    
    // ... existing initialization ...
    
    // Trigger version check after delay
    _ = Task.Run(async () =>
    {
        await Task.Delay(5000);
        await CheckForUpdatesAsync();
    });
}
```

### 2. **Settings Storage**

**Decision**: Use `%APPDATA%\DesktopInk\settings.json` with System.Text.Json.

**Rationale**:
- Standard location for Windows application data
- JSON is human-readable and easily editable for advanced users
- System.Text.Json is built-in (.NET 10) and lightweight
- Future-proof for additional settings

**Schema**:
```json
{
  "versionCheck": {
    "enabled": true,
    "skippedVersion": null,
    "lastChecked": "2026-01-24T10:00:00Z"
  }
}
```

### 3. **GitHub API Integration**

**Decision**: Use GitHub REST API v3 public endpoint (no authentication).

**Endpoint**: `GET https://api.github.com/repos/atman-33/desktop-ink/releases/latest`

**Rationale**:
- Public API, no auth required
- Rate limit: 60 requests/hour/IP (sufficient for startup-only checks)
- Returns structured JSON with version and release notes
- Reliable and maintained by GitHub

**Response Example**:
```json
{
  "tag_name": "v1.4.0",
  "name": "Version 1.4.0",
  "html_url": "https://github.com/atman-33/desktop-ink/releases/tag/v1.4.0",
  "body": "## Changes\n- Added feature X\n- Fixed bug Y"
}
```

### 4. **Version Comparison**

**Decision**: Use semantic versioning comparison with fallback.

**Approach**:
1. Parse versions as SemVer (major.minor.patch)
2. Compare numerically (e.g., 1.4.0 > 1.3.0)
3. Fallback to string comparison if parsing fails
4. Handle versions with/without "v" prefix

**Edge Cases**:
- Pre-release versions (1.4.0-beta) are considered older than release (1.4.0)
- Invalid formats are logged and treated as "unknown" (no notification)

### 5. **Dialog Design**

**Decision**: Modal WPF dialog with three clear action buttons.

**Layout**:
```
┌──────────────────────────────────────────────┐
│  Desktop Ink - Update Available              │
├──────────────────────────────────────────────┤
│                                               │
│  A new version is available!                 │
│                                               │
│  Current Version:  v1.3.0                    │
│  Latest Version:   v1.4.0                    │
│                                               │
│  Release Notes:                              │
│  ┌──────────────────────────────────────┐   │
│  │ - Added feature X                    │   │
│  │ - Fixed bug Y                        │   │
│  │ - Performance improvements           │   │
│  └──────────────────────────────────────┘   │
│                                               │
│  [Open Download Page]  [Remind Later]  [Skip]│
│                                               │
└──────────────────────────────────────────────┘
```

**Button Behaviors**:
- **Open Download Page**: Launch browser to GitHub release page, close dialog
- **Remind Later**: Close dialog, show again next startup
- **Skip This Version**: Save skipped version to settings, don't show until newer version

### 6. **Error Handling**

**Decision**: Fail silently with logging only.

**Scenarios**:
- **Network Timeout**: Log error, don't show notification
- **Invalid JSON Response**: Log error, don't show notification
- **Settings File Corrupt**: Use default settings, log warning
- **Settings File Locked**: Skip save, log warning

**Rationale**: Version check is a convenience feature, not critical. Errors should not disrupt user workflow.

### 7. **Rate Limiting and Caching**

**Decision**: Check once per day maximum, cache last check timestamp.

**Logic**:
```csharp
if (settings.LastChecked.HasValue && 
    DateTime.UtcNow - settings.LastChecked.Value < TimeSpan.FromDays(1))
{
    // Skip check, already checked recently
    return;
}
```

**Rationale**: Respects GitHub API rate limits and reduces unnecessary network calls.

## Component Specifications

### AppSettings.cs

**Responsibilities**:
- Define settings model
- Load settings from JSON file
- Save settings to JSON file
- Provide default values if file missing

**Public API**:
```csharp
public class AppSettings
{
    public VersionCheckSettings VersionCheck { get; set; }
    
    public static AppSettings Load();
    public void Save();
}

public class VersionCheckSettings
{
    public bool Enabled { get; set; } = true;
    public string? SkippedVersion { get; set; }
    public DateTime? LastChecked { get; set; }
}
```

### VersionChecker.cs

**Responsibilities**:
- Query GitHub API for latest release
- Parse and compare versions
- Return result with latest version info

**Public API**:
```csharp
public class VersionChecker
{
    public async Task<VersionCheckResult> CheckForUpdatesAsync(string currentVersion, CancellationToken ct);
}

public record VersionCheckResult(
    bool IsNewVersionAvailable,
    string? LatestVersion,
    string? ReleaseNotesUrl,
    string? ReleaseNotes
);
```

### UpdateNotificationDialog.xaml

**Responsibilities**:
- Display version information
- Show release notes preview
- Handle user action selection
- Update settings based on user choice

**Public API**:
```csharp
public partial class UpdateNotificationDialog : Window
{
    public UpdateNotificationDialog(VersionCheckResult result);
    
    public UserAction? UserChoice { get; private set; }
}

public enum UserAction
{
    OpenDownloadPage,
    RemindLater,
    SkipVersion
}
```

## Testing Strategy

### Unit Tests
- Version comparison logic (various formats)
- Settings serialization/deserialization
- Default settings creation

### Integration Tests
- HTTP client with mock responses (success, timeout, error)
- Settings file I/O (create, read, update)

### Manual Tests
- First launch (no settings)
- Network offline
- New version available
- Already up-to-date
- Skipped version persistence
- All dialog actions

## Performance Considerations

- Async operations don't block UI thread
- 5-second startup delay prevents UI freeze
- 5-second HTTP timeout prevents long waits
- Settings file is small (~200 bytes)
- GitHub API response is typically <10 KB

## Security Considerations

- HTTPS for GitHub API (encrypted)
- No credentials stored or transmitted
- No code execution from remote sources
- Only opens browser to official GitHub domain
- Settings file is local user data (no sharing)

## Future Enhancements (Out of Scope)

- Manual "Check for Updates" button in Control Palette
- In-app changelog viewer
- Optional automatic download (complex, installer required)
- Custom update notification frequency
- Support for beta/pre-release channels
