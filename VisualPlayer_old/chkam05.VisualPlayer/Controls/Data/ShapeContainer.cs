using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace chkam05.VisualPlayer.Controls.Data
{
    public class ShapeContainer : INotifyPropertyChanged
    {

        //  EVENTS

        public event PropertyChangedEventHandler PropertyChanged;


        //  VARIABLES

        private List<Point> _points;
        private Shape _shape = null;

        private Brush _background = new SolidColorBrush(Color.FromArgb(192, 0, 0, 0));
        private Brush _borderBrush = new SolidColorBrush(Colors.Black);
        private double _borderThickness = 1;


        //  GETTERS & SETTERS

        public List<Point> Points
        {
            get => _points;
            set
            {
                _points = value;
                OnPropertyChanged(nameof(Shape));
                CreateShapeFromPoints();
            }
        }

        [JsonIgnore]
        public Shape Shape
        {
            get
            {
                if (_shape == null && _points != null && _points.Any())
                    CreateShapeFromPoints();

                return _shape;
            }
            set
            {
                _shape = value;
                OnPropertyChanged(nameof(Shape));
            }
        }

        public Brush Background
        {
            get => _background;
            set
            {
                _background = value;
                OnPropertyChanged(nameof(Background));

                if (Shape != null)
                    Shape.Fill = _background;
            }
        }

        public Brush BorderBrush
        {
            get => _borderBrush;
            set
            {
                _borderBrush = value;
                OnPropertyChanged(nameof(BorderBrush));

                if (Shape != null)
                    Shape.Stroke = _background;
            }
        }

        public double BorderThickness
        {
            get => _borderThickness;
            set
            {
                _borderThickness = value;
                OnPropertyChanged(nameof(BorderThickness));

                if (Shape != null)
                    Shape.StrokeThickness = _borderThickness;
            }
        }


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> ShapeContainer class constructor. </summary>
        public ShapeContainer()
        {
            _points = new List<Point>();
        }

        #endregion CLASS METHODS

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

        #region SHAPE CREATION METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Create shape from points. </summary>
        private void CreateShapeFromPoints()
        {
            Polygon polygon = new Polygon();

            polygon.Stroke = _borderBrush;
            polygon.StrokeThickness = _borderThickness;
            polygon.Fill = _background;

            foreach (var point in Points)
                polygon.Points.Add(point);

            Shape = polygon;
        }

        #endregion SHAPE CREATION METHODS

    }
}
