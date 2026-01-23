# control-palette Specification Delta

## MODIFIED Requirements

### Requirement: Icons scale properly
The palette icons SHALL scale properly according to the current display DPI setting.

#### Scenario: Icons scale properly
- **GIVEN** the palette buttons have a fixed logical size (36×36px)
- **WHEN** the icons are rendered at any Windows display scaling level (100%, 125%, 150%, 175%, 200%)
- **THEN** the icons maintain proper proportions and clarity
- **AND** no pixelation or distortion occurs

#### Scenario: Icons remain clear after DPI change
- **GIVEN** the application is running with the control palette visible
- **WHEN** the user changes the Windows display scaling setting
- **THEN** the control palette icons update to render correctly at the new DPI
- **AND** the icons remain clear and properly sized

#### Scenario: Icons scale correctly across monitors with different DPI
- **GIVEN** the system has multiple monitors with different DPI settings
- **WHEN** the user drags the control palette from one monitor to another
- **THEN** the control palette adjusts to the target monitor's DPI
- **AND** the icons render correctly on the new monitor

### Requirement: Control palette responds to system DPI changes
The control palette window SHALL respond to Windows `WM_DPICHANGED` messages to maintain proper rendering when display scaling changes.

#### Scenario: Handle DPI change message
- **GIVEN** the control palette is displayed on a monitor
- **WHEN** Windows sends a `WM_DPICHANGED` message (due to display scaling change or monitor transition)
- **THEN** the control palette updates its internal DPI tracking
- **AND** the window geometry adjusts appropriately

#### Scenario: Application declares DPI awareness
- **GIVEN** the application is installed on a Windows 10 system
- **WHEN** the application starts
- **THEN** Windows recognizes the application as Per-Monitor V2 DPI aware
- **AND** the system enables proper DPI scaling behavior
