# Tasks for add-version-update-notification

## Implementation Tasks

### 1. Create settings infrastructure
- [x] Create `AppSettings.cs` class with version check settings model
- [x] Implement JSON serialization/deserialization using System.Text.Json
- [x] Implement settings file path resolution (`%APPDATA%\DesktopInk\settings.json`)
- [x] Add safe file I/O with error handling for missing/corrupt files
- [x] Add unit tests for settings load/save

### 2. Implement version checker service
- [x] Create `VersionChecker.cs` class
- [x] Implement GitHub API client for latest release endpoint
- [x] Add semantic version comparison logic (current vs latest)
- [x] Implement caching logic (check once per day maximum)
- [x] Add timeout and error handling for HTTP requests
- [x] Add unit tests for version comparison
- [x] Add integration tests with mock HTTP responses

### 3. Create update notification dialog
- [x] Create `UpdateNotificationDialog.xaml` WPF window
- [x] Design modal dialog layout with version info and release notes preview
- [x] Add three action buttons: "Open Download Page", "Remind Later", "Skip This Version"
- [x] Create `UpdateNotificationDialog.xaml.cs` code-behind
- [x] Implement button handlers (open browser, close dialog, update settings)
- [x] Style dialog to match application aesthetic

### 4. Integrate version check into application startup
- [x] Add System.Text.Json NuGet package to `DesktopInk.csproj`
- [x] Modify `App.xaml.cs` `OnStartup` to trigger version check
- [x] Implement async check with 5-second delay after window initialization
- [x] Add AppSettings initialization on startup
- [x] Handle version check result and show dialog when update available
- [x] Ensure exceptions don't crash the application

### 5. Testing and validation
- [ ] Manual test: Fresh install scenario (no settings file)
- [ ] Manual test: New version available scenario
- [ ] Manual test: Already up-to-date scenario
- [ ] Manual test: Network offline scenario
- [ ] Manual test: Skipped version persistence
- [ ] Manual test: Dialog interaction (all three buttons)
- [ ] Verify no startup performance degradation
- [ ] Test with various version formats (v1.2.3, 1.2.3-beta, etc.)

### 6. Documentation
- [x] Update README.md to mention version notification feature
- [x] Add settings.json schema documentation
- [x] Update release notes template

## Validation Criteria

- Settings file is created correctly on first run
- Version check completes within 5 seconds (timeout) or startup delay
- Dialog appears only when new version is detected
- User choices are persisted correctly
- Application doesn't crash on network errors
- No noticeable startup delay for users

## Dependencies

- No blocking dependencies; this is a new standalone feature
- Uses existing WPF infrastructure
- Requires internet connection for version check (gracefully degrades offline)

## Estimated Effort

- Implementation: ~4-6 hours
- Testing: ~2 hours
- Documentation: ~1 hour
- **Total**: ~7-9 hours
