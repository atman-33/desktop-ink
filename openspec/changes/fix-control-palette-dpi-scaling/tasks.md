# Tasks: Fix Control Palette DPI Scaling

## Implementation Tasks

- [ ] **Add DPI member variables to ControlWindow** - Add `_dpiX` and `_dpiY` fields to track the current DPI of the control window, initialized from system DPI at construction.

- [ ] **Implement WM_DPICHANGED handling in ControlWindow.WndProc** - Add `WM_DPICHANGED` message handling to update stored DPI values and window position/size, following the pattern established in OverlayWindow.

- [ ] **Create app.manifest with Per-Monitor V2 DPI awareness** - Create `src/DesktopInk/app.manifest` with proper XML structure declaring `dpiAwareness` as `PerMonitorV2` and `dpiAware` as `true/PM` for backwards compatibility.

- [ ] **Update DesktopInk.csproj to include manifest** - Add `<ApplicationManifest>app.manifest</ApplicationManifest>` property to the project file.

- [ ] **Test on multiple DPI settings** - Verify that control palette icons render correctly when switching between 100%, 125%, 150%, 175%, and 200% display scaling in Windows settings.

- [ ] **Test on multi-monitor with different DPI** - Verify correct rendering when dragging the control palette between monitors with different DPI settings.

## Validation

- [ ] **Visual verification** - Icons remain clear and properly sized at all DPI settings
- [ ] **No regression** - OverlayWindow continues to handle DPI changes correctly
- [ ] **Manual testing** - Change Windows display scaling and verify control palette updates correctly without restart

## Documentation

- [ ] **Code comments** - Add comments explaining DPI handling logic if not already clear from OverlayWindow implementation
