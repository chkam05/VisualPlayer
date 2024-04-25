using chkam05.VisualPlayer.Controls.Data;
using chkam05.VisualPlayer.Utilities;
using chkam05.VisualPlayer.Visualisations;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
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

namespace chkam05.VisualPlayer.Controls
{
    public partial class LogoControl : UserControl, INotifyPropertyChanged
    {

        //  CONST

        public const double MIN_ROTATION = -360;
        public const double MAX_ROTATION = 360;
        public const double MIN_SCALE = 1.0;


        //  DEPENDENCY PROPERTIES

        public static readonly DependencyProperty LockVisibilityProperty = DependencyProperty.Register(
            nameof(LockVisibility),
            typeof(bool),
            typeof(LogoControl),
            new PropertyMetadata(false));

        public static readonly DependencyProperty TransformRotateProperty = DependencyProperty.Register(
            nameof(TransformRotate),
            typeof(double),
            typeof(LogoControl),
            new PropertyMetadata(0.0));

        public static readonly DependencyProperty TransformScaleBoundariesProperty = DependencyProperty.Register(
            nameof(TransformScaleBoundaries),
            typeof(Thickness),
            typeof(LogoControl),
            new PropertyMetadata(new Thickness(0)));

        public static readonly DependencyProperty TransformScaleMaxProperty = DependencyProperty.Register(
            nameof(TransformScaleMax),
            typeof(double),
            typeof(LogoControl),
            new PropertyMetadata(double.NaN));

        public static readonly DependencyProperty TransformScaleMinProperty = DependencyProperty.Register(
            nameof(TransformScaleMin),
            typeof(double),
            typeof(LogoControl),
            new PropertyMetadata(2.0));

        public static readonly DependencyProperty TransformScaleShiftProperty = DependencyProperty.Register(
            nameof(TransformScaleShift),
            typeof(double),
            typeof(LogoControl),
            new PropertyMetadata(0.0));

        public static readonly DependencyProperty TransformScaleXProperty = DependencyProperty.Register(
            nameof(TransformScaleX),
            typeof(double),
            typeof(LogoControl),
            new PropertyMetadata(1.0));
        
        public static readonly DependencyProperty TransformScaleYProperty = DependencyProperty.Register(
            nameof(TransformScaleY),
            typeof(double),
            typeof(LogoControl),
            new PropertyMetadata(1.0));
        
        public static readonly DependencyProperty TransformSkewXProperty = DependencyProperty.Register(
            nameof(TransformSkewX),
            typeof(double),
            typeof(LogoControl),
            new PropertyMetadata(0.0));
        
        public static readonly DependencyProperty TransformSkewYProperty = DependencyProperty.Register(
            nameof(TransformSkewY),
            typeof(double),
            typeof(LogoControl),
            new PropertyMetadata(0.0));
        
        public static readonly DependencyProperty TransformXProperty = DependencyProperty.Register(
            nameof(TransformX),
            typeof(double),
            typeof(LogoControl),
            new PropertyMetadata(0.0));
        
        public static readonly DependencyProperty TransformYProperty = DependencyProperty.Register(
            nameof(TransformY),
            typeof(double),
            typeof(LogoControl),
            new PropertyMetadata(0.0));


        //  EVENTS

        public event PropertyChangedEventHandler PropertyChanged;


        //  VARIABLES

        private List<ShapeContainer> _shapes;


        //  GETTERS & SETTERS

        public bool IsShowed
        {
            get => Visibility == Visibility.Visible;
        }

        public bool LockVisibility
        {
            get => (bool)GetValue(LockVisibilityProperty);
            set
            {
                SetValue(LockVisibilityProperty, value);
                OnPropertyChanged(nameof(LockVisibility));
            }
        }

        public List<ShapeContainer> Shapes
        {
            get => _shapes;
            set
            {
                _shapes = value;
                OnPropertyChanged(nameof(Shapes));
                UpdateLogoUsingShapes();
            }
        }

