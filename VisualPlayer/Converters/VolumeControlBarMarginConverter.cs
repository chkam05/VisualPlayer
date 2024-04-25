using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using VisualPlayer.Controls;
using VisualPlayer.Utilities;

namespace VisualPlayer.Converters
{
    public class VolumeControlBarMarginConverter : IMultiValueConverter
    {

        //  CONST

        private const int REQUIRED_ITEMS_COUNT = 2;


        //  METHODS

        //  --------------------------------------------------------------------------------
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var controlBar = ObjectHelper.GetValue<IControlBar>(values, null);
            var margin = ObjectHelper.GetValue<Thickness>(values, new Thickness(0));
            var volumeControlBar = ObjectHelper.GetValue<VolumeControlBar>(values, null);

            if (controlBar == null || volumeControlBar == null)
                return margin;

            var topLevelGrid = controlBar.GetOuterGrid();
            var volumeButton = controlBar.GetVolumeControlButton();

            double buttonRelativePosition = volumeButton.Margin.Left;
            double buttonWidth = volumeButton.ActualWidth;

            FrameworkElement element = volumeButton;

            while (!(element.Parent is Grid grid && grid == topLevelGrid))
            {
                element = element.Parent as FrameworkElement;

                var elementMargin = ObjectHelper.GetPropertyValue(element, "Margin") as Thickness?;
                var padding = ObjectHelper.GetPropertyValue(element, "Padding") as Thickness?;

                if (elementMargin.HasValue)
                    buttonRelativePosition += elementMargin.Value.Left;

                if (padding.HasValue)
                    buttonRelativePosition += padding.Value.Left;

                if (element.Parent is Grid parentGrid)
                {
                    var columnIndex = Grid.GetColumn(element);
                    buttonRelativePosition += GetColumnsWidth(parentGrid, columnIndex);
                }
            }

            double buttonRightMargin = topLevelGrid.ActualWidth - (buttonRelativePosition + buttonWidth);
            double marginDiff = (volumeButton.ActualWidth - volumeControlBar.ActualWidth) / 2;

            return ObjectHelper.ModifyThickness(margin, right: buttonRightMargin + marginDiff);
        }

        //  --------------------------------------------------------------------------------
        private object OldConvert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (ValidateParameters(values,
                out Thickness? margin,
                out Button button,
                out Grid topLevelGrid,
                out VolumeControlBar volumeControlBar))
            {
                double buttonRelativePosition = button.Margin.Left;
                double buttonWidth = button.ActualWidth;

                FrameworkElement element = button;

                while (!(element.Parent is Grid grid && grid == topLevelGrid))
                {
                    element = element.Parent as FrameworkElement;

                    var elementMargin = ObjectHelper.GetPropertyValue(element, "Margin") as Thickness?;
                    var padding = ObjectHelper.GetPropertyValue(element, "Padding") as Thickness?;

                    if (margin.HasValue)
                        buttonRelativePosition += elementMargin.Value.Left;

                    if (padding.HasValue)
                        buttonRelativePosition += padding.Value.Left;

                    if (element.Parent is Grid parentGrid)
                    {
                        var columnIndex = Grid.GetColumn(element);
                        buttonRelativePosition += GetColumnsWidth(parentGrid, columnIndex);
                    }
                }

                double buttonRightMargin = topLevelGrid.ActualWidth - (buttonRelativePosition + buttonWidth);
                double marginDiff = (button.ActualWidth - volumeControlBar.ActualWidth) / 2;

                return ObjectHelper.ModifyThickness(margin.Value, right: buttonRightMargin + marginDiff);
            }

            return margin ?? new Thickness(0);
        }

        //  --------------------------------------------------------------------------------
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #region UTILITY METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Get columns width. </summary>
        /// <param name="parentGrid"> Grid with columns. </param>
        /// <param name="columnIndex"> Index of the column in which the object is. </param>
        /// <returns> Columns width. </returns>
        private double GetColumnsWidth(Grid parentGrid, int columnIndex)
        {
            double result = 0;

            for (int i = 0; i < columnIndex; i++)
            {
                ColumnDefinition column = parentGrid.ColumnDefinitions[i];
                result += column.ActualWidth;
            }

            return result;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Validate parameters. </summary>
        /// <param name="values"> Aray of parameter. </param>
        /// <param name="margin"> Output margin parameter. </param>
        /// <param name="button"> Output button parameter. </param>
        /// <param name="topLevelGrid"> Output top level grid parameter. </param>
        /// <param name="volumeControlBar"> Volume control bar parameter. </param>
        /// <returns> True - parameters are correct; False - otherwise. </returns>
        private bool ValidateParameters(object[] values, out Thickness? margin,
            out Button button, out Grid topLevelGrid, out VolumeControlBar volumeControlBar)
        {
            if (values.Length != REQUIRED_ITEMS_COUNT)
            {
                margin = new Thickness(0);
                button = null;
                topLevelGrid = null;
                volumeControlBar = null;
                return false;
            }

            margin = values.Any(v => v is Thickness) ? (Thickness)values.First(v => v is Thickness) : new Thickness(0);
            button = values.FirstOrDefault(v => v is Button) as Button;
            topLevelGrid = values.FirstOrDefault(v => v is Grid) as Grid;
            volumeControlBar = values.FirstOrDefault(v => v is VolumeControlBar) as VolumeControlBar;

            if (button == null || topLevelGrid == null || volumeControlBar == null)
                return false;

            return true;
        }

        #endregion UTILITY METHODS

    }
}
