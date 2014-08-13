using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using Cube_Visualization;

namespace Interactive_LED_Cube
{
    /* Provides utilities for working with Twitter keywords and statuses, and the
     * animations that correspond to them in the cube.
     */
    public class KeywordUtil
    {
        //private TweetListener tl;
        //private PIC32 port;
        //private int mode;
        //private HypnocubeImpl hc;
        private Animations anim;
        private bool physical;

        /*
        public KeywordUtil(PIC32 port, HypnocubeImpl hc, Animations anim)
        {
            this.port = port;
            mode = 1;
            this.hc = hc;
            this.anim = anim;
        }
        */
        /* Constructor to be used when in OpenGL visualization mode. */
        /*
        public KeywordUtil(TweetListener tl, HypnocubeImpl hc, Animations anim)
        {
            this.tl = tl;
            mode = 0;
            this.hc = hc;
            this.anim = anim;
        }
         * */

        public KeywordUtil(Animations anim, bool physical)
        {
            this.anim = anim;
            this.physical = physical;
        }

        /* 
         * Determines which Twitter keywords are in the Tweet's status string, and returns
         * an list of those keywords.
         */
        public List<string> determineKeywordsFromString(string[] keywords, string s)
        {
            string status = s.ToLower();
            List<string> keywordsPresent = new List<String>();
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
            List<string> keywordsPresent)
        {
            List<byte[]> imageFrames = new List<byte[]>();
            //HypnocubeImpl hc = new HypnocubeImpl();

            foreach( string element in keywordsPresent )
            {
                Console.WriteLine("Keyword: " + element);
                int value;
                if (keywordDict.TryGetValue(element, out value))
                {
                    Console.WriteLine("Value is: " + value.ToString());

                    //Add cube behavior here

                    switch(value)
                    {
                        case 0:
                            {
                                if(keywordsPresent.Count == 1)
                                {
                                    anim.SimpleLight(new AutodeskPalette());
                                }
                                break;
                            }
                        case 1:
                            {
                                anim.SimpleLight(new RainbowPalette());
                                break;
                            }
                        default:
                            break;
                    }
                }
                else
                {
                    Console.WriteLine("Value not in dictionary.");
                }
            }

            if(!physical)
            {
                //tl.ReceiveAndSendSignal(imageFrames);
            }
        }

        /* Given a list of commands which were present in the Tweet, returns a tuple where the bool value indicates
         * if the command "cube" was found in the tweet, and two string values representing the desired pattern
         * and color.
         */
        public Tuple<bool, string,string> ParseCommand(List<string> commandsPresent, string[] patterns, string[] colors)
        {
            bool cubeCommandFound = false;
            string color = null;
            string pattern = null;

            foreach(string command in commandsPresent)
            {
                foreach(string c in patterns)
                {
                    if(command.Equals(c))
                    {
                        pattern = command;
                    }
                    if(command.Equals("cube"))
                    {
                        cubeCommandFound = true;
                    }
                }

                foreach(string c in colors)
                {
                    if(command.Equals(c))
                    {
                        color = command;
                    }
                }
            }

            return new Tuple<bool, string, string>(cubeCommandFound, pattern, color);
            
        }

        public void RunAnimationBasedOnCommands(string pattern, string colorString)
        {
            List<byte[]> imageFrames = new List<byte[]>();
            ColorDict cd = new ColorDict();
            ColorPalette cp;
            bool colorInDict = cd.Dict.TryGetValue(colorString, out cp);

            switch (pattern)
            {
                case "fade":
                    {
                        Console.WriteLine("fade");
                        anim.Fade(cp);
                        break;
                    }
                case "blink":
                    {
                        anim.Blink(cp);
                        break;
                    }
                case "cluster":
                    {
                        anim.RandomFill(cp);
                        break;
                    }
                case "zigzag":
                    {
                        anim.ZigZagFill(cp);
                        break;
                    }
                case "box":
                    {
                        anim.ExpandingCube(cp);
                        break;
                    }
                case "fireflies":
                    {
                        anim.Fireflies(cp);
                        break;
                    }
                case "blocks":
                    {
                        anim.Blocks(cp);
                        break;
                    }
                case "trail":
                    {
                        anim.Trail(cp);
                        break;
                    }
                default:
                    {
                        Console.WriteLine("Invalid command. ");
                        break;
                    }
            }
        }

    }
}