        public Size Size
        {
            get => new Size(this.ActualWidth, this.ActualHeight);
        }
        public double TransformRotate
        {
            get => (double)GetValue(TransformRotateProperty);
            set
            {
                SetValue(TransformRotateProperty, Math.Max(Math.Min(MAX_ROTATION, value), MIN_ROTATION));
                OnPropertyChanged(nameof(TransformRotate));
            }
        }

        public Thickness TransformScaleBoundaries
        {
            get => (Thickness)GetValue(TransformScaleBoundariesProperty);
            set
            {
                SetValue(TransformScaleBoundariesProperty, value);
                OnPropertyChanged(nameof(TransformScaleBoundaries));
            }
        }

        public double TransformScaleMin
        {
            get => (double)GetValue(TransformScaleMinProperty);
            set
            {
                SetValue(TransformScaleBoundariesProperty, Math.Max(MIN_SCALE, value));
                OnPropertyChanged(nameof(TransformScaleBoundaries));
            }
        }

        public double TransformScaleMax
        {
            get => (double)GetValue(TransformScaleMaxProperty);
            set
            {
                SetValue(TransformScaleMaxProperty, Math.Max(TransformScaleMin, value));
                OnPropertyChanged(nameof(TransformScaleMax));
            }
        }
        
        public double TransformScaleShift
        {
            get => (double)GetValue(TransformScaleShiftProperty);
            set
            {
                SetValue(TransformScaleShiftProperty, value);
                OnPropertyChanged(nameof(TransformScaleShift));
            }
        }

        public double TransformScaleX
        {
            get => (double)GetValue(TransformScaleXProperty);
            set
            {
                SetValue(TransformScaleXProperty, Math.Max(MIN_SCALE, value));
                OnPropertyChanged(nameof(TransformScaleX));
            }
        }

        public double TransformScaleY
        {
            get => (double)GetValue(TransformScaleYProperty);
            set
            {
                SetValue(TransformScaleYProperty, Math.Max(MIN_SCALE, value));
                OnPropertyChanged(nameof(TransformScaleY));
            }
        }

        public double TransformSkewX
        {
            get => (double)GetValue(TransformSkewXProperty);
            set
            {
                SetValue(TransformSkewXProperty, value);
                OnPropertyChanged(nameof(TransformSkewX));
            }
        }

        public double TransformSkewY
        {
            get => (double)GetValue(TransformSkewYProperty);
            set
            {
                SetValue(TransformSkewYProperty, value);
                OnPropertyChanged(nameof(TransformSkewY));
            }
        }

        public double TransformX
        {
            get => (double)GetValue(TransformXProperty);
            set
            {
                SetValue(TransformXProperty, value);
                OnPropertyChanged(nameof(TransformX));
            }
        }

        public double TransformY
        {
            get => (double)GetValue(TransformYProperty);
            set
            {
                SetValue(TransformYProperty, value);
                OnPropertyChanged(nameof(TransformY));
            }
        }


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> LogoControl class constructor. </summary>
        public LogoControl()
        {
            //  Setup data containers.
            _shapes = new List<ShapeContainer>();

            //  Initialize interface and components.
            InitializeComponent();
        }

        #endregion CLASS METHODS

        #region CREATE LOGO METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Create shapes from resource. </summary>
        /// <param name="resourceData"> Resource as byte array. </param>
        public void CreateLogoFromResource(byte[] resourceData)
        {
            try
            {
                using (var memoryStream = new MemoryStream(resourceData))
                {
                    using (var streamReader = new StreamReader(memoryStream))
                    {
                        string logoJsonData = streamReader.ReadToEnd();
                        var shapes = JsonConvert.DeserializeObject<List<ShapeContainer>>(logoJsonData);

                        if (shapes != null && shapes.Any())
                            Shapes = shapes;
                    }
                }
            }
            catch (Exception)
            {
                //
            }
        }

        #endregion CREATE LOGO METHODS

