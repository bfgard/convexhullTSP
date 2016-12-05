using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TSP
{
    class ConvexHullTSP
    {
        // find a convex hull give an array of cities (Ben)
        City[] FindConvexHull(City[] cities)
        {
            // FIXME implement
            return null;
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
            // FIXME implement
            return null;
        }

        City[] run(City[] cities)
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
