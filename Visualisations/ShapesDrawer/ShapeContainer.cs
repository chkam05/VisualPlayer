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
    public class ShapeContainer
    {

        //  VARIABLES

        private List<Point> _points;
        private Shape _shape = null;
        private Size _shift = new Size(0, 0);
        private Size _size = new Size(0, 0);

        private Brush _borderBrush = new SolidColorBrush(Colors.Black);
        private double _borderThickness = 1;
        private Brush _backgroundBrush = new SolidColorBrush(Color.FromArgb(192, 0, 0, 0));

        private Point _pivotPoint = new Point(0.5, 0.5);
        private TransformGroup _transform = null;


        #region GETTERS & SETTERS

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
                _borderThickness = Math.Max(0, value);

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

        public int PointsCount
        {
            get => _points.Count;
        }

        public List<Point> Points
        {
            get => _points;
            set
            {
                _points = value;
                SetupPolygon();
            }
        }

        public Shape Shape
        {
            get => _shape;
        }

        public Size Shift
        {
            get => _shift;
        }

        public Size Size
        {
            get => _size;
        }

        public Point PivotPoint
        {
            get => _pivotPoint;
            set
            {
                _pivotPoint = value;

                if (_shape != null)
                    _shape.RenderTransformOrigin = _pivotPoint;
            }
        }

        public TransformCollection Transforms
        {
            get => _transform.Children;
        }

        #endregion GETTERS & SETTERS


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        public ShapeContainer()
        {
            _points = new List<Point>();
            _transform = new TransformGroup();
        }

        #endregion CLASS METHODS

        #region INTERACTION METHODS

        //  --------------------------------------------------------------------------------
        private void BuildPolygon()
        {
            var polygon = new Polygon();

            polygon.Stroke = _borderBrush;
            polygon.StrokeThickness = _borderThickness;
            polygon.Fill = _backgroundBrush;

            foreach (Point point in _points)
                polygon.Points.Add(point);

            polygon.RenderTransformOrigin = _pivotPoint;

            if (polygon.RenderTransform != _transform)
                polygon.RenderTransform = _transform;

            _shape = polygon;
        }

        #endregion INTERACTION METHODS

        #region SETUP METHODS

        //  --------------------------------------------------------------------------------
        public void SetupPolygon()
        {
            FixClose();
            BuildPolygon();
            CalculateShapeSize();
        }

        #endregion SETUP METHODS

        #region UTILITY METHODS

        //  --------------------------------------------------------------------------------
        private void CalculateShapeSize()
        {
            double minX = double.MaxValue;
            double minY = double.MaxValue;
            double maxX = double.MinValue;
            double maxY = double.MinValue;

            foreach (Point p in _points)
            {
                if (p.X < minX)
                    minX = p.X;

                if (p.Y < minY)
                    minY = p.Y;

                if (p.X > maxX)
                    maxX = p.X;

                if (p.Y > maxY)
                    maxY = p.Y;
            }

            _shift = new Size(minX, minY);
            _size = new Size(maxX - minX, maxY - minY);
        }

        //  --------------------------------------------------------------------------------
        private bool ComparePoints(Point pointA, Point pointB)
        {
            return (pointA.X == pointB.X && pointA.Y == pointB.Y);
        }

        //  --------------------------------------------------------------------------------
        private void FixClose()
        {
            if (_points.Any() && ComparePoints(_points[0], _points[PointsCount - 1]))
                _points.Add(_points[0]);
        }

        #endregion UTILITY METHODS

    }
}
