using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using VisualPlayer.Controls;
using VisualPlayer.Data.Attributes;
using VisualPlayer.Data.ColorModels;
using VisualPlayer.InternalMessages.Enums;
using VisualPlayer.Utilities;

namespace VisualPlayer.InternalMessages.Base
{
    public abstract class IMBase : Page, INotifyPropertyChanged
    {

        //  DELEGATES

        public delegate void CloseEventHandler<T>(object sender, T e) where T: IIMCloseEventArgs;


        //  DEPENDENCY PROPERTIES

        public static readonly DependencyProperty BorderBrushProperty = DependencyProperty.Register(
            nameof(BorderBrush),
            typeof(Brush),
            typeof(IMBase),
            new PropertyMetadata(new SolidColorBrush(BaseColors.DefaultAccentColor)));

        public static readonly DependencyProperty BorderThicknessProperty = DependencyProperty.Register(
            nameof(BorderThickness),
            typeof(Thickness),
            typeof(IMBase),
            new PropertyMetadata(new Thickness(1)));

        public static readonly DependencyProperty CornerRadiusProperty = DependencyProperty.Register(
            nameof(CornerRadius),
            typeof(CornerRadius),
            typeof(IMBase),
            new PropertyMetadata(new CornerRadius(8)));

        public static readonly DependencyProperty PaddingProperty = DependencyProperty.Register(
            nameof(Padding),
            typeof(Thickness),
            typeof(IMBase),
            new PropertyMetadata(new Thickness(0)));


        //  EVENTS

        public CloseEventHandler<IIMCloseEventArgs> Closed;
        public event PropertyChangedEventHandler PropertyChanged;


        //  VARIABLES

        public IIMControl IMControl { get; private set; }

        public bool IsShowed { get; private set; }


        //  GETTERS & SETTERS

        public Brush BorderBrush
        {
            get => (Brush)GetValue(BorderBrushProperty);
            set => SetValue(BorderBrushProperty, value);
        }

        public Thickness BorderThickness
        {
            get => (Thickness)GetValue(BorderThicknessProperty);
            set => SetValue(BorderThicknessProperty, value);
        }

        public CornerRadius CornerRadius
        {
            get => (CornerRadius)GetValue(CornerRadiusProperty);
            set => SetValue(CornerRadiusProperty, value);
        }

        public Thickness Padding
        {
            get => (Thickness)GetValue(PaddingProperty);
            set => SetValue(PaddingProperty, value);
        }


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Static IMBase class constructor. </summary>
        static IMBase()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(IMBase),
                new FrameworkPropertyMetadata(typeof(IMBase)));
        }

        //  --------------------------------------------------------------------------------
        /// <summary> IMBase class constructor. </summary>
        /// <param name="imControl"> Internal messages control interface. </param>
        public IMBase(IIMControl imControl)
        {
            //  Setup data.
            IsShowed = false;
            IMControl = imControl;

            //  Setup events.
            Loaded += OnIMLoaded;
            Unloaded += OnIMUnloaded;
        }

        #endregion CLASS METHODS

        #region CLOSE EVENT HANDLER

        //  --------------------------------------------------------------------------------
        /// <summary> Create close event arguments. </summary>
        /// <param name="result"> Internal message result </param>
        /// <returns> Internal message result event arguments interface. </returns>
        protected virtual IIMCloseEventArgs CreateCloseEventArgs(IMResult result)
        {
            return new IMCloseEventArgs(result);
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Invoke closed event handler. </summary>
        /// <param name="result"> Internal message result. </param>
        protected virtual void InvokeClosedEventHandler(IMResult? result = null)
        {
            var finalResult = result ?? IMResult.Cancel;

            Closed?.Invoke(this, CreateCloseEventArgs(finalResult));
        }

        #endregion CLOSE EVENT HANDLER

        #region INTERACTION METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Close internal message. </summary>
        public virtual void Close()
        {
            InvokeClosedEventHandler(IMResult.Cancel);
        }

        #endregion INTERACTION METHODS

        #region INTERNAL MESSAGE METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after loading internal message. </summary>
        /// <param name="sender"> Object that invoked the method. </param>
        /// <param name="e"> Routed Event Arguments. </param>
        protected virtual void OnIMLoaded(object sender, RoutedEventArgs e)
        {
            IsShowed = true;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after unloading internal message. </summary>
        /// <param name="sender"> Object that invoked the method. </param>
        /// <param name="e"> Routed Event Arguments. </param>
        protected virtual void OnIMUnloaded(object sender, RoutedEventArgs e)
        {
            IsShowed = false;
        }

        #endregion INTERNAL MESSAGE METHODS

        #region NOTIFICATION PROPERTIES CHANGED METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Trigger notify property changed in user interface. </summary>
        /// <param name="propertyName"> Property name. </param>
        protected virtual void NotifyPropertyChanged(string propertyName)
        {
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

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion NOTIFICATION PROPERTIES CHANGED METHODS

    }
}
