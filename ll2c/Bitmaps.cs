using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Threading.Tasks;
using llParser;

namespace ll2c
{
    //TODO tests, ???CodeReview SE
    class WrappingBitmap : IAsyncBitmap
    {
        public Bitmap Bitmap { get; set; }
        public int Width => Bitmap.Width;
        public int Height => Bitmap.Height;
        public void SetPixel(Point p, Color color) => Bitmap.SetPixel(p.X, p.Y, color);
        public Task<Color> GetPixel(Point p) => Task.FromResult(Bitmap.GetPixel(p.X, p.Y));
    }

    abstract class DelgatingAsyncBitmap : IAsyncBitmap
    {
        public IAsyncBitmap IAsyncBitmap { private get; set; }
        public int Width => IAsyncBitmap.Width;
        public int Height => IAsyncBitmap.Height;
        public void SetPixel(Point p, Color color) => IAsyncBitmap.SetPixel(p, color);
        public virtual Task<Color> GetPixel(Point p) => IAsyncBitmap.GetPixel(p);
    }

    //TODO Should everything use .ConfigureAwait(false) ?
    class DelayedBitmap : DelgatingAsyncBitmap
    {
        Stopwatch stopwatch = Stopwatch.StartNew();
        int count;

        public int DelayInterval { get; set; }
        public int DelayCount { get; set; } = int.MaxValue;

        public async override Task<Color> GetPixel(Point p) {
            ++count;
            if (stopwatch.ElapsedMilliseconds >= DelayInterval || count > DelayCount) {
                await Task.Delay(DelayInterval);
                stopwatch.Restart();
                count = 0;
            }
            return await base.GetPixel(p);
        }
    }

    class InvertingTrackingBitmap : DelgatingAsyncBitmap
    {
        public int KeepCount { get; set; }

        Queue<Point> touched = new Queue<Point>();

        public override async Task<Color> GetPixel(Point p) {
            var baseColor = await base.GetPixel(p);

            if (touched.Contains(p)) {
                // color has already been inverted; restore it
                baseColor = baseColor.Invert();
            } else {
                touched.Enqueue(p);
                base.SetPixel(p, baseColor.Invert());

                if (touched.Count > KeepCount) {
                    var toClear = touched.Dequeue();
                    var inverted = await base.GetPixel(toClear);
                    base.SetPixel(toClear, inverted.Invert());
                }
            }

            return baseColor;
        }
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
