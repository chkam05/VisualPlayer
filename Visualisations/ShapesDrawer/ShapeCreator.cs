using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace chkam05.Visualisations.LogoDrawing
{
    public class ShapeCreator
    {

        //  VARIABLES

        private List<Point> _points;
        private Shape _shape = null;

        private Brush _backgroundBrush = new SolidColorBrush(Color.FromArgb(192, 0, 0, 0));
        private Brush _borderBrush = new SolidColorBrush(Colors.Black);
        private double _borderThickness = 1;


        #region GETTERS & SETTERS

        public int PointsCount
        {
            get => _points != null ? _points.Count : 0;
        }

        public List<Point> Points
        {
            get => _points;
            set
            {
                if (value != null)
                {
                    _points = value;

                    if (_shape != null)
                        CreateShape();
                }
            }
        }

        public Shape Shape
        {
            get
            {
                if (_shape == null)
                    return CreateShape();
                return _shape;
            }
        }

        public Brush BorderBrush
        {
            get => _borderBrush;
            set
            {
                _borderBrush = value;

                if (_shape != null)
                    _shape.Stroke = _borderBrush;
            }
        }

        public double BorderThickness
        {
            get => _borderThickness;
            set
            {
                _borderThickness = value;

                if (_shape != null)
                    _shape.StrokeThickness = _borderThickness;
            }
        }

        public Brush BackgroundBrush
        {
            get => _backgroundBrush;
            set
            {
                _backgroundBrush = value;

                if (_shape != null)
                    _shape.Fill = _backgroundBrush;
            }
        }

        #endregion GETTERS & SETTERS


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> ShapeCreator class constructor. </summary>
        public ShapeCreator()
        {
            _points = new List<Point>();
        }

        //  --------------------------------------------------------------------------------
        /// <summary> ShapeCreator class constructor. </summary>
        /// <param name="_points"> Shape list of points. </param>
        public ShapeCreator(List<Point> points)
        {
            if (points != null)
                _points = points;
        }

        #endregion CLASS METHODS

        #region CREATE METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Create shape from defined points and parameters. </summary>
        /// <returns> Shape. </returns>
        public Shape CreateShape()
        {
            Polygon polygon = new Polygon();

            polygon.Stroke = _borderBrush;
            polygon.StrokeThickness = _borderThickness;
            polygon.Fill = _backgroundBrush;

            foreach (Point point in _points)
                polygon.Points.Add(point);

            _shape = polygon;

            return _shape;
        }

        #endregion CREATE METHODS

        #region GET INFO METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Get size of shape. </summary>
        /// <returns> Size of shape. </returns>
        public Size GetSize()
        {
            return GetSize(out Size shift);
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Get shape shift from point 0, 0. (Position of shape). </summary>
        /// <returns> Shape shift (position of shape). </returns>
        public Size GetShift()
        {
            Size shift;
            GetSize(out shift);
            return shift;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Get size and shift of shape. </summary>
        /// <param name="shift"> Out shape shift from point 0, 0. (Position of shape). </param>
        /// <returns> Size of shape. </returns>
        private Size GetSize(out Size shift)
        {
            Size size = new Size(0, 0);
            shift = new Size(0, 0);

            if (_points != null && _points.Any())
            {
                Rect rect = new Rect(double.MaxValue, double.MaxValue, double.MinValue, double.MinValue);

                foreach (Point p in _points)
                {
                    if (p.X < rect.X)
                        rect.X = p.X;

                    if (p.Y < rect.Y)
                        rect.Y = p.Y;

                    if (p.X > rect.Width)
                        rect.Width = p.X;

                    if (p.Y > rect.Height)
                        rect.Height = p.Y;
                }
            }

            return size;
        }

        #endregion GET INFO METHODS

    }
}
