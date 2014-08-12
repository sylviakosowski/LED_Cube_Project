using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cube_Visualization;

namespace Interactive_LED_Cube.Cube
{
    /* Tests various HypnocubeImpl animations. */
    public class TestingHarness
    {
        private HypnocubeImpl hc;
        private TweetListener tl;
        private PIC32 port;
        private List<byte[]> imageFrames;
        private bool physical;

        private static RGBColor red = new RGBColor(255, 0, 0);
        private static RGBColor green = new RGBColor(0, 255, 0);
        private static RGBColor yellow = new RGBColor(255, 255, 0);
        private static RGBColor cyan = new RGBColor(0, 255, 255);
        private static RGBColor purple = new RGBColor(255, 0, 255);
        private static RGBColor blue = new RGBColor(0, 0, 255);
        private static RGBColor black = new RGBColor(0,0,0);

        private ColorPalette redP = new SolidPalette(red);
        private ColorPalette greenP = new SolidPalette(green);
        private ColorPalette yellowP = new SolidPalette(yellow);
        private ColorPalette cyanP = new SolidPalette(cyan);
        private ColorPalette purpleP = new SolidPalette(purple);
        private ColorPalette cp = new RainbowPalette();

        public TestingHarness(HypnocubeImpl hc, TweetListener tl, PIC32 port, bool physical)
        {
            this.hc = hc;
            this.tl = tl;
            this.port = port;
            imageFrames = new List<byte[]>();
            this.physical = physical;
        }

        /* Method to begin all the tests. */
        public void BeginTests()
        {
            /* Tests you want to perform go here. */
            //RandomFullColorCubeChangeRepeatTest();
            //BlinkLEDTest();
            //LightIntersectionTest();

            /* SHIFTING TESTS */
            //ShiftOnceTest(HypnocubeImpl.Direction.X, true);
            //ShiftAlongCubeTest(HypnocubeImpl.Direction.Z, false);

            /* FADING TESTS */
            //FadeLEDTest();
            //FadeLEDsSameRateTest();
            //FadeLEDsDiffRatesTest();

            /* BLOCK TESTS */
            //LightBlockSingleColorTest();
            //FadeBlockSingleColorRateTest();
            //BlinkBlockSingleColorRateTest();
            //LightLEDsTest();

            /* SHIFT TESTS*/
            //ShiftBlockOnceDecreasingTest();
            //ShiftBlockAlongCubeDecreasingTest();
            //ShiftBlockOnceIncreasingTest();
            //ShiftBlockAlongCubeIncreasingTest();

            /* BLINKING TESTS */
            //
            //BlinkLEDTest();
            //BlinkLEDsTest();

            //RandomFillTest();
            //ZigZagFillTest();
            //ExpandingCubeTest();
            //LittleRoamerTest();

            //FadeLEDsRainbowTest();
            //LittleRoamerTest();
            //RoamerTest();
            //ManyRoamersTest();

            for (int i = 0; i < 1; i++ )
            {
                
                //FadeLEDsSameRateTest();
                //FadeLEDsDiffRatesTest();
                //FadeLEDsRainbowTest();
                //BlinkLEDsTest();
                RandomFillTest();
                ZigZagFillTest();
                ExpandingCubeTest();
                ManyRoamersTest();
                RoamerTest();
            }


                //Need to keep this to send signal to visualization.
                //Won't need for actual cube.
                if (physical)
                {
                    DataTransfer.SendImagesToCube(port, imageFrames);
                }
                else
                {
                    tl.ReceiveAndSendSignal(imageFrames);
                }
        }

        ///////////////// BASIC FUNCTIONALITY TESTS /////////////////

        /* Test RandomFullColorCubeChange */
        private void RandomFullColorCubeChangeTest()
        {
            hc.RandomFullCubeColorChange(imageFrames, 20, true);
        }

        /* Test repeat frames with random color cube */
        private void RandomFullColorCubeChangeRepeatTest()
        {
            List<byte[]> test = hc.RandomFullCubeColorChange(imageFrames, 20, true);
            hc.RepeatFrames(test, imageFrames, 2);
        }

