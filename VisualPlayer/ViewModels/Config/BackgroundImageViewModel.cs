using System.IO;
using VisualPlayer.Attributes;

namespace VisualPlayer.ViewModels.Config;

public class BackgroundImageViewModel : ViewModelBase
{
    private bool _isDefault;
    private string _name;
    private string _path;

    #region PROPERTIES

    [NotifyPropertyUpdate]
    public bool CanRemove => !IsDefault;

    [NotifyPropertyUpdate]
    public bool IsDefault
    {
        get => _isDefault;

        set
        {
            UpdateProperty(ref _isDefault, value);
            NotifyPropertyChanged(nameof(CanRemove));
        }
    }

    [NotifyPropertyUpdate]
    public string Name
    {
        get => _name;
        set => UpdateProperty(ref _name, value?.Trim() ?? string.Empty);
    }

    [NotifyPropertyUpdate]
    public string Path
    {
        get => _path;
        set => UpdateProperty(ref _path, value?.Trim() ?? string.Empty);
    }

    #endregion PROPERTIES

    #region CONSTRUCTOR

    public BackgroundImageViewModel(string path, bool isDefault)
    {
        _path = path;
        _isDefault = isDefault;
        _name = GetName(path);
    }

    #endregion CONSTRUCTOR

    private static string GetName(string path)
    {
        var fileName = System.IO.Path.GetFileNameWithoutExtension(path);

        return string.IsNullOrWhiteSpace(fileName)
            ? "Tło"
            : fileName;
    }
}