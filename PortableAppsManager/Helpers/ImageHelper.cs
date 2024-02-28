using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Reflection.PortableExecutable;
using System.Threading.Tasks;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using Newtonsoft.Json;
using Windows.Graphics.Imaging;
using Windows.Storage.Streams;
using Microsoft.UI.Xaml.Media.Imaging;
using System.Runtime.InteropServices.WindowsRuntime;
using PortableAppsManager.Classes;
using static System.Net.Mime.MediaTypeNames;
using PortableAppsManager.Enums;

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
        public static ImageSource ConvertIconToImageSource(Icon icon, int dpi = 512)
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

        public static ImageSource GetImageSource(AppItem Item)
        {
            ImageSource returnval = null;

            if (Item != null && File.Exists(Item.ExePath))
            {
                if (Item.SourceType == Enums.AppImageSourceType.Executable)
                {
                    returnval = ImageHelper.ConvertIconToImageSource(System.Drawing.Icon.ExtractAssociatedIcon(Item.ExePath));
                }
                else if (Item.SourceType == Enums.AppImageSourceType.File && Item.AppImageSourcePath != null)
                {
                    if (File.Exists(Item.AppImageSourcePath))
                    {
                        BitmapImage bitmapImage = new BitmapImage();

                        bitmapImage.UriSource = new System.Uri(Item.AppImageSourcePath);

                        returnval = bitmapImage;
                    }
                }
                else
                {
                    returnval = ImageHelper.ConvertIconToImageSource(System.Drawing.Icon.ExtractAssociatedIcon(Item.ExePath));
                }
            }

            return returnval;
        }

        public static ImageSource GetImageSource(AppImageSourceType SourceType, string AppImageSourcePath, string ExePath)
        {
            ImageSource returnval = null;

            if (SourceType == Enums.AppImageSourceType.Executable)
            {
                if (File.Exists(ExePath))
                {
                    returnval = ImageHelper.ConvertIconToImageSource(System.Drawing.Icon.ExtractAssociatedIcon(ExePath));
                }
            }
            else if (SourceType == Enums.AppImageSourceType.File && AppImageSourcePath != null)
            {
                if (File.Exists(AppImageSourcePath))
                {
                    BitmapImage bitmapImage = new BitmapImage();

                    bitmapImage.UriSource = new System.Uri(AppImageSourcePath);

                    returnval = bitmapImage;
                }
            }
            else
            {
                returnval = ImageHelper.ConvertIconToImageSource(System.Drawing.Icon.ExtractAssociatedIcon(ExePath));
            }

            return returnval;
        }
    }
}
