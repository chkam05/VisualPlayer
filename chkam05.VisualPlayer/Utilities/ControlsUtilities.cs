using MaterialDesignThemes.Wpf;
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

namespace chkam05.VisualPlayer.Utilities
{
    public static class ControlsUtilities
    {

        //  METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Find parent control object from child control object. </summary>
        /// <typeparam name="T"> Parent control object tpye to find. </typeparam>
        /// <param name="interfaceObject"> Start object. </param>
        /// <returns> Found parent object or null. </returns>
        public static T FindParent<T>(FrameworkElement interfaceObject) where T : FrameworkElement
        {
            if (interfaceObject != null)
            {
                if (interfaceObject.GetType() == typeof(T)
                    || interfaceObject.GetType().IsSubclassOf(typeof(T)))
                    return (T)interfaceObject;

                else
                    return FindParent<T>(interfaceObject.Parent as FrameworkElement);
            }

            return null;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Draw image from interface control. </summary>
        /// <param name="control"> Interface control. </param>
        /// <param name="width"> Image width. </param>
        /// <param name="height"> Image height. </param>
        /// <param name="background"> Background brush. </param>
        /// <returns> Image source of drawed interface control. </returns>
        public static ImageSource DrawImageFromControl(Control control, double width, double height, Brush background)
        {
            Grid drawingGrid = new Grid()
            {
                Background = background,
                Height = height,
                Width = width
            };

            var size = new Size(width, height);

            drawingGrid.Children.Add(control);
            drawingGrid.Measure(size);
            drawingGrid.Arrange(new Rect(size));
            drawingGrid.UpdateLayout();

            var renderBitmap = new RenderTargetBitmap((int)width, (int)height, 96, 96, PixelFormats.Default);
            renderBitmap.Render(drawingGrid);

            var bitmap = BitmapFrame.Create(renderBitmap);
            bitmap.Freeze();
            return bitmap;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Draw PackIcon control with specified PackIconKind. </summary>
        /// <param name="packIconKind"> Pack icon kind. </param>
        /// <param name="width"> Image width. </param>
        /// <param name="height"> Image height. </param>
        /// <param name="background"> Background brush. </param>
        /// <param name="foreground"> Foreground brush. </param>
        /// <returns> Image source of drawed pack icon. </returns>
        public static ImageSource DrawImageFromPackIconKind(PackIconKind packIconKind, double width, double height, Brush background, Brush foreground)
        {
            PackIcon packIcon = new PackIcon()
            {
                Foreground = foreground,
                Kind = packIconKind,
                Height = height,
                Width = width
            };

            return DrawImageFromControl(packIcon, width, height, background);
        }

    }
}
