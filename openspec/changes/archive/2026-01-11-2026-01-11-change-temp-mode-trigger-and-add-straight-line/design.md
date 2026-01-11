# Design: Change temporary draw mode trigger to Alt and add straight line drawing with Shift

## Overview
This change addresses two related goals:
1. Move temporary draw mode trigger from Shift to Alt key to avoid conflicts
2. Enable standard constrained drawing behavior (Shift + drag for straight lines)

## Architecture

### Component Changes

#### KeyboardHookManager (Core/KeyboardHookManager.cs)
**Current behavior:**
- Monitors Shift key (VK_LSHIFT=0xA0, VK_RSHIFT=0xA1) for double-click pattern
- Tracks press/release timing to detect double-click + hold
- Raises `TemporaryModeActivated` on Shift double-click + hold
- Raises `TemporaryModeDeactivated` on Shift release

**New behavior:**
- Monitor Alt key (VK_LMENU=0xA4, VK_RMENU=0xA5) instead of Shift
- Same double-click timing and hold logic
- Same event raising pattern

**Implementation approach:**
- Replace virtual key code constants (0xA0/0xA1 → 0xA4/0xA5)
- Rename internal state variables for clarity (`_lastAltPressTime`, `_isAltHeld`, etc.)
- Update log messages to reference Alt instead of Shift
- No changes to timing logic or event interface

**Risk assessment:**
- Low risk: Minimal logic changes, only key code substitution
- Alt key is less commonly used in drawing contexts (reduces accidental activation)
- Alt does not conflict with standard Windows shortcuts in drawing scenarios

#### OverlayWindow (Windows/OverlayWindow.xaml.cs)
**Current behavior:**
- Captures mouse down, move, up events in draw mode
- Adds each mouse position as a point to active `Polyline`
- Renders freehand strokes by connecting all points

**New behavior:**
- Check Shift key state during `OnMouseMove`
- If Shift is held:
  - Calculate angle between stroke start point and current mouse position
  - Determine if angle is closer to horizontal (0°/180°) or vertical (90°/270°)
  - Snap current point to constrained axis
  - Replace all intermediate points with just start and constrained end point
- If Shift is released:
  - Resume adding points normally (freehand mode)

**Implementation approach:**
- Store `_strokeStartPoint` when mouse down occurs
- In `OnMouseMove`, use `Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift)`
- Calculate angle: `Math.Atan2(dy, dx)`
- Snap logic:
  ```
  if (|angle| < 45° or |angle| > 135°) → horizontal snap (keep Y, vary X)
  else → vertical snap (keep X, vary Y)
  ```
- Clear intermediate points in `Polyline.Points` and keep only start + constrained end

**Risk assessment:**
- Medium risk: Modifies core drawing loop
- Need to handle edge cases:
  - Shift pressed mid-stroke (transition from freehand to constrained)
  - Shift released mid-stroke (transition from constrained to freehand)
  - Very short drags (avoid division by zero or unstable angles)
- Performance consideration: Angle calculation on every mouse move (acceptable, simple math)

### Data Flow

#### Alt Double-Click Flow
```
User presses Alt (1st time)
  → KeyboardHookManager detects key down
  → Records timestamp, waits for release

User releases Alt (1st time)
  → KeyboardHookManager detects key up
  → Sets waiting state, starts double-click window

User presses Alt (2nd time within threshold)
  → KeyboardHookManager detects key down within window
  → Sets _isAltHeld = true
  → Raises TemporaryModeActivated event

ControlWindow receives event
  → Calls OverlayManager.ActivateTemporaryDrawMode()
  → OverlayManager sets _isTemporaryDrawMode = true
  → Calls SetMode(OverlayMode.Draw, isTemporary: true) on all overlays

User releases Alt
  → KeyboardHookManager detects key up
  → Sets _isAltHeld = false
  → Raises TemporaryModeDeactivated event

ControlWindow receives event
  → Calls OverlayManager.DeactivateTemporaryDrawMode()
  → OverlayManager clears all strokes across all monitors
  → Calls SetMode(OverlayMode.PassThrough) on all overlays
```

