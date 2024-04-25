using chkam05.VisualPlayer.Controls.Static;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chkam05.VisualPlayer.Controls.Data
{
    public class MenuItem : INotifyPropertyChanged
    {

        //  EVENTS

        public event PropertyChangedEventHandler PropertyChanged;


        //  VARIABLES

        private PackIconKind _kind;
        private string _title;

        public MenuItemType Type { get; private set; }
        public MenuItemSubType SubType { get; private set; }


        //  GETTERS & SETTERS

        public PackIconKind Kind
        {
            get => _kind;
            set
            {
                _kind = value;
                OnPropertyChanged(nameof(Kind));
            }
        }

        public string Title
        {
            get => _title;
            set
            {
                _title = value;
                OnPropertyChanged(nameof(Title));
            }
        }


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> MenuItem class constructor. </summary>
        /// <param name="type"> Menu item type. </param>
        /// <param name="title"> Menu item title. </param>
        /// <param name="kind"> Menu item pack icon kind. </param>
        public MenuItem(MenuItemType type, MenuItemSubType subType, string title, PackIconKind kind)
        {
            Type = type;
            SubType = subType;
            Title = title;
            Kind = kind;
        }

        #endregion CLASS METHODS

        #region NOTIFY PROPERTIES CHANGED INTERFACE METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Method for invoking PropertyChangedEventHandler external method. </summary>
        /// <param name="propertyName"> Changed property name. </param>
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;

            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion NOTIFY PROPERTIES CHANGED INTERFACE METHODS

    }
}
