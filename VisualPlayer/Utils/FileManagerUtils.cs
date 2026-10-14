using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace VisualPlayer.Utils;

public static class FileManagerUtils
{
    public static async Task<bool> RevealFileAsync(string filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath) || !File.Exists(filePath))
            return false;

        try
        {
            using var process = CreateRevealProcess(Path.GetFullPath(filePath));

            if (process is null || !process.Start())
                return await OpenLinuxDirectoryFallbackAsync(filePath);

            await process.WaitForExitAsync();

            if (process.ExitCode == 0)
                return true;

            return await OpenLinuxDirectoryFallbackAsync(filePath);
        }
        catch
        {
            return await OpenLinuxDirectoryFallbackAsync(filePath);
        }
    }

    private static async Task<bool> OpenLinuxDirectoryFallbackAsync(string filePath)
    {
        if (!OperatingSystem.IsLinux())
            return false;

        try
        {
            using var process = CreateProcess("xdg-open", Path.GetDirectoryName(filePath) ?? filePath);

            if (!process.Start())
                return false;

            await process.WaitForExitAsync();
            return process.ExitCode == 0;
        }
        catch
        {
            return false;
        }
    }

    private static Process? CreateRevealProcess(string filePath)
    {
        if (OperatingSystem.IsMacOS())
            return CreateProcess("open", "-R", filePath);

        if (OperatingSystem.IsWindows())
            return CreateProcess("explorer.exe", "/select,", filePath);

        if (OperatingSystem.IsLinux())
        {
            return CreateProcess(
                "dbus-send",
                "--session",
                "--dest=org.freedesktop.FileManager1",
                "--type=method_call",
                "/org/freedesktop/FileManager1",
                "org.freedesktop.FileManager1.ShowItems",
                $"array:string:{new Uri(filePath).AbsoluteUri}",
                "string:");
        }

        return null;
    }

    private static Process CreateProcess(string executable, params string[] arguments)
    {
        var startInfo = new ProcessStartInfo
        {
            FileName = executable,
            UseShellExecute = false
        };

        foreach (var argument in arguments)
            startInfo.ArgumentList.Add(argument);

        return new Process
        {
            StartInfo = startInfo
        };
    }
}