        /* Test fading a single LED. */
        private void FadeLEDTest()
        {
            List<Coordinate> coords = new List<Coordinate>();
            List<RGBColor> colors = new List<RGBColor>();
            List<int> rates = new List<int>();
            LightingMethod fader = new Fader(hc);

            Coordinate c = new Coordinate(7,7,7);
            coords.Add(c);

            RGBColor red = new RGBColor(255, 0, 0);
            hc.changeColorLED(c, red, false);

            RGBColor blue = new RGBColor(0, 0, 255);
            colors.Add(blue);

            rates.Add(50);

            hc.LightLEDs(imageFrames, coords, colors, rates, fader, false);
        }

        /* Test fading multiple LEDs with the same rate. */
        private void FadeLEDsSameRateTest()
        {
            List<Coordinate> coords = new List<Coordinate>();
            List<RGBColor> colors = new List<RGBColor>();
            List<int> rates = new List<int>();
            RGBColor blue = new RGBColor(0, 0, 255);
            LightingMethod fader = new Fader(hc);

            for(int i = 0; i < 8; i++)
            {
                for(int j = 0; j < 8; j++)
                {
                    for(int k = 0; k < 8; k++)
                    {
                        coords.Add(new Coordinate(i,j,k));
                        colors.Add(blue);
                        rates.Add(50);
                    }
                }
            }

            hc.LightLEDs(imageFrames, coords, colors, rates, fader, false);
        }

        /* Test fading multiple LEDs with different rates. 
         * Be careful not to have a rate of 0 for anything!
         * LEDs with rate of zero will not change color.
         * Especially important with Coordinate (0,0,0).
         */
        private void FadeLEDsDiffRatesTest()
        {
            List<Coordinate> coords = new List<Coordinate>();
            List<RGBColor> colors = new List<RGBColor>();
            List<int> rates = new List<int>();
            RGBColor blue = new RGBColor(0, 0, 255);
            LightingMethod fader = new Fader(hc);
            

            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    for (int k = 0; k < 8; k++)
                    {
                        coords.Add(new Coordinate(i, j, k));
                        colors.Add(red);
                        if(i == 0 && j == 0 && k == 0)
                        {
                            rates.Add(5);
                        }
                        else
                        {
                            rates.Add((i + j + k) * 10);
                        }
                    }
                }
            }

