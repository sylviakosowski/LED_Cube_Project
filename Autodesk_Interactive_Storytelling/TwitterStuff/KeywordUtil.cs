using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using Cube_Visualization;

namespace Autodesk_Interactive_Storytelling
{
    /* Provides utilities for working with Twitter keywords and statuses, and the
     * animations that correspond to them in the cube.
     */
    public class KeywordUtil
    {
        private TweetListener tl;
        private int mode;

        public KeywordUtil()
        {
        }

        /* Constructor to be used when in OpenGL visualization mode. */
        public KeywordUtil(TweetListener tl, int mode)
        {
            this.tl = tl;
            this.mode = mode;
        }

        /* 
         * Determines which Twitter keywords are in the Tweet's status string, and returns
         * an ArrayList of those keywords. 
         */
        public ArrayList determineKeywordsFromString(string[] keywords, string s)
        {
            string status = s.ToLower();
            ArrayList keywordsPresent = new ArrayList();
            foreach(string element in keywords)
            {
                if(status.Contains(element))
                {
                    keywordsPresent.Add(element);
                }
            }

            return keywordsPresent;
        }

        /* 
         * For each keyword in the ArrayList keywordsPresent, perform an action which is
         * specific to that keyword (as detailed by the dictionary int value corresponding
         * to that keyword.
         * 
         * TODO: Handling values which are greater than the number of animations available will
         * be done in some sort of animation handler, not here.
         */
        public void RunAnimationBasedOnKeywords(Dictionary<string,int> keywordDict, 
            ArrayList keywordsPresent)
        {
            List<byte[]> imageFrames = new List<byte[]>();
            HypnocubeImpl hc = new HypnocubeImpl();

            foreach( string element in keywordsPresent )
            {
                Console.WriteLine("Keyword: " + element);
                int value;
                if (keywordDict.TryGetValue(element, out value))
                {
                    Console.WriteLine("Value is: " + value.ToString());

                    //TODO CHANGE MAKE MORE GENERIC

                    //hc.RandomFullCubeColorChange(10, imageFrames);
                    //hc.BlinkLED(imageFrames, 0, 0, 7);
                    //Coordinate c = new Coordinate(7,0,0);
                    //hc.LightHorizontalStrip(imageFrames, c,4,255,0,0);
                    Coordinate c = new Coordinate(7,0,0);
                    RGBColor col = new RGBColor(255,0,0);
                    //hc.LightHorizontalStrip(imageFrames, c, 7, col);
                    //hc.LightIntersection(imageFrames);
                    hc.LightCrossSection(imageFrames, col, c, HypnocubeImpl.Direction.X, false);
                    //hc.LightCrossSectionTest(imageFrames, col, c, HypnocubeImpl.Direction.Y, false);
                    //hc.LightCrossSectionTest(imageFrames, col, c, HypnocubeImpl.Direction.Z, false);
                    hc.ShiftOnce(imageFrames, HypnocubeImpl.Direction.X, true);
                }
                else
                {
                    Console.WriteLine("Value not in dictionary.");
                }
            }

            if(mode == 0)
            {
                tl.ReceiveAndSendSignal(imageFrames);
            }
        }

    }
}
