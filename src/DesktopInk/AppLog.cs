using System;
using System.IO;
using System.Text;

namespace DesktopInk;

internal static class AppLog
{
    private static readonly object LockObj = new();

    internal static string LogPath
    {
        get
        {
            var root = Environment.CurrentDirectory;
            return Path.Combine(root, ".tmp", "desktopink", "desktopink.log");
        }
    }

    internal static void Info(string message)
    {
        Write("INFO", message);
    }

    internal static void Error(string message, Exception? ex = null)
    {
        var full = ex is null ? message : message + "\n" + ex;
        Write("ERROR", full);
    }

    private static void Write(string level, string message)
    {
        try
        {
            var path = LogPath;
            Directory.CreateDirectory(Path.GetDirectoryName(path)!);

            var line = $"[{DateTimeOffset.Now:yyyy-MM-dd HH:mm:ss.fff zzz}] {level} {message}";

            lock (LockObj)
            {
                File.AppendAllText(path, line + Environment.NewLine, Encoding.UTF8);
            }
        }
        catch
        {
            // Never crash due to logging.
        }
    }
}
