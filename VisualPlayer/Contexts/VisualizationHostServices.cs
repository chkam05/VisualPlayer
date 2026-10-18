using System;
using System.Diagnostics;
using VisualPlayer.VisualizationBase.Localization;
using VisualPlayer.VisualizationBase.Plugins;

namespace VisualPlayer.Contexts;

public sealed class VisualizationHostServices(ITranslationScope translations, ITranslationScope commonTranslations)
    : IVisualizationHostServices
{
    public ITranslationScope Translations { get; } = translations;
    public ITranslationScope CommonTranslations { get; } = commonTranslations;
    public void ReportDiagnostic(VisualizationDiagnosticLevel level, string message, Exception? exception = null)
        => Debug.WriteLine($"Visualization [{level}] {message} {exception}");
}
