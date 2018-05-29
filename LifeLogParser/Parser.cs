using System;
using System.Drawing;
using System.Threading.Tasks;

namespace LifeLogParser
{
    static class Colors
    {
        public static Color DayShading => Color.FromArgb(248, 246, 249);
        public static Color Background => Color.FromArgb(255, 255, 255);
    }

    public interface IAsyncBitmap
    {
        int Width { get; }
        int Height { get; }
        bool Synchronous { get; set; }
        void SetPixel(Point p, Color color);
        Task<Color> GetPixel(Point p);
    }

    public class Parser
    {
        IAsyncBitmap image;
        public Parser(IAsyncBitmap image) {
            this.image = image;
        }

        public async Task Run() {
            image.Synchronous = true;

            var threeQuarters = new Point(image.Width * 3 / 4, 0);

            var sunTop = await FindColor(threeQuarters, Colors.DayShading, Down);
            var sunTopLeft = (await FindColor(sunTop, Colors.Background, Left)).Right();
            var sunTopRight = await FindColor(sunTop, Colors.Background, Right);
            var sunBottomLeft = await FindColor(sunTopLeft, Colors.Background, Down);

            var dayHeight = sunBottomLeft.Y - sunTop.Y;
            var dayWidth = sunTopRight.X - sunTopRight.X;

            //image.Synchronous = false;

            //TODO search for text for amount of time slept i.e "7:22" 
            var textFarRight = new Point(image.Width, (sunTop.Y + sunBottomLeft.Y) / 2).Left(); TODO need to go left for background, then find not background!
            var textRight = (await FindAnotherColor(textFarRight, Colors.Background, Left)).Right();

            var textFarLeft = new Point(sunTopRight.X, textRight.Y).Right();
            var textLeft = await FindAnotherColor(textFarLeft, Colors.Background, Right);
        }

        Task<Point> FindColor(Point p, Color color, Func<Point, Point> nextPoint) =>
            FindPixel(p, async pp => await image.GetPixel(pp) == color, nextPoint);

        Task<Point> FindAnotherColor(Point p, Color color, Func<Point, Point> nextPoint) =>
            FindPixel(p, async pp => await image.GetPixel(pp) != color, nextPoint);

        async Task<Point> FindPixel(Point p, Func<Point, Task<bool>> finished, Func<Point, Point> nextPoint) {
            for (p = nextPoint(p); 0 <= p.Y && p.Y < image.Height && 0 <= p.X && p.X < image.Height; p = nextPoint(p)) {
                if (await finished(p)) {
                    await Pulse(p);
                    return p;
                }
            }
            throw new InvalidOperationException($"Couldn't find Color!");
        }

        async Task Pulse(Point center) {
            var size = new Size(8, 8);

            var bounding = new Rectangle(center - size, size + size);
            for (var p = new Point(bounding.Left, bounding.Top); bounding.Contains(p); ++p.X) { await image.GetPixel(p); }
            for (var p = new Point(bounding.Left, bounding.Top); bounding.Contains(p); ++p.Y) { await image.GetPixel(p); }
            for (var p = new Point(bounding.Left, bounding.Bottom - 1); bounding.Contains(p); ++p.X) { await image.GetPixel(p); }
            for (var p = new Point(bounding.Right - 1, bounding.Top); bounding.Contains(p); ++p.Y) { await image.GetPixel(p); }
        }

        //static Point Between(Point a, Point b) => new Point((a.X + b.X) / 2, (a.Y + b.Y) / 2);

        // Duplicating this because https://stackoverflow.com/q/38984853/771768 *Sigh
        static Point Up(Point p) => p.Up();
        static Point Down(Point p) => p.Down();
        static Point Left(Point p) => p.Left();
        static Point Right(Point p) => p.Right();
    }

    public static class Extensions
    {
        public static Point Up(this Point p) => new Point(p.X, p.Y - 1);
        public static Point Down(this Point p) => new Point(p.X, p.Y + 1);
        public static Point Left(this Point p) => new Point(p.X - 1, p.Y);
        public static Point Right(this Point p) => new Point(p.X + 1, p.Y);
    }
}
