using System;
using System.Collections.ObjectModel;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Interactivity;
using VisualPlayer.Events;
using VisualPlayer.Models.Config;
using VisualPlayer.Resources;
using VisualPlayer.ViewModels.Config;

namespace VisualPlayer.Controls;

[TemplatePart("PART_PaletteList", typeof(ListBox), IsRequired = true)]
[TemplatePart("PART_RecentList", typeof(ListBox), IsRequired = true)]
[TemplatePart("PART_AddColorButton", typeof(Button), IsRequired = true)]
public class ColorPaletteControl : TemplatedControl
{
    private const int MAX_RECENT_COLORS = 5;

    private bool _isSynchronizingSelection;
    private Button? _addColorButton;
    private ListBox? _paletteList;
    private ListBox? _recentList;

    #region AVALONIA PROPERTIES

    public static readonly StyledProperty<AccentColor?> SelectedColorProperty
        = AvaloniaProperty.Register<ColorPaletteControl, AccentColor?>(nameof(SelectedColor), defaultBindingMode: BindingMode.TwoWay);

    public static readonly StyledProperty<string> TitleProperty
        = AvaloniaProperty.Register<ColorPaletteControl, string>(nameof(Title), string.Empty);

    public static readonly StyledProperty<ObservableCollection<AccentColor>> RecentAccentColorsProperty
        = AvaloniaProperty.Register<ColorPaletteControl, ObservableCollection<AccentColor>>(nameof(RecentAccentColors));

    #endregion AVALONIA PROPERTIES

    #region EVENTS

    public static readonly RoutedEvent<RoutedEventArgs> AddColorRequestedEvent
        = RoutedEvent.Register<ColorPaletteControl, RoutedEventArgs>(nameof(AddColorRequested), RoutingStrategies.Bubble);


    public event EventHandler<RoutedEventArgs> AddColorRequested
    {
        add => AddHandler(AddColorRequestedEvent, value);
        remove => RemoveHandler(AddColorRequestedEvent, value);
    }

    public event EventHandler<AccentColorChangedEventArgs>? SelectionChanged;

    #endregion EVENTS

    #region PROPERTIES

    public ObservableCollection<AccentColorViewModel> PaletteItems { get; }

    public ObservableCollection<AccentColor> RecentAccentColors
    {
        get => GetValue(RecentAccentColorsProperty);
        set => SetValue(RecentAccentColorsProperty, value);
    }

    public ObservableCollection<AccentColorViewModel> RecentColors { get; } = new();

    public AccentColor? SelectedColor
    {
        get => GetValue(SelectedColorProperty);
        set => SetValue(SelectedColorProperty, value);
    }

    public string Title
    {
        get => GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    protected override Type StyleKeyOverride => typeof(ColorPaletteControl);

    #endregion PROPERTIES

    #region CONSTRUCTORS

    public ColorPaletteControl()
    {
        RecentAccentColors = new ObservableCollection<AccentColor>();
        PaletteItems = AccentColors.ToObservableCollection();
    }

    #endregion CONSTRUCTORS

    #region BUTTON ACTIONS

    private void AddColorButton_Click(object? sender, RoutedEventArgs e)
    {
        RaiseEvent(new RoutedEventArgs(AddColorRequestedEvent, this));
    }

    #endregion BUTTON ACTIONS

    #region PROPERTY CHANGE HANDLING

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == RecentAccentColorsProperty)
        {
            NormalizeRecentColors();
            RebuildRecentColors();
            SynchronizeLists(SelectedColor);
            return;
        }

        if (change.Property != SelectedColorProperty)
            return;

        var color = change.GetNewValue<AccentColor?>();

        if (color is null)
        {
            SynchronizeLists(null);
            return;
        }

        UpdateRecentColors(color);
        SynchronizeLists(color);
        OnSelectionChanged(color);
    }

    #endregion PROPERTY CHANGE HANDLING

    #region SELECTION

    private void NormalizeRecentColors()
    {
        while (RecentAccentColors.Count > MAX_RECENT_COLORS)
            RecentAccentColors.RemoveAt(RecentAccentColors.Count - 1);
    }

    protected virtual void OnSelectionChanged(AccentColor color)
    {
        SelectionChanged?.Invoke(this, new AccentColorChangedEventArgs(color));
    }

    private void PaletteList_SelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (_isSynchronizingSelection)
            return;

        if (_paletteList?.SelectedItem is AccentColorViewModel color)
        {
            SetCurrentValue(SelectedColorProperty, color.AccentColor);
            return;
        }

    }

    private void RebuildRecentColors()
    {
        RecentColors.Clear();

        foreach (var color in RecentAccentColors)
            RecentColors.Add(new AccentColorViewModel(color));
    }

    private void RecentList_SelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (_isSynchronizingSelection)
            return;

        if (_recentList?.SelectedItem is AccentColorViewModel color)
        {
            UpdateRecentColors(color.AccentColor);
            SetCurrentValue(SelectedColorProperty, color.AccentColor);
            SynchronizeLists(color.AccentColor);
        }
    }

    private void SynchronizeLists(AccentColor? color)
    {
        _isSynchronizingSelection = true;

        try
        {
            if (_paletteList is not null)
            {
                _paletteList.SelectedItem = color is null
                    ? null
                    : PaletteItems.FirstOrDefault(item => item.AccentColor.Equals(color));
            }

            if (_recentList is not null)
            {
                _recentList.SelectedItem = color is null
                    ? null
                    : RecentColors.FirstOrDefault(item => item.AccentColor.Equals(color));
            }
        }
        finally
        {
            _isSynchronizingSelection = false;
        }
    }

    private void UpdateRecentColors(AccentColor color)
    {
        var existing = RecentAccentColors.FirstOrDefault(item => item.Equals(color));

        if (existing is not null)
            RecentAccentColors.Remove(existing);

        RecentAccentColors.Insert(0, existing ?? color);

        NormalizeRecentColors();
        RebuildRecentColors();
    }

    #endregion SELECTION

    #region TEMPLATE

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        DetachHandlers();
        base.OnApplyTemplate(e);

        _addColorButton = e.NameScope.Find<Button>("PART_AddColorButton");
        _paletteList = e.NameScope.Find<ListBox>("PART_PaletteList");
        _recentList = e.NameScope.Find<ListBox>("PART_RecentList");

        if (_addColorButton is not null)
            _addColorButton.Click += AddColorButton_Click;

        if (_paletteList is not null)
            _paletteList.SelectionChanged += PaletteList_SelectionChanged;

        if (_recentList is not null)
            _recentList.SelectionChanged += RecentList_SelectionChanged;

        SynchronizeLists(SelectedColor);
    }

    private void DetachHandlers()
    {
        if (_addColorButton is not null)
            _addColorButton.Click -= AddColorButton_Click;

        if (_paletteList is not null)
            _paletteList.SelectionChanged -= PaletteList_SelectionChanged;

        if (_recentList is not null)
            _recentList.SelectionChanged -= RecentList_SelectionChanged;

        _addColorButton = null;
        _paletteList = null;
        _recentList = null;
    }

    #endregion TEMPLATE
}
