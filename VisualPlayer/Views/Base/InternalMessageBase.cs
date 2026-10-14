using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using VisualPlayer.Interfaces;

namespace VisualPlayer.Views.Base;

[TemplatePart("PART_CloseButton", typeof(Button), IsRequired = true)]
public abstract class InternalMessageBase : ContentControl, IInternalContent
{
    private Button? _closeButton;

    #region AVALONIA PROPERTIES

    public static readonly StyledProperty<string> TitleProperty
        = AvaloniaProperty.Register<InternalMessageBase, string>(nameof(Title), string.Empty);

    #endregion AVALONIA PROPERTIES

    #region EVENTS

    public event EventHandler? CloseRequested;

    #endregion EVENTS

    #region PROPERTIES

    public string Title
    {
        get => GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    #endregion PROPERTIES

    #region ACTIONS

    private void CloseButton_Click(object? sender, RoutedEventArgs e)
    {
        RequestClose();
    }

    protected virtual void OnClosed()
    {
        //
    }

    void IInternalContent.OnClosedByController()
    {
        OnClosed();
    }

    protected void RequestClose()
    {
        CloseRequested?.Invoke(this, EventArgs.Empty);
    }

    #endregion ACTIONS

    #region TEMPLATE

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        if (_closeButton is not null)
            _closeButton.Click -= CloseButton_Click;

        base.OnApplyTemplate(e);

        _closeButton = e.NameScope.Find<Button>("PART_CloseButton");

        if (_closeButton is not null)
            _closeButton.Click += CloseButton_Click;
    }

    #endregion TEMPLATE
}
