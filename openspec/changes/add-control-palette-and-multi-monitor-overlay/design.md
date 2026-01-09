# Design: Control palette and multi-monitor overlay

## Context
The MVP overlay supports only the primary monitor and uses global hotkeys as the sole control surface. Users want:
1) A visible on-screen palette for click operations, and
2) Drawing to work on secondary monitors in extended display setups.

## Goals
- Provide an always-available, semi-transparent, topmost palette window.
- Make draw mode work on any connected monitor in extended desktop mode.
- Preserve the key MVP property: pass-through mode must not block underlying apps.

## Non-Goals
- Persist drawings
- Add rich editing tools
- Full handling of runtime monitor hot-plug / dynamic topology changes (MVP+)

## Proposed Architecture

### Window separation
- **Overlay windows** are responsible for drawing and pass-through behavior.
- **Control palette window** is always interactive and remains clickable even when overlays are in pass-through.

This separation avoids having to support "partial click-through" inside a single WPF window.

### Multi-monitor strategy
Use **one overlay window per monitor** rather than a single virtual-screen window.

Rationale:
- Simpler coordinate model (each window is local to its monitor bounds).
- More predictable input behavior across monitors.
- Avoids some DPI/negative-coordinate edge cases with a single virtual-screen-sized window.

## Key UX Decisions
- Palette controls are arranged vertically.
- Default palette placement is near the right edge of the primary monitor.
- Palette is draggable (via full-body drag or a header, TBD).

## Risks
- DPI differences per monitor can cause sizing/position issues; per-monitor windows reduce, but do not eliminate, this risk.
- Pass-through correctness must be verified on all monitors.
