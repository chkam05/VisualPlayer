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

namespace chkam05.VisualPlayer.Pages.Settings
{
    public partial class InfoSettingsPage : Page
    {

        //  VARIABLES

        private App _application;


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> SettingsPage class constructor. </summary>
        public InfoSettingsPage()
        {
            _application = (App) Application.Current;

            InitializeComponent();
        }

        #endregion CLASS METHODS

        #region PAGE METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Mathod called after loading page. </summary>
        /// <param name="sender"> Object that invoked event. </param>
        /// <param name="e"> Routed event arguments. </param>
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            var version = VersionParagraph.Inlines;

            version.Clear();
            version.Add($"Version: {_application.Version}");
        }

        #endregion PAGE METHODS

    }
}
