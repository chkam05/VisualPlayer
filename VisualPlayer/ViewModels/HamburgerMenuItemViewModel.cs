using System;
using System.Windows.Input;
using Lucide.Avalonia;

namespace VisualPlayer.ViewModels;

public class HamburgerMenuItemViewModel : MenuItemViewModel
{
    #region CONSTRUCTORS

    public HamburgerMenuItemViewModel(LucideIconKind icon, string title, ICommand action)
        : base(icon, title, action)
    {
        //
    }

    public HamburgerMenuItemViewModel(LucideIconKind icon, string title, Action? action)
        : base(icon, title, action)
    {
        //
    }

    public new static HamburgerMenuItemViewModel CreateSeparator()
    {
        return new HamburgerMenuItemViewModel(default, string.Empty, (Action?)null)
        {
            IsSeparator = true
        };
    }

    #endregion CONSTRUCTORS

}
