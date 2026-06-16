using System;
using System.Drawing;
using System.Windows.Forms;

namespace ellipse
{
    public class Form1 : Form
    {
        TextBox txtX, txtY, txtRx, txtRy;
        Button btnDraw;
        PictureBox pictureBox;
        Bitmap canvas;

        float scale = 10f;
        Point panOffset = new Point(0, 0);
        Point lastMousePos;
        bool isPanning = false;

        public Form1()
        {
            this.Text = "Midpoint Ellipse Drawing ";
            this.Size = new Size(900, 700);
            InitializeUI();
        }

        private void InitializeUI()
        {
            Label lblX = new Label { Text = "X", Location = new Point(10, 26) };
            txtX = new TextBox { Location = new Point(30, 10), Width = 50 };

            Label lblY = new Label { Text = "   Y", Location = new Point(100, 26) };
            txtY = new TextBox { Location = new Point(110, 10), Width = 50 };

            Label lblRx = new Label { Text = "   Rx", Location = new Point(188, 26) };
            txtRx = new TextBox { Location = new Point(200, 10), Width = 50 };

            Label lblRy = new Label { Text = "   Ry", Location = new Point(275, 26) };
            txtRy = new TextBox { Location = new Point(290, 10), Width = 50 };

            btnDraw = new Button { Text = "Draw Ellipse", Location = new Point(375, 10) };
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

            Controls.AddRange(new Control[] { lblX, txtX, lblY, txtY, lblRx, txtRx, lblRy, txtRy, btnDraw, pictureBox });
        }

        private void BtnDraw_Click(object sender, EventArgs e)
        {
            int xc = int.Parse(txtX.Text);
            int yc = int.Parse(txtY.Text);
            int rx = int.Parse(txtRx.Text);
            int ry = int.Parse(txtRy.Text);

            canvas = new Bitmap(pictureBox.Width, pictureBox.Height);
            DrawAxes();
            DrawEllipse(xc, yc, rx, ry);
            pictureBox.Image = canvas;
        }

        private void DrawAxes()
        {
            Graphics g = Graphics.FromImage(canvas);
            g.Clear(Color.White);

            int midX = canvas.Width / 2 + panOffset.X;
            int midY = canvas.Height / 2 + panOffset.Y;

            Pen axisPen = new Pen(Color.Black, 2);
            Font font = new Font("Arial", 8);

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

        private void DrawPixel(Graphics g, float x, float y, Color color)
        {
            Point p = ConvertToScreen(x, y);
            if (p.X >= 0 && p.X < canvas.Width && p.Y >= 0 && p.Y < canvas.Height)
            {
                Brush b = new SolidBrush(color);
                g.FillRectangle(b, p.X, p.Y, 2, 2);
                Font font = new Font("Arial", 7);
                g.DrawString($" ({x},{y}) ", font, Brushes.Red, p.X + 3, p.Y);
            }
        }

        
        // Ellipse Algorithm
        private void DrawEllipse(int xc, int yc, int rx, int ry)
        {
            Graphics g = Graphics.FromImage(canvas);

            float x = 0;
            float y = ry;
            float rxSq = rx * rx;
            float rySq = ry * ry;
            float dx = 2 * rySq * x;
            float dy = 2 * rxSq * y;

            float p1 = rySq - (rxSq * ry) + (0.25f * rxSq);

            void PlotPoints(float px, float py)
            {
                DrawPixel(g, xc + px, yc + py, Color.Purple);
                DrawPixel(g, xc - px, yc + py, Color.Purple);
                DrawPixel(g, xc + px, yc - py, Color.Purple);
                DrawPixel(g, xc - px, yc - py, Color.Purple);
            }

            while (dx < dy)
            {
                PlotPoints(x, y);
                if (p1 < 0)
                {
                    x++;
                    dx += 2 * rySq;
                    p1 += dx + rySq;
                }
                else
                {
                    x++;
                    y--;
                    dx += 2 * rySq;
                    dy -= 2 * rxSq;
                    p1 += dx - dy + rySq;
                }
            }

            float p2 = rySq * (x + 0.5f) * (x + 0.5f) + rxSq * (y - 1) * (y - 1) - rxSq * rySq;

            while (y >= 0)
            {
                PlotPoints(x, y);
                if (p2 > 0)
                {
                    y--;
                    dy -= 2 * rxSq;
                    p2 += rxSq - dy;
                }
                else
                {
                    y--;
                    x++;
                    dx += 2 * rySq;
                    dy -= 2 * rxSq;
                    p2 += dx - dy + rxSq;
                }
            }
        }
    //    //////////////////////////////////////////////////////////

        private void PictureBox_MouseWheel(object sender, MouseEventArgs e)
        {
            float oldScale = scale;
            scale *= (e.Delta > 0) ? 1.1f : 0.9f;
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
