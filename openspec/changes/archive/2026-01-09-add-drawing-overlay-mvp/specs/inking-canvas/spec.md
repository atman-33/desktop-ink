# Capability: Inking Canvas

## ADDED Requirements

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
The system SHALL provide a clear-all action that removes all strokes from the overlay.

#### Scenario: Clear all drawings
- **GIVEN** there are one or more strokes on the overlay
- **WHEN** the clear-all action is invoked
- **THEN** the overlay contains no visible strokes.
