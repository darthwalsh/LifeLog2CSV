using System;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using LifeLogParser;

namespace LifeLogGUI
{
    public partial class ParserForm : Form
    {
        public ParserForm() {
            InitializeComponent();
        }

        private async void Form_Load(object sender, EventArgs e) {
            const int fps = 1000 / 60;

            var image = new Bitmap(Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), 
                @"Downloads\Screenshot_20170906-100313.png"));

            pictureBox.Image = image;
            pictureBox.Height = Screen.PrimaryScreen.WorkingArea.Height;
            var ratio = (double)image.Height / pictureBox.Height;
            pictureBox.Width = (int)(image.Width / ratio);
                        
            Top = 0;
            Size = pictureBox.Size;
            
            using (var timer = new Timer { Interval = fps })
            {
                timer.Tick += (_, __) => pictureBox.Refresh();
                timer.Start();

                var parser = new Parser(new InvertingTrackingBitmap
                {
                    KeepCount = 2000,
                    IAsyncBitmap = new DelayedBitmap
                    {
                        DelayInterval = fps,
                        DelayCount = 10,
                        IAsyncBitmap = new WrappingBitmap
                        {
                            Bitmap = image,
                        },
                    },
                });
                await parser.Run(); 
            }
        }
    }
}
