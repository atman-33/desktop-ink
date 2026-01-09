# Capability: Runtime Safety & Performance

## ADDED Requirements

### Requirement: Low overhead when idle
When the overlay is not actively drawing, the system SHALL avoid unnecessary work so CPU/GPU usage remains low.

#### Scenario: Idle in pass-through mode
- **GIVEN** the overlay is in pass-through mode and the user is not drawing
- **WHEN** the system is observed over time
- **THEN** it performs no continuous rendering or polling beyond what is required to remain available.

### Requirement: Safe exit
When the quit action is invoked, the system SHALL exit without leaving the desktop in a blocked input state.

#### Scenario: Quit releases control
- **GIVEN** the application is running
- **WHEN** the user invokes quit (e.g., `Ctrl+Alt+Q`)
- **THEN** the application exits and the desktop continues to accept input normally.
