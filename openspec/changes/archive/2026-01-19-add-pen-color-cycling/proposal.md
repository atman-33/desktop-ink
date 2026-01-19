# Change: Add pen color cycling

## Why
Users need the ability to draw with different colors to distinguish different annotations or ideas. Currently, the application only supports red strokes, limiting visual organization and expressiveness.

## What Changes
- Add a color cycling button to the control palette positioned below the Toggle Draw button
- Implement color cycling through red (default) → blue → green → red... sequence
- Display the current pen color visually through the button's icon color
- Add Alt+S hotkey during temporary draw mode (Alt double-tap) to cycle colors
- Maintain existing strokes in their original colors when pen color changes
- Persist red as the default color on application startup

## Impact
- Affected specs:
  - `inking-canvas`: Stroke appearance now varies by selected color
  - `control-palette`: New color cycle button added to vertical stack
  - `global-hotkeys`: New keyboard shortcut (Alt+S) during temporary draw mode
- Affected code:
  - `OverlayWindow.xaml.cs`: `CreateStroke()` method modified to use current color
  - `ControlWindow.xaml`: New button added to vertical stack panel
  - `ControlWindow.xaml.cs`: Color cycling logic and button click handler
  - `KeyboardHookManager.cs`: Alt+S detection during temporary mode
  - `OverlayManager.cs`: Color state management and propagation to overlays
