using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using llParser;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;

namespace ll2c
{
    public partial class ParserForm : Form
    {
        const int fps = 1000 / 60;
        Bitmap image;
        Timer timer;

        public ParserForm() {
            InitializeComponent();
        }

        private async void Form_Load(object sender, EventArgs e) {
            image = new Bitmap(@"C:\Users\cwalsh\Downloads\Screenshot_20170906-100313.png");

            imageLabel.Size = image.Size;
            imageLabel.Image = image;
            Size = image.Size;

            timer = new Timer {
                Interval = fps,
            };
            timer.Tick += (_, __) => imageLabel.Refresh();
            timer.Start();


            var parser = new Parser(new InvertingTrackingBitmap {
                KeepCount = 2000,
                IAsyncBitmap = new DelayedBitmap {
                    DelayInterval = fps,
                    DelayCount = 10,
                    IAsyncBitmap = new WrappingBitmap {
                        Bitmap = image,
                    },
                },
            });
            await parser.Run();
        }

        //TODO refactor, tests, ???CodeReview SE
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
