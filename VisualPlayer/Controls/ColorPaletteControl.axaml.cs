using System;
using System.Collections.ObjectModel;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Interactivity;
using Avalonia.Threading;
using VisualPlayer.Events;
using VisualPlayer.Models.Config;
using VisualPlayer.Resources;
using VisualPlayer.ViewModels.Config;

namespace VisualPlayer.Controls;

public partial class ColorPaletteControl : UserControl
{
    private const int MAX_RECENT_COLORS = 5;

    private bool _isSynchronizingSelection;
    private bool _isRecentColorsRefreshPending;
    private Button? _addColorButton;
    private ListBox? _paletteList;
    private ListBox? _recentList;

    #region AVALONIA PROPERTIES

    public static readonly StyledProperty<AccentColor?> SelectedColorProperty
        = AvaloniaProperty.Register<ColorPaletteControl, AccentColor?>(nameof(SelectedColor), defaultBindingMode: BindingMode.TwoWay);

    public static readonly StyledProperty<string> TitleProperty
        = AvaloniaProperty.Register<ColorPaletteControl, string>(nameof(Title), "Kolor wiodący");

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

    #endregion PROPERTIES

    #region CONSTRUCTORS

    public ColorPaletteControl()
    {
        RecentAccentColors = new ObservableCollection<AccentColor>();
        PaletteItems = AccentColors.ToObservableCollection();

        InitializeComponent();
        InitializeElements();
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
        if (_isRecentColorsRefreshPending)
            return;

        _isRecentColorsRefreshPending = true;
        // SelectionChanged can run while Avalonia is committing a selection or
        // processing a collection change. Wait until that operation has finished.
        Dispatcher.UIThread.Post(RefreshRecentColors);
    }

    private void RefreshRecentColors()
    {
        _isRecentColorsRefreshPending = false;
        var colors = RecentAccentColors.ToArray();
        var wasSynchronizingSelection = _isSynchronizingSelection;
        _isSynchronizingSelection = true;

        try
        {
            RecentColors.Clear();

            foreach (var color in colors)
                RecentColors.Add(new AccentColorViewModel(color));

            SynchronizeLists(SelectedColor);
        }
        finally
        {
            _isSynchronizingSelection = wasSynchronizingSelection;
        }
    }

    private void RecentList_SelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (_isSynchronizingSelection)
            return;

        if (_recentList?.SelectedItem is AccentColorViewModel color)
        {
            SetCurrentValue(SelectedColorProperty, color.AccentColor);
        }
    }

    private void SynchronizeLists(AccentColor? color)
    {
        var wasSynchronizingSelection = _isSynchronizingSelection;
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
            _isSynchronizingSelection = wasSynchronizingSelection;
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

    #region ELEMENT INITIALIZATION

    private void InitializeElements()
    {
        DetachHandlers();

        _addColorButton = this.FindControl<Button>("AddColorButton");
        _paletteList = this.FindControl<ListBox>("PaletteList");
        _recentList = this.FindControl<ListBox>("RecentList");

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

    #endregion ELEMENT INITIALIZATION
}
