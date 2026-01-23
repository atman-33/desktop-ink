# control-palette Specification Delta

## Added Requirements

### Requirement: Palette SHALL track its current monitor position
The control palette SHALL determine which monitor it is currently positioned on based on its center point and SHALL notify the overlay manager when it moves to a different monitor.

#### Scenario: Palette detects monitor during startup
- **GIVEN** the application is starting and the palette is being positioned
- **WHEN** the palette window is shown
- **THEN** the palette calculates which monitor contains its center point
- **AND** notifies the overlay manager of its current monitor

#### Scenario: Palette detects monitor change during drag
- **GIVEN** the palette is positioned on monitor A
- **WHEN** the user drags the palette so its center point moves to monitor B
- **THEN** the palette detects the monitor change
- **AND** notifies the overlay manager of the new monitor

#### Scenario: Center point determines monitor assignment
- **GIVEN** the palette window has a defined position and size
- **WHEN** the system determines which monitor the palette is on
- **THEN** the calculation uses the center point of the palette window
- **AND** uses system APIs (e.g., MonitorFromPoint) to identify the monitor

### Requirement: Palette SHALL provide current monitor to overlay manager on mode changes
When the user triggers a mode change (toggle or temporary mode), the palette SHALL inform the overlay manager which monitor it is currently on.

#### Scenario: Mode toggle includes monitor information
- **GIVEN** the palette is positioned on monitor B
- **WHEN** the user clicks the toggle draw mode button
- **THEN** the palette calls the overlay manager with the mode toggle command
- **AND** provides the current monitor (B) as part of the command

#### Scenario: Temporary mode activation includes monitor information
- **GIVEN** the palette is positioned on monitor A
- **WHEN** temporary draw mode is activated (e.g., Shift key pressed)
- **THEN** the palette provides monitor A information to the overlay manager
- **AND** only monitor A's overlay enters draw mode
