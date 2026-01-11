# inking-canvas Spec Delta

## ADDED Requirements

### Requirement: Constrained straight line drawing with Shift modifier
When drawing in draw mode (permanent or temporary), the system SHALL constrain strokes to straight horizontal or vertical lines when the user holds the Shift key during a drag gesture.

#### Scenario: Draw horizontal straight line with Shift held
- **GIVEN** the overlay is in draw mode
- **WHEN** the user holds Shift and performs a drag gesture where the horizontal distance is greater than or equal to the vertical distance
- **THEN** a horizontal straight line is rendered from the drag start point to the current mouse position (Y coordinate constrained to start point Y).

#### Scenario: Draw vertical straight line with Shift held
- **GIVEN** the overlay is in draw mode
- **WHEN** the user holds Shift and performs a drag gesture where the vertical distance is greater than the horizontal distance
- **THEN** a vertical straight line is rendered from the drag start point to the current mouse position (X coordinate constrained to start point X).

#### Scenario: Transition from freehand to constrained mid-stroke
- **GIVEN** the user is drawing a freehand stroke (left button held, Shift not pressed)
- **WHEN** the user presses Shift while continuing to drag
- **THEN** the stroke transitions to a straight line from the original start point to the current constrained position, and intermediate freehand points are removed.

#### Scenario: Transition from constrained to freehand mid-stroke
- **GIVEN** the user is drawing a constrained straight line (left button held, Shift pressed)
- **WHEN** the user releases Shift while continuing to drag
- **THEN** the stroke continues as freehand from the last constrained position, and subsequent mouse movements add points normally.

#### Scenario: Straight line constraint works in temporary draw mode
- **GIVEN** temporary draw mode is active (Alt held)
- **WHEN** the user holds Shift and performs a drag gesture
- **THEN** straight line constraint applies as in permanent draw mode.

#### Scenario: Shift modifier does not interfere with hotkeys
- **GIVEN** the application is running
- **WHEN** the user presses Win+Shift+D, Win+Shift+C, or Win+Shift+Q
- **THEN** the corresponding hotkey action is executed (toggle draw mode, clear all, quit) without triggering straight line constraint.

## Rationale
Constrained drawing with the Shift modifier is an industry-standard pattern found in most professional and consumer drawing applications (e.g., Microsoft Paint, Adobe Photoshop, PowerPoint). This feature improves the usability of the drawing overlay by enabling users to create precise horizontal and vertical annotations.

The implementation uses a simple angle-based snapping algorithm: calculate the angle between the stroke start point and current mouse position, and snap to the nearest cardinal axis (0°, 90°, 180°, 270°). A 45-degree threshold determines whether to snap horizontally or vertically.

Allowing mid-stroke transitions (pressing or releasing Shift during a drag) provides flexibility and matches the behavior of most drawing tools. The constraint applies independently to each stroke, and does not persist across multiple strokes.

## Cross-References
- Related to `global-hotkeys` spec delta: Shift key is freed from temporary mode trigger, enabling this constraint feature
- Maintains compatibility with existing hotkeys that use Win+Shift combinations
