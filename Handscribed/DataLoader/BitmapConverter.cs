using Handscribed.Dataset;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Windows;

namespace Handscribed.DataLoader
{
    internal class BitmapConverter
    {
        public static WriteableBitmap ConvertToBitmap(MnistImage mnistImage, int scale = 10)
        {
            int width = 28;
            int height = 28;

            // Create bitmap (scaled up for visibility)
            var bitmap = new WriteableBitmap(
                width * scale,
                height * scale,
                96, 96,
                PixelFormats.Gray8,
                null);

            int stride = bitmap.PixelWidth;
            byte[] pixels = new byte[stride * bitmap.PixelHeight];

            // Fill pixels (with scaling)
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    byte pixelValue = mnistImage.PixelData[y * width + x];

                    // Fill the scaled block
                    for (int sy = 0; sy < scale; sy++)
                    {
                        for (int sx = 0; sx < scale; sx++)
                        {
                            int destX = x * scale + sx;
                            int destY = y * scale + sy;

                            if (destX < stride && destY < bitmap.PixelHeight)
                            {
                                pixels[destY * stride + destX] = pixelValue;
                            }
                        }
                    }
                }
            }

            // Write to bitmap
            bitmap.WritePixels(
                new Int32Rect(0, 0, bitmap.PixelWidth, bitmap.PixelHeight),
                pixels, stride, 0);

            return bitmap;
        }

        private static BitmapSource ConvertToColorBitmap(MnistImage mnistImage, int scale = 10)
        {
            int width = 28;
            int height = 28;
            int scaledWidth = width * scale;
            int scaledHeight = height * scale;

            // Create color bitmap
            var bitmap = new WriteableBitmap(
                scaledWidth, scaledHeight,
                96, 96,
                PixelFormats.Bgr32,
                null);

            int stride = bitmap.PixelWidth * 4; // 4 bytes per pixel (BGRA)
            byte[] pixels = new byte[stride * scaledHeight];

            // Fill with grayscale colors
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    byte pixelValue = mnistImage.PixelData[y * width + x];

                    // Create color (BGR format)
                    byte[] color = { pixelValue, pixelValue, pixelValue, 255 };

                    // Fill scaled block
                    for (int sy = 0; sy < scale; sy++)
                    {
                        for (int sx = 0; sx < scale; sx++)
                        {
                            int destX = x * scale + sx;
                            int destY = y * scale + sy;

                            if (destX < bitmap.PixelWidth && destY < scaledHeight)
                            {
                                int index = (destY * stride) + (destX * 4);
                                Array.Copy(color, 0, pixels, index, 3);
                                pixels[index + 3] = 255; // Alpha
                            }
                        }
                    }
                }
            }

            bitmap.WritePixels(
                new Int32Rect(0, 0, scaledWidth, scaledHeight),
                pixels, stride, 0);

            return bitmap;
        }
    }
}
