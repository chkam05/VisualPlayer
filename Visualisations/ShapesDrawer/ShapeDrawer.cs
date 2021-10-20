using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace chkam05.Visualisations.LogoDrawing
{
    public class ShapeDrawer
    {

        //  VARIABLES

        private Canvas _canvas;
        private List<ShapeContainer> _shapes;


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        public ShapeDrawer(Canvas canvas)
        {
            _shapes = new List<ShapeContainer>();

            SetupCanvas(canvas);
        }

        #endregion CLASS METHODS

        #region DRAWING METHODS

        //  --------------------------------------------------------------------------------
        public void ApplyScale(double x, double y)
        {
            ScaleTransform scale = null;

            foreach (var shapeContainer in _shapes)
            {
                if (!shapeContainer.Transforms.Any(s => s.GetType() == typeof(ScaleTransform)))
                {
                    scale = new ScaleTransform();
                    shapeContainer.Transforms.Add(scale);
                }
                else
                {
                    scale = (ScaleTransform)shapeContainer.Transforms.FirstOrDefault(s => s.GetType() == typeof(ScaleTransform));
                }

                scale.ScaleX = Math.Min(2.0, Math.Max(1.0, x));
                scale.ScaleY = Math.Min(2.0, Math.Max(1.0, y));
            }
        }

        //  --------------------------------------------------------------------------------
        public void ClearShapes()
        {
            if (_canvas != null && _shapes.Any())
            {
                foreach (var shapeContainer in _shapes)
                {
                    Shape shape = shapeContainer.Shape;

                    if (shape != null && _canvas.Children.Contains(shape))
                        _canvas.Children.Remove(shape);
                }
            }
        }

        //  --------------------------------------------------------------------------------
        public void RedrawShapes()
        {
            if (_canvas != null)
            {
                foreach (var shapeContainer in _shapes)
                {
                    RedrawShape(shapeContainer);
                }
            }
        }

        //  --------------------------------------------------------------------------------
        private void RedrawShape(ShapeContainer shapeContainer)
        {
            Shape shape = shapeContainer.Shape;
            Size shift = shapeContainer.Shift;
            Size size = shapeContainer.Size;

            if (!_canvas.Children.Contains(shape))
                _canvas.Children.Add(shape);

            int zIndex = Canvas.GetZIndex(shape);
            if (zIndex > 0)
                Canvas.SetZIndex(shape, -1);

            RecalculateShapeToCenter(shape, shift, size);
        }

        #endregion DRAWING METHODS

        #region SETUP METHODS

        //  --------------------------------------------------------------------------------
        private void SetupCanvas(Canvas canvas)
        {
            if (canvas != null)
                _canvas = canvas;
        }

        //  --------------------------------------------------------------------------------
        public void SetupShape(string shapeJson = "", bool autoReDraw = false)
        {
            bool redraw = _canvas != null && autoReDraw;

            try
            {
                var shapes = JsonConvert.DeserializeObject<List<ShapeContainer>>(shapeJson);

                if (redraw && shapes.Any())
                    ClearShapes();

                foreach (var shapeContainer in shapes)
                {
                    shapeContainer.SetupPolygon();

                    if (redraw)
                        RedrawShape(shapeContainer);
                }

                _shapes = shapes;
            }
            catch
            {
                //  Just continue without shape.
            }
        }

        #endregion SETUP METHODS

        #region UTILITY METHODS

        //  --------------------------------------------------------------------------------
        private void RecalculateShapeToCenter(Shape shape, Size shift, Size size)
        {
            double left = (_canvas.ActualWidth - size.Width) / 2;
            double leftShifted = left + (shift.Width / 2);
            Canvas.SetLeft(shape, leftShifted);

            double top = (_canvas.ActualHeight - size.Height) / 2;
            double topShifted = top - (shift.Height / 2);
            Canvas.SetTop(shape, topShifted);
        }

        #endregion UTILITY METHODS

    }
}
