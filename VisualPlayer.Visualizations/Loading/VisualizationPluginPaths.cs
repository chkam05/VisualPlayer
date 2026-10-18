namespace VisualPlayer.Visualizations.Loading;

public enum VisualizationPackageSource { BuiltIn, User }
public sealed record VisualizationPluginRoot(string Path, VisualizationPackageSource Source);

public static class VisualizationPluginPaths
{
    public static IReadOnlyList<VisualizationPluginRoot> ForCurrentHost()
    {
        var builtin = Path.Combine(AppContext.BaseDirectory, "Plugins");
        if (OperatingSystem.IsMacOS() && new DirectoryInfo(AppContext.BaseDirectory).Name == "MacOS")
            builtin = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "Resources", "Plugins"));
        string user;
        if (OperatingSystem.IsWindows())
            user = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "VisualPlayer", "Plugins");
        else if (OperatingSystem.IsMacOS())
            user = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Library", "Application Support", "VisualPlayer", "Plugins");
        else
        {
            var data = Environment.GetEnvironmentVariable("XDG_DATA_HOME");
            if (string.IsNullOrWhiteSpace(data) || !Path.IsPathFullyQualified(data))
                data = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".local", "share");
            user = Path.Combine(data, "VisualPlayer", "Plugins");
        }
        return [new(builtin, VisualizationPackageSource.BuiltIn), new(user, VisualizationPackageSource.User)];
    }
}
