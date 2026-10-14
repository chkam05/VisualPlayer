using System;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using Avalonia.Threading;
using Lucide.Avalonia;
using VisualPlayer.Enums.UI;
using VisualPlayer.Views.Base;

namespace VisualPlayer.Views.Dialogs;

[TemplatePart("PART_CancelButton", typeof(Button), IsRequired = true)]
[TemplatePart("PART_Input", typeof(TextBox), IsRequired = true)]
[TemplatePart("PART_OkButton", typeof(Button), IsRequired = true)]
public class InternalMessageInput : InternalMessageBase
{
    private Button? _cancelButton;
    private TextBox? _input;
    private Button? _okButton;

    private readonly TaskCompletionSource<InternalDialogResult> _resultSource
        = new(TaskCreationOptions.RunContinuationsAsynchronously);

    #region AVALONIA PROPERTIES

    public static readonly StyledProperty<string> DefaultValueProperty
        = AvaloniaProperty.Register<InternalMessageInput, string>(nameof(DefaultValue), string.Empty);

    public static readonly StyledProperty<bool> EnableCloseButtonProperty
        = AvaloniaProperty.Register<InternalMessageInput, bool>(nameof(EnableCloseButton), true);

    public static readonly StyledProperty<LucideIconKind> IconProperty
        = AvaloniaProperty.Register<InternalMessageInput, LucideIconKind>(nameof(Icon), LucideIconKind.MessageCircle);

    #endregion AVALONIA PROPERTIES

    #region PROPERTIES

    public string DefaultValue
    {
        get => GetValue(DefaultValueProperty);
        set => SetValue(DefaultValueProperty, value);
    }

    public bool EnableCloseButton
    {
        get => GetValue(EnableCloseButtonProperty);
        set => SetValue(EnableCloseButtonProperty, value);
    }

    public LucideIconKind Icon
    {
        get => GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }

    public Task<InternalDialogResult> ResultTask => _resultSource.Task;

    public string Value => DefaultValue.Trim();

    protected override Type StyleKeyOverride => typeof(InternalMessageInput);

    #endregion PROPERTIES

    #region CONSTRUCTORS

    public InternalMessageInput()
    {
        //
    }

    public InternalMessageInput(
        string title,
        string defaultValue = "",
        LucideIconKind icon = LucideIconKind.MessageCircle,
        bool enableCloseButton = true)
    {
        Title = title;
        DefaultValue = defaultValue;
        Icon = icon;
        EnableCloseButton = enableCloseButton;
    }

    #endregion CONSTRUCTORS

    #region TEMPLATE

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        DetachHandlers();
        base.OnApplyTemplate(e);

        _cancelButton = e.NameScope.Find<Button>("PART_CancelButton");
        _input = e.NameScope.Find<TextBox>("PART_Input");
        _okButton = e.NameScope.Find<Button>("PART_OkButton");

        if (_cancelButton is not null)
            _cancelButton.Click += CancelButton_Click;

        if (_okButton is not null)
            _okButton.Click += OkButton_Click;

        if (_input is not null)
        {
            Dispatcher.UIThread.Post(
                () =>
                {
                    _input.Focus();
                    _input.SelectAll();
                },
                DispatcherPriority.Input);
        }
    }

    private void DetachHandlers()
    {
        if (_cancelButton is not null)
            _cancelButton.Click -= CancelButton_Click;

        if (_okButton is not null)
            _okButton.Click -= OkButton_Click;

        _cancelButton = null;
        _input = null;
        _okButton = null;
    }

    #endregion TEMPLATE

    #region ACTIONS

    private void Complete(InternalDialogResult result)
    {
        _resultSource.TrySetResult(result);
        RequestClose();
    }

    protected override void OnClosed()
    {
        _resultSource.TrySetResult(InternalDialogResult.None);
    }

    #endregion ACTIONS

    #region BUTTON ACTIONS

    private void CancelButton_Click(object? sender, RoutedEventArgs e)
    {
        Complete(InternalDialogResult.Cancel);
    }

    private void OkButton_Click(object? sender, RoutedEventArgs e)
    {
        Complete(InternalDialogResult.Ok);
    }

    #endregion BUTTON ACTIONS
}