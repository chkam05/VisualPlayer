using System;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Media;
using VisualPlayer.Core;
using VisualPlayer.Enums;

namespace VisualPlayer.Controls;

public class LyricsControl : TemplatedControl
{
    private const int SIDE_TEXT_MAXIMUM_LENGTH = 48;

    public static readonly StyledProperty<double> BackgroundOpacityProperty
        = AvaloniaProperty.Register<LyricsControl, double>(nameof(BackgroundOpacity), 0.85);

    public static readonly StyledProperty<bool> DisplayLyricsProperty
        = AvaloniaProperty.Register<LyricsControl, bool>(nameof(DisplayLyrics), true);

    public static readonly StyledProperty<LyricsCore?> LyricsCoreProperty
        = AvaloniaProperty.Register<LyricsControl, LyricsCore?>(nameof(LyricsCore));

    public static readonly DirectProperty<LyricsControl, bool> HasLyricsProperty
        = AvaloniaProperty.RegisterDirect<LyricsControl, bool>(nameof(HasLyrics), control => control.HasLyrics);

    public static readonly DirectProperty<LyricsControl, bool> IsPlainTextProperty
        = AvaloniaProperty.RegisterDirect<LyricsControl, bool>(nameof(IsPlainText), control => control.IsPlainText);

    public static readonly DirectProperty<LyricsControl, bool> IsTimedTextProperty
        = AvaloniaProperty.RegisterDirect<LyricsControl, bool>(nameof(IsTimedText), control => control.IsTimedText);

    public static readonly DirectProperty<LyricsControl, string> PlainTextProperty
        = AvaloniaProperty.RegisterDirect<LyricsControl, string>(nameof(PlainText), control => control.PlainText);

    public static readonly DirectProperty<LyricsControl, string> PreviousTextProperty
        = AvaloniaProperty.RegisterDirect<LyricsControl, string>(nameof(PreviousText), control => control.PreviousText);

    public static readonly DirectProperty<LyricsControl, string> CurrentTextProperty
        = AvaloniaProperty.RegisterDirect<LyricsControl, string>(nameof(CurrentText), control => control.CurrentText);

    public static readonly DirectProperty<LyricsControl, string> NextTextProperty
        = AvaloniaProperty.RegisterDirect<LyricsControl, string>(nameof(NextText), control => control.NextText);

    private bool _hasLyrics;
    private bool _isPlainText;
    private bool _isTimedText;
    private string _plainText = string.Empty;
    private string _previousText = string.Empty;
    private string _currentText = string.Empty;
    private string _nextText = string.Empty;
    private int _animationVersion;
    private TextBlock? _previousLine;
    private TextBlock? _currentLine;
    private TextBlock? _nextLine;
    private TextBlock? _incomingLine;

    static LyricsControl()
    {
        LyricsCoreProperty.Changed.AddClassHandler<LyricsControl>((control, args) =>
            control.ChangeCore(args.OldValue as LyricsCore, args.NewValue as LyricsCore));
        DisplayLyricsProperty.Changed.AddClassHandler<LyricsControl>((control, _) => control.Refresh());
    }

    public double BackgroundOpacity
    {
        get => GetValue(BackgroundOpacityProperty);
        set => SetValue(BackgroundOpacityProperty, value);
    }

    public bool DisplayLyrics
    {
        get => GetValue(DisplayLyricsProperty);
        set => SetValue(DisplayLyricsProperty, value);
    }

    public LyricsCore? LyricsCore
    {
        get => GetValue(LyricsCoreProperty);
        set => SetValue(LyricsCoreProperty, value);
    }

    public bool HasLyrics => _hasLyrics;
    public bool IsPlainText => _isPlainText;
    public bool IsTimedText => _isTimedText;
    public string PlainText => _plainText;
    public string PreviousText => _previousText;
    public string CurrentText => _currentText;
    public string NextText => _nextText;

    private void ChangeCore(LyricsCore? oldCore, LyricsCore? newCore)
    {
        if (oldCore is not null)
            oldCore.PropertyChanged -= LyricsCore_PropertyChanged;

        if (newCore is not null)
            newCore.PropertyChanged += LyricsCore_PropertyChanged;

        Refresh();
    }

