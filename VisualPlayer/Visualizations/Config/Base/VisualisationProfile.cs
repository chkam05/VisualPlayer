using System.Text.Json;
using System.Text.Json.Serialization;
using VisualPlayer.Attributes;
using VisualPlayer.Enums;
using VisualPlayer.Interfaces;
using VisualPlayer.ViewModels;
using VisualPlayer.Visualizations.Utils;

namespace VisualPlayer.Visualizations.Config.Base;

[JsonConverter(typeof(VisualisationProfileJsonConverter))]
public class VisualisationProfile : ViewModelBase
{
    private VisualizationColorConfig? _color;
    private IVisualizationConfig? _config;
    private bool _fillScreen;
    private bool _isDefault;
    private string _name = "Peaks";
    private VisualizationType _visualizationType = VisualizationType.Peaks;

    #region PROPERTIES

    [NotifyPropertyUpdate]
    public bool FillScreen
    {
        get => _fillScreen;
        set => UpdateProperty(ref _fillScreen, value);
    }

    [NotifyPropertyUpdate]
    public VisualizationColorConfig? Color
    {
        get => _color;
        set => UpdateProperty(ref _color, value);
    }

    [NotifyPropertyUpdate]
    public IVisualizationConfig? Config
    {
        get => _config;
        set => UpdateProperty(ref _config, value);
    }

    [NotifyPropertyUpdate]
    public bool IsDefault
    {
        get => _isDefault;
        set
        {
            UpdateProperty(ref _isDefault, value);
            NotifyPropertyChanged(nameof(CanChangeVisualizationType));
            NotifyPropertyChanged(nameof(CanDelete));
        }
    }

    [NotifyPropertyUpdate]
    public string Name
    {
        get => _name;
        set => UpdateProperty(ref _name, string.IsNullOrWhiteSpace(value) ? "Profil wizualizacji" : value.Trim());
    }

    [NotifyPropertyUpdate]
    public VisualizationType VisualizationType
    {
        get => _visualizationType;
        set
        {
            UpdateProperty(ref _visualizationType, value);
            NotifyPropertyChanged(nameof(SupportsFloatingBarGravity));
        }
    }


    [JsonIgnore]
    public bool CanChangeVisualizationType
    {
        get => !IsDefault;
    }

    [JsonIgnore]
    public bool CanDelete
    {
        get => !IsDefault;
    }

    [JsonIgnore]
    public bool SupportsFloatingBarGravity
    {
        get => VisualizationType is VisualizationType.Peaks
            or VisualizationType.Stripes
            or VisualizationType.PeaksMirrored
            or VisualizationType.StripesMirrored;
    }

    #endregion PROPERTIES

    #region METHODS

    public VisualisationProfile Copy(string name)
    {
        return new VisualisationProfile
        {
            IsDefault = false,
            Name = name,
            VisualizationType = VisualizationType,
            FillScreen = FillScreen,
            Color = Color?.Copy(),
            Config = CopyConfig(Config)
        };
    }

    public T GetConfig<T>() where T : class, IVisualizationConfig, new()
    {
        if (Config is T config)
            return config;

        config = new T();
        Config = config;
        return config;
    }

    public VisualizationColorConfig GetColorConfig()
    {
        if (Color is not null)
            return Color;

        Color = new VisualizationColorConfig();
        return Color;
    }

    public void OverwriteFrom(VisualisationProfile source)
    {
        VisualizationType = source.VisualizationType;
        FillScreen = source.FillScreen;
        Color = source.Color?.Copy();
        Config = CopyConfig(source.Config);
    }


    private static IVisualizationConfig? CopyConfig(IVisualizationConfig? config)
    {
        if (config is null)
            return null;

        var configType = config.GetType();
        var json = JsonSerializer.Serialize(config, configType);
        return JsonSerializer.Deserialize(json, configType) as IVisualizationConfig;
    }

    #endregion METHODS
}
