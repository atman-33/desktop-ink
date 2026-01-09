# multi-monitor-overlay Specification

## Purpose
TBD - created by archiving change add-control-palette-and-multi-monitor-overlay. Update Purpose after archive.
## Requirements
### Requirement: Overlay covers each connected monitor
When multiple monitors are connected in an extended desktop configuration, the system SHALL present an overlay surface that covers the bounds of each connected monitor.

#### Scenario: Overlays are created per monitor
- **GIVEN** the system has two or more connected monitors
- **WHEN** the application starts
- **THEN** an overlay surface exists for each monitor and matches its bounds.

### Requirement: Draw mode works on any monitor
In draw mode, the system SHALL allow the user to start a stroke on any connected monitor.

#### Scenario: Draw on a secondary monitor
- **GIVEN** a secondary monitor is connected and the overlay is in draw mode
- **WHEN** the user performs a left-button drag on the secondary monitor
- **THEN** a stroke is rendered on that monitor.

### Requirement: Pass-through works on any monitor
In pass-through mode, the system SHALL not interfere with underlying applications on any connected monitor.

#### Scenario: Click underlying app on a secondary monitor
- **GIVEN** a secondary monitor is connected and the overlay is in pass-through mode
- **WHEN** the user clicks an underlying application on the secondary monitor
- **THEN** the underlying application receives the input normally.

### Requirement: Clear-all affects all monitors
The system SHALL clear strokes across all monitors when clear-all is invoked.

#### Scenario: Clear across monitors
- **GIVEN** there are strokes on multiple monitors
- **WHEN** the clear-all action is invoked (via hotkey or palette)
- **THEN** strokes are removed from all monitors.

