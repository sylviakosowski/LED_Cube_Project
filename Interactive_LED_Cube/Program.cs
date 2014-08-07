﻿using System;
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
        /* Cube Mode: 0 for starting cube (opengl or phys) with Twitter.
         * 1 for Visualization Mode, No Twitter
         * 2 for Physical Mode, No Twitter
         */
        private static int CUBE_MODE = 2;

        private static TwitterObj t;
        private static HypnocubeImpl hc;
        private static Game game;
        private static KeywordUtil ku;
        private static TweetListener tl;
        private static Cube.TestingHarness th;
        private static PIC32 port;

        //File name of the file containing keywords to use in keyword streaming mode.
        private static string keywordStreamPath = Path.Combine(Environment.CurrentDirectory, "keywordstream.txt");

        //File name of the file containing keywords to use in user streaming mode.
        private static string userStreamPath = Path.Combine(Environment.CurrentDirectory, "userstream.txt");

        //File name of the file containing words to parse out of tweets.
        private static string commandsPath = Path.Combine(Environment.CurrentDirectory, "commands.txt");

        static void Main(string[] args)
        {
            Begin(CUBE_MODE);
        }

        /* Creates a new Twitter instance and prompts for mode selection. */
        private static void Begin(int phys_mode)
        {

            Console.WriteLine("Welcome to the Tweeting Hypnocube!");
            Console.WriteLine("Enter one of the following commands:");
            Console.WriteLine("0 for Physical Cube without Twitter");
            Console.WriteLine("1 for Physical Cube with Twitter");
            Console.WriteLine("2 for OpenGL Visualization without Twitter");
            Console.WriteLine("3 for OpenGL Visualization with Twitter");

            SeekResponse();

            /*
            if(phys_mode == 1)
            {
                Console.WriteLine("Cube Visualization Mode: No Twitter");
                VisualizationTestingMode();
            }
            else if(phys_mode == 2)
            {
                Console.WriteLine("Physical Hypnocube Mode: No Twitter");
                HypnocubeTestingMode();
            }
            else if(phys_mode == 0)
            {
                Console.WriteLine("Enter 0 for Passive Mode, 1 for Interactive Mode \n");
                t = new TwitterObj();
                t.DoEverything();
                VisualizationMode();
            }
            */
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
                //PassiveMode();
                //Physical cube + no twitter
                Console.WriteLine("Hypnocube Mode: No Twitter");
                HypnocubeTestingMode();
            }
            else if (response == 1)
            {
                //InteractiveMode();
                //Physical cube + twitter

                t = new TwitterObj();
                t.DoEverything();

                Console.WriteLine("Hypnocube Mode: Twitter");
                HypnocubeMode();
            }
            else if (response == 2)
            {
                //OpenGL visualization + no twitter
                Console.WriteLine("Cube Visualization Mode: No Twitter");
                VisualizationTestingMode();
            }
            else if (response == 3)
            {
                //OpenGL visualization + twitter

                t = new TwitterObj();
                t.DoEverything();

                Console.WriteLine("Cube Visualization Mode: Twitter");
                VisualizationMode();
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
            string[] commands = kr.ExtractKeywordsOnly(commandsPath);
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

        /* Run light animations for Twitter on OpenGL visualization. */
        private static void VisualizationMode()
        {
            hc = new HypnocubeImpl(false);
            game = new Game();
            tl = new TweetListener(game);
            ku = new KeywordUtil(tl, hc);

            //SeekResponse();
            PassiveMode();

            game.Run(30, 30);
        }

        /* Run light animations for Twitter on physical Hypnocube */
        private static void HypnocubeMode()
        {
            hc = new HypnocubeImpl(true);
            port = new PIC32();

            tl = new TweetListener(game);
            ku = new KeywordUtil(port, hc);
            PassiveMode();
            //Nothing here yet!
        }

        /* Starts a game without Twitter, useful for testing in visualization mode. */
        private static void VisualizationTestingMode()
        {
            hc = new HypnocubeImpl(false);
            game = new Game();
            port = new PIC32();
            tl = new TweetListener(game);
            th = new Cube.TestingHarness(hc, tl, port, false);

            th.BeginTests();
            game.Run(30, 30);
        }

        /* Starts the actual cube without Twitter, useful for testing animations on the
         * physical cube itself.
         */
        private static void HypnocubeTestingMode()
        {
            hc = new HypnocubeImpl(true);
            port = new PIC32();
            game = new Game();
            tl = new TweetListener(game);
            th = new Cube.TestingHarness(hc, tl, port, true);

            port.Open("COM5");

            Console.WriteLine(port.IsConnected);

            th.BeginTests();
            //game.Run(30, 30);


            port.ClosePort();
        }
    }
}
