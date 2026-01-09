# Design: Drawing overlay MVP

## Context
This change defines the MVP behavior for a Windows desktop overlay that can switch between a non-intrusive pass-through mode and an interactive draw mode.

Key constraints:
- Windows 10/11
- WPF rendering
- Primary monitor only (MVP)
- Global hotkeys required (work without focus)

## Goals
- Fast mode toggle: user can immediately start/stop drawing.
- In pass-through mode, the overlay must not interfere with underlying applications.
- In draw mode, freehand strokes must render smoothly with minimal overhead.
- Clear and quit actions are always available via hotkeys.

## Non-Goals
- Persistence (save/export)
- Multi-monitor completeness
- Rich editing tools (eraser, undo/redo, shapes, text)

## Architecture Overview (Proposed)
- **OverlayWindow (WPF)**
  - Borderless, transparent, topmost window sized to the primary monitor.
  - Hosts an **InkingCanvas** surface responsible for stroke collection and rendering.
  - Hosts a small **ModeIndicator** UI element.
- **ModeController**
  - Owns current mode state and transitions.
  - Updates input behavior (pass-through vs interactive) and UI indicator.
- **HotkeyService (Win32 interop)**
  - Registers system-wide hotkeys and dispatches commands to the app.

## Key Technical Decisions (Proposed)

### Pass-through implementation
WPF alone does not guarantee true click-through for the entire window. The pass-through mode should rely on Win32 window styles so the OS routes pointer input to underlying windows.

- Candidate approach:
  - Toggle extended window style (e.g., `WS_EX_TRANSPARENT` / `WS_EX_LAYERED`) on the overlay hwnd.
  - Keep the overlay visible but non-interactive.

### Drawing implementation
Use a simple retained-mode model:
- Collect points during a drag gesture.
- Render the stroke with fixed attributes (color/width, round caps/joins).
- Store strokes as independent visuals so multiple strokes persist.

### Performance strategy
- Avoid continuous timers when idle.
- Only update visuals in response to input events.
- Keep visual tree small (one element per stroke).

## Trade-offs / Risks
- Click-through behavior can be sensitive to window style details and composition; must be verified on Windows 10 and 11.
- Global hotkeys can fail if already registered by another app; required behavior is currently an open question.

## Validation Notes
Manual validation must emphasize:
- Pass-through never blocks underlying input.
- Mode toggle is immediate.
- Hotkeys work when the app is not focused.