        #region INTERFACE MANAGEMENT METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Show logo control interface. </summary>
        public void ShowInterface()
        {
            if (!LockVisibility)
                this.Visibility = Visibility.Visible;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Hide logo control interface. </summary>
        public void HideInterface()
        {
            if (!LockVisibility)
                this.Visibility = Visibility.Collapsed;
        }

        #endregion INTERFACE MANAGEMENT METHODS

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

        #region SCALING METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Scale logo to object size with shift. </summary>
        /// <param name="destObjectSize"> Scaling destination object size. </param>
        /// <param name="shift"> Scale shift. </param>
        public void ScaleToObject(Size destObjectSize, double shift)
        {
            var boundaries = TransformScaleBoundaries;
            var updatedObjectSize = new Size(
                Math.Max(0, destObjectSize.Width - (boundaries.Left + boundaries.Right)),
                Math.Max(0, destObjectSize.Height - (boundaries.Top + boundaries.Bottom)));
            var visibility = Visibility;

            //  Calculate scaling.
            if (visibility != Visibility.Visible)
                Visibility = Visibility.Visible;

            var maxScale = MathUtilities.CalculateTransformScale(Size, updatedObjectSize);

            Visibility = visibility;

            if (!double.IsNaN(TransformScaleMax))
                maxScale = Math.Min(TransformScaleMax, maxScale);

            //  Calculate scaling parts.
            var scalingPart = maxScale / 2;
            var shiftPart = scalingPart * shift / 1.0;
            var scale = Math.Min(maxScale, 
                Math.Max(TransformScaleMin, (scalingPart + shiftPart) - TransformScaleShift));

            TransformScaleX = scale;
            TransformScaleY = scale;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Scale logo to object size. </summary>
        /// <param name="destObjectSize"> Scaling destination object size. </param>
        public void ScaleToObject(Size destObjectSize)
        {
            var boundaries = TransformScaleBoundaries;
            var updatedObjectSize = new Size(
                Math.Max(0, destObjectSize.Width - (boundaries.Left + boundaries.Right)),
                Math.Max(0, destObjectSize.Height - (boundaries.Top + boundaries.Bottom)));
            var visibility = Visibility;

            //  Calculate scaling.
            if (visibility != Visibility.Visible)
                Visibility = Visibility.Visible;

            var maxScale = MathUtilities.CalculateTransformScale(Size, updatedObjectSize);

            Visibility = visibility;

            if (!double.IsNaN(TransformScaleMax))
                maxScale = Math.Min(TransformScaleMax, maxScale);

            //  Scale.
            var scalingPart = maxScale / 2;
            var scale = Math.Max(TransformScaleMin, scalingPart - TransformScaleShift);

            TransformScaleX = scale;
            TransformScaleY = scale;
        }

        #endregion SCALING METHODS

        #region UPDATE LOGO METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Set shapes global background brush. </summary>
        /// <param name="backgroundBrush"> Background brush. </param>
        public void SetShapesBackground(Brush backgroundBrush)
        {
            if (_shapes != null && _shapes.Any())
                _shapes.ForEach(s => s.Background = backgroundBrush);
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Set shapes global border brush. </summary>
        /// <param name="borderBrush"> Border brush. </param>
        public void SetShapesBorderBrush(Brush borderBrush)
        {
            if (_shapes != null && _shapes.Any())
                _shapes.ForEach(s => s.BorderBrush = borderBrush);
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Set shapes global thickness. </summary>
        /// <param name="thickness"> Thickness. </param>
        public void SetShapesThickness(double thickness)
        {
            if (_shapes != null && _shapes.Any())
                _shapes.ForEach(s => s.BorderThickness = Math.Max(1, thickness));
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Clear shapes container. </summary>
        private void RemoveLogo()
        {
            containerGrid.Children.Clear();
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Update logo drawing shapes in interface. </summary>
        private void UpdateLogoUsingShapes()
        {
            RemoveLogo();

            if (Shapes != null && Shapes.Any())
            {
                foreach (var shapeContainer in Shapes)
                {
                    var shape = shapeContainer.Shape;

                    if (!containerGrid.Children.Contains(shape))
                        containerGrid.Children.Add(shape);

                    int zIndex = Canvas.GetZIndex(shape);
                    if (zIndex > 0)
                        Canvas.SetZIndex(shape, -1);
                }
            }
        }

        #endregion UPDATE LOGO METHODS

    }
}
