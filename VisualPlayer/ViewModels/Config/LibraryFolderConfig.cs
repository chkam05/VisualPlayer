using System.Text.Json.Serialization;
using VisualPlayer.Attributes;

namespace VisualPlayer.ViewModels.Config;

public class LibraryFolderConfig : ViewModelBase
{
    private bool _includeSubdirectories = true;
    private string _path = string.Empty;
    private bool _isScanning;
    private int? _scanPercentage;

    #region PROPERTIES

    [JsonIgnore]
    [NotifyPropertyUpdate]
    public bool IsScanning
    {
        get => _isScanning;
        internal set
        {
            UpdateProperty(ref _isScanning, value);
            NotifyPropertyChanged(nameof(HasScanPercentage));
        }
    }

    // Unknown while discovering files; measurable once metadata loading starts.
    [JsonIgnore]
    [NotifyPropertyUpdate]
    public int? ScanPercentage
    {
        get => _scanPercentage;
        internal set
        {
            UpdateProperty(ref _scanPercentage, value);
            NotifyPropertyChanged(nameof(HasScanPercentage));
        }
    }

    [JsonIgnore]
    public bool HasScanPercentage => IsScanning && ScanPercentage.HasValue;

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
