using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interactive_LED_Cube
{
    /* Represents an RGB color. */
    public class RGBColor
    {
        private byte red;
        private byte green;
        private byte blue;

        public RGBColor(byte r, byte g, byte b)
        {
            red = r;
            green = g;
            blue = b;
        }

        public byte R
        {
            get { return red; }
        }

        public byte G
        {
            get { return green; }
        }

        public byte B
        {
            get { return blue; }
        }
    }
}
