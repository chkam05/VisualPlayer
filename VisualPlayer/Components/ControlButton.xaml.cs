using chkam05.VisualPlayer.Components.EventArgs;
using chkam05.VisualPlayer.Data.States;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace chkam05.VisualPlayer.Components
{
    public partial class ControlButton : Button
    {

        //  DEPENDENCY PROPERTIES

        public static readonly DependencyProperty BackgroundPressedBrushProperty =
            DependencyProperty.Register(
                "BackgroundPressedBrush",
                typeof(Brush),
                typeof(ControlButton)
            );

        public static readonly DependencyProperty ForegroundPressedBrushProperty =
            DependencyProperty.Register(
                "ForegroundPressedBrush",
                typeof(Brush),
                typeof(ControlButton)
            );


        //  VARIABLES

        private Brush _backgroundBrush;
        private Brush _foregroundBrush;

        private ControlButtonMode _mode = ControlButtonMode.CLICKABLE;
        private bool _pressed = false;
        private int _stages = 2;
        private int _stage = 0;


        #region GETTERS & SETTERS

        public Brush BackgroundPressedBrush
        {
            get { return (Brush)GetValue(BackgroundPressedBrushProperty); }
            set { SetValue(BackgroundPressedBrushProperty, value); }
        }

        public Brush ForegroundPressedBrush
        {
            get { return (Brush)GetValue(ForegroundPressedBrushProperty); }
            set { SetValue(ForegroundPressedBrushProperty, value); }
        }

        public PackIconKind Icon
        {
            get => PackIcon.Kind;
            set => PackIcon.Kind = value;
        }

        public Thickness IconMargin
        {
            get => PackIcon.Margin;
            set => PackIcon.Margin = value;
        }

        public Size IconSize
        {
            get => new Size(PackIcon.ActualWidth, PackIcon.ActualHeight);
            set
            {
                PackIcon.Height = Math.Max(28, value.Height);
                PackIcon.Width = Math.Max(28, value.Width);
            }
        }

        public ControlButtonMode Mode
        {
            get => _mode;
            set => OnChangeMode(value);
        }

        public bool Pressed
        {
            get => _pressed;
            set
            {
                if (_mode == ControlButtonMode.PRESSABLE)
                    OnPress(value);
            }
        }

        public int Stages
        {
            get => _stages;
            set => _stages = Math.Max(2, value);
        }

        public int Stage
        {
            get => _stage;
            set
            {
                int newStage = Math.Max(0, Math.Min(value, _stages - 1));

                if (_mode == ControlButtonMode.STAGEABLE)
                    OnStageChange(newStage);
            }
        }

        public string Title
        {
            get => TitleTextBlock.Text;
            set
            {
                TitleTextBlock.Text = value;
                ShowHideTitle();
            }
        }

        #endregion GETTERS & SETTERS


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> ControlButton class constructor. </summary>
        public ControlButton()
        {
            InitializeComponent();

            //  Create copy of current background and foreground.
            _backgroundBrush = Background;
            _foregroundBrush = Foreground;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Mathod called after loading button. </summary>
        /// <param name="sender"> Object that invoked event. </param>
        /// <param name="e"> Routed event arguments. </param>
        private void Button_Loaded(object sender, RoutedEventArgs e)
        {
            ShowHideTitle();
        }

        #endregion CLASS METHODS

        #region INTERACTION INTERFACE METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Method called when button is pressed. </summary>
        /// <param name="sender"> Object that dispatched event. </param>
        /// <param name="e"> Event Arguments. </param>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //  Reverse pressed if is pressable.
            if (_mode == ControlButtonMode.PRESSABLE)
                OnPress(!_pressed);

            else if (_mode == ControlButtonMode.STAGEABLE)
                OnStageChange(_stage + 1);
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Change button working mode. </summary>
        /// <param name="newMode"> New button mode. </param>
        private void OnChangeMode(ControlButtonMode newMode)
        {
            //  Change mode.
            _mode = newMode;

            //  Reset pressed lock.
            if (_pressed)
                OnPress(false);

            //  Reset stage.
            _stage = 0;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method that change style of button when is pressed or unpressed. </summary>
        /// <param name="pressed"> Button boolean pressed state. </param>
        private void OnPress(bool pressed)
        {
            if (!_pressed)
            {
                //  Create copy of current background and foreground.
                _backgroundBrush = Background;
                _foregroundBrush = Foreground;
            }

            //  Change button style.
            Background = pressed && BackgroundPressedBrush != null ? BackgroundPressedBrush : _backgroundBrush;
            BorderBrush = pressed ? _backgroundBrush : null;
            BorderThickness = pressed ? new Thickness(2) : new Thickness(0);
            Foreground = pressed && ForegroundPressedBrush != null ? ForegroundPressedBrush : _foregroundBrush;

            //  Save pressed value.
            _pressed = pressed;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method that change style of button when stage is changed. </summary>
        /// <param name="newStage"> Index of next stage. </param>
        private void OnStageChange(int newStage)
        {
            //  Set stage value.
            _stage = newStage % _stages;

            //  Visualise pressed for other stages than first.
            OnPress(_stage != 0);
        }

        #endregion INTERACTION INTERFACE METHODS

        #region INTERFACE MANAGEMENT METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Show or hide title textblock depends on text iside. </summary>
        private void ShowHideTitle()
        {
            if (!string.IsNullOrEmpty(TitleTextBlock.Text))
                TitleTextBlock.Visibility = Visibility.Visible;
            else
                TitleTextBlock.Visibility = Visibility.Collapsed;
        }

        #endregion INTERFACE MANAGEMENT METHODS

    }
}
