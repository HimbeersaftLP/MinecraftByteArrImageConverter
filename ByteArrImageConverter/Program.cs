using System;
using System.Drawing;
using System.IO;

namespace ByteArrImageConverter {
    class Program {
        const int byteAmount = 64 * 64 * 4;

        static void Main(string[] args) {
            Console.Write("Convert\na) Image to Text\nb) Text to Image\n(a, b): ");
            string selection = Console.ReadLine();

            if (selection == "a" || selection == "b") {
                Console.Write("Path of file (drag file here): ");
                string path = Console.ReadLine();
                if (path.StartsWith('"') && path.EndsWith('"')) path = path.Substring(1, path.Length - 2);

                bool succ;
                if (selection == "a") succ = ImageToByteArr(path);
                else succ = ByteArrToImage(path);

                if (!succ) Console.WriteLine("Invalid image size, must be 64x64 pixels with alpha values!");
            } else {
                Console.WriteLine("Option not recognised!");
            }

            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
        }

        static bool ImageToByteArr(string path) {
            var img = new Bitmap(Image.FromFile(path));
            if (img.Width != 64 || img.Height != 64) return false;

            Console.WriteLine("Converting");
            var byteArr = new byte[byteAmount];
            for (int y = 0; y < 64; y++) {
                for (int x = 0; x < 64; x++) {
                    var pixel = img.GetPixel(x, y);
                    int i = 64 * 4 * y + x * 4;
                    byteArr[i] = pixel.R;
                    byteArr[i + 1] = pixel.G;
                    byteArr[i + 2] = pixel.B;
                    byteArr[i + 3] = pixel.A;
                }
            }
            var file = File.Create(path + ".txt");
            file.Write(byteArr);
            Console.WriteLine("Saved as " + path + ".txt");
            return true;
        }

        static bool ByteArrToImage(string path) {
            var pixels = File.ReadAllBytes(path);
            if (pixels.Length != byteAmount) return false;

            Console.WriteLine("Converting...");
            var img = new Bitmap(64, 64);
            for (int i = 0; i < byteAmount; i += 4) {
                int x = (i / 4) % 64,
                y = (i / 4) / 64,
                r = pixels[i],
                g = pixels[i + 1],
                b = pixels[i + 2],
                a = pixels[i + 3];
                img.SetPixel(x, y, Color.FromArgb(a, r, g, b));
            }
            img.Save(path + ".png");
            Console.WriteLine("Saved as " + path + ".png");
            return true;
        }
    }
}