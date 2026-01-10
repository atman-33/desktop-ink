# Implementation Tasks

## 1. Icon Resource Selection
- [ ] Select appropriate SVG icon sources (Fluent UI System Icons or Material Design Icons)
- [ ] Extract SVG Path data for pen/pencil icon (draw mode)
- [ ] Extract SVG Path data for eraser or trash bin icon (clear)
- [ ] Extract SVG Path data for close × icon (quit)
- [ ] Verify icon licenses are compatible (MIT, Apache 2.0, or CC BY 4.0)

## 2. XAML Implementation
- [ ] Update `ControlWindow.xaml` to replace button Content with Viewbox + Path elements
- [ ] Implement pen/pencil icon for ToggleButton (draw mode)
- [ ] Implement eraser/trash bin icon for Clear button
- [ ] Implement close × icon for Quit button
- [ ] Set Viewbox dimensions (20×20px recommended)
- [ ] Configure Path Fill color to White for visibility on dark background
- [ ] Ensure Stretch="Uniform" for proper scaling

## 3. Visual State Management
- [ ] Implement visual state feedback for draw mode toggle button
- [ ] Add Style or Trigger to change appearance when draw mode is active
- [ ] Test color change or highlight effect for active state (e.g., green, blue, or brighter shade)
- [ ] Ensure state changes are synchronized with `_overlayManager.CurrentMode`

## 4. Testing
- [ ] Verify all three icons render correctly on startup
- [ ] Test draw mode toggle button shows correct state (active/inactive)
- [ ] Verify icons are clearly visible against dark background (#AA000000)
- [ ] Test icons remain clear at default palette size (56×160px window)
- [ ] Verify no pixelation or rendering artifacts
- [ ] Confirm tooltips still function correctly with icon content
- [ ] Test on high DPI displays to ensure vector scaling works properly
- [ ] Verify hover and click interactions remain responsive

## 5. Code Review
- [ ] Review XAML for consistency and maintainability
- [ ] Ensure no hardcoded values that should be configurable
- [ ] Verify accessibility (tooltips provide text alternatives for icons)
- [ ] Document icon sources and licenses in code comments

## 6. Documentation
- [ ] Update relevant comments in `ControlWindow.xaml` to reference icon implementation
- [ ] Document icon design choices (which icons were selected and why)
- [ ] Note icon sources and license information

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
