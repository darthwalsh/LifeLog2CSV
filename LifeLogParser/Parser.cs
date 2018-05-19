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

            var sunTop = await FindColor(threeQuarters, Colors.DayShading, dy: 1);
            var sunTopLeft = await FindColor(sunTop, Colors.Background, dx: -1);
            ++sunTopLeft.X;
            var sunTopRight = await FindColor(sunTop, Colors.Background, dx: 1);
            var sunBottomLeft = await FindColor(sunTopLeft, Colors.Background, dy: 1);

            var dayHeight = sunBottomLeft.Y - sunTop.Y;
            var dayWidth = sunTopRight.X - sunTopRight.X;
        }

        async Task<Point> FindColor(Point p, Color color, int dx = 0, int dy = 0) {
            for (; 0 <= p.Y && p.Y < image.Height && 0 <= p.X && p.X < image.Height; p.X += dx, p.Y += dy) {
                if (await image.GetPixel(p) == color) {
                    await Pulse(p);
                    return p;
                }
            }
            throw new System.InvalidOperationException($"Couldn't find {color}!");
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
}
