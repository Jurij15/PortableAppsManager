using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage.Streams;
using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.UI.Xaml.Media;
using System.Drawing.Imaging;
using System.Drawing;

namespace PortableAppsManager.Helpers
{
    public class ImageHelper
    {
        /* last working state
        public static ImageSource ConvertIconToImageSource(System.Drawing.Icon icon)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                icon.ToBitmap().Save(stream, System.Drawing.Imaging.ImageFormat.Png); // Convert Icon to Bitmap

                BitmapImage bitmapImage = new BitmapImage();
                stream.Seek(0, SeekOrigin.Begin);

                bitmapImage.DecodePixelWidth = 1024;
                bitmapImage.DecodePixelHeight = 1024;
                bitmapImage.SetSource(stream.AsRandomAccessStream());

                return bitmapImage;
            }
        }
        */
        public static ImageSource ConvertIconToImageSource(Icon icon, int dpi = 196)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                // Create a new Bitmap with specified DPI
                Bitmap newBitmap = new Bitmap(icon.ToBitmap(), new Size(icon.Width, icon.Height));
                newBitmap.SetResolution(dpi, dpi);

                // Save the new Bitmap to the stream
                newBitmap.Save(stream, ImageFormat.Png);

                // Create BitmapImage
                BitmapImage bitmapImage = new BitmapImage();
                stream.Seek(0, SeekOrigin.Begin);

                // Set the source from the stream
                bitmapImage.SetSource(stream.AsRandomAccessStream());

                return bitmapImage;
            }
        }
    }
}
