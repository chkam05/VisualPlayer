using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace chkam05.VisualPlayer.Components
{
    public class SideBarMenuListView : ListView
    {

        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> SideBarMenuListView static class constructor. </summary>
        static SideBarMenuListView()
        {
            DefaultStyleKeyProperty.OverrideMetadata(
                typeof(SideBarMenuListView), 
                new FrameworkPropertyMetadata(typeof(SideBarMenuListView)));
        }

        #endregion CLASS METHODS

        #region ITEMS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Creates and returns a new System.Windows.Controls.ListViewItem container. </summary>
        /// <returns> A new System.Windows.Controls.ListViewItem control. </returns>
        protected override DependencyObject GetContainerForItemOverride()
        {
            return new SideBarMenuItem();
        }

        #endregion ITEMS METHODS

    }
}
