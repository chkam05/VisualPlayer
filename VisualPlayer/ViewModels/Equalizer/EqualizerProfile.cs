using System;
using System.Collections.ObjectModel;
using System.Text.Json.Serialization;
using VisualPlayer.Attributes;
using VisualPlayer.ViewModels;

namespace VisualPlayer.ViewModels.Equalizer;

public class EqualizerProfile : ViewModelBase
{
    #region CONST

    public const double GAIN_MAXIMUM = 10;
    public const double GAIN_MINIMUM = -10;

    #endregion CONST

    private ObservableCollection<EqualizerBand> _bands = [];
    private bool _isDefault;
    private string _name = "Equalizer";
    private double _preamp;
    private Func<string, string>? _translate;
    private string? _translationKey;

    #region PROPERTIES

    [NotifyPropertyUpdate]
    public ObservableCollection<EqualizerBand> Bands
    {
        get => _bands;
        set => UpdateProperty(ref _bands, value ?? []);
    }

    [NotifyPropertyUpdate]
    public bool IsDefault
    {
        get => _isDefault;
        set
        {
            UpdateProperty(ref _isDefault, value);
            NotifyPropertyChanged(nameof(CanDelete));
            NotifyPropertyChanged(nameof(CanEdit));
        }
    }

    [NotifyPropertyUpdate]
    public string Name
    {
        get => _name;
        set => UpdateProperty(ref _name, string.IsNullOrWhiteSpace(value) ? "Equalizer" : value.Trim());
    }

    [JsonIgnore]
    public string DisplayName => _translationKey is null || _translate is null
        ? Name
        : _translate(_translationKey);

    [NotifyPropertyUpdate]
    public double Preamp
    {
        get => _preamp;
        set => UpdateProperty(ref _preamp, NormalizeGain(value));
    }


    [JsonIgnore]
    public bool CanDelete
    {
        get => !IsDefault;
    }

    [JsonIgnore]
    public bool CanEdit
    {
        get => !IsDefault;
    }

    #endregion PROPERTIES

    #region METHODS

    public EqualizerProfile Copy(string name)
    {
        var copy = new EqualizerProfile
        {
            IsDefault = false,
            Name = name,
            Preamp = Preamp
        };

        foreach (var band in Bands)
            copy.Bands.Add(band.Copy());

        return copy;
    }

    internal void ConfigureTranslation(string? translationKey, Func<string, string>? translate)
    {
        _translationKey = translationKey;
        _translate = translate;
        NotifyPropertyChanged(nameof(DisplayName));
    }

    internal void RefreshTranslation()
    {
        NotifyPropertyChanged(nameof(DisplayName));
    }

    public static double NormalizeGain(double value)
    {
        if (!double.IsFinite(value))
            return 0;

        return Math.Clamp(value, GAIN_MINIMUM, GAIN_MAXIMUM);
    }

    #endregion METHODS
}
