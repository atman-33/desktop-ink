# Add Version Update Notification

**Change ID**: `add-version-update-notification`  
**Status**: Proposed  
**Created**: 2026-01-24

## Overview

Add an unobtrusive version update notification system that informs users when a new version of Desktop Ink is available on GitHub releases. The notification appears once on startup, provides clear action options, and respects user preferences to skip specific versions.

## Context

Desktop Ink is distributed as a standalone executable without an installer or automatic update mechanism. Users manually download new versions from GitHub releases. Currently, there is no way for users to know when updates are available unless they actively check the repository.

This change introduces a lightweight notification system that:
- Checks for updates on application startup (with delay to avoid disrupting initialization)
- Uses GitHub's public API to retrieve the latest release version
- Displays a modal dialog when a new version is available
- Allows users to open the download page, postpone the notification, or skip a specific version
- Persists user preferences locally to avoid repeated notifications for skipped versions

## Goals

1. **Improve User Awareness**: Ensure users know when new features and bug fixes are available
2. **Respect User Choice**: Provide clear options without forcing actions
3. **Minimize Disruption**: Keep the notification simple, non-intrusive, and appear only at startup
4. **Maintain Simplicity**: Avoid complex update mechanisms; just notify and link to GitHub releases
5. **Handle Offline Gracefully**: Fail silently when network is unavailable

## Non-Goals

- Automatic downloads or installations
- Background periodic checks during runtime
- Update downloading or patching within the application
- Support for alternative distribution channels (only GitHub releases)

## Related Capabilities

This change introduces a new capability:
- **version-notification**: System to check version and notify users

This change may interact with:
- **control-palette**: Could potentially add a "Check for Updates" option in the future
- **runtime-safety-performance**: Async operations must not impact startup performance

## Impact

- **New Files**: `VersionChecker.cs`, `UpdateNotificationDialog.xaml`, `UpdateNotificationDialog.xaml.cs`, `AppSettings.cs`
- **Modified Files**: `App.xaml.cs` (add startup check), `DesktopInk.csproj` (add System.Text.Json dependency)
- **User Data**: New settings file at `%APPDATA%\DesktopInk\settings.json`
- **External Dependencies**: GitHub REST API (public, no authentication required)

## Risks & Mitigations

| Risk | Mitigation |
|------|-----------|
| Network timeout delays startup | Use short timeout (5s) and run check 5 seconds after startup |
| GitHub API rate limits | Limit checks to once per day, cache last check timestamp |
| Dialog blocks user workflow | Show only once per session, provide "Skip this version" option |
| Version comparison errors | Use semantic versioning parser with fallback to string comparison |

## Validation

- Manual testing with mock GitHub API responses
- Unit tests for version comparison logic
- Integration test for settings persistence
- Verify graceful handling of network failures
- Test with various version formats (v1.2.3, 1.2.3-beta, etc.)

## Success Metrics

- Users can see update notifications within 5-10 seconds of startup
- No noticeable impact on application startup performance
- Settings are correctly persisted across sessions
- No crashes or errors when network is unavailable
