# Tasks for Sync Mode Indicator Color with Pen Color

## Implementation Tasks
- [x] 1. **Update SetMode method** - Apply current pen color to indicator text foreground when entering draw mode
- [x] 2. **Update SetPenColor method** - Refresh indicator text color when pen color changes in draw mode
- [x] 3. **Refactor color-to-brush conversion** - Create helper method to convert PenColor to Brush for both strokes and text

## Testing Tasks
- [ ] 4. **Manual verification** - Test that indicator color matches pen color for all three colors (Red, Blue, Green)
- [ ] 5. **Manual verification** - Test that indicator color updates when cycling through colors with Win+Shift+C
- [ ] 6. **Manual verification** - Test that indicator color is correct for both permanent and temporary draw modes

## Documentation Tasks
- [x] 7. **Update mode-feedback spec** - Add new requirement for indicator color synchronization with pen color
