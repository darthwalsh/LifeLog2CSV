using System;
using System.Drawing;
using LifeLogParser;
using System.Windows.Forms;

namespace LifeLogGUI
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

            pictureBox.Image = image;
            pictureBox.Height = Screen.PrimaryScreen.WorkingArea.Height;
            var ratio = (double)image.Height / pictureBox.Height;
            pictureBox.Width = (int)(image.Width / ratio);
                        
            Top = 0;
            Size = pictureBox.Size;

            timer = new Timer {
                Interval = fps,
            };
            timer.Tick += (_, __) => pictureBox.Refresh();
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
    }
}
