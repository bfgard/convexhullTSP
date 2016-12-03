using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace _1_convex_hull
{
    class Hull
    {
        public Hull()
        {
            points = new List<PointF>();
        }
        public Hull(PointF point)
        {
            points = new List<PointF>();
            points.Add(point);
        }
        public List<PointF> points;

        public PointF this[int i]
        {
            get
            {
                return points[i];
            }
            set
            {
                points[i] = value;
            }
        }
        public int Count
        {
            get
            {
                return points.Count;
            }
        }


        public int rmp; // right most point
        public int lmp; // left most point

        public static Hull combine(Hull left, Hull right)
        {
            Tuple<int, int> top = findTop(left, right);
            Tuple<int, int> bottom = findBottom(left, right);

            Hull r = new Hull();
            int R = r.lmp = left.lmp;
            do
            {
                r.points.Add(left[R]);
                R++;

            } while (R <= top.Item1);

            R = top.Item2;

            while (true)
            {
                if (R == right.rmp)
                    r.rmp = r.Count;

                if (R == bottom.Item2)
                    break;

                r.points.Add(right[R]);
                R = modAdd(R, 1, right.Count);
            }
            r.points.Add(right[R]);

            R = bottom.Item1;
            while (R != left.lmp && R < left.points.Count)
            {
                r.points.Add(left[R]);
                R++;
            }

            return r;
        }

        // g is 1 for top or -1 for bottom
        private static Tuple<int, int> findTop(Hull left, Hull right)
        {
            int l = left.rmp;
            int r = right.lmp;

            double s = slope(left[l], right[r]);
            bool L, R;
            do
            {
                L = R = false;
                while (slope(left[modAdd(l, -1, left.Count)], right[r]) < s)
                {
                    L = true;
                    s = slope(left[l = modAdd(l, -1, left.Count)], right[r]);
                }
                while (slope(left[l], right[modAdd(r, 1, right.Count)]) > s)
                {
                    R = true;
                    s = slope(left[l], right[r = modAdd(r, 1, right.Count)]);
                }
            } while (L || R);

            return new Tuple<int,int>(l, r);
        }

        private static Tuple<int, int> findBottom(Hull left, Hull right)
        {
            int l = left.rmp;
            int r = right.lmp;

            double s = slope(left[l], right[r]);
            bool L, R;
            do
            {
                L = R = false;
                while (slope(left[modAdd(l, 1, left.Count)], right[r]) > s)
                {
                    L = true;
                    s = slope(left[l = modAdd(l, 1, left.Count)], right[r]);
                }
                while (slope(left[l], right[modAdd(r, -1, right.Count)]) < s)
                {
                    R = true;
                    s = slope(left[l], right[r = modAdd(r, -1, right.Count)]);
                }
            } while (L || R);

            return new Tuple<int, int>(l, r);
        }

        private static double slope(PointF a, PointF b)
        {
            return (((double)-a.Y) - ((double)-b.Y)) / ((double)a.X - (double)b.X);
        }

        private static int modAdd(int i, int g, int size) {
            i += g;
            if (i < 0) i = size + i;
            if (i >= size) i = i % size;
            return i;
        }


    }
}
