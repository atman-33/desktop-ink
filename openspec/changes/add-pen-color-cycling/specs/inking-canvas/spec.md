# inking-canvas Delta

## MODIFIED Requirements

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

## ADDED Requirements

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
