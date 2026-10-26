using System;
using Avalonia;
using Avalonia.Controls;
using Lucide.Avalonia;

namespace VisualPlayer.Components;

public class SettingsControl : ContentControl
{
    #region AVALONIA PROPERTIES

    public static readonly StyledProperty<string> DescriptionProperty
        = AvaloniaProperty.Register<SettingsControl, string>(nameof(Description), string.Empty);

    public static readonly StyledProperty<LucideIconKind> IconProperty
        = AvaloniaProperty.Register<SettingsControl, LucideIconKind>(nameof(Icon), LucideIconKind.Settings);

    public static readonly StyledProperty<string> TitleProperty
        = AvaloniaProperty.Register<SettingsControl, string>(nameof(Title), string.Empty);

    #endregion AVALONIA PROPERTIES

    #region PROPERTIES

    public string Description
    {
        get => GetValue(DescriptionProperty);
        set => SetValue(DescriptionProperty, value);
    }

    public LucideIconKind Icon
    {
        get => GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }

    protected override Type StyleKeyOverride => typeof(SettingsControl);

    public string Title
    {
        get => GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    #endregion PROPERTIES
}
