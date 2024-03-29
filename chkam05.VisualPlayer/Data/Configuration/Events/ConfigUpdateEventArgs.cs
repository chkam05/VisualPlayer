﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chkam05.VisualPlayer.Data.Configuration.Events
{
    public class ConfigUpdateEventArgs : EventArgs
    {

        //  VARIABLES

        private Type _propertyType;

        public string PropertyName { get; private set; }
        public object PropertyValue { get; private set; }


        //  GETTERS & SETTERS

        public Type PropertyType
        {
            get => _propertyType == null ? typeof(object) : _propertyType;
        }


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        public ConfigUpdateEventArgs(string propertyName, object propertyValue = null, Type propertyType = null)
        {
            PropertyName = propertyName;
            PropertyValue = propertyValue;
            _propertyType = propertyType;
        }

        #endregion CLASS METHODS

    }
}
