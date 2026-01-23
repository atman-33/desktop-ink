# control-palette Specification Delta

## ADDED Requirements

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
