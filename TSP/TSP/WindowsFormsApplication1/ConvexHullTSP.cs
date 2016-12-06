using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using _2_convex_hull;

namespace TSP
{
    class ConvexHullTSP
    {
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
            return cityHull;
        }

        // set subtractions citiesA - citiesB
        City[] SubtractCities(City[] citiesA, City[] citiesB)
        {
            // FIXME implement something better than n^2
            return citiesA.Where(x => !citiesB.Contains(x)).ToArray();
        }

        // finds Visible points (Clint)
	    // Returns an array of tuples from the outer hull to the inner hull
        Tuple<City, City>[] FindVisiblePoints(City[] citiesOuter, City[] citiesInner)
        {
	        var visible = new List<Tuple<City, City>>();
	        foreach (var city in citiesOuter)
	        {
		        foreach (var city1 in citiesInner)
		        {
			        for (var i = 0; i < citiesInner.Length; i++)
			        {
				        var begin = i > 0 ? i - 1 : citiesInner.Length - 1;
				        var intersects = test_line_intersection(city, city1, citiesInner[begin], citiesInner[i]);
				        if (!intersects)
				        {
					        visible.Add(new Tuple<City, City>(city, city1));
				        }
			        }
		        }
	        }
	        return visible.ToArray();
        }

	    // Returns true if the lines intersect, otherwise false. In addition, if the lines
		// intersect the intersection point may be stored in the floats i_x and i_y.
	    private static bool test_line_intersection(City city0, City city1,
		    City city2, City city3)
	    {
		    var s1_x = city1.X - city0.X;
		    var s1_y = city1.Y - city0.Y;

		    var s2_x = city3.X - city2.X;
		    var s2_y = city3.Y - city2.Y;

		    var s = (-s1_y * (city0.X - city2.X) + s1_x * (city0.Y - city2.Y)) / (-s2_x * s1_y + s1_x * s2_y);
		    var t = ( s2_x * (city0.Y - city2.Y) - s2_y * (city0.X - city2.X)) / (-s2_x * s1_y + s1_x * s2_y);

//			    if (i_x != null)
//				    i_x = city0.X + (t * s1_x);
//			    if (i_y != null)
//				    i_y = city0.Y + (t * s1_y);

		    return s >= 0 && s <= 1 && t >= 0 && t <= 1;
	    }

	    // returns minimum combination of two hulls (Roy)
        City[] CombineHulls(City[] citiesOuter, City[] citiesInner, Tuple<City, City>[] visible)
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
                int[] local_visible = FindVisibleFromOuter(citiesOuter[i_outer], citiesInner, visible);
                
                double min = double.MaxValue;
                foreach(int lv in local_visible) {
                    double next = citiesOuter[i_outer].costToGetTo(citiesInner[lv]);
                    if(next < min)
                    {
                        min = next;
                        i_inner = lv;
                    }
                }
                // if there are no visible points then it moves to the next point in citiesOuter
                if (i_inner == -1)
                {
                    combineHull[i_combine++] = citiesOuter[++i_outer];
                }
            }

            bool on_outer = true;

            while(true)
            {
                const int lookahead = 1;
                // find the best connection locally with lookahead + 1 look aheads.
                // the final '+ 1' local ahead always assumes connection between the hulls
                CombineState initial = new CombineState(citiesOuter, citiesInner, visible,i_inner, i_outer, on_outer, 0);

                double across = goAcross(initial, lookahead);
                double around = goAround(initial, lookahead);
                if(across < around)
                {
                    // go across
                    if(on_outer)
                    {
                        combineHull[i_combine++] = citiesInner[++i_inner];
                    }
                    else
                    {
                        combineHull[i_combine++] = citiesOuter[++i_outer];
                    }
                    on_outer = !on_outer;
                }
                else
                {
                    // go around
                    if (!on_outer)
                    {
                        combineHull[i_combine++] = citiesInner[++i_inner];
                    }
                    else
                    {
                        combineHull[i_combine++] = citiesOuter[++i_outer];
                    }
                }
            }
            
            return combineHull;
        }

        double goAcross(CombineState state, int depth)
        {
            //FIXME think about when the loop is almost done
            //FIXME think about if it is not visible
            if (state.on_outer)
            {
                int[] local_visible = FindVisibleFromOuter(state.citiesOuter[state.i_outer], state.citiesInner, state.visible);
                if (local_visible.Contains(state.i_inner))
                {
                    state.length += state.citiesOuter[state.i_outer].costToGetTo(state.citiesInner[state.i_inner]);
                    state.on_outer = false;
                    
                }
            }
            else
            {
                int[] local_visible = FindVisibleFromOuter(state.citiesInner[state.i_inner], state.citiesOuter, state.visible);
                if (local_visible.Contains(state.i_outer))
                {
                    state.length += state.citiesInner[state.i_inner].costToGetTo(state.citiesOuter[state.i_outer]);
                    state.on_outer = true;
                }
            }
            if (depth < 0)
            {
                return state.length;
            } else if (depth < 1)
            {
                return goAcross(new CombineState(state), depth - 1);
            }
            else
            {
                return Math.Min(
                    goAcross(new CombineState(state), depth - 1),
                    goAround(new CombineState(state), depth - 1));
            }
        }

        double goAround(CombineState state, int depth)
        {
            //FIXME think about when the loop is almost done
            if (state.on_outer)
            {
                state.length += state.citiesOuter[state.i_outer].costToGetTo(state.citiesOuter[ModInc(state.i_outer, state.citiesOuter.Length)]);
            }
            else
            {
                state.length += state.citiesInner[state.i_inner].costToGetTo(state.citiesInner[ModInc(state.i_inner, state.citiesInner.Length)]);
            }

            if (depth < 1)
            {
                return goAcross(new CombineState(state), depth - 1);
            }
            else
            {
                return Math.Min(
                    goAcross(new CombineState(state), depth - 1),
                    goAround(new CombineState(state), depth - 1));
            }
        }

        int ModInc(int i, int n)
        {
            return (i + 1) % n;
        }

        int[] FindVisibleFromOuter(City cityOuter, City[] citiesInner, Tuple<City, City>[] visible)
        {
            List<int> result = new List<int>();
            for(int i = 0; i < visible.Length; i++)
            {
                if(visible[i].Item1 == cityOuter)
                {
                    for(int j = 0; j < citiesInner.Length; j++)
                    {
                        if(visible[i].Item2 == citiesInner[j])
                        {
                            result.Add(j);
                        }
                    }
                }
            }
            return result.ToArray();
        }

        int[] FindVisibleFromInner(City cityInner, City[] citiesOuter, Tuple<City, City>[] visible)
        {
            List<int> result = new List<int>();
            for (int i = 0; i < visible.Length; i++)
            {
                if (visible[i].Item2 == cityInner)
                {
                    for (int j = 0; j < citiesOuter.Length; j++)
                    {
                        if (visible[i].Item1 == citiesOuter[j])
                        {
                            result.Add(j);
                        }
                    }
                }
            }
            return result.ToArray();
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
