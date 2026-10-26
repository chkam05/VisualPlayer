using System;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Threading;
using Avalonia.VisualTree;
using VisualPlayer.ViewModels;

namespace VisualPlayer.Controls;

public partial class PlayListControl : UserControl
{
    #region CONST

    private const double AUTO_SCROLL_EDGE_SIZE = 56;
    private const double AUTO_SCROLL_MAX_STEP = 18;

    #endregion CONST

    private Button? _closeButton;
    private MenuItem? _clearPlaylistMenuItem;
    private ListBoxItem? _draggedItem;
    private TrackViewModel? _draggedTrack;
    private Point _dragPointerOffset;
    private double _dragPointerY;
    private ContentControl? _dragPreview;
    private Point _dragStartPoint;
    private readonly DispatcherTimer _dragScrollTimer;
    private ListBoxItem? _dropIndicatorItem;
    private bool _isReorderDragging;
    private MenuItem? _exportPlaylistMenuItem;
    private ContextMenu? _moreContextMenu;
    private Button? _moreButton;
    private MenuItem? _openPlaylistMenuItem;
    private int _reorderTargetIndex = -1;
    private MenuItem? _savePlaylistMenuItem;
    private Button? _sortButton;
    private ContextMenu? _sortContextMenu;
    private ListBox? _trackList;
    private ScrollViewer? _trackScrollViewer;
    
    #region EVENTS

    public static readonly RoutedEvent<RoutedEventArgs> AddFileRequestedEvent
        = RoutedEvent.Register<PlayListControl, RoutedEventArgs>(nameof(AddFileRequested), RoutingStrategies.Bubble);

    public static readonly RoutedEvent<RoutedEventArgs> ClearPlaylistRequestedEvent
        = RoutedEvent.Register<PlayListControl, RoutedEventArgs>(nameof(ClearPlaylistRequested), RoutingStrategies.Bubble);

    public static readonly RoutedEvent<RoutedEventArgs> CloseRequestedEvent
        = RoutedEvent.Register<PlayListControl, RoutedEventArgs>(nameof(CloseRequested), RoutingStrategies.Bubble);

    public static readonly RoutedEvent<RoutedEventArgs> OpenPlaylistRequestedEvent
        = RoutedEvent.Register<PlayListControl, RoutedEventArgs>(nameof(OpenPlaylistRequested), RoutingStrategies.Bubble);

    public static readonly RoutedEvent<RoutedEventArgs> SavePlaylistRequestedEvent
        = RoutedEvent.Register<PlayListControl, RoutedEventArgs>(nameof(SavePlaylistRequested), RoutingStrategies.Bubble);

    public static readonly RoutedEvent<RoutedEventArgs> ExportPlaylistRequestedEvent
        = RoutedEvent.Register<PlayListControl, RoutedEventArgs>(nameof(ExportPlaylistRequested), RoutingStrategies.Bubble);
    

    public event EventHandler<RoutedEventArgs>? AddFileRequested
    {
        add => AddHandler(AddFileRequestedEvent, value);
        remove => RemoveHandler(AddFileRequestedEvent, value);
    }

    public event EventHandler<RoutedEventArgs>? ClearPlaylistRequested
    {
        add => AddHandler(ClearPlaylistRequestedEvent, value);
        remove => RemoveHandler(ClearPlaylistRequestedEvent, value);
    }

    public event EventHandler<RoutedEventArgs>? CloseRequested
    {
        add => AddHandler(CloseRequestedEvent, value);
        remove => RemoveHandler(CloseRequestedEvent, value);
    }

    public event EventHandler<RoutedEventArgs>? OpenPlaylistRequested
    {
        add => AddHandler(OpenPlaylistRequestedEvent, value);
        remove => RemoveHandler(OpenPlaylistRequestedEvent, value);
    }

    public event EventHandler<RoutedEventArgs>? SavePlaylistRequested
    {
        add => AddHandler(SavePlaylistRequestedEvent, value);
        remove => RemoveHandler(SavePlaylistRequestedEvent, value);
    }

    public event EventHandler<RoutedEventArgs>? ExportPlaylistRequested
    {
        add => AddHandler(ExportPlaylistRequestedEvent, value);
        remove => RemoveHandler(ExportPlaylistRequestedEvent, value);
    }

    #endregion EVENTS

    #region CONSTRUCTORS

    public PlayListControl()
    {
        _dragScrollTimer = new DispatcherTimer
        {
            Interval = TimeSpan.FromMilliseconds(16)
        };

        _dragScrollTimer.Tick += DragScrollTimer_Tick;

        InitializeComponent();
        InitializeElements();
    }

    #endregion CONSTRUCTORS

    #region ACTIONS

    private void CloseButton_Click(object? sender, RoutedEventArgs e)
    {
        RaiseEvent(new RoutedEventArgs(CloseRequestedEvent));
        e.Handled = true;
    }

    private void ClearPlaylistMenuItem_Click(object? sender, RoutedEventArgs e)
    {
        RaiseEvent(new RoutedEventArgs(ClearPlaylistRequestedEvent));
    }

    private void DragScrollTimer_Tick(object? sender, EventArgs e)
    {
        if (_trackList is null || _draggedTrack is null)
        {
            StopAutoScroll();
            return;
        }

        _trackScrollViewer ??= _trackList
            .GetVisualDescendants()
            .OfType<ScrollViewer>()
            .FirstOrDefault();

        if (_trackScrollViewer is null)
            return;

        var step = GetAutoScrollStep();
        if (Math.Abs(step) < double.Epsilon)
        {
            StopAutoScroll();
            return;
        }

        var maximumOffset = Math.Max(
            0,
            _trackScrollViewer.Extent.Height - _trackScrollViewer.Viewport.Height);
        var targetOffset = Math.Clamp(
            _trackScrollViewer.Offset.Y + step,
            0,
            maximumOffset);

        if (Math.Abs(targetOffset - _trackScrollViewer.Offset.Y) < 0.1)
            return;

        _trackScrollViewer.Offset = new Vector(
            _trackScrollViewer.Offset.X,
            targetOffset);

        UpdateDropTarget(_draggedTrack, _dragPointerY);
    }

    private void OpenPlaylistMenuItem_Click(object? sender, RoutedEventArgs e)
    {
        RaiseEvent(new RoutedEventArgs(OpenPlaylistRequestedEvent));
    }

    private void SavePlaylistMenuItem_Click(object? sender, RoutedEventArgs e)
    {
        RaiseEvent(new RoutedEventArgs(SavePlaylistRequestedEvent));
    }

    private void ExportPlaylistMenuItem_Click(object? sender, RoutedEventArgs e)
    {
        RaiseEvent(new RoutedEventArgs(ExportPlaylistRequestedEvent));
    }

    private void MoreButton_Click(object? sender, RoutedEventArgs e)
    {
        if (_moreButton is null || _moreContextMenu is null)
            return;

        _moreContextMenu.Open(_moreButton);
        e.Handled = true;
    }

    private void SortButton_Click(object? sender, RoutedEventArgs e)
    {
        if (_sortButton is null || _sortContextMenu is null)
            return;

        _sortContextMenu.Open(_sortButton);
        e.Handled = true;
    }

    private void TrackItem_DoubleTapped(object? sender, TappedEventArgs e)
    {
        if (sender is not Control { DataContext: TrackViewModel track }
            || IsTrackActionSource(e.Source as Visual))
            return;

        if (!track.IsSelectionMode && DataContext is PlayListViewModel viewModel)
            viewModel.Play(track);

        e.Handled = true;
    }

    private void TrackItem_PointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (sender is not Control { DataContext: TrackViewModel track } item
            || !track.IsSelectionMode
            || !e.GetCurrentPoint(item).Properties.IsLeftButtonPressed
            || e.ClickCount != 1
            || IsTrackActionSource(e.Source as Visual))
            return;

