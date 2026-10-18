using System;
using System.Collections.Generic;
using Avalonia.Controls;
using VisualPlayer.Enums;
using Avalonia;
using VisualPlayer.UI.Localization;
using VisualPlayer.VisualizationBase.Localization;
using VisualPlayer.VisualizationBase.Plugins;
using System.Linq;

namespace VisualPlayer.UI.Views;

public partial class VisualisationColorEditor : UserControl
{
    public static readonly StyledProperty<VisualizationEditorContext?> EditorContextProperty
        = AvaloniaProperty.Register<VisualisationColorEditor, VisualizationEditorContext?>(nameof(EditorContext));
    public static readonly DirectProperty<VisualisationColorEditor, ITranslationScope> TranslationsProperty
        = AvaloniaProperty.RegisterDirect<VisualisationColorEditor, ITranslationScope>(nameof(Translations), view => view.Translations);
    public static readonly DirectProperty<VisualisationColorEditor, IReadOnlyList<LocalizedEnumValue<VisualizationColorMode>>> ColorModeOptionsProperty
        = AvaloniaProperty.RegisterDirect<VisualisationColorEditor, IReadOnlyList<LocalizedEnumValue<VisualizationColorMode>>>(nameof(ColorModeOptions), view => view.ColorModeOptions);
    public static readonly DirectProperty<VisualisationColorEditor, IReadOnlyList<LocalizedEnumValue<GradientType>>> GradientTypeOptionsProperty
        = AvaloniaProperty.RegisterDirect<VisualisationColorEditor, IReadOnlyList<LocalizedEnumValue<GradientType>>>(nameof(GradientTypeOptions), view => view.GradientTypeOptions);
    public IReadOnlyList<LocalizedEnumValue<VisualizationColorMode>> ColorModeOptions
        => ColorModes.Select(value => new LocalizedEnumValue<VisualizationColorMode>(value, Translations)).ToArray();
    public IReadOnlyList<LocalizedEnumValue<GradientType>> GradientTypeOptions
        => GradientTypes.Select(value => new LocalizedEnumValue<GradientType>(value, Translations)).ToArray();
    public VisualizationEditorContext? EditorContext
    {
        get => GetValue(EditorContextProperty);
        set => SetValue(EditorContextProperty, value);
    }
    public ITranslationScope Translations => EditorContext?.Services.CommonTranslations ?? UiCatalogs.English;

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == EditorContextProperty)
        {
            var previous = (change.OldValue as VisualizationEditorContext)?.Services.CommonTranslations ?? UiCatalogs.English;
            RaisePropertyChanged(TranslationsProperty, previous, Translations);
            RaisePropertyChanged(ColorModeOptionsProperty, null!, ColorModeOptions);
            RaisePropertyChanged(GradientTypeOptionsProperty, null!, GradientTypeOptions);
        }
    }
    public IReadOnlyList<VisualizationColorMode> ColorModes { get; }
        = Enum.GetValues<VisualizationColorMode>();

    public IReadOnlyList<GradientType> GradientTypes { get; }
        = Enum.GetValues<GradientType>();


    public VisualisationColorEditor()
    {
        InitializeComponent();
    }
}
