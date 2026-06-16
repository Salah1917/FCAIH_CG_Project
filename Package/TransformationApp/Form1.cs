using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace TransformationApp
{
    public class Form1 : Form
    {
        private TextBox[] txtPoints;
        private TextBox txtTransX, txtTransY, txtScaleX, txtScaleY, txtRotateAngle, txtShearX, txtShearY;
        private ComboBox cmbOperation;
        private Button btnApply, btnReset;
        private PictureBox pictureBox;
        private Bitmap canvas;

        private List<PointF> trianglePoints = new List<PointF>();
        private float scale = 40f;
        private Point panOffset = new Point(0, 0);
        private Point lastMousePos;
        private bool isPanning = false;

        public Form1()
        {
            Text = "2D Transformations";
            Size = new Size(1000, 700);
            InitializeUI();
        }

        private void InitializeUI()
        {
            Label[] lbls = new Label[6];
            txtPoints = new TextBox[6];
            string[] coords = { "X1", "Y1", "X2", "Y2", "X3", "Y3" };

            for (int i = 0; i < 6; i++)
            {
                lbls[i] = new Label { Text = coords[i], Location = new Point(10 + (i * 55), 10), Width = 40 };
                txtPoints[i] = new TextBox { Location = new Point(10 + (i * 55), 33), Width = 50 };
                Controls.Add(lbls[i]);
                Controls.Add(txtPoints[i]);
            }

            txtTransX = new TextBox { Location = new Point(10, 70), Width = 50, PlaceholderText = "TX" };
            txtTransY = new TextBox { Location = new Point(70, 70), Width = 50, PlaceholderText = "TY" };
            txtScaleX = new TextBox { Location = new Point(130, 70), Width = 50, PlaceholderText = "SX" };
            txtScaleY = new TextBox { Location = new Point(190, 70), Width = 50, PlaceholderText = "SY" };
            txtRotateAngle = new TextBox { Location = new Point(250, 70), Width = 50, PlaceholderText = "θ" };
            txtShearX = new TextBox { Location = new Point(310, 70), Width = 50, PlaceholderText = "ShX" };
            txtShearY = new TextBox { Location = new Point(370, 70), Width = 50, PlaceholderText = "ShY" };

            Controls.AddRange(new Control[] { txtTransX, txtTransY, txtScaleX, txtScaleY, txtRotateAngle, txtShearX, txtShearY });

            cmbOperation = new ComboBox { Location = new Point(430, 70), Width = 120 };
            cmbOperation.Items.AddRange(new string[] { "Translate", "Scale", "Rotate", "Reflect X", "Reflect Y", "Shear X", "Shear Y" });
            cmbOperation.SelectedIndex = 0;
            Controls.Add(cmbOperation);

            btnApply = new Button { Text = "Apply", Location = new Point(560, 70) };
            btnApply.Click += BtnApply_Click;
            Controls.Add(btnApply);

            btnReset = new Button { Text = "Reset", Location = new Point(640, 70) };
            btnReset.Click += (s, e) => {
                trianglePoints.Clear();
                RedrawCanvas();
            };
            Controls.Add(btnReset);

            pictureBox = new PictureBox { Location = new Point(10, 110), Size = new Size(960, 540), BorderStyle = BorderStyle.FixedSingle };
            pictureBox.MouseWheel += PictureBox_MouseWheel;
            pictureBox.MouseDown += PictureBox_MouseDown;
            pictureBox.MouseMove += PictureBox_MouseMove;
            pictureBox.MouseUp += PictureBox_MouseUp;
            Controls.Add(pictureBox);

            canvas = new Bitmap(pictureBox.Width, pictureBox.Height);
            pictureBox.Image = canvas;
            RedrawCanvas();
        }

        private float Sin(float angle)
        {
            angle %= 2 * (float)Math.PI;
            float term = angle, sum = 0;
            for (int i = 1, sign = 1; i <= 9; i += 2, sign *= -1)
            {
                sum += sign * term;
                term *= angle * angle / ((i + 1) * (i + 2));
            }
            return sum;
        }

        private float Cos(float angle)
        {
            angle %= 2 * (float)Math.PI;
            float term = 1, sum = 0;
            for (int i = 0, sign = 1; i <= 8; i += 2, sign *= -1)
            {
                sum += sign * term;
                term *= angle * angle / ((i + 1) * (i + 2));
            }
            return sum;
        }

        private void BtnApply_Click(object sender, EventArgs e)
        {
            if (trianglePoints.Count == 0)
            {
                for (int i = 0; i < 6; i += 2)
                {
                    if (float.TryParse(txtPoints[i].Text, out float x) && float.TryParse(txtPoints[i + 1].Text, out float y))
                        trianglePoints.Add(new PointF(x, y));
                }

                if (trianglePoints.Count != 3)
                {
                    MessageBox.Show("Please enter valid coordinates for all three points.");
                    return;
                }
            }

            string op = cmbOperation.SelectedItem.ToString();
            float tx = GetVal(txtTransX), ty = GetVal(txtTransY);
            float sx = GetVal(txtScaleX, 1), sy = GetVal(txtScaleY, 1);
            float angleDeg = GetVal(txtRotateAngle);
            float angleRad = angleDeg * (float)Math.PI / 180f;
            float shx = GetVal(txtShearX), shy = GetVal(txtShearY);

            // operations algorithm 
            for (int i = 0; i < trianglePoints.Count; i++)          
            {
                PointF p = trianglePoints[i];
                switch (op)
                {
                    case "Translate": p = new PointF(p.X + tx, p.Y + ty); break;
                    case "Scale": p = new PointF(p.X * sx, p.Y * sy); break;
                    case "Rotate":
                        float cos = Cos(angleRad);
                        float sin = Sin(angleRad);
                        p = new PointF(p.X * cos - p.Y * sin, p.X * sin + p.Y * cos);
                        break;
                    case "Reflect X": p = new PointF(p.X, -p.Y); break;
                    case "Reflect Y": p = new PointF(-p.X, p.Y); break;
                    case "Shear X": p = new PointF(p.X + shx * p.Y, p.Y); break;
                    case "Shear Y": p = new PointF(p.X, p.Y + shy * p.X); break;
                }
                trianglePoints[i] = p;
            }

            RedrawCanvas();
        }

        private float GetVal(TextBox txt, float def = 0)
        {
            return float.TryParse(txt.Text, out float val) ? val : def;
        }

        private void RedrawCanvas()
        {
            using (Graphics g = Graphics.FromImage(canvas))
            {
                g.Clear(Color.White);
                int midX = canvas.Width / 2 + panOffset.X;
                int midY = canvas.Height / 2 + panOffset.Y;

                Pen gridPen = new Pen(Color.LightGray);
                for (int x = midX % (int)scale; x < canvas.Width; x += (int)scale)
                    g.DrawLine(gridPen, x, 0, x, canvas.Height);
                for (int y = midY % (int)scale; y < canvas.Height; y += (int)scale)
                    g.DrawLine(gridPen, 0, y, canvas.Width, y);

                Pen axisPen = new Pen(Color.Black, 2);
                g.DrawLine(axisPen, 0, midY, canvas.Width, midY);
                g.DrawLine(axisPen, midX, 0, midX, canvas.Height);

                Font font = new Font("Arial", 7);
                for (int x = midX % (int)scale; x < canvas.Width; x += (int)scale)
                {
                    int val = (int)((x - midX) / scale);
                    g.DrawString(val.ToString(), font, Brushes.Black, x + 2, midY + 2);
                }
                for (int y = midY % (int)scale; y < canvas.Height; y += (int)scale)
                {
                    int val = (int)((midY - y) / scale);
                    if (val != 0)
                        g.DrawString(val.ToString(), font, Brushes.Black, midX + 2, y);
                }

                if (trianglePoints.Count == 3)
                {
                    Pen triPen = new Pen(Color.Blue, 2);
                    Brush pointBrush = Brushes.Red;
                    for (int i = 0; i < 3; i++)
                    {
                        PointF p = trianglePoints[i];
                        Point screen = new Point((int)(midX + p.X * scale), (int)(midY - p.Y * scale));
                        g.FillEllipse(pointBrush, screen.X - 3, screen.Y - 3, 6, 6);
                        g.DrawString($"({p.X:F1},{p.Y:F1})", font, Brushes.Black, screen.X + 5, screen.Y);
                        PointF p2 = trianglePoints[(i + 1) % 3];
                        Point s2 = new Point((int)(midX + p2.X * scale), (int)(midY - p2.Y * scale));
                        g.DrawLine(triPen, screen, s2);
                    }
                }
            }
            pictureBox.Image = canvas;
        }

        private void PictureBox_MouseWheel(object sender, MouseEventArgs e)
        {
            float oldScale = scale;
            scale *= e.Delta > 0 ? 1.1f : 0.9f;
            panOffset.X = (int)(e.X - (e.X - panOffset.X) * (scale / oldScale));
            panOffset.Y = (int)(e.Y - (e.Y - panOffset.Y) * (scale / oldScale));
            RedrawCanvas();
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
                panOffset.X += e.X - lastMousePos.X;
                panOffset.Y += e.Y - lastMousePos.Y;
                lastMousePos = e.Location;
                RedrawCanvas();
            }
        }

        private void PictureBox_MouseUp(object sender, MouseEventArgs e)
        {
            isPanning = false;
        }

    }
}
