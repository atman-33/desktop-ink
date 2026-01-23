# Design: Per-Monitor Draw Mode

## Architecture Overview
This change modifies the draw mode control system from a global all-monitors approach to a selective per-monitor approach based on the control palette's location.

## Key Design Decisions

### 1. Monitor Identification Strategy
**Decision**: Use monitor bounds (Rect) as the identifier for matching overlays to monitors.

**Rationale**:
- Each OverlayWindow already knows its bounds from construction
- ControlWindow can calculate its center point and determine which monitor bounds contain it
- Bounds provide a stable, unambiguous way to match windows to monitors
- Avoids need to store HMONITOR handles which may change across display settings changes

**Implementation**:
- OverlayWindow stores its monitor bounds as passed during construction
- ControlWindow uses `MonitorFromPoint` (or WPF `Screen` utilities) to determine which monitor contains its center
- OverlayManager compares bounds to determine which overlay corresponds to the palette's monitor

### 2. Palette Position Tracking
**Decision**: Track palette position changes via WPF `LocationChanged` event and calculate center point on each change.

**Rationale**:
- LocationChanged fires during drag operations, providing real-time updates
- Center point provides the most intuitive definition of "which monitor the palette is on"
- Avoids polling or complex window message handling

**Implementation**:
- Subscribe to LocationChanged in ControlWindow
- Calculate center point using window position + (width/2, height/2)
- Detect monitor changes by comparing new monitor to previous monitor
- Fire event/callback when monitor changes

### 3. Mode Control Redesign
**Decision**: OverlayManager maintains global mode state but applies it selectively based on palette monitor.

**Rationale**:
- Preserves existing mode toggle semantics (user toggles "draw mode" not "draw mode for this monitor")
- Simplifies state management - one source of truth for intended mode
- Allows easy future extension (e.g., hotkey to toggle all monitors again if needed)

**Implementation**:
```
when mode changes to Draw:
  - for overlay on palette monitor: set Draw mode
  - for other overlays: set PassThrough mode

when mode changes to PassThrough:
  - for all overlays: set PassThrough mode

when palette moves to different monitor:
  - if current mode is Draw:
    - set previous monitor overlay to PassThrough
    - set new monitor overlay to Draw
  - if current mode is PassThrough:
    - no change needed (all already PassThrough)
```

### 4. Temporary Mode Handling
**Decision**: Apply same per-monitor logic to temporary draw mode.

**Rationale**:
- Consistency: users expect temporary mode to behave the same as permanent mode
- Same use case: user wants to draw on one monitor while working on others
- Simpler mental model: "draw mode only works where the palette is"

**Implementation**:
- ActivateTemporaryDrawMode: only set Draw on palette monitor's overlay
- DeactivateTemporaryDrawMode: only clear strokes and return to PassThrough on palette monitor's overlay

### 5. Clear-All Behavior
**Decision**: Determine during implementation whether clear-all should clear all monitors or just the palette monitor.

**Rationale**:
- Argument for all monitors: "clear all" implies global action
- Argument for palette monitor only: consistent with per-monitor mode paradigm
- Need user feedback to determine most intuitive behavior
- Current tasks.md leaves this as a decision point during testing

**Initial Implementation**: Clear all monitors (preserve existing behavior) and note in testing tasks to verify user preference.

## Component Responsibilities

### OverlayWindow
- Store monitor bounds (for identification)
- Respond to SetMode commands (unchanged)
- No awareness of palette or other overlays

### ControlWindow
- Track current monitor based on center point
- Detect monitor changes during position updates
- Notify OverlayManager of current monitor on mode changes and monitor changes
- No direct awareness of overlays

### OverlayManager
- Receive palette monitor information from ControlWindow
- Apply mode changes selectively based on palette monitor
- Match overlays to monitors using bounds comparison
- Handle both permanent and temporary mode with per-monitor logic

## Data Flow

### Startup
1. OverlayManager creates OverlayWindows with monitor bounds
2. ControlWindow initializes on primary monitor
3. OverlayManager sets primary monitor overlay to PassThrough (default)
4. Other monitor overlays also PassThrough

### Mode Toggle
1. User clicks toggle button in ControlWindow
2. ControlWindow calls OverlayManager.ToggleMode()
3. OverlayManager sets global _mode state
4. OverlayManager queries ControlWindow for current monitor
5. OverlayManager applies Draw mode to matching overlay only
6. OverlayManager applies PassThrough to other overlays
7. ModeChanged event fires with new mode

### Palette Drag
1. User drags ControlWindow to different monitor
2. LocationChanged event fires
3. ControlWindow calculates center point and current monitor
4. If monitor changed from previous:
   - ControlWindow notifies OverlayManager of new monitor
   - If current mode is Draw:
     - OverlayManager sets previous monitor overlay to PassThrough
     - OverlayManager sets new monitor overlay to Draw

### Temporary Mode
1. User presses Shift key (or other temp trigger)
2. KeyboardHookManager calls OverlayManager.ActivateTemporaryDrawMode()
3. OverlayManager queries ControlWindow for current monitor
4. OverlayManager sets only that monitor's overlay to Draw (temporary)
5. On Shift release: DeactivateTemporaryDrawMode() reverses changes for that monitor only

## Testing Strategy
- Unit tests for bounds comparison logic
- Unit tests for monitor change detection
- Integration tests for mode control with simulated monitor configurations
- Manual testing with real multi-monitor setup (required for final verification)

## Risks and Mitigations
**Risk**: Palette positioned exactly on monitor boundary - ambiguous which monitor it's on
- **Mitigation**: Center point provides clear, deterministic answer

**Risk**: Display settings change while palette is open
- **Mitigation**: Existing RefreshOverlays mechanism will recreate overlays; palette will recalculate monitor

**Risk**: User confused about why only one monitor draws
- **Mitigation**: Intuitive behavior (palette location determines draw location); can add tooltip hint if needed

## Future Enhancements
- Optional hotkey to toggle "all monitors" vs "palette monitor only" mode
- Visual indicator on overlays showing which is currently draw-enabled
- Remember palette monitor across app restarts
