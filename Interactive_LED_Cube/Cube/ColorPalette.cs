using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interactive_LED_Cube
{
    public interface ColorPalette
    {
        /* Given a coordinate, defines a function for determining how
         * that coordinate should be colored. */
        RGBColor MapCoordToColor(Coordinate c);
    }
}