#### Shift Constraint Flow
```
User presses left mouse button in draw mode
  → OverlayWindow.OnMouseLeftButtonDown
  → Stores _strokeStartPoint = mouse position
  → Creates _activeStroke (Polyline)
  → Adds start point to _activeStroke.Points

User moves mouse while holding left button + Shift
  → OverlayWindow.OnMouseMove
  → Checks Keyboard.IsKeyDown(Key.LeftShift || Key.RightShift)
  → Calculates angle from _strokeStartPoint to current mouse position
  → Determines snap direction (horizontal or vertical)
  → Calculates constrained point (snapped to axis)
  → Clears _activeStroke.Points (except start point)
  → Adds constrained point to _activeStroke.Points
  → Renders straight line from start to constrained point

User releases Shift while still dragging
  → OverlayWindow.OnMouseMove (next frame)
  → Shift not detected as held
  → Resumes freehand mode
  → Adds current mouse position to _activeStroke.Points
  → Continues adding points normally

User releases left mouse button
  → OverlayWindow.OnMouseLeftButtonUp
  → Finalizes stroke
  → Clears _activeStroke reference
```

### Edge Cases and Handling

#### Edge Case 1: Shift pressed mid-stroke
**Scenario:** User starts drawing freehand, then presses Shift.

**Handling:**
- Next `OnMouseMove` detects Shift is held
- Calculate angle from **original stroke start point** to current position
- Clear all intermediate freehand points
- Render only start point + current constrained point
- Visual effect: Stroke "snaps" to straight line

#### Edge Case 2: Shift released mid-stroke
**Scenario:** User draws a straight line, then releases Shift.

**Handling:**
- Next `OnMouseMove` detects Shift is not held
- Resume adding points normally
- Keep existing points (start + last constrained end)
- Add new points as user continues dragging
- Visual effect: Stroke continues from constrained endpoint as freehand

#### Edge Case 3: Very short drag distance
**Scenario:** User clicks and drags just a few pixels.

**Handling:**
- Set minimum distance threshold (e.g., 5 pixels) before applying constraint
- If distance < threshold, render as freehand (no constraint)
- Avoids unstable angle calculations and jittery behavior

#### Edge Case 4: Alt hotkey conflict with Windows
**Scenario:** Alt key may trigger Windows menu bar in some contexts.

**Handling:**
- Low-level keyboard hook captures Alt before Windows processes it
- Hook callback uses `CallNextHookEx` to pass event to next handler
- No interference expected (Alt + mouse drag does not trigger menu bar)
- If issues arise, document as known limitation

#### Edge Case 5: Alt + Shift both held
**Scenario:** User holds Alt (temporary mode) and Shift (constraint).

**Handling:**
- Temporary mode activates (Alt priority)
- Shift constraint applies during drawing
- Both features work independently
- No special handling needed (orthogonal features)

### Testing Strategy

#### Unit Tests
- `KeyboardHookManagerTests`:
  - Test Alt double-click detection within threshold
  - Test Alt double-click timeout (too slow, no activation)
  - Test Alt hold activates temporary mode
  - Test Alt release deactivates temporary mode
  - Test left Alt and right Alt both work

- `OverlayWindowTests` (if applicable):
  - Test straight line angle calculation (horizontal, vertical)
  - Test constraint snapping logic (45° threshold)
  - Test transition from freehand to constrained
  - Test transition from constrained to freehand

#### Integration Tests
- Manual testing checklist:
  - [ ] Alt double-click activates temporary draw mode
  - [ ] Alt release clears strokes and deactivates mode
  - [ ] Shift + drag creates horizontal line (drag mostly horizontal)
  - [ ] Shift + drag creates vertical line (drag mostly vertical)
  - [ ] Shift press mid-stroke snaps to straight line
  - [ ] Shift release mid-stroke resumes freehand
  - [ ] Alt temporary mode + Shift constraint work together
  - [ ] Win+Shift+D, Win+Shift+C, Win+Shift+Q hotkeys still work
  - [ ] Multi-monitor support maintained for both features

