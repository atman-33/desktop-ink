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

### Requirement: Constrained straight line drawing with Shift modifier
When drawing in draw mode (permanent or temporary), the system SHALL constrain strokes to straight horizontal or vertical lines when the user holds the Shift key during a drag gesture.

#### Scenario: Draw horizontal straight line with Shift held
- **GIVEN** the overlay is in draw mode
- **WHEN** the user holds Shift and performs a drag gesture where the horizontal distance is greater than or equal to the vertical distance
- **THEN** a horizontal straight line is rendered from the drag start point to the current mouse position (Y coordinate constrained to start point Y).

#### Scenario: Draw vertical straight line with Shift held
- **GIVEN** the overlay is in draw mode
- **WHEN** the user holds Shift and performs a drag gesture where the vertical distance is greater than the horizontal distance
- **THEN** a vertical straight line is rendered from the drag start point to the current mouse position (X coordinate constrained to start point X).

#### Scenario: Transition from freehand to constrained mid-stroke
- **GIVEN** the user is drawing a freehand stroke (left button held, Shift not pressed)
- **WHEN** the user presses Shift while continuing to drag
- **THEN** the stroke transitions to a straight line from the original start point to the current constrained position, and intermediate freehand points are removed.

#### Scenario: Transition from constrained to freehand mid-stroke
- **GIVEN** the user is drawing a constrained straight line (left button held, Shift pressed)
- **WHEN** the user releases Shift while continuing to drag
- **THEN** the stroke continues as freehand from the last constrained position, and subsequent mouse movements add points normally.

#### Scenario: Straight line constraint works in temporary draw mode
- **GIVEN** temporary draw mode is active (Alt held)
- **WHEN** the user holds Shift and performs a drag gesture
- **THEN** straight line constraint applies as in permanent draw mode.

#### Scenario: Shift modifier does not interfere with hotkeys
- **GIVEN** the application is running
- **WHEN** the user presses Win+Shift+D, Win+Shift+C, or Win+Shift+Q
- **THEN** the corresponding hotkey action is executed (toggle draw mode, clear all, quit) without triggering straight line constraint.

