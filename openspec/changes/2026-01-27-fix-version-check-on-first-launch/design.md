# Design: Fix Version Check on First Launch

## Architecture Decision

This is a minimal change that requires no architectural modifications. The fix is implemented entirely within the existing `VersionChecker.CheckForUpdatesAsync` method by adding early-return logic.

## Design Rationale

### Why This Approach?

1. **Minimal Code Change**: Only adds a few lines at the method's beginning
2. **Reuses Existing State**: Leverages `SkippedVersion` mechanism already in place
3. **No New Dependencies**: No new properties or state management required
4. **Clear Semantics**: Setting `SkippedVersion` to current version clearly indicates "user has seen this version"

### Implementation Details

The first-launch detection logic will be placed immediately after the `Enabled` check and before the existing `LastChecked` check:

```csharp
public async Task<VersionCheckResult> CheckForUpdatesAsync(string currentVersion, VersionCheckSettings settings, CancellationToken ct)
{
    if (!settings.Enabled)
    {
        AppLog.Info("Version check disabled via settings.");
        return VersionCheckResult.Disabled();
    }

    // NEW: First-launch detection
    if (!settings.LastChecked.HasValue)
    {
        AppLog.Info($"First launch detected. Setting baseline version to '{currentVersion}'.");
        settings.SkippedVersion = currentVersion;
        settings.LastChecked = DateTime.UtcNow;
        return VersionCheckResult.Skipped();
    }

    // Existing logic continues...
    if (DateTime.UtcNow - settings.LastChecked.Value < TimeSpan.FromDays(1))
    {
        AppLog.Info("Version check skipped due to daily cache.");
        return VersionCheckResult.Skipped();
    }

    // ... rest of method
}
```

### State Transitions

**First Launch:**
- `LastChecked`: `null` → `DateTime.UtcNow`
- `SkippedVersion`: `null` → `currentVersion` (e.g., "1.5.0")
- Result: No notification, version check skipped

**Second Launch (same day):**
- `LastChecked`: within 24 hours
- Result: Skipped due to daily cache

**Second Launch (next day):**
- `LastChecked`: > 24 hours ago
- `SkippedVersion`: "1.5.0"
- If GitHub latest is "1.5.0" or older: No notification
- If GitHub latest is "1.6.0": Notification shown (as expected)

### Interaction with Existing Logic

The fix integrates seamlessly with existing behavior:

1. **"Skip Version" button**: If user skips a version, `SkippedVersion` is updated to the new version, overwriting the initial baseline
2. **"Remind Later" button**: No change to settings, notification will appear on next check
3. **Multiple launches per day**: Daily cache prevents excessive API calls

### Logging

Added log message for first-launch scenario provides clear debugging information without exposing sensitive data.

## Testing Considerations

- **Unit Tests**: Can mock `VersionCheckSettings` with `LastChecked = null` to simulate first launch
- **Manual Testing**: Requires deleting `settings.json` to reset application state
- **No Breaking Changes**: Existing installations will continue to work normally

## Security & Privacy

No security or privacy implications. The change only affects local state management.
