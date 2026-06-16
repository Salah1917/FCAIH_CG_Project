
using System;
using System.Drawing;
using System.Windows.Forms;

namespace dda
{
    public class Form1 : Form
    {
        TextBox txtX1, txtY1, txtX2, txtY2;
        Button btnDraw;
        PictureBox pictureBox;
        Bitmap canvas;

        float scale = 10f; // pixels per unit
        Point panOffset = new Point(0, 0);
        Point lastMousePos;
        bool isPanning = false;

        public Form1()
        {
            this.Text = "DDA Line Drawing";
            this.Size = new Size(900, 700);
            InitializeUI();
        }

        private void InitializeUI()
        {
            Label lblX1 = new Label { Text = "X1:", Location = new Point(10, 25) };
            txtX1 = new TextBox { Location = new Point(40, 10), Width = 50 };

            Label lblY1 = new Label { Text = "  Y1:", Location = new Point(100, 25) };
            txtY1 = new TextBox { Location = new Point(130, 10), Width = 50 };

            Label lblX2 = new Label { Text = "  X2:", Location = new Point(190, 25) };
            txtX2 = new TextBox { Location = new Point(220, 10), Width = 50 };

            Label lblY2 = new Label { Text = "  Y2:", Location = new Point(280, 25) };
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
            DrawDDA(x1, y1, x2, y2);
            DrawStraightLine(x1, y1, x2, y2);
            pictureBox.Image = canvas;
        }

        private void DrawAxes()
        {
            Graphics g = Graphics.FromImage(canvas);
            g.Clear(Color.White);

            int midX = canvas.Width / 2 + panOffset.X;
            int midY = canvas.Height / 2 + panOffset.Y;

            Pen axisPen = new Pen(Color.Black, 2);
            Font font = new Font("Arial", 9);

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

        private void DrawPixel(Graphics g, float x, float y, Color color, bool drawLabel = true)
        {
            Point p = ConvertToScreen(x, y);
            if (p.X >= 0 && p.X < canvas.Width && p.Y >= 0 && p.Y < canvas.Height)
            {
                Brush b = new SolidBrush(color);
                g.FillRectangle(b, p.X, p.Y, 2, 2);
                if (drawLabel)
                {
                    Font font = new Font("Arial", 9);
                    g.DrawString($"({Round(x)},{Round(y)})", font, Brushes.Red, p.X + 3, p.Y);
                }
            }
        }


        /////////////////////////////
      
        // DDA Algorithm
        private void DrawDDA(int x1, int y1, int x2, int y2)
        {
            Graphics g = Graphics.FromImage(canvas);
            float dx = x2 - x1;
            float dy = y2 - y1;
            int steps = Max(Abs((int)dx), Abs((int)dy));
            float xInc = dx / steps;
            float yInc = dy / steps;

            float x = x1, y = y1;

            for (int i = 0; i <= steps; i++)
            {
                Point screenPoint = ConvertToScreen(x, y);
                if (screenPoint.X >= 0 && screenPoint.X < canvas.Width && screenPoint.Y >= 0 && screenPoint.Y < canvas.Height)
                {
                    g.FillRectangle(Brushes.Blue, screenPoint.X, screenPoint.Y, 2, 2);
                    Font font = new Font("Arial", 7);
                    g.DrawString($"({Round(x)},{Round(y)})", font, Brushes.Red, screenPoint.X + 3, screenPoint.Y);
                }
                x += xInc;
                y += yInc;
            }
        }

        ///////////////////////////////////////////////////

        private void DrawStraightLine(int x1, int y1, int x2, int y2)
        {
            Graphics g = Graphics.FromImage(canvas);
            Point p1 = ConvertToScreen(x1, y1);
            Point p2 = ConvertToScreen(x2, y2);

            Pen straightLinePen = new Pen(Color.Green, 1)
            {
                DashStyle = System.Drawing.Drawing2D.DashStyle.Solid
            };

            g.DrawLine(straightLinePen, p1, p2);
        }

        private int Abs(int n) => (n < 0) ? -n : n;
        private int Max(int a, int b) => (a > b) ? a : b;
        private int Round(float n) => (int)(n + 0.5f);

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
