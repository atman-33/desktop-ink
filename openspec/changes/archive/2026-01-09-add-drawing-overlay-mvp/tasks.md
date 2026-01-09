# Tasks: Add drawing overlay MVP

## 1. Project & App Shell
- [x] 1.1 Create a WPF application skeleton (Windows 10/11 target)
- [x] 1.2 Add a single overlay window configured for borderless + transparent rendering
- [x] 1.3 Ensure overlay targets the primary monitor bounds

## 2. Modes & Input Behavior
- [x] 2.1 Implement pass-through mode as the default at startup
- [x] 2.2 Implement draw mode that receives mouse input
- [x] 2.3 Implement mode switching logic (single source of truth for current mode)

## 3. Drawing (Freehand)
- [x] 3.1 Implement stroke capture for left mouse drag (down/move/up)
- [x] 3.2 Render strokes on a canvas (fixed color/width, rounded caps/joins)
- [x] 3.3 Persist multiple strokes until cleared

## 4. Clear & Quit
- [x] 4.1 Implement clear-all action that removes all strokes
- [x] 4.2 Implement graceful quit action (unregister resources and exit)

## 5. Global Hotkeys
- [x] 5.1 Register global hotkeys: `Ctrl+Alt+D`, `Ctrl+Alt+C`, `Ctrl+Alt+Q`
- [x] 5.2 Route hotkey events to: toggle mode, clear, quit
- [x] 5.3 Define behavior on hotkey registration failure (see proposal open question)

## 6. Draw Mode Feedback
- [x] 6.1 Add minimal visual indicator when draw mode is active
- [x] 6.2 Ensure indicator is hidden or unobtrusive in pass-through mode

## 7. Non-Functional Requirements
- [x] 7.1 Verify low idle activity (no unnecessary timers/render invalidations)
- [x] 7.2 Verify pass-through mode never blocks underlying app input
- [x] 7.3 Verify crash/exit does not leave the desktop in a blocked state

## 8. Validation
- [x] 8.1 Manual smoke test checklist:
  - Start app: default is pass-through
  - Toggle draw mode: overlay starts drawing
  - Toggle back: underlying app receives input
  - Clear all: canvas cleared
  - Quit: app exits and hotkeys are released
- [x] 8.2 Add minimal automated tests for pure logic (mode state + hotkey command mapping), if a test harness exists in the repo (not applicable: no test harness present)
- [x] 8.3 Run `openspec validate add-drawing-overlay-mvp --strict --no-interactive` and fix all issues
