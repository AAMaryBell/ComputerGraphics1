using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

namespace WinFormsApp1
{
    public partial class Form1 : Form
    {
        private Graphics gr;
        private Point A = new Point(100, 100);
        private Point B = new Point(300, 400);
        private Point C = new Point(200, 500);
        private Point S;
        private Point P = new Point(50, 50);
        private Point Q = new Point(150, 160);
        private Point T = new Point(253, 60);
        
        private Point routeCenter;
        private Point figureCenter;
        private List<Point> routePoints = new List<Point>();
        private List<Point> figurePoints = new List<Point>();

        private int angle = 0;
        private float t = 0;
        private int routeEditIndx = -1;
        private int figureEditIndx = -1;
        private int moveIndx = -1;
        
        List<Point> routeRotationPoints = new List<Point>();
        List<Point> figureMovingPoints = new List<Point>();

        private int figureMovingIndex = 0;
        
        public Form1() {
            InitializeComponent();
        }
        
        private void Form1_Load_1(object sender, EventArgs e) {
            gr = CreateGraphics();
            label1.Text = routePoints.Count.ToString();
            label2.Text = "-";
            label3.Text = figurePoints.Count.ToString();
        }

        private void button1_Click(object sender, EventArgs e) {
            
            routeCenter = new Point((A.X + B.X + C.X) / 3, (A.Y + B.Y + C.Y) / 3);
            
            gr.DrawLine(Pens.Crimson, A, B);
            gr.DrawLine(Pens.Crimson, B, C);
            gr.DrawLine(Pens.Crimson, C, A);

            gr.DrawEllipse(Pens.Blue, routeCenter.X - 3, routeCenter.Y - 3, 7, 7);
        }

        private void button2_Click(object sender, EventArgs e) {
            timer1.Enabled = !timer1.Enabled;
        }

        private void timer1_Elapsed(object sender, ElapsedEventArgs e) {
            double fi = Math.PI * angle / 180;
            int sumX = 0, sumY = 0;
            foreach (var p in routePoints) { 
                sumX += p.X; 
                sumY += p.Y;
            }
            routeCenter = new Point(sumX / routePoints.Count, sumY / routePoints.Count);

            foreach (var p in routePoints) { 
                routeRotationPoints.Add(new Point(
                        (int) (routeCenter.X + (p.X - routeCenter.X) * Math.Cos(fi) - (p.Y - routeCenter.Y) * Math.Sin(fi)),
                          (int) (routeCenter.Y + (p.X - routeCenter.X) * Math.Sin(fi) + (p.Y - routeCenter.Y) * Math.Cos(fi))));
            }
            
            gr.Clear(DefaultBackColor);
            gr.DrawPolygon(Pens.Teal, routeRotationPoints.ToArray());
            foreach (var p in routeRotationPoints) { 
                gr.DrawRectangle(Pens.CornflowerBlue, p.X, p.Y, 7, 7);
            }
            //------------------------------------------------------------------------
            if (figurePoints.Count > 1) {
                gr.DrawPolygon(Pens.Tomato, figurePoints.ToArray());
                foreach (var p in figurePoints)
                {
                    gr.DrawRectangle(Pens.Green, p.X, p.Y, 7, 7);
                }
            }

            //------------------------------------------------------------------------
            angle += 3;
            routeRotationPoints.Clear();
        }

        private void Form1_MouseDown(object sender, MouseEventArgs e) {
            
            if (e.Button == MouseButtons.Left) {
                gr.Clear(DefaultBackColor);
                routePoints.Add(new Point(e.Location.X, e.Location.Y));
                label1.Text = routePoints.Count.ToString();

                foreach (var p in routePoints) {
                    gr.DrawRectangle(Pens.CornflowerBlue, p.X, p.Y, 7, 7);
                }
                foreach (var p in figurePoints) {
                    gr.DrawRectangle(Pens.Green, p.X, p.Y, 7, 7);
                }
                
                if (routePoints.Count > 1) gr.DrawPolygon(Pens.Teal, routePoints.ToArray());
                if (figurePoints.Count > 1) gr.DrawPolygon(Pens.Tomato, figurePoints.ToArray());
            }

            if (e.Button == MouseButtons.Right)
            {
                routeEditIndx = -1;
                figureEditIndx = -1;
                for(int i = 0; i < routePoints.Count; i++)
                {
                    if ((routePoints[i].X - 7 < e.X) && (e.X < routePoints[i].X + 7) &&
                        (routePoints[i].Y - 7 < e.Y) && (e.Y < routePoints[i].Y + 7)) routeEditIndx = i;
                }

                if (routeEditIndx == -1) {
                    for(int i = 0; i < figurePoints.Count; i++)
                    {
                        if ((figurePoints[i].X - 7 < e.X) && (e.X < figurePoints[i].X + 7) &&
                            (figurePoints[i].Y - 7 < e.Y) && (e.Y < figurePoints[i].Y + 7)) figureEditIndx = i;
                    }
                }

                if (routeEditIndx != -1) label2.Text = "choosed: r " + routeEditIndx;
                else if (figureEditIndx != -1) label2.Text = "choosed: f " + figureEditIndx;
                else {
                    label2.Text = "-";
                    gr.Clear(DefaultBackColor);
                    figurePoints.Add(new Point(e.Location.X, e.Location.Y));
                    label3.Text = figurePoints.Count.ToString();

                    foreach (var p in figurePoints) {
                        gr.DrawRectangle(Pens.Green, p.X, p.Y, 7, 7);
                    }
                    
                    foreach (var p in routePoints) {
                        gr.DrawRectangle(Pens.CornflowerBlue, p.X, p.Y, 7, 7);
                    }
                    if (routePoints.Count > 1) gr.DrawPolygon(Pens.Teal, routePoints.ToArray());

                    if (figurePoints.Count > 1) gr.DrawPolygon(Pens.Tomato, figurePoints.ToArray());
                }


            }
        }

