# Change: Add icon graphics to control palette buttons

## Why
The current control palette displays text-only buttons ("D", "C", "Q") which are not intuitive for users. Visual icons using SVG paths would provide immediate recognition of button functions without requiring users to memorize letter meanings or rely solely on tooltips.

## What Changes
- Replace text content ("D", "C", "Q") with SVG-based vector icons
- Add pen/pencil icon for draw mode toggle button
- Add eraser or trash bin icon for clear strokes button
- Add close (×) icon for quit button
- Implement visual state feedback for draw mode toggle (different appearance when active)
- Maintain existing tooltip functionality
- Use WPF Path elements with SVG geometry for scalable, resolution-independent graphics

## Scope
- UI framework: WPF XAML
- Target elements: Three buttons in `ControlWindow.xaml`
- Icon format: SVG Path geometry embedded in XAML
- Icon size: 20×20px within 36×36px buttons
- Colors: White fill on dark background (#AA000000)
- No external image files required

## Relationship to Existing Changes
- Modifies: `control-palette`
  - Updates visual representation of existing buttons
  - No functional changes to button behavior
  - Improves user experience and visual feedback

## Non-Goals
- Changing button functionality or behavior
- Adding animation effects (may be considered in future)
- Making icons configurable or themeable
- Supporting multiple icon sets

## Acceptance Criteria
- All three buttons display recognizable vector icons instead of text
- Icons are clearly visible against the dark background
- Icons scale properly with button size
- Draw mode toggle button shows visual state (active vs inactive)
- Tooltips continue to work correctly
- Icons render smoothly without pixelation
- No performance degradation in rendering

## Icon Design Details
- **Draw mode toggle**: Pen or pencil icon at 45° angle, representing drawing action
- **Clear strokes**: Eraser icon or trash bin icon, representing deletion/clearing
- **Quit**: Close (×) symbol, standard for closing/exiting applications
- All icons use simple, recognizable shapes from common design systems (Fluent UI or Material Design)

## Open Questions
- Should the draw mode toggle use different icons for ON/OFF states, or just different colors?
- Should hover effects be enhanced with icon color changes?
- Future consideration: Should icons support high-contrast themes?
