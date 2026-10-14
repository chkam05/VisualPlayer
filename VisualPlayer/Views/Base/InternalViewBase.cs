using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using VisualPlayer.Interfaces;

namespace VisualPlayer.Views.Base;

[TemplatePart("PART_BackButton", typeof(Button), IsRequired = true)]
[TemplatePart("PART_CloseButton", typeof(Button), IsRequired = true)]
[TemplatePart("PART_SaveButton", typeof(Button), IsRequired = true)]
public abstract class InternalViewBase : ContentControl, IInternalContent, IInternalNavigableContent
{
    private Button? _backButton;
    private bool _canGoBack;
    private Button? _closeButton;
    private Button? _saveButton;

    #region AVALONIA PROPERTIES

    public static readonly DirectProperty<InternalViewBase, bool> CanGoBackProperty
        = AvaloniaProperty.RegisterDirect<InternalViewBase, bool>(nameof(CanGoBack), view => view.CanGoBack);

    public static readonly StyledProperty<bool> EnableSaveButtonProperty
        = AvaloniaProperty.Register<InternalViewBase, bool>(nameof(EnableSaveButton));

    public static readonly StyledProperty<string> TitleProperty
        = AvaloniaProperty.Register<InternalViewBase, string>(nameof(Title), string.Empty);

    #endregion AVALONIA PROPERTIES

    #region EVENTS

    public event EventHandler? BackRequested;
    public event EventHandler? CloseRequested;
    public event EventHandler? SaveRequested;

    #endregion EVENTS

    #region PROPERTIES

    public bool CanGoBack
    {
        get => _canGoBack;
        internal set => SetAndRaise(CanGoBackProperty, ref _canGoBack, value);
    }

    public bool EnableSaveButton
    {
        get => GetValue(EnableSaveButtonProperty);
        protected set => SetValue(EnableSaveButtonProperty, value);
    }

    protected override Type StyleKeyOverride => typeof(InternalViewBase);

    public string Title
    {
        get => GetValue(TitleProperty);
        protected set => SetValue(TitleProperty, value);
    }

    #endregion PROPERTIES

    #region ACTIONS

    private void BackButton_Click(object? sender, RoutedEventArgs e)
    {
        BackRequested?.Invoke(this, EventArgs.Empty);
    }

    private void CloseButton_Click(object? sender, RoutedEventArgs e)
    {
        CloseRequested?.Invoke(this, EventArgs.Empty);
    }

    private void SaveButton_Click(object? sender, RoutedEventArgs e)
    {
        SaveRequested?.Invoke(this, EventArgs.Empty);
    }

    void IInternalContent.OnClosedByController()
    {
        OnClosed();
    }

    protected virtual void OnClosed()
    {
        //
    }

    #endregion ACTIONS

    #region TEMPLATE

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        DetachHandlers();
        base.OnApplyTemplate(e);

        _backButton = e.NameScope.Find<Button>("PART_BackButton");
        _closeButton = e.NameScope.Find<Button>("PART_CloseButton");
        _saveButton = e.NameScope.Find<Button>("PART_SaveButton");

        if (_backButton is not null)
            _backButton.Click += BackButton_Click;

        if (_closeButton is not null)
            _closeButton.Click += CloseButton_Click;

        if (_saveButton is not null)
            _saveButton.Click += SaveButton_Click;
    }

    private void DetachHandlers()
    {
        if (_backButton is not null)
            _backButton.Click -= BackButton_Click;

        if (_closeButton is not null)
            _closeButton.Click -= CloseButton_Click;

        if (_saveButton is not null)
            _saveButton.Click -= SaveButton_Click;

        _backButton = null;
        _closeButton = null;
        _saveButton = null;
    }

    #endregion TEMPLATE
}
