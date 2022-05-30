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
    public class ExtendedTabControl : TabControl, INotifyPropertyChanged
    {

        //  EVENTS

        public event PropertyChangedEventHandler PropertyChanged;


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> ExtendedTabControl static class constructor. </summary>
        static ExtendedTabControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(
                typeof(ExtendedTabControl),
                new FrameworkPropertyMetadata(typeof(ExtendedTabControl)));
        }

        #endregion CLASS METHODS

        #region ITEMS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Creates and returns a new System.Windows.Controls.ListViewItem container. </summary>
        /// <returns> A new System.Windows.Controls.ListViewItem control. </returns>
        protected override DependencyObject GetContainerForItemOverride()
        {
            return new ExtendedTabItem();
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
