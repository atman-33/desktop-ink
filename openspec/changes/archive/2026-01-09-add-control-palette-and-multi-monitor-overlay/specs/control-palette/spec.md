# Capability: Control Palette

## ADDED Requirements

### Requirement: Always-on-top semi-transparent palette
The system SHALL provide a small, semi-transparent palette window that stays on top of other windows.

#### Scenario: Palette is visible
- **GIVEN** the application is running
- **WHEN** the user looks at the desktop
- **THEN** the palette is visible and remains topmost.

### Requirement: Palette provides click controls for core actions
The palette SHALL provide click controls for:
- Toggle draw/pass-through mode
- Clear all strokes
- Quit the application

#### Scenario: Toggle draw mode via palette
- **GIVEN** the application is running
- **WHEN** the user clicks the palette control to toggle draw mode
- **THEN** the overlay mode changes accordingly.

#### Scenario: Clear all via palette
- **GIVEN** there are strokes on one or more monitors
- **WHEN** the user clicks the palette control to clear
- **THEN** all strokes are removed.

#### Scenario: Quit via palette
- **GIVEN** the application is running
- **WHEN** the user clicks the palette control to quit
- **THEN** the application exits safely.

### Requirement: Vertical layout
The palette controls SHALL be arranged vertically.

#### Scenario: Controls are stacked
- **GIVEN** the palette is visible
- **WHEN** the user views the palette
- **THEN** the controls are stacked vertically.

### Requirement: Palette is draggable
The system SHALL allow the user to drag the palette to reposition it.

#### Scenario: Drag the palette
- **GIVEN** the palette is visible
- **WHEN** the user drags the palette
- **THEN** the palette moves to the new location.

### Requirement: Default placement near primary right edge
On startup, the system SHALL place the palette near the right edge of the primary monitor.

#### Scenario: Initial placement
- **GIVEN** the application has just started
- **WHEN** the palette window is shown
- **THEN** it appears near the right edge of the primary monitor.
