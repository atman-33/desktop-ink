# Tasks: Enhance Control Palette Tooltip Discoverability

## Implementation Tasks

1. **Update Toggle Draw button tooltip**
   - File: `src/DesktopInk/Windows/ControlWindow.xaml`
   - Change the `ToolTip` attribute on the `ToggleButton` from single-line to multi-line format
   - Add information about Alt+Double-click for temporary mode

2. **Manual verification**
   - Build and run the application
   - Hover over the Toggle Draw button
   - Verify tooltip displays correctly on multiple DPI settings
   - Verify tooltip remains readable and properly formatted

3. **Update spec delta**
   - Update `control-palette` spec to document the tooltip requirement
   - Add scenario for tooltip discoverability of advanced interactions

## Documentation Tasks

4. **Update README (optional but recommended)**
   - Add or update section describing control palette interactions
   - Document Alt+Double-click for temporary mode
   - Ensure consistency with tooltip text

## Testing Tasks

5. **Manual testing checklist**
   - [x] Tooltip appears on hover
   - [x] Both lines of text are visible
   - [x] Text is readable on 100%, 125%, 150% DPI scaling
   - [x] Tooltip does not obstruct important UI elements
   - [x] Alt+Double-click still functions correctly (regression test)

## Notes

- No automated tests required (UI tooltip is visual-only)
- Manual verification is sufficient for this change
- Consider adding similar enhanced tooltips to other buttons in future changes
