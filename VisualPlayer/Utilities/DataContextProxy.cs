using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace VisualPlayer.Utilities
{
    public class DataContextProxy : Freezable
    {

        //  DEPENDENCY PROPERTIES

        public static readonly DependencyProperty DataProperty = DependencyProperty.Register(
            "Data",
            typeof(object),
            typeof(DataContextProxy),
            new UIPropertyMetadata(null));


        //  GETTERS & SETTERS

        public object Data
        {
            get { return (object)GetValue(DataProperty); }
            set { SetValue(DataProperty, value); }
        }


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Create data context proxy. </summary>
        /// <returns> Data context proxy. </returns>
        protected override Freezable CreateInstanceCore()
        {
            return new DataContextProxy();
        }

        #endregion CLASS METHODS

    }
}
