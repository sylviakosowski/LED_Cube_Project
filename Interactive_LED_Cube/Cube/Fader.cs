using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interactive_LED_Cube
{
    /* 
     * LightingMethod which implements Fading behavior for LEDs. Allows an LED to fade 
     * to a specific color, at a speed indicated by a given rate.
     */
    public class Fader : AbstractLightingMethod
    {
        HypnocubeImpl hc;

        public Fader(HypnocubeImpl hc)
            : base(hc)
        {
            this.hc = hc;
        }

        public override List<RGBColor> CreateSingleLEDBehavior(Coordinate c, RGBColor endColor, int rate, int count)
        {
            return fadeLED(c, endColor, rate);
        }

        /* Fade a single LED from one color to another, using Linear
         * Interpolation.
         * 
         * c is the coordinate of the LED we want to change.
         * endColor is the final color we want the LED to be.
         * Rate represents how fast it should fade from one color to
         * another, and is given as the number of frames the fading
         * should span across.
         * 
         * Returns a list of colors of length rate, which represents the
         * color the LED will be at each of these frames.
         */
        private List<RGBColor> fadeLED(Coordinate c, RGBColor endColor, int rate)
        {
            List<RGBColor> fadeAnimation = new List<RGBColor>();
            //Get old RGB value at c.
            int index = hc.IndexFromCoord(c);
            
            int oldR = hc.ColorArray[index];
            int oldG = hc.ColorArray[index + 1];
            int oldB = hc.ColorArray[index + 2];

            //Calculate difference between new color and old color.
            int rDiff = endColor.R - oldR;
            int gDiff = endColor.G - oldG;
            int bDiff = endColor.B - oldB;


            //Calculate increment for each frame
            double increment = 1.0 / ((double)rate);

            byte newR = 0;
            byte newG = 0;
            byte newB = 0;

            for (int i = 0; i < rate; i++)
            {
                newR = (byte)(oldR + (increment * (i + 1)) * rDiff);
                newG = (byte)(oldG + (increment * (i + 1)) * gDiff);
                newB = (byte)(oldB + (increment * (i + 1)) * bDiff);

                fadeAnimation.Add(new RGBColor(newR, newG, newB));
            }

            return fadeAnimation;
        }

        public override void LightBlockUniform(List<byte[]> imageFrames, Coordinate c1, Coordinate c2,
            int rate, bool resetFrames, ColorPalette cp)
        {
            List<Coordinate> coords = hc.GenerateCoordBlock(c1, c2);
            List<RGBColor> colors = new List<RGBColor>();
            List<int> rates = new List<int>();

            foreach( Coordinate c in coords )
            {
                colors.Add(cp.MapCoordToColor(c));
                rates.Add(rate);
            }

            hc.LightLEDs(imageFrames, coords, colors, rates, this, resetFrames);
        }
    }
}
