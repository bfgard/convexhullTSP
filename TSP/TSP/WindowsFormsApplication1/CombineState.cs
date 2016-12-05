using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TSP
{
    class CombineState
    {
        public CombineState(CombineState state)
        {
            this.citiesOuter = state.citiesOuter;
            this.citiesInner = state.citiesInner;
            this.visable = state.visable;
            this.i_inner = state.i_inner;
            this.i_outer = state.i_outer;
            this.on_outer = state.on_outer;
            this.length = state.length;
        }
        public CombineState(City[] citiesOuter, City[] citiesInner, Tuple<City, City>[] visable, int i_inner, int i_outer, bool on_outer, double length)
        {
            this.citiesOuter = citiesOuter;
            this.citiesInner = citiesInner;
            this.visable = visable;
            this.i_inner = i_inner;
            this.i_outer = i_outer;
            this.on_outer = on_outer;
            this.length = length;
        }

        public City[] citiesOuter;
        public City[] citiesInner;
        public Tuple<City, City>[] visable;
        public int i_inner;
        public int i_outer;
        public bool on_outer;
        public double length;
    }
}
