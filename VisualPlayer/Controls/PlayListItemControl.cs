using System;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.VisualTree;
using VisualPlayer.Events;
using VisualPlayer.ViewModels;

namespace VisualPlayer.Controls;

[PseudoClasses(":drop-before", ":drop-after")]
[TemplatePart("PART_OptionsButton", typeof(Button), IsRequired = true)]
[TemplatePart("PART_ReorderHandle", typeof(Control), IsRequired = true)]
public class PlayListItemControl : TemplatedControl
{
    private ContextMenu? _contextMenu;
    private MenuItem? _moveBottomMenuItem;
    private MenuItem? _moveDownMenuItem;
    private MenuItem? _moveTopMenuItem;
    private MenuItem? _moveUpMenuItem;
    private Button? _optionsButton;
    private MenuItem? _playMenuItem;
    private MenuItem? _removeMenuItem;
    
    #region EVENTS

    public static readonly RoutedEvent<TrackRequestedEventArgs> PlayRequestedEvent
        = RoutedEvent.Register<PlayListItemControl, TrackRequestedEventArgs>(nameof(PlayRequested), RoutingStrategies.Bubble);
    
    public static readonly RoutedEvent<TrackRequestedEventArgs> RemoveRequestedEvent
        = RoutedEvent.Register<PlayListItemControl, TrackRequestedEventArgs>(nameof(RemoveRequested), RoutingStrategies.Bubble);

    public static readonly RoutedEvent<TrackRequestedEventArgs> MoveUpRequestedEvent
        = RoutedEvent.Register<PlayListItemControl, TrackRequestedEventArgs>(nameof(MoveUpRequested), RoutingStrategies.Bubble);

    public static readonly RoutedEvent<TrackRequestedEventArgs> MoveDownRequestedEvent
        = RoutedEvent.Register<PlayListItemControl, TrackRequestedEventArgs>(nameof(MoveDownRequested), RoutingStrategies.Bubble);

    public static readonly RoutedEvent<TrackRequestedEventArgs> MoveTopRequestedEvent
        = RoutedEvent.Register<PlayListItemControl, TrackRequestedEventArgs>(nameof(MoveTopRequested), RoutingStrategies.Bubble);

    public static readonly RoutedEvent<TrackRequestedEventArgs> MoveBottomRequestedEvent
        = RoutedEvent.Register<PlayListItemControl, TrackRequestedEventArgs>(nameof(MoveBottomRequested), RoutingStrategies.Bubble);

    
    public event EventHandler<TrackRequestedEventArgs>? PlayRequested
    {
        add => AddHandler(PlayRequestedEvent, value);
        remove => RemoveHandler(PlayRequestedEvent, value);
    }

    public event EventHandler<TrackRequestedEventArgs>? RemoveRequested
    {
        add => AddHandler(RemoveRequestedEvent, value);
        remove => RemoveHandler(RemoveRequestedEvent, value);
    }

    public event EventHandler<TrackRequestedEventArgs>? MoveUpRequested
    {
        add => AddHandler(MoveUpRequestedEvent, value);
        remove => RemoveHandler(MoveUpRequestedEvent, value);
    }

    public event EventHandler<TrackRequestedEventArgs>? MoveDownRequested
    {
        add => AddHandler(MoveDownRequestedEvent, value);
        remove => RemoveHandler(MoveDownRequestedEvent, value);
    }

    public event EventHandler<TrackRequestedEventArgs>? MoveTopRequested
    {
        add => AddHandler(MoveTopRequestedEvent, value);
        remove => RemoveHandler(MoveTopRequestedEvent, value);
    }

    public event EventHandler<TrackRequestedEventArgs>? MoveBottomRequested
    {
        add => AddHandler(MoveBottomRequestedEvent, value);
        remove => RemoveHandler(MoveBottomRequestedEvent, value);
    }

    #endregion EVENTS

    #region CONSTRUCTORS

    public PlayListItemControl()
    {
        PointerPressed += OnPointerPressed;
        DoubleTapped += OnDoubleTapped;
    }

    #endregion CONSTRUCTORS

    #region ACTIONS

    private void OnDoubleTapped(object? sender, TappedEventArgs e)
    {
        if (DataContext is TrackViewModel { IsSelectionMode: true })
        {
            e.Handled = true;
            return;
        }

        if (e.Source is Visual visual)
        {
            if (IsReorderHandle(visual))
                return;

            if (visual.FindAncestorOfType<Button>(includeSelf: true) is not null)
                return;

            if (visual.FindAncestorOfType<CheckBox>(includeSelf: true) is not null)
                return;

            if (visual.FindAncestorOfType<Thumb>(includeSelf: true) is not null)
                return;
        }

        RequestPlay();
        e.Handled = true;
    }

    private void OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (DataContext is not TrackViewModel track)
            return;

        if (!track.IsSelectionMode)
            return;

        if (!e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
            return;

        if (e.ClickCount != 1)
            return;

        if (e.Source is Visual visual)
        {
            if (IsReorderHandle(visual))
                return;

            if (visual.FindAncestorOfType<Button>(includeSelf: true) is not null)
                return;

            if (visual.FindAncestorOfType<CheckBox>(includeSelf: true) is not null)
                return;

            if (visual.FindAncestorOfType<Thumb>(includeSelf: true) is not null)
                return;
        }

        track.IsSelected = !track.IsSelected;
        e.Handled = true;
    }

