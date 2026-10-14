using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using Lucide.Avalonia;
using VisualPlayer.Contexts;
using VisualPlayer.Enums;
using VisualPlayer.Enums.UI;
using VisualPlayer.Interfaces;
using VisualPlayer.ViewModels.Lyrics;
using VisualPlayer.Views.Base;
using VisualPlayer.Views.Dialogs;
using VisualPlayer.Windows;

namespace VisualPlayer.Views;

public partial class InternalViewLyricsEditor : InternalViewBase, IInternalCloseGuard
{
    private static readonly JsonSerializerOptions SnapshotJsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    private readonly Stack<string> _redoHistory = new();
    private readonly Stack<string> _undoHistory = new();
    private bool _canRedo;
    private bool _canSave;
    private bool _canUndo;
    private string _currentSnapshot = string.Empty;
    private bool _isApplyingSnapshot;
    private bool _isDirty;
    private bool _isPositionSeeking;
    private bool _isPositionTextEditing;
    private bool _isPlaybackAnchorLocked;
    private double? _playbackAnchor;
    private double _playbackSpeed = Core.PlayerCore.PLAYBACK_SPEED_NORMAL;
    private string _positionText = "00:00.00";
    private string _savedSnapshot = string.Empty;

    #region AVALONIA PROPERTIES

    public static readonly DirectProperty<InternalViewLyricsEditor, bool> CanRedoProperty
        = AvaloniaProperty.RegisterDirect<InternalViewLyricsEditor, bool>(nameof(CanRedo), view => view.CanRedo);

    public static readonly DirectProperty<InternalViewLyricsEditor, bool> CanSaveProperty
        = AvaloniaProperty.RegisterDirect<InternalViewLyricsEditor, bool>(nameof(CanSave), view => view.CanSave);

    public static readonly DirectProperty<InternalViewLyricsEditor, bool> CanUndoProperty
        = AvaloniaProperty.RegisterDirect<InternalViewLyricsEditor, bool>(nameof(CanUndo), view => view.CanUndo);

    public static readonly DirectProperty<InternalViewLyricsEditor, bool> IsDirtyProperty
        = AvaloniaProperty.RegisterDirect<InternalViewLyricsEditor, bool>(nameof(IsDirty), view => view.IsDirty);

    public static readonly DirectProperty<InternalViewLyricsEditor, double> PlaybackSpeedProperty
        = AvaloniaProperty.RegisterDirect<InternalViewLyricsEditor, double>(
            nameof(PlaybackSpeed),
            view => view.PlaybackSpeed,
            (view, value) => view.PlaybackSpeed = value);

    public static readonly DirectProperty<InternalViewLyricsEditor, bool> IsPlaybackAnchorLockedProperty
        = AvaloniaProperty.RegisterDirect<InternalViewLyricsEditor, bool>(
            nameof(IsPlaybackAnchorLocked),
            view => view.IsPlaybackAnchorLocked);

    public static readonly DirectProperty<InternalViewLyricsEditor, string> PositionTextProperty
        = AvaloniaProperty.RegisterDirect<InternalViewLyricsEditor, string>(
            nameof(PositionText),
            view => view.PositionText,
            (view, value) => view.PositionText = value);

    #endregion AVALONIA PROPERTIES

    #region PROPERTIES

    public bool CanRedo
    {
        get => _canRedo;
        private set => SetAndRaise(CanRedoProperty, ref _canRedo, value);
    }

    public bool CanSave
    {
        get => _canSave;
        private set => SetAndRaise(CanSaveProperty, ref _canSave, value);
    }

    public bool CanUndo
    {
        get => _canUndo;
        private set => SetAndRaise(CanUndoProperty, ref _canUndo, value);
    }

    public LyricsDocument Document { get; }

    public bool IsDirty
    {
        get => _isDirty;
        private set => SetAndRaise(IsDirtyProperty, ref _isDirty, value);
    }

    public bool IsPlaybackAnchorLocked
    {
        get => _isPlaybackAnchorLocked;
        private set
        {
            if (_isPlaybackAnchorLocked == value)
                return;

            SetAndRaise(IsPlaybackAnchorLockedProperty, ref _isPlaybackAnchorLocked, value);
        }
    }

    public IReadOnlyList<double> PlaybackSpeeds { get; } = [0.5, 0.75, 1, 1.25, 1.5, 2];

