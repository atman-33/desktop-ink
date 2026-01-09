# Tasks: Add control palette and multi-monitor overlay

## 1. Multi-Monitor Overlay
- [ ] 1.1 Enumerate connected monitors and their bounds
- [ ] 1.2 Create one overlay window per monitor (position/size to monitor bounds)
- [ ] 1.3 Ensure mode switching applies to all overlay windows consistently
- [ ] 1.4 Ensure clear-all clears strokes across all overlays
- [ ] 1.5 Validate drawing input works on primary and secondary monitors
- [ ] 1.6 Validate pass-through works on primary and secondary monitors

## 2. Control Palette Window
- [ ] 2.1 Add a small always-on-top palette window (semi-transparent)
- [ ] 2.2 Layout controls vertically: Toggle Draw / Clear / Quit
- [ ] 2.3 Implement drag-move behavior for the palette
- [ ] 2.4 Place palette initially near the right edge of the primary monitor
- [ ] 2.5 Wire palette actions to the same command handlers as hotkeys

## 3. Command Wiring & Consistency
- [ ] 3.1 Centralize commands (toggle mode, clear, quit) so hotkeys and palette share logic
- [ ] 3.2 Ensure draw-mode feedback remains correct while using palette

## 4. Validation
- [ ] 4.1 Manual checklist (multi-monitor):
  - Start app with extended displays
  - Toggle draw mode via hotkey and via palette
  - Draw on each monitor
  - Toggle pass-through and confirm underlying apps receive input on each monitor
  - Clear all and confirm all monitors are cleared
  - Quit via palette and confirm hotkeys are unregistered
- [ ] 4.2 Run `openspec validate add-control-palette-and-multi-monitor-overlay --strict --no-interactive` and fix all issues
