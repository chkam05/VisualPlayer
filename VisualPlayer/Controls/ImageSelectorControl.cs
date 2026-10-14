using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Interactivity;
using VisualPlayer.Events;
using VisualPlayer.ViewModels.Config;

namespace VisualPlayer.Controls;

[TemplatePart("PART_AddImageButton", typeof(Button), IsRequired = true)]
[TemplatePart("PART_ImageList", typeof(ListBox), IsRequired = true)]
public class ImageSelectorControl : TemplatedControl
{
    #region CONST

    private static readonly string[] DEFAULT_IMAGE_PATHS =
    [
        "avares://VisualPlayer/Assets/Backgrounds/BlueGlow.png",
        "avares://VisualPlayer/Assets/Backgrounds/Glass.png",
        "avares://VisualPlayer/Assets/Backgrounds/Music.png",
        "avares://VisualPlayer/Assets/Backgrounds/LowPoly.png",
        "avares://VisualPlayer/Assets/Backgrounds/Space.png"
    ];

    #endregion CONST

    private Button? _addImageButton;
    private bool _isSynchronizingSelection;
    private ListBox? _imageList;

    #region AVALONIA PROPERTIES

    public static readonly StyledProperty<ObservableCollection<string>> CustomImagesProperty
        = AvaloniaProperty.Register<ImageSelectorControl, ObservableCollection<string>>(nameof(CustomImages));

    public static readonly StyledProperty<string?> SelectedImageProperty
        = AvaloniaProperty.Register<ImageSelectorControl, string?>(nameof(SelectedImage), defaultBindingMode: BindingMode.TwoWay);

    #endregion AVALONIA PROPERTIES

    #region EVENTS

    public event EventHandler? AddImageRequested;
    public event EventHandler<BackgroundImageRemovedEventArgs>? ImageRemoved;
    public event EventHandler<BackgroundImageChangedEventArgs>? SelectionChanged;

    #endregion EVENTS

    #region PROPERTIES

    public ObservableCollection<string> CustomImages
    {
        get => GetValue(CustomImagesProperty);
        set => SetValue(CustomImagesProperty, value);
    }

    public ObservableCollection<BackgroundImageViewModel> Images { get; } = new();

    public string? SelectedImage
    {
        get => GetValue(SelectedImageProperty);
        set => SetValue(SelectedImageProperty, value);
    }

    protected override Type StyleKeyOverride => typeof(ImageSelectorControl);

    #endregion PROPERTIES

    #region CONSTRUCTORS

    public ImageSelectorControl()
    {
        CustomImages = new ObservableCollection<string>();
        RebuildImages();

        AddHandler(Button.ClickEvent, RemoveImageButton_Click);
    }

    #endregion CONSTRUCTORS

    #region ACTIONS

    public void AddImage(string imagePath)
    {
        if (string.IsNullOrWhiteSpace(imagePath))
            return;

        var existing = CustomImages.FirstOrDefault(path => PathsEqual(path, imagePath));

        if (existing is null)
        {
            CustomImages.Add(imagePath);
            existing = imagePath;
        }

        SetCurrentValue(SelectedImageProperty, existing);
    }

    private void AddImageButton_Click(object? sender, RoutedEventArgs e)
    {
        OnAddImageRequested();
    }

    private void ImageList_SelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (_isSynchronizingSelection || _imageList?.SelectedItem is not BackgroundImageViewModel image)
            return;

        SetCurrentValue(SelectedImageProperty, image.Path);
    }

    private void RemoveImageButton_Click(object? sender, RoutedEventArgs e)
    {
        if (e.Source is not Button { Tag: BackgroundImageViewModel image } || !image.CanRemove)
            return;

        var path = CustomImages.FirstOrDefault(item => PathsEqual(item, image.Path));

        if (path is null)
            return;

        CustomImages.Remove(path);

        if (PathsEqual(SelectedImage, path))
            SetCurrentValue(SelectedImageProperty, null);

        OnImageRemoved(path);

        e.Handled = true;
    }

    protected virtual void OnAddImageRequested()
    {
        AddImageRequested?.Invoke(this, EventArgs.Empty);
    }

    protected virtual void OnSelectionChanged(string imagePath)
    {
        SelectionChanged?.Invoke(this, new BackgroundImageChangedEventArgs(imagePath));
    }

    protected virtual void OnImageRemoved(string imagePath)
    {
        ImageRemoved?.Invoke(this, new BackgroundImageRemovedEventArgs(imagePath));
    }

    #endregion ACTIONS

    #region PROPERTY CHANGE HANDLING

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == CustomImagesProperty)
        {
            if (change.GetOldValue<ObservableCollection<string>>() is { } oldCollection)
                oldCollection.CollectionChanged -= CustomImages_CollectionChanged;

            if (change.GetNewValue<ObservableCollection<string>>() is { } newCollection)
                newCollection.CollectionChanged += CustomImages_CollectionChanged;

            RebuildImages();
            return;
        }

        if (change.Property != SelectedImageProperty)
            return;

        var imagePath = change.GetNewValue<string?>();
        SynchronizeSelection(imagePath);

        if (!string.IsNullOrWhiteSpace(imagePath))
            OnSelectionChanged(imagePath);
    }

    private void CustomImages_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        RebuildImages();
    }

    #endregion PROPERTY CHANGE HANDLING

    #region ITEMS

    private void RebuildImages()
    {
        Images.Clear();

        foreach (var path in DEFAULT_IMAGE_PATHS)
            Images.Add(new BackgroundImageViewModel(path, true));

        foreach (var path in CustomImages.Where(path => !string.IsNullOrWhiteSpace(path)))
            Images.Add(new BackgroundImageViewModel(path, false));

        SynchronizeSelection(SelectedImage);
    }

    private void SynchronizeSelection(string? imagePath)
    {
        if (_imageList is null)
            return;

        _isSynchronizingSelection = true;

        try
        {
            _imageList.SelectedItem = string.IsNullOrWhiteSpace(imagePath)
                ? null
                : Images.FirstOrDefault(item => PathsEqual(item.Path, imagePath));
        }
        finally
        {
            _isSynchronizingSelection = false;
        }
    }

    private static bool PathsEqual(string? first, string? second)
    {
        return string.Equals(first, second, StringComparison.OrdinalIgnoreCase);
    }

    #endregion ITEMS

    #region TEMPLATE

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        DetachHandlers();
        base.OnApplyTemplate(e);

        _addImageButton = e.NameScope.Find<Button>("PART_AddImageButton");
        _imageList = e.NameScope.Find<ListBox>("PART_ImageList");

        if (_addImageButton is not null)
            _addImageButton.Click += AddImageButton_Click;

        if (_imageList is not null)
            _imageList.SelectionChanged += ImageList_SelectionChanged;

        SynchronizeSelection(SelectedImage);
    }

    private void DetachHandlers()
    {
        if (_addImageButton is not null)
            _addImageButton.Click -= AddImageButton_Click;

        if (_imageList is not null)
            _imageList.SelectionChanged -= ImageList_SelectionChanged;

        _addImageButton = null;
        _imageList = null;
    }

    #endregion TEMPLATE
}