    public double PlaybackSpeed
    {
        get => _playbackSpeed;
        set
        {
            var speed = Math.Clamp(value, Core.PlayerCore.PLAYBACK_SPEED_MINIMUM, Core.PlayerCore.PLAYBACK_SPEED_MAXIMUM);

            if (Math.Abs(_playbackSpeed - speed) < double.Epsilon)
                return;

            SetAndRaise(PlaybackSpeedProperty, ref _playbackSpeed, speed);
            ApplicationContext.Instance.PlayerCore.PlaybackSpeed = speed;
        }
    }

    public string PositionText
    {
        get => _positionText;
        set => SetAndRaise(PositionTextProperty, ref _positionText, value ?? string.Empty);
    }

    public IReadOnlyList<LyricsType> LyricsTypes { get; } = Enum.GetValues<LyricsType>();

    #endregion PROPERTIES

    #region CONSTRUCTORS

    public InternalViewLyricsEditor()
        : this(new LyricsDocument())
    {
    }

    public InternalViewLyricsEditor(LyricsDocument document)
    {
        Document = document;
        Title = "Edytor tekstu";
        InitializeComponent();

        PART_PositionSlider.AddHandler(
            InputElement.PointerPressedEvent,
            PositionSlider_PointerPressed,
            RoutingStrategies.Tunnel,
            handledEventsToo: true);
        PART_PositionSlider.AddHandler(
            InputElement.PointerReleasedEvent,
            PositionSlider_PointerReleased,
            RoutingStrategies.Bubble,
            handledEventsToo: true);
        PART_PositionSlider.AddHandler(
            InputElement.PointerCaptureLostEvent,
            PositionSlider_PointerCaptureLost,
            RoutingStrategies.Bubble,
            handledEventsToo: true);
        ApplicationContext.Instance.PlayerDataContext.PropertyChanged += PlayerDataContext_PropertyChanged;
        UpdatePositionText();
        SubscribeDocument();
        _currentSnapshot = CaptureSnapshot();
        _savedSnapshot = _currentSnapshot;
        UpdateEditorState();
    }

    #endregion CONSTRUCTORS

    #region BUTTON ACTIONS

    private void AddLineButton_Click(object? sender, RoutedEventArgs e)
    {
        Document.Lines.Add(new LyricsLine { Start = GetCurrentPosition() });
        SortLines();
    }

    private void CopyAtCurrentTimeMenuItem_Click(object? sender, RoutedEventArgs e)
    {
        if (GetLine(sender) is not { } line)
            return;

        Document.Lines.Add(new LyricsLine
        {
            Start = GetCurrentPosition(),
            Text = line.Text
        });
        SortLines();
    }

    private void DeleteLineMenuItem_Click(object? sender, RoutedEventArgs e)
    {
        if (GetLine(sender) is { } line)
            Document.Lines.Remove(line);
    }

    private void OptionsButton_Click(object? sender, RoutedEventArgs e)
    {
        if (sender is Button button)
            button.ContextMenu?.Open(button);
    }

    private void PlayPauseButton_Click(object? sender, RoutedEventArgs e)
    {
        ApplicationContext.Instance.PlayerCore.TogglePlayPause();
    }

    private void SetPlaybackAnchorButton_Click(object? sender, RoutedEventArgs e)
    {
        _playbackAnchor = ApplicationContext.Instance.PlayerCore.Position;
        IsPlaybackAnchorLocked = true;
    }

    private void RemovePlaybackAnchorButton_Click(object? sender, RoutedEventArgs e)
    {
        _playbackAnchor = null;
        IsPlaybackAnchorLocked = false;
    }

    private void ReturnToPlaybackAnchorButton_Click(object? sender, RoutedEventArgs e)
    {
        if (_playbackAnchor is { } anchor)
            ApplicationContext.Instance.PlayerCore.Seek(anchor);
    }

    private void StopButton_Click(object? sender, RoutedEventArgs e)
    {
        ApplicationContext.Instance.PlayerCore.Stop();
    }

    private void RedoButton_Click(object? sender, RoutedEventArgs e)
    {
        if (!_redoHistory.TryPop(out var snapshot))
            return;

        _undoHistory.Push(_currentSnapshot);
        ApplySnapshot(snapshot);
    }

    private void SaveButton_Click(object? sender, RoutedEventArgs e)
    {
        SaveDocument();
    }

    private void SeekButton_Click(object? sender, RoutedEventArgs e)
    {
        if (sender is not Button { Tag: string action })
            return;

        var direction = action.Length > 0 && action[0] == '-' ? -1 : 1;
        var intervalName = action.TrimStart('-', '+');
        var config = ApplicationContext.Instance.Config.Lyrics;
        var interval = intervalName switch
        {
            "Short" => config.SeekShortInterval,
            "Medium" => config.SeekMediumInterval,
            "Long" => config.SeekLongInterval,
            _ => 0
        };
        var player = ApplicationContext.Instance.PlayerCore;
        player.Seek(Math.Clamp(player.Position + direction * interval, 0, player.Duration));
    }

