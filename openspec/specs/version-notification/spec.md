# version-notification Specification

## Purpose
TBD - created by archiving change add-version-update-notification. Update Purpose after archive.
## Requirements
### Requirement: Check for updates on application startup

The system SHALL check for the latest version on GitHub releases when the application starts, after a 5-second delay to avoid blocking initialization.

#### Scenario: Version check is triggered after startup

- **GIVEN** the application is starting up
- **WHEN** 5 seconds have passed since `OnStartup` completed
- **THEN** the system initiates an asynchronous version check against the GitHub API
- **AND** the check does not block the UI thread or prevent the application from being usable.

#### Scenario: Version check respects rate limiting

- **GIVEN** the application was started and checked for updates within the last 24 hours
- **WHEN** the application starts again
- **THEN** the version check is skipped
- **AND** the last check timestamp is preserved in settings.

#### Scenario: Version check handles network timeout gracefully

- **GIVEN** the application is starting and initiating a version check
- **WHEN** the GitHub API does not respond within 5 seconds
- **THEN** the version check is aborted silently
- **AND** no error dialog is shown to the user
- **AND** the failure is logged for diagnostic purposes.

#### Scenario: Version check handles offline scenario gracefully

- **GIVEN** the application is starting without network connectivity
- **WHEN** the version check attempts to contact the GitHub API
- **THEN** the request fails silently
- **AND** no error dialog is shown to the user
- **AND** the application continues normal operation.

### Requirement: Compare current version with latest release

The system SHALL parse the current application version from assembly metadata and compare it with the latest release version from GitHub using semantic versioning rules.

#### Scenario: Latest version is newer than current version

- **GIVEN** the current version is `1.3.0`
- **WHEN** the GitHub API returns latest version `1.4.0`
- **THEN** the system detects a new version is available.

#### Scenario: Current version is up-to-date

- **GIVEN** the current version is `1.4.0`
- **WHEN** the GitHub API returns latest version `1.4.0`
- **THEN** the system determines no update is needed
- **AND** no notification is shown.

#### Scenario: Version tags with 'v' prefix are handled correctly

- **GIVEN** the GitHub release uses tag format `v1.4.0`
- **WHEN** the system compares it with current version `1.3.0`
- **THEN** the 'v' prefix is stripped and versions are compared correctly as `1.4.0 > 1.3.0`.

#### Scenario: Invalid version formats are handled safely

- **GIVEN** the GitHub API returns an unexpected version format
- **WHEN** the system attempts to parse the version
- **THEN** the parse fails safely
- **AND** no notification is shown
- **AND** the error is logged.

### Requirement: Display update notification dialog

The system SHALL display a modal dialog when a new version is detected, showing version information and providing three action options.

#### Scenario: Update dialog shows version information

- **GIVEN** a new version `1.4.0` is available
- **WHEN** the update notification dialog is displayed
- **THEN** the dialog shows current version `1.3.0`
- **AND** the dialog shows latest version `1.4.0`
- **AND** the dialog shows a preview of the release notes.

#### Scenario: User opens download page

- **GIVEN** the update notification dialog is displayed
- **WHEN** the user clicks "Open Download Page"
- **THEN** the default browser opens the GitHub release page
- **AND** the dialog closes
- **AND** no settings are modified.

#### Scenario: User chooses to be reminded later

- **GIVEN** the update notification dialog is displayed
- **WHEN** the user clicks "Remind Later"
- **THEN** the dialog closes
- **AND** the notification will appear again on next startup (if still newer).

#### Scenario: User skips a specific version

- **GIVEN** the update notification dialog is displayed for version `1.4.0`
- **WHEN** the user clicks "Skip This Version"
- **THEN** the dialog closes
- **AND** `settings.json` is updated with `skippedVersion: "1.4.0"`
- **AND** the notification for `1.4.0` will not appear again.

#### Scenario: Skipped version does not suppress newer versions

- **GIVEN** the user previously skipped version `1.4.0`
- **WHEN** version `1.5.0` is released and detected
- **THEN** the update notification is shown for `1.5.0`
- **AND** the skipped version setting is updated to reflect the new notification context.

### Requirement: Persist user preferences in settings file

The system SHALL store version check preferences in a JSON file located at `%APPDATA%\DesktopInk\settings.json`.

#### Scenario: Settings file is created on first run

- **GIVEN** the application is running for the first time
- **WHEN** the version check feature initializes
- **THEN** a new settings file is created at `%APPDATA%\DesktopInk\settings.json`
- **AND** the settings contain default values (enabled: true, skippedVersion: null, lastChecked: null).

#### Scenario: Settings are loaded on startup

- **GIVEN** a settings file exists with `skippedVersion: "1.3.5"`
- **WHEN** the application starts
- **THEN** the settings are loaded into memory
- **AND** the skipped version value is respected during version comparison.

#### Scenario: Settings are updated when user skips a version

- **GIVEN** the user clicks "Skip This Version" for version `1.4.0`
- **WHEN** the dialog closes
- **THEN** the settings file is updated with `skippedVersion: "1.4.0"`
- **AND** the `lastChecked` timestamp is updated to current UTC time.

#### Scenario: Corrupt settings file is handled gracefully

- **GIVEN** the settings file exists but contains invalid JSON
- **WHEN** the application attempts to load settings
- **THEN** the system falls back to default settings
- **AND** a warning is logged
- **AND** the corrupt file is overwritten with valid defaults on next save.

### Requirement: GitHub API integration

The system SHALL use the GitHub REST API v3 to retrieve the latest release information from the `atman-33/desktop-ink` repository.

