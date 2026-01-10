# actions-consistency Specification

## Purpose
TBD - created by archiving change add-control-palette-and-multi-monitor-overlay. Update Purpose after archive.
## Requirements
### Requirement: Palette actions match hotkey actions
The system SHALL ensure that palette operations (toggle draw mode, clear all, quit) perform the same actions as their corresponding global hotkeys.

#### Scenario: Toggle via hotkey and palette is equivalent
- **GIVEN** the application is running
- **WHEN** the user toggles draw mode via the hotkey and via the palette
- **THEN** the resulting mode state is consistent.

#### Scenario: Clear via hotkey and palette is equivalent
- **GIVEN** the application is running and strokes exist
- **WHEN** the user clears via hotkey or via palette
- **THEN** strokes are removed in the same way.

