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
        private Animations anim;
        private bool physical;

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
         * This method will be called if the command "cube" is NOT found, i.e. this is
         * what will be called when the cube is passively listening to keywords, i.e.
         * #Autodesk with no extra cube information in it.
         * 
         * The purpose behind KeywordDict rather than hardcoding the keyword strings into
         * this is so that you can easily change what keywords you want to listen for
         * in Twitter.
         */
        public void RunAnimationBasedOnKeywords(Dictionary<string,int> keywordDict, 
            List<string> keywordsPresent)
        {
            List<byte[]> imageFrames = new List<byte[]>();

            foreach( string element in keywordsPresent )
            {
                Console.WriteLine("Keyword: " + element);
                int value;
                if (keywordDict.TryGetValue(element, out value))
                {
                    Console.WriteLine("Value is: " + value.ToString());

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
                            {
                                anim.SimpleLight(new AutodeskPalette());
                                break;
                            }
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

        /* Run an animation based on the given pattern and color commands. If you 
         * create new animations, they must be added to this switch statement. */
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
                        Console.WriteLine("HI IT WORKED");
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
