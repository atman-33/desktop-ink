# Change: Add temporary draw mode with Shift double-click and hold

## Why
The current implementation requires users to toggle draw mode explicitly via hotkey (`Win+Shift+D`) or control palette button, then manually clear strokes afterward. This workflow interrupts the user's flow when they want to quickly annotate something on screen and then continue their work.

A temporary draw mode activated by double-clicking and holding the Shift key would allow users to:
- Quickly annotate or mark up their screen without changing modes permanently
- Automatically return to pass-through mode when done
- Keep the screen clean by auto-clearing strokes when releasing the key

This creates a more fluid, pen-like experience where drawing is available on-demand without disrupting the user's workflow.

## What Changes
- Add detection for Shift key double-click pattern (two rapid presses within a short time window)
- When Shift is double-clicked and held down:
  - Activate draw mode temporarily
  - Allow ink strokes to be drawn while held
- When Shift is released:
  - Clear all current strokes automatically
  - Return to pass-through mode
- The temporary draw mode works independently from the permanent draw mode toggle
  - User can still use `Win+Shift+D` to toggle persistent draw mode
  - Temporary mode takes precedence when active

## Scope
- Target OS: Windows 10/11
- UI framework: WPF
- Input: Low-level keyboard hook to detect Shift double-click pattern
- No UI changes needed (behavior only)

## Relationship to Existing Changes
- Extends: `global-hotkeys`
  - Adds keyboard event detection beyond existing hotkey registration
  - Introduces temporary mode concept alongside existing toggle mode
- Relates to: `inking-canvas`, `mode-feedback`
  - Uses existing ink rendering infrastructure
  - May need mode indicator to show temporary vs permanent draw mode

## Non-Goals
- Configuring the double-click speed threshold (use system default)
- Customizing which key triggers temporary mode (Shift only for now)
- Preventing strokes from being cleared (always clear on release)
- Persisting temporary mode across app restarts

## Acceptance Criteria
- Double-clicking Shift within system double-click threshold activates temporary draw mode
- Holding Shift after double-click keeps temporary draw mode active
- User can draw ink strokes while Shift is held
- Releasing Shift automatically clears all strokes and returns to pass-through mode
- Temporary mode works on all monitors (multi-monitor support)
- Temporary mode does not interfere with existing `Win+Shift+D` toggle functionality
- Temporary mode takes visual priority if permanent draw mode is already active

## Open Questions
- Should there be a visual indicator distinguishing temporary draw mode from permanent draw mode?
- Should temporary mode be disabled while permanent draw mode is active, or should they work independently?
- Should the double-click time window be configurable in future iterations?
- Should there be a minimum hold duration before temporary mode activates (to avoid accidental activation)?