        private void button3_Click(object sender, EventArgs e) {
            routePoints.Clear();
            label1.Text = routePoints.Count.ToString();
            label2.Text = "-";
            figurePoints.Clear();
            label3.Text = figurePoints.Count.ToString();
            routeRotationPoints.Clear();
            figureMovingPoints.Clear();
            timer1.Enabled = false;
            timer2.Enabled = false;
            timer3.Enabled = false;
            timer4.Enabled = false;
            angle = 0;
            routeEditIndx = -1;
            figureEditIndx = -1;
            t = 0.0f;
            gr.Clear(DefaultBackColor);
        }

        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            if ((e.Button == MouseButtons.Right) && (routeEditIndx != -1))
            {
                routePoints[routeEditIndx] = new Point(e.X, e.Y);
                gr.Clear(DefaultBackColor);
                foreach(var p in routePoints) {
                    gr.DrawRectangle(Pens.CornflowerBlue, p.X, p.Y, 7, 7);
                }
                foreach (var p in figurePoints) {
                    gr.DrawRectangle(Pens.Green, p.X, p.Y, 7, 7);
                }
                if (routePoints.Count > 1) gr.DrawPolygon(Pens.Teal, routePoints.ToArray());
                if (figurePoints.Count > 1) gr.DrawPolygon(Pens.Tomato, figurePoints.ToArray());
            }
            if ((e.Button == MouseButtons.Right) && (figureEditIndx != -1))
            {
                figurePoints[figureEditIndx] = new Point(e.X, e.Y);
                gr.Clear(DefaultBackColor);
                foreach(var p in routePoints) {
                    gr.DrawRectangle(Pens.CornflowerBlue, p.X, p.Y, 7, 7);
                }
                foreach (var p in figurePoints) {
                    gr.DrawRectangle(Pens.Green, p.X, p.Y, 7, 7);
                }
                if (routePoints.Count > 1) gr.DrawPolygon(Pens.Teal, routePoints.ToArray());
                if (figurePoints.Count > 1) gr.DrawPolygon(Pens.Tomato, figurePoints.ToArray());
            }
        }

        private void button3_Click_1(object sender, EventArgs e) {
            timer2.Enabled = !timer2.Enabled;
        }


