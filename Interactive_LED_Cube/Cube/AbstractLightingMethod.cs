using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Autodesk_Interactive_Storytelling
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
        List<Coordinate> coords, List<RGBColor> endColors, List<int> rates)
        {

            List<RGBColor> animation = new List<RGBColor>();
            Dictionary<Coordinate, List<RGBColor>> animDict =
                new Dictionary<Coordinate, List<RGBColor>>();
            longestAnim = 0;

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
            Dictionary<Coordinate, List<RGBColor>> animDict, int longestAnim)
        {
            int index;
            RGBColor color;

            /* For each animation frame, update the behavior of each LED in coords. */
            for (int i = 0; i < longestAnim; i++)
            {
                foreach (KeyValuePair<Coordinate, List<RGBColor>> entry in animDict)
                {
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
    }
}
