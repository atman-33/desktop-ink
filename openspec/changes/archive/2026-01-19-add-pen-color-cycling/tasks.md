# Tasks

## 1. Core Color Management
- [x] 1.1 Create `PenColor` enum with Red, Blue, Green values in `OverlayMode.cs` or separate file
- [x] 1.2 Add current color state to `OverlayManager` with default Red
- [x] 1.3 Add color change event to `OverlayManager` for notifying overlays
- [x] 1.4 Implement `CycleColor()` method in `OverlayManager` (Red→Blue→Green→Red)

## 2. Stroke Rendering
- [x] 2.1 Modify `OverlayWindow.CreateStroke()` to accept a `PenColor` parameter
- [x] 2.2 Add color-to-brush mapping logic (Red→#FF0000, Blue→#0000FF, Green→#00FF00)
- [x] 2.3 Add `SetPenColor(PenColor)` method to `OverlayWindow` to update current color
- [x] 2.4 Ensure `OverlayManager` propagates color changes to all `OverlayWindow` instances

## 3. Control Palette Button
- [x] 3.1 Add color cycle button XAML to `ControlWindow.xaml` below Toggle Draw button
- [x] 3.2 Use circular or color swatch icon geometry (16×16 circle in 36×36 button)
- [x] 3.3 Bind button's icon fill color to current pen color using data trigger or code-behind
- [x] 3.4 Implement `OnColorCycleClick` event handler in `ControlWindow.xaml.cs`
- [x] 3.5 Call `OverlayManager.CycleColor()` from click handler
- [x] 3.6 Subscribe to `OverlayManager` color change event to update button appearance
- [x] 3.7 Add tooltip: "Cycle Color (Alt+S in temp mode)"

## 4. Keyboard Shortcut (Alt+S)
- [x] 4.1 Add S key detection to `KeyboardHookManager.HookCallback`
- [x] 4.2 Create `ColorCycleRequested` event in `KeyboardHookManager`
- [x] 4.3 Trigger event only when `_isAltHeld` is true (temporary draw mode active)
- [x] 4.4 Subscribe to `ColorCycleRequested` event in `ControlWindow.OnSourceInitialized`
- [x] 4.5 Call `OverlayManager.CycleColor()` from event handler
- [x] 4.6 Ensure Alt+S does not conflict with system shortcuts

## 5. Testing & Validation
- [x] 5.1 Verify color cycles correctly: red → blue → green → red
- [x] 5.2 Verify button icon color updates immediately after cycling
- [x] 5.3 Verify new strokes use current color, existing strokes retain original colors
- [x] 5.4 Verify Alt+S cycles color during temporary draw mode
- [x] 5.5 Verify Alt+S has no effect outside temporary draw mode
- [x] 5.6 Verify color persists correctly across monitor switches
- [x] 5.7 Verify default color is red on application startup
- [x] 5.8 Test on multi-monitor setup for color consistency

## 6. Documentation
- [x] 6.1 Update README.md with Alt+S shortcut description
- [x] 6.2 Update button tooltips to reflect color cycling feature