    private void SetCurrentTimeMenuItem_Click(object? sender, RoutedEventArgs e)
    {
        if (GetLine(sender) is { } line)
        {
            line.Start = GetCurrentPosition();
            SortLines();
        }
    }

    private void UndoButton_Click(object? sender, RoutedEventArgs e)
    {
        if (!_undoHistory.TryPop(out var snapshot))
            return;

        _redoHistory.Push(_currentSnapshot);
        ApplySnapshot(snapshot);
    }

    private void PositionSlider_PointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        CompletePositionSeek();
    }

    private void PositionSlider_PointerPressed(object? sender, PointerPressedEventArgs e)
    {
        _isPositionSeeking = true;
    }

    private void PositionSlider_PointerCaptureLost(object? sender, PointerCaptureLostEventArgs e)
    {
        CompletePositionSeek();
    }

    private void PositionSlider_ValueChanged(object? sender, RangeBaseValueChangedEventArgs e)
    {
        if (_isPositionSeeking)
            ApplicationContext.Instance.PlayerCore.Seek(e.NewValue);
    }

    private void CompletePositionSeek()
    {
        if (!_isPositionSeeking)
            return;

        _isPositionSeeking = false;
        ApplicationContext.Instance.PlayerCore.Seek(PART_PositionSlider.Value);
    }

    private void PositionTextBox_GotFocus(object? sender, RoutedEventArgs e)
    {
        _isPositionTextEditing = true;
    }

    private void PositionTextBox_KeyDown(object? sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter)
        {
            CommitPositionText();
            PART_PositionTextBox.Focus(NavigationMethod.Unspecified);
            e.Handled = true;
        }
        else if (e.Key == Key.Escape)
        {
            _isPositionTextEditing = false;
            UpdatePositionText();
            e.Handled = true;
        }
    }

    private void PositionTextBox_LostFocus(object? sender, RoutedEventArgs e)
    {
        CommitPositionText();
    }

    private void CommitPositionText()
    {
        _isPositionTextEditing = false;

        if (TryParsePosition(PositionText, out var position))
            ApplicationContext.Instance.PlayerCore.Seek(position.TotalSeconds);

        UpdatePositionText();
    }

    private void PlayerDataContext_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(PlayerDataContext.Position) && !_isPositionTextEditing)
            UpdatePositionText();
    }

    private void UpdatePositionText()
    {
        PositionText = FormatPosition(ApplicationContext.Instance.PlayerCore.Position);
    }

    #endregion BUTTON ACTIONS

    #region HISTORY

    private void ApplySnapshot(string snapshot)
    {
        var restored = JsonSerializer.Deserialize<LyricsDocument>(snapshot, SnapshotJsonOptions);

        if (restored is null)
            return;

        _isApplyingSnapshot = true;
        Document.Lines.CollectionChanged -= Lines_CollectionChanged;
        UnsubscribeLines(Document.Lines);
        Document.Name = restored.Name;
        Document.Type = restored.Type;
        Document.Lines = restored.Lines;
        Document.Lines.CollectionChanged += Lines_CollectionChanged;
        SubscribeLines(Document.Lines);
        _currentSnapshot = snapshot;
        _isApplyingSnapshot = false;
        UpdateEditorState();
    }

    private string CaptureSnapshot()
    {
        return JsonSerializer.Serialize(Document, SnapshotJsonOptions);
    }

    private void RecordChange()
    {
        if (_isApplyingSnapshot)
            return;

        var snapshot = CaptureSnapshot();

        if (snapshot != _currentSnapshot)
        {
            _undoHistory.Push(_currentSnapshot);
            _redoHistory.Clear();
            _currentSnapshot = snapshot;
        }

        UpdateEditorState();
    }

    private void UpdateEditorState()
    {
        CanRedo = _redoHistory.Count > 0;
        CanUndo = _undoHistory.Count > 0;
        IsDirty = _currentSnapshot != _savedSnapshot;
        CanSave = IsDirty && Document.Lines.All(line => !line.HasTimeError);
        Title = IsDirty ? "Edytor tekstu *" : "Edytor tekstu";
    }

    #endregion HISTORY

    #region CHANGES

    private void Document_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        RecordChange();
    }

    private void Line_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        RecordChange();
    }

    private void Lines_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.OldItems is not null)
            UnsubscribeLines(e.OldItems.Cast<LyricsLine>());
        if (e.NewItems is not null)
            SubscribeLines(e.NewItems.Cast<LyricsLine>());
        RecordChange();
    }

    private void SubscribeDocument()
    {
        Document.PropertyChanged += Document_PropertyChanged;
        Document.Lines.CollectionChanged += Lines_CollectionChanged;
        SubscribeLines(Document.Lines);
    }

    private void SubscribeLines(IEnumerable<LyricsLine> lines)
    {
        foreach (var line in lines)
            line.PropertyChanged += Line_PropertyChanged;
    }

    private void UnsubscribeDocument()
    {
        Document.PropertyChanged -= Document_PropertyChanged;
        Document.Lines.CollectionChanged -= Lines_CollectionChanged;
        UnsubscribeLines(Document.Lines);
    }

    private void UnsubscribeLines(IEnumerable<LyricsLine> lines)
    {
        foreach (var line in lines)
            line.PropertyChanged -= Line_PropertyChanged;
    }

    #endregion CHANGES

    #region DOCUMENT

    public async Task<bool> CanCloseAsync()
    {
        if (!IsDirty)
            return true;
        if (TopLevel.GetTopLevel(this) is not MainWindow window)
            return false;

        var dialog = new InternalMessageDialog(
            "Niezapisane zmiany",
            "Tekst zawiera niezapisane zmiany. Czy chcesz zamknąć edytor bez zapisywania?",
            InternalDialogButtons.YesNo,
            LucideIconKind.TriangleAlert);
        return await window.ShowDialogAsync(dialog) == InternalDialogResult.Yes;
    }

    private void SaveDocument()
    {
        if (Document.Lines.Any(line => line.HasTimeError))
            return;

        SortLines();
        ApplicationContext.Instance.LyricsCore.Save(Document);
        _currentSnapshot = CaptureSnapshot();
        _savedSnapshot = _currentSnapshot;
        UpdateEditorState();
    }

    #endregion DOCUMENT

    #region HELPERS

    private TimeSpan GetCurrentPosition()
    {
        return TimeSpan.FromSeconds(ApplicationContext.Instance.PlayerCore.Position);
    }

    private static string FormatPosition(double totalSeconds)
    {
        var time = TimeSpan.FromSeconds(Math.Max(0, totalSeconds));
        return string.Create(
            CultureInfo.InvariantCulture,
            $"{(int)time.TotalMinutes:00}:{time.Seconds:00}.{time.Milliseconds / 10:00}");
    }

    private static bool TryParsePosition(string value, out TimeSpan result)
    {
        var match = Regex.Match(
            value.Trim(),
            @"^(?<minutes>\d{1,4}):(?<seconds>[0-5]\d)(?:[\.:](?<fraction>\d{1,3}))?$",
            RegexOptions.CultureInvariant);

        if (!match.Success)
        {
            result = default;
            return false;
        }

        var minutes = int.Parse(match.Groups["minutes"].Value, CultureInfo.InvariantCulture);
        var seconds = int.Parse(match.Groups["seconds"].Value, CultureInfo.InvariantCulture);
        var fractionText = match.Groups["fraction"].Value;
        var milliseconds = fractionText.Length switch
        {
            0 => 0,
            1 => int.Parse(fractionText, CultureInfo.InvariantCulture) * 100,
            2 => int.Parse(fractionText, CultureInfo.InvariantCulture) * 10,
            _ => int.Parse(fractionText[..3], CultureInfo.InvariantCulture)
        };

        result = TimeSpan.FromMinutes(minutes)
            + TimeSpan.FromSeconds(seconds)
            + TimeSpan.FromMilliseconds(milliseconds);
        return true;
    }

    private static LyricsLine? GetLine(object? sender)
    {
        return sender is MenuItem { DataContext: LyricsLine line } ? line : null;
    }

    private void SortLines()
    {
        var ordered = Document.Lines.OrderBy(line => line.Start).ToArray();

        if (ordered.SequenceEqual(Document.Lines))
            return;

        _isApplyingSnapshot = true;
        Document.Lines.Clear();
        foreach (var line in ordered)
            Document.Lines.Add(line);
        _isApplyingSnapshot = false;
        RecordChange();
    }

    #endregion HELPERS

    #region VIEW ACTIONS

    protected override void OnClosed()
    {
        UnsubscribeDocument();
        ApplicationContext.Instance.PlayerDataContext.PropertyChanged -= PlayerDataContext_PropertyChanged;
        ApplicationContext.Instance.PlayerCore.PlaybackSpeed = Core.PlayerCore.PLAYBACK_SPEED_NORMAL;
        base.OnClosed();
    }

    #endregion VIEW ACTIONS
}
