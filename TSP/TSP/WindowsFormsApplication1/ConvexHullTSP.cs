using System;
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

        // finds Visable points (Clint)
        Tuple<City, City>[] FindVisablePoints(City[] citiesOuter, City[] citiesInner)
        {
            // FIXME implemnt 
            return null;
        }

        // returns minimum combination of two hulls (Roy)
        City[] CombineHulls(City[] citiesOuter, City[] citiesInner, Tuple<City, City>[] visable)
        {
            // FIXME implement
            return null;
        }

        City[] run(City[] cities)
        {
            // FIXME implement
            return null;
        }

    }
}
