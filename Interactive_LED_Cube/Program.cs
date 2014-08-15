using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Cube_Visualization;

namespace Interactive_LED_Cube
{
    /* Run this class to begin everything.*/
    public class Program
    {
        private static TwitterObj t;
        private static HypnocubeImpl hc;
        private static Game game;
        private static KeywordUtil ku;
        private static TweetListener tl;
        private static Cube.TestingHarness th;
        private static PIC32 port;
        private static Animations anim;

        //File name of the file containing keywords to use in keyword streaming mode.
        private static string keywordStreamPath = Path.Combine(Environment.CurrentDirectory, "keywordstream.txt");

        //File name of the file containing keywords to use in user streaming mode.
        private static string userStreamPath = Path.Combine(Environment.CurrentDirectory, "userstream.txt");

        //File name of the file containing pattern words to parse out of tweets.
        private static string patternsPath = Path.Combine(Environment.CurrentDirectory, "patterns.txt");

        //File name of the file containing color words to parse out of tweets.
        private static string colorsPath = Path.Combine(Environment.CurrentDirectory, "colors.txt");

        static void Main(string[] args)
        {
            Begin();
        }

        /* Creates a new Twitter instance and prompts for mode selection. */
        private static void Begin()
        {

            Console.WriteLine("Welcome to the Tweeting Hypnocube!");
            Console.WriteLine("Enter one of the following commands:");
            Console.WriteLine("0 for Physical Cube without Twitter");
            Console.WriteLine("1 for Physical Cube with Twitter");
            Console.WriteLine("2 for OpenGL Visualization without Twitter");
            Console.WriteLine("3 for OpenGL Visualization with Twitter");

            SeekResponse();

            Console.ReadLine();
        }

        /* Waits for input from standard input and converts it to an int. */
        private static void SeekResponse()
        {
            int response = Convert.ToInt32(Console.ReadLine());
            DetermineResponse(response);
        }

        /* Determines if input was 0, 1, 2, 3 or invalid, and acts accordingly. */
        private static void DetermineResponse(int response)
        {

            if (response == 0)
            {
                //Physical cube + no twitter
                Initialize(true);
                Console.WriteLine("Hypnocube Mode: No Twitter");
                HypnocubeTestingMode();
            }
            else if (response == 1)
            {
                //Physical cube + twitter
                t = new TwitterObj();
                t.DoEverything();
                Initialize(true);

                Console.WriteLine("Hypnocube Mode: Twitter");
                HypnocubeMode();
            }
            else if (response == 2)
            {
                //OpenGL visualization + no twitter
                Initialize(false);
                Console.WriteLine("Cube Visualization Mode: No Twitter");
                VisualizationTestingMode();
            }
            else if (response == 3)
            {
                //OpenGL visualization + twitter
                Initialize(false);
                t = new TwitterObj();
                t.DoEverything();

                Console.WriteLine("Cube Visualization Mode: Twitter");
                VisualizationMode();
            }
            else
            {
                Console.Write("Invalid Response, please re-enter response.");
                SeekResponse();
            }
        }

        /*///////////////////////// MODES FOR TWITTER /////////////////////////////*/

        /* 
         * Running this method will stream from Twitter according to a predetermined
         * list of keywords. It will display a different pattern on the Hypnocube for 
         * each different keyword in the list. Therefore, this mode makes the cube a 
         * passive visualization of Twitter activity relating to Autodesk.
         */
        private static void PassiveMode()
        {
            KeywordReader kr = new KeywordReader(keywordStreamPath);
            kr.ExtractKeywords();
            string[] patterns = kr.ExtractKeywordsOnly(patternsPath);
            string[] colors = kr.ExtractKeywordsOnly(colorsPath);
            KeywordStreamer ks = 
                new KeywordStreamer(t.TwitterCtx, kr.CombinedString, kr.KeywordArray,
                    kr.KeywordDict, ku, patterns, colors);
            ks.BeginStream();
        }

        /*
         * Right now this method isn't used in the code. It takes step to open up a 
         * stream of the authenticated user's Twitter. The name is also MISLEADING,
         * since the interactive component of this project (users tweeting commands
         * to the cube) is actually combined with the passivemode above. So,
         * don't worry about this method, don't use it unless user streams are
         * later incorporated into the project.
         */
        private static void InteractiveMode()
        {
            KeywordReader kr = new KeywordReader(keywordStreamPath);
            kr.ExtractKeywords();
            UserStreamer us = new UserStreamer(t.TwitterCtx, kr.KeywordArray, kr.KeywordDict, ku);
            us.BeginStream();
        }

        /*///////////////////////// MODES FOR VISUALIZATION /////////////////////////////*/

        /* Initializes a ton of necessary stuff. */
        private static void Initialize(bool physical)
        {
            hc = new HypnocubeImpl(physical);
            game = new Game();
            tl = new TweetListener(game);
            port = new PIC32();

            anim = new Animations(hc, tl, port, physical);
            ku = new KeywordUtil(anim, physical);
        }

        /* Run light animations for Twitter on OpenGL visualization. */
        private static void VisualizationMode()
        {
            //SeekResponse();
            PassiveMode();
            game.Run(30, 30);
        }

        /* Run light animations for Twitter on physical Hypnocube */
        private static void HypnocubeMode()
        {
            port.Open("COM5");
            Console.WriteLine(port.IsConnected);
            PassiveMode();
        }

        /* Starts a game without Twitter, useful for testing in visualization mode. */
        private static void VisualizationTestingMode()
        {
            th = new Cube.TestingHarness(hc, tl, port, false);
            //th.BeginTests();
            anim = new Animations(hc, tl, port, false);
            Random r = new Random();

            anim.DoAll(new RandomPalette(r));
            anim.DoAll(new RainbowPalette());
            anim.DoAll(new SunrisePalette());
            anim.DoAll(new RedBluePalette());
            anim.DoAll(new AutodeskPalette());

            game.Run(30, 30);
        }

        /* Starts the actual cube without Twitter, useful for testing animations on the
         * physical cube itself.
         */
        private static void HypnocubeTestingMode()
        {

            th = new Cube.TestingHarness(hc, tl, port, true);
            port.Open("COM5");

            Console.WriteLine(port.IsConnected);

            //th.BeginTests();
            //game.Run(30, 30);
            bool physical = true;
            anim = new Animations(hc, tl, port, physical);
            Random r = new Random();
            
            anim.DoAll(new RandomPalette(r));
            anim.DoAll(new RainbowPalette());
            anim.DoAll(new SunrisePalette());
            anim.DoAll(new RedBluePalette());
            anim.DoAll(new AutodeskPalette());

            port.ClosePort();
        }
    }
}
