using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TSP
{
    class Branch
    {
        static private int next_id = 0;
        public int id;
        public double[,] costs;
        public double bestCost;

        public City[] path;
        public int[] path_index;
        public int visitedCount;

        public override bool Equals(object obj)
        {
            Branch b = obj as Branch;

            return b.id == id;
        }

        public override int GetHashCode()
        {
            return id;
        }

        private Branch(Branch branch, City city, int city_index)
        {
            costs = new double[branch.costs.GetLength(0), branch.costs.GetLength(1)];
            Array.Copy(branch.costs, costs, costs.Length);

            path = new City[branch.path.Length];
            Array.Copy(branch.path, path, path.Length);

            path_index = new int[branch.path.Length];
            Array.Copy(branch.path_index, path_index, path_index.Length);

            visitedCount = branch.visitedCount;
            path[visitedCount] = city;
            path_index[visitedCount++] = city_index;

            bestCost = branch.bestCost;
            id = next_id++;
        }

        public Branch(City[] cities)
        {
            costs = new double[cities.Length, cities.Length];

            for(int r = 0; r < cities.Length; r++)
            {
                for(int c = 0; c < cities.Length; c++)
                {
                    if(r == c)
                    {
                        costs[r, c] = double.PositiveInfinity;
                        continue;
                    }
                    costs[r, c] = cities[r].costToGetTo(cities[c]);
                }
            }
            reduceCosts();

            path = new City[cities.Length];
            path[0] = cities[0];

            path_index = new int[cities.Length];
            path_index[0] = 0;

            visitedCount = 1;

            id = next_id++;
        }

        public Branch choose(City[] cities)
        {
            int prev = path_index[visitedCount - 1];
            double minCost = double.PositiveInfinity;
            int next = -1;
            for(int i = 1; i < costs.GetLength(1); i++)
            {
               if(minCost > costs[prev, i])
                {
                    minCost = costs[prev, i];
                    next = i;
                }
            }
            return next != -1 ? choose(cities, prev, next) : null;
        }

        public Branch choose(City[] cities, int prev, int next)
        {
            Branch branch = new Branch(this, cities[next], next);
            branch.bestCost += costs[prev, next];

            costs[prev, next] = double.PositiveInfinity;

            if (visitedCount > 0)
            {
                for (int i = 0; i < cities.Length; i++)
                {
                    branch.costs[prev, i] = double.PositiveInfinity;
                    branch.costs[i, next] = double.PositiveInfinity;
                }
            }
            branch.reduceCosts();

            return branch;
        }

        private void reduceCosts()
        {
            // reduce rows
            for(int r = 0; r < costs.GetLength(0); r++)
            {
                bool hasZero = false;
                bool hasNumber = false;
                double min = double.PositiveInfinity;
                for(int c = 0; c < costs.GetLength(1); c++)
                {
                    if (costs[r, c] == 0)
                    {
                        hasZero = true;
                        break;
                    }
                    if(!hasNumber && !double.IsInfinity(costs[r,c]))
                    {
                        hasNumber = true;
                    }
                    if (min > costs[r, c])
                    {
                        min = costs[r, c];
                    }
                }
                if (hasZero || !hasNumber) continue;

                for(int c = 0; c < costs.GetLength(1); c++)
                {
                    if (!double.IsInfinity(costs[r, c]))
                    {
                        costs[r, c] -= min;
                    }
                }
                bestCost += min;
            }

            // reduce columns
            for (int c = 0; c < costs.GetLength(0); c++)
            {
                bool hasZero = false;

                bool hasNumber = false;
                double min = double.PositiveInfinity;
                for (int r = 0; r < costs.GetLength(1); r++)
                {
                    if (costs[r, c] == 0)
                    {
                        hasZero = true;
                        break;
                    }
                    if (!hasNumber && !double.IsInfinity(costs[r, c]))
                    {
                        hasNumber = true;
                    }
                    if (min > costs[r, c])
                    {
                        min = costs[r, c];
                    }
                }
                if (hasZero || !hasNumber) continue;

                for (int r = 0; r < costs.GetLength(1); r++)
                {
                    if (!double.IsInfinity(costs[r, c]))
                    {
                        costs[r, c] -= min;
                    }
                }
                bestCost += min;
            }
        }
        public bool isIrrelavent(double bssf)
        {
            return bestCost >= bssf;
        }
        public bool isFinished()
        {
            if(visitedCount == path.Length)
            {
                bestCost += costs[path_index[path_index.Length - 1],0];
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
