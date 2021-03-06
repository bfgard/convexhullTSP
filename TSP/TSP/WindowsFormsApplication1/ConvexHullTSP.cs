﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using _2_convex_hull;

namespace TSP
{
    class ConvexHullTSP
    {
        public ConvexHullTSP()
        {

        }

        // find a convex hull give an array of cities (Ben)
        City[] FindConvexHull(City[] cities)
        {
            ConvexHullSolver solver = new ConvexHullSolver();   
            List<System.Drawing.PointF> pointList = new List<System.Drawing.PointF>();
            for (int i = 0; i < cities.Length; i++)
            {
                City c = cities[i];
                System.Drawing.PointF pointCity = new System.Drawing.PointF((float)c.X, (float)c.Y);
                pointList.Add(pointCity);
            }

            int[] hull = solver.Solve(pointList);
            City[] cityHull = new City[hull.Length];
            for (int i = 0; i < hull.Length; i++)
            {
                cityHull[i] = cities[hull[i]];
            }
            return OrderClockwise(cityHull);
        }

        // set subtractions citiesA - citiesB
        City[] SubtractCities(City[] citiesA, City[] citiesB)
        {
            // FIXME implement something better than n^2
            return citiesA.Where(x => !citiesB.Contains(x)).ToArray();
        }

        // finds Visible points (Clint)
        // Returns an array of tuples from the outer hull to the inner hull
        Tuple< List<int>[], List<int>[] > FindVisiblePoints(City[] citiesOuter, City[] citiesInner)
        {
            List<int>[] outToIn = new List<int>[citiesOuter.Length];
            List<int>[] inToOut = new List<int>[citiesInner.Length];

            for(int outer = 0; outer < citiesOuter.Length; outer++)
            {
                outToIn[outer] = new List<int>();
            }
            for(int inner = 0; inner < citiesInner.Length; inner++)
            {
                inToOut[inner] = new List<int>();
            }

            for (int outer = 0; outer < citiesOuter.Length; outer++)
            {
                City city = citiesOuter[outer];

                for (int inner = 0; inner < citiesInner.Length; inner++)
                {
                    City city1 = citiesInner[inner];
                    var intersects = false;
                    for (var i = 0; i < citiesInner.Length; i++)
                    {
                        var begin = i > 0 ? i - 1 : citiesInner.Length - 1;
	                    if(begin == inner || i == inner)
                        {
                            continue;
                        }
                        if(test_line_intersection(city, city1, citiesInner[begin], citiesInner[i]))
                        {
                            intersects = true;
                            break;
                        }
                    }
                    if (!intersects) // If we never intersected.
                    {
//	                    Debug.WriteLine($"(({citiesOuter[outer].X},{citiesOuter[outer].Y}), ({citiesInner[inner].X},{citiesInner[inner].Y})),");
                        outToIn[outer].Add(inner);
                        inToOut[inner].Add(outer);
                    }
                }
            }
            return new Tuple<List<int>[], List<int>[]>(outToIn, inToOut);
        }

	    private City[] OrderClockwise(City[] pointList)
	    {
		    // Find center
		    var cx = 0.0;
		    var cy = 0.0;
		    foreach (var City in pointList)
		    {
			    cx += City.X;
			    cy += City.Y;
		    }
		    cx /= pointList.Length;
		    cy /= pointList.Length;
		    // Sort by angle to center
		    var points = pointList.OrderBy(x => Math.Atan2(cy - x.Y, cx - x.X)).ToArray();
		    return points;
	    }


	    // Returns true if the lines intersect, otherwise false. In addition, if the lines
        // intersect the intersection point may be stored in the floats i_x and i_y.
        private static bool test_line_intersection(City city0, City city1, City city2, City city3)
	    {
		    var s1_x = city1.X - city0.X;
		    var s1_y = city1.Y - city0.Y;

		    var s2_x = city3.X - city2.X;
		    var s2_y = city3.Y - city2.Y;

		    var s = (-s1_y * (city0.X - city2.X) + s1_x * (city0.Y - city2.Y)) / (-s2_x * s1_y + s1_x * s2_y);
		    var t = ( s2_x * (city0.Y - city2.Y) - s2_y * (city0.X - city2.X)) / (-s2_x * s1_y + s1_x * s2_y);

		    return s >= 0 && s <= 1 && t >= 0 && t <= 1;
	    }

