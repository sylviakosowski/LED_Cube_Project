using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using CubeVisualization;

namespace Autodesk_Interactive_Storytelling
{
    /* Run this class to begin everything.*/
    public class Program
    {
        //Change to 0 for the OpenGL visualization, 1 for the actual cube.
        private static int CUBE_MODE = 0;

        private static TwitterObj t;
        private static HypnocubeImpl hc;
        private static Game game;
        private static KeywordUtil ku;
        private static TweetListener tl;

        //File name of the file containing keywords to use in keyword streaming mode.
        private static string keywordStreamPath = Path.Combine(Environment.CurrentDirectory, "keywordstream.txt");

        //File name of the file containing keywords to use in user streaming mode.
        private static string userStreamPath = Path.Combine(Environment.CurrentDirectory, "userstream.txt");

        static void Main(string[] args)
        {
            Begin(CUBE_MODE);
        }

        /* Creates a new Twitter instance and prompts for mode selection. */
        private static void Begin(int phys_mode)
        {
            t = new TwitterObj();
            hc = new HypnocubeImpl();

            t.DoEverything();

            Console.Write("Welcome to the Tweeting Hypnocube!\n");
            Console.Write("Please enter 0 for Passive Mode and 1 for Interactive Mode.\n");

            if(phys_mode == 0)
            {
                beginVisualizationMode();
            }

            Console.ReadLine();
        }

        /* Waits for input from standard input and converts it to an int. */
        private static void SeekResponse()
        {
            int response = Convert.ToInt32(Console.ReadLine());
            DetermineResponse(response);
        }

        /* Determines if input was 0, 1, or invalid, and acts accordingly. */
        private static void DetermineResponse(int response)
        {
            if (response == 0)
            {
                PassiveMode();
            }
            else if (response == 1)
            {
                InteractiveMode();
            }
            else
            {
                Console.Write("Invalid Response: Please enter either 0 or 1.\n");
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
            KeywordStreamer ks = 
                new KeywordStreamer(t.TwitterCtx, kr.CombinedString, kr.KeywordArray,
                    kr.KeywordDict, ku);
            ks.BeginStream();
        }

        /*
         * Running this method will stream from the authenticated user's Twitter account. 
         * This mode allows for a more interactive display: users will Tweet directly to
         * the authenticating account and the nature of their Tweet will change how the
         * Hypnocube will behave.
         * 
         * TODO: Right now the authenticating account is my own personal Twitter account.
         * We need to make an account for the Hypnocube.
         */
        private static void InteractiveMode()
        {
            KeywordReader kr = new KeywordReader(keywordStreamPath);
            kr.ExtractKeywords();
            UserStreamer us = new UserStreamer(t.TwitterCtx, kr.KeywordArray, kr.KeywordDict, ku);
            us.BeginStream();
        }

        /*///////////////////////// MODES FOR VISUALIZATION /////////////////////////////*/

        /* Run light animations on OpenGL visualization. */
        private static void beginVisualizationMode()
        {
            game = new Game();
            tl = new TweetListener(game);
            ku = new KeywordUtil(tl, CUBE_MODE);

            SeekResponse();

            game.Run(30, 30);
        }

        /* Run light animations on physical Hypnocube */
        private static void beginPhysicalMode()
        {
            //Nothing here yet!
        }
    }
}