    private void LyricsCore_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(VisualPlayer.Core.LyricsCore.CurrentLine))
        {
            AnimateLineChange();
        }
        else if (e.PropertyName == nameof(VisualPlayer.Core.LyricsCore.CurrentDocument))
        {
            Refresh();
        }
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        _previousLine = e.NameScope.Find<TextBlock>("PART_PreviousLine");
        _currentLine = e.NameScope.Find<TextBlock>("PART_CurrentLine");
        _nextLine = e.NameScope.Find<TextBlock>("PART_NextLine");
        _incomingLine = e.NameScope.Find<TextBlock>("PART_IncomingLine");
        Refresh();
    }

    private async void AnimateLineChange()
    {
        var version = ++_animationVersion;

        if (_previousLine?.RenderTransform is not TranslateTransform previousTransform
            || _currentLine?.RenderTransform is not TranslateTransform currentTransform
            || _nextLine?.RenderTransform is not TranslateTransform nextTransform
            || _incomingLine?.RenderTransform is not TranslateTransform incomingTransform)
        {
            Refresh();
            return;
        }

        var previousCenter = GetHorizontalCenter(_previousLine);
        var currentCenter = GetHorizontalCenter(_currentLine);
        var nextCenter = GetHorizontalCenter(_nextLine);
        var previousDistance = Math.Max(80, currentCenter - previousCenter);
        var nextDistance = Math.Max(80, nextCenter - currentCenter);
        var (_, _, incomingText) = GetTimedTexts();
        var incomingTransformTransitions = incomingTransform.Transitions;
        incomingTransform.Transitions = null;
        incomingTransform.X = nextDistance;
        _incomingLine.Text = incomingText;
        _incomingLine.Opacity = 0;
        await Task.Yield();
        incomingTransform.Transitions = incomingTransformTransitions;

        previousTransform.X = -previousDistance;
        currentTransform.X = -previousDistance;
        nextTransform.X = -nextDistance;
        incomingTransform.X = 0;
        _previousLine.Opacity = 0;
        _currentLine.Opacity = 0.5;
        _currentLine.SetCurrentValue(TextBlock.FontSizeProperty, (double)LyricsCore!.Config.SurroundingLineFontSize);
        _nextLine.Opacity = 1;
        _nextLine.SetCurrentValue(TextBlock.FontSizeProperty, (double)LyricsCore.Config.CurrentLineFontSize);
        _incomingLine.Opacity = 0.5;

        await Task.Delay(240);

        if (version != _animationVersion)
            return;

        var previousTransitions = previousTransform.Transitions;
        var currentTransitions = currentTransform.Transitions;
        var nextTransitions = nextTransform.Transitions;
        var previousControlTransitions = _previousLine.Transitions;
        var currentControlTransitions = _currentLine.Transitions;
        var nextControlTransitions = _nextLine.Transitions;
        var incomingControlTransitions = _incomingLine.Transitions;
        previousTransform.Transitions = null;
        currentTransform.Transitions = null;
        nextTransform.Transitions = null;
        incomingTransform.Transitions = null;
        _previousLine.Transitions = null;
        _currentLine.Transitions = null;
        _nextLine.Transitions = null;
        _incomingLine.Transitions = null;

        Refresh();
        previousTransform.X = 0;
        currentTransform.X = 0;
        nextTransform.X = 0;
        incomingTransform.X = nextDistance;
        _previousLine.Opacity = 0.5;
        _previousLine.SetCurrentValue(TextBlock.FontSizeProperty, (double)LyricsCore.Config.SurroundingLineFontSize);
        _currentLine.Opacity = 1;
        _currentLine.SetCurrentValue(TextBlock.FontSizeProperty, (double)LyricsCore.Config.CurrentLineFontSize);
        _nextLine.Opacity = 0.5;
        _nextLine.SetCurrentValue(TextBlock.FontSizeProperty, (double)LyricsCore.Config.SurroundingLineFontSize);
        _incomingLine.Opacity = 0;
        _incomingLine.Text = string.Empty;

        await Task.Yield();

        previousTransform.Transitions = previousTransitions;
        currentTransform.Transitions = currentTransitions;
        nextTransform.Transitions = nextTransitions;
        incomingTransform.Transitions = incomingTransformTransitions;
        _previousLine.Transitions = previousControlTransitions;
        _currentLine.Transitions = currentControlTransitions;
        _nextLine.Transitions = nextControlTransitions;
        _incomingLine.Transitions = incomingControlTransitions;
    }

    private void Refresh()
    {
        var document = LyricsCore?.CurrentDocument;
        var lines = document?.Lines.OrderBy(line => line.Start).ToArray() ?? [];
        var isPlain = document?.Type == LyricsType.Plain;
        var (previousText, currentText, nextText) = GetTimedTexts(lines);

        SetAndRaise(HasLyricsProperty, ref _hasLyrics, DisplayLyrics && lines.Length > 0);
        SetAndRaise(IsPlainTextProperty, ref _isPlainText, DisplayLyrics && isPlain && lines.Length > 0);
        SetAndRaise(IsTimedTextProperty, ref _isTimedText, DisplayLyrics && !isPlain && lines.Length > 0);
        SetAndRaise(PlainTextProperty, ref _plainText, string.Join(Environment.NewLine, lines.Select(line => line.Text)));
        SetAndRaise(PreviousTextProperty, ref _previousText, previousText);
        SetAndRaise(CurrentTextProperty, ref _currentText, currentText);
        SetAndRaise(NextTextProperty, ref _nextText, nextText);
    }

    private (string Previous, string Current, string Next) GetTimedTexts()
    {
        var lines = LyricsCore?.CurrentDocument?.Lines
            .OrderBy(line => line.Start)
            .ToArray() ?? [];
        return GetTimedTexts(lines);
    }

    private (string Previous, string Current, string Next) GetTimedTexts(
        VisualPlayer.ViewModels.Lyrics.LyricsLine[] lines)
    {
        var currentIndex = LyricsCore?.CurrentLine is { } current
            ? Array.IndexOf(lines, current)
            : -1;

        return (
            currentIndex > 0 ? FormatPreviousText(lines[currentIndex - 1].Text) : string.Empty,
            currentIndex >= 0 ? lines[currentIndex].Text : string.Empty,
            currentIndex >= 0 && currentIndex + 1 < lines.Length
                ? FormatNextText(lines[currentIndex + 1].Text)
                : currentIndex < 0 && lines.Length > 0
                    ? FormatNextText(lines[0].Text)
                    : string.Empty);
    }

    private static string FormatPreviousText(string text)
    {
        return text.Length > SIDE_TEXT_MAXIMUM_LENGTH
            ? $"…{text[^SIDE_TEXT_MAXIMUM_LENGTH..]}"
            : text;
    }

    private static string FormatNextText(string text)
    {
        return text.Length > SIDE_TEXT_MAXIMUM_LENGTH
            ? $"{text[..SIDE_TEXT_MAXIMUM_LENGTH]}…"
            : text;
    }

    private double GetHorizontalCenter(Control control)
    {
        var origin = control.TranslatePoint(default, this)?.X ?? 0;
        return origin + control.Bounds.Width / 2;
    }
}
