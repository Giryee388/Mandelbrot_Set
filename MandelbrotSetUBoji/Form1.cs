using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace MandelbrotSetUBoji
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private Mandelbrot _mandelbrot;
        public Bitmap _bitmap;
        public Bitmap _bitmap1;
        int w;
        int h;
        int saveRez = 8000;

        private double _zoom = 5;
        private double camPosX = 0;
        private double camPosY = 0;

        private int colorOffset = 0;

        private void Form1_Load(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Maximized;
            w = this.Height;
            h = this.Height;

            pictureBox1.Width = Height;
            pictureBox1.Height = Height;
            pictureBox1.Location = new Point(0,0);


            _bitmap = new Bitmap(w, h);
            _bitmap1 = new Bitmap(saveRez, saveRez);
            _mandelbrot = new Mandelbrot();
            pictureBox1.Image = _bitmap;

            textBox1.Enabled = false;
            textBox2.Enabled = false;
            textBox3.Enabled = false;
            textBox4.Enabled = false;
            textBox5.Enabled = false;
            textBox6.Enabled = false;
            textBox7.Enabled = false;
            textBox8.Enabled = false;

            button1.Enabled = false;
            button2.Enabled = false;


            UpdateMandelbrot();
        }

        private void UpdateMandelbrot()
        {
            Rectangle rect = new Rectangle(0, 0, _bitmap.Width, _bitmap.Height);
            BitmapData data = _bitmap.LockBits(rect, ImageLockMode.ReadWrite, _bitmap.PixelFormat);
            int depth = Image.GetPixelFormatSize(data.PixelFormat) / 8; 
            byte[] buffer = new byte[data.Width * data.Height * depth];

            //copy iz bitmape u niz bytova
            Marshal.Copy(data.Scan0, buffer, 0, buffer.Length);
            double max = 0;
            double avg = 0;
            int halfW = w / 2;
            int halfH = h / 2;
            Parallel.For(0, w * h, i =>
            {
                int x = i % w;
                int y = i / h;

                //krajnja pozicija
                double mX = (double)(x - halfW) / w * _zoom + camPosX;
                double mY = (double)(y - halfH) / h * -_zoom + camPosY;
                double calc = _mandelbrot.Calculate(mX, mY);

                int bitmapOffset = (y * w + x) * depth;

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
            _bitmap.UnlockBits(data);

            pictureBox1.Image = _bitmap;
            pictureBox1.Update();
        }
        


        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            var speed = 0.1;

            if (e.Modifiers == Keys.Control && e.KeyCode == Keys.S)
            {
                SaveWindow sw = new SaveWindow(_zoom, camPosX, camPosY, colorOffset, _mandelbrot);
                sw.ShowDialog();

            }
            else
            {

                switch (e.KeyCode)
                {
                    case Keys.Q:
                        _mandelbrot.TotalIterations -= 10;
                        break;
                    case Keys.W:
                        _mandelbrot.TotalIterations += 10;
                        break;

                    case Keys.A:
                        w -= 10;
                        h -= 10;
                        _bitmap = new Bitmap(w, h);
                        break;
                    case Keys.S:
                        w += 10;
                        h += 10;
                        _bitmap = new Bitmap(w, h);
                        break;

                    case Keys.Z:
                        colorOffset -= 5;
                        break;
                    case Keys.X:
                        colorOffset += 5;
                        break;

                    case Keys.Subtract:
                        _zoom *= 1.1;
                        break;
                    case Keys.Add:
                        _zoom *= 0.9;
                        break;

                    case Keys.Left:
                        camPosX -= speed * _zoom;
                        break;
                    case Keys.Right:
                        camPosX += speed * _zoom;
                        break;
                    case Keys.Up:
                        camPosY += speed * _zoom;
                        break;
                    case Keys.Down:
                        camPosY -= speed * _zoom;
                        break;

                    case Keys.Space:
                        _zoom = 5;
                        camPosX = 0;
                        camPosY = 0;
                        break;
                }
                UpdateMandelbrot();


            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            colorOffset -= 50;

        }

        private void button2_Click(object sender, EventArgs e)
        {
            colorOffset += 50;
        }
    }
}