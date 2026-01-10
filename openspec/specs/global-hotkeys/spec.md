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
The system SHALL activate a temporary draw mode when the user double-clicks the Shift key and holds it down, and SHALL deactivate temporary draw mode when the Shift key is released.

#### Scenario: Activate temporary draw mode with Shift double-click and hold
- **GIVEN** the application is running in any mode
- **WHEN** the user double-clicks the Shift key (two rapid presses within system double-click threshold) and holds it down
- **THEN** temporary draw mode is activated and the overlay starts receiving pointer input.

#### Scenario: Deactivate temporary draw mode on Shift release
- **GIVEN** temporary draw mode is active (Shift is being held)
- **WHEN** the user releases the Shift key
- **THEN** temporary draw mode is deactivated and the overlay returns to pass-through mode.

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

