using Avalonia;
using System;
using System.Linq;
using VisualPlayer.Utils;

namespace VisualPlayer;

class Program
{
    internal static SingleInstanceService? SingleInstance { get; private set; }

    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    [STAThread]
    public static void Main(string[] args)
    {
        // Resolve paths in the sending process, whose working directory may differ.
        args = args.Select(path => AudioFileUtils.NormalizePath(path) ?? path).ToArray();
        using var singleInstance = new SingleInstanceService();
        if (singleInstance.ForwardToExistingInstance(args))
            return;

        SingleInstance = singleInstance;
        BuildAvaloniaApp()
            .StartWithClassicDesktopLifetime(args);
    }

    // Avalonia configuration, don't remove; also used by visual designer.
    public static AppBuilder BuildAvaloniaApp()
    {
        return AppBuilder.Configure<App>()
            .UsePlatformDetect()
#if DEBUG
            .WithDeveloperTools()
#endif
            .WithInterFont()
            .LogToTrace();
    }
}
