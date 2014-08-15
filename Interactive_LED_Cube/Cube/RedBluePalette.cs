using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interactive_LED_Cube
{
    /* Color palette which fades from Red to Blue. 
     * Also known as "Twilight" in the command section 
     * in the twitter code.
     */
    public class RedBluePalette : ColorPalette
    {
        public RGBColor MapCoordToColor(Coordinate c)
        {
            int R = 256 - 32 * (c.X + 1);
            int G = 0;
            int B = 256 - 32 * (c.Z + 1);

            return new RGBColor((byte)R, (byte)G, (byte)B);
        }
    }
}
