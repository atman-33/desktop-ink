# mode-feedback Spec Delta

## ADDED Requirements

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

## MODIFIED Requirements

None - existing mode feedback requirements remain compatible.

## REMOVED Requirements

None.
