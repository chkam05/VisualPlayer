using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using VisualPlayer.Controls;
using VisualPlayer.Data.Attributes;
using VisualPlayer.Data.Enums;

namespace VisualPlayer.Utilities
{
    public static class ObjectHelper
    {

        //  METHODS

        #region LIST VIEW METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Get horizontal scroll bar visibility. </summary>
        /// <param name="listView"> List view of list view item. </param>
        /// <returns> Horizontal scroll bar visibility. </returns>
        public static Visibility GetListViewHorizontalCustomScrollBarVisibility(CustomListView listView)
        {
            var scrollViewer = (CustomScrollViewer)listView.Template.FindName("scrollViewer", listView);
            return scrollViewer.ComputedHorizontalScrollBarVisibility;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Get vertical scroll bar visibility. </summary>
        /// <param name="listView"> List view of list view item. </param>
        /// <returns> Vertical scroll bar visibility. </returns>
        public static Visibility GetListViewVerticalCustomScrollBarVisibile(CustomListView listView)
        {
            var scrollViewer = (CustomScrollViewer)listView.Template.FindName("scrollViewer", listView);
            return scrollViewer.ComputedVerticalScrollBarVisibility;
        }

        #endregion LIST VIEW METHODS

        #region OBJECT MANAGEMENT METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Convert object. </summary>
        /// <typeparam name="T"> Target object type. </typeparam>
        /// <param name="value"> Object. </param>
        /// <param name="defaultValue"> Default value. </param>
        /// <returns> Value from object or default value. </returns>
        public static T ConvertObject<T>(object value, T defaultValue)
        {
            return value is T tValue ? tValue : defaultValue;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Find parent element by type. </summary>
        /// <typeparam name="T"> Target element type. </typeparam>
        /// <param name="frameworkElement"> Child framework element. </param>
        /// <returns> Target element or null. </returns>
        public static T FindParentElementByTemplate<T>(FrameworkElement frameworkElement) where T: class
        {
            if (frameworkElement == null)
                return null;

            else if (frameworkElement is T targetType)
                return targetType;

            else if (frameworkElement.TemplatedParent != null)
                return FindParentElementByTemplate<T>(frameworkElement.TemplatedParent as FrameworkElement);

            else
                return null;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Get value from objects. </summary>
        /// <typeparam name="T"> Target object type. </typeparam>
        /// <param name="values"> List of objects. </param>
        /// <param name="defaultValue"> Default value. </param>
        /// <returns> Value from objects or default value. </returns>
        public static T GetValue<T>(IEnumerable<object> values, T defaultValue, int skip = 0)
        {
            var tValues = values?.Where(v => v is T).Select(v => (T)v);

            if (tValues.Any())
            {
                var tValue = tValues.Skip(skip).FirstOrDefault();
                return tValue != null ? tValue : defaultValue;
            }

            return defaultValue;
        }

        #endregion OBJECT MANAGEMENT METHODS

        #region PROPERTY METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Get property access level. </summary>
        /// <param name="object"> Object that should contains property. </param>
        /// <param name="propertyName"> Property name. </param>
        /// <returns> Property access level or null. </returns>
        public static PropertyAccessLevel? GetPropertyAccessLevel(object @object, string propertyName)
        {
            PropertyInfo propertyInfo = GetPropertyInfo(@object, propertyName);

            return GetPropertyAccessLevel(propertyInfo);
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Get property access level. </summary>
        /// <param name="propertyInfo"> Property info. </param>
        /// <returns> Property access level or null. </returns>
        public static PropertyAccessLevel? GetPropertyAccessLevel(PropertyInfo propertyInfo)
        {
            if (propertyInfo == null)
                return null;

            MethodInfo methodInfo = propertyInfo.GetGetMethod();

            if (methodInfo == null)
                return null;

            if (methodInfo.IsPublic)
                return PropertyAccessLevel.Public;

            else if (methodInfo.IsAssembly)
                return PropertyAccessLevel.Internal;

            else if (methodInfo.IsFamily)
                return PropertyAccessLevel.Protected;

            else if (methodInfo.IsPrivate)
                return PropertyAccessLevel.Private;

            else
                return null;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Get property attribute with given type. </summary>
        /// <typeparam name="T"> Attribute type. </typeparam>
        /// <param name="object"> Object that should contains attribute. </param>
        /// <param name="propertyName"> Property name. </param>
        /// <returns> Attribute or null. </returns>
        public static T GetPropertyAttribute<T>(object @object, string propertyName) where T : Attribute
        {
            PropertyInfo propertyInfo = GetPropertyInfo(@object, propertyName);

            if (propertyInfo == null)
                return null;

            return (T)Attribute.GetCustomAttribute(propertyInfo, typeof(T));
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Get property info. </summary>
        /// <param name="object"> Object that should contains property. </param>
        /// <param name="propertyName"> Property name. </param>
        /// <returns> Property info or null. </returns>
        public static PropertyInfo GetPropertyInfo(object @object, string propertyName)
        {
            if (string.IsNullOrEmpty(propertyName))
                return null;

            Type objectType = @object.GetType();

            return objectType.GetProperty(propertyName);
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Get property value. </summary>
        /// <param name="object"> Object that should contains property. </param>
        /// <param name="propertyName"> Property name. </param>
        /// <returns> Value object or null. </returns>
        public static object GetPropertyValue(object @object, string propertyName)
        {
            var propertyInfo = GetPropertyInfo(@object, propertyName);

            if (propertyInfo == null)
                return null;

            return propertyInfo.GetValue(@object);
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Chec if property contains attribute with given type. </summary>
        /// <typeparam name="T"> Attribute type. </typeparam>
        /// <param name="object"> Object that should contains attribute. </param>
        /// <param name="propertyName"> Property name. </param>
        /// <returns> True - object contains attribute with given type; False - otherwise. </returns>
        public static bool HasAttribute<T>(object @object, string propertyName) where T : Attribute
        {
            T attribute = GetPropertyAttribute<T>(@object, propertyName);

            return attribute != null;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Check if object contains property with given name. </summary>
        /// <param name="object"> Object that should contains property. </param>
        /// <param name="propertyName"> Property name. </param>
        /// <returns> True - object contains property with given name; False - otherwise. </returns>
        public static bool HasProperty(object @object, string propertyName)
        {
            PropertyInfo propertyInfo = GetPropertyInfo(@object, propertyName);

            return propertyInfo != null;
        }

        #endregion PROPERTY METHODS

        #region THICKNESS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Clone thickness. </summary>
        /// <param name="thickness"> Thickness. </param>
        /// <returns> Thickness clone. </returns>
        public static Thickness CloneThickness(Thickness thickness)
        {
            return new Thickness(thickness.Left, thickness.Top, thickness.Right, thickness.Bottom);
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Modify thickness. </summary>
        /// <param name="thicknes"> Original thickness. </param>
        /// <param name="left"> New left param. </param>
        /// <param name="top"> Net top param. </param>
        /// <param name="right"> New right param. </param>
        /// <param name="bottom"> New bottom param. </param>
        /// <returns></returns>
        public static Thickness ModifyThickness(Thickness thicknes,
            double? left = null, double? top = null, double? right = null, double? bottom = null)
        {
            return new Thickness(
                left ?? thicknes.Left,
                top ?? thicknes.Top,
                right ?? thicknes.Right,
                bottom ?? thicknes.Bottom);
        }

        #endregion THICKNESS METHODS

    }
}
