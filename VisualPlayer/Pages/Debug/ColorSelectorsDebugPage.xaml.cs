using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using VisualPlayer.Controls;
using VisualPlayer.Data.Attributes;
using VisualPlayer.Data.ColorModels;
using VisualPlayer.Pages.Base;
using VisualPlayer.Utilities;

namespace VisualPlayer.Pages.Debug
{
    public partial class ColorSelectorsDebugPage : BasePage, INotifyPropertyChanged
    {

        //  EVENTS

        public event PropertyChangedEventHandler PropertyChanged;


        //  VARIABLES

        private ThemeColor _selectedColor = null;


        //  GETTERS & SETTERS

        public ThemeColor SelectedColor
        {
            get => _selectedColor;
            set => UpdateProperty(ref _selectedColor, value);
        }


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> ColorSelectorsDebugPage class constructor. </summary>
        /// <param name="contentViewer"> Content viewer interface. </param>
        public ColorSelectorsDebugPage(IContentViewer contentViewer) : base(contentViewer)
        {
            //  Initialize user interface.
            InitializeComponent();
        }

        #endregion CLASS METHODS

        #region HEADER INTERACTION METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after clicking back button. </summary>
        /// <param name="sender"> Object that invoked the method. </param>
        /// <param name="e"> Routed Event Arguments. </param>
        private void BackButtonClick(object sender, RoutedEventArgs e)
        {
            ContentViewer.GoBack();
        }

        #endregion HEADER INTERACTION METHODS

        #region NOTIFICATION PROPERTIES CHANGED METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Trigger notify property changed in user interface. </summary>
        /// <param name="propertyName"> Property name. </param>
        protected virtual void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Update property value and trigger notify property changed in user interface. </summary>
        /// <typeparam name="T"> Property type. </typeparam>
        /// <param name="field"> Reference to property field. </param>
        /// <param name="newValue"> New value. </param>
        /// <param name="propertyName"> Property name. </param>
        protected virtual void UpdateProperty<T>(ref T field, T newValue, [CallerMemberName] string propertyName = null)
        {
            if (string.IsNullOrEmpty(propertyName))
                throw new ArgumentException("Property update failed. Property name cannot be null or empty.");

            field = newValue;

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion NOTIFICATION PROPERTIES CHANGED METHODS

    }
}
