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

