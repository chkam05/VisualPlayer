using System;
using System.ComponentModel;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using VisualPlayer.Enums.Config;
using VisualPlayer.UI.Localization;
using VisualPlayer.ViewModels.Config;
using VisualPlayer.VisualizationBase.Localization;

namespace VisualPlayer.Contexts;

/// <summary>Application-side adapter; the shared UI never references AppearanceConfig.</summary>
public sealed class UiResourceBridge : IDisposable
{
    private readonly ConfigurationContext _config;
    private readonly IResourceDictionary _resources;
    private readonly IDisposable _translations;
    private readonly string[] _keys;
    private AppearanceConfig _appearance;

    public UiResourceBridge(ConfigurationContext config, IResourceDictionary resources, ITranslationScope translations)
    {
        _config = config;
        _resources = resources;
        _appearance = config.Appearance;
        var defaults = (ResourceDictionary)AvaloniaXamlLoader.Load(new Uri("avares://VisualPlayer.UI/Themes/Defaults.axaml"));
        _keys = defaults.Keys.OfType<string>().Where(key => key.StartsWith("UI.") && !key.StartsWith("UI.Text.")).ToArray();
        _translations = UiResources.BindTranslations(resources, translations);
        _config.PropertyChanged += ConfigurationChanged;
        _appearance.PropertyChanged += AppearanceChanged;
        Publish();
    }

    private void ConfigurationChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName != nameof(ConfigurationContext.Appearance)) return;
        _appearance.PropertyChanged -= AppearanceChanged;
        _appearance = _config.Appearance;
        _appearance.PropertyChanged += AppearanceChanged;
        Publish();
    }

    private void AppearanceChanged(object? sender, PropertyChangedEventArgs e) => Publish();

    private void Publish()
    {
        foreach (var key in _keys)
        {
            if (key == "UI.ControlSurfaceBrush")
            {
                if (_appearance.ControlBackgroundBrush is SolidColorBrush brush)
                    _resources[key] = new SolidColorBrush(brush.Color, brush.Opacity * _appearance.InterfaceOpacity);
                continue;
            }
            var parts = key.Split('.');
            var property = parts[1] == "Colorized"
                ? parts[_appearance.ColorizationLevel == ColorizationLevel.Poorly ? 2 : 3]
                : parts[1];
            if (typeof(AppearanceConfig).GetProperty(property)?.GetValue(_appearance) is { } value)
                _resources[key] = value;
        }
    }

    public void Dispose()
    {
        _config.PropertyChanged -= ConfigurationChanged;
        _appearance.PropertyChanged -= AppearanceChanged;
        _translations.Dispose();
    }
}
