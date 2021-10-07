using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
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

namespace chkam05.InternalMessages.Pages
{
    public partial class InternalMessageLoader : Page, IInternalMessage
    {

        //  CONST

        private const double INDICATOR_HEIGHT = 8;


        //  VARIABLES

        private InternalMessagesContainer _container;
        private bool _loaded = false;

        private bool _animationStopper = false;
        private BackgroundWorker _animationWorker;
        
        private Point _indicatorCenterPoint;
        private Geometry _indicatorGeometry;
        private double _indicatorRadius;
        private double _indicatorRotation = 0;


        #region GETTERS & SETTERS

        public string Message
        {
            get => MessageTextBlock.Text;
            set => MessageTextBlock.Text = value;
        }
        public new string Title
        {
            get => TitleTextBlock.Text;
            set => TitleTextBlock.Text = value;
        }
        public PackIconKind Icon
        {
            get => PackIcon.Kind;
            set => PackIcon.Kind = value;
        }
        public Brush IconColor
        {
            get => PackIcon.Foreground;
            set => PackIcon.Foreground = value;
        }

        public Brush IndicatorColor
        {
            get => Indicator.Fill;
            set => Indicator.Fill = value;
        }

        #endregion GETTERS & SETTERS


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> InternalMessageLoader class constructor. </summary>
        /// <param name="container"> Messages container. </param>
        public InternalMessageLoader(InternalMessagesContainer container)
        {
            //  Setup variables.
            _container = container;

            //  Setup background worker.
            _animationWorker = new BackgroundWorker();
            _animationWorker.WorkerReportsProgress = true;
            _animationWorker.DoWork += AnimationWorker;
            _animationWorker.ProgressChanged += AnimationProgressChanged;

            InitializeComponent();
        }

        #endregion CLASS METHODS

        #region INDICATOR ANIMATION METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Set calculated indicator geomatry data and rotation. </summary>
        /// <param name="sender"> Object that invoked event. </param>
        /// <param name="e"> Progress change event arguments. </param>
        private void AnimationProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (_loaded)
            {
                Indicator.Data = _indicatorGeometry;
                IndicatorOffset.Angle = _indicatorRotation;
            }
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Calculate next indicator geomatry data and rotation in background. </summary>
        /// <param name="sender"> Object that invoked event. </param>
        /// <param name="e"> Do work event arguments. </param>
        private void AnimationWorker(object sender, DoWorkEventArgs e)
        {
            bool sideSwitch = false;
            double step = 2;
            double startAngle = 270;
            double stopAngle = 270;

            if (_loaded)
            {
                while (!_animationStopper)
                {
                    if (sideSwitch)
                    {
                        startAngle = (startAngle + step) % 360;
                        if (startAngle == stopAngle)
                            sideSwitch = false;
                    }
                    else
                    {
                        stopAngle = (stopAngle + step) % 360;

                        if (startAngle >= 180 && stopAngle < 180 && (stopAngle + 360) - startAngle >= (180 - step))
                            sideSwitch = true;
                        else if (startAngle < 180 && stopAngle - startAngle >= (180 - step))
                            sideSwitch = true;
                    }

                    _indicatorRotation = (_indicatorRotation + 0.5) % 360;
                    _indicatorGeometry = CalculateIdicatorGeometry(startAngle, stopAngle);
                    (sender as BackgroundWorker).ReportProgress(0);
                    Thread.Sleep(5);
                }
            }
        }

        #endregion INDICATOR ANIMATION METHODS

        #region INTERFACE METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Close message and load previous message if was showen. </summary>
        /// <returns> Previous showen message or NULL. </returns>
        public IInternalMessage Close()
        {
            //  Close message and return previous message if was showen or null.
            return _container.Close();
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Check if interface type is particular type of internal message. </summary>
        /// <param name="type"> Internal message type to check. </param>
        /// <returns> True - interface is type of internal message type; False - otherwise. </returns>
        public bool IsTypeOf(Type type) => this.GetType() == type;

        #endregion INTERFACE METHODS

        #region PAGE METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Mathod called after loading page. </summary>
        /// <param name="sender"> Object that invoked event. </param>
        /// <param name="e"> Routed event arguments. </param>
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            //  Setup indicator size.
            Indicator.Height = IndicatorCanvas.Height;
            Indicator.Width = IndicatorCanvas.Width;

            //  Setup indicator points and radius.
            _indicatorCenterPoint = new Point(
                IndicatorCanvas.Width / 2,
                IndicatorCanvas.Height / 2);
            _indicatorRadius = (IndicatorCanvas.Width / 2) - 2;

            //  Set loaded flag to True.
            _loaded = true;

            //  Run animation background worker.
            _animationWorker.RunWorkerAsync();
        }

        #endregion PAGE METHODS

        #region UTILITY METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Calculate point placed on circle border. </summary>
        /// <param name="radius"> Radius of circle. </param>
        /// <param name="angle"> Point angle. </param>
        /// <param name="offset"> Point offset. </param>
        /// <returns> Point placed on circle border. </returns>
        private Point CalculateArcPoint(double radius, double angle, Point offset)
        {
            //  Prevent offset to be null.
            if (offset == null)
                offset = new Point(0, 0);

            //  Convert degree to radian.
            double rad = (angle * Math.PI) / 180.0;

            //  Calculate point on cirlce with 0,0 center point.
            double x = radius * Math.Cos(rad);
            double y = radius * Math.Sin(rad);

            //  Return point on circle.
            return new Point(x + offset.X, y + offset.Y);
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Create loading indicator geometry. </summary>
        /// <param name="startAngle"> Indicator start angle. </param>
        /// <param name="stopAngle"> Indicator end angle. </param>
        /// <returns> Loading indicator geometry. </returns>
        private Geometry CalculateIdicatorGeometry(double startAngle, double stopAngle)
        {
            //  Calculate points for indicator gemoetry.
            var points = new Point[]
            {
               CalculateArcPoint(_indicatorRadius, startAngle, _indicatorCenterPoint),
               CalculateArcPoint(_indicatorRadius, stopAngle, _indicatorCenterPoint),
               CalculateArcPoint(_indicatorRadius - INDICATOR_HEIGHT, stopAngle, _indicatorCenterPoint),
               CalculateArcPoint(_indicatorRadius - INDICATOR_HEIGHT, startAngle, _indicatorCenterPoint)
            };

            //  Create and return geometry data.
            return Geometry.Parse($"M{(int)points[0].X},{(int)points[0].Y} " +
                $"A{(int)_indicatorRadius},{(int)_indicatorRadius} 0 0 1 {(int)points[1].X},{(int)points[1].Y} " +
                $"L{(int)points[2].X},{(int)points[2].Y} " +
                $"A{(int)_indicatorRadius - INDICATOR_HEIGHT},{(int)_indicatorRadius - INDICATOR_HEIGHT} 0 0 0 {(int)points[3].X},{(int)points[3].Y} " +
                $"z");
        }

        #endregion UTILITY METHODS

    }
}