            hc.LightLEDs(imageFrames, coords, colors, rates, fader, false);
        }

        /* Test blinking a single LED */
        private void BlinkLEDTest()
        {
            List<Coordinate> coords = new List<Coordinate>();
            List<RGBColor> colors = new List<RGBColor>();
            List<int> rates = new List<int>();
            List<int> numBlinks = new List<int>();
            

            Coordinate c = new Coordinate(7, 0, 7);
            coords.Add(c);

            RGBColor red = new RGBColor(255, 0, 0);
            colors.Add(red);

            rates.Add(5);
            numBlinks.Add(10);


            LightingMethod blinker = new Blinker(hc, numBlinks);
            hc.LightLEDs(imageFrames, coords, colors, rates, blinker, false);
        }
        
        /* Test blinking multiple LEDs */
        private void BlinkLEDsTest()
        {
            List<Coordinate> coords = new List<Coordinate>();
            List<RGBColor> colors = new List<RGBColor>();
            List<int> rates = new List<int>();
            List<int> numBlinks = new List<int>();
            ColorPalette cp = new RainbowPalette();

            RGBColor red = new RGBColor(255,0,0);

            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    for (int k = 0; k < 8; k++)
                    {
                        coords.Add(new Coordinate(i, j, k));
                        colors.Add(cp.MapCoordToColor(new Coordinate(i,j,k)));
                        rates.Add((i + j + k));
                        numBlinks.Add(i + j+ k);
                    }
                }
            }

            LightingMethod blinker = new Blinker(hc, numBlinks);
            hc.LightLEDs(imageFrames, coords, colors, rates, blinker, false);
        }

        /* Test LightLEDs */
        private void LightLEDsTest()
        {
            List<Coordinate> coords = new List<Coordinate>();
            List<RGBColor> colors = new List<RGBColor>();
            List<int> rates = new List<int>();
            LightingMethod fader = new Fader(hc);

            Coordinate c = new Coordinate(7,7,7);
            coords.Add(c);

            RGBColor green = new RGBColor(0, 255, 0);
            hc.changeColorLED(c, green, false);

            RGBColor red = new RGBColor(255, 0, 0);
            colors.Add(red);

            rates.Add(50);

            hc.LightLEDs(imageFrames, coords, colors, rates, fader, false);
        }

        /* Test lighting up a block. */
        private void LightBlockSingleColorTest()
        {
            ColorFiller filler = new ColorFiller(hc);

            filler.LightBlockUniform(imageFrames, new Coordinate(7, 7, 7), new Coordinate(5, 5, 5), 20, false, redP);
            filler.LightBlockUniform(imageFrames, new Coordinate(0, 0, 0), new Coordinate(0, 7, 7), 20, false, greenP);
            filler.LightBlockUniform(imageFrames, new Coordinate(0, 0, 0), new Coordinate(7, 0, 0), 20, false, yellowP);
            filler.LightBlockUniform(imageFrames, new Coordinate(7, 3, 4), new Coordinate(7, 3, 3), 20, false, cyanP);
            filler.LightBlockUniform(imageFrames, new Coordinate(7, 7, 0), new Coordinate(7, 7, 0), 20, false, purpleP);
        }

        /* Fade a block into a single color at a uniform rate */
        private void FadeBlockSingleColorRateTest()
        {
            Fader fader = new Fader(hc);

            fader.LightBlockUniform(imageFrames, new Coordinate(7, 7, 7), new Coordinate(5, 5, 5), 20, false, redP);
            fader.LightBlockUniform(imageFrames, new Coordinate(0, 0, 0), new Coordinate(0, 7, 7), 20, false, greenP);
            fader.LightBlockUniform(imageFrames, new Coordinate(0, 0, 0), new Coordinate(7, 0, 0), 20, false, yellowP);
            fader.LightBlockUniform(imageFrames, new Coordinate(7, 3, 4), new Coordinate(7, 3, 3), 20, false, cyanP);
            fader.LightBlockUniform(imageFrames, new Coordinate(7, 7, 0), new Coordinate(7, 7, 0), 20, false, purpleP);            
        }

        /* Blink a block in a single color at a uniform rate */
        private void BlinkBlockSingleColorRateTest()
        {
            Blinker blinker = new Blinker(hc, null);

            blinker.BlinkBlockUniform(imageFrames, new Coordinate(7, 7, 7), new Coordinate(5, 5, 5), 10, 3, false, redP);
            blinker.BlinkBlockUniform(imageFrames, new Coordinate(0, 0, 0), new Coordinate(0, 7, 7), 1, 10, false, greenP);
        }





        ////////////// WORK IN PROGRESS TESTS /////////////////

        /* Shift a block in a decreasing direction once. */
        private void ShiftBlockOnceDecreasingTest()
        {
            ColorFiller filler = new ColorFiller(hc);
            Blinker blinker = new Blinker(hc, null);
            Fader fader = new Fader(hc);

            fader.LightBlockUniform(imageFrames, new Coordinate(0, 0, 0), new Coordinate(0, 7, 7), 20, false, greenP);
            fader.LightBlockUniform(imageFrames, new Coordinate(0, 0, 0), new Coordinate(7, 0, 0), 20, false, yellowP);
            fader.LightBlockUniform(imageFrames, new Coordinate(7, 3, 4), new Coordinate(7, 3, 3), 20, false, cyanP);
            fader.LightBlockUniform(imageFrames, new Coordinate(7, 7, 0), new Coordinate(7, 7, 0), 20, false, purpleP);
            fader.LightBlockUniform(imageFrames, new Coordinate(7, 7, 7), new Coordinate(5, 5, 5), 40, false, redP);
            //blinker.BlinkBlockUniform(imageFrames, new Coordinate(7, 7, 7), new Coordinate(5, 5, 5), red, 10, 10);

            
            Tuple<Coordinate, Coordinate> meep = 
                hc.ShiftOnce(imageFrames, (imageFrames.Count - 25), HypnocubeImpl.Direction.X, true,
                new Coordinate(7, 7, 7), new Coordinate(5, 5, 5), black);

            Console.WriteLine("Tuple is: " + meep.Item1.ToString() + ", " + meep.Item2.ToString());

            hc.ShiftOnce(imageFrames, (imageFrames.Count - 15), HypnocubeImpl.Direction.X, true,
                meep.Item1, meep.Item2, black);
        }

        /* Shift a block in a decreasing direction multiple times. */
        private void ShiftBlockAlongCubeDecreasingTest()
        {
            Fader fader = new Fader(hc);
            Blinker blinker = new Blinker(hc, null);

            /*
            fader.LightBlockUniform(imageFrames, new Coordinate(0, 0, 0), new Coordinate(0, 7, 7), green, 20);
            fader.LightBlockUniform(imageFrames, new Coordinate(0, 0, 0), new Coordinate(7, 0, 0), yellow, 20);
            fader.LightBlockUniform(imageFrames, new Coordinate(7, 3, 4), new Coordinate(7, 3, 3), cyan, 20);
            fader.LightBlockUniform(imageFrames, new Coordinate(7, 7, 0), new Coordinate(7, 7, 0), purple, 20);

             * */
            /* UNCOMMENT TO TEST A FADING SHIFTING BLOCK, COMMENT OUT BLINK CODE BELOW*/
            /*
            fader.LightBlockUniform(imageFrames, new Coordinate(7, 7, 7), new Coordinate(5, 5, 5), red, 60);
            fader.LightBlockUniform(imageFrames, new Coordinate(7, 7, 7), new Coordinate(5, 5, 5), blue, 60);
            fader.LightBlockUniform(imageFrames, new Coordinate(7, 7, 7), new Coordinate(5, 5, 5), yellow, 60);
            */
            /* UNCOMMENT TO TEST A BLINKING SHIFTING BLOCK, COMMENT OUT FADE CODE ABOVE */
            
            /*
            blinker.BlinkBlockUniform(imageFrames, new Coordinate(7, 7, 7), new Coordinate(5, 5, 5), red, 5, 5);
            blinker.BlinkBlockUniform(imageFrames, new Coordinate(7, 7, 7), new Coordinate(5, 5, 5), blue, 5, 5);
            blinker.BlinkBlockUniform(imageFrames, new Coordinate(7, 7, 7), new Coordinate(5, 5, 5), yellow, 5, 5);
            */

            Console.WriteLine(imageFrames.Count);

            
            Tuple<Coordinate,Coordinate> meep1 = 
                hc.ShiftAlongCube(imageFrames, (imageFrames.Count - 150), HypnocubeImpl.Direction.X, true,
                new Coordinate(7, 7, 7), new Coordinate(5, 5, 5), 4, 4, black);
            Tuple<Coordinate,Coordinate> meep2 = 
                hc.ShiftAlongCube(imageFrames, (imageFrames.Count - 90), HypnocubeImpl.Direction.Y, true,
                meep1.Item1, meep1.Item2, 4, 4, black);
            Tuple<Coordinate, Coordinate> meep3 =
                hc.ShiftAlongCube(imageFrames, (imageFrames.Count - 30), HypnocubeImpl.Direction.Z, true,
                meep2.Item1, meep2.Item2, 4, 4, black);
        }

        private void ShiftBlockOnceIncreasingTest()
        {
            ColorFiller filler = new ColorFiller(hc);
            Blinker blinker = new Blinker(hc, null);
            Fader fader = new Fader(hc);

            /*
            fader.LightBlockUniform(imageFrames, new Coordinate(0, 0, 0), new Coordinate(0, 7, 7), green, 20);
            fader.LightBlockUniform(imageFrames, new Coordinate(0, 0, 0), new Coordinate(7, 0, 0), yellow, 20);
            fader.LightBlockUniform(imageFrames, new Coordinate(7, 3, 4), new Coordinate(7, 3, 3), cyan, 20);
            fader.LightBlockUniform(imageFrames, new Coordinate(7, 7, 0), new Coordinate(7, 7, 0), purple, 20);
            //fader.FadeBlockUniform(imageFrames, new Coordinate(1, 0, 1), new Coordinate(3, 2, 3), red, 40);
            fader.LightBlockUniform(imageFrames, new Coordinate(1, 3, 3), new Coordinate(3, 1, 1), red, 40);
            //blinker.BlinkBlockUniform(imageFrames, new Coordinate(7, 7, 7), new Coordinate(2,2,2), red, 10, 10);
            */

            Tuple<Coordinate, Coordinate> meep =
                hc.ShiftOnce(imageFrames, (imageFrames.Count - 25), HypnocubeImpl.Direction.X, false,
                new Coordinate(1, 3, 3), new Coordinate(3, 1, 1), black);

            Console.WriteLine("Tuple is: " + meep.Item1.ToString() + ", " + meep.Item2.ToString());

            
            Tuple<Coordinate, Coordinate> wut = 
                hc.ShiftOnce(imageFrames, (imageFrames.Count - 15), HypnocubeImpl.Direction.X, false,
                meep.Item1, meep.Item2, black);
        }

        private void ShiftBlockAlongCubeIncreasingTest()
        {
            Fader fader = new Fader(hc);
            Blinker blinker = new Blinker(hc, null);

            /*
            fader.LightBlockUniform(imageFrames, new Coordinate(0, 0, 0), new Coordinate(0, 7, 7), green, 20);
            fader.LightBlockUniform(imageFrames, new Coordinate(0, 0, 0), new Coordinate(7, 0, 0), yellow, 20);
            fader.LightBlockUniform(imageFrames, new Coordinate(7, 3, 4), new Coordinate(7, 3, 3), cyan, 20);
            fader.LightBlockUniform(imageFrames, new Coordinate(7, 7, 0), new Coordinate(7, 7, 0), purple, 20);
            */
            /* UNCOMMENT TO TEST A FADING SHIFTING BLOCK, COMMENT OUT BLINK CODE BELOW*/
            /*
            fader.LightBlockUniform(imageFrames, new Coordinate(1, 1, 0), new Coordinate(3, 3, 2), red, 60);
            fader.LightBlockUniform(imageFrames, new Coordinate(1, 1, 0), new Coordinate(3, 3, 2), blue, 60);
            fader.LightBlockUniform(imageFrames, new Coordinate(1, 1, 0), new Coordinate(3, 3, 2), yellow, 60);
            */
            /* UNCOMMENT TO TEST A BLINKING SHIFTING BLOCK, COMMENT OUT FADE CODE ABOVE */

            /*
            blinker.BlinkBlockUniform(imageFrames, new Coordinate(7, 7, 7), new Coordinate(5, 5, 5), red, 5, 5);
            blinker.BlinkBlockUniform(imageFrames, new Coordinate(7, 7, 7), new Coordinate(5, 5, 5), blue, 5, 5);
            blinker.BlinkBlockUniform(imageFrames, new Coordinate(7, 7, 7), new Coordinate(5, 5, 5), yellow, 5, 5);
            */

            Console.WriteLine(imageFrames.Count);


            Tuple<Coordinate, Coordinate> meep1 =
                hc.ShiftAlongCube(imageFrames, (imageFrames.Count - 150), HypnocubeImpl.Direction.X, false,
                new Coordinate(1, 1, 0), new Coordinate(3, 3, 2), 4, 2, black);

            /*
            Tuple<Coordinate, Coordinate> meep2 =
                hc.ShiftAlongCube(imageFrames, (imageFrames.Count - 90), HypnocubeImpl.Direction.Y, false,
                meep1.Item1, meep1.Item2, 4, 1, black);
             * */

            /*
            Tuple<Coordinate, Coordinate> meep3 =
                hc.ShiftAlongCube(imageFrames, (imageFrames.Count - 30), HypnocubeImpl.Direction.Z, false,
                meep2.Item1, meep2.Item2, 4, 3, purple);
             * */
        }
    




        //////////////// OFFICIAL ANIMATIONS TEST //////////////

        /* Fill the block with random colors and then disappear back out. */
        private void RandomFillTest()
        {
            hc.RandomFill(imageFrames, black, true, 4);
            hc.RandomFill(imageFrames, black, false, 8);
        }
    
        /* Fill the block in a zigzag pattern. */
        private void ZigZagFillTest()
        {
            //ColorPalette cp = new RainbowPalette();
            ColorPalette cp = new AutodeskPalette();

            ColorPalette bp = new SolidPalette(black);
            //hc.RandomFullCubeColorChange(imageFrames, 1, false);
            hc.ZigZagFill(imageFrames, true, 1, HypnocubeImpl.Direction.X, cp);
            hc.ZigZagFill(imageFrames, true, 1, HypnocubeImpl.Direction.Y, bp);
            hc.ZigZagFill(imageFrames, true, 1, HypnocubeImpl.Direction.Z, cp);
            hc.ZigZagFill(imageFrames, true, 1, HypnocubeImpl.Direction.Y, bp);
        }

        /* Expanding cube in the center, filled solidly. */
        private void ExpandingCubeTest()
        {
            ColorFiller cf = new ColorFiller(hc);
            Fader f = new Fader(hc);
            ColorPalette cp = new RainbowPalette();

            hc.ExpandingSolidCube(imageFrames, false, 40, f, cp);
        }

        private void LittleRoamerTest()
        {
            ColorFiller cf = new ColorFiller(hc);
            ColorPalette cp = new RainbowPalette();

            hc.LittleRoamer(imageFrames, 4, cf, true, cp);
            hc.LittleRoamer(imageFrames, 4, cf, false, cp);
        }

        private void RoamerTest()
        {
            ColorFiller cf = new ColorFiller(hc);
            ColorPalette cp = new RainbowPalette();
            Coordinate c1 = new Coordinate(3,3,3);
            Coordinate c2 = new Coordinate(5,5,5);

            hc.Roamer(imageFrames, 4, cf, true, cp, c1, c2);
            //hc.Roamer(imageFrames, 4, cf, false, cp, c1, c2);
        }
        
        private void ManyRoamersTest()
        {
            ColorFiller cf = new ColorFiller(hc);
            ColorPalette cp = new RainbowPalette();

            hc.ManyLittleRoamers(imageFrames, 4, cf, true, cp,20);
            //hc.ManyLittleRoamers(imageFrames, 4, cf, false, cp,10);
        }

        private void FadeLEDsRainbowTest()
        {
            List<Coordinate> coords = new List<Coordinate>();  
            List<RGBColor> colors = new List<RGBColor>();
            List<int> rates = new List<int>();

            LightingMethod fader = new Fader(hc);
            ColorPalette cp = new RainbowPalette();
            //ColorPalette cp = new AutodeskPalette();
            //ColorPalette cp = new BlueGreenPalette();
            //ColorPalette cp = new RedBluePalette();

            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    for (int k = 0; k < 8; k++)
                    {
                        coords.Add(new Coordinate(i, j, k));
                        colors.Add(cp.MapCoordToColor(new Coordinate(i, j, k)));
                        if (i == 0 && j == 0 && k == 0)
                        {
                            rates.Add(5);
                        }
                        else
                        {
                            rates.Add((i + j + k) * 10);
                        }
                    }
                }
            }

            hc.LightLEDs(imageFrames, coords, colors, rates, fader, false);
        }

    }
}
