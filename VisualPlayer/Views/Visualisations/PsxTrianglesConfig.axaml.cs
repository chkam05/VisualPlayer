using System;
using System.Collections.ObjectModel;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.VisualTree;
using VisualPlayer.Contexts;
using VisualPlayer.Enums;
using VisualPlayer.Visualizations.Config;
using VisualPlayer.Visualizations.Config.Base;
using VisualPlayer.Visualizations.Data;

namespace VisualPlayer.Views.Visualisations;

public partial class PsxTrianglesConfig : UserControl
{
    private ListBoxItem? _draggedItem;
    private PsxTriangleLayerItem? _draggedLayer;
    private Point _dragStartPoint;
    private bool _isDragging;

    #region PROPERTIES

    public BeamEffect[] BeamEffects { get; } = Enum.GetValues<BeamEffect>();

    public ObservableCollection<PsxTriangleLayerItem> LayerItems { get; } = [];

    #endregion PROPERTIES

    #region CONSTRUCTORS

    public PsxTrianglesConfig()
    {
        InitializeComponent();

        LayerOrderList.AddHandler(
            PointerPressedEvent,
            LayerOrderList_PointerPressed,
            RoutingStrategies.Bubble,
            handledEventsToo: true);

        LayerOrderList.AddHandler(
            PointerMovedEvent,
            LayerOrderList_PointerMoved,
            RoutingStrategies.Bubble,
            handledEventsToo: true);

        LayerOrderList.AddHandler(
            PointerReleasedEvent,
            LayerOrderList_PointerReleased,
            RoutingStrategies.Bubble,
            handledEventsToo: true);
            
        LayerOrderList.AddHandler(
            PointerCaptureLostEvent,
            LayerOrderList_PointerCaptureLost,
            RoutingStrategies.Bubble,
            handledEventsToo: true);

        DataContextChanged += PsxTrianglesConfig_DataContextChanged;
    }

    #endregion CONSTRUCTORS

    #region LAYER ORDER ITEMS ACTIONS

    private void LayerOrderList_PointerCaptureLost(object? sender, PointerCaptureLostEventArgs e)
    {
        if (_draggedItem is not null)
            _draggedItem.Opacity = 1;

        _draggedItem = null;
        _draggedLayer = null;
        _isDragging = false;
    }

    private void LayerOrderList_PointerMoved(object? sender, PointerEventArgs e)
    {
        if (sender is not ListBox listBox || _draggedLayer is null)
            return;

        var position = e.GetPosition(listBox);
        if (!_isDragging)
        {
            var distance = position - _dragStartPoint;
            if (Math.Abs(distance.X) < 4 && Math.Abs(distance.Y) < 4)
                return;

            _isDragging = true;
            if (_draggedItem is not null)
                _draggedItem.Opacity = 0.35;
        }

        var targetItem = listBox.GetVisualDescendants()
            .OfType<ListBoxItem>()
            .FirstOrDefault(item =>
            {
                var itemPosition = item.TranslatePoint(default, listBox);
                return itemPosition is not null
                    && position.Y >= itemPosition.Value.Y
                    && position.Y <= itemPosition.Value.Y + item.Bounds.Height;
            });

        if (targetItem?.DataContext is not PsxTriangleLayerItem target
            || ReferenceEquals(target, _draggedLayer))
        {
            return;
        }

        var oldIndex = LayerItems.IndexOf(_draggedLayer);
        var newIndex = LayerItems.IndexOf(target);
        if (oldIndex >= 0 && newIndex >= 0)
        {
            LayerItems.Move(oldIndex, newIndex);
            PersistLayerOrder();
        }

        e.Handled = true;
    }

    private void LayerOrderList_PointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (sender is not ListBox listBox
            || !e.GetCurrentPoint(listBox).Properties.IsLeftButtonPressed
            || e.Source is not Visual source)
        {
            return;
        }

        var handle = source.GetSelfAndVisualAncestors()
            .OfType<Control>()
            .FirstOrDefault(control => control.Name == "PART_ReorderHandle");
        var item = handle?.FindAncestorOfType<ListBoxItem>();
        if (item?.DataContext is not PsxTriangleLayerItem layer)
            return;

        _draggedItem = item;
        _draggedLayer = layer;
        _dragStartPoint = e.GetPosition(listBox);
        e.Pointer.Capture(listBox);
        e.Handled = true;
    }

    private void LayerOrderList_PointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        if (_draggedLayer is null)
            return;

        if (_isDragging)
            PersistLayerOrder();

        if (_draggedItem is not null)
            _draggedItem.Opacity = 1;

        e.Pointer.Capture(null);
        _draggedItem = null;
        _draggedLayer = null;
        _isDragging = false;
        e.Handled = true;
    }

    #endregion LAYER ORDER ITEMS ACTIONS

    #region LAYER ORDER ITEMS MANAGEMENT

    private static PsxTriangleLayerItem CreateLayerItem(
        PsxTrianglesVisualizationConfig config,
        int groupIndex)
    {
        var translations = ApplicationContext.Instance.Translations;
        var number = groupIndex + 1;
        return new PsxTriangleLayerItem(
            groupIndex,
            translations[$"VIEW_VISUALIZATIONS_PSX_TRIANGLES_GROUP_{number}_TITLE"],
            translations[$"VIEW_VISUALIZATIONS_PSX_TRIANGLES_GROUP_{number}_DESC"],
            config);
    }

    private void PersistLayerOrder()
    {
        if (DataContext is VisualisationProfile { Config: PsxTrianglesVisualizationConfig config })
            config.LayerOrder = LayerItems.Select(item => item.GroupIndex).ToArray();
    }

    private void RebuildLayerItems()
    {
        LayerItems.Clear();
        if (DataContext is not VisualisationProfile { Config: PsxTrianglesVisualizationConfig config })
            return;

        foreach (var groupIndex in config.LayerOrder)
            LayerItems.Add(CreateLayerItem(config, groupIndex));
    }

    #endregion LAYER ORDER ITEMS MANAGEMENT

    #region PROPERTY CHANGES

    private void PsxTrianglesConfig_DataContextChanged(object? sender, EventArgs e)
    {
        RebuildLayerItems();
    }

    #endregion PROPERTY CHANGES
}
