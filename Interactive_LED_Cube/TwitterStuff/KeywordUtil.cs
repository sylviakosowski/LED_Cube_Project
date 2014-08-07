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
        private TweetListener tl;
        private PIC32 port;
        private int mode;

        public KeywordUtil(PIC32 port)
        {
            this.port = port;
            mode = 1;
        }

        /* Constructor to be used when in OpenGL visualization mode. */
        public KeywordUtil(TweetListener tl)
        {
            this.tl = tl;
            mode = 0;
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

        public void RunAnimationBasedOnCommands(List<string> commandsPresent, string[] patterns,
            string[] colors)
        {
            Tuple<bool, string, string> parsedCommand = ParseCommand(commandsPresent, patterns, colors);
            if(parsedCommand.Item1)
            {
                switch(parsedCommand.Item2)
                {
                    case "fade":
                        {

                            break;
                        }
                    case "blink":
                        {
                            break;
                        }
                    case "random":
                        {
                            break;
                        }
                    case "zigzag":
                        {
                            break;
                        }
                    case "box":
                        {
                            break;
                        }
                    case "roamer":
                        {
                            break;
                        }
                    default:
                        {
                            Console.WriteLine("Invalid command. ");
                            break;
                        }
                }
            }
            else
            {
                Console.WriteLine("Command cube was not found.");
            }
        }

    }
}
