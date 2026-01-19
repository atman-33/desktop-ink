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
    private DateTime _lastAltPressTime;
    private DateTime _lastAltReleaseTime;
    private bool _isAltHeld;
    private bool _waitingForSecondPress;
    private readonly int _doubleClickThreshold;
    private bool _isSKeyHeld;

    public event EventHandler? TemporaryModeActivated;
    public event EventHandler? TemporaryModeDeactivated;
    public event EventHandler? ColorCycleRequested;

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
            if (vkCode == Win32.VkLMenu || vkCode == Win32.VkRMenu)
            {
                var isKeyDown = wParam == (IntPtr)Win32.WmKeydown || wParam == (IntPtr)Win32.WmSyskeydown;
                var isKeyUp = wParam == (IntPtr)Win32.WmKeyup || wParam == (IntPtr)Win32.WmSyskeyup;
                AppLog.Info($"KBHook: Alt vk=0x{vkCode:X2} down={isKeyDown} up={isKeyUp} held={_isAltHeld} wait={_waitingForSecondPress}");
                if (isKeyDown) HandleAltPress();
                else if (isKeyUp) HandleAltRelease();
            }
            else if (vkCode == Win32.VkS)
            {
                var isKeyDown = wParam == (IntPtr)Win32.WmKeydown || wParam == (IntPtr)Win32.WmSyskeydown;
                var isKeyUp = wParam == (IntPtr)Win32.WmKeyup || wParam == (IntPtr)Win32.WmSyskeyup;

                if (isKeyDown && _isAltHeld && !_isSKeyHeld)
                {
                    _isSKeyHeld = true;
                    ColorCycleRequested?.Invoke(this, EventArgs.Empty);
                }
                else if (isKeyUp)
                {
                    _isSKeyHeld = false;
                }
            }
        }
        return Win32.CallNextHookEx(_hookId, nCode, wParam, lParam);
    }

    private void HandleAltPress()
    {
        var now = DateTime.UtcNow;
        if (_isAltHeld)
        {
            AppLog.Info("KBHook: Alt press ignored (already held).");
            return;
        }
        if (_waitingForSecondPress)
        {
            var timeSinceRelease = (now - _lastAltReleaseTime).TotalMilliseconds;
            AppLog.Info($"KBHook: 2nd press. Time={timeSinceRelease:F1}ms Thresh={_doubleClickThreshold}ms");
            if (timeSinceRelease <= _doubleClickThreshold)
            {
                _isAltHeld = true;
                _waitingForSecondPress = false;
                TemporaryModeActivated?.Invoke(this, EventArgs.Empty);
                AppLog.Info("KBHook: TempMode ACTIVATED.");
            }
            else
            {
                _waitingForSecondPress = false;
                _lastAltPressTime = now;
                AppLog.Info("KBHook: Too slow, reset.");
            }
        }
        else
        {
            _lastAltPressTime = now;
            AppLog.Info("KBHook: First Alt press.");
        }
    }

    private void HandleAltRelease()
    {
        var now = DateTime.UtcNow;
        if (_isAltHeld)
        {
            _isAltHeld = false;
            _waitingForSecondPress = false;
            _isSKeyHeld = false;
            TemporaryModeDeactivated?.Invoke(this, EventArgs.Empty);
            AppLog.Info("KBHook: TempMode DEACTIVATED.");
        }
        else if (!_waitingForSecondPress)
        {
            _lastAltReleaseTime = now;
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
