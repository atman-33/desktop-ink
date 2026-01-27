# version-notification Specification Delta

## Added Requirements

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
