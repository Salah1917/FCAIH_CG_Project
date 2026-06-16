using System;
using System.Drawing;
using System.Windows.Forms;

namespace bresenham
{
    public class Form1 : Form
    {
        TextBox txtX1, txtY1, txtX2, txtY2;
        Button btnDraw;
        PictureBox pictureBox;
        Bitmap canvas;

        float scale = 20f; // pixels per unit
        Point panOffset = new Point(0, 0);
        Point lastMousePos;
        bool isPanning = false;

        public Form1()
        {
            this.Text = "Bresenham Line Drawing";
            this.Size = new Size(900, 700);
            InitializeUI();
        }

        private void InitializeUI()
        {
            Label lblX1 = new Label { Text = "X1", Location = new Point(10, 25) };
            txtX1 = new TextBox { Location = new Point(30, 10), Width = 50 };

            Label lblY1 = new Label { Text = "  Y1", Location = new Point(100, 25) };
            txtY1 = new TextBox { Location = new Point(130, 10), Width = 50 };

            Label lblX2 = new Label { Text = "  X2", Location = new Point(190, 25) };
            txtX2 = new TextBox { Location = new Point(220, 10), Width = 50 };

            Label lblY2 = new Label { Text = "  Y2", Location = new Point(280, 25) };
            txtY2 = new TextBox { Location = new Point(310, 10), Width = 50 };

            btnDraw = new Button { Text = "Draw", Location = new Point(380, 10) };
            btnDraw.Click += BtnDraw_Click;

            pictureBox = new PictureBox
            {
                Location = new Point(10, 50),
                Size = new Size(850, 600),
                BorderStyle = BorderStyle.FixedSingle
            };

            pictureBox.MouseWheel += PictureBox_MouseWheel;
            pictureBox.MouseDown += PictureBox_MouseDown;
            pictureBox.MouseMove += PictureBox_MouseMove;
            pictureBox.MouseUp += PictureBox_MouseUp;

            canvas = new Bitmap(pictureBox.Width, pictureBox.Height);
            pictureBox.Image = canvas;

            Controls.AddRange(new Control[] { lblX1, txtX1, lblY1, txtY1, lblX2, txtX2, lblY2, txtY2, btnDraw, pictureBox });
        }

        private void BtnDraw_Click(object sender, EventArgs e)
        {
            int x1 = int.Parse(txtX1.Text);
            int y1 = int.Parse(txtY1.Text);
            int x2 = int.Parse(txtX2.Text);
            int y2 = int.Parse(txtY2.Text);

            canvas = new Bitmap(pictureBox.Width, pictureBox.Height);
            DrawAxes();
            DrawBresenham(x1, y1, x2, y2);
            pictureBox.Image = canvas;
        }

        private void DrawAxes()
        {
            Graphics g = Graphics.FromImage(canvas);
            g.Clear(Color.White);

            int midX = canvas.Width / 2 + panOffset.X;
            int midY = canvas.Height / 2 + panOffset.Y;

            Pen axisPen = new Pen(Color.Black, 2);
            Font font = new Font("Arial", 7);

            g.DrawLine(axisPen, 0, midY, canvas.Width, midY);
            g.DrawLine(axisPen, midX, 0, midX, canvas.Height);

            for (int x = 0; x < canvas.Width; x += (int)(2 * scale))
            {
                g.DrawLine(Pens.LightGray, x, 0, x, canvas.Height);
                int val = (int)((x - midX) / scale);
                g.DrawString(val.ToString(), font, Brushes.Black, x + 2, midY + 2);
            }

            for (int y = 0; y < canvas.Height; y += (int)(2 * scale))
            {
                g.DrawLine(Pens.LightGray, 0, y, canvas.Width, y);
                int val = (int)((midY - y) / scale);
                if (y != midY)
                    g.DrawString(val.ToString(), font, Brushes.Black, midX + 2, y);
            }
        }

        private Point ConvertToScreen(float x, float y)
        {
            int midX = canvas.Width / 2 + panOffset.X;
            int midY = canvas.Height / 2 + panOffset.Y;
            return new Point((int)(midX + x * scale), (int)(midY - y * scale));
        }

