using System;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Pendulum
{
    public partial class Canvas : Form
    {
        // Settings        
        const float GravityDelta = 0.15f;
        const float ExtraBallGravityDelta = 0.5f;
        const int ContraForceGranularity = 40;

        const int PinY = 150;
        const int PinX = 350;
        const int PointsCount = 30;
        const int PointsSpacing = 8;
        const int BallConnectorPointIdx = PointsCount - 5; // Last - 4 points
        const int BallDiameterPx = 50;
        const int DrawBall_X_Correction = 25;
        const int DrawBall_Y_Correction = 35;

        const int MouseInfluencePx = 40;        
        const int RedrawDelaySec = 10;

        // Instances
        Graphics ctx = null;
        MouseCapture mouseCapture = new MouseCapture();
        LinePoint[] linePoints = new LinePoint[PointsCount];
        Pen pen = new Pen(Color.FromArgb(153, 222, 222, 222), 2);

        // Static 
        static int width;
        static int height;
        static int boundsX;
        static int boundsY;
        
        public Canvas()
        {
            InitializeComponent();
            width = this._canvas.Width;
            height = this._canvas.Height;
            boundsX = this._canvas.Width - 1;
            boundsY = this._canvas.Height - 1;

            // Initial set
            linePoints[0].X = PinX;
            linePoints[0].Y = PinY;
            linePoints[0].PreviousX = linePoints[0].X;
            linePoints[0].PreviousY = linePoints[0].Y;
            linePoints[0].IsPin = true;
            for (int i = 1; i < PointsCount; i++)
            {
                linePoints[i].X = PinX;
                linePoints[i].Y = linePoints[i - 1].Y + PointsSpacing;
                linePoints[i].PreviousX = linePoints[i].X;
                linePoints[i].PreviousY = linePoints[i].Y;

                // Mark last 4 points as those contained within ball.
                if (i >= BallConnectorPointIdx)
                {
                    linePoints[i].IsWithinBall = true;
                }
            }

            this._canvas.MouseDown += (object sender, MouseEventArgs e) =>
            {
                mouseCapture.Button = e.Button;
                mouseCapture.PreviousX = mouseCapture.X;
                mouseCapture.PreviousY = mouseCapture.Y;
                mouseCapture.X = e.X - this._canvas.Location.X; // Bounding position within canvas
                mouseCapture.Y = e.Y - this._canvas.Location.Y;
                mouseCapture.IsDown = true;
            };
            this._canvas.MouseUp += (object sender, MouseEventArgs e) =>
            {
                mouseCapture.IsDown = false;
            };
            this._canvas.MouseMove += (object sender, MouseEventArgs e) =>
            {
                mouseCapture.PreviousX = mouseCapture.X;
                mouseCapture.PreviousY = mouseCapture.Y;
                mouseCapture.X = e.X - this._canvas.Location.X;
                mouseCapture.Y = e.Y - this._canvas.Location.Y;
            };
        }
                
        private void Canvas_Shown(object sender, EventArgs e)
        {            
            Bitmap bitmap;
            var b = new Bitmap(width, height);
            Graphics.FromImage(b).FillRectangle(Brushes.Black, 0, 0, width, height);
            
            Task.Run(() =>
            {
                while (true)
                {
                    bitmap = new Bitmap(b, width, height);
                    using (this.ctx = Graphics.FromImage(bitmap))
                    {
                        this.Redraw();
                        this.UpdateContraForce();
                        this.UpdateGravity();

                        this._canvas.Image = bitmap;
                    }
                    Thread.Sleep(RedrawDelaySec);
                }
            });
        }

        private void UpdateContraForce()
        {
            for (int j = 0; j < ContraForceGranularity; j++)
            {
                for (int i = PointsCount - 1; i >= 0; i--)
                {
                    if (linePoints[i].IsPin)
                    {
                        linePoints[i].X = PinX;
                        linePoints[i].Y = PinY;
                        continue;
                    }
                    
                    float diff_x = linePoints[i].X - linePoints[i - 1].X;
                    float diff_y = linePoints[i].Y - linePoints[i - 1].Y;
                    float dist = (float)Math.Sqrt(diff_x * diff_x + diff_y * diff_y); // Distance between connecting points                    
                    float diff = (PointsSpacing / dist) - 1; // Shows how 'dist' differs by the 'PointsSpacing' scale from the original distance with respect to sign.
                    
                    // Adjust the scale.
                    float dx = diff_x * diff * 0.5f;
                    float dy = diff_y * diff * 0.5f; 

                    linePoints[i].X     += dx;
                    linePoints[i - 1].X -= dx;

                    linePoints[i].Y     += dy;
                    linePoints[i - 1].Y -= dy;
                }
            }
        }

        private void UpdateGravity()
        {
            for (int i = PointsCount - 1; i >= 0; i--)
            {
                if (mouseCapture.IsDown)
                {
                    double diff_x = linePoints[i].X - mouseCapture.X;
                    double diff_y = linePoints[i].Y - mouseCapture.Y;
                    double dist = Math.Sqrt(diff_x * diff_x + diff_y * diff_y); // Distance between point and cursor position.
                    if (mouseCapture.Button == MouseButtons.Left && dist < MouseInfluencePx)
                    {
                        // Apply mouse down/move influence on the line.
                        linePoints[i].PreviousX = linePoints[i].X - ((mouseCapture.X - mouseCapture.PreviousX) * 1.1f);
                        linePoints[i].PreviousY = linePoints[i].Y - ((mouseCapture.Y - mouseCapture.PreviousY) * 1.1f);
                    }
                }

                // Horizontal gravity influence
                float currentX = linePoints[i].X;
                linePoints[i].X = linePoints[i].X + linePoints[i].X - linePoints[i].PreviousX;
                this.linePoints[i].PreviousX = currentX;

                // Vertical gravity influence
                float currentY = linePoints[i].Y;
                if (linePoints[i].IsWithinBall) 
                {
                    linePoints[i].Y = linePoints[i].Y + linePoints[i].Y - linePoints[i].PreviousY + GravityDelta + ExtraBallGravityDelta;
                }
                else
                {
                    linePoints[i].Y = linePoints[i].Y + linePoints[i].Y - linePoints[i].PreviousY + GravityDelta;
                }
                this.linePoints[i].PreviousY = currentY;
            }
        }
        
        private void Redraw()
        {
            for (int i = 0; i < BallConnectorPointIdx; i++)
            {
                // Draw line
                this.ctx.DrawLine(pen,
                    linePoints[i].X, linePoints[i].Y,
                    linePoints[i + 1].X, linePoints[i + 1].Y);
            }
            // Draw ball
            ctx.FillEllipse(Brushes.Red, 
                linePoints[BallConnectorPointIdx].X - DrawBall_X_Correction, linePoints[BallConnectorPointIdx].Y - DrawBall_Y_Correction, 
                BallDiameterPx, BallDiameterPx);
        }
    }
}