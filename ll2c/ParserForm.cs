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
    }
}
