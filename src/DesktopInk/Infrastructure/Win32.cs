using System;
using System.Runtime.InteropServices;

namespace DesktopInk.Infrastructure;

internal static class Win32
{
    internal static readonly IntPtr HwndTopmost = new(-1);

    internal const int GwlExStyle = -20;

    internal const int WsExTransparent = 0x00000020;
    internal const int WsExLayered = 0x00080000;
    internal const int WsExToolWindow = 0x00000080;

    internal const int WmHotkey = 0x0312;
    internal const int WmMouseActivate = 0x0021;
    internal const int WmDpichanged = 0x02E0;

    internal const int MaNoActivate = 3;

    // Keyboard hook constants
    internal const int WhKeyboardLl = 13;
    internal const int WmKeydown = 0x0100;
    internal const int WmKeyup = 0x0101;
    internal const int WmSyskeydown = 0x0104;
    internal const int WmSyskeyup = 0x0105;
    internal const int VkShift = 0x10;
    internal const int VkLShift = 0xA0;
    internal const int VkRShift = 0xA1;
    internal const int VkLMenu = 0xA4;
    internal const int VkRMenu = 0xA5;

    internal const uint ModAlt = 0x0001;
    internal const uint ModControl = 0x0002;
    internal const uint ModShift = 0x0004;
    internal const uint ModWin = 0x0008;

    internal const uint SwpNoSize = 0x0001;
    internal const uint SwpNoMove = 0x0002;
    internal const uint SwpNoZOrder = 0x0004;
    internal const uint SwpNoActivate = 0x0010;
    internal const uint SwpFrameChanged = 0x0020;

    [DllImport("user32.dll", SetLastError = true)]
    internal static extern bool SetWindowPos(
        IntPtr hWnd,
        IntPtr hWndInsertAfter,
        int x,
        int y,
        int cx,
        int cy,
        uint uFlags);

    [DllImport("user32.dll", SetLastError = true)]
    internal static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

    [DllImport("user32.dll", SetLastError = true)]
    internal static extern bool UnregisterHotKey(IntPtr hWnd, int id);

    [DllImport("user32.dll", SetLastError = true)]
    internal static extern bool GetWindowRect(IntPtr hWnd, out Rect lpRect);

    [DllImport("user32.dll")]
    internal static extern uint GetDpiForWindow(IntPtr hWnd);

    // Keyboard hook types and functions
    internal delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

    [DllImport("user32.dll", SetLastError = true)]
    internal static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

    [DllImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool UnhookWindowsHookEx(IntPtr hhk);

    [DllImport("user32.dll")]
    internal static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

    [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    internal static extern IntPtr GetModuleHandle(string? lpModuleName);

    [DllImport("user32.dll")]
    internal static extern int GetDoubleClickTime();

    [StructLayout(LayoutKind.Sequential)]
    internal struct KbdllHookStruct
    {
        public int VkCode;
        public int ScanCode;
        public int Flags;
        public int Time;
        public IntPtr DwExtraInfo;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct Rect
    {
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;

        public int Width => Right - Left;
        public int Height => Bottom - Top;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct MonitorInfo
    {
        public int CbSize;
        public Rect RcMonitor;
        public Rect RcWork;
        public uint DwFlags;
    }

    internal delegate bool MonitorEnumProc(IntPtr hMonitor, IntPtr hdcMonitor, IntPtr lprcMonitor, IntPtr dwData);

    [DllImport("user32.dll", SetLastError = true)]
    internal static extern bool EnumDisplayMonitors(IntPtr hdc, IntPtr lprcClip, MonitorEnumProc lpfnEnum, IntPtr dwData);

    [DllImport("user32.dll", SetLastError = true)]
    internal static extern bool GetMonitorInfo(IntPtr hMonitor, ref MonitorInfo lpmi);

    internal enum MonitorDpiType
    {
        EffectiveDpi = 0,
        AngularDpi = 1,
        RawDpi = 2,
        Default = EffectiveDpi,
    }

    [DllImport("shcore.dll")]
    private static extern int GetDpiForMonitor(IntPtr hmonitor, MonitorDpiType dpiType, out uint dpiX, out uint dpiY);

    internal static bool TryGetMonitorBounds(IntPtr hMonitor, out Rect bounds)
    {
        var info = new MonitorInfo { CbSize = Marshal.SizeOf<MonitorInfo>() };
        if (!GetMonitorInfo(hMonitor, ref info))
        {
            bounds = default;
            return false;
        }

        bounds = info.RcMonitor;
        return true;
    }

    internal static bool TryGetMonitorDpi(IntPtr hMonitor, out uint dpiX, out uint dpiY)
    {
        // shcore GetDpiForMonitor returns HRESULT.
        var hr = GetDpiForMonitor(hMonitor, MonitorDpiType.EffectiveDpi, out dpiX, out dpiY);
        if (hr == 0 && dpiX != 0 && dpiY != 0)
        {
            return true;
        }

        dpiX = 96;
        dpiY = 96;
        return false;
    }

    internal static IntPtr GetWindowLongPtr(IntPtr hWnd, int nIndex)
    {
        return IntPtr.Size == 8
            ? GetWindowLongPtr64(hWnd, nIndex)
            : new IntPtr(GetWindowLong32(hWnd, nIndex));
    }

    internal static IntPtr SetWindowLongPtr(IntPtr hWnd, int nIndex, IntPtr dwNewLong)
    {
        return IntPtr.Size == 8
            ? SetWindowLongPtr64(hWnd, nIndex, dwNewLong)
            : new IntPtr(SetWindowLong32(hWnd, nIndex, dwNewLong.ToInt32()));
    }

    [DllImport("user32.dll", EntryPoint = "GetWindowLong", SetLastError = true)]
    private static extern int GetWindowLong32(IntPtr hWnd, int nIndex);

    [DllImport("user32.dll", EntryPoint = "SetWindowLong", SetLastError = true)]
    private static extern int SetWindowLong32(IntPtr hWnd, int nIndex, int dwNewLong);

    [DllImport("user32.dll", EntryPoint = "GetWindowLongPtr", SetLastError = true)]
    private static extern IntPtr GetWindowLongPtr64(IntPtr hWnd, int nIndex);

    [DllImport("user32.dll", EntryPoint = "SetWindowLongPtr", SetLastError = true)]
    private static extern IntPtr SetWindowLongPtr64(IntPtr hWnd, int nIndex, IntPtr dwNewLong);
}
