using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace chkam05.VisualPlayer.Components
{
    public class ExtendedScrollViewer : ScrollViewer
    {

        //  GETTERS & SETTERS

        public ExtendedScrollBar HorizontalScrollBar
        {
            get => Template.FindName("PART_HorizontalScrollBar", this) as ExtendedScrollBar;
        }

        public ExtendedScrollBar VerticalScrollBar
        {
            get => Template.FindName("PART_VerticalScrollBar", this) as ExtendedScrollBar;
        }


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> ExtendedScrollViewer static class constructor. </summary>
        static ExtendedScrollViewer()
        {
            DefaultStyleKeyProperty.OverrideMetadata(
                typeof(ExtendedScrollViewer),
                new FrameworkPropertyMetadata(typeof(ExtendedScrollViewer)));
        }

        #endregion CLASS METHODS

    }
}
