using System;
using System.Drawing;
using System.Threading.Tasks;
using ImageParse;

namespace LifeLogParser
{
    static class Colors
    {
        public static Color DayShading => Color.FromArgb(248, 246, 249);
        public static Color Background => Color.FromArgb(255, 255, 255);
    }

    public class Parser
    {
        IAsyncBitmap image;
        Finder finder;
        public Parser(IAsyncBitmap image) {
            this.image = image;
            finder = new Finder(image);
        }

        public async Task Run() {
            var threeQuarters = new Point(image.Width * 3 / 4, 0);

            var sunTop = await finder.FindColor(threeQuarters, Colors.DayShading, Dir.Down);
            var sunTopLeft = (await finder.FindColor(sunTop, Colors.Background, Dir.Left)).Right();
            var sunTopRight = await finder.FindColor(sunTop, Colors.Background, Dir.Right);
            var sunBottomLeft = await finder.FindColor(sunTopLeft, Colors.Background, Dir.Down);

            var dayHeight = sunBottomLeft.Y - sunTop.Y;
            var dayWidth = sunTopRight.X - sunTopRight.X;
        }
    }
}
