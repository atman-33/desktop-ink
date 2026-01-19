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
The system SHALL render strokes using configurable color (selected by user), fixed width (3px), and rounded caps and joins. The default color SHALL be red (#FF0000).

#### Scenario: Stroke appearance matches selected color
- **GIVEN** the user has selected a pen color (red, blue, or green)
- **WHEN** the user draws multiple strokes
- **THEN** each stroke uses the currently selected color with fixed width (3px) and rounded caps/joins.

#### Scenario: Default color is red
- **GIVEN** the application has just started
- **WHEN** the user draws a stroke
- **THEN** the stroke is rendered in red (#FF0000).

#### Scenario: Color change applies only to new strokes
- **GIVEN** the user has drawn strokes in red
- **WHEN** the user cycles to blue and draws new strokes
- **THEN** the old strokes remain red and the new strokes are blue.

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

### Requirement: User-selectable pen color with cycling
The system SHALL provide three pen colors: red (#FF0000), blue (#0000FF), and green (#00FF00). The user MAY cycle through colors in sequence: red → blue → green → red. The selected color applies to all new strokes until changed.

#### Scenario: Cycle pen color via control palette button
- **GIVEN** the current pen color is red
- **WHEN** the user clicks the color cycle button in the control palette
- **THEN** the pen color changes to blue
- **AND** subsequent strokes are rendered in blue.

#### Scenario: Cycle through all colors
- **GIVEN** the pen color starts at red
- **WHEN** the user cycles three times
- **THEN** the pen color returns to red after passing through blue and green.

#### Scenario: Cycle pen color via Alt+S during temporary draw mode
- **GIVEN** temporary draw mode is active (Alt key held after double-tap)
- **WHEN** the user presses S while holding Alt
- **THEN** the pen color cycles to the next color in sequence
- **AND** subsequent strokes are rendered in the new color.

#### Scenario: Alt+S has no effect outside temporary draw mode
- **GIVEN** the application is in pass-through mode or permanent draw mode (not temporary)
- **WHEN** the user presses Alt+S
- **THEN** no color change occurs.

#### Scenario: Color persists across mode switches
- **GIVEN** the user has selected blue as the pen color
- **WHEN** the user toggles between draw and pass-through modes
- **THEN** the pen color remains blue when returning to draw mode.

#### Scenario: Color applies to all monitors
- **GIVEN** the user has selected green as the pen color
- **WHEN** the user draws on multiple monitors
- **THEN** all strokes on all monitors use green.