        private void timer2_Elapsed(object sender, ElapsedEventArgs e) {
            /*Point O = new Point((A.X + B.X + C.X) / 3, (A.Y + B.Y + C.Y) / 3);
            S = new Point((int) ((1 - t) * P.X + t * Q.X), (int) ((1 - t) * P.Y + t * Q.Y));
            angle += 3;
            double fi = Math.PI * angle / 180;
            
            Point A1 = new Point((int) (S.X + (A.X - O.X) * Math.Cos(fi) - (A.Y - O.Y) * Math.Sin(fi)),
                                (int) (S.Y + (A.X - O.X) * Math.Sin(fi) + (A.Y - O.Y) * Math.Cos(fi))); 
            Point B1 = new Point((int) (S.X + (B.X - O.X) * Math.Cos(fi) - (B.Y - O.Y) * Math.Sin(fi)),
                (int) (S.Y + (B.X - O.X) * Math.Sin(fi) + (B.Y - O.Y) * Math.Cos(fi))); 
            Point C1 = new Point((int) (S.X + (C.X - O.X) * Math.Cos(fi) - (C.Y - O.Y) * Math.Sin(fi)),
                (int) (S.Y + (C.X - O.X) * Math.Sin(fi) + (C.Y - O.Y) * Math.Cos(fi))); 
            
            gr.Clear(DefaultBackColor);
            gr.DrawLine(Pens.Crimson, A1, B1);
            gr.DrawLine(Pens.Crimson, B1, C1);
            gr.DrawLine(Pens.Crimson, C1, A1);
            
            Point Q1 = new Point((A1.X + B1.X + C1.X) / 3, (A1.Y + B1.Y + C1.Y) / 3);

            gr.DrawEllipse(Pens.Blue, Q1.X - 3, Q1.Y - 3, 7, 7);

            t += 0.01f;
            if (t > 1)
            {
                t = 0.0f;
                P = Q;
                Q = T;
                A = A1;
                B = B1;
                C = C1;
            }*/

            gr.Clear(DefaultBackColor);
            
            int routePointsCount = routePoints.Count;
            
            int sumX = 0, sumY = 0;
            foreach (var p in figurePoints) { 
                sumX += p.X; 
                sumY += p.Y;
            }
            figureCenter = new Point(sumX / figurePoints.Count, sumY / figurePoints.Count);

            S = new Point((int) ((1 - t) * routePoints[figureMovingIndex%routePointsCount].X + t * routePoints[(figureMovingIndex+1)%routePointsCount].X), 
                          (int) ((1 - t) * routePoints[figureMovingIndex%routePointsCount].Y + t * routePoints[(figureMovingIndex+1)%routePointsCount].Y));
            
            foreach (var p in figurePoints) {
                figureMovingPoints.Add(new Point(S.X + (p.X - figureCenter.X) - (p.Y - figureCenter.Y), 
                                                     S.Y + (p.X - figureCenter.X) + (p.Y - figureCenter.Y)));
            }

            gr.DrawPolygon(Pens.Tomato, figureMovingPoints.ToArray());

            sumX = 0; sumY = 0;
            foreach (var p in figureMovingPoints) { 
                sumX += p.X; 
                sumY += p.Y;
            }
            
            Point newFigureCenter = new Point(sumX / figureMovingPoints.Count, sumY / figureMovingPoints.Count);

            foreach (var p in figureMovingPoints) {
                gr.DrawRectangle(Pens.Green, p.X, p.Y, 7, 7);
            }

            gr.DrawEllipse(Pens.Blue, newFigureCenter.X - 3, newFigureCenter.Y - 3, 7, 7);
            
            foreach(var p in routePoints) {
                gr.DrawRectangle(Pens.CornflowerBlue, p.X, p.Y, 7, 7);
            }
            if (routePoints.Count > 1) gr.DrawPolygon(Pens.Teal, routePoints.ToArray());

            t += 0.05f;
            if (t > 1) {
                t = 0.0f; 
                figureMovingIndex++;
            }
            figureMovingPoints.Clear();
        }

        private void button1_Click_1(object sender, EventArgs e) {
            timer2.Enabled = !timer2.Enabled;
        }

        private void button4_Click(object sender, EventArgs e) {
            timer3.Enabled = !timer3.Enabled;
        }

        private void timer3_Elapsed(object sender, ElapsedEventArgs e)
        {
            gr.Clear(DefaultBackColor);
            
            angle += 3;
            double fi = Math.PI * angle / 180;
            
            int routePointsCount = routePoints.Count;
            
            int sumX = 0, sumY = 0;
            foreach (var p in figurePoints) { 
                sumX += p.X; 
                sumY += p.Y;
            }
            figureCenter = new Point(sumX / figurePoints.Count, sumY / figurePoints.Count);

            S = new Point((int) ((1 - t) * routePoints[figureMovingIndex%routePointsCount].X + t * routePoints[(figureMovingIndex+1)%routePointsCount].X), 
                          (int) ((1 - t) * routePoints[figureMovingIndex%routePointsCount].Y + t * routePoints[(figureMovingIndex+1)%routePointsCount].Y));
            
            /*S = new Point((int) ((1 - t) * routeRotationPoints[figureMovingIndex%routePointsCount].X + t * routeRotationPoints[(figureMovingIndex+1)%routePointsCount].X), 
                (int) ((1 - t) * routeRotationPoints[figureMovingIndex%routePointsCount].Y + t * routeRotationPoints[(figureMovingIndex+1)%routePointsCount].Y));
            */
            foreach (var p in figurePoints)
            {
                figureMovingPoints.Add(new Point(
                    (int) (S.X + (p.X - figureCenter.X) * Math.Cos(fi) - (p.Y - figureCenter.Y) * Math.Sin(fi)),
                    (int) (S.Y + (p.X - figureCenter.X) * Math.Sin(fi) + (p.Y - figureCenter.Y) * Math.Cos(fi))));
            }

            gr.DrawPolygon(Pens.Tomato, figureMovingPoints.ToArray());

            sumX = 0; sumY = 0;
            foreach (var p in figureMovingPoints) { 
                sumX += p.X; 
                sumY += p.Y;
            }
            
            Point newFigureCenter = new Point(sumX / figureMovingPoints.Count, sumY / figureMovingPoints.Count);

            foreach (var p in figureMovingPoints) {
                gr.DrawRectangle(Pens.Green, p.X, p.Y, 7, 7);
            }

            gr.DrawEllipse(Pens.Blue, newFigureCenter.X - 3, newFigureCenter.Y - 3, 7, 7);
            
            foreach(var p in routePoints) {
                gr.DrawRectangle(Pens.CornflowerBlue, p.X, p.Y, 7, 7);
            }
            if (routePoints.Count > 1) gr.DrawPolygon(Pens.Teal, routePoints.ToArray());

            t += 0.05f;
            if (t > 1) {
                t = 0.0f; 
                figureMovingIndex++;
            }
            figureMovingPoints.Clear();
        }