        track.IsSelected = !track.IsSelected;
        e.Handled = true;
    }

    private static bool IsTrackActionSource(Visual? source)
    {
        return source?.GetSelfAndVisualAncestors().Any(visual =>
            visual is Button or CheckBox or Thumb
            || visual is Control { Name: "PART_ReorderHandle" }) == true;
    }

    private void TrackOptionsButton_Click(object? sender, RoutedEventArgs e)
    {
        if (sender is Button { ContextMenu: { } menu } button)
            menu.Open(button);

        e.Handled = true;
    }

    private void TrackPlayMenuItem_Click(object? sender, RoutedEventArgs e)
    {
        if (sender is MenuItem { DataContext: TrackViewModel track }
            && DataContext is PlayListViewModel viewModel)
            viewModel.Play(track);

        e.Handled = true;
    }

    private void TrackRemoveMenuItem_Click(object? sender, RoutedEventArgs e)
    {
        if (sender is MenuItem { DataContext: TrackViewModel track }
            && DataContext is PlayListViewModel viewModel)
            viewModel.RemoveTrack(track);

        e.Handled = true;
    }

    private void TrackMoveMenuItem_Click(object? sender, RoutedEventArgs e)
    {
        if (sender is not MenuItem { DataContext: TrackViewModel track } item
            || DataContext is not PlayListViewModel viewModel)
            return;

        switch (item.Tag)
        {
            case "MoveUp": viewModel.MoveTrackUp(track); break;
            case "MoveDown": viewModel.MoveTrackDown(track); break;
            case "MoveTop": viewModel.MoveTrackToTop(track); break;
            case "MoveBottom": viewModel.MoveTrackToBottom(track); break;
        }

        e.Handled = true;
    }

    private void TrackList_PointerCaptureLost(object? sender, PointerCaptureLostEventArgs e)
    {
        ResetReorderDrag();
    }

    private void TrackList_PointerMoved(object? sender, PointerEventArgs e)
    {
        if (_trackList is null || _draggedTrack is null || _draggedItem is null)
            return;

        var position = e.GetPosition(_trackList);

        if (!_isReorderDragging)
        {
            var distance = position - _dragStartPoint;
            if (Math.Abs(distance.X) < 4 && Math.Abs(distance.Y) < 4)
                return;

            StartReorderDrag();
        }

        _dragPointerY = position.Y;
        UpdateDragPreview(position);
        UpdateDropTarget(_draggedTrack, _dragPointerY);
        UpdateAutoScroll();
        e.Handled = true;
    }

    private void TrackList_PointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (_trackList is null ||
            !e.GetCurrentPoint(_trackList).Properties.IsLeftButtonPressed ||
            e.Source is not Visual source)
            return;

        var listBoxItem = source.FindAncestorOfType<ListBoxItem>(includeSelf: true);
        var scrollBar = source.FindAncestorOfType<ScrollBar>(includeSelf: true);

        if (listBoxItem is null && scrollBar is null)
            _trackList.SelectedItem = null;

        var reorderHandle = source
            .GetSelfAndVisualAncestors()
            .OfType<Control>()
            .FirstOrDefault(control => control.Name == "PART_ReorderHandle");
        var item = reorderHandle?.FindAncestorOfType<ListBoxItem>();

        if (item?.DataContext is not TrackViewModel track)
            return;

        _draggedItem = item;
        _draggedTrack = track;
        _dragStartPoint = e.GetPosition(_trackList);
        _dragPointerOffset = e.GetPosition(item);
        _dragPointerY = _dragStartPoint.Y;
        e.Pointer.Capture(_trackList);
        e.Handled = true;
    }

    private void TrackList_PointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        if (_draggedTrack is null)
            return;

        if (_isReorderDragging && DataContext is PlayListViewModel viewModel)
        {
            var oldIndex = viewModel.Tracks.IndexOf(_draggedTrack);
            viewModel.MoveTrack(oldIndex, _reorderTargetIndex);
        }

        e.Pointer.Capture(null);
        e.Handled = true;
        ResetReorderDrag();
    }

    #endregion ACTIONS

    #region ELEMENT INITIALIZATION

    private void InitializeElements()
    {
        DetachHandlers();

        _closeButton = this.FindControl<Button>("CloseButton");
        _dragPreview = this.FindControl<ContentControl>("DragPreview");
        _moreButton = this.FindControl<Button>("MoreButton");
        _sortButton = this.FindControl<Button>("SortButton");
        _trackList = this.FindControl<ListBox>("TrackList");

        if (_moreButton is not null)
            _moreContextMenu = _moreButton.ContextMenu;

        if (_moreContextMenu is not null)
        {
            _openPlaylistMenuItem = FindMenuItem(_moreContextMenu, "OpenPlaylist");
            _savePlaylistMenuItem = FindMenuItem(_moreContextMenu, "SavePlaylist");
            _exportPlaylistMenuItem = FindMenuItem(_moreContextMenu, "ExportPlaylist");
            _clearPlaylistMenuItem = FindMenuItem(_moreContextMenu, "ClearPlaylist");
        }
        
        if (_sortButton is not null)
            _sortContextMenu = _sortButton.ContextMenu;

        if (_closeButton is not null)
            _closeButton.Click += CloseButton_Click;

        if (_moreButton is not null)
            _moreButton.Click += MoreButton_Click;

        if (_openPlaylistMenuItem is not null)
            _openPlaylistMenuItem.Click += OpenPlaylistMenuItem_Click;

        if (_savePlaylistMenuItem is not null)
            _savePlaylistMenuItem.Click += SavePlaylistMenuItem_Click;

        if (_exportPlaylistMenuItem is not null)
            _exportPlaylistMenuItem.Click += ExportPlaylistMenuItem_Click;

        if (_clearPlaylistMenuItem is not null)
            _clearPlaylistMenuItem.Click += ClearPlaylistMenuItem_Click;

        if (_sortButton is not null)
            _sortButton.Click += SortButton_Click;
        
        if (_trackList is not null)
        {
            _trackList.AddHandler(PointerPressedEvent, TrackList_PointerPressed, RoutingStrategies.Bubble, true);
            _trackList.AddHandler(PointerMovedEvent, TrackList_PointerMoved, RoutingStrategies.Bubble, true);
            _trackList.AddHandler(PointerReleasedEvent, TrackList_PointerReleased, RoutingStrategies.Bubble, true);
            _trackList.PointerCaptureLost += TrackList_PointerCaptureLost;
        }
    }

    private void DetachHandlers()
    {
        if (_closeButton is not null)
            _closeButton.Click -= CloseButton_Click;

        if (_moreButton is not null)
            _moreButton.Click -= MoreButton_Click;

        if (_openPlaylistMenuItem is not null)
            _openPlaylistMenuItem.Click -= OpenPlaylistMenuItem_Click;

        if (_savePlaylistMenuItem is not null)
            _savePlaylistMenuItem.Click -= SavePlaylistMenuItem_Click;

        if (_exportPlaylistMenuItem is not null)
            _exportPlaylistMenuItem.Click -= ExportPlaylistMenuItem_Click;

        if (_clearPlaylistMenuItem is not null)
            _clearPlaylistMenuItem.Click -= ClearPlaylistMenuItem_Click;

        if (_sortButton is not null)
            _sortButton.Click -= SortButton_Click;

        if (_trackList is not null)
        {
            _trackList.RemoveHandler(PointerPressedEvent, TrackList_PointerPressed);
            _trackList.RemoveHandler(PointerMovedEvent, TrackList_PointerMoved);
            _trackList.RemoveHandler(PointerReleasedEvent, TrackList_PointerReleased);
            _trackList.PointerCaptureLost -= TrackList_PointerCaptureLost;
        }

        ResetReorderDrag();

        _closeButton = null;
        _moreButton = null;
        _moreContextMenu = null;
        _clearPlaylistMenuItem = null;
        _openPlaylistMenuItem = null;
        _savePlaylistMenuItem = null;
        _exportPlaylistMenuItem = null;
        _sortButton = null;
        _sortContextMenu = null;
        _dragPreview = null;
        _trackList = null;
        _trackScrollViewer = null;
    }

    #endregion ELEMENT INITIALIZATION

    #region UTILITIES

    private void ClearDropIndicator()
    {
        SetDropPosition(_dropIndicatorItem, false, false);
        _dropIndicatorItem = null;
    }

    private static MenuItem? FindMenuItem(ContextMenu contextMenu, string tag)
    {
        return contextMenu.Items
            .OfType<MenuItem>()
            .FirstOrDefault(item => Equals(item.Tag, tag));
    }
    
    private double GetAutoScrollStep()
    {
        if (_trackList is null)
            return 0;

        if (_dragPointerY < AUTO_SCROLL_EDGE_SIZE)
        {
            var intensity = 1 - Math.Clamp(_dragPointerY / AUTO_SCROLL_EDGE_SIZE, 0, 1);
            return -AUTO_SCROLL_MAX_STEP * intensity;
        }

        var bottomEdgeStart = _trackList.Bounds.Height - AUTO_SCROLL_EDGE_SIZE;
        if (_dragPointerY > bottomEdgeStart)
        {
            var intensity = Math.Clamp(
                (_dragPointerY - bottomEdgeStart) / AUTO_SCROLL_EDGE_SIZE,
                0,
                1);
            return AUTO_SCROLL_MAX_STEP * intensity;
        }

        return 0;
    }

    private void ResetReorderDrag()
    {
        StopAutoScroll();
        ClearDropIndicator();

        if (_draggedItem is not null)
            _draggedItem.Opacity = 1;

        if (_dragPreview is not null)
        {
            _dragPreview.IsVisible = false;
            _dragPreview.Content = null;
        }

        _trackList?.ClearValue(CursorProperty);

        _draggedItem = null;
        _draggedTrack = null;
        _dragPointerY = 0;
        _reorderTargetIndex = -1;
        _isReorderDragging = false;
    }

    private static void SetDropPosition(ListBoxItem? item, bool before, bool after)
    {
        if (item is null)
            return;

        foreach (var border in item.GetVisualDescendants().OfType<Border>())
        {
            if (border.Name == "PART_DropBefore")
                border.IsVisible = before;
            else if (border.Name == "PART_DropAfter")
                border.IsVisible = after;
        }
    }

    private void SetDropIndicator(ListBoxItem? item, bool before)
    {
        if (!ReferenceEquals(_dropIndicatorItem, item))
            ClearDropIndicator();

        _dropIndicatorItem = item;
        SetDropPosition(_dropIndicatorItem, before, !before);
    }

    private void StartReorderDrag()
    {
        if (_dragPreview is null || _draggedItem is null || _draggedTrack is null)
            return;

        _isReorderDragging = true;
        _draggedItem.Opacity = 0.3;
        _trackList?.SetCurrentValue(CursorProperty, new Cursor(StandardCursorType.Hand));
        _dragPreview.Content = _draggedTrack;
        _dragPreview.Width = _draggedItem.Bounds.Width;
        _dragPreview.Height = _draggedItem.Bounds.Height;
        _dragPreview.IsVisible = true;
    }

    private void StopAutoScroll()
    {
        _dragScrollTimer.Stop();
    }

    private void UpdateAutoScroll()
    {
        var step = GetAutoScrollStep();

        if (Math.Abs(step) < double.Epsilon)
            _dragScrollTimer.Stop();
        else if (!_dragScrollTimer.IsEnabled)
            _dragScrollTimer.Start();
    }

    private void UpdateDragPreview(Point pointerPosition)
    {
        if (_dragPreview is null)
            return;

        Canvas.SetLeft(_dragPreview, pointerPosition.X - _dragPointerOffset.X);
        Canvas.SetTop(_dragPreview, pointerPosition.Y - _dragPointerOffset.Y);
    }

    private void UpdateDropTarget(TrackViewModel draggedTrack, double pointerY)
    {
        if (_trackList is null || DataContext is not PlayListViewModel viewModel)
            return;

        var remainingTracks = viewModel.VisibleTracks
            .Where(item => !ReferenceEquals(item, draggedTrack))
            .ToList();
        var itemControls = _trackList
            .GetVisualDescendants()
            .OfType<ListBoxItem>()
            .ToList();

        ListBoxItem? indicatorItem = null;
        var indicatorBefore = true;
        var targetIndex = viewModel.Tracks.Count - 1;
        var draggedIndex = viewModel.Tracks.IndexOf(draggedTrack);

        for (var index = 0; index < remainingTracks.Count; index++)
        {
            var candidate = remainingTracks[index];
            var item = itemControls.FirstOrDefault(control => ReferenceEquals(control.DataContext, candidate));
            var position = item?.TranslatePoint(default, _trackList);

            if (item is null || position is null)
                continue;

            var centerY = position.Value.Y + item.Bounds.Height / 2;
            if (pointerY < centerY)
            {
                var candidateIndex = viewModel.Tracks.IndexOf(candidate);
                targetIndex = draggedIndex < candidateIndex
                    ? candidateIndex - 1
                    : candidateIndex;
                indicatorItem = item;
                indicatorBefore = true;
                break;
            }

            var trailingCandidateIndex = viewModel.Tracks.IndexOf(candidate);
            targetIndex = draggedIndex < trailingCandidateIndex
                ? trailingCandidateIndex
                : trailingCandidateIndex + 1;
            indicatorItem = item;
            indicatorBefore = false;
        }

        _reorderTargetIndex = Math.Clamp(targetIndex, 0, viewModel.Tracks.Count - 1);
        SetDropIndicator(indicatorItem, indicatorBefore);
    }

    #endregion UTILITIES
}
