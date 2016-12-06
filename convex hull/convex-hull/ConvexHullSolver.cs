using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using _1_convex_hull;

namespace _2_convex_hull
{
	public class ConvexHullSolver
    {
        System.Drawing.Graphics g;
        System.Windows.Forms.PictureBox pictureBoxView;

        public ConvexHullSolver(System.Drawing.Graphics g, System.Windows.Forms.PictureBox pictureBoxView)
        {
            this.g = g;
            this.pictureBoxView = pictureBoxView;
        }

        public void Refresh()
        {
            // Use this especially for debugging and whenever you want to see what you have drawn so far
            pictureBoxView.Refresh();
        }

        public void Pause(int milliseconds)
        {
            // Use this especially for debugging and to animate your algorithm slowly
            pictureBoxView.Refresh();
            System.Threading.Thread.Sleep(milliseconds);
        }
        Pen pen = new Pen(Color.Red);

        public void Solve(List<PointF> pointList)
        {
            pointList.Sort(delegate(PointF a, PointF b)
            {
                float result = a.X - b.X;
                if (result < 0) return -1;
                if (result > 0) return 1;
                return 0;
            });

            List<Hull> hulls = new List<Hull>();
            for (int i = 0; i < pointList.Count; i++)
            {
                hulls.Add(new Hull(pointList[i]));

            }
            while (hulls.Count > 1) { 
                List<Hull> joins = new List<Hull>();
                int i = 0;
                while (i + 1 < hulls.Count) {
                    joins.Add(Hull.combine(hulls[i], hulls[i + 1]));
                    i += 2;
                }
                while(i < hulls.Count)
                {
                    joins.Add(hulls[i]);
                    i++;
                }
                hulls = joins;
            }
            DrawHull(hulls[0]);
        }

        void DrawHull(Hull hull)
        {
            for (int i = 0; i < hull.Count - 1; i++)
            {
                g.DrawLine(pen, hull[i], hull[i + 1]);
            }
            g.DrawLine(pen, hull[hull.Count - 1], hull[0]);

            Refresh();
        }
    }
}
