using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interactive_LED_Cube
{
    /* Represents the x, y, and z position of a LED in the Hypnocube. */
    public class Coordinate
    {
        private int x;
        private int y;
        private int z;

        public Coordinate(int x, int y, int z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public int X
        {
            get { return x; }
            set { x = value; }
        }

        public int Y
        {
            get { return y; }
            set { y = value; }
        }

        public int Z
        {
            get { return z; }
            set { z = value; }
        }

        public int IncDec(int component, int changer)
        {
            int newComp = component + changer;

            if(newComp < 0 || newComp > 7)
            {
                return component;
            }
            else
            {
                return newComp;
            }
        }

        public override string ToString()
        {
            return "(" + x.ToString() + ", " + y.ToString() + ", " + z.ToString() + ")";
        }
    }
}
