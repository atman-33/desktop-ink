# Spec Delta: Sync Mode Indicator Color with Pen Color

## Requirements to Add

### Requirement: Mode indicator color matches pen color
The system SHALL display the mode indicator text in the same color as the currently selected pen color.

#### Scenario: Indicator color matches red pen
- **GIVEN** the pen color is set to Red
- **AND** the overlay is in draw mode
- **WHEN** the mode indicator is displayed
- **THEN** the indicator text color is red (#FFFF3B3B or #FF0000)

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
