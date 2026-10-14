using System;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using Lucide.Avalonia;
using VisualPlayer.Enums.UI;
using VisualPlayer.Views.Base;

namespace VisualPlayer.Views.Dialogs;

[TemplatePart("PART_CancelButton", typeof(Button), IsRequired = true)]
[TemplatePart("PART_NoButton", typeof(Button), IsRequired = true)]
[TemplatePart("PART_OkButton", typeof(Button), IsRequired = true)]
[TemplatePart("PART_YesButton", typeof(Button), IsRequired = true)]
public class InternalMessageDialog : InternalMessageBase
{
    private Button? _cancelButton;
    private Button? _noButton;
    private Button? _okButton;
    private Button? _yesButton;

    private readonly TaskCompletionSource<InternalDialogResult> _resultSource
        = new(TaskCreationOptions.RunContinuationsAsynchronously);

    #region AVALONIA PROPERTIES

    public static readonly StyledProperty<InternalDialogButtons> ButtonsProperty
        = AvaloniaProperty.Register<InternalMessageDialog, InternalDialogButtons>(nameof(Buttons), InternalDialogButtons.Ok);

    public static readonly StyledProperty<bool> EnableCloseButtonProperty
        = AvaloniaProperty.Register<InternalMessageDialog, bool>(nameof(EnableCloseButton), true);

    public static readonly StyledProperty<LucideIconKind> IconProperty
        = AvaloniaProperty.Register<InternalMessageDialog, LucideIconKind>(nameof(Icon), LucideIconKind.MessageCircle);

    #endregion AVALONIA PROPERTIES

    #region PROPERTIES

    public InternalDialogButtons Buttons
    {
        get => GetValue(ButtonsProperty);
        set => SetValue(ButtonsProperty, value);
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

    protected override Type StyleKeyOverride => typeof(InternalMessageDialog);

    #endregion PROPERTIES

    #region CONSTRUCTORS

    public InternalMessageDialog()
    {
        //
    }

    public InternalMessageDialog(
        string title,
        object? content,
        InternalDialogButtons buttons = InternalDialogButtons.Ok,
        LucideIconKind icon = LucideIconKind.MessageCircle,
        bool enableCloseButton = true)
    {
        Title = title;
        Content = content;
        Buttons = buttons;
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
        _noButton = e.NameScope.Find<Button>("PART_NoButton");
        _okButton = e.NameScope.Find<Button>("PART_OkButton");
        _yesButton = e.NameScope.Find<Button>("PART_YesButton");

        if (_cancelButton is not null)
            _cancelButton.Click += CancelButton_Click;
        if (_noButton is not null)
            _noButton.Click += NoButton_Click;
        if (_okButton is not null)
            _okButton.Click += OkButton_Click;
        if (_yesButton is not null)
            _yesButton.Click += YesButton_Click;
    }

    private void DetachHandlers()
    {
        if (_cancelButton is not null)
            _cancelButton.Click -= CancelButton_Click;
        if (_noButton is not null)
            _noButton.Click -= NoButton_Click;
        if (_okButton is not null)
            _okButton.Click -= OkButton_Click;
        if (_yesButton is not null)
            _yesButton.Click -= YesButton_Click;

        _cancelButton = null;
        _noButton = null;
        _okButton = null;
        _yesButton = null;
    }

    #endregion TEMPLATE

    #region ACTIONS

    protected override void OnClosed()
    {
        _resultSource.TrySetResult(InternalDialogResult.None);
    }

    private void Complete(InternalDialogResult result)
    {
        _resultSource.TrySetResult(result);
        RequestClose();
    }

    #endregion ACTIONS

    #region BUTTON ACTIONS

    private void CancelButton_Click(object? sender, RoutedEventArgs e) => Complete(InternalDialogResult.Cancel);
    private void NoButton_Click(object? sender, RoutedEventArgs e) => Complete(InternalDialogResult.No);
    private void OkButton_Click(object? sender, RoutedEventArgs e) => Complete(InternalDialogResult.Ok);
    private void YesButton_Click(object? sender, RoutedEventArgs e) => Complete(InternalDialogResult.Yes);

    #endregion BUTTON ACTIONS
}
