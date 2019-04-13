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
            var threeQuarters = new Point(image.Width * 3 / 4, 0);

            var sunTop = await FindColor(threeQuarters, Colors.DayShading, Dir.Down);
            var sunTopLeft = (await FindColor(sunTop, Colors.Background, Dir.Left)).Right();
            var sunTopRight = await FindColor(sunTop, Colors.Background, Dir.Right);
            var sunBottomLeft = await FindColor(sunTopLeft, Colors.Background, Dir.Down);

            var dayHeight = sunBottomLeft.Y - sunTop.Y;
            var dayWidth = sunTopRight.X - sunTopRight.X;
        }

        async Task<Point> FindColor(Point p, Color color, Func<Point, Point> nextPoint) {
            for (; 0 <= p.Y && p.Y < image.Height && 0 <= p.X && p.X < image.Height; p = nextPoint(p)) {
                if (await image.GetPixel(p) == color) {
                    await Pulse(p);
                    return p;
                }
            }
            throw new InvalidOperationException($"Couldn't find {color}!");
        }

        async Task Pulse(Point center) {
            var size = new Size(8, 8);

            var bounding = new Rectangle(center - size, size + size);
            for (var p = new Point(bounding.Left, bounding.Top); bounding.Contains(p); ++p.X) { await image.GetPixel(p); }
            for (var p = new Point(bounding.Left, bounding.Top); bounding.Contains(p); ++p.Y) { await image.GetPixel(p); }
            for (var p = new Point(bounding.Left, bounding.Bottom - 1); bounding.Contains(p); ++p.X) { await image.GetPixel(p); }
            for (var p = new Point(bounding.Right - 1, bounding.Top); bounding.Contains(p); ++p.Y) { await image.GetPixel(p); }
        }
    }

    public static class Dir
    {
        public static Point Up(this Point p) => new Point(p.X, p.Y - 1);
        public static Point Down(this Point p) => new Point(p.X, p.Y + 1);
        public static Point Left(this Point p) => new Point(p.X - 1, p.Y);
        public static Point Right(this Point p) => new Point(p.X + 1, p.Y);
    }
}
