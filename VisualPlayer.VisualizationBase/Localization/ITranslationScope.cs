using System.ComponentModel;

namespace VisualPlayer.VisualizationBase.Localization;

/// <summary>A single catalog; keys never implicitly resolve in another plugin's catalog.</summary>
public interface ITranslationScope : INotifyPropertyChanged
{
    event EventHandler? LanguageChanged;
    string CurrentLanguageCode { get; }
    string this[string key] { get; }
    string Format(string key, params object?[] arguments);
}
