# Proposal: Per-Monitor Draw Mode Based on Control Palette Position

## Summary
Enable drawing mode only on the monitor where the control palette is located, allowing users to interact with applications on other monitors while drawing on the palette's monitor.

## Context
Currently, when draw mode is activated, all monitors become draw-enabled simultaneously. This prevents users from interacting with applications on other monitors while drawing, limiting the utility of multi-monitor setups. Users want to be able to draw on one monitor while continuing to work on others.

## Motivation
The current behavior forces all monitors into draw mode together, which is inconvenient for multi-monitor workflows. For example:
- A user presenting on one monitor cannot interact with notes or applications on another monitor during draw mode
- Developers cannot draw annotations on one screen while continuing to code on another
- The control palette's location becomes arbitrary since it doesn't affect which monitor is draw-enabled

## Proposal
Change the draw mode behavior so that only the monitor containing the control palette becomes draw-enabled. The system will:
1. Track the control palette's current monitor position based on its center point
2. Enable draw mode only on the overlay for that monitor
3. Keep all other monitors in pass-through mode
4. Update the active draw monitor dynamically when the palette is dragged to a different monitor
5. Apply the same per-monitor logic to temporary draw mode (Shift key hold)

## Impact
### Affected Capabilities
- `overlay-window`: Mode control logic changes from global to per-overlay
- `control-palette`: Needs to track its monitor position and notify the overlay manager
- `multi-monitor-overlay`: Draw mode behavior changes from all-monitors to single-monitor
- `mode-feedback`: Tooltip text may need adjustment to reflect per-monitor behavior

### User Experience
- **Positive**: Users can draw on one monitor while using applications on others
- **Positive**: More intuitive - draw mode follows the control palette location
- **Change**: Users must move the palette to the desired monitor to draw there
- **Initial position**: On startup, only the primary monitor (where palette appears) will be draw-enabled

### Technical Considerations
- Each `OverlayWindow` needs monitor identification
- `ControlWindow` must track its current monitor and notify `OverlayManager` of changes
- `OverlayManager` needs logic to determine which overlay should be in draw mode based on palette position
- Window position change detection required (during drag operations)

## Alternatives Considered
1. **Hotkey to cycle active monitor**: More explicit control but adds complexity and cognitive load
2. **Click on monitor to activate**: Could cause accidental mode switches
3. **Keep current all-monitors behavior**: Simplest but doesn't solve the user's workflow problem

## Open Questions
None - user has confirmed all design decisions.
