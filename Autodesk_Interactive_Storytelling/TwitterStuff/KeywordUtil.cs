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
                    Random rand = new Random();

                    Console.WriteLine("printing out rand colors generated for each image");
                    for (int i = 0; i < 100; i++ )
                    {
                        //PROBLEM HERE: IT IS NOT GENERATING A RANDOM THING FOR EACH FRAME?
                        changeToRandColor(hc, rand);

                        byte[] newImage = new byte[hc.ColorArray.Length];
                        Array.Copy(hc.ColorArray, newImage, hc.ColorArray.Length);

                        Console.WriteLine("Image frame value this loop:" + newImage[0].ToString());

                        imageFrames.Add(newImage);
                        Console.WriteLine("Color array value this loop:" + hc.ColorArray[0].ToString());
                    }

                    Console.WriteLine("Printing out imageframe values");
                    foreach ( byte[] image in imageFrames)
                    {
                        Console.WriteLine(image[0].ToString());
                    }
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

        /* Simple method which changes the whole cube to a random color. */
        private void changeToRandColor(HypnocubeImpl hc, Random rand) 
        {
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
