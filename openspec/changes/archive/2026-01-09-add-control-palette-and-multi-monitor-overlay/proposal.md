# Change: Add control palette and multi-monitor overlay

## Why
The MVP relies solely on global hotkeys and only supports the primary monitor. In practice, users want a visible, always-available control surface and the ability to draw on any extended display.

## What Changes
- Add a small, semi-transparent, always-on-top control palette window.
  - Provides click controls for: toggle draw mode, clear all, quit.
  - Palette is draggable.
  - Controls are arranged vertically.
  - Default palette placement is near the right side of the primary monitor.
- Extend overlay rendering and input behavior to multi-monitor setups (extended displays).
  - In draw mode, the user can start drawing from anywhere on any connected monitor.
  - In pass-through mode, underlying apps remain fully operable on all monitors.
  - Clear-all clears strokes across all monitors.

## Scope
- Target OS: Windows 10/11
- UI framework: WPF
- Multi-monitor: support extended desktop; mirror/duplicate mode is not a focus

## Relationship to Existing Changes
- Builds on: `add-drawing-overlay-mvp`
  - Reuses the same core actions (toggle mode, clear, quit).
  - Expands from primary-monitor-only to all connected monitors.

## Non-Goals
- Persisting or exporting drawings
- Advanced tools (eraser, undo/redo, shapes, colors/width UI)
- Saving palette position between runs (can be added later)

## Acceptance Criteria
- The control palette is always visible, semi-transparent, topmost, and draggable.
- Clicking palette controls performs the same actions as the corresponding hotkeys.
- With multiple monitors connected, draw mode allows drawing on both primary and secondary monitors.
- Pass-through mode does not interfere with underlying apps on any monitor.
- Clear-all removes strokes across all monitors.

## Open Questions
- Palette hit area: should the entire palette be draggable, or only a small header?
- Palette safety: should there be a confirmation prompt on Quit?
