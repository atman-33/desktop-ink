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

### Requirement: Pass-through mode remains unobtrusive
When pass-through mode is active, the system SHALL hide or minimize the draw mode indicator so it does not distract the user.

#### Scenario: Pass-through mode indicator is hidden
- **GIVEN** the overlay is in pass-through mode
- **WHEN** the mode changes into pass-through mode
- **THEN** the draw mode indicator is not shown or is visually minimized.