### Performance Considerations
- Angle calculation: `Math.Atan2` called on every mouse move when Shift is held
  - Performance: O(1) constant time, negligible overhead (~0.1 microseconds)
  - No caching needed (mouse move events are already throttled by OS/WPF)

- Point array manipulation: Clearing and re-adding points on every mouse move
  - `PointCollection.Clear()` and `Add()` are fast operations
  - Only 2 points in collection when constrained (minimal memory)
  - No performance concerns

### Rollback Plan
If issues arise after deployment:
1. Revert `KeyboardHookManager.cs` to monitor Shift keys (restore previous behavior)
2. Remove straight line constraint logic from `OverlayWindow.xaml.cs`
3. Restore original spec files from git history
4. Document decision to postpone change

Risk mitigation:
- Change is isolated to two components (low coupling)
- No database or persistence changes (stateless)
- Easy to detect in testing (visual and interactive behavior)

## Alternative Approaches Considered

### Alternative 1: Use Ctrl instead of Alt for temporary mode
**Pros:**
- Ctrl is commonly used for modifiers in drawing apps
- Less likely to conflict with Windows behavior

**Cons:**
- Ctrl+Shift is a common hotkey combination (e.g., Ctrl+Shift+D in many apps)
- Ctrl may interfere with copy/paste workflows
- Alt is less "overloaded" in drawing contexts

**Decision:** Use Alt (original proposal)

### Alternative 2: Add preference to toggle Shift constraint on/off
**Pros:**
- Allows users to disable straight line feature if unwanted
- More flexible

**Cons:**
- Adds complexity (settings UI, persistence)
- Out of scope for this change (can be added later)
- Most drawing tools enable Shift constraint by default

**Decision:** Always enable Shift constraint (can add setting in future)

### Alternative 3: Snap to multiple angles (0°, 45°, 90°, 135°, 180°, etc.)
**Pros:**
- More powerful drawing capability
- Useful for technical diagrams

**Cons:**
- More complex logic (multiple angle thresholds)
- Increases scope significantly
- Not requested by user

**Decision:** Only horizontal and vertical snapping (original proposal)

### Alternative 4: Visual feedback for constraint (e.g., guide lines)
**Pros:**
- Helps user understand snap behavior
- Common in professional drawing tools

**Cons:**
- Adds visual complexity (rendering guide lines)
- Requires additional UI implementation
- Out of scope for MVP

**Decision:** No visual feedback for now (can add later if needed)

## Open Design Questions

### Question 1: Minimum drag distance threshold
**Options:**
- A: No threshold (constraint always applies)
- B: 5 pixel threshold
- C: 10 pixel threshold

**Recommendation:** Option B (5 pixels) - prevents jitter on small drags, still responsive

### Question 2: Angle snap threshold
**Options:**
- A: 30° threshold (snap to horizontal if within ±30° of 0°/180°)
- B: 45° threshold (snap based on which axis is dominant)
- C: 60° threshold (biased toward vertical)

**Recommendation:** Option B (45°) - equal preference for horizontal and vertical, aligns with most drawing tools

### Question 3: Stroke continuity on Shift toggle
**Options:**
- A: Start new stroke when Shift state changes
- B: Continue same stroke, change rendering mode

**Recommendation:** Option B - more intuitive, allows seamless transition between constrained and freehand in one gesture

### Question 4: Line cap style for straight lines
**Options:**
- A: Keep rounded caps (consistent with freehand)
- B: Use square caps (more precise for straight lines)

**Recommendation:** Option A - maintain visual consistency with freehand strokes

## Validation
- [ ] Design reviewed against requirements in proposal.md
- [ ] Edge cases documented and handled
- [ ] Performance impact assessed as acceptable
- [ ] Rollback plan defined
- [ ] Testing strategy covers all acceptance criteria
