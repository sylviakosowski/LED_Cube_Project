using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LinqToTwitter;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections;

namespace Interactive_LED_Cube
{
    /*
     * A class which streams content from Twitter depending on keywords
     * which are specified in the items passed into the constructor. 
     * Right now the behavior is to call a Util method to run an 
     * animation based on the keyword(s) present in the Tweet which
     * was streamed.
     */
    public class KeywordStreamer
    {
        private TwitterContext twitterCtx; //Instance of Twitter
        private string keywords; //Keywords to track.
        private string[] keywordArray; //Keywrods in array.
        private Dictionary<string, int> keywordDict; //Keywords paired with dict for anim
        private KeywordUtil ku;
        private string[] patterns; //Array of pattern commands for cube.
        private string[] colors; //Array of color commands for cube.

        /* Constructor */
        public KeywordStreamer(TwitterContext twitterCtx, 
            string keywords, string[] keywordArray,
            Dictionary<string,int> keywordDict, KeywordUtil ku, string[] patterns,
            string[] colors)
        {
            this.twitterCtx = twitterCtx;
            this.keywords = keywords;
            this.keywordArray = keywordArray;
            this.keywordDict = keywordDict;
            this.ku = ku;
            this.patterns = patterns;
            this.colors = colors;

            Console.WriteLine("\nTracking keywords:" + keywords);
        }

        /* Begin the keyword stream */
        public async void BeginStream()
        {
            Console.WriteLine("\nEntering Keyword Streaming Mode: \n");

            await
                (from strm in twitterCtx.Streaming
                 where strm.Type == StreamingType.Filter &&
                       strm.Track == keywords
                 select strm)
                .StartAsync(async strm =>
                {
                    bool exceptionFound = false;
                    JObject o = null;
                    try
                    {
                        //Parse the content into a Json object.
                        o = JObject.Parse(strm.Content);
                    }
                    catch (JsonReaderException) 
                    {
                        Console.WriteLine("Json Exception caught\n");
                        exceptionFound = true;
                    }
                    finally
                    {
                        if(!exceptionFound)
                        {
                            ///Extract the Tweet's text.
                            string tweetText = (string)o.SelectToken("text");

                            if (tweetText != null)
                            {
                                Console.WriteLine(tweetText + "\n");

                                //Determine which keywords were found.
                                /*
                                List<string> keywordsFound =
                                    ku.determineKeywordsFromString(keywordArray, tweetText);
                                */

                                /* Determine if this post which contains #Autodesk also contains #cube.
                                 * cubeFound should have length 1 if this is so.
                                 */
                                List<string> cubeFound = 
                                    ku.determineKeywordsFromString(new string[]{"#cube"}, tweetText);

                                /* If the user did indeed include #cube, look for other commands.*/
                                if(cubeFound.Count == 1)
                                {
                                    /* Find the valid pattern and color commands included in the tweet. */
                                    List<string> patternsFound = ku.determineKeywordsFromString(patterns, tweetText);
                                    List<string> colorsFound = ku.determineKeywordsFromString(colors, tweetText);

                                    /* */
                                    if(patternsFound.Count > 0 && colorsFound.Count > 0)
                                    {
                                        ku.RunAnimationBasedOnCommands(patternsFound[0], colorsFound[0]);
                                    }
                                    else
                                    {
                                        Console.WriteLine("Commands entered incorrectly.");
                                    }
                                }
                                else
                                {
                                    Console.WriteLine("Command cube not found.");
                                }
                            }
                        }
                    }
                });
        }
    }
}
