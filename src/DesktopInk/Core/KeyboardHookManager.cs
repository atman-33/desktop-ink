using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using DesktopInk.Infrastructure;

namespace DesktopInk.Core;

internal sealed class KeyboardHookManager : IDisposable
{
    private IntPtr _hookId = IntPtr.Zero;
    private Win32.LowLevelKeyboardProc? _hookProc;
    private bool _isDisposed;
    private DateTime _lastShiftPressTime;
    private DateTime _lastShiftReleaseTime;
    private bool _isShiftHeld;
    private bool _waitingForSecondPress;
    private readonly int _doubleClickThreshold;

    public event EventHandler? TemporaryModeActivated;
    public event EventHandler? TemporaryModeDeactivated;

    public KeyboardHookManager()
    {
        _doubleClickThreshold = Win32.GetDoubleClickTime();
        _hookProc = HookCallback;
        AppLog.Info($"KeyboardHookManager: Init. DoubleClickThreshold={_doubleClickThreshold}ms");
    }

    public void Install()
    {
        if (_hookId != IntPtr.Zero) return;
        using var curProcess = Process.GetCurrentProcess();
        using var curModule = curProcess.MainModule;
        if (curModule is null) throw new InvalidOperationException("Could not get main module");
        _hookId = Win32.SetWindowsHookEx(Win32.WhKeyboardLl, _hookProc!, Win32.GetModuleHandle(curModule.ModuleName), 0);
        if (_hookId == IntPtr.Zero)
        {
            var error = Marshal.GetLastWin32Error();
            throw new InvalidOperationException($"Failed to install keyboard hook. Error: {error}");
        }
        AppLog.Info("KeyboardHookManager: Installed keyboard hook.");
    }

    public void Uninstall()
    {
        if (_hookId == IntPtr.Zero) return;
        Win32.UnhookWindowsHookEx(_hookId);
        _hookId = IntPtr.Zero;
        AppLog.Info("KeyboardHookManager: Uninstalled keyboard hook.");
    }

    private IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
    {
        if (nCode >= 0)
        {
            var kbd = Marshal.PtrToStructure<Win32.KbdllHookStruct>(lParam);
            var vkCode = kbd.VkCode;
            if (vkCode == Win32.VkLShift || vkCode == Win32.VkRShift)
            {
                var isKeyDown = wParam == (IntPtr)Win32.WmKeydown || wParam == (IntPtr)Win32.WmSyskeydown;
                var isKeyUp = wParam == (IntPtr)Win32.WmKeyup || wParam == (IntPtr)Win32.WmSyskeyup;
                AppLog.Info($"KBHook: Shift vk=0x{vkCode:X2} down={isKeyDown} up={isKeyUp} held={_isShiftHeld} wait={_waitingForSecondPress}");
                if (isKeyDown) HandleShiftPress();
                else if (isKeyUp) HandleShiftRelease();
            }
        }
        return Win32.CallNextHookEx(_hookId, nCode, wParam, lParam);
    }

    private void HandleShiftPress()
    {
        var now = DateTime.UtcNow;
        if (_isShiftHeld)
        {
            AppLog.Info("KBHook: Shift press ignored (already held).");
            return;
        }
        if (_waitingForSecondPress)
        {
            var timeSinceRelease = (now - _lastShiftReleaseTime).TotalMilliseconds;
            AppLog.Info($"KBHook: 2nd press. Time={timeSinceRelease:F1}ms Thresh={_doubleClickThreshold}ms");
            if (timeSinceRelease <= _doubleClickThreshold)
            {
                _isShiftHeld = true;
                _waitingForSecondPress = false;
                TemporaryModeActivated?.Invoke(this, EventArgs.Empty);
                AppLog.Info("KBHook: TempMode ACTIVATED.");
            }
            else
            {
                _waitingForSecondPress = false;
                _lastShiftPressTime = now;
                AppLog.Info("KBHook: Too slow, reset.");
            }
        }
        else
        {
            _lastShiftPressTime = now;
            AppLog.Info("KBHook: First Shift press.");
        }
    }

    private void HandleShiftRelease()
    {
        var now = DateTime.UtcNow;
        if (_isShiftHeld)
        {
            _isShiftHeld = false;
            _waitingForSecondPress = false;
            TemporaryModeDeactivated?.Invoke(this, EventArgs.Empty);
            AppLog.Info("KBHook: TempMode DEACTIVATED.");
        }
        else if (!_waitingForSecondPress)
        {
            _lastShiftReleaseTime = now;
            _waitingForSecondPress = true;
            AppLog.Info("KBHook: Released, waiting for 2nd press...");
        }
        else
        {
            _waitingForSecondPress = false;
            AppLog.Info("KBHook: Released while waiting, cancelled.");
        }
    }

    public void Dispose()
    {
        if (_isDisposed) return;
        Uninstall();
        _isDisposed = true;
    }
}
