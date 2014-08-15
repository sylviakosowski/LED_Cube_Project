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
     * A class which streams content from the authenticated user.
     * (The Twitter account we are usining to authenticate using
     * OAuth keys and stuff). The Tweets received from this user
     * stream can then be sorted by keywords they might contain.
     * 
     * This is NOT USED in the current hypnocube implementation.
     * But could maybe be useful later.
     */
    public class UserStreamer
    {
        private TwitterContext twitterCtx; //Instance of Twitter
        private string[] keywordArray; //Keywrods in array.
        private Dictionary<string, int> keywordDict; //Keywords paired with dict for anim
        private KeywordUtil ku;

        /* Constructor */
        public UserStreamer(TwitterContext twitterCtx, string[] keywordArray,
            Dictionary<string, int> keywordDict, KeywordUtil ku)
        {
            this.twitterCtx = twitterCtx;
            this.keywordArray = keywordArray;
            this.keywordDict = keywordDict;
            this.ku = ku;
        }

        /* Begin the stream */
        public async void BeginStream()
        {
            Console.WriteLine("\nEntering User Streaming Mode: \n");

            await
                 (from strm in twitterCtx.Streaming
                  where strm.Type == StreamingType.User
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
                         if (!exceptionFound)
                         {
                             ///Extract the Tweet's text.
                             string tweetText = (string)o.SelectToken("text");

                             if (tweetText != null)
                             {
                                 Console.WriteLine(tweetText + "\n");

                                 //Determine which keywords were found.
                                 List<string> keywordsFound =
                                     ku.determineKeywordsFromString(keywordArray, tweetText);

                                 //Run animations based on the keywords found.
                                 ku.RunAnimationBasedOnKeywords(keywordDict, keywordsFound);
                             }
                         }
                     }

                 });
        }
    }
}
