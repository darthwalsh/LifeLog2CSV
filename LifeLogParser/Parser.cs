using System.Drawing;
using System.Threading.Tasks;

namespace LifeLogParser
{
    static class Colors
    {
        public static Color DayShading => Color.FromArgb(248, 246, 249);
        public static Color BelowDayShading => Color.FromArgb(248, 246, 249);
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

            for (var p = threeQuarters; p.Y < image.Height; ++p.Y) {
                if (await image.GetPixel(p) == Colors.BelowDayShading) {
                    break;
                }
            }
        }
    }
}
