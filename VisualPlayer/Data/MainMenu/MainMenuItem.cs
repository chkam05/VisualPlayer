using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using VisualPlayer.ViewModels;

namespace VisualPlayer.Data.MainMenu
{
    public class MainMenuItem : BaseViewModel
    {

        //  DELEGATES

        public delegate void MainMenuItemClickEventHandler(object sender, MainMenuItemClickEventArgs e);


        //  EVENTS

        public MainMenuItemClickEventHandler Click;


        //  VARIABLES

        private string _description;
        private PackIconKind _icon;
        private string _title;


        //  GETTERS & SETTERS

        public string Description
        {
            get => _description;
            set => UpdateProperty(ref _description, value);
        }

        public PackIconKind Icon
        {
            get => _icon;
            set => UpdateProperty(ref _icon, value);
        }

        public string Title
        {
            get => _title;
            set => UpdateProperty(ref _title, value);
        }


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> MainMenuItem class constructor. </summary>
        /// <param name="title"> Menu item title. </param>
        /// <param name="icon"> Menu item icon kind. </param>
        /// <param name="clickEvent"> Menu item click event method. </param>
        public MainMenuItem(string title, PackIconKind icon, MainMenuItemClickEventHandler clickEvent)
        {
            Title = title;
            Icon = icon;
            Click = clickEvent;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> MainMenuItem class constructor. </summary>
        /// <param name="title"> Menu item title. </param>
        /// <param name="icon"> Menu item icon kind. </param>
        /// <param name="description"> Menu item description. </param>
        /// <param name="clickEvent"> Menu item click event method. </param>
        public MainMenuItem(string title, PackIconKind icon, string description, MainMenuItemClickEventHandler clickEvent)
        {
            Title = title;
            Icon = icon;
            Description = description;
            Click = clickEvent;
        }

        #endregion CLASS METHODS

        #region INVOKE METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Invoke click event. </summary>
        public void InvokeClick()
        {
            Click?.Invoke(this, new MainMenuItemClickEventArgs(Title));
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Invoke click event. </summary>
        /// <param name="parameters"> Click event arguments. </param>
        public void InvokeClick(params object[] parameters)
        {
            Click?.Invoke(this, new MainMenuItemClickEventArgs(Title, parameters));
        }

        #endregion INVOKE METHODS

    }
}