	    // returns minimum combination of two hulls (Roy)
        City[] CombineHulls(City[] citiesOuter, City[] citiesInner, Tuple<List<int>[], List<int>[]> visible)
        {
            City[] combineHull = new City[citiesOuter.Length + citiesInner.Length];

            //indecise the inner/outer/combine hull
            int i_outer = 0;
            int i_inner = -1;
            int i_combine = 0;

            combineHull[i_combine++] = citiesOuter[i_outer];

            while(i_inner == -1)
            {
                // find the closest visible point from citiesOuter[0]
                int[] local_visible = visible.Item1[0].ToArray();
                
                double min = double.MaxValue;
                foreach(int lv in local_visible) {
                    double next_v = citiesOuter[i_outer].costToGetTo(citiesInner[lv]);
                    if(next_v < min)
                    {
                        min = next_v;
                        i_inner = lv;
                    }
                }
                // if there are no visible points then it moves to the next point in citiesOuter
                if (i_inner == -1)
                {
                    combineHull[i_combine++] = citiesOuter[++i_outer];
                }
            }

            CombineState top = new CombineState(citiesOuter, citiesInner, visible, i_inner, i_outer, true, 0);
            CombineState last = null;
            const int lookahead = 20;
            // find the best connection locally with 'lookahead' look aheads.

            List<CombineState> bottom = new List<CombineState>();
            bottom.Add(top);

            //fill binary tree with possible paths
            List<CombineState> next = new List<CombineState>();
            for (int d = 0; d < lookahead; d++)
            {
                foreach (CombineState state in bottom)
                {
                    if (state == null)
                    {
                        next.Add(null);
                        next.Add(null);
                    }
                    else
                    {
                        next.Add(state.Across);
                        next.Add(state.Around);
                    }
                }
                bottom = next;
                next = new List<CombineState>();

                for(int i = 0; i < bottom.Count; i++)
                {
                    if (bottom[i] == null)
                    {
                        continue;
                    }
                    if (bottom[i].end == true)
                    {
                        if (last == null || last.length > bottom[i].length)
                            last = bottom[i];
                    }
                }
                if(last != null)
                {
                    break;
                }
            }

            while (last == null  && i_combine < combineHull.Length)
            {
                int min_index = -1;
                for (int i = 0; i < bottom.Count; i++)
                {
                    if (bottom[i] == null)
                    {
                        continue;
                    }
                    if(bottom[i].end == true)
                    {
                        if(last == null || last.length > bottom[i].length)
                            last = bottom[i];
                    }
                    if (min_index == -1 || bottom[i].length < bottom[min_index].length)
                    {
                        min_index = i;
                    }
                }
                if(last != null)
                {
                    break;
                }
                if(min_index == -1)
                {
                    return null; //no valid paths
                }
                if (min_index < bottom.Count / 2)
                {
                    //across
                    top = top.Across;
                    bottom = bottom.Take(bottom.Count / 2).ToList();
                } else
                {
                    // around
                    top = top.Around;
                    bottom = bottom.Skip(bottom.Count / 2).Take(bottom.Count / 2).ToList();
                }
                combineHull[i_combine++] = top._City;

                List<CombineState> inc = new List<CombineState>();
                foreach (CombineState state in bottom)
                {
                    if (state == null)
                    {
                        inc.Add(null);
                        inc.Add(null);
                    }
                    else
                    {
                        inc.Add(state.Across);
                        inc.Add(state.Around);
                    }
                }
                bottom = inc;
            }
            if (last == null)
                return null; // error
            List<CombineState> finalRoute = new List<CombineState>();
            last = last.parent;
            while(last != top)
            {
                finalRoute.Add(last);
                last = last.parent;
            }
            for(int i = finalRoute.Count - 1; i >= 0; i--)
            {
                combineHull[i_combine++] = finalRoute[i]._City;
            }

            return combineHull;
        }

       public City[] run(City[] cities)
        {
            List<City[]> hulls = new List<City[]>();

            City[] remaining = cities;
            while(remaining.Length > 0)
            {
                hulls.Add( FindConvexHull(remaining) );
                remaining = SubtractCities(remaining, hulls[hulls.Count - 1]);
            }

            // keeping the combine hull in hull[0] and continuing to combine with each other 1 -> N
            for (int i = 1; i < hulls.Count; i++)
            {
                hulls[0] = 
                    CombineHulls(
                        hulls[0],
                        hulls[i],
                        FindVisiblePoints(hulls[0], hulls[i])
                        );
            }

            return hulls[0];
        }
    }
}
