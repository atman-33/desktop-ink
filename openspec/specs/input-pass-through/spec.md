# input-pass-through Specification

## Purpose
TBD - created by archiving change add-drawing-overlay-mvp. Update Purpose after archive.
## Requirements
### Requirement: Pass-through mode does not interfere with underlying apps
In pass-through mode, the system SHALL allow pointer input (click, drag, scroll) to be handled by underlying applications as if the overlay were not present.

#### Scenario: Clicking an underlying window
- **GIVEN** the overlay is in pass-through mode
- **WHEN** the user clicks on a control in an underlying application
- **THEN** the underlying application receives the click and responds normally.

### Requirement: Draw mode receives pointer input for drawing
In draw mode, the system SHALL receive pointer input so the user can draw strokes on the overlay.

#### Scenario: Starting a stroke
- **GIVEN** the overlay is in draw mode
- **WHEN** the user presses and drags the left mouse button on the overlay
- **THEN** the overlay receives the events needed to create a stroke.

