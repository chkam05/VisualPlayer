using VisualPlayer.Attributes;
using VisualPlayer.ViewModels;

namespace VisualPlayer.ViewModels.Equalizer;

public class EqualizerBand : ViewModelBase
{
    private double _frequency;
    private double _gain;
    private string _title = string.Empty;


    #region PROPERTIES

    [NotifyPropertyUpdate]
    public double Frequency
    {
        get => _frequency;
        set => UpdateProperty(ref _frequency, value);
    }

    [NotifyPropertyUpdate]
    public double Gain
    {
        get => _gain;
        set => UpdateProperty(ref _gain, EqualizerProfile.NormalizeGain(value));
    }

    [NotifyPropertyUpdate]
    public string Title
    {
        get => _title;
        set => UpdateProperty(ref _title, value ?? string.Empty);
    }

    #endregion PROPERTIES

    #region METHODS

    public EqualizerBand Copy()
    {
        return new EqualizerBand
        {
            Frequency = Frequency,
            Gain = Gain,
            Title = Title
        };
    }

    #endregion METHODS
}
