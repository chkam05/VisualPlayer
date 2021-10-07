using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace chkam05.VisualPlayer.Utilities
{
    public static class ComponentUtils
    {

        //  METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Find parent component of child component. </summary>
        /// <typeparam name="T"> Particular type parent component. </typeparam>
        /// <param name="childObject"> Child component. </param>
        /// <returns> Parent component or NULL. </returns>
        public static T FindVisualParent<T>(DependencyObject childObject) where T : DependencyObject
        {
            var parentObject = VisualTreeHelper.GetParent(childObject);

            if (parentObject != null)
            {
                T parent = parentObject as T;

                if (parent != null)
                    return parent;
                else
                    return FindVisualParent<T>(parentObject);
            }

            return null;
        }

    }
}
