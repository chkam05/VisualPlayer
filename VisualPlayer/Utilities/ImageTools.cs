using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace chkam05.VisualPlayer.Utilities
{
    public static class ImageTools
    {

        //  METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Convert base64 string to image source. </summary>
        /// <param name="base64"> Base64 to convert. </param>
        /// <returns> Image source converted from base64. </returns>
        public static ImageSource ImageSourceFromBase64(string base64)
        {
            byte[] sourceBytes = Convert.FromBase64String(base64);

            using (var stream = new MemoryStream(sourceBytes))
            {
                BitmapImage bmpImage = new BitmapImage();
                bmpImage.BeginInit();
                bmpImage.StreamSource = stream;
                bmpImage.EndInit();

                return bmpImage;
            }
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Convert image source to base64 string. </summary>
        /// <param name="source"> Image source to convert. </param>
        /// <param name="encoder"> Image type encoder (ex. Png, Bmp, Jpg). </param>
        /// <returns> Image source as base64 string. </returns>
        public static string ImageSourceToBase64(ImageSource source, BitmapEncoder encoder)
        {
            string base64 = null;
            var bmpSource = source as BitmapSource;

            if (bmpSource != null)
            {
                var frame = BitmapFrame.Create(bmpSource);
                encoder.Frames.Add(frame);

                using (var stream = new MemoryStream())
                {
                    encoder.Save(stream);
                    stream.Position = 0;

                    byte[] sourceBytes = stream.ToArray();
                    base64 = Convert.ToBase64String(sourceBytes);
                }
            }

            return base64;
        }

    }
}
