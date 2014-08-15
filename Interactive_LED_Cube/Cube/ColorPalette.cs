using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interactive_LED_Cube
{
    /* Determines the colors in which an animation will be run. 
     * Colors are defined over the coordinates of the cube. 
     * (What coordinate the LED is in the cube will determine 
     * what its color is in the palette).
     */
    public interface ColorPalette
    {
        /* Given a coordinate, defines a function for determining how
         * that coordinate should be colored. */
        RGBColor MapCoordToColor(Coordinate c);
    }
}
