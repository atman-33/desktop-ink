# Change: Change temporary draw mode trigger to Alt and add straight line drawing with Shift

## Why
The current implementation uses Shift double-click to activate temporary draw mode. However, this creates a conflict when users want to draw straight lines using the Shift key modifier, which is a common pattern in many drawing applications (e.g., Paint, Photoshop, PowerPoint).

By changing the temporary draw mode trigger from Shift to Alt double-click:
- Frees up the Shift key for standard drawing modifiers
- Allows implementation of constrained drawing (horizontal/vertical straight lines with Shift)
- Maintains the quick temporary draw mode access pattern
- Aligns with conventional drawing tool behaviors where Shift constrains geometry

This change enhances the drawing experience by providing:
1. More intuitive temporary mode activation (Alt is less commonly used during drawing)
2. Industry-standard straight line drawing behavior (Shift + drag)

## What Changes
### Temporary Mode Trigger Change
- Replace Shift double-click detection with Alt double-click detection in `KeyboardHookManager`
- Update keyboard hook to monitor Alt key (VK_LMENU, VK_RMENU) instead of Shift keys
- Keep the same double-click timing and hold-release logic
- Auto-clear behavior remains the same (clear strokes on Alt release)

### Straight Line Drawing Feature
- Detect Shift key state during mouse drag in draw mode
- When Shift is held while drawing:
  - Calculate the angle between start point and current point
  - Snap to horizontal (0°/180°) if angle is closer to horizontal than vertical
  - Snap to vertical (90°/270°) if angle is closer to vertical than horizontal
  - Render only the start and end points (creating a straight line)
- When Shift is released during drawing:
  - Return to freehand drawing mode
  - Continue the same stroke

## Scope
- Target OS: Windows 10/11
- UI framework: WPF
- Input: 
  - Low-level keyboard hook for Alt key detection
  - Keyboard state check for Shift during mouse move events
- Modified components:
  - `KeyboardHookManager.cs` - change key monitoring from Shift to Alt
  - `OverlayWindow.xaml.cs` - add straight line constraint logic during drawing

## Relationship to Existing Changes
- Modifies: `global-hotkeys` spec
  - Changes the key used for temporary draw mode (Shift → Alt)
- Extends: `inking-canvas` spec
  - Adds constrained drawing capability (straight lines)
- Relates to: `mode-feedback`
  - Mode indicator should still show "DRAW (TEMP)" for Alt-triggered temporary mode

## Non-Goals
- Supporting diagonal angle snapping (only horizontal and vertical)
- Making the constraint angle threshold configurable
- Supporting other shape constraints (circles, squares, etc.)
- Adding visual guides or snap indicators during constrained drawing
- Changing the permanent draw mode toggle hotkey (Win+Shift+D remains unchanged)

## Acceptance Criteria
### Temporary Mode Trigger Change
- Double-clicking Alt within system double-click threshold activates temporary draw mode
- Holding Alt after double-click keeps temporary draw mode active
- Releasing Alt automatically clears all strokes and returns to pass-through mode
- Shift key no longer triggers temporary draw mode
- Temporary mode works on all monitors (multi-monitor support)
- Existing Win+Shift+D hotkey continues to work for permanent draw mode toggle

### Straight Line Drawing
- Holding Shift while dragging in draw mode constrains the stroke to a straight line
- The line snaps to horizontal or vertical based on the dominant direction
- Releasing Shift during a drag allows continuation with freehand drawing
- Straight line constraint works in both permanent and temporary draw modes
- Straight line constraint does not interfere with existing hotkeys (Win+Shift+D, Win+Shift+C, Win+Shift+Q)

## Open Questions
- Should there be a minimum drag distance before straight line snapping engages?
- What should be the angle threshold for horizontal vs vertical snapping? (e.g., 45 degrees)
- Should releasing Shift mid-stroke transition smoothly to freehand, or start a new stroke?
- Should straight lines use rounded caps like freehand strokes, or square caps?
- Should there be any visual feedback when Shift constraint is active?
