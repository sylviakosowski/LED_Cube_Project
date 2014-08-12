using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interactive_LED_Cube
{
    public class AutodeskPalette : ColorPalette
    {
        public RGBColor MapCoordToColor(Coordinate c)
        {
            //int R = 256 - 32 * (c.X + 1);
            //int G = 256 - 32 * (c.Y + 1);
            //int B = 256 - 32 * (c.Z + 1);

            int R = 6 + 16 * (c.X + 1);
            int G = 150 + 4 * (c.Y + 1);
            int B = 215 - 19 * (c.Z + 1);

            return new RGBColor((byte)R, (byte)G, (byte)B);
        }
    }
}
