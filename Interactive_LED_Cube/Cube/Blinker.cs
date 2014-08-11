using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interactive_LED_Cube
{
    /* 
     * LightingMethod which implements Blinking behavior for LEDs. Allows an LED to blink 
     * in a specific color, at a speed indicated by a given rate. Along with HypnocubeImpl,
     * an int list value numBlinks is passed in which helps determine blink speed and indicates
     * the number of times the LEDs should blink.
     */
    class Blinker : AbstractLightingMethod
    {
        HypnocubeImpl hc;
        List<int> numBlinks;

        public Blinker(HypnocubeImpl hc, List<int> numBlinks) : base(hc)
        {
            this.hc = hc;
            this.numBlinks = numBlinks;
        }

        public override List<RGBColor> CreateSingleLEDBehavior(Coordinate c, RGBColor endColor, int rate, int count)
        {
            return BlinkLED(c, endColor, rate, numBlinks[count]);
        }

        /* Blink a specific LED at Coordinate c, with speed of blink
         * determined by rate. (rate = the number of frames the LED
         * should be lighted up in the blink)
         */
        public List<RGBColor> BlinkLED(Coordinate c, RGBColor color, int rate, int numBlinks)
        {
            List<RGBColor> colors = new List<RGBColor>();
            RGBColor black = new RGBColor(0, 0, 0);

            for (int j = 0; j < numBlinks; j++)
            {
                for (int i = 0; i < rate * 2; i++)
                {
                    if (i < rate)
                    {
                        colors.Add(color);
                    }
                    else
                    {
                        colors.Add(black);
                    }
                }
            }

            return colors;
        }

        public override void LightBlockUniform(List<byte[]> imageFrames, Coordinate c1, Coordinate c2, 
            int rate, bool resetFrames, ColorPalette cp)
        {
            BlinkBlockUniform(imageFrames, c1, c2, rate, 10, resetFrames, cp);
        }

        public void BlinkBlockUniform(List<byte[]> imageFrames, Coordinate c1, Coordinate c2,
            int rate, int num, bool resetFrames, ColorPalette cp)
        {
            List<Coordinate> coords = hc.GenerateCoordBlock(c1, c2);
            List<RGBColor> colors = new List<RGBColor>();
            List<int> rates = new List<int>();
            numBlinks = new List<int>();

            foreach(Coordinate c in coords)
            {
                colors.Add(cp.MapCoordToColor(c));
                rates.Add(rate);
                numBlinks.Add(num);
            }

            hc.LightLEDs(imageFrames, coords, colors, rates, this, resetFrames);
        }

        public override void LightManyBlocksUniform(List<byte[]> imageFrames, List<Coordinate> coords, 
            int rate, bool resetFrames, ColorPalette cp)
        {

        }
    }
}
