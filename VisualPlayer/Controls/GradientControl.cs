using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Styling;
using VisualPlayer.Enums;
using VisualPlayer.Utils;
using VisualPlayer.Visualizations.Config;
using VisualPlayer.Visualizations.Config.Base;

namespace VisualPlayer.Controls;

[TemplatePart("PART_AddPointButton", typeof(Button), IsRequired = true)]
[TemplatePart("PART_Preview", typeof(Border), IsRequired = true)]
[TemplatePart("PART_RemovePointButton", typeof(Button), IsRequired = true)]
[TemplatePart("PART_StopsCanvas", typeof(Canvas), IsRequired = true)]
public class GradientControl : TemplatedControl
{
    private Button? _addPointButton;
    private IBrush _gradientBrush = Brushes.Transparent;
    private Border? _preview;
    private Button? _removePointButton;
    private GradientPoint? _selectedPoint;
    private Canvas? _stopsCanvas;


    public IReadOnlyList<GradientType> GradientTypes { get; } = Enum.GetValues<GradientType>();

    #region AVALONIA PROPERTIES

    public static readonly StyledProperty<GradientDefinition> GradientProperty
        = AvaloniaProperty.Register<GradientControl, GradientDefinition>(nameof(Gradient));

    public static readonly DirectProperty<GradientControl, IBrush> GradientBrushProperty
        = AvaloniaProperty.RegisterDirect<GradientControl, IBrush>(nameof(GradientBrush), control => control.GradientBrush);

    public static readonly DirectProperty<GradientControl, GradientPoint?> SelectedPointProperty
        = AvaloniaProperty.RegisterDirect<GradientControl, GradientPoint?>(nameof(SelectedPoint), control => control.SelectedPoint, (control, value) => control.SelectedPoint = value);

    #endregion AVALONIA PROPERTIES

    #region PROPERTIES

    public GradientDefinition Gradient
    {
        get => GetValue(GradientProperty);
        set => SetValue(GradientProperty, value);
    }

    public IBrush GradientBrush
    {
        get => _gradientBrush;
        private set => SetAndRaise(GradientBrushProperty, ref _gradientBrush, value);
    }

    public GradientPoint? SelectedPoint
    {
        get => _selectedPoint;
        set
        {
            if (ReferenceEquals(_selectedPoint, value))
                return;

            SetAndRaise(SelectedPointProperty, ref _selectedPoint, value);
            UpdateHandleVisuals();
            UpdateButtons();
        }
    }

    protected override Type StyleKeyOverride => typeof(GradientControl);

    #endregion PROPERTIES

    #region CONSTRUCTOR

    public GradientControl()
    {
        SetCurrentValue(GradientProperty, new GradientDefinition());
    }

    #endregion CONSTRUCTOR

    #region BUTTON ACTIONS

    private void AddPointButton_Click(object? sender, RoutedEventArgs e)
    {
        var ordered = Gradient.Points.OrderBy(point => point.Position).ToArray();
        var position = 0.5;
        var largestGap = -1d;

        for (var index = 0; index < ordered.Length - 1; index++)
        {
            var gap = ordered[index + 1].Position - ordered[index].Position;

            if (gap <= largestGap)
                continue;

            largestGap = gap;
            position = ordered[index].Position + gap / 2;
        }

        var point = new GradientPoint
        {
            Color = new Models.Config.AccentColor("#0078D7", string.Empty),
            Position = position
        };

        Gradient.Points.Add(point);
        SelectedPoint = point;
    }

    private void RemovePointButton_Click(object? sender, RoutedEventArgs e)
    {
        if (SelectedPoint is null || Gradient.Points.Count <= 2)
            return;

        var index = Gradient.Points.IndexOf(SelectedPoint);
        Gradient.Points.Remove(SelectedPoint);
        SelectedPoint = Gradient.Points[Math.Clamp(index - 1, 0, Gradient.Points.Count - 1)];
    }

    #endregion BUTTON ACTIONS

    #region GRADIENT CONTROL

