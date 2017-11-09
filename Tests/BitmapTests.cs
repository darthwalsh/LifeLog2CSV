using System.Drawing;
using System.Threading.Tasks;
using LifeLogGUI;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests
{
    // TODO make this an EXE that loads and runs BitmapTests through reflection?
    [TestClass]
    public class BitmapTests
    {
        [TestMethod]
        public async Task InvertingTrackingBitmapTests() {
            var image = new Bitmap(5, 1);
            var bitmap = new InvertingTrackingBitmap {
                KeepCount = 2,
                IAsyncBitmap = new WrappingBitmap {
                    Bitmap = image,
                },
            };

            var original = image.GetPixel(0, 0);
            var inverted = original.Invert();

            var firstGet = await bitmap.GetPixel(new Point(0, 0));
            Assert.AreEqual(original, firstGet, "Getting a pixel returns real color");
            Assert.AreEqual(inverted, image.GetPixel(0, 0), "Backing first pixel is inverted");

            var secondGet = await bitmap.GetPixel(new Point(1, 0));
            Assert.AreEqual(original, secondGet, "Getting second pixel returns real color");
            Assert.AreEqual(inverted, image.GetPixel(1, 0), "Backing second pixel is inverted");

            var thirdGet = await bitmap.GetPixel(new Point(2, 0));
            Assert.AreEqual(original, thirdGet, "Getting third pixel returns real color");
            Assert.AreEqual(inverted, image.GetPixel(2, 0), "Backing third pixel is inverted");
            Assert.AreEqual(original, image.GetPixel(0, 0), "Backing first pixel is back to original");

            var repeatedGet = await bitmap.GetPixel(new Point(2, 0));
            Assert.AreEqual(original, firstGet, "Getting repeated pixel returns real color");
            Assert.AreEqual(inverted, image.GetPixel(2, 0), "Backing repeated pixel stays inverted");
            Assert.AreEqual(inverted, image.GetPixel(1, 0), "Backing second pixel stays inverted");
        }

        [TestMethod]
        public void InvertTests() {
            Assert.AreEqual(Color.FromArgb(0x80, 0x81, 0x82), Color.FromArgb(0x0, 0x1, 0x2).Invert(), "dark");
            Assert.AreEqual(Color.FromArgb(0x40, 0x41, 0x42), Color.FromArgb(0xC0, 0xC1, 0xC2).Invert(), "light");
        }
    }
}
