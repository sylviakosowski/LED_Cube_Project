using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interactive_LED_Cube.Cube
{
    /* Fills the LED with a single color, no fancy animation. Rate
     * deterimnes the number of frames that the LED will be filled
     * as this color.
     */
    class ColorFiller : AbstractLightingMethod
    {
        HypnocubeImpl hc;

        public ColorFiller(HypnocubeImpl hc)
            : base(hc)
        {
            this.hc = hc;
        }

        public override List<RGBColor> CreateSingleLEDBehavior(Coordinate c, RGBColor color, int rate, int count)
        {
            return fillLED(c, color, rate);
        }

        /* Add rate number of frames where LED at coord c is color. */
        private List<RGBColor> fillLED(Coordinate c, RGBColor color, int rate)
        {
            List<RGBColor> fillAnimation = new List<RGBColor>();

            for (int i = 0; i < rate; i++)
            {
                fillAnimation.Add(color);
            }
            return fillAnimation;
        }

        public void UniformColorRate(int numCoords, RGBColor c, int rate, List<RGBColor> colors,
            List<int> rates)
        {
            for(int i = 0; i < numCoords; i++)
            {
                colors.Add(c);
                rates.Add(rate);
            }
        }
    }
}
