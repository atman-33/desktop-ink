# Tasks for Per-Monitor Draw Mode

## Implementation Tasks

### 1. [x] Add monitor identification to OverlayWindow
**Description**: Add properties to identify which monitor each overlay belongs to
- Add monitor identifier field (e.g., HMONITOR handle or bounds)
- Pass monitor information during OverlayWindow construction
- Store monitor information for later comparison

### 2. [x] Add monitor tracking to ControlWindow
**Description**: Implement logic to track which monitor the control palette is currently on
- Add method to determine current monitor based on window center point
- Use Win32 API `MonitorFromPoint` or similar to identify monitor
- Add event or callback to notify when monitor changes

### 3. [x] Implement position change detection in ControlWindow
**Description**: Detect when the palette is dragged to a different monitor
- Hook into window position change events (LocationChanged or similar)
- Calculate center point after position changes
- Determine if monitor has changed compared to previous position
- Trigger notification when monitor changes

### 4. [x] Update OverlayManager mode control logic
**Description**: Change mode setting from global to per-overlay based on palette position
- Add method to set mode for a specific monitor only
- Update `SetMode` to accept optional monitor parameter
- Update `ActivateTemporaryDrawMode` to accept optional monitor parameter
- Update `DeactivateTemporaryDrawMode` to accept optional monitor parameter
- Keep non-palette monitors in PassThrough mode regardless of global mode

### 5. [x] Wire up palette-to-overlay communication
**Description**: Connect ControlWindow monitor changes to OverlayManager
- Add callback or event subscription from OverlayManager to ControlWindow
- Call OverlayManager with current palette monitor on mode changes
- Call OverlayManager with current palette monitor when palette moves to different monitor

### 6. [x] Update mode toggle behavior
**Description**: Ensure mode toggle only affects the palette's monitor
- Modify `ToggleMode` to only affect overlay on palette's monitor
- Ensure ModeChanged event still fires with correct effective mode for UI feedback

### 7. [x] Update temporary draw mode behavior
**Description**: Apply per-monitor logic to temporary mode (Shift hold)
- Modify temporary mode activation to only affect palette's monitor
- Modify temporary mode deactivation to only affect palette's monitor
- Ensure clear-all on temp mode exit only clears the palette's monitor

### 8. [x] Add integration tests
**Description**: Test per-monitor mode control behavior
- Test initial state: only primary monitor in draw mode
- Test palette drag: draw mode follows palette
- Test mode toggle on specific monitor
- Test temporary mode on specific monitor
- Test that non-palette monitors remain in pass-through

### 9. [x] Update documentation
**Description**: Document the per-monitor behavior
- Update README if behavior is described
- Update any user-facing documentation about multi-monitor usage

## Testing Tasks

### 10. Manual verification
**Description**: Verify the feature works as expected in real multi-monitor setup
- [ ] Verify startup: only primary monitor draw-enabled
- [ ] Verify palette drag to secondary: draw mode moves
- [ ] Verify can interact with apps on non-palette monitors
- [ ] Verify temporary mode works per-monitor
- [ ] Verify mode toggle works per-monitor
- [ ] Verify clear-all behavior (determine if should clear all monitors or just palette monitor)

## Order
Tasks should be completed in the order listed (1-10), as each builds on the previous.
