# global-hotkeys Delta

## ADDED Requirements

### Requirement: Alt+S cycles pen color during temporary draw mode
The system SHALL detect Alt+S key combination when temporary draw mode is active (Alt key held after double-tap activation). Pressing S while holding Alt SHALL cycle the pen color through red → blue → green → red sequence.

#### Scenario: Alt+S cycles color in temporary draw mode
- **GIVEN** temporary draw mode is active (Alt held after double-tap)
- **WHEN** the user presses S while continuing to hold Alt
- **THEN** the pen color cycles to the next color in sequence
- **AND** the control palette button updates to reflect the new color.

#### Scenario: Alt+S does not interfere with drawing
- **GIVEN** temporary draw mode is active and the user is drawing
- **WHEN** the user presses S while holding Alt
- **THEN** the color changes without disrupting the current stroke or drawing operation.

#### Scenario: Alt+S has no effect outside temporary draw mode
- **GIVEN** the application is in pass-through mode
- **WHEN** the user presses Alt+S
- **THEN** no color change occurs
- **AND** no error or side effect is observed.

#### Scenario: Alt+S has no effect in permanent draw mode
- **GIVEN** the application is in permanent draw mode (toggled via Win+Shift+D, not temporary)
- **WHEN** the user presses Alt+S
- **THEN** no color change occurs.

#### Scenario: S key alone has no effect
- **GIVEN** temporary draw mode is active
- **WHEN** the user presses S without holding Alt
- **THEN** no color change occurs
- **AND** normal keyboard behavior is preserved.
