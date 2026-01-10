# overlay-window Specification

## Purpose
TBD - created by archiving change add-drawing-overlay-mvp. Update Purpose after archive.
## Requirements
### Requirement: Transparent, topmost overlay on the primary monitor
The system SHALL present a borderless, transparent overlay window that stays on top of other windows and covers the bounds of the primary monitor.

#### Scenario: Overlay is shown at startup
- **GIVEN** the application has started
- **WHEN** the overlay window is created
- **THEN** it covers the primary monitor bounds and is visually transparent.

### Requirement: Default to pass-through mode on startup
The system SHALL start in pass-through mode.

#### Scenario: Application starts non-intrusively
- **GIVEN** the application has just started
- **WHEN** no user action has been taken
- **THEN** the overlay does not intercept mouse input.

