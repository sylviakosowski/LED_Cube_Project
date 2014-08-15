using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interactive_LED_Cube
{
    /* LightingMethod which fills the LED with a single color, no fancy animation. Rate
     * deterimnes the number of frames that the LED will be filled
     * as this color.
     */
    public class ColorFiller : AbstractLightingMethod
    {
        HypnocubeImpl hc;
        private Coordinate c1;
        private Coordinate c2;

        public ColorFiller(HypnocubeImpl hc)
            : base(hc)
        {
            this.hc = hc;
            c1 = null;
            c2 = null;
        }

        public Coordinate C1
        {
            get { return c1; }
            set { c1 = value; }
        }

        public Coordinate C2
        {
            get { return c2; }
            set { c2 = value; }
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

        public void UniformColorRate(int numCoords, int rate, List<RGBColor> colors,
            List<int> rates, ColorPalette cp)
        {
            for(int i = 0; i < numCoords; i++)
            {
                //colors.Add(cp.MapCoordToColor(c));
                rates.Add(rate);
            }
        }

        /* Light an entire block with the same color and rate. */
        public override void LightBlockUniform(List<byte[]> imageFrames, Coordinate c1, Coordinate c2, 
            int rate, bool resetFrames, ColorPalette cp)
        {
            List<Coordinate> coords = hc.GenerateCoordBlock(c1, c2);

            List<RGBColor> colors = new List<RGBColor>();
            List<int> rates = new List<int>();

            //UniformColorRate(coords.Count, rate, colors, rates, cp);
            foreach(Coordinate c in coords)
            {
                rates.Add(rate);
                colors.Add(cp.MapCoordToColor(c));
            }

            hc.LightLEDs(imageFrames, coords, colors, rates, this, resetFrames);
        }

        public override void LightManyBlocksUniform(List<byte[]> imageFrames, List<Coordinate> coords, 
            int rate, bool resetFrames, ColorPalette cp)
        {
            List<int> rates = new List<int>();
            List<RGBColor> colors = new List<RGBColor>();

            foreach(Coordinate c in coords)
            {
                rates.Add(rate);
                colors.Add(cp.MapCoordToColor(c));
            }

            hc.LightLEDs(imageFrames, coords, colors, rates, this, resetFrames);
        }
    }
}
