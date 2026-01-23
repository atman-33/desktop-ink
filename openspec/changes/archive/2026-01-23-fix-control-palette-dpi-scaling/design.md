# Design: Fix Control Palette DPI Scaling

## Overview
This change implements proper DPI awareness for the control palette window to fix icon rendering issues when Windows display scaling changes. The solution follows the existing pattern established in OverlayWindow.

## Technical Approach

### 1. DPI Change Detection
ControlWindow will respond to `WM_DPICHANGED` (0x02E0) messages sent by Windows when:
- The user changes display scaling in system settings
- The window is moved between monitors with different DPI settings

### 2. DPI Value Storage
Add private fields to ControlWindow:
```csharp
private uint _dpiX;
private uint _dpiY;
```

Initialize in constructor or `OnSourceInitialized` using `Win32.GetDpiForWindow(_hwnd)`, defaulting to 96 (100% scaling) if unavailable.

### 3. Message Handling Pattern
Extend existing `WndProc` method to handle `WM_DPICHANGED`:

```csharp
private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
{
    if (msg == Win32.WmHotkey)
    {
        // ... existing hotkey handling ...
    }
    
    if (msg == Win32.WmDpichanged)
    {
        // Extract new DPI from wParam (LOWORD=X, HIWORD=Y)
        var newDpiX = (uint)(wParam.ToInt32() & 0xFFFF);
        var newDpiY = (uint)((wParam.ToInt32() >> 16) & 0xFFFF);
        
        // Update stored values (default to 96 if 0)
        _dpiX = newDpiX == 0 ? 96 : newDpiX;
        _dpiY = newDpiY == 0 ? 96 : newDpiY;
        
        // Optional: Apply recommended bounds from lParam
        // (May not be necessary if WPF handles layout automatically)
        
        handled = true;
        return IntPtr.Zero;
    }
    
    return IntPtr.Zero;
}
```

### 4. Application Manifest
Create `src/DesktopInk/app.manifest` with the following DPI awareness declarations:

```xml
<?xml version="1.0" encoding="utf-8"?>
<assembly manifestVersion="1.0" xmlns="urn:schemas-microsoft-com:asm.v1">
  <assemblyIdentity version="1.0.0.0" name="DesktopInk.app"/>
  
  <compatibility xmlns="urn:schemas-microsoft-com:compatibility.v1">
    <application>
      <!-- Windows 10 and later -->
      <supportedOS Id="{8e0f7a12-bfb3-4fe8-b9a5-48fd50a15a9a}" />
    </application>
  </compatibility>
  
  <application xmlns="urn:schemas-microsoft-com:asm.v3">
    <windowsSettings>
      <!-- Per-Monitor V2 DPI awareness for Windows 10 1703+ -->
      <dpiAwareness xmlns="http://schemas.microsoft.com/SMI/2016/WindowsSettings">PerMonitorV2</dpiAwareness>
      <!-- Fallback for earlier Windows 10 versions -->
      <dpiAware xmlns="http://schemas.microsoft.com/SMI/2005/WindowsSettings">true/PM</dpiAware>
    </windowsSettings>
  </application>
</assembly>
```

## Why This Approach

### Consistency with OverlayWindow
OverlayWindow already implements this pattern successfully. By using the same approach, we ensure:
- Consistent behavior across all application windows
- Proven, working code pattern
- Easier maintenance

### Per-Monitor V2 DPI Awareness
This is the recommended DPI awareness mode for modern Windows applications:
- Automatic DPI scaling by Windows for WPF content
- `WM_DPICHANGED` messages sent on DPI changes
- Smooth scaling when moving between monitors
- Works with Windows 10 version 1703 (Creators Update) and later

### Minimal Code Changes
The fix requires minimal changes to ControlWindow:
- Add 2 member variables
- Add ~15 lines to WndProc
- Create manifest file
- Update project file

## Alternative Approaches Considered

### 1. Rely on WPF's Default DPI Handling
**Rejected** - Current behavior shows this is insufficient. The icons still render incorrectly without explicit DPI message handling.

### 2. Use System-Aware DPI Mode
**Rejected** - System-aware mode scales the entire application uniformly based on the primary monitor. This provides a worse user experience in multi-monitor setups with mixed DPI settings.

### 3. Set Layout Properties (UseLayoutRounding, SnapsToDevicePixels)
**Rejected as primary solution** - These properties help with sub-pixel rendering but don't address the root cause of the window not responding to DPI changes. May be useful as supplementary improvements.

## Testing Strategy

### Manual Testing Steps
1. **Single monitor DPI changes**:
   - Start application at 100% scaling
   - Change Windows display scaling to 125%, 150%, 175%, 200%
   - Verify icons remain clear at each setting

2. **Multi-monitor DPI differences**:
   - Set up monitors with different DPI settings
   - Drag control palette between monitors
   - Verify icons scale correctly during and after the move

3. **Regression testing**:
   - Verify OverlayWindow still handles DPI correctly
   - Verify all hotkeys still work
   - Verify dragging functionality still works

## Dependencies on Existing Code

### Win32 Constants
Requires `Win32.WmDpichanged` constant (already defined as `0x02E0` in `Infrastructure/Win32.cs`).

### Win32 Functions
May use existing Win32 interop:
- `Win32.GetDpiForWindow(_hwnd)` - for initial DPI detection
- `Win32.SetWindowPos` - if manual positioning is needed after DPI change

## Risks and Mitigations

### Risk: Breaking Existing Layout
**Mitigation**: The change follows the proven OverlayWindow pattern. If issues arise, the icon geometry is vector-based and should scale correctly with proper DPI handling.

### Risk: Compatibility with Older Windows Versions
**Mitigation**: The manifest includes fallback `dpiAware` setting for Windows versions that don't support Per-Monitor V2. Application targets Windows 10+, which has good DPI awareness support.

### Risk: Performance Impact
**Mitigation**: DPI changes are infrequent user-initiated events. The message handling adds negligible overhead.

## Future Considerations

This implementation provides the foundation for:
- Enhanced DPI logging for diagnostics
- Dynamic icon sizing based on DPI
- User preferences for control palette size

However, these are out of scope for this fix, which focuses on correcting the icon rendering issue.
