using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interactive_LED_Cube
{
    /* A color palette where each LED in the cube
     * is mapped to the same color.
     */
    class SolidPalette : ColorPalette
    {
        private RGBColor color;

        public SolidPalette(RGBColor color)
        {
            this.color = color;
        }

        public RGBColor MapCoordToColor(Coordinate c)
        {
            return color;
        }
    }
}
