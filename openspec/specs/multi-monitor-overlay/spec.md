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

### Requirement: Draw mode works on palette-focused monitor
In draw mode, the system SHALL allow the user to draw only on the monitor where the control palette is located. Other monitors SHALL remain in pass-through mode, allowing normal interaction with underlying applications.

#### Scenario: Draw only on palette's monitor
- **GIVEN** the control palette is positioned on monitor A and a secondary monitor B is connected
- **WHEN** the user activates draw mode
- **THEN** the user can draw strokes on monitor A
- **AND** the user can interact with underlying applications on monitor B normally
- **AND** monitor B's overlay remains in pass-through mode

#### Scenario: Draw mode follows palette to different monitor
- **GIVEN** draw mode is active and the palette is on monitor A
- **WHEN** the user drags the palette to monitor B
- **THEN** monitor A returns to pass-through mode
- **AND** monitor B becomes draw-enabled
- **AND** the user can now draw on monitor B and interact with apps on monitor A

#### Scenario: Initial draw mode on primary monitor only
- **GIVEN** the application has just started with multiple monitors connected
- **WHEN** the user activates draw mode for the first time
- **THEN** only the primary monitor (where the palette is initially positioned) becomes draw-enabled
- **AND** all other monitors remain in pass-through mode

### Requirement: Pass-through works on non-focused monitors
In draw mode, the system SHALL ensure that all monitors except the palette's monitor remain in pass-through mode and do not interfere with underlying applications.

#### Scenario: Interact with apps on non-palette monitors during draw mode
- **GIVEN** draw mode is active and the palette is on monitor A
- **WHEN** the user clicks or types on monitor B
- **THEN** the underlying application on monitor B receives the input normally
- **AND** no stroke input is captured on monitor B
- **AND** the overlay on monitor B does not intercept any user input

### Requirement: Pass-through works on any monitor in pass-through mode
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

### Requirement: Temporary draw mode works only on palette's monitor
When temporary draw mode is activated (e.g., via Shift key hold), the system SHALL enable drawing only on the monitor where the control palette is located.

#### Scenario: Temporary mode on palette monitor only
- **GIVEN** the palette is on monitor A and the system is in pass-through mode
- **WHEN** the user activates temporary draw mode
- **THEN** only monitor A's overlay enters draw mode
- **AND** monitor B remains in pass-through mode
- **AND** the user can draw on A while working on B

#### Scenario: Temporary mode respects palette position changes
- **GIVEN** temporary draw mode is active with palette on monitor A
- **WHEN** the user drags the palette to monitor B while holding the temp mode trigger
- **THEN** monitor A returns to pass-through mode
- **AND** monitor B becomes draw-enabled
- **AND** drawing capability follows the palette immediately

