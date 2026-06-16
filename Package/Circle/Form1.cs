
using System;
using System.Drawing;
using System.Windows.Forms;

namespace circle
{
    public class Form1 : Form
    {
        TextBox txtX1, txtY1, txtR;
        Button btnDraw;
        PictureBox pictureBox;
        Bitmap canvas;

        float scale = 10f;
        Point panOffset = new Point(0, 0);
        Point lastMousePos;
        bool isPanning = false;

        public Form1()
        {
            this.Text = "Midpoint Circle Drawing ";
            this.Size = new Size(900, 700);
            InitializeUI();
        }

        private void InitializeUI()
        {
            Label lblX1 = new Label { Text = "X", Location = new Point(10, 25) };
            txtX1 = new TextBox { Location = new Point(30, 10), Width = 50 };

            Label lblY1 = new Label { Text = "  Y", Location = new Point(100, 25) };
            txtY1 = new TextBox { Location = new Point(110, 10), Width = 50 };

            Label lblR = new Label { Text = "  R", Location = new Point(190, 25) };
            txtR = new TextBox { Location = new Point(190, 10), Width = 50 };

            btnDraw = new Button { Text = "Draw Circle", Location = new Point(290, 10) };
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

            Controls.AddRange(new Control[] { lblX1, txtX1, lblY1, txtY1, lblR, txtR, btnDraw, pictureBox });
        }

        private void BtnDraw_Click(object sender, EventArgs e)
        {
            int xc = int.Parse(txtX1.Text);
            int yc = int.Parse(txtY1.Text);
            int r = int.Parse(txtR.Text);

            canvas = new Bitmap(pictureBox.Width, pictureBox.Height);
            DrawAxes();
            DrawCircle(xc, yc, r);
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
                g.DrawString($" ({x},{y})   ", font, Brushes.Red, p.X + 3, p.Y);
            }
        }


           //////////////////////////////////////////
       
        // Circle Algorithm
        private void DrawCircle(int xc, int yc, int r)
        {
            Graphics g = Graphics.FromImage(canvas);
            int x = 0;
            int y = r;
            int p = 1 - r;

            void PlotCirclePoints(int cx, int cy, int px, int py)
            {
                int[,] points = new int[,]
                {
                    { px, py }, {-px, py }, { px, -py }, {-px, -py },
                    { py, px }, {-py, px }, { py, -px }, {-py, -px }
                };

                for (int i = 0; i < 8; i++)
                {
                    DrawPixel(g, cx + points[i, 0], cy + points[i, 1], Color.Green);
                }
            }

            PlotCirclePoints(xc, yc, x, y);

            while (x < y)
            {
                x++;
                if (p < 0)
                {
                    p += 2 * x + 1;
                }
                else
                {
                    y--;
                    p += 2 * (x - y) + 1;
                }
                PlotCirclePoints(xc, yc, x, y);
            }
        }
        // ///////////////////////////////////////////////////////////

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