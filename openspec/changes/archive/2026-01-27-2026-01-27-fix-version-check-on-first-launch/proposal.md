# Proposal: Fix Version Check on First Launch

## Context

Currently, Desktop Ink displays a version update notification even on the first application launch. This occurs because the version check logic does not distinguish between the first launch and subsequent launches.

When a user installs Desktop Ink and launches it for the first time, the `LastChecked` property in `VersionCheckSettings` is `null`. The current implementation only skips version checks when `LastChecked` has a value and is within 24 hours. As a result, the first launch performs a version check against GitHub's latest release, and if a newer version exists, displays an update notification.

This behavior creates a poor user experience:
- Users who just installed the latest version may see an immediate update notification
- The notification appears before users have had a chance to use the application
- It creates confusion about whether the installed version is correct

## Problem Statement

The version check notification should not appear on first launch. The installed version should be treated as the user's baseline version, and notifications should only appear for versions released after the first launch.

## Proposed Solution

Modify `VersionChecker.CheckForUpdatesAsync` to detect first launch and handle it appropriately:

1. Check if `LastChecked` is `null` (indicates first launch)
2. If first launch:
   - Set `SkippedVersion` to the current application version
   - Set `LastChecked` to the current timestamp
   - Return early without performing version check
3. If not first launch, proceed with existing logic

This approach treats the installed version as the baseline and prevents unnecessary notifications while preserving the ability to notify users about future releases.

## Impact

- **User Experience**: Eliminates confusing first-launch notifications
- **Code Changes**: Minimal - single method modification in `VersionChecker.cs`
- **Testing**: Requires testing both first launch and subsequent launch scenarios
- **Backward Compatibility**: Fully compatible - only affects new installations

## Alternatives Considered

1. **Add a separate "IsFirstLaunch" flag**: More complex, requires additional state management
2. **Skip version check entirely on first launch**: Would not record the baseline version
3. **Show notification but with different messaging**: Still creates user confusion

The proposed solution is the simplest and most effective approach that aligns with user expectations.
