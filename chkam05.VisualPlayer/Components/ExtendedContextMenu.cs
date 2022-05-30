using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace chkam05.VisualPlayer.Components
{
    public class ExtendedContextMenu : ContextMenu, INotifyPropertyChanged
    {

        //  EVENTS

        public event PropertyChangedEventHandler PropertyChanged;


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> ExtendedContextMenu static class constructor. </summary>
        static ExtendedContextMenu()
        {
            DefaultStyleKeyProperty.OverrideMetadata(
                typeof(ExtendedContextMenu),
                new FrameworkPropertyMetadata(typeof(ExtendedContextMenu)));
        }

        #endregion CLASS METHODS

        #region ITEMS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Creates and returns a new System.Windows.Controls.ComboBoxItem container. </summary>
        /// <returns> A new System.Windows.Controls.ComboBoxItem control. </returns>
        protected override DependencyObject GetContainerForItemOverride()
        {
            return new ExtendedContextMenuItem();
        }

        #endregion ITEMS METHODS

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
