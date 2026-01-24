# mode-feedback Specification

## Purpose
TBD - created by archiving change add-drawing-overlay-mvp. Update Purpose after archive.
## Requirements
### Requirement: Provide minimal feedback when draw mode is active
When draw mode is active, the system SHALL display a minimal visual indicator so the user can tell drawing is enabled.

#### Scenario: Draw mode indicator is visible
- **GIVEN** the overlay is in draw mode
- **WHEN** the mode changes into draw mode
- **THEN** a visual indicator (e.g., a small "DRAW" label) is visible.

### Requirement: Mode indicator color matches pen color
The system SHALL display the mode indicator text in the same color as the currently selected pen color.

#### Scenario: Indicator color matches red pen
- **GIVEN** the pen color is set to Red
- **AND** the overlay is in draw mode
- **WHEN** the mode indicator is displayed
- **THEN** the indicator text color is red (#FF0000)

#### Scenario: Indicator color matches blue pen
- **GIVEN** the pen color is set to Blue
- **AND** the overlay is in draw mode
- **WHEN** the mode indicator is displayed
- **THEN** the indicator text color is blue (#0000FF)

#### Scenario: Indicator color matches green pen
- **GIVEN** the pen color is set to Green
- **AND** the overlay is in draw mode
- **WHEN** the mode indicator is displayed
- **THEN** the indicator text color is green (#00FF00)

#### Scenario: Indicator color updates when pen color changes
- **GIVEN** the overlay is in draw mode with pen color Red
- **WHEN** the user cycles to the next pen color (Blue)
- **THEN** the indicator text color updates to blue immediately

### Requirement: Pass-through mode remains unobtrusive
When pass-through mode is active, the system SHALL hide or minimize the draw mode indicator so it does not distract the user.

#### Scenario: Pass-through mode indicator is hidden
- **GIVEN** the overlay is in pass-through mode
- **WHEN** the mode changes into pass-through mode
- **THEN** the draw mode indicator is not shown or is visually minimized.

### Requirement: Distinguish temporary draw mode from permanent draw mode
The system SHALL provide visual feedback to distinguish temporary draw mode (Shift hold) from permanent draw mode (toggled via `Win+Shift+D`).

#### Scenario: Temporary draw mode indicator is visually distinct
- **GIVEN** the overlay enters temporary draw mode
- **WHEN** the mode indicator is displayed
- **THEN** the indicator shows that temporary mode is active (e.g., "DRAW (Temp)" or different color).

#### Scenario: Permanent draw mode indicator remains consistent
- **GIVEN** the overlay is in permanent draw mode
- **WHEN** temporary mode is not active
- **THEN** the indicator shows standard draw mode (e.g., "DRAW").

