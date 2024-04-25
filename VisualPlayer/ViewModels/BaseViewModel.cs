using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using VisualPlayer.Data.Attributes;
using VisualPlayer.Utilities;

namespace VisualPlayer.ViewModels
{
    public class BaseViewModel : INotifyPropertyChanged
    {

        //  EVENTS

        public event PropertyChangedEventHandler PropertyChanged;


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> BaseViewModel class constructor. </summary>
        public BaseViewModel()
        {
            //
        }

        #endregion CLASS METHODS

        #region NOTIFICATION PROPERTIES CHANGED METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Check notify property update attribute to allow notify property changed in user interface. </summary>
        /// <param name="propertyName"> Property name. </param>
        /// <returns> True - notify property changed allowed; False - otherwise. </returns>
        protected virtual bool CheckNotifyPropertyUpdateAttribute(string propertyName)
        {
            var attribute = ObjectHelper.GetPropertyAttribute<NotifyPropertyUpdateAttribute>(this, propertyName);

            if (attribute == null)
                return true;

            return attribute.AllowNotify;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Trigger notify property changed in user interface. </summary>
        /// <param name="propertyName"> Property name. </param>
        protected virtual void NotifyPropertyChanged(string propertyName)
        {
            if (!CheckNotifyPropertyUpdateAttribute(propertyName))
                return;

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Update property value and trigger notify property changed in user interface. </summary>
        /// <typeparam name="T"> Property type. </typeparam>
        /// <param name="field"> Reference to property field. </param>
        /// <param name="newValue"> New value. </param>
        /// <param name="propertyName"> Property name. </param>
        protected virtual void UpdateProperty<T>(ref T field, T newValue, [CallerMemberName] string propertyName = null)
        {
            if (string.IsNullOrEmpty(propertyName))
                throw new ArgumentException("Property update failed. Property name cannot be null or empty.");

            field = newValue;

            if (!CheckNotifyPropertyUpdateAttribute(propertyName))
                return;

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion NOTIFICATION PROPERTIES CHANGED METHODS

    }
}
