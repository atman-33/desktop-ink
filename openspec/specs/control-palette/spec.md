# control-palette Specification

## Purpose
TBD - created by archiving change add-control-palette-and-multi-monitor-overlay. Update Purpose after archive.
## Requirements
### Requirement: Always-on-top semi-transparent palette
The system SHALL provide a small, semi-transparent palette window that stays on top of other windows.

#### Scenario: Palette is visible
- **GIVEN** the application is running
- **WHEN** the user looks at the desktop
- **THEN** the palette is visible and remains topmost.

### Requirement: Palette provides click controls for core actions
The palette SHALL provide click controls with **visual SVG icons** for:
- Toggle draw/pass-through mode (pen/pencil icon)
- Cycle pen color (color button)
- Clear all strokes (eraser or trash bin icon)
- Quit the application (close × icon)

The icons SHALL be rendered using WPF Path elements with vector geometry for resolution independence.

#### Scenario: Icons are clearly visible
- **GIVEN** the palette is visible
- **WHEN** the user views the palette buttons
- **THEN** each button displays a recognizable vector icon instead of text
- **AND** the icons are clearly visible against the dark background

#### Scenario: Draw mode state is visually indicated
- **GIVEN** the application is running
- **WHEN** draw mode is toggled on or off
- **THEN** the draw mode button icon shows visual feedback of the current state
- **AND** the state change is immediately apparent to the user

#### Scenario: Icons scale properly
- **GIVEN** the palette buttons have a fixed size (36×36px)
- **WHEN** the icons are rendered
- **THEN** the icons maintain proper proportions and clarity
- **AND** no pixelation or distortion occurs

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

### Requirement: Palette determines active draw monitor
The system SHALL use the control palette's location to determine which monitor is active for drawing.

#### Scenario: Drag palette to switch monitors
- **GIVEN** the application is in draw mode
- **WHEN** the user drags the control palette from Monitor A to Monitor B
- **THEN** drawing becomes disabled on Monitor A (pass-through)
- **AND** drawing becomes enabled on Monitor B

### Requirement: Default placement near primary right edge
On startup, the system SHALL place the palette near the right edge of the primary monitor.

#### Scenario: Initial placement
- **GIVEN** the application has just started
- **WHEN** the palette window is shown
- **THEN** it appears near the right edge of the primary monitor.

### Requirement: Icon specifications
The system SHALL implement icons with the following specifications:
- Icon size: 20×20px within 36×36px button area
- Fill color: White (#FFFFFF) for default state
- Format: SVG Path geometry embedded in XAML
- Design source: Standard icon sets (Fluent UI System Icons or Material Design Icons)
- Viewbox: Used for proper scaling within button boundaries

#### Scenario: Draw mode icon represents drawing action
- **GIVEN** the draw mode toggle button is visible
- **WHEN** the user views the button
- **THEN** the icon resembles a pen or pencil at approximately 45° angle
- **AND** the icon clearly communicates the drawing function

#### Scenario: Clear icon represents deletion
- **GIVEN** the clear button is visible
- **WHEN** the user views the button
- **THEN** the icon resembles an eraser or trash bin
- **AND** the icon clearly communicates the clearing function

#### Scenario: Quit icon represents closing
- **GIVEN** the quit button is visible
- **WHEN** the user views the button
- **THEN** the icon resembles a close symbol (×)
- **AND** the icon clearly communicates the exit function

### Requirement: Visual state feedback for draw mode toggle
The draw mode toggle button SHALL provide distinct visual feedback for its active and inactive states through color or appearance changes.

#### Scenario: Draw mode active state
- **GIVEN** draw mode is currently active
- **WHEN** the user views the draw mode button
- **THEN** the button displays a distinct appearance (e.g., different color, highlighted state)
- **AND** the active state is immediately recognizable

#### Scenario: Draw mode inactive state
- **GIVEN** draw mode is currently inactive (pass-through mode)
- **WHEN** the user views the draw mode button
- **THEN** the button displays its default appearance
- **AND** clearly differs from the active state

### Requirement: Color cycle button
The palette SHALL provide a color cycle button positioned below the Toggle Draw button and above the Clear button. The button SHALL display a visual indicator of the currently selected pen color through its icon fill color.

#### Scenario: Color cycle button is visible
- **GIVEN** the control palette is visible
- **WHEN** the user views the palette
- **THEN** a color cycle button is visible between the Toggle Draw and Clear buttons.

#### Scenario: Button icon color matches selected pen color
- **GIVEN** the current pen color is blue
- **WHEN** the user views the color cycle button
- **THEN** the button's icon fill color is blue
- **AND** the visual feedback clearly indicates the current color selection.

#### Scenario: Clicking the button cycles the pen color
- **GIVEN** the current pen color is red
- **WHEN** the user clicks the color cycle button
- **THEN** the pen color changes to blue
- **AND** the button icon color updates to blue immediately.

#### Scenario: Button tooltip describes functionality
- **GIVEN** the user hovers over the color cycle button
- **WHEN** the tooltip appears
- **THEN** it displays "Cycle Color (Alt+S in temp mode)" or similar descriptive text.

#### Scenario: Button icon is distinct and recognizable
- **GIVEN** the color cycle button is visible
- **WHEN** the user views the button
- **THEN** the icon clearly represents color or drawing (e.g., circular color swatch, palette icon)
- **AND** the icon remains clear and unobscured at 16×16px or 20×20px size within the 36×36px button.

### Requirement: Palette button tooltips SHALL include keyboard shortcuts and advanced interactions
Tooltips for palette buttons SHALL inform users about both primary and secondary interaction methods when applicable, including keyboard shortcuts and advanced interactions.

#### Scenario: Toggle Draw button tooltip reveals temporary mode interaction
- **GIVEN** the Toggle Draw button is visible
- **WHEN** the user hovers over the button
- **THEN** the tooltip displays information about the primary interaction (click to toggle)
- **AND** the tooltip displays information about the keyboard shortcut (Win+Shift+D)
- **AND** the tooltip displays information about the Alt+Double-click interaction for temporary mode
- **AND** all information is clearly readable in a multi-line format

#### Scenario: Tooltip text is concise and formatted
- **GIVEN** the Toggle Draw button tooltip is displayed
- **WHEN** the user reads the tooltip
- **THEN** the text is organized in a clear, multi-line format
- **AND** the tooltip does not exceed three lines
- **AND** each line conveys one distinct piece of information

### Requirement: Tooltips SHALL use multi-line formatting for clarity
When tooltips contain multiple pieces of information, the system SHALL format them as multi-line text for improved readability.

#### Scenario: Toggle Draw tooltip uses line breaks
- **GIVEN** the Toggle Draw tooltip contains multiple pieces of information
- **WHEN** the tooltip is displayed
- **THEN** each distinct piece of information appears on a separate line
- **AND** line breaks improve visual scanning and comprehension

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

### Requirement: Control palette icons render correctly at all DPI levels

The control palette icons SHALL render clearly and without distortion at all Windows display scaling levels (100%, 125%, 150%, 175%, 200%).

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

