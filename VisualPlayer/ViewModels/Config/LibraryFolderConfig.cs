using VisualPlayer.Attributes;

namespace VisualPlayer.ViewModels.Config;

public class LibraryFolderConfig : ViewModelBase
{
    private bool _includeSubdirectories = true;
    private string _path = string.Empty;

    #region PROPERTIES

    [NotifyPropertyUpdate]
    public bool IncludeSubdirectories
    {
        get => _includeSubdirectories;
        set => UpdateProperty(ref _includeSubdirectories, value);
    }

    [NotifyPropertyUpdate]
    public string Path
    {
        get => _path;
        set => UpdateProperty(ref _path, value ?? string.Empty);
    }

    public string Name
    {
        get => System.IO.Path.GetFileName(Path.TrimEnd(
            System.IO.Path.DirectorySeparatorChar,
            System.IO.Path.AltDirectorySeparatorChar)) is { Length: > 0 } name
                ? name
                : Path;
    }

    #endregion PROPERTIES
}
