using System;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Threading;
using Avalonia.VisualTree;
using VisualPlayer.Events;
using VisualPlayer.ViewModels;

namespace VisualPlayer.Controls;

[TemplatePart("PART_CloseButton", typeof(Button), IsRequired = true)]
[TemplatePart("PART_DragPreview", typeof(PlayListItemControl), IsRequired = true)]
[TemplatePart("PART_MoreButton", typeof(Button), IsRequired = true)]
[TemplatePart("PART_SortButton", typeof(Button), IsRequired = true)]
[TemplatePart("PART_TrackList", typeof(ListBox), IsRequired = true)]
public class PlayListControl : TemplatedControl
{
    #region CONST

    private const double AUTO_SCROLL_EDGE_SIZE = 56;
    private const double AUTO_SCROLL_MAX_STEP = 18;

    #endregion CONST

    private Button? _closeButton;
    private MenuItem? _clearPlaylistMenuItem;
    private PlayListItemControl? _draggedItem;
    private TrackViewModel? _draggedTrack;
    private Point _dragPointerOffset;
    private double _dragPointerY;
    private PlayListItemControl? _dragPreview;
    private Point _dragStartPoint;
    private readonly DispatcherTimer _dragScrollTimer;
    private PlayListItemControl? _dropIndicatorItem;
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

    private void Track_PlayRequested(object? sender, TrackRequestedEventArgs e)
    {
        if (DataContext is not PlayListViewModel viewModel)
            return;

        viewModel.Play(e.Track);
        e.Handled = true;
    }

    private void Track_RemoveRequested(object? sender, TrackRequestedEventArgs e)
    {
        if (DataContext is not PlayListViewModel viewModel)
            return;

        viewModel.RemoveTrack(e.Track);
        e.Handled = true;
    }

    private void Track_MoveUpRequested(object? sender, TrackRequestedEventArgs e)
    {
        if (DataContext is PlayListViewModel viewModel)
            viewModel.MoveTrackUp(e.Track);
        e.Handled = true;
    }

    private void Track_MoveDownRequested(object? sender, TrackRequestedEventArgs e)
    {
        if (DataContext is PlayListViewModel viewModel)
            viewModel.MoveTrackDown(e.Track);
        e.Handled = true;
    }

    private void Track_MoveTopRequested(object? sender, TrackRequestedEventArgs e)
    {
        if (DataContext is PlayListViewModel viewModel)
            viewModel.MoveTrackToTop(e.Track);
        e.Handled = true;
    }

    private void Track_MoveBottomRequested(object? sender, TrackRequestedEventArgs e)
    {
        if (DataContext is PlayListViewModel viewModel)
            viewModel.MoveTrackToBottom(e.Track);
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
        var item = reorderHandle?.FindAncestorOfType<PlayListItemControl>();

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

    #region TEMPLATE

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        DetachHandlers();

        base.OnApplyTemplate(e);

        _closeButton = e.NameScope.Find<Button>("PART_CloseButton");
        _dragPreview = e.NameScope.Find<PlayListItemControl>("PART_DragPreview");
        _moreButton = e.NameScope.Find<Button>("PART_MoreButton");
        _sortButton = e.NameScope.Find<Button>("PART_SortButton");
        _trackList = e.NameScope.Find<ListBox>("PART_TrackList");

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
            _trackList.AddHandler(PlayListItemControl.PlayRequestedEvent, Track_PlayRequested);
            _trackList.AddHandler(PlayListItemControl.RemoveRequestedEvent, Track_RemoveRequested);
            _trackList.AddHandler(PlayListItemControl.MoveUpRequestedEvent, Track_MoveUpRequested);
            _trackList.AddHandler(PlayListItemControl.MoveDownRequestedEvent, Track_MoveDownRequested);
            _trackList.AddHandler(PlayListItemControl.MoveTopRequestedEvent, Track_MoveTopRequested);
            _trackList.AddHandler(PlayListItemControl.MoveBottomRequestedEvent, Track_MoveBottomRequested);
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
            _trackList.RemoveHandler(PlayListItemControl.PlayRequestedEvent, Track_PlayRequested);
            _trackList.RemoveHandler(PlayListItemControl.RemoveRequestedEvent, Track_RemoveRequested);
            _trackList.RemoveHandler(PlayListItemControl.MoveUpRequestedEvent, Track_MoveUpRequested);
            _trackList.RemoveHandler(PlayListItemControl.MoveDownRequestedEvent, Track_MoveDownRequested);
            _trackList.RemoveHandler(PlayListItemControl.MoveTopRequestedEvent, Track_MoveTopRequested);
            _trackList.RemoveHandler(PlayListItemControl.MoveBottomRequestedEvent, Track_MoveBottomRequested);
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

    #endregion TEMPLATE

    #region UTILITIES

    private void ClearDropIndicator()
    {
        _dropIndicatorItem?.SetDropPosition(false, false);
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
            _dragPreview.DataContext = null;
        }

        _trackList?.ClearValue(CursorProperty);

        _draggedItem = null;
        _draggedTrack = null;
        _dragPointerY = 0;
        _reorderTargetIndex = -1;
        _isReorderDragging = false;
    }

    private void SetDropIndicator(PlayListItemControl? item, bool before)
    {
        if (!ReferenceEquals(_dropIndicatorItem, item))
            ClearDropIndicator();

        _dropIndicatorItem = item;
        _dropIndicatorItem?.SetDropPosition(before, !before);
    }

    private void StartReorderDrag()
    {
        if (_dragPreview is null || _draggedItem is null || _draggedTrack is null)
            return;

        _isReorderDragging = true;
        _draggedItem.Opacity = 0.3;
        _trackList?.SetCurrentValue(CursorProperty, new Cursor(StandardCursorType.Hand));
        _dragPreview.DataContext = _draggedTrack;
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
            .OfType<PlayListItemControl>()
            .ToList();

        PlayListItemControl? indicatorItem = null;
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
