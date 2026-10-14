using System;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;

using VisualPlayer.Interfaces;

namespace VisualPlayer.Controls;

public abstract class InternalControllerBase : ContentControl
{
    private readonly Stack<Control> _history = new();
    
    private bool _canGoBack;
    private Control? _currentView;
    private bool _isOpen;


    #region EVENTS

    public event EventHandler? CurrentViewChanged;

    #endregion EVENTS

    #region AVALONIA PROPERTIES

    public static readonly DirectProperty<InternalControllerBase, bool> CanGoBackProperty
        = AvaloniaProperty.RegisterDirect<InternalControllerBase, bool>(nameof(CanGoBack), control => control.CanGoBack);

    public static readonly DirectProperty<InternalControllerBase, Control?> CurrentViewProperty
        = AvaloniaProperty.RegisterDirect<InternalControllerBase, Control?>(nameof(CurrentView), control => control.CurrentView);

    public static readonly DirectProperty<InternalControllerBase, bool> IsOpenProperty
        = AvaloniaProperty.RegisterDirect<InternalControllerBase, bool>(nameof(IsOpen), control => control.IsOpen);

    #endregion AVALONIA PROPERTIES

    #region PROPERTIES

    public bool CanGoBack
    {
        get => _canGoBack;
        private set => SetAndRaise(CanGoBackProperty, ref _canGoBack, value);
    }

    public Control? CurrentView
    {
        get => _currentView;
        private set => SetAndRaise(CurrentViewProperty, ref _currentView, value);
    }

    public int HistoryCount
    {
        get => _history.Count;
    }

    public bool IsOpen
    {
        get => _isOpen;
        private set => SetAndRaise(IsOpenProperty, ref _isOpen, value);
    }

    #endregion PROPERTIES

    #region CONSTRUCTORS

    protected InternalControllerBase()
    {
        IsVisible = false;
    }

    #endregion CONSTRUCTORS

    #region ACTIONS

    private async void View_BackRequested(object? sender, EventArgs e)
    {
        if (CurrentView is IInternalCloseGuard guard && !await guard.CanCloseAsync())
            return;

        GoBack();
    }

    private async void View_CloseRequested(object? sender, EventArgs e)
    {
        if (CurrentView is IInternalCloseGuard guard && !await guard.CanCloseAsync())
            return;

        Close();
    }

    #endregion ACTIONS

    #region CONTENT

    protected abstract bool IsSupportedView(Control view);

    private static void NotifyClosed(Control view)
    {
        if (view is IInternalContent content)
            content.OnClosedByController();
    }

    private void RemoveCurrentView()
    {
        if (CurrentView is not IInternalContent content)
            return;

        content.CloseRequested -= View_CloseRequested;

        if (CurrentView is IInternalNavigableContent navigableContent)
            navigableContent.BackRequested -= View_BackRequested;

        content.OnClosedByController();
    }

    private void SetCurrentView(Control? view)
    {
        CurrentView = view;
        Content = view;
        CanGoBack = _history.Count > 0;
        IsOpen = view is not null;
        IsVisible = IsOpen;

        if (view is IInternalContent content)
        {
            content.CloseRequested += View_CloseRequested;
        }

        if (view is IInternalNavigableContent navigableContent)
            navigableContent.BackRequested += View_BackRequested;

        UpdateViewState(view);
        CurrentViewChanged?.Invoke(this, EventArgs.Empty);
    }

    private void SuspendCurrentView()
    {
        if (CurrentView is not IInternalContent content)
            return;

        content.CloseRequested -= View_CloseRequested;

        if (CurrentView is IInternalNavigableContent navigableContent)
            navigableContent.BackRequested -= View_BackRequested;
    }

    protected virtual void UpdateViewState(Control? view)
    {
        //
    }

    private void ValidateView(Control view)
    {
        ArgumentNullException.ThrowIfNull(view);

        if (!IsSupportedView(view))
            throw new ArgumentException(
                $"Widok {view.GetType().Name} nie jest obsługiwany przez {GetType().Name}.",
                nameof(view));
    }

    #endregion CONTENT

    #region NAVIGATION

    public void Clear()
    {
        RemoveCurrentView();

        while (_history.TryPop(out var view))
            NotifyClosed(view);

        SetCurrentView(null);
    }

    public void Close()
    {
        RemoveCurrentView();

        if (_history.TryPop(out var previousView))
            SetCurrentView(previousView);
        else
            SetCurrentView(null);
    }

    public bool GoBack()
    {
        if (_history.Count == 0)
            return false;

        RemoveCurrentView();
        SetCurrentView(_history.Pop());
        return true;
    }

    protected void OpenCore(Control view)
    {
        ValidateView(view);

        if (ReferenceEquals(CurrentView, view))
            return;

        if (CurrentView is not null)
        {
            SuspendCurrentView();
            _history.Push(CurrentView);
        }

        SetCurrentView(view);
    }

    protected void ReplaceCore(Control view)
    {
        ValidateView(view);
        RemoveCurrentView();
        SetCurrentView(view);
    }

    #endregion NAVIGATION
}
