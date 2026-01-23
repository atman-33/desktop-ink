# multi-monitor-overlay Specification Delta

## Modified Requirements

### Requirement: Draw mode works only on the palette's monitor
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

### Requirement: Pass-through works on all non-palette monitors
In draw mode, the system SHALL ensure that all monitors except the palette's monitor remain in pass-through mode and do not interfere with underlying applications.

#### Scenario: Interact with apps on non-palette monitors during draw mode
- **GIVEN** draw mode is active and the palette is on monitor A
- **WHEN** the user clicks or types on monitor B
- **THEN** the underlying application on monitor B receives the input normally
- **AND** no stroke input is captured on monitor B
- **AND** the overlay on monitor B does not intercept any user input

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

## Existing Requirements (Unchanged)

### Requirement: Clear-all affects all monitors
*(No change - clear-all continues to clear strokes across all monitors)*
