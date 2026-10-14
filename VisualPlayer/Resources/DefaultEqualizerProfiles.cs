using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using VisualPlayer.ViewModels.Equalizer;

namespace VisualPlayer.Resources;

public static class DefaultEqualizerProfiles
{
    #region CONST

    public const string BASS_NAME = "Wzmocnienie basu";
    public const string CLASSICAL_NAME = "Klasyczna";
    public const string FLAT_NAME = "Płaski";
    public const string POP_NAME = "Pop";
    public const string ROCK_NAME = "Rock";
    public const string TREBLE_NAME = "Wzmocnienie wysokich tonów";

    private static readonly (double Frequency, string Title)[] Bands =
    [
        (60, "60 Hz"),
        (100, "100 Hz"),
        (200, "200 Hz"),
        (500, "500 Hz"),
        (1000, "1 kHz"),
        (1800, "1.8 kHz"),
        (3400, "3.4 kHz"),
        (5700, "5.7 kHz"),
        (9000, "9 kHz"),
        (13000, "13 kHz")
    ];

    #endregion CONST

    #region CREATE

    public static IReadOnlyList<EqualizerProfile> Create(Func<string, string>? translate = null)
    {
        return
        [
            CreateProfile(FLAT_NAME, "EQUALIZER_PRESET_FLAT", translate, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0),
            CreateProfile(CLASSICAL_NAME, "EQUALIZER_PRESET_CLASSICAL", translate, -1, 4, 3, 2, 0, -1, -1, 0, 2, 3, 4),
            CreateProfile(POP_NAME, "EQUALIZER_PRESET_POP", translate, -1, -1, 1, 3, 4, 2, 0, -1, -1, 1, 2),
            CreateProfile(ROCK_NAME, "EQUALIZER_PRESET_ROCK", translate, -2, 5, 4, 2, -1, -2, 0, 2, 4, 5, 5),
            CreateProfile(BASS_NAME, "EQUALIZER_PRESET_BASS", translate, -1, 6, 5, 3, 1, 0, 0, 0, 0, 0, 0),
            CreateProfile(TREBLE_NAME, "EQUALIZER_PRESET_TREBLE", translate, -1, 0, 0, 0, 0, 0, 1, 3, 5, 6, 6),
        ];
    }

    public static EqualizerProfile Create(string name)
    {
        return Create().FirstOrDefault(profile =>
                string.Equals(profile.Name, name, StringComparison.OrdinalIgnoreCase))
            ?? Create().First();
    }

    private static EqualizerProfile CreateProfile(
        string name,
        string translationKey,
        Func<string, string>? translate,
        double preamp,
        params double[] gains)
    {
        var profile = new EqualizerProfile
        {
            IsDefault = true,
            Name = name,
            Preamp = preamp,
            Bands = new ObservableCollection<EqualizerBand>()
        };
        profile.ConfigureTranslation(translationKey, translate);

        for (var index = 0; index < Bands.Length; index++)
        {
            profile.Bands.Add(new EqualizerBand
            {
                Frequency = Bands[index].Frequency,
                Gain = index < gains.Length ? gains[index] : 0,
                Title = Bands[index].Title
            });
        }

        return profile;
    }

    #endregion CREATE

    #region VALIDATION

    public static bool IsDefaultName(string name, Func<string, string>? translate = null)
    {
        return Create(translate).Any(profile =>
            string.Equals(profile.Name, name, StringComparison.OrdinalIgnoreCase)
            || string.Equals(profile.DisplayName, name, StringComparison.CurrentCultureIgnoreCase));
    }

    public static bool IsCompatible(EqualizerProfile profile)
    {
        var expected = Create(profile.Name);

        return profile.Bands.Count == expected.Bands.Count
            && profile.Bands.Select(band => band.Frequency)
                .SequenceEqual(expected.Bands.Select(band => band.Frequency));
    }

    #endregion VALIDATION
}
