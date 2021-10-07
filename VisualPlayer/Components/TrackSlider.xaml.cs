using chkam05.VisualPlayer.Components.EventArgs;
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
    public partial class TrackSlider : UserControl
    {

        //  CONST

        private const int BORDER_CORNER_RADIUS = 4;
        private const int HANDLER_CORNER_RADIUS = 4;
        private const int HANDLER_MARGIN_LR = 2;
        private const int HANDLER_MARGIN_BT = 2;
        public const int MAX_POSITION = 100;


        //  EVENTS

        public event EventHandler<SliderDragEventArgs> OnHandlerDrag;


        //  VARIABLES

        private bool _dragging = false;
        private double _handlerSize = 0;
        private bool _initialized = false;
        private double _position = 0;


        #region GETTERS & SETTERS

        public Brush BackgroundBrush
        {
            get => ComponentBorder.Background;
            set => ComponentBorder.Background = value;
        }

        public bool IsDragging
        {
            get => _dragging;
        }

        public Brush HandlerBrush
        {
            get => Handler.Background;
            set => Handler.Background = value;
        }

        public double HandlerSize
        {
            get => Handler.Width;
            set => UpdateHandleSize(value);
        }

        public new double Height
        {
            get => base.Height;
            set => UpdateComponentSize(base.Width, value);
        }

        public new double Width
        {
            get => base.Width;
            set => UpdateComponentSize(value, base.Height);
        }

        public double Position
        {
            get => _position;
            set => SetPosition(value);
        }

        #endregion GETTERS & SETTERS


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> TrackSlider class constructor. </summary>
        public TrackSlider()
        {
            InitializeComponent();
        }

        #endregion CLASS METHODS

        #region HANDLE MANAGEMENT METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Set handler position on slider based on cursor position. </summary>
        /// <param name="position"> Cursor position over component. </param>
        private void SetPosition(Point position)
        {
            //  Calculate new handler position.
            var halfHandler = Handler.Width / 2;
            var widthSpace = ContentCanvas.ActualWidth - (Handler.Width + HANDLER_MARGIN_LR * 2);
            var newPosition = Math.Max(0, Math.Min(widthSpace, position.X - halfHandler));

            //  Update position value.
            _position = (newPosition * MAX_POSITION) / widthSpace;

            //  Set new handler position.
            Canvas.SetLeft(Handler, newPosition + HANDLER_MARGIN_LR);
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Set handler position on slider based on procentage position. </summary>
        /// <param name="position"> Procentage position of handler on slider. </param>
        private void SetPosition(double position)
        {
            //  Update position value.
            _position = Math.Max(0, Math.Min(position, MAX_POSITION));

            //  Calculate new handler position.
            var widthSpace = ContentCanvas.ActualWidth - (Handler.Width + HANDLER_MARGIN_LR * 2);
            var newPosition = Math.Max(0, (widthSpace * position) / MAX_POSITION);

            //  Set new handler position.
            Canvas.SetLeft(Handler, newPosition + HANDLER_MARGIN_LR);
        }

        #endregion HANDLE MANAGEMENT METHODS

        #region INTERFACE MANAGEMENT METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Update size of the component and subcomponents. </summary>
        /// <param name="width"> New width of the component. </param>
        /// <param name="height"> New height of the component. </param>
        private void UpdateComponentSize(double width, double height)
        {
            //  Update component size.
            base.Height = Math.Max(0, height);
            base.Width = Math.Max(0, width);

            //  Update Handler size.
            UpdateHandleSize(Handler.Width);

            //  Update components corners.
            UpdateUICorners();
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Update size of the handler. </summary>
        /// <param name="width"> New width of the handler. </param>
        /// <param name="height"> New height of the handler. </param>
        private void UpdateHandleSize(double width)
        {
            _handlerSize = Math.Max(0, width);

            Handler.Height = Math.Max(0, ContentCanvas.ActualHeight - (HANDLER_MARGIN_BT * 2));
            Handler.Width = Math.Max(0, Math.Min(ContentCanvas.ActualWidth - (HANDLER_MARGIN_LR * 2), _handlerSize));

            Canvas.SetTop(Handler, HANDLER_MARGIN_BT);
            SetPosition(_position);
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Update components corner radius properties. </summary>
        private void UpdateUICorners()
        {
            ComponentBorder.CornerRadius = new CornerRadius(BORDER_CORNER_RADIUS);
            Handler.CornerRadius = new CornerRadius(HANDLER_CORNER_RADIUS);
        }

        #endregion INTERFACE MANAGEMENT METHODS

        #region SLIDER INTERACTION METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Method called after pressing mouse button on component border. </summary>
        /// <param name="sender"> Object that invoked event. </param>
        /// <param name="e"> Mouse button event arguments. </param>
        private void ComponentBorder_MouseDown(object sender, MouseButtonEventArgs e)
        {
            //  Enable drag functionality.
            _dragging = true;
            e.Handled = true;

            //  Lock Handler to mouse cursor.
            UIElement handler = (UIElement) Handler;
            handler.CaptureMouse();

            //  Get cursor position.
            var cursorPosition = Mouse.GetPosition(ContentCanvas);

            //  Update position of Handler.
            SetPosition(cursorPosition);

            //  Get Handler position.
            var handlerPosition = new Point(Canvas.GetLeft(Handler), Canvas.GetTop(Handler));

            //  Invoke external action.
            OnHandlerDrag?.Invoke(this, 
                new SliderDragEventArgs(SliderDragAction.ON_GRAB, Position, handlerPosition));
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method called after pressing mouse button on handler. </summary>
        /// <param name="sender"> Object that invoked event. </param>
        /// <param name="e"> Mouse button event arguments. </param>
        private void Handler_MouseDown(object sender, MouseButtonEventArgs e)
        {
            //  Enable drag functionality.
            _dragging = true;
            e.Handled = true;

            //  Lock Handler to mouse cursor.
            UIElement handler = (UIElement) Handler;
            handler.CaptureMouse();

            //  Get cursor position.
            var cursorPosition = Mouse.GetPosition(ContentCanvas);

            //  Update position of Handler.
            SetPosition(cursorPosition);

            //  Get Handler position.
            var handlerPosition = new Point(Canvas.GetLeft(Handler), Canvas.GetTop(Handler));

            //  Invoke external action.
            OnHandlerDrag?.Invoke(this,
                new SliderDragEventArgs(SliderDragAction.ON_GRAB, Position, handlerPosition));
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Metod called when cursor is moving over content canvas. </summary>
        /// <param name="sender"> Object that invoked event. </param>
        /// <param name="e"> Mouse event arguments. </param>
        private void ContentCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (_dragging)
            {
                //  Get cursor position.
                var cursorPosition = Mouse.GetPosition(ContentCanvas);

                //  Update position of Handler.
                SetPosition(cursorPosition);

                //  Get Handler position.
                var handlerPosition = new Point(Canvas.GetLeft(Handler), Canvas.GetTop(Handler));

                //  Invoke external action.
                OnHandlerDrag?.Invoke(this,
                    new SliderDragEventArgs(SliderDragAction.ON_DRAG, Position, handlerPosition));
            }
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method called after releasing mouse button on handler. </summary>
        /// <param name="sender"> Object that invoked event. </param>
        /// <param name="e"> Mouse button event arguments. </param>
        private void Handler_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (_dragging)
            {
                //  Disable drag functionality.
                _dragging = false;

                //  Unlock Handler from mouse cursor.
                UIElement handler = (UIElement) Handler;
                handler.ReleaseMouseCapture();

                //  Get cursor position.
                var cursorPosition = Mouse.GetPosition(ContentCanvas);

                //  Update position of Handler.
                SetPosition(cursorPosition);

                //  Get Handler position.
                var handlerPosition = new Point(Canvas.GetLeft(Handler), Canvas.GetTop(Handler));

                //  Invoke external action.
                OnHandlerDrag?.Invoke(this,
                    new SliderDragEventArgs(SliderDragAction.ON_RELEASE, Position, handlerPosition));
            }
        }

        #endregion SLIDER INTERACTION METHODS

        #region USER CONTROL METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Mathod called after loading user control. </summary>
        /// <param name="sender"> Object that invoked event. </param>
        /// <param name="e"> Routed event arguments. </param>
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            //  Update initialization flag.
            _initialized = true;

            //  Adjust component size.
            UpdateComponentSize(base.Width, base.Height);
            Position = 0;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method called after size of user control is changed. </summary>
        /// <param name="sender"> Object that invoked event. </param>
        /// <param name="e"> Size changed event arguments. </param>
        private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            //  Update Handler size.
            UpdateHandleSize(_handlerSize);

            //  Update components corners.
            UpdateUICorners();
        }

        #endregion USER CONTROL METHODS

    }
}
