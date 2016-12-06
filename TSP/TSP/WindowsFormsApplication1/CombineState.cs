using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TSP
{
    class CombineState
    {
        //get next combine state
        public CombineState(CombineState state, double addedLength, bool go_across)
        {
            this.parent = state;
            this.citiesOuter = state.citiesOuter;
            this.citiesInner = state.citiesInner;
            this.visible = state.visible;
            this.i_inner = state.i_inner;
            this.i_inner_start = state.i_inner_start;
            this.i_outer = state.i_outer;
            this.i_outer_start = state.i_outer_start;
            this.on_outer = state.on_outer;
            this.movedInner = state.movedInner;
            this.movedOuter = state.movedOuter;
            
            if(on_outer)
            {
                this.i_outer = ModInc(this.i_outer, citiesOuter.Length);
                this.movedOuter = true;
            } else
            {
                this.i_inner = ModInc(this.i_inner, this.citiesInner.Length);
                this.movedInner = true;
            }
            if(go_across)
            {
                this.on_outer = !this.on_outer;
            }
            if (this.movedInner && this.i_inner == this.i_inner_start &&
                this.movedOuter && this.i_outer == this.i_outer_start)
            {
                this.end = true;
            }
            this.length = state.length + addedLength;
        }

        public CombineState(City[] citiesOuter, City[] citiesInner, Tuple<List<int>[], List<int>[]> visible, int i_inner, int i_outer, bool on_outer, double length)
        {
            this.movedOuter = i_outer != 0;
            this.movedInner = false;
            this.citiesOuter = citiesOuter;
            this.citiesInner = citiesInner;
            this.visible = visible;
            this.i_inner = i_inner;
            this.i_inner_start = i_inner;
            this.i_outer = i_outer;
            this.i_outer_start = i_outer;
            this.on_outer = on_outer;
            this.length = length;
        }

        private bool movedOuter;
        private bool movedInner;

        public City[] citiesOuter;
        public City[] citiesInner;
        public Tuple<List<int>[], List<int>[]> visible;

        public int i_inner, i_inner_start;
        public int i_outer, i_outer_start;

        public bool on_outer;
        public double length;

        public CombineState parent = null;
        private CombineState across = null;
        private CombineState around = null;

        public bool end = false;

        public City _City
        {
            get
            {
                if(on_outer)
                {
                    return citiesOuter[i_outer];
                }
                else
                {
                    return citiesInner[i_inner];
                }
            }
        }

        public CombineState Across
        {
            get
            {
                if(across == null)
                {
                    across = goAcross();
                }
                return across;
            }
        }

        public CombineState Around
        {
            get
            {
                if(around == null)
                {
                    around = goAround();
                }
                return around;
            }
        }

        CombineState goAcross()
        {
            if (on_outer)
            {
                if (movedInner && i_inner == i_inner_start)
                {
                    return null;
                }
                int[] local_visible = visible.Item1[i_outer].ToArray();
                if (local_visible.Contains(i_inner))
                {
                    return new CombineState(this, citiesOuter[i_outer].costToGetTo(citiesInner[i_inner]), true);
                }
            }
            else
            {
                if(citiesInner.Length == 1)
                {
                    return null;
                }
                /*if (movedOuter && i_outer == i_outer_start)
                {
                    if (i_inner == ModDec(i_inner_start, citiesInner.Length))
                    {
                        CombineState final = new CombineState(this, citiesInner[i_inner].costToGetTo(citiesOuter[i_outer]), true);
                        final.end = true;
                        return final;
                    }
                }*/
                int[] local_visible = visible.Item2[i_inner].ToArray();
                if (local_visible.Contains(i_outer))
                {
                    return new CombineState(this, citiesInner[i_inner].costToGetTo(citiesOuter[i_outer]), true);
                }
            }
            return null;
        }

        CombineState goAround()
        {
            if (on_outer)
            {
                /*if (movedOuter && i_outer == ModDec(i_outer_start,citiesOuter.Length))
                {
                    if (i_inner == i_inner_start)
                    {
                        CombineState final =  new CombineState(this, citiesOuter[i_outer].costToGetTo(citiesOuter[ModInc(i_outer, citiesOuter.Length)]), false);
                        final.end = false;
                        return final;
                    }
                }*/
                return new CombineState(this, citiesOuter[i_outer].costToGetTo(citiesOuter[ModInc(i_outer, citiesOuter.Length)]), false);
            }
            else
            {
                if (movedInner && i_inner == ModDec(i_inner_start,citiesInner.Length))
                {
                    return null;
                }
                return new CombineState(this, citiesInner[i_inner].costToGetTo(citiesInner[ModInc(i_inner, citiesInner.Length)]), false);
            }
        }

        int ModInc(int i, int n)
        {
            return (i + 1) % n;
        }

        int ModDec(int i, int n)
        {
            if (i == 0)
                return n - 1;
            else
                return i - 1;
        }
    }
}
