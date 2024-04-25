using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;

namespace VisualPlayer.Utilities
{
    public static class ResourcesHelper
    {

        //  METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Get resource value from framework element. </summary>
        /// <typeparam name="T"> Resource type. </typeparam>
        /// <param name="frameworkElement"> Framework element. </param>
        /// <param name="resourceName"> Resource name. </param>
        /// <param name="value"> Result value. </param>
        /// <returns> True - resource returned; False - otherwise. </returns>
        public static bool GetResource<T>(FrameworkElement frameworkElement, string resourceName, out T value)
        {
            try
            {
                if (frameworkElement.Resources.Contains(resourceName))
                {
                    object resourceValue = frameworkElement.Resources[resourceName];

                    if (resourceValue is T targetValue)
                    {
                        value = targetValue;
                        return true;
                    }
                    else
                    {
                        throw new InvalidOperationException($"{resourceName} is not type of {typeof(T)}.");
                    }
                }
                else
                {
                    throw new KeyNotFoundException($"{resourceName} resource not found.");
                }
            }
            catch (Exception)
            {
                value = default(T);
                return false;
            }
        }

    }
}
