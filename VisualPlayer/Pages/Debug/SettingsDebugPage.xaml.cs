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
using VisualPlayer.Controls;
using VisualPlayer.InternalMessages;
using VisualPlayer.Pages.Base;

namespace VisualPlayer.Pages.Debug
{
    public partial class SettingsDebugPage : BasePage
    {

        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> SettingsDebugPage class constructor. </summary>
        /// <param name="contentViewer"> Content viewer interface. </param>
        public SettingsDebugPage(IContentViewer contentViewer) : base(contentViewer)
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

        #region ITEMS INTERACTION METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after clicking on internal messages debug settings button. </summary>
        /// <param name="sender"> Object that invoked the method. </param>
        /// <param name="e"> Routed Event Arguments. </param>
        private void InternalMessagesDebugSettingsButtonClick(object sender, RoutedEventArgs e)
        {
            ContentViewer.LoadPage(new InternalMessagesDebugPage(ContentViewer));
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after clicking on color selectors debug settings button. </summary>
        /// <param name="sender"> Object that invoked the method. </param>
        /// <param name="e"> Routed Event Arguments. </param>
        private void ColorSelectorsDebugSettingsButtonClick(object sender, RoutedEventArgs e)
        {
            ContentViewer.LoadPage(new ColorSelectorsDebugPage(ContentViewer));
        }

        #endregion ITEMS INTERACTION METHODS

    }
}
