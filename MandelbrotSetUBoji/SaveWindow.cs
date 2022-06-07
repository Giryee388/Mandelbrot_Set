using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MandelbrotSetUBoji
{
    public partial class SaveWindow : Form
    {
        public SaveWindow()
        {
            InitializeComponent();
        }
        Bitmap _bitmap1;
        public SaveWindow(double _Zoom, double CamPosX, double CamPosY, int ColorOffset, Mandelbrot _Mandelbrot)
        {
            _zoom = _Zoom;
            camPosX = CamPosX;
            camPosY = CamPosY;
            colorOffset = ColorOffset;
            _mandelbrot = _Mandelbrot;
            InitializeComponent();
        }
        private double _zoom;
        private double camPosX;
        private double camPosY;
        private Mandelbrot _mandelbrot;
        int saveRez = 8000;

        private int colorOffset;
        private void UpdateMandelbrotForSave()
        {
            int saveRez = Convert.ToInt32(textBox1.Text);
            Rectangle rect = new Rectangle(0, 0, _bitmap1.Width, _bitmap1.Height);
            BitmapData data = _bitmap1.LockBits(rect, ImageLockMode.ReadWrite, _bitmap1.PixelFormat);
            int depth = Image.GetPixelFormatSize(data.PixelFormat) / 8;
            byte[] buffer = new byte[data.Width * data.Height * depth];

            //copy iz bitmape u niz bytova
            Marshal.Copy(data.Scan0, buffer, 0, buffer.Length);
            int halfW = saveRez / 2;
            int halfH = saveRez / 2;
            Parallel.For(0, saveRez * saveRez, i =>
            {
                int x = i % saveRez;
                int y = i / saveRez;

                //krajnja pozicija
                double mX = (double)(x - halfW) / saveRez * _zoom + camPosX;
                double mY = (double)(y - halfH) / saveRez * -_zoom + camPosY;
                double calc = _mandelbrot.Calculate(mX, mY);

                int bitmapOffset = (y * saveRez + x) * depth;

                if (calc == 1)
                {
                    buffer[bitmapOffset] = 0;
                    buffer[bitmapOffset + 1] = 0;
                    buffer[bitmapOffset + 2] = 0;
                    //buffer[bitmapOffset] = 255;
                    //buffer[bitmapOffset + 1] = 255;
                    //buffer[bitmapOffset + 2] = 255;
                }
                else
                {
                    HslColor hslColor = new HslColor((calc * 250 + colorOffset) % 360, 200, 20 + Math.Sin(calc) * 340);
                    Color col = hslColor;
                    buffer[bitmapOffset] = col.R;
                    buffer[bitmapOffset + 1] = col.G;
                    buffer[bitmapOffset + 2] = col.B;
                    //buffer[bitmapOffset] = 255;
                    //buffer[bitmapOffset + 1] = 255;
                    //buffer[bitmapOffset + 2] = 255;
                }

                buffer[bitmapOffset + 3] = 255;
            });
            Marshal.Copy(buffer, 0, data.Scan0, buffer.Length);
            _bitmap1.UnlockBits(data);
        }
        private void SaveWindow_Load(object sender, EventArgs e)
        {
            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            saveRez = Convert.ToInt32(textBox1.Text);
            _bitmap1 = new Bitmap(saveRez, saveRez);
            UpdateMandelbrotForSave();
            var dialog = new SaveFileDialog
            {
                DefaultExt = "png",
                FileName = "mandelbrot.png",
                Filter = "PNG Image (*.png)|*.png"
            };
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                //_mandelbrot.TotalIterations += 1000;
                //UpdateMandelbrotForSave();

                _bitmap1.Save(dialog.FileName, ImageFormat.Png);

                //_mandelbrot.TotalIterations -= 1000;
            }
        }
    }
}
