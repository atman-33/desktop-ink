# Sync Mode Indicator Color with Pen Color

## Why
The Desktop Ink application displays a mode indicator in the top-left corner of the screen showing "DRAW" or "DRAW (TEMP)" when drawing mode is active. Currently, this indicator text is always displayed in red (`#FFFF3B3B`), regardless of the selected pen color.

Users can cycle through different pen colors (Red, Blue, Green) using the `Win+Shift+C` hotkey. However, the mode indicator color does not reflect the current pen color, which can be confusing for users who want visual feedback about which color they are currently drawing with.

## What Changes
Synchronize the mode indicator text color with the currently selected pen color to provide clearer visual feedback to users about which pen color is active.

### Implementation
1. Update the `SetMode` method in `OverlayWindow` to apply the current pen color to the indicator text when entering draw mode
2. Update the `SetPenColor` method to refresh the indicator text color when the pen color changes while in draw mode
3. Extract the color-to-brush logic into a reusable method that can be used for both stroke drawing and indicator text
4. Add a new requirement to the `mode-feedback` spec to document this behavior

## Impact
- **User Experience**: Improved visual clarity - users can immediately see which pen color is active
- **Code Changes**: Minor modifications to `OverlayWindow.xaml.cs`
- **Testing**: Update existing tests if needed; manual verification of color synchronization
- **Documentation**: Update `mode-feedback` spec with new requirement

## Risks
- Low risk: The change is localized to the indicator text color logic
- No breaking changes to the public API

## References
- [OverlayWindow.xaml.cs](../../src/DesktopInk/Windows/OverlayWindow.xaml.cs)
- [mode-feedback spec](../specs/mode-feedback/spec.md)
