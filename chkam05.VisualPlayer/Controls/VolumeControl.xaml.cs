using chkam05.Tools.ControlsEx;
using chkam05.VisualPlayer.Components;
using chkam05.VisualPlayer.Components.Events;
using chkam05.VisualPlayer.Controls.Events;
using chkam05.VisualPlayer.Data.Config;
using chkam05.VisualPlayer.Data.Configuration;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace chkam05.VisualPlayer.Controls
{
    public partial class VolumeControl : UserControl, INotifyPropertyChanged
    {

        //  CONST

        public static readonly double MIN_VOLUME = 0.0;
        public static readonly double MAX_VOLUME = 100.0;


        //  DEPENDENCY PROPERTIES

        public static readonly DependencyProperty VolumeProperty = DependencyProperty.Register(
            nameof(Volume),
            typeof(double),
            typeof(VolumeControl),
            new PropertyMetadata(50.0));


        //  EVENTS

        public event PropertyChangedEventHandler PropertyChanged;
        public event EventHandler<RoutedEventArgs> OnVolumeMuteClick;
        public event EventHandler<SliderValueChangedEventArgs<double>> OnSliderValueChanged;
        public event EventHandler<VolumeControlAnimateEventArgs> OnAnimate;
        public event EventHandler<VolumeControlAnimationFinishEventArgs> OnAnimationFinish;


        //  VARIABLES

        private double _controlHeight = 0;
        private bool _volumeSliderInUse = false;

        public Configuration Configuration { get; private set; }
        public ConfigManager ConfigManager { get; private set; }


        //  GETTERS & SETTERS

        public double ControlHeight
        {
            get => _controlHeight;
            set
            {
                _controlHeight = value;
                OnPropertyChanged(nameof(ControlHeight));
            }
        }

        public bool IsExpanded
        {
            get => ((int)VolumeControlGrid.ActualHeight) > 0;
        }

        public double Volume
        {
            get => (double)GetValue(VolumeProperty);
            set
            {
                if (_volumeSliderInUse)
                    return;

                SetValue(VolumeProperty, Math.Max(MIN_VOLUME, Math.Min(value, MAX_VOLUME)));
                OnPropertyChanged(nameof(Volume));
            }
        }


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> VolumeControl class constructor. </summary>
        public VolumeControl()
        {
            //  Setup modules.
            Configuration = Configuration.Instance;
            ConfigManager = ConfigManager.Instance;

            //  Initialize interface and components.
            InitializeComponent();

            //  Setup initial data.
            VolumeControlGrid.Height = 0;
        }

        #endregion CLASS METHODS

        #region ANIMATION METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Show VolumeControl interface. </summary>
        private void AnimateShowInterface()
        {
            if (((int)VolumeControlGrid.ActualHeight) < 1)
            {
                Storyboard storyboard = Resources["ShowHideStoryboard"] as Storyboard;
                ControlHeight = VolumeControlGrid.MaxHeight;
                OnAnimate?.Invoke(this, new VolumeControlAnimateEventArgs(true));
                storyboard?.Begin();
            }
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Hide VolumeControl interface. </summary>
        private void AnimateHideInterface()
        {
            if (((int)VolumeControlGrid.ActualHeight) > 0)
            {
                Storyboard storyboard = Resources["ShowHideStoryboard"] as Storyboard;
                ControlHeight = VolumeControlGrid.MinHeight;
                OnAnimate?.Invoke(this, new VolumeControlAnimateEventArgs(false));
                storyboard?.Begin();
            }
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after completing ExpandingStoryboard animation. </summary>
        /// <param name="sender"> Object that invoked method. </param>
        /// <param name="e"> Event Arguments. </param>
        private void Storyboard_Completed(object sender, EventArgs e)
        {
            OnAnimationFinish?.Invoke(this, new VolumeControlAnimationFinishEventArgs(IsExpanded));
        }

        #endregion ANIMATION METHODS

        #region BUTTONS CONTROL METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after clicking Volume ControlButton. </summary>
        /// <param name="sender"> Object that invoked method. </param>
        /// <param name="e"> Routed Event Arguments. </param>
        private void VolumeButton_Click(object sender, RoutedEventArgs e)
        {
            OnVolumeMuteClick?.Invoke(this, e);
        }

        #endregion BUTTONS CONTROL METHODS

        #region INTERFACE MANAGEMENT METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Show interface programmatically. </summary>
        public void ShowInterface()
        {
            AnimateShowInterface();
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Hide interface programmatically. </summary>
        /// <param name="ifNotMouseOver"> Only if cursor is not over control. </param>
        public void HideInterface(bool ifNotMouseOver = true)
        {
            if (ifNotMouseOver && IsMouseOver)
                return;

            AnimateHideInterface();
        }

        #endregion INTERFACE MANAGEMENT METHODS

        #region MOVEMENT INTERACTION METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after entering cursor over Volume ControlBarMenu grid. </summary>
        /// <param name="sender"> Object that invoked method. </param>
        /// <param name="e"> Mouse Event Arguments. </param>
        private void VolumeControlGrid_MouseEnter(object sender, MouseEventArgs e)
        {
            //
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after leaving cursor from Volume ControlBarMenu grid. </summary>
        /// <param name="sender"> Object that invoked method. </param>
        /// <param name="e"> Mouse Event Arguments. </param>
        private void VolumeControlGrid_MouseLeave(object sender, MouseEventArgs e)
        {
            AnimateHideInterface();
        }

        #endregion MOVEMENT INTERACTION METHODS

        #region NOTIFY PROPERTIES CHANGED INTERFACE METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Method for invoking PropertyChangedEventHandler external method. </summary>
        /// <param name="propertyName"> Changed property name. </param>
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;

            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion NOTIFY PROPERTIES CHANGED INTERFACE METHODS

        #region SLIDER CONTROL METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after clicking on ControlSlider. </summary>
        /// <param name="sender"> Object that invoked method. </param>
        /// <param name="e"> Mouse Event Arguments. </param>
        private void ControlSlider_GotMouseCapture(object sender, MouseEventArgs e)
        {
            _volumeSliderInUse = true;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after releasing click from ControlSlider. </summary>
        /// <param name="sender"> Object that invoked method. </param>
        /// <param name="e"> Mouse Event Arguments. </param>
        private void ControlSlider_LostMouseCapture(object sender, MouseEventArgs e)
        {
            if (_volumeSliderInUse)
            {
                var slider = (SliderEx)sender;
                var args = new SliderValueChangedEventArgs<double>(slider.Value, true);
                OnSliderValueChanged?.Invoke(sender, args);
            }

            _volumeSliderInUse = false;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after changing value of ControlSlider. </summary>
        /// <param name="sender"> Object that invoked method. </param>
        /// <param name="e"> Routed Property Changed Event Arguments. </param>
        private void ControlSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (_volumeSliderInUse)
            {
                var args = new SliderValueChangedEventArgs<double>(e.NewValue, false);
                OnSliderValueChanged?.Invoke(sender, args);
            }
        }

        #endregion SLIDER CONTROL METHODS

        #region USER CONTROL METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after loading user control. </summary>
        /// <param name="sender"> Object that invoked method. </param>
        /// <param name="e"> Routed Event Arguments. </param>
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            //
        }

        #endregion USER CONTROL METHODS

    }
}
