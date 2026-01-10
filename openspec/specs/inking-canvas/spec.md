# inking-canvas Specification

## Purpose
TBD - created by archiving change add-drawing-overlay-mvp. Update Purpose after archive.
## Requirements
### Requirement: Freehand drawing with a single drag gesture
In draw mode, the system SHALL create a visible stroke when the user presses, drags, and releases the left mouse button.

#### Scenario: Draw a single continuous stroke
- **GIVEN** the overlay is in draw mode
- **WHEN** the user performs a left-button drag gesture
- **THEN** a continuous stroke is rendered along the drag path.

### Requirement: Fixed stroke appearance (MVP)
The system SHALL render strokes using fixed attributes: a fixed color, a fixed width, and rounded caps and joins.

#### Scenario: Stroke appearance is consistent
- **GIVEN** the user draws multiple strokes
- **WHEN** strokes are rendered
- **THEN** each stroke uses the same configured color and width with rounded caps/joins.

### Requirement: Clear-all removes all strokes
The system SHALL provide a clear-all action that removes all strokes from the overlay. Clear-all can be invoked manually via hotkey (`Win+Shift+C`) or automatically when temporary draw mode is deactivated.

#### Scenario: Clear all drawings manually
- **GIVEN** there are one or more strokes on the overlay
- **WHEN** the clear-all action is invoked via `Win+Shift+C`
- **THEN** the overlay contains no visible strokes.

#### Scenario: Clear all drawings automatically on temporary mode exit
- **GIVEN** temporary draw mode is active with strokes on the overlay
- **WHEN** the user releases the Shift key
- **THEN** the overlay contains no visible strokes.

### Requirement: Auto-clear strokes on temporary mode exit
When temporary draw mode is deactivated (Shift key released), the system SHALL automatically clear all strokes from the canvas across all monitors.

#### Scenario: Strokes are cleared when Shift is released
- **GIVEN** temporary draw mode is active and the user has drawn one or more strokes
- **WHEN** the user releases the Shift key
- **THEN** all strokes are immediately cleared from all overlay windows.

#### Scenario: Auto-clear works on multi-monitor setup
- **GIVEN** temporary draw mode is active with strokes on multiple monitors
- **WHEN** the user releases the Shift key
- **THEN** all strokes across all monitors are cleared simultaneously.