    private void AttachGradient(GradientDefinition gradient)
    {
        gradient.PropertyChanged += Gradient_PropertyChanged;
        gradient.Points.CollectionChanged += Points_CollectionChanged;

        foreach (var point in gradient.Points)
            point.PropertyChanged += Point_PropertyChanged;
    }

    private void DetachGradient(GradientDefinition gradient)
    {
        gradient.PropertyChanged -= Gradient_PropertyChanged;
        gradient.Points.CollectionChanged -= Points_CollectionChanged;

        foreach (var point in gradient.Points)
            point.PropertyChanged -= Point_PropertyChanged;
    }

    private void Rebuild()
    {
        GradientBrush = CreatePreviewBrush();

        if (_preview is not null)
            _preview.Background = GradientBrush;

        if (_stopsCanvas is not null
            && _stopsCanvas.Children.OfType<Thumb>().Count() != Gradient.Points.Count)
        {
            UpdateHandles();
        }
        else
        {
            UpdateHandlePositions();
        }

        UpdateHandleVisuals();
        UpdateButtons();
    }

    private void StopsCanvas_SizeChanged(object? sender, SizeChangedEventArgs e)
    {
        UpdateHandles();
    }

    private void StopThumb_DragDelta(object? sender, VectorEventArgs e)
    {
        if (sender is not Thumb { Tag: GradientPoint point } || _stopsCanvas is null)
            return;

        SelectedPoint = point;
        point.Position += e.Vector.X / Math.Max(1, _stopsCanvas.Bounds.Width);
    }