        private void button3_Click_2(object sender, EventArgs e) {
            timer4.Enabled = !timer4.Enabled;
        }

        private void timer4_Elapsed(object sender, ElapsedEventArgs e)
        {
            gr.Clear(DefaultBackColor);
            
            double fi = Math.PI * angle / 180;
            int sumX = 0, sumY = 0;
            foreach (var p in routePoints) { 
                sumX += p.X; 
                sumY += p.Y;
            }
            routeCenter = new Point(sumX / routePoints.Count, sumY / routePoints.Count);

            foreach (var p in routePoints) { 
                routeRotationPoints.Add(new Point(
                    (int) (routeCenter.X + (p.X - routeCenter.X) * Math.Cos(fi) - (p.Y - routeCenter.Y) * Math.Sin(fi)),
                    (int) (routeCenter.Y + (p.X - routeCenter.X) * Math.Sin(fi) + (p.Y - routeCenter.Y) * Math.Cos(fi))));
            }
            
            gr.DrawPolygon(Pens.Teal, routeRotationPoints.ToArray());
            foreach (var p in routeRotationPoints) { 
                gr.DrawRectangle(Pens.CornflowerBlue, p.X, p.Y, 7, 7);
            }
            
            
            angle += 3;

            int routePointsCount = routeRotationPoints.Count;

            sumX = 0; sumY = 0;
            foreach (var p in figurePoints) { 
                sumX += p.X; 
                sumY += p.Y;
            }
            figureCenter = new Point(sumX / figurePoints.Count, sumY / figurePoints.Count);

            S = new Point((int) ((1 - t) * routeRotationPoints[figureMovingIndex%routePointsCount].X + t * routeRotationPoints[(figureMovingIndex+1)%routePointsCount].X), 
                (int) ((1 - t) * routeRotationPoints[figureMovingIndex%routePointsCount].Y + t * routeRotationPoints[(figureMovingIndex+1)%routePointsCount].Y));
            
            foreach (var p in figurePoints)
            {
                figureMovingPoints.Add(new Point(
                    (int) (S.X + (p.X - figureCenter.X) * Math.Cos(-3*fi) - (p.Y - figureCenter.Y) * Math.Sin(-3*fi)),
                    (int) (S.Y + (p.X - figureCenter.X) * Math.Sin(-3*fi) + (p.Y - figureCenter.Y) * Math.Cos(-3*fi))));
            }

            gr.DrawPolygon(Pens.Tomato, figureMovingPoints.ToArray());

            sumX = 0; sumY = 0;
            foreach (var p in figureMovingPoints) { 
                sumX += p.X; 
                sumY += p.Y;
            }
            
            Point newFigureCenter = new Point(sumX / figureMovingPoints.Count, sumY / figureMovingPoints.Count);

            foreach (var p in figureMovingPoints) {
                gr.DrawRectangle(Pens.Green, p.X, p.Y, 7, 7);
            }

            gr.DrawEllipse(Pens.Blue, newFigureCenter.X - 3, newFigureCenter.Y - 3, 7, 7);
            
            foreach(var p in routeRotationPoints) {
                gr.DrawRectangle(Pens.CornflowerBlue, p.X, p.Y, 7, 7);
            }
            if (routeRotationPoints.Count > 1) gr.DrawPolygon(Pens.Teal, routeRotationPoints.ToArray());

            t += 0.05f;
            if (t > 1) {
                t = 0.0f; 
                figureMovingIndex++;
            }
            
            routeRotationPoints.Clear();
            figureMovingPoints.Clear();
        }
    }
}