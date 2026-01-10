# control-palette Specification (Delta)

## MODIFIED Requirements

### Requirement: Palette provides click controls for core actions
The palette SHALL provide click controls with **visual SVG icons** for:
- Toggle draw/pass-through mode (pen/pencil icon)
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

## ADDED Requirements

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
