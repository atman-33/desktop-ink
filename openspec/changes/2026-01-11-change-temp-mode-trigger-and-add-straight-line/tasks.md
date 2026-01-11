# Tasks: Change temporary draw mode trigger to Alt and add straight line drawing with Shift

## 1. Update keyboard hook for Alt key detection
- [ ] Update `KeyboardHookManager.cs` to monitor Alt keys (VK_LMENU=0xA4, VK_RMENU=0xA5) instead of Shift keys
- [ ] Replace Shift key constants with Alt key constants in double-click detection logic
- [ ] Update event handlers (`HandleAltPress`, `HandleAltRelease`) to work with Alt key
- [ ] Update logging messages to reflect Alt instead of Shift
- [ ] Verify double-click timing logic still works with Alt key

## 2. Update Win32 key code constants
- [ ] Add `VkLMenu` and `VkRMenu` constants to `Win32.cs` if not already present
- [ ] Ensure proper virtual key codes are used (0xA4 for Left Alt, 0xA5 for Right Alt)

## 3. Update tests for Alt key detection
- [ ] Update `KeyboardHookManagerTests` to test Alt double-click instead of Shift
- [ ] Verify temporary mode activation tests work with Alt key
- [ ] Verify temporary mode deactivation tests work with Alt release
- [ ] Add test cases for Alt key edge cases (left vs right Alt)

## 4. Implement straight line constraint logic
- [ ] Add method in `OverlayWindow.xaml.cs` to detect Shift key state during mouse move
- [ ] Implement angle calculation between stroke start point and current mouse position
- [ ] Implement snapping logic (horizontal vs vertical based on angle threshold, e.g., 45 degrees)
- [ ] Modify `OnMouseMove` to apply straight line constraint when Shift is held
- [ ] Store stroke start position for angle calculation
- [ ] Handle Shift press/release mid-stroke (transition between constrained and freehand)

## 5. Update stroke rendering for straight lines
- [ ] When Shift is held, render only two points (start and current constrained position)
- [ ] Clear intermediate points when transitioning from freehand to constrained
- [ ] Preserve stroke continuity when transitioning from constrained to freehand
- [ ] Ensure straight lines use the same stroke appearance (color, thickness, caps)

## 6. Integration and testing
- [ ] Test Alt double-click + hold activates temporary draw mode
- [ ] Test Alt release clears strokes and deactivates temporary mode
- [ ] Test Shift + drag creates horizontal or vertical straight lines
- [ ] Test Shift release mid-stroke transitions to freehand drawing
- [ ] Test straight line drawing in both permanent and temporary draw modes
- [ ] Test on multi-monitor setups
- [ ] Verify Win+Shift+D, Win+Shift+C, Win+Shift+Q hotkeys still work correctly
- [ ] Verify no interference between Alt temporary mode and Shift straight line feature

## 7. Update specification deltas
- [ ] Create spec delta for `global-hotkeys` (change temporary mode trigger key)
- [ ] Create spec delta for `inking-canvas` (add straight line constraint capability)
- [ ] Document new Alt double-click behavior
- [ ] Document Shift constraint behavior

## 8. Validation and cleanup
- [ ] Run `openspec validate 2026-01-11-change-temp-mode-trigger-and-add-straight-line` (or `--all`)
- [ ] Resolve any validation errors
- [ ] Update code comments to reflect Alt key usage
- [ ] Remove obsolete Shift-related comments in `KeyboardHookManager`

## Dependencies
- Task 1 must complete before Task 3 (tests depend on implementation)
- Task 2 must complete before Task 1 (constants needed for Alt key codes)
- Task 4 must complete before Task 5 (constraint logic needed for rendering)
- Task 7 depends on Tasks 1-6 (spec deltas document implemented behavior)

## Parallel Work Opportunities
- Tasks 1-3 (Alt key changes) can be done in parallel with Tasks 4-5 (straight line feature)
- Task 7 (spec deltas) can be drafted in parallel with implementation
