# Tasks: Fix Version Check on First Launch

## Implementation Tasks

1. [x] **Modify VersionChecker.CheckForUpdatesAsync method**
   - Add first-launch detection logic at the beginning of the method
   - If `settings.LastChecked` is `null`, treat as first launch:
     - Set `settings.SkippedVersion` to `currentVersion`
     - Set `settings.LastChecked` to `DateTime.UtcNow`
     - Return `VersionCheckResult.Skipped()`
   - Preserve existing logic for subsequent launches

2. [x] **Add unit tests for first-launch scenario**
   - Test that first launch skips version check
   - Test that `SkippedVersion` is set to current version
   - Test that `LastChecked` is set on first launch
   - Test that subsequent launches proceed with normal version check

3. [x] **Add unit tests for edge cases**
   - Test behavior when `LastChecked` is `null` but `SkippedVersion` already has a value
   - Test that second launch after first launch proceeds normally

4. [x] **Manual verification**
   - Delete `settings.json` from `%APPDATA%\DesktopInk`
   - Launch application
   - Verify no version notification appears
   - Check `settings.json` contains correct `skippedVersion` and `lastChecked`
   - Launch application again
   - Verify normal version check behavior

## Testing Strategy

- Unit tests will cover the `CheckForUpdatesAsync` logic
- Manual testing will verify the end-to-end first-launch behavior
- No integration tests required as the change is isolated to one method
