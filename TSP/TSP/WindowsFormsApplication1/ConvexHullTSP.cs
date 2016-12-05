﻿using System;
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
        Tuple<City, City>[] FindVisiblePoints(City[] citiesOuter, City[] citiesInner)
        {

	        foreach (var city in citiesOuter)
	        {
		        foreach (var city1 in citiesInner)
		        {
			        var dx = city1.X - city.X;
			        var dy = city1.Y = city.Y;
		        }
	        }
	        return null;
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
