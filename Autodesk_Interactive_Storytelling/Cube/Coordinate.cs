﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Autodesk_Interactive_Storytelling
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
        }

        public int Y
        {
            get { return y; }
        }

        public int Z
        {
            get { return z; }
        }
    }
}
