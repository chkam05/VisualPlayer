using System;
using Avalonia;

namespace VisualPlayer.Controls;

public class SettingsExpandControl : SettingsControl
{
    #region AVALONIA PROPERTIES

    public static readonly StyledProperty<bool> IsExpandedProperty
        = AvaloniaProperty.Register<SettingsExpandControl, bool>(nameof(IsExpanded));

    #endregion AVALONIA PROPERTIES

    #region PROPERTIES

    public bool IsExpanded
    {
        get => GetValue(IsExpandedProperty);
        set => SetValue(IsExpandedProperty, value);
    }

    protected override Type StyleKeyOverride => typeof(SettingsExpandControl);

    #endregion PROPERTIES
}