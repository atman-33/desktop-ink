# global-hotkeys Spec Delta

## ADDED Requirements

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

## MODIFIED Requirements

None - existing hotkey requirements remain unchanged.

## REMOVED Requirements

None.
