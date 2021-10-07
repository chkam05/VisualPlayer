using System;
using System.Collections.Generic;
using System.Linq;
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

namespace chkam05.VisualPlayer.Components
{
    public partial class BorderedPasswordBox : UserControl
    {

        //  EVENTS

        public event EventHandler<string> OnUserPasswordChange;


        //  VARIABLES

        private bool _focused = false;


        #region GETTERS & SETTERS

        public new Brush Background
        {
            get => ContentBorder.Background;
            set => ContentBorder.Background = value;
        }

        public new Brush BorderBrush
        {
            get => ContentBorder.BorderBrush;
            set => ContentBorder.BorderBrush = value;
        }

        public new double BorderThickness
        {
            get => ContentBorder.BorderThickness.Left;
            set => ContentBorder.BorderThickness = new Thickness(value);
        }

        public string Password
        {
            get => InputPasswordBox.Password;
            set => InputPasswordBox.Password = value;
        }

        #endregion GETTERS & SETTERS


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> BorderedPasswordBox class constructor. </summary>
        public BorderedPasswordBox()
        {
            InitializeComponent();
        }

        #endregion CLASS METHODS

        #region INTERACTION METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Method called when passwordbox is selected. </summary>
        /// <param name="sender"> Object that invoked event. </param>
        /// <param name="e"> Routed event arguments. </param>
        private void InputPasswordBox_GotFocus(object sender, RoutedEventArgs e)
        {
            _focused = true;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method called when passwordbox is unselected. </summary>
        /// <param name="sender"> Object that invoked event. </param>
        /// <param name="e"> Routed event arguments. </param>
        private void InputPasswordBox_LostFocus(object sender, RoutedEventArgs e)
        {
            _focused = false;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method called when password is changed. </summary>
        /// <param name="sender"> Object that invoked event. </param>
        /// <param name="e"> Text change event arguments. </param>
        private void InputPasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (_focused)
                OnUserPasswordChange?.Invoke(this, (sender as PasswordBox).Password);
        }

        #endregion INTERACTION METHODS

    }
}
