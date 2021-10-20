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

        private Grid _container;
        private bool _settedUp;
        private List<ShapeCreator> _shapes;


        public bool IsSettedUp
        {
            get => _settedUp;
        }


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> ShapeDrawer class constructor. </summary>
        /// <param name="container"> Shapes container. </param>
        public ShapeDrawer(Grid container)
        {
            _shapes = new List<ShapeCreator>();

            SetupContainer(container);
        }

        #endregion CLASS METHODS

        #region DRAWING METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Apply scale transform on shapes container. </summary>
        /// <param name="x"> Scale X. </param>
        /// <param name="y"> Scale Y. </param>
        public void ApplyScale(double x, double y)
        {
            if (_container != null)
            {
                var renderTransform = _container.RenderTransform;

                if (renderTransform.GetType() == typeof(TransformGroup))
                {
                    var transformGroup = (TransformGroup) renderTransform;
                    var scale = (ScaleTransform) transformGroup.Children
                        .FirstOrDefault(t => t.GetType() == typeof(ScaleTransform));

                    if (scale != null)
                    {
                        scale.ScaleX = Math.Min(2.0, Math.Max(1.0, x));
                        scale.ScaleY = Math.Min(2.0, Math.Max(1.0, y));
                    }
                }
            }
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Clear shapes from container. </summary>
        public void ClearShapes()
        {
            if (_container != null && _shapes.Any())
            {
                foreach (var shapeCreator in _shapes)
                {
                    Shape shape = shapeCreator.Shape;

                    if (shape != null && _container.Children.Contains(shape))
                        _container.Children.Remove(shape);
                }

                _settedUp = false;
            }
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Recreate shapes in container. </summary>
        public void RedrawShapes()
        {
            if (_container != null)
            {
                foreach (var shapeCreator in _shapes)
                    RedrawShape(shapeCreator);

                _settedUp = true;
            }
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Recreate shape in container. </summary>
        /// <param name="shapeCreator"> Shape creator. </param>
        private void RedrawShape(ShapeCreator shapeCreator)
        {
            Shape shape = shapeCreator.Shape;

            if (!_container.Children.Contains(shape))
                _container.Children.Add(shape);

            int zIndex = Canvas.GetZIndex(shape);
            if (zIndex > 0)
                Canvas.SetZIndex(shape, -1);
        }

        #endregion DRAWING METHODS

        #region SETUP METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Setup container for drawing shapes. </summary>
        /// <param name="container"> Container where shapes will be drawed. </param>
        private void SetupContainer(Grid container)
        {
            if (container != null)
                _container = container;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Create shapes from json shapes definitions structure. </summary>
        /// <param name="shapeJson"> Json shapes definitions structure. </param>
        /// <param name="autoReDraw"> Draw shape after setup. </param>
        public void SetupShape(string shapeJson = "", bool autoReDraw = false)
        {
            bool redraw = _container != null && autoReDraw;

            try
            {
                var shapesCreators = JsonConvert.DeserializeObject<List<ShapeCreator>>(shapeJson);

                if (shapesCreators.Any())
                {
                    if (redraw)
                        ClearShapes();

                    foreach (var shapeContainer in shapesCreators)
                    {
                        shapeContainer.CreateShape();

                        if (redraw)
                            RedrawShape(shapeContainer);
                    }

                    _shapes = shapesCreators;
                }
            }
            catch
            {
                //  Just continue without shape.
            }
        }

        #endregion SETUP METHODS

    }
}
