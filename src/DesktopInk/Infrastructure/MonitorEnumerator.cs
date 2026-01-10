using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace DesktopInk.Infrastructure;

internal static class MonitorEnumerator
{
    internal sealed record MonitorInfo(IntPtr HMonitor, Win32.Rect BoundsPx, uint DpiX, uint DpiY);

    internal static IReadOnlyList<MonitorInfo> GetMonitors()
    {
        var monitors = new List<MonitorInfo>();

        _ = Win32.EnumDisplayMonitors(
            IntPtr.Zero,
            IntPtr.Zero,
            (hMonitor, _, _, _) =>
            {
                if (!Win32.TryGetMonitorBounds(hMonitor, out var bounds))
                {
                    return true;
                }

                Win32.TryGetMonitorDpi(hMonitor, out var dpiX, out var dpiY);

                monitors.Add(new MonitorInfo(hMonitor, bounds, dpiX, dpiY));
                return true;
            },
            IntPtr.Zero);

        return monitors;
    }
}
