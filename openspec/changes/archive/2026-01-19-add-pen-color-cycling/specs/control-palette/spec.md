# control-palette Delta

## ADDED Requirements

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
