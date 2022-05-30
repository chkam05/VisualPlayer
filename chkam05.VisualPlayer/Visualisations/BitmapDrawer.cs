using chkam05.VisualPlayer.Visualisations.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace chkam05.VisualPlayer.Visualisations
{
    public class BitmapDrawer
    {

        //  CONST

        public readonly int DPI_X = 96;
        public readonly int DPI_Y = 96;


        //  VARIABLES

        private DrawingContext _context;
        private DrawingVisual _visual;

        public bool IsDrawing { get; private set; }
        public Size Size { get; private set; }


        //  GETTERS & SETTERS

        public DrawingContext Context
        {
            get => IsDrawing ? _context : null;
        }

        public int Height
        {
            get => (int)Size.Height;
        }

        public int Width
        {
            get => (int)Size.Width;
        }


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> BitmapDrawer class constructor. </summary>
        /// <param name="width"> Drawing area width. </param>
        /// <param name="height"> Drawing area height. </param>
        public BitmapDrawer(double width, double height)
        {
            IsDrawing = false;
            Size = new Size(width, height);

            BeginDrawing();
        }

        //  --------------------------------------------------------------------------------
        /// <summary> BitmapDrawer class constructor. </summary>
        /// <param name="size"> Drawing area size. </param>
        public BitmapDrawer(Size size)
        {
            IsDrawing = false;
            Size = size;

            BeginDrawing();
        }

        #endregion CLASS METHODS

        #region DRAWING METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Begin drawing bitmap. </summary>
        public void BeginDrawing()
        {
            _visual = new DrawingVisual();
            _context = _visual.RenderOpen();

            //  Draw transparent background.
            DrawRectangle(Brushes.Transparent, null, new Point(0, 0), Size);

            IsDrawing = true;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> End drawing bitmap. </summary>
        public void EndDrawing()
        {
            if (IsDrawing)
            {
                IsDrawing = false;

                try
                {
                    _context.Close();
                }
                catch (Exception)
                {
                    //
                }
            }
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Draw rectangle. </summary>
        /// <param name="fill"> Fill brush. </param>
        /// <param name="stroke"> Stroke brush and size. </param>
        /// <param name="point"> Start drawing point (Left Top). </param>
        /// <param name="size"> Rectangle size. </param>
        public void DrawRectangle(Brush fill, Pen stroke, Point point, Size size)
        {
            if (IsDrawing)
                _context.DrawRectangle(fill, stroke, new Rect(point.X, point.Y, size.Width, size.Height));
        }

        #endregion DRAWING METHODS

        #region RENDERING METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Render bitmap to image source. </summary>
        /// <param name="freeze"> Freeze image. </param>
        /// <returns> Bitmap as image source. </returns>
        public ImageSource RenderImage(bool freeze = false)
        {
            EndDrawing();

            var renderBitmap = new RenderTargetBitmap(Width, Height, DPI_X, DPI_Y, PixelFormats.Pbgra32);
            renderBitmap.Render(_visual);

            var bitmap = BitmapFrame.Create(renderBitmap);

            if (freeze)
                bitmap.Freeze();

            return bitmap;
        }

        #endregion RENDERING METHODS

    }
}
