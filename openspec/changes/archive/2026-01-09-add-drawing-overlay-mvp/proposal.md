# Change: Add drawing overlay MVP

## Why
Desktop-ink needs a minimal, low-friction way to draw temporary annotations directly on the Windows desktop without interrupting normal interaction with other applications.

## What Changes
- Add a transparent, always-on-top desktop overlay window (primary monitor first).
- Provide two modes:
  - **Pass-through mode**: overlay does not intercept mouse input; underlying apps behave normally.
  - **Draw mode**: overlay receives mouse input and allows freehand drawing.
- Implement MVP drawing:
  - Left mouse drag produces a continuous stroke.
  - Fixed stroke attributes (color, width, rounded caps/joins).
  - Clear-all only (no eraser, no undo/redo).
- Add global hotkeys (work without focus):
  - `Ctrl+Alt+D`: toggle draw/pass-through mode
  - `Ctrl+Alt+C`: clear all strokes
  - `Ctrl+Alt+Q`: quit
- Provide minimal on-screen feedback when draw mode is active.

## Scope
- Target OS: Windows 10/11
- UI framework: WPF
- Display target: primary monitor only

## Non-Goals (MVP)
- Saving/restoring drawings (image export, session restore)
- Full multi-monitor support
- Pen pressure / Windows Ink integration
- Color/width UI, eraser, shapes, text, layers
- Undo/redo
- Installer / auto-update

## Acceptance Criteria
- Pressing `Ctrl+Alt+D` switches modes immediately.
- In draw mode, dragging draws visible strokes on the desktop.
- In pass-through mode, clicks/drags are handled by underlying apps (overlay does not interfere).
- Pressing `Ctrl+Alt+C` clears all drawings.
- Pressing `Ctrl+Alt+Q` exits safely.

## Open Questions
- Hotkey registration failure behavior: should the app refuse to start, retry with alternatives, or run without hotkeys?
- Draw-mode feedback: prefer an always-visible small "DRAW" label, a transient toast, or a tray icon state change?
- Should the overlay appear in Alt-Tab / taskbar, or be fully hidden except when drawing?

## Impact
- New capabilities proposed (delta specs in this change):
  - `overlay-window`
  - `input-pass-through`
  - `inking-canvas`
  - `global-hotkeys`
  - `mode-feedback`
  - `runtime-safety-performance`
