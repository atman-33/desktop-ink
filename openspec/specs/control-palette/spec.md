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