#### Scenario: Successful API call returns release data

- **GIVEN** the version check is triggered
- **WHEN** the system makes a GET request to `https://api.github.com/repos/atman-33/desktop-ink/releases/latest`
- **THEN** the response contains `tag_name`, `html_url`, and `body` fields
- **AND** the system parses the data successfully.

#### Scenario: API request includes appropriate headers

- **GIVEN** the version check is triggered
- **WHEN** the system makes a GET request to the GitHub API
- **THEN** the request includes a `User-Agent` header identifying Desktop Ink
- **AND** the request accepts JSON responses (`Accept: application/json`).

#### Scenario: API rate limit is respected

- **GIVEN** the application checks for updates multiple times in a short period
- **WHEN** GitHub's rate limit (60 requests/hour) is approached
- **THEN** the system's 24-hour check interval prevents excessive requests
- **AND** rate limit errors are logged but do not crash the application.

### Requirement: Update notification only appears when appropriate

The system SHALL only display the update notification dialog when a new version is detected and the user has not skipped that specific version.

#### Scenario: No notification when version is skipped

- **GIVEN** the user previously skipped version `1.4.0`
- **WHEN** the version check detects version `1.4.0` as the latest
- **THEN** no notification dialog is shown.

#### Scenario: No notification when already up-to-date

- **GIVEN** the current version matches the latest version
- **WHEN** the version check completes
- **THEN** no notification dialog is shown.

#### Scenario: Notification shown only once per session

- **GIVEN** a new version is detected and the dialog is shown
- **WHEN** the user closes the dialog with "Remind Later"
- **THEN** the dialog does not appear again during the same session
- **AND** the dialog will appear again on the next application startup.

### Requirement: Version check can be disabled

The system SHALL allow users to disable the version check feature by modifying the settings file.

#### Scenario: Version check is disabled in settings

- **GIVEN** the settings file contains `versionCheck.enabled: false`
- **WHEN** the application starts
- **THEN** the version check is not performed
- **AND** no network requests are made to GitHub.

#### Scenario: Version check is re-enabled in settings

- **GIVEN** the settings file previously had `versionCheck.enabled: false`
- **WHEN** the user edits the settings to `versionCheck.enabled: true`
- **THEN** version checks resume on next startup.

### Requirement: Error logging for diagnostics

The system SHALL log all version check operations, including successes, failures, and user actions, to the application log for troubleshooting.

#### Scenario: Successful version check is logged

- **GIVEN** the version check completes successfully
- **WHEN** the latest version is retrieved
- **THEN** an info-level log entry is created with current and latest versions.

#### Scenario: Network errors are logged

- **GIVEN** the version check fails due to network timeout
- **WHEN** the error occurs
- **THEN** an error-level log entry is created with exception details
- **AND** the application continues without crashing.

#### Scenario: User actions are logged

- **GIVEN** the user interacts with the update notification dialog
- **WHEN** the user clicks any of the three action buttons
- **THEN** an info-level log entry is created recording the user's choice.

### Requirement: Skip version check on first launch

The system SHALL skip version check on the first application launch and establish the installed version as the baseline version.

#### Scenario: First launch with no previous settings

- **GIVEN** the application is launching for the first time
- **AND** no `settings.json` file exists in `%APPDATA%\DesktopInk`
- **WHEN** the version check is initiated
- **THEN** the system detects `LastChecked` is `null`
- **AND** the system sets `SkippedVersion` to the current application version
- **AND** the system sets `LastChecked` to the current timestamp
- **AND** the version check returns `Skipped` without making a GitHub API call
- **AND** no update notification dialog is displayed.

#### Scenario: First launch with existing but corrupted settings

- **GIVEN** `settings.json` exists but `LastChecked` is `null` (due to corruption or manual edit)
- **WHEN** the version check is initiated
- **THEN** the system treats it as first launch
- **AND** sets `SkippedVersion` to current version
- **AND** sets `LastChecked` to current timestamp
- **AND** no notification is displayed.

#### Scenario: Second launch after first launch

- **GIVEN** the application was launched once before
- **AND** `SkippedVersion` is set to the initial installed version (e.g., "1.5.0")
- **AND** `LastChecked` is more than 24 hours ago
- **WHEN** the version check is initiated
- **THEN** the system performs a normal version check against GitHub API
- **AND** if the latest version is "1.5.0" or older, no notification is shown
- **AND** if the latest version is "1.6.0" or newer, the update notification is displayed.

#### Scenario: First launch baseline is preserved

- **GIVEN** the application set `SkippedVersion` to "1.5.0" on first launch
- **AND** the user later launches the application when "1.6.0" is available
- **WHEN** the user clicks "Remind Later" on the update notification
- **THEN** `SkippedVersion` remains "1.5.0"
- **AND** the notification will appear again on the next launch (after 24 hours).

#### Scenario: First launch baseline is overridden by explicit skip

- **GIVEN** the application set `SkippedVersion` to "1.5.0" on first launch
- **AND** the user later sees a notification for version "1.6.0"
- **WHEN** the user clicks "Skip This Version"
- **THEN** `SkippedVersion` is updated to "1.6.0"
- **AND** no notification is shown for "1.6.0" on subsequent launches
- **AND** a notification will appear for "1.7.0" or later versions.

#### Scenario: Logging indicates first launch

- **GIVEN** the application is launching for the first time
- **WHEN** the version check detects `LastChecked` is `null`
- **THEN** a log message is written: "First launch detected. Setting baseline version to '{currentVersion}'."
- **AND** the current version value is included in the log for diagnostics.

