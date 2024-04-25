using Microsoft.Shell;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using VisualPlayer.Controls;

namespace VisualPlayer.Pages.Base
{
    public class BasePage : Page
    {

        //  DEPENDENCY PROPERTIES

        public static readonly DependencyProperty SingleInstanceProperty = DependencyProperty.Register(
            nameof(SingleInstance),
            typeof(bool),
            typeof(BasePage),
            new PropertyMetadata(false));


        //  VARIABLES

        public IContentViewer ContentViewer { get; private set; }


        //  GETTERS & SETTERS

        public bool SingleInstance
        {
            get => (bool)GetValue(SingleInstanceProperty);
            set => SetValue(SingleInstanceProperty, value);
        }


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Static BasePage class constructor. </summary>
        static BasePage()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(BasePage),
                new FrameworkPropertyMetadata(typeof(BasePage)));
        }

        //  --------------------------------------------------------------------------------
        /// <summary> BasePage class constructor. </summary>
        /// <param name="contentViewer"> Content viewer interface. </param>
        public BasePage(IContentViewer contentViewer)
        {
            ContentViewer = contentViewer;
        }

        #endregion CLASS METHODS

    }
}
