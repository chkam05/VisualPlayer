using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace VisualPlayer.Controls
{
    public class CustomTreeView : TreeView
    {

        //  DEPENDENCY PROPERTIES

        public static readonly DependencyProperty CornerRadiusProperty = DependencyProperty.Register(
            nameof(CornerRadius),
            typeof(CornerRadius),
            typeof(CustomTreeView),
            new PropertyMetadata(new CornerRadius(4)));


        //  GETTERS & SETTERS

        public CornerRadius CornerRadius
        {
            get => (CornerRadius)GetValue(CornerRadiusProperty);
            set => SetValue(CornerRadiusProperty, value);
        }


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> CustomTreeView static class constructor. </summary>
        static CustomTreeView()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CustomTreeView),
                new FrameworkPropertyMetadata(typeof(CustomTreeView)));
        }

        #endregion CLASS METHODS

        #region ITEMS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Creates and returns new CustomTreeViewItem container. </summary>
        /// <returns> A new CustomTreeViewItem control. </returns>
        protected override DependencyObject GetContainerForItemOverride()
        {
            return new CustomTreeViewItem();
        }

        #endregion ITEMS METHODS

    }
}
