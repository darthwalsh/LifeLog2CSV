using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace llParser
{
    public class Parser
    {
        Bitmap image;
        public Parser(Bitmap image) {
            this.image = image;
        }

        public async Task Run() {
            var threeQuarters = new Point(image.Width * 3 / 4, 0);
            var shading = Color.FromArgb(248, 246, 249); //TODO use this

            for (var p = threeQuarters; p.Y < image.Height; ++p.Y) {
                Invert(p);
                if (p.Y % 10 == 0) {
                    await Task.Delay(16);
                }
            }
        }

        void Invert(Point p) => image.SetPixel(p.X, p.Y, image.GetPixel(p.X, p.Y).Invert());
    }

    static class Extensions
    {
        public static Color Invert(this Color c) => Color.FromArgb(c.R.Invert(), c.G.Invert(), c.B.Invert());

        public static byte Invert(this byte b) {
            unchecked {
                return (byte)(b + 128);
            }
        }
    }
}
