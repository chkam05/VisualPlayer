using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Lucide.Avalonia;
using VisualPlayer.Enums.UI;

namespace VisualPlayer.Components;

[PseudoClasses(":has-content")]
public class ToggleButtonWithIcon : ToggleButton
{
    #region AVALONIA PROPERTIES

    public static readonly StyledProperty<double> CalculatedIconSizeProperty
        = AvaloniaProperty.Register<ToggleButtonWithIcon, double>(nameof(CalculatedIconSize), 24);

    public static readonly StyledProperty<LucideIconKind?> CheckedKindProperty
        = AvaloniaProperty.Register<ToggleButtonWithIcon, LucideIconKind?>(nameof(CheckedKind));

    public static readonly StyledProperty<ContentAlignmentMode> ContentAlignmentModeProperty
        = AvaloniaProperty.Register<ToggleButtonWithIcon, ContentAlignmentMode>(nameof(ContentAlignmentMode), ContentAlignmentMode.Auto);

    public static readonly StyledProperty<LucideIconKind> CurrentKindProperty
        = AvaloniaProperty.Register<ToggleButtonWithIcon, LucideIconKind>(nameof(CurrentKind));

    public static readonly StyledProperty<double> GapProperty
        = AvaloniaProperty.Register<ToggleButtonWithIcon, double>(nameof(Gap), 6);

    public static readonly StyledProperty<IconPosition> IconPositionProperty
        = AvaloniaProperty.Register<ToggleButtonWithIcon, IconPosition>(nameof(IconPosition), IconPosition.Left);

    public static readonly StyledProperty<LucideIconKind> KindProperty
        = AvaloniaProperty.Register<ToggleButtonWithIcon, LucideIconKind>(nameof(Kind));

    public static readonly StyledProperty<double> IconSizeProperty
        = AvaloniaProperty.Register<ToggleButtonWithIcon, double>(nameof(IconSize), 24);

    #endregion AVALONIA PROPERTIES
    
    #region PROPERTIES

    public double CalculatedIconSize
    {
        get => GetValue(CalculatedIconSizeProperty);
        private set => SetValue(CalculatedIconSizeProperty, value);
    }

    public LucideIconKind? CheckedKind
    {
        get => GetValue(CheckedKindProperty);
        set => SetValue(CheckedKindProperty, value);
    }

    public ContentAlignmentMode ContentAlignmentMode
    {
        get => GetValue(ContentAlignmentModeProperty);
        set => SetValue(ContentAlignmentModeProperty, value);
    }

    public LucideIconKind CurrentKind
    {
        get => GetValue(CurrentKindProperty);
        private set => SetValue(CurrentKindProperty, value);
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

        if (change.Property == KindProperty ||
            change.Property == CheckedKindProperty ||
            change.Property == IsCheckedProperty)
        {
            UpdateCurrentKind();
        }
    }

    #endregion PROPERTY CHANGE HANDLING

    #region TEMPLATE

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);

        PseudoClasses.Set(":has-content", Content is not null);
        UpdateCurrentKind();
    }

    #endregion TEMPLATE

    #region UTILITIES

    private void UpdateCurrentKind()
    {
        CurrentKind = IsChecked == true && CheckedKind.HasValue ? CheckedKind.Value : Kind;
    }

    #endregion UTILITIES
}
