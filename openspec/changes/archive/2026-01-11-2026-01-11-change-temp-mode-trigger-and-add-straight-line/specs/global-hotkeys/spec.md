# global-hotkeys Spec Delta

## MODIFIED Requirements

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

## Rationale
Changing the temporary mode trigger from Shift to Alt avoids conflicts with standard drawing modifiers. The Shift key is conventionally used in drawing applications to constrain geometry (e.g., draw straight lines, perfect circles). By using Alt instead, we preserve the Shift key for future constraint features while maintaining the quick temporary mode access pattern.

The Alt key is less commonly used during active drawing workflows, reducing the likelihood of accidental activation. The double-click + hold pattern remains the same, ensuring user familiarity with the gesture.

## Cross-References
- Related to `inking-canvas` spec delta: Frees up Shift key for straight line constraint feature
- Maintains compatibility with existing permanent draw mode toggle (`Win+Shift+D`)
