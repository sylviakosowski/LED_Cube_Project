using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interactive_LED_Cube
{
    /* Color palette where each coordinate is mapped to
     * a random color. */
    class RandomPalette : ColorPalette
    {
        Random r;

        public RandomPalette(Random r)
        {
            this.r = r;
        }

        public RGBColor MapCoordToColor(Coordinate c)
        {
            byte R;
            byte G;
            byte B;

            //Random r = new Random();

            R = (byte)r.Next(0, 255);
            G = (byte)r.Next(0, 255);
            B = (byte)r.Next(0, 255);

            return new RGBColor((byte)R, (byte)G, (byte)B);
        }
    }
}
