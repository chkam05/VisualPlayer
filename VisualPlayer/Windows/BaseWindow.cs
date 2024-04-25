using MaterialDesignThemes.Wpf;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace VisualPlayer.Windows
{
    public class BaseWindow : Window
    {

        //  CONST

        private const string BUTTON_MINIMIZE_NAME = "_minimizeButton";
        private const string BUTTON_MAXIMIZE_NAME = "_maximizeButton";
        private const string BUTTON_CLOSE_NAME = "_closeButton";
        private const string RESIZE_BORDER_LEFT_NAME = "_resizeBorderLeft";
        private const string RESIZE_BORDER_TOP_LEFT_NAME = "_resizeBorderTopLeft";
        private const string RESIZE_BORDER_TOP_NAME = "_resizeBorderTop";
        private const string RESIZE_BORDER_TOP_RIGHT_NAME = "_resizeBorderTopRight";
        private const string RESIZE_BORDER_RIGHT_NAME = "_resizeBorderRight";
        private const string RESIZE_BORDER_BOTTOM_RIGHT_NAME = "_resizeBorderBottomRight";
        private const string RESIZE_BORDER_BOTTOM_NAME = "_resizeBorderBottom";
        private const string RESIZE_BORDER_BOTTOM_LEFT_NAME = "_resizeBorderBottomLeft";
        private const string TITLE_GRID_NAME = "_titleGrid";


        //  DEPENDENCY PROPERTIES

        public static readonly DependencyProperty BackgroundOpacityProperty = DependencyProperty.Register(
            nameof(BackgroundOpacity),
            typeof(double),
            typeof(BaseWindow),
            new PropertyMetadata(1d));

        public static readonly DependencyProperty CornerRadiusProperty = DependencyProperty.Register(
            nameof(CornerRadius),
            typeof(CornerRadius),
            typeof(BaseWindow),
            new PropertyMetadata(new CornerRadius(8)));

        public static readonly DependencyProperty IconKindProperty = DependencyProperty.Register(
            nameof(IconKind),
            typeof(PackIconKind),
            typeof(BaseWindow),
            new PropertyMetadata(PackIconKind.ApplicationOutline));

        public static readonly DependencyProperty ImageSourceProperty = DependencyProperty.Register(
            nameof(ImageSource),
            typeof(ImageSource),
            typeof(BaseWindow),
            new PropertyMetadata(null));


        //  VARIABLES

        protected double _posTop, _posLeft;
        protected double _startW, _startH;
        protected double _startX, _startY;


        //  GETTERS & SETTERS

        public double BackgroundOpacity
        {
            get => (double)GetValue(BackgroundOpacityProperty);
            set => SetValue(BackgroundOpacityProperty, value);
        }

        public CornerRadius CornerRadius
        {
            get => (CornerRadius)GetValue(CornerRadiusProperty);
            set => SetValue(CornerRadiusProperty, value);
        }

        public PackIconKind IconKind
        {
            get => (PackIconKind)GetValue(IconKindProperty);
            set => SetValue(IconKindProperty, value);
        }

        public ImageSource ImageSource
        {
            get => (ImageSource)GetValue(ImageSourceProperty);
            set => SetValue(ImageSourceProperty, value);
        }


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Static BaseWindow class constructor. </summary>
        static BaseWindow()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(BaseWindow),
                new FrameworkPropertyMetadata(typeof(BaseWindow)));
        }

        #endregion CLASS METHODS

        #region BUTTONS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after clicking Close button. </summary>
        /// <param name="sender"> Object that invoked method. </param>
        /// <param name="e"> Routed Event Arguments. </param>
        protected virtual void CloseButtonClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after clicking Maximize button. </summary>
        /// <param name="sender"> Object that invoked method. </param>
        /// <param name="e"> Routed Event Arguments. </param>
        protected virtual void MaximizeButtonClick(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState != WindowState.Maximized
                ? WindowState.Maximized
                : WindowState.Normal;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after clicking Minimize button. </summary>
        /// <param name="sender"> Object that invoked method. </param>
        /// <param name="e"> Routed Event Arguments. </param>
        protected virtual void MinimizeButtonClick(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        #endregion BUTTONS METHODS

        #region RESIZE METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after pressing left mouse button when cursor is over resize border. </summary>
        /// <param name="sender"> Object that invoked method. </param>
        /// <param name="e"> Mouse Button Event Arguments. </param>
        private void ResizeBorderMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is Border border)
            {
                _posTop = Top;
                _posLeft = Left;
                _startX = e.GetPosition(this).X;
                _startY = e.GetPosition(this).Y;
                _startW = Width;
                _startH = Height;

                switch (border.Name)
                {
                    case RESIZE_BORDER_LEFT_NAME:
                        Cursor = Cursors.SizeWE;
                        break;

                    case RESIZE_BORDER_TOP_LEFT_NAME:
                        Cursor = Cursors.SizeNWSE;
                        break;

                    case RESIZE_BORDER_TOP_NAME:
                        Cursor = Cursors.SizeNS;
                        break;

                    case RESIZE_BORDER_TOP_RIGHT_NAME:
                        Cursor = Cursors.SizeNESW;
                        break;

                    case RESIZE_BORDER_RIGHT_NAME:
                        Cursor = Cursors.SizeWE;
                        break;

                    case RESIZE_BORDER_BOTTOM_RIGHT_NAME:
                        Cursor = Cursors.SizeNWSE;
                        break;

                    case RESIZE_BORDER_BOTTOM_NAME:
                        Cursor = Cursors.SizeNS;
                        break;

                    case RESIZE_BORDER_BOTTOM_LEFT_NAME:
                        Cursor = Cursors.SizeNESW;
                        break;
                }

                border.CaptureMouse();
            }
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after releasing left mouse button when cursor is over resize border. </summary>
        /// <param name="sender"> Object that invoked method. </param>
        /// <param name="e"> Mouse Button Event Arguments. </param>
        private void ResizeBorderMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (sender is Border border)
            {
                border.ReleaseMouseCapture();
                Cursor = Cursors.Arrow;
            }
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after moving cursor over resize border. </summary>
        /// <param name="sender"> Object that invoked method. </param>
        /// <param name="e"> Mouse Button Event Arguments. </param>
        private void ResizeBorderMouseMove(object sender, MouseEventArgs e)
        {
            if (sender is Border border && e.LeftButton == MouseButtonState.Pressed)
            {
                double x = e.GetPosition(this).X;
                double y = e.GetPosition(this).Y;
                double fX = System.Windows.Forms.Cursor.Position.X;
                double fY = System.Windows.Forms.Cursor.Position.Y;
                double w = Width;
                double h = Height;

                switch (border.Name)
                {
                    case RESIZE_BORDER_LEFT_NAME:
                        w = _startW - (fX - _posLeft);

                        if (w < MinWidth)
                            w = MinWidth;

                        if (w > MinWidth)
                            Left = fX;
                        break;

                    case RESIZE_BORDER_TOP_LEFT_NAME:
                        w = _startW - (fX - _posLeft);
                        h = _startH - (fY - _posTop);

                        if (w < MinWidth)
                            w = MinWidth;

                        if (h < MinHeight)
                            h = MinHeight;

                        if (w > MinWidth)
                            Left = fX;

                        if (h > MinHeight)
                            Top = fY;
                        break;

                    case RESIZE_BORDER_TOP_NAME:
                        h = _startH - (fY - _posTop);

                        if (h < MinHeight)
                            h = MinHeight;

                        if (h > MinHeight)
                            Top = fY;
                        break;

                    case RESIZE_BORDER_TOP_RIGHT_NAME:
                        w = _startW + (x - _startX);
                        h = _startH - (fY - _posTop);

                        if (w < MinWidth)
                            w = MinWidth;

                        if (h < MinHeight)
                            h = MinHeight;

                        if (h > MinHeight)
                            Top = fY;
                        break;

                    case RESIZE_BORDER_RIGHT_NAME:
                        w = _startW + (x - _startX);

                        if (w < MinWidth)
                            w = MinWidth;
                        break;

                    case RESIZE_BORDER_BOTTOM_RIGHT_NAME:
                        w = _startW + (x - _startX);
                        h = _startH + (y - _startY);

                        if (w < MinWidth)
                            w = MinWidth;

                        if (h < MinHeight)
                            h = MinHeight;
                        break;

                    case RESIZE_BORDER_BOTTOM_NAME:
                        h = _startH + (y - _startY);

                        if (h < MinHeight)
                            h = MinHeight;
                        break;

                    case RESIZE_BORDER_BOTTOM_LEFT_NAME:
                        w = _startW - (fX - _posLeft);
                        h = _startH + (y - _startY);

                        if (w < MinWidth)
                            w = MinWidth;

                        if (h < MinHeight)
                            h = MinHeight;

                        if (w > MinWidth)
                            Left = fX;
                        break;
                }

                if (w >= MinWidth)
                    Width = w;

                if (h >= MinHeight)
                    Height = h;
            }
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Method invoked after pressing left mouse button when cursor is over title bar grid. </summary>
        /// <param name="sender"> Object that invoked method. </param>
        /// <param name="e"> Mouse Button Event Arguments. </param>
        private void TitleBarGridMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        #endregion RESIZE METHODS

        #region TEMPLATE METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Apply method for button. </summary>
        /// <param name="buttonName"> Button name. </param>
        /// <param name="eventHandler"> Method event handler. </param>
        private void ApplyButtonMethod(string buttonName, RoutedEventHandler eventHandler)
        {
            var button = GetButton(buttonName);

            if (button != null)
                button.Click += eventHandler;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Apply methods for move window. </summary>
        /// <param name="titleGridName"> Title grid name. </param>
        /// <param name="eventHandler"> Method event handler. </param>
        private void ApplyMoveMethod(string titleGridName, MouseButtonEventHandler eventHandler)
        {
            var grid = GetGrid(titleGridName);

            if (grid != null)
                grid.MouseLeftButtonDown += eventHandler;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Apply methods for resize border components. </summary>
        /// <param name="borderName"> Border name. </param>
        private void ApplyResizeMethods(string borderName)
        {
            var border = GetBorder(borderName);

            if (border != null)
            {
                border.MouseLeftButtonDown += ResizeBorderMouseLeftButtonDown;
                border.MouseLeftButtonUp += ResizeBorderMouseLeftButtonUp;
                border.MouseMove += ResizeBorderMouseMove;
            }
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Get Border from ContentTemplate. </summary>
        /// <param name="borderName"> Border name. </param>
        /// <returns> Border or null. </returns>
        private Border GetBorder(string borderName)
        {
            return this.Template.FindName(borderName, this) as Border;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Get Button from ContentTemplate. </summary>
        /// <param name="buttonName"> Button name. </param>
        /// <returns> Button or null. </returns>
        private Button GetButton(string buttonName)
        {
            return this.Template.FindName(buttonName, this) as Button;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Get Grid from ContentTemplate. </summary>
        /// <param name="gridName"> Grid name. </param>
        /// <returns> Grid or null. </returns>
        private Grid GetGrid(string gridName)
        {
            return this.Template.FindName(gridName, this) as Grid;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> When overridden in a derived class,cis invoked whenever 
        /// application code or internal processes call ApplyTemplate. </summary>
        public override void OnApplyTemplate()
        {
            //  Apply Template
            base.OnApplyTemplate();

            ApplyButtonMethod(BUTTON_MINIMIZE_NAME, MinimizeButtonClick);
            ApplyButtonMethod(BUTTON_MAXIMIZE_NAME, MaximizeButtonClick);
            ApplyButtonMethod(BUTTON_CLOSE_NAME, CloseButtonClick);

            ApplyMoveMethod(TITLE_GRID_NAME, TitleBarGridMouseLeftButtonDown);

            ApplyResizeMethods(RESIZE_BORDER_LEFT_NAME);
            ApplyResizeMethods(RESIZE_BORDER_TOP_LEFT_NAME);
            ApplyResizeMethods(RESIZE_BORDER_TOP_NAME);
            ApplyResizeMethods(RESIZE_BORDER_TOP_RIGHT_NAME);
            ApplyResizeMethods(RESIZE_BORDER_RIGHT_NAME);
            ApplyResizeMethods(RESIZE_BORDER_BOTTOM_RIGHT_NAME);
            ApplyResizeMethods(RESIZE_BORDER_BOTTOM_NAME);
            ApplyResizeMethods(RESIZE_BORDER_BOTTOM_LEFT_NAME);
        }

        #endregion TEMPLATE METHODS
    }
}
