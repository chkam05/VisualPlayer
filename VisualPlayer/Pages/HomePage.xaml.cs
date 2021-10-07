using chkam05.VisualPlayer.Components;
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

namespace chkam05.VisualPlayer.Pages
{
    public partial class HomePage : Page, IContentPage
    {

        //  VARIABLES

        private bool _fullBackground = true;


        #region GETTERS & SETTERS

        public bool FullBackground
        {
            get => _fullBackground;
            set => SwitchBackground(value);
        }

        public double ControlPanelMargin
        {
            get => this.Margin.Bottom;
            set => SetControlMargin(value);
        }

        public double SideBarMargin
        {
            get => this.Margin.Left;
            set => SetSideBarMargin(value);
        }

        public Canvas Canvas
        {
            get => VisualisationCanvas;
        }

        public OnScreenDisplay OSD
        {
            get => OSDComponent;
        }

        #endregion GETTERS & SETTERS


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> HomePage class constructor. </summary>
        public HomePage()
        {
            InitializeComponent();
        }

        #endregion CLASS METHODS

        #region INTERFACE MANAGEMENT METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Set bottom margin for control component. </summary>
        /// <param name="bottomMargin"> New bottom margin size. </param>
        private void SetControlMargin(double bottomMargin)
        {
            //  Get current margin.
            var currentMargin = this.Margin;

            //  Setup new margin.
            this.Margin = new Thickness(currentMargin.Left, 0, 0, bottomMargin);
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Set left margin for sidebar component. </summary>
        /// <param name="leftMargin"> New left margin size. </param>
        private void SetSideBarMargin(double leftMargin)
        {
            //  Get current margin.
            var currentMargin = this.Margin;

            //  Setup new margin.
            this.Margin = new Thickness(leftMargin, 0, 0, currentMargin.Bottom);
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Switch background mode to full or limited. </summary>
        /// <param name="fullBackground"> True - full background mode; False - limited background mode. </param>
        private void SwitchBackground(bool fullBackground)
        {
            //  Get current brush.
            //  var currentBrush = _fullBackground ? this.Background : ContentGrid.Background;

            //  Switch brushes.
            //  this.Background = fullBackground ? currentBrush : null;
            //  ContentGrid.Background = fullBackground ? null : currentBrush;

            //  Set full background variable.
            _fullBackground = fullBackground;
        }

        #endregion INTERFACE MANAGEMENT METHODS

    }
}
