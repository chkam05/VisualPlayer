using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace VisualPlayer.Controls
{
    public class CustomContextMenu : ContextMenu
    {

        //  DEPENDENCY PROPERTIES

        public static readonly DependencyProperty CornerRadiusProperty = DependencyProperty.Register(
            nameof(CornerRadius),
            typeof(CornerRadius),
            typeof(CustomContextMenu),
            new PropertyMetadata(new CornerRadius(8)));


        //  GETTERS & SETTERS

        public CornerRadius CornerRadius
        {
            get => (CornerRadius)GetValue(CornerRadiusProperty);
            set => SetValue(CornerRadiusProperty, value);
        }


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Static CustomContextMenu class constructor. </summary>
        static CustomContextMenu()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CustomContextMenu),
                new FrameworkPropertyMetadata(typeof(CustomContextMenu)));
        }

        #endregion CLASS METHODS

        #region ITEMS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Creates and returns new CustomContextMenuItem container. </summary>
        /// <returns> A new CustomContextMenuItem control. </returns>
        protected override DependencyObject GetContainerForItemOverride()
        {
            return new CustomContextMenuItem();
        }

        #endregion ITEMS METHODS

    }
}