    private void StopThumb_PointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (sender is Thumb { Tag: GradientPoint point })
            SelectedPoint = point;
    }

    #endregion GRADIENT CONTROL

    #region PROPERTY CHANGES

    private void Gradient_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(GradientDefinition.Points))
        {
            DetachGradient(Gradient);
            AttachGradient(Gradient);
        }

        Rebuild();
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        var oldGradient = change.Property == GradientProperty
            ? change.GetOldValue<GradientDefinition?>()
            : null;

        base.OnPropertyChanged(change);

        if (change.Property != GradientProperty)
            return;

        if (oldGradient is not null)
            DetachGradient(oldGradient);

        AttachGradient(Gradient);
        SelectedPoint = Gradient.Points.FirstOrDefault();
        Rebuild();
    }

    private void Point_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        GradientBrush = CreatePreviewBrush();

        if (_preview is not null)
            _preview.Background = GradientBrush;

        UpdateHandlePositions();
        UpdateHandleVisuals();
    }

    private void Points_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.OldItems is not null)
        {
            foreach (GradientPoint point in e.OldItems)
                point.PropertyChanged -= Point_PropertyChanged;
        }

        if (e.NewItems is not null)
        {
            foreach (GradientPoint point in e.NewItems)
                point.PropertyChanged += Point_PropertyChanged;
        }

        Rebuild();
    }

    #endregion PROPERTY CHANGES

    #region TEMPLATE

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        DetachTemplateHandlers();
        base.OnApplyTemplate(e);

        _addPointButton = e.NameScope.Find<Button>("PART_AddPointButton");
        _preview = e.NameScope.Find<Border>("PART_Preview");
        _removePointButton = e.NameScope.Find<Button>("PART_RemovePointButton");
        _stopsCanvas = e.NameScope.Find<Canvas>("PART_StopsCanvas");

        if (_addPointButton is not null)
            _addPointButton.Click += AddPointButton_Click;

        if (_removePointButton is not null)
            _removePointButton.Click += RemovePointButton_Click;

        if (_stopsCanvas is not null)
            _stopsCanvas.SizeChanged += StopsCanvas_SizeChanged;

        SelectedPoint ??= Gradient.Points.FirstOrDefault();
        Rebuild();
    }

    private void DetachTemplateHandlers()
    {
        if (_addPointButton is not null)
            _addPointButton.Click -= AddPointButton_Click;

        if (_removePointButton is not null)
            _removePointButton.Click -= RemovePointButton_Click;

        if (_stopsCanvas is not null)
            _stopsCanvas.SizeChanged -= StopsCanvas_SizeChanged;

        _addPointButton = null;
        _preview = null;
        _removePointButton = null;
        _stopsCanvas = null;
    }

    private void UpdateButtons()
    {
        if (_removePointButton is not null)
            _removePointButton.IsEnabled = SelectedPoint is not null && Gradient.Points.Count > 2;
    }

    private void UpdateHandles()
    {
        if (_stopsCanvas is null)
            return;

        _stopsCanvas.Children.Clear();
        var width = Math.Max(0, _stopsCanvas.Bounds.Width);

        foreach (var point in Gradient.Points.OrderBy(point => point.Position))
        {
            var thumb = new Thumb
            {
                Background = new SolidColorBrush(point.Color.ToColor()),
                Classes = { "GradientPoint" },
                Tag = point
            };

            if (Application.Current is { } application
                && application.TryGetResource("GradientPoint.ThumbTheme", ActualThemeVariant, out var resource)
                && resource is ControlTheme theme)
            {
                thumb.Theme = theme;
            }

            if (ReferenceEquals(point, SelectedPoint))
                thumb.Classes.Add("Selected");

            thumb.DragDelta += StopThumb_DragDelta;
            thumb.PointerPressed += StopThumb_PointerPressed;
            Canvas.SetLeft(thumb, GetHandleLeft(point.Position, width));
            Canvas.SetTop(thumb, 0);
            _stopsCanvas.Children.Add(thumb);

            var positionText = new TextBlock
            {
                Classes = { "GradientPointPosition" },
                IsHitTestVisible = false,
                Tag = point,
                Text = FormatPosition(point.Position),
                TextAlignment = TextAlignment.Center,
                Width = 48
            };

            Canvas.SetLeft(positionText, GetPositionTextLeft(point.Position, width));
            Canvas.SetTop(positionText, 110);
            _stopsCanvas.Children.Add(positionText);
        }
    }

    private void UpdateHandlePositions()
    {
        if (_stopsCanvas is null)
            return;

        var width = Math.Max(0, _stopsCanvas.Bounds.Width);

        foreach (var thumb in _stopsCanvas.Children.OfType<Thumb>())
        {
            if (thumb.Tag is GradientPoint point)
                Canvas.SetLeft(thumb, GetHandleLeft(point.Position, width));
        }

        foreach (var positionText in _stopsCanvas.Children.OfType<TextBlock>())
        {
            if (positionText.Tag is not GradientPoint point)
                continue;

            positionText.Text = FormatPosition(point.Position);
            Canvas.SetLeft(positionText, GetPositionTextLeft(point.Position, width));
        }
    }

    private static double GetHandleLeft(double position, double width)
    {
        const double handleWidth = 18;
        return position * width - handleWidth / 2;
    }

    private static double GetPositionTextLeft(double position, double width)
    {
        const double textWidth = 48;
        return Math.Clamp(position * width - textWidth / 2, 0, Math.Max(0, width - textWidth));
    }

    private static string FormatPosition(double position)
    {
        return position.ToString("0.00", CultureInfo.InvariantCulture);
    }

    private IBrush CreatePreviewBrush()
    {
        var brush = new LinearGradientBrush
        {
            StartPoint = new RelativePoint(0, 0.5, RelativeUnit.Relative),
            EndPoint = new RelativePoint(1, 0.5, RelativeUnit.Relative),
            SpreadMethod = GradientSpreadMethod.Pad
        };

        foreach (var point in Gradient.Points.OrderBy(point => point.Position))
            brush.GradientStops.Add(new GradientStop(point.Color.ToColor(), point.Position));

        return brush;
    }

    private void UpdateHandleVisuals()
    {
        if (_stopsCanvas is null)
            return;

        foreach (var thumb in _stopsCanvas.Children.OfType<Thumb>())
        {
            if (thumb.Tag is not GradientPoint point)
                continue;

            thumb.Background = new SolidColorBrush(point.Color.ToColor());
            thumb.Classes.Set("Selected", ReferenceEquals(point, SelectedPoint));
        }
    }

    #endregion TEMPLATE
}
