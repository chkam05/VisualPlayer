using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Media;
using Lucide.Avalonia;
using VisualPlayer.Enums.UI;

namespace VisualPlayer.Components;

[PseudoClasses(":has-content")]
public class ButtonWithIcon : Button
{
    #region AVALONIA PROPERTIES

    public static readonly StyledProperty<double> CalculatedIconSizeProperty 
        = AvaloniaProperty.Register<ButtonWithIcon, double>(nameof(CalculatedIconSize), 24);

    public static readonly StyledProperty<ContentAlignmentMode> ContentAlignmentModeProperty
        = AvaloniaProperty.Register<ButtonWithIcon, ContentAlignmentMode>(nameof(ContentAlignmentMode), ContentAlignmentMode.Auto);

    public static readonly StyledProperty<LucideIconKind> KindProperty 
        = AvaloniaProperty.Register<ButtonWithIcon, LucideIconKind>(nameof(Kind));

    public static readonly StyledProperty<double> GapProperty
        = AvaloniaProperty.Register<ButtonWithIcon, double>(nameof(Gap), 6);

    public static readonly StyledProperty<IconPosition> IconPositionProperty 
        = AvaloniaProperty.Register<ButtonWithIcon, IconPosition>(nameof(IconPosition), IconPosition.Left);

    public static readonly StyledProperty<double> IconSizeProperty
        = AvaloniaProperty.Register<ButtonWithIcon, double>(nameof(IconSize), 24);

    #endregion AVALONIA PROPERTIES

    #region PROPERTIES

    public double CalculatedIconSize
    {
        get => GetValue(CalculatedIconSizeProperty);
        private set => SetValue(CalculatedIconSizeProperty, value);
    }

    public ContentAlignmentMode ContentAlignmentMode
    {
        get => GetValue(ContentAlignmentModeProperty);
        set => SetValue(ContentAlignmentModeProperty, value);
    }

    public LucideIconKind Kind
    {
        get => GetValue(KindProperty);
        set => SetValue(KindProperty, value);
    }

    public double Gap
    {
        get => GetValue(GapProperty);
        set => SetValue(GapProperty, value);
    }

    public IconPosition IconPosition
    {
        get => GetValue(IconPositionProperty);
        set => SetValue(IconPositionProperty, value);
    }

    public double IconSize
    {
        get => GetValue(IconSizeProperty);
        set => SetValue(IconSizeProperty, Math.Max(0, value));
    }

    #endregion PROPERTIES

    #region CONSTRUCTORS

    static ButtonWithIcon()
    {
        ForegroundProperty.OverrideDefaultValue<ButtonWithIcon>(Brushes.White);
    }

    #endregion CONSTRUCTORS

    #region LAYOUT

    protected override Size ArrangeOverride(Size finalSize)
    {
        var result = base.ArrangeOverride(finalSize);

        var width = Math.Max(
            0,
            finalSize.Width
            - Padding.Left
            - Padding.Right
            - BorderThickness.Left
            - BorderThickness.Right);
        var height = Math.Max(
            0,
            finalSize.Height
            - Padding.Top
            - Padding.Bottom
            - BorderThickness.Top
            - BorderThickness.Bottom);

        var iconSize = IconPosition is IconPosition.Left or IconPosition.Right
            ? Math.Min(IconSize, height)
            : Math.Min(IconSize, width);

        if (Content is null)
            iconSize = Math.Min(iconSize, Math.Min(width, height));

        if (Math.Abs(CalculatedIconSize - iconSize) > 0.1)
            CalculatedIconSize = iconSize;

        return result;
    }

    #endregion LAYOUT

    #region PROPERTY CHANGE HANDLING

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == ContentProperty)
            PseudoClasses.Set(":has-content", Content is not null);
    }

    #endregion PROPERTY CHANGE HANDLING

}
