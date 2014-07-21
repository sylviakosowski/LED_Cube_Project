using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using CubeVisualization;

namespace Autodesk_Interactive_Storytelling
{
    /* Provides utilities for working with Twitter Keywords and statuses. */
    public class KeywordUtil
    {
        TweetListener tl;

        public KeywordUtil()
        {
        }

        public KeywordUtil(TweetListener tl)
        {
            this.tl = tl;
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
            ArrayList keywordsPresent, int mode)
        {
            List<byte[]> imageFrames = new List<byte[]>();
            Hypnocube hc = new Hypnocube();

            foreach( string element in keywordsPresent )
            {
                Console.WriteLine("Keyword: " + element);
                int value;
                if (keywordDict.TryGetValue(element, out value))
                {
                    Console.WriteLine("Value is: " + value.ToString());
                    //TODO CHANGE MAKE MORE GENERIC

                    changeToRandColor(hc);

                    imageFrames.Add(hc.ColorArray);
                    Console.WriteLine(hc.ColorArray[0].ToString());
                }
                else
                {
                    Console.WriteLine("Value not in dictionary.");
                }
            }

            if(mode == 0)
            {
                tl.ReceiveSignal(imageFrames);
            }
        }

        private void changeToRandColor(Hypnocube hc) 
        {
            Random rand = new Random();
            byte randR = (byte)rand.Next(0, 255);
            byte randG = (byte)rand.Next(0, 255);
            byte randB = (byte)rand.Next(0, 255);


            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    for (int k = 0; k < 8; k++)
                    {
                        hc.changeColorLED(i, j, k,
                            randR, randG, randB);
                    }
                }
            }
        }

    }
}
