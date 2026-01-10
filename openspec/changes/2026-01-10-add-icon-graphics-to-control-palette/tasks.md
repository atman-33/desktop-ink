# Implementation Tasks

## 1. Icon Resource Selection
- [x] Select appropriate SVG icon sources (Fluent UI System Icons or Material Design Icons)
- [x] Extract SVG Path data for pen/pencil icon (draw mode)
- [x] Extract SVG Path data for eraser or trash bin icon (clear)
- [x] Extract SVG Path data for close × icon (quit)
- [x] Verify icon licenses are compatible (MIT, Apache 2.0, or CC BY 4.0)

## 2. XAML Implementation
- [x] Update `ControlWindow.xaml` to replace button Content with Viewbox + Path elements
- [x] Implement pen/pencil icon for ToggleButton (draw mode)
- [x] Implement eraser/trash bin icon for Clear button
- [x] Implement close × icon for Quit button
- [x] Set Viewbox dimensions (20×20px recommended)
- [x] Configure Path Fill color to White for visibility on dark background
- [x] Ensure Stretch="Uniform" for proper scaling

## 3. Visual State Management
- [x] Implement visual state feedback for draw mode toggle button
- [x] Add Style or Trigger to change appearance when draw mode is active
- [x] Test color change or highlight effect for active state (e.g., green, blue, or brighter shade)
- [x] Ensure state changes are synchronized with `_overlayManager.CurrentMode`

## 4. Testing
- [x] Verify all three icons render correctly on startup
- [x] Test draw mode toggle button shows correct state (active/inactive)
- [x] Verify icons are clearly visible against dark background (#AA000000)
- [x] Test icons remain clear at default palette size (56×160px window)
- [x] Verify no pixelation or rendering artifacts
- [x] Confirm tooltips still function correctly with icon content
- [x] Test on high DPI displays to ensure vector scaling works properly
- [x] Verify hover and click interactions remain responsive

## 5. Code Review
- [x] Review XAML for consistency and maintainability
- [x] Ensure no hardcoded values that should be configurable
- [x] Verify accessibility (tooltips provide text alternatives for icons)
- [x] Document icon sources and licenses in code comments

## 6. Documentation
- [x] Update relevant comments in `ControlWindow.xaml` to reference icon implementation
- [x] Document icon design choices (which icons were selected and why)
- [x] Note icon sources and license information

## Implementation Notes

### Recommended Icon Sources
- **Fluent UI System Icons**: https://github.com/microsoft/fluentui-system-icons (MIT License)
- **Material Design Icons**: https://github.com/google/material-design-icons (Apache 2.0 License)
- **Font Awesome** (Free): https://fontawesome.com (CC BY 4.0)

### Example XAML Pattern
```xaml
<Button x:Name="ToggleButton" Width="36" Height="36" Click="OnToggleClick">
    <Viewbox Width="20" Height="20">
        <Path Data="M..." Fill="White" Stretch="Uniform"/>
    </Viewbox>
</Button>
```

### Visual State Options
- Use Style with DataTrigger bound to overlay mode
- Change Fill color when active (e.g., LightGreen, LightBlue)
- Or change Background color of button itself
