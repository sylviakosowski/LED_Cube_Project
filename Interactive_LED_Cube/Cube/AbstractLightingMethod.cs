using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interactive_LED_Cube
{
    /* 
     * All lighting methods should inherit from this class. 
     * 
     * Provides an implemented CreateAnimation method, which creates
     * an animation for each of the specified LEDs as determined
     * by the colors, rates, and the specific LightingMethod itself
     * (i.e. fading, blinking, etc.).
     * 
     * CreateSingleLEDBehavior is an abstract method which must be
     * implemented by classes which inherit from this class. Determines
     * what behavior this LightingMethod gives to a single LED.
     * 
     * Provides an implemented CreateFrames method, which given
     * the animation represented as coordinate-color list dictionary,
     * creates animation frames for the cube and adds them to the 
     * animation queue.
     * 
     * LightBlockUniform and LightManyBlocksUniform are methods for
     * lighting a single block and many blocks in a uniform way, using
     * a specific color palette and this LightingMethod. They are 
     * abstract and must be implemented by classes inheriting form
     * this class.
     * 
     * TODO: Make LightBlockUniform is basically a subset of
     * LightManyBlocksUniform. We could combine them into a single
     * function given time.
     */
    public abstract class AbstractLightingMethod : LightingMethod
    {
        private HypnocubeImpl hc;
        private int longestAnim;

        public AbstractLightingMethod(HypnocubeImpl hc)
        {
            this.hc = hc;
        }

        public int LongestAnim
        {
            get { return longestAnim; }
            set { longestAnim = value; }
        }

        public int GetLongestAnim()
        {
            return longestAnim;
        }

        public Dictionary<Coordinate, List<RGBColor>> CreateAnimation(
        List<Coordinate> coords, List<RGBColor> endColors, List<int> rates, bool resetFrame)
        {

            List<RGBColor> animation = new List<RGBColor>();
            Dictionary<Coordinate, List<RGBColor>> animDict =
                new Dictionary<Coordinate, List<RGBColor>>();
            longestAnim = 0;

            if(resetFrame)
            {
                hc.SpecificColorWholeCube(new RGBColor(0, 0, 0), false);
            }

            /* Create fading animation for each specific LED in coords, map it
             * to each coordinate in a dictionary.
             */
            for (int i = 0; i < coords.Count; i++)
            {
                animation = CreateSingleLEDBehavior(coords[i], endColors[i], rates[i], i);
                animDict.Add(coords[i], animation);
                longestAnim = Math.Max(longestAnim, animation.Count);
            }

            return animDict;
        }

        public abstract List<RGBColor> CreateSingleLEDBehavior(Coordinate c, RGBColor endColor, int rate, int count);

        public void CreateFrames(List<byte[]> imageFrames,
            Dictionary<Coordinate, List<RGBColor>> animDict, int longestAnim, bool resetFrame)
        {
            int index;
            RGBColor color;
            List<byte[]> imageFramesTest = new List<byte[]>();

            if(resetFrame)
            {
                hc.SpecificColorWholeCube(new RGBColor(0,0,0), false);
            }

            /* For each animation frame, update the behavior of each LED in coords. */
            for (int i = 0; i < longestAnim; i++)
            {
                foreach (KeyValuePair<Coordinate, List<RGBColor>> entry in animDict)
                {
                    /* TODO: Change this to ChangeColorLED */
                    if (i < entry.Value.Count)
                    {
                        index = hc.IndexFromCoord(entry.Key);
                        color = entry.Value[i];
                        hc.ColorArray[index] = color.R;
                        hc.ColorArray[index + 1] = color.G;
                        hc.ColorArray[index + 2] = color.B;
                    }
                }
                hc.AddImageFrame(imageFrames);
            }
        }

        public abstract void LightBlockUniform(List<byte[]> imageFrames, Coordinate c1, Coordinate c2,
            int rate, bool resetFrames, ColorPalette cp);

        public abstract void LightManyBlocksUniform(List<byte[]> imageFrames, List<Coordinate> coords,
            int rate, bool resetFrames, ColorPalette cp);
    }
}