    private void OptionsButton_Click(object? sender, RoutedEventArgs e)
    {
        if (_optionsButton is null || _contextMenu is null)
            return;

        // ContextMenu normalnie reaguje na prawy click. Tutaj otwieramy go również zwykłym lewym kliknięciem w "...".

        _contextMenu.Open(_optionsButton);
        e.Handled = true;
    }

    private void PlayMenuItem_Click(object? sender, RoutedEventArgs e)
    {
        RequestPlay();
        e.Handled = true;
    }

    private void RemoveMenuItem_Click(object? sender, RoutedEventArgs e)
    {
        if (DataContext is not TrackViewModel track)
            return;

        RaiseEvent(new TrackRequestedEventArgs(RemoveRequestedEvent, track));
        e.Handled = true;
    }

    private void MoveMenuItem_Click(object? sender, RoutedEventArgs e)
    {
        if (DataContext is not TrackViewModel track || sender is not MenuItem menuItem)
            return;

        var routedEvent = menuItem.Tag switch
        {
            "MoveUp" => MoveUpRequestedEvent,
            "MoveDown" => MoveDownRequestedEvent,
            "MoveTop" => MoveTopRequestedEvent,
            "MoveBottom" => MoveBottomRequestedEvent,
            _ => null
        };

        if (routedEvent is null)
            return;

        RaiseEvent(new TrackRequestedEventArgs(routedEvent, track));
        e.Handled = true;
    }

    private void RequestPlay()
    {
        if (DataContext is not TrackViewModel track)
            return;

        RaiseEvent(new TrackRequestedEventArgs(PlayRequestedEvent, track));
    }

    private static bool IsReorderHandle(Visual visual)
    {
        return visual
            .GetSelfAndVisualAncestors()
            .OfType<Control>()
            .Any(control => control.Name == "PART_ReorderHandle");
    }

    internal void SetDropPosition(bool before, bool after)
    {
        PseudoClasses.Set(":drop-before", before);
        PseudoClasses.Set(":drop-after", after);
    }

    #endregion ACTIONS

    #region TEMPLATE

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        DetachHandlers();

        base.OnApplyTemplate(e);

        _optionsButton = e.NameScope.Find<Button>("PART_OptionsButton");

        if (_optionsButton is not null)
            _contextMenu = _optionsButton.ContextMenu;

        if (_contextMenu is not null)
        {
            _playMenuItem = _contextMenu.Items
                .OfType<MenuItem>()
                .FirstOrDefault(item => Equals(item.Tag, "Play"));

            _removeMenuItem = _contextMenu.Items
                .OfType<MenuItem>()
                .FirstOrDefault(item => Equals(item.Tag, "Remove"));

            _moveUpMenuItem = FindMenuItem("MoveUp");
            _moveDownMenuItem = FindMenuItem("MoveDown");
            _moveTopMenuItem = FindMenuItem("MoveTop");
            _moveBottomMenuItem = FindMenuItem("MoveBottom");
        }

        if (_optionsButton is not null)
            _optionsButton.Click += OptionsButton_Click;

        if (_playMenuItem is not null)
            _playMenuItem.Click += PlayMenuItem_Click;

        if (_removeMenuItem is not null)
            _removeMenuItem.Click += RemoveMenuItem_Click;

        AttachMoveHandler(_moveUpMenuItem);
        AttachMoveHandler(_moveDownMenuItem);
        AttachMoveHandler(_moveTopMenuItem);
        AttachMoveHandler(_moveBottomMenuItem);
        
    }

    private void DetachHandlers()
    {
        if (_optionsButton is not null)
            _optionsButton.Click -= OptionsButton_Click;

        if (_playMenuItem is not null)
            _playMenuItem.Click -= PlayMenuItem_Click;

        if (_removeMenuItem is not null)
            _removeMenuItem.Click -= RemoveMenuItem_Click;

        DetachMoveHandler(_moveUpMenuItem);
        DetachMoveHandler(_moveDownMenuItem);
        DetachMoveHandler(_moveTopMenuItem);
        DetachMoveHandler(_moveBottomMenuItem);


        _optionsButton = null;
        _contextMenu = null;
        _playMenuItem = null;
        _removeMenuItem = null;
        _moveUpMenuItem = null;
        _moveDownMenuItem = null;
        _moveTopMenuItem = null;
        _moveBottomMenuItem = null;
    }

    private void AttachMoveHandler(MenuItem? menuItem)
    {
        if (menuItem is not null)
            menuItem.Click += MoveMenuItem_Click;
    }

    private void DetachMoveHandler(MenuItem? menuItem)
    {
        if (menuItem is not null)
            menuItem.Click -= MoveMenuItem_Click;
    }

    private MenuItem? FindMenuItem(string tag)
    {
        return _contextMenu?.Items
            .OfType<MenuItem>()
            .FirstOrDefault(item => Equals(item.Tag, tag));
    }

    #endregion TEMPLATE
}
