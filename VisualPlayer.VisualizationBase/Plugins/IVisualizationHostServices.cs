using VisualPlayer.VisualizationBase.Localization;

namespace VisualPlayer.VisualizationBase.Plugins;

public enum VisualizationDiagnosticLevel
{
    Information,
    Warning,
    Error
}

/// <summary>Services scoped to this plugin; no player, window or filesystem singleton.</summary>
public interface IVisualizationHostServices
{
    ITranslationScope Translations { get; }
    ITranslationScope CommonTranslations { get; }
    void ReportDiagnostic(VisualizationDiagnosticLevel level, string message, Exception? exception = null);
}
