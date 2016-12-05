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
            City[] combineHull = new City[citiesOuter.Length + citiesInner.Length];

            //indecise the inner/outer/combine hull
            int i_outer = 0;
            int i_inner = -1;
            int i_combine = 0;

            combineHull[i_combine++] = citiesOuter[i_outer];

            while(i_inner == -1)
            {
                // find the closest visable point from citiesOuter[0]
                int[] local_visable = FindVisableFromOuter(citiesOuter[i_outer], citiesInner, visable);
                
                double min = double.MaxValue;
                foreach(int lv in local_visable) {
                    double next = citiesOuter[i_outer].costToGetTo(citiesInner[lv]);
                    if(next < min)
                    {
                        min = next;
                        i_inner = lv;
                    }
                }
                // if there are no visable points then it moves to the next point in citiesOuter
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
                CombineState initial = new CombineState(i_inner, i_outer, on_outer, 0);

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
            //FIXME think about if it is not visable
            if (state.on_outer)
            {
                int[] local_visable = FindVisableFromOuter(state.citiesOuter[state.i_outer], state.citiesInner, state.visable);
                if (local_visable.Contains(state.i_inner))
                {
                    state.length += state.citiesOuter[state.i_outer].costToGetTo(state.citiesInner[state.i_inner]);
                    state.on_outer = false;
                    
                }
            }
            else
            {
                int[] local_visable = FindVisableFromOuter(state.citiesInner[state.i_inner], state.citiesOuter, state.visable);
                if (local_visable.Contains(state.i_outer))
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
                return goAcross(new CombineState(state), depth - 1),
            }
            else
            {
                return Math.Min(
                    goAcross(new CombineState(state), depth - 1),
                    goArround(new CombineState(state), depth - 1));
            }
        }

        double goArround(CombineState state, int depth)
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
                    goArround(new CombineState(state), depth - 1));
            }
        }

        int ModInc(int i, int n)
        {
            return (i + 1) % n;
        }

        int[] FindVisableFromOuter(City cityOuter, City[] citiesInner, Tuple<City, City>[] visable)
        {
            List<int> result = new List<int>();
            for(int i = 0; i < visable.Length; i++)
            {
                if(visable[i].Item1 == cityOuter)
                {
                    for(int j = 0; j < citiesInner.Length; j++)
                    {
                        if(visable[i].Item2 == citiesInner[j])
                        {
                            result.Add(j);
                        }
                    }
                }
            }
            return result.ToArray();
        }

        int[] FindVisableFromInner(City cityInner, City[] citiesOuter, Tuple<City, City>[] visable)
        {
            List<int> result = new List<int>();
            for (int i = 0; i < visable.Length; i++)
            {
                if (visable[i].Item2 == cityInner)
                {
                    for (int j = 0; j < citiesOuter.Length; j++)
                    {
                        if (visable[i].Item1 == citiesOuter[j])
                        {
                            result.Add(j);
                        }
                    }
                }
            }
            return result.ToArray();
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
                        FindVisablePoints(hulls[0], hulls[i])
                        );
            }

            return hulls[0];
        }
    }
}
