using System;
using System.Windows.Input;
using Lucide.Avalonia;
using VisualPlayer.Attributes;
using VisualPlayer.Commands;

namespace VisualPlayer.ViewModels;

public class MenuItemViewModel : ViewModelBase
{
    private ICommand _action;
    private LucideIconKind _icon;
    private string _title;

    #region PROPERTIES

    [NotifyPropertyUpdate]
    public ICommand Action
    {
        get => _action;
        set => UpdateProperty(ref _action, value);
    }

    public bool IsSeparator { get; init; }

    public bool IsButtonVisible => !IsSeparator;

    [NotifyPropertyUpdate]
    public LucideIconKind Icon
    {
        get => _icon;
        set => UpdateProperty(ref _icon, value);
    }

    [NotifyPropertyUpdate]
    public string Title
    {
        get => _title;
        set => UpdateProperty(ref _title, value);
    }

    #endregion PROPERTIES

    #region CONSTRUCTORS

    public MenuItemViewModel(LucideIconKind icon, string title, ICommand action)
    {
        _action = action;
        _icon = icon;
        _title = title;
    }

    public MenuItemViewModel(LucideIconKind icon, string title, Action? action)
        : this(icon, title, new RelayCommand(action ?? NoAction))
    {
        //
    }

    public static MenuItemViewModel CreateSeparator()
    {
        return new MenuItemViewModel(default, string.Empty, (Action?)null)
        {
            IsSeparator = true
        };
    }

    #endregion CONSTRUCTORS

    #region UTILITIES

    private static void NoAction()
    {
        //
    }

    #endregion UTILITIES
}
