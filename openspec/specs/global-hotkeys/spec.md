# global-hotkeys Specification

## Purpose
TBD - created by archiving change add-drawing-overlay-mvp. Update Purpose after archive.
## Requirements
### Requirement: Register global hotkeys
The system SHALL register the following global hotkeys so they work even when the application is not focused:
- `Win+Shift+D` to toggle draw/pass-through mode
- `Win+Shift+C` to clear all strokes
- `Win+Shift+Q` to quit the application

#### Scenario: Hotkeys work while the app is unfocused
- **GIVEN** the application is running in the background
- **WHEN** the user presses a configured hotkey
- **THEN** the corresponding action is executed.

### Requirement: Mode toggle updates input behavior
When the user toggles modes via `Win+Shift+D`, the system SHALL switch between draw mode and pass-through mode, and the input behavior SHALL change accordingly.

#### Scenario: Toggle from pass-through to draw mode
- **GIVEN** the overlay is in pass-through mode
- **WHEN** the user presses `Win+Shift+D`
- **THEN** the overlay enters draw mode and starts receiving pointer input.

### Requirement: Temporary draw mode via Shift double-click and hold
The system SHALL activate a temporary draw mode when the user double-clicks the Alt key and holds it down, and SHALL deactivate temporary draw mode when the Alt key is released.

**Changed from:** Shift double-click and hold → Alt double-click and hold

#### Scenario: Activate temporary draw mode with Alt double-click and hold
- **GIVEN** the application is running in any mode
- **WHEN** the user double-clicks the Alt key (two rapid presses within system double-click threshold) and holds it down
- **THEN** temporary draw mode is activated and the overlay starts receiving pointer input.

**Changed from:** User double-clicks the Shift key → User double-clicks the Alt key

#### Scenario: Deactivate temporary draw mode on Alt release
- **GIVEN** temporary draw mode is active (Alt is being held)
- **WHEN** the user releases the Alt key
- **THEN** temporary draw mode is deactivated and the overlay returns to pass-through mode.

**Changed from:** Shift is being held / releases the Shift key → Alt is being held / releases the Alt key

### Requirement: Temporary mode independence from permanent toggle
The temporary draw mode SHALL work independently from the permanent draw mode toggle (`Win+Shift+D`), and temporary mode SHALL take precedence when both are active.

#### Scenario: Temporary mode activates while permanent draw mode is off
- **GIVEN** the overlay is in pass-through mode (permanent draw mode is off)
- **WHEN** the user activates temporary draw mode via Shift double-click and hold
- **THEN** temporary draw mode is active regardless of permanent mode state.

#### Scenario: Temporary mode takes precedence over permanent mode
- **GIVEN** permanent draw mode is already active
- **WHEN** the user activates temporary draw mode
- **THEN** temporary draw mode takes precedence and controls the overlay behavior.

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

