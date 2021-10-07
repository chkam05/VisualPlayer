using MaterialDesignThemes.Wpf;
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
    public partial class MenuListViewItem : ListViewItem
    {

        //  CONST

        private const int MIN_ICON_SIZE = 24;


        //  VARIABLES

        public PackIconKind Icon
        {
            get => PackIcon.Kind;
            set => PackIcon.Kind = value;
        }

        public Size IconSize
        {
            get => new Size(PackIcon.ActualWidth, PackIcon.ActualHeight);
            set
            {
                PackIcon.Height = Math.Max(MIN_ICON_SIZE, value.Height);
                PackIcon.Width = Math.Max(MIN_ICON_SIZE, value.Width);
            }
        }

        public string Title
        {
            get => TitleTextBlock.Text;
            set => TitleTextBlock.Text = value;
        }

        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Menu ListView Item class constructor. </summary>
        public MenuListViewItem()
        {
            InitializeComponent();
        }

        #endregion CLASS METHODS

    }
}
