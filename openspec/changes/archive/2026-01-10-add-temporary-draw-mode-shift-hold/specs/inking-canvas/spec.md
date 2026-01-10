# inking-canvas Spec Delta

## ADDED Requirements

### Requirement: Auto-clear strokes on temporary mode exit
When temporary draw mode is deactivated (Shift key released), the system SHALL automatically clear all strokes from the canvas across all monitors.

#### Scenario: Strokes are cleared when Shift is released
- **GIVEN** temporary draw mode is active and the user has drawn one or more strokes
- **WHEN** the user releases the Shift key
- **THEN** all strokes are immediately cleared from all overlay windows.

#### Scenario: Auto-clear works on multi-monitor setup
- **GIVEN** temporary draw mode is active with strokes on multiple monitors
- **WHEN** the user releases the Shift key
- **THEN** all strokes across all monitors are cleared simultaneously.

## MODIFIED Requirements

### Requirement: Clear-all removes all strokes
The system SHALL provide a clear-all action that removes all strokes from the overlay. Clear-all can be invoked manually via hotkey (`Win+Shift+C`) or automatically when temporary draw mode is deactivated.

#### Scenario: Clear all drawings manually
- **GIVEN** there are one or more strokes on the overlay
- **WHEN** the clear-all action is invoked via `Win+Shift+C`
- **THEN** the overlay contains no visible strokes.

#### Scenario: Clear all drawings automatically on temporary mode exit
- **GIVEN** temporary draw mode is active with strokes on the overlay
- **WHEN** the user releases the Shift key
- **THEN** the overlay contains no visible strokes.

## REMOVED Requirements

None.
