using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Platform.Storage;
using VisualPlayer.Contexts;
using VisualPlayer.Utils;
using VisualPlayer.Windows;

namespace VisualPlayer;

public partial class App : Application
{
    private ApplicationContext? _applicationContext = null;

    public ApplicationContext ApplicationContext
    {
        get
        {
            if (_applicationContext == null)
                _applicationContext = ApplicationContext.Instance;
            
            return _applicationContext;
        }
    }


    #region INITIALIZATION

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (this.TryGetFeature<IActivatableLifetime>() is { } activatableLifetime)
            activatableLifetime.Activated += ActivatableLifetime_Activated;
        
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            ApplicationContext.InitializeStartupFiles(desktop.Args ?? []);

            desktop.MainWindow = CreateWindow();
            desktop.Exit += Desktop_Exit;
        }

        base.OnFrameworkInitializationCompleted();
    }

    #endregion INITIALIZATION

    #region ACTIONS

    private void ActivatableLifetime_Activated(object? sender, ActivatedEventArgs e)
    {
        if (e is not FileActivatedEventArgs fileEvent)
            return;

        var filePaths = fileEvent.Files
            .OfType<IStorageFile>()
            .Select(file => file.TryGetLocalPath())
            .Where(path => path is not null)
            .Select(path => path!)
            .ToArray();

        if (filePaths.Length == 0)
            return;

        ApplicationContext.AddFiles(filePaths);
    }

    private void Desktop_Exit(object? sender, ControlledApplicationLifetimeExitEventArgs e)
    {
        ApplicationContext.Dispose();
    }

    private void MainWindow_PositionChanged(object? sender, PixelPointEventArgs e)
    {
        if (sender is not MainWindow window)
            return;

        if (window.WindowState != WindowState.Normal)
            return;

        var windowConfig = ApplicationContext.Config.Window;

        windowConfig.PosX = window.Position.X;
        windowConfig.PosY = window.Position.Y;
    }

    private void MainWindow_Resized(object? sender, WindowResizedEventArgs e)
    {
        if (sender is not MainWindow window)
            return;

        if (window.WindowState != WindowState.Normal)
            return;

        var windowConfig = ApplicationContext.Config.Window;

        windowConfig.Width = window.ClientSize.Width;
        windowConfig.Height = window.ClientSize.Height;
    }

    #endregion ACTIONS

    #region WINDOWS

    private MainWindow CreateWindow()
    {
        var mainWindow = new MainWindow();

        var windowConfig = ApplicationContext.Config.Window;
        var placement = DisplayUtils.CorrectWindowPlacement(
            mainWindow,
            windowConfig.PosX,
            windowConfig.PosY,
            windowConfig.Width,
            windowConfig.Height);
        
        mainWindow.WindowStartupLocation = WindowStartupLocation.Manual;
        mainWindow.Width = placement.Width;
        mainWindow.Height = placement.Height;
        mainWindow.Position = placement.Position;

        mainWindow.PositionChanged += MainWindow_PositionChanged;
        mainWindow.Resized += MainWindow_Resized;

        return mainWindow;
    }

    #endregion WINDOWS
}
