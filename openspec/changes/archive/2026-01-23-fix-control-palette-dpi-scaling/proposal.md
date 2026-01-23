# Proposal: Fix Control Palette DPI Scaling

## Why

The control palette window displays icon rendering issues when the Windows display scaling (DPI) setting is changed. Icons become distorted or improperly sized because ControlWindow does not respond to `WM_DPICHANGED` messages, unlike OverlayWindow which handles DPI changes correctly.

### Background

The issue was identified through investigation of icon rendering problems. Key findings:

1. **OverlayWindow handles DPI changes** - The OverlayWindow.xaml.cs implements `WM_DPICHANGED` message handling and properly recalculates window bounds and DPI scaling when the system DPI changes.

2. **ControlWindow lacks DPI handling** - The ControlWindow.xaml.cs only processes `WM_HOTKEY` messages in its `WndProc` method and does not handle `WM_DPICHANGED`, causing icons to render incorrectly when display scaling changes.

3. **Fixed window dimensions** - ControlWindow uses fixed pixel dimensions (Width="56" Height="204") without DPI-aware scaling.

4. **No DPI awareness manifest** - The application lacks an app manifest file declaring DPI awareness mode (e.g., Per-Monitor V2), relying on WPF's default DPI handling.

## What Changes

Implement proper DPI awareness for the control palette by:

1. **Add DPI change handling to ControlWindow** - Implement `WM_DPICHANGED` message processing in ControlWindow.xaml.cs, following the same pattern used in OverlayWindow to track DPI values and adjust window geometry.

2. **Add application manifest with Per-Monitor V2 DPI awareness** - Create an app.manifest file declaring Per-Monitor V2 DPI awareness mode for predictable and consistent DPI behavior across all windows.

3. **Update project configuration** - Configure DesktopInk.csproj to include the manifest file.

### Benefits

- Icons render correctly when display scaling is changed
- Consistent DPI handling across all application windows
- Better support for multi-monitor setups with different DPI settings
- Improved visual quality on high-DPI displays

### Scope

This change modifies the `control-palette` capability to ensure proper DPI scaling behavior.

### Dependencies

None - this is a self-contained fix to existing functionality.

### Risks

- Minimal risk - follows the established pattern already proven in OverlayWindow
- May require testing on various DPI settings and multi-monitor configurations
