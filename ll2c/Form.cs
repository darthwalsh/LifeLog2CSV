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

namespace ll2c
{
    public partial class Form : System.Windows.Forms.Form
    {
        Bitmap image;
        Timer timer;

        public Form()
        {
            InitializeComponent();
        }

        private async void Form_Load(object sender, EventArgs e)
        {
            image = new Bitmap(@"C:\Users\cwalsh\Downloads\Screenshot_20170906-100313.png");

            imageLabel.Size = image.Size;
            imageLabel.Image = image;
            Size = image.Size;

            timer = new Timer {
                Interval = 1000 / 60,
            };
            timer.Tick += (_, __) => imageLabel.Refresh();
            timer.Start();

            var parser = new Parser(image);
            await parser.Run();
        }
    }
}
