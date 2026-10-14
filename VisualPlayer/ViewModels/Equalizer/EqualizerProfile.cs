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

    public static double NormalizeGain(double value)
    {
        if (!double.IsFinite(value))
            return 0;

        return Math.Clamp(value, GAIN_MINIMUM, GAIN_MAXIMUM);
    }

    #endregion METHODS
}
