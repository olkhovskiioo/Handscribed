using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Handscribed.Dataset
{
    public class MnistImage
    {
        public byte Label { get; set; }
        public byte[] PixelData { get; set; }
    }


    internal class MnistDataLoader
    {
        public static List<MnistImage> LoadImages(string imageFile, string labelFile)
        {
            var images = new List<MnistImage>();

            // Read image file
            using (var imageStream = new FileStream(imageFile, FileMode.Open))
            using (var imageReader = new BinaryReader(imageStream))
            {
                // Read and validate magic number
                int magicNumber = ReadBigEndianInt32(imageReader);
                if (magicNumber != 2051) // 0x00000803
                {
                    throw new InvalidDataException($"Invalid magic number in image file: {magicNumber}");
                }

                int numberOfImages = ReadBigEndianInt32(imageReader);
                int rows = ReadBigEndianInt32(imageReader);
                int cols = ReadBigEndianInt32(imageReader);

                Console.WriteLine($"Loading {numberOfImages} images of {rows}x{cols}");

                byte[] labels = null;

                // Read labels if label file is provided
                if (File.Exists(labelFile))
                {
                    using (var labelStream = new FileStream(labelFile, FileMode.Open))
                    using (var labelReader = new BinaryReader(labelStream))
                    {
                        int labelMagicNumber = ReadBigEndianInt32(labelReader);
                        if (labelMagicNumber != 2049) // 0x00000801
                        {
                            throw new InvalidDataException($"Invalid magic number in label file: {labelMagicNumber}");
                        }

                        int numberOfLabels = ReadBigEndianInt32(labelReader);

                        if (numberOfLabels != numberOfImages)
                        {
                            throw new InvalidDataException($"Mismatch: {numberOfImages} images but {numberOfLabels} labels");
                        }

                        labels = labelReader.ReadBytes(numberOfLabels);
                        Console.WriteLine($"Loaded {numberOfLabels} labels");
                    }
                }

                for (int i = 0; i < numberOfImages; i++)
                {
                    byte[] pixelData = imageReader.ReadBytes(rows * cols);

                    var image = new MnistImage
                    {
                        PixelData = pixelData,
                        Label = labels != null ? labels[i] : (byte)0
                    };

                    images.Add(image);
                }
            }

            return images;
        }
        private static int ReadBigEndianInt32(BinaryReader reader)
        {
            byte[] bytes = reader.ReadBytes(4);
            if (bytes.Length != 4)
                throw new EndOfStreamException();

            // MNIST uses big-endian, C# uses little-endian
            Array.Reverse(bytes);
            return BitConverter.ToInt32(bytes, 0);
        }
    }
}