        private void DrawPixel(Graphics g, float x, float y, Color color, int octant)
        {
            Point screen = ConvertToScreen(x, y);
            if (screen.X >= 0 && screen.X < canvas.Width && screen.Y >= 0 && screen.Y < canvas.Height)
            {
                g.FillRectangle(new SolidBrush(color), screen.X, screen.Y, 3, 3);
                Font font = new Font("Arial", 7);
                Brush brush = Brushes.DarkRed;
                g.DrawString($"  ({x},{y}) O{octant}  ", font, brush, screen.X + 4, screen.Y - 10);
            }
        }

        private void DrawLineManual(Graphics g, Point start, Point end, Color color)
        {
            int x0 = start.X;
            int y0 = start.Y;
            int x1 = end.X;
            int y1 = end.Y;

            int dx = x1 - x0;
            int dy = y1 - y0;

            int sx = dx >= 0 ? 1 : -1;
            int sy = dy >= 0 ? 1 : -1;

            dx = Abs(dx);
            dy = Abs(dy);

            int err = dx - dy;

            while (true)
            {
                if (x0 >= 0 && x0 < canvas.Width && y0 >= 0 && y0 < canvas.Height)
                    canvas.SetPixel(x0, y0, color);

                if (x0 == x1 && y0 == y1)
                    break;

                int e2 = 2 * err;
                if (e2 > -dy)
                {
                    err -= dy;
                    x0 += sx;
                }
                if (e2 < dx)
                {
                    err += dx;
                    y0 += sy;
                }
            }
        }


        /// algorithm Bresenham
        private void DrawBresenham(int x1, int y1, int x2, int y2)
        {
            Graphics g = Graphics.FromImage(canvas);

            int dx = x2 - x1;
            int dy = y2 - y1;

            int absDx = Abs(dx);
            int absDy = Abs(dy);

            int sx = dx >= 0 ? 1 : -1;
            int sy = dy >= 0 ? 1 : -1;

            int x = x1, y = y1;

            bool isSteep = absDy > absDx;

            int err = (isSteep ? absDy : absDx) / 2;

            int octant = GetOctant(dx, dy);

            Point startScreen = ConvertToScreen(x1, y1);
            Point endScreen = ConvertToScreen(x2, y2);

            for (int i = 0; i <= (isSteep ? absDy : absDx); i++)
            {
                DrawPixel(g, x, y, Color.Blue, octant);

                if (isSteep)
                {
                    y += sy;
                    err -= absDx;
                    if (err < 0)
                    {
                        x += sx;
                        err += absDy;
                    }
                }
                else
                {
                    x += sx;
                    err -= absDy;
                    if (err < 0)
                    {
                        y += sy;
                        err += absDx;
                    }
                }
            }

            // Draw line
            DrawLineManual(g, startScreen, endScreen, Color.Red);
        }
         
         // number of octan
        private int GetOctant(int dx, int dy)
        {
            if (dx >= 0 && dy >= 0)
                return (Abs(dx) >= Abs(dy)) ? 1 : 2;
            else if (dx < 0 && dy >= 0)
                return (Abs(dx) >= Abs(dy)) ? 4 : 3;
            else if (dx < 0 && dy < 0)
                return (Abs(dx) >= Abs(dy)) ? 5 : 6;
            else
                return (Abs(dx) >= Abs(dy)) ? 8 : 7;
        }

        private int Abs(int n)
        {
            return (n < 0) ? -n : n;
        }

        private void PictureBox_MouseWheel(object sender, MouseEventArgs e)
        {
            float oldScale = scale;
            if (e.Delta > 0) scale *= 1.1f;
            else scale /= 1.1f;

            panOffset.X = (int)(e.X - (e.X - panOffset.X) * (scale / oldScale));
            panOffset.Y = (int)(e.Y - (e.Y - panOffset.Y) * (scale / oldScale));

            BtnDraw_Click(null, null);
        }

        private void PictureBox_MouseDown(object sender, MouseEventArgs e)
        {
            isPanning = true;
            lastMousePos = e.Location;
        }

        private void PictureBox_MouseMove(object sender, MouseEventArgs e)
        {
            if (isPanning && e.Button == MouseButtons.Left)
            {
                int dx = e.X - lastMousePos.X;
                int dy = e.Y - lastMousePos.Y;
                panOffset.X += dx;
                panOffset.Y += dy;
                lastMousePos = e.Location;
                BtnDraw_Click(null, null);
            }
        }

        private void PictureBox_MouseUp(object sender, MouseEventArgs e)
        {
            isPanning = false;
        }

    }
}
