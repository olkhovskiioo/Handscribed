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
            int width = Consts.MNIST_IMAGE_WIDGHT;
            int height = Consts.MNIST_IMAGE_HEIGHT;

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
    }
}
