using VisualPlayer.VisualizationBase.Localization;

namespace VisualPlayer.UI.Localization;

/// <summary>Carries the catalog into popup item templates, outside the editor's visual tree.</summary>
public sealed record LocalizedEnumValue<T>(T Value, ITranslationScope Translations) where T : struct, Enum;
