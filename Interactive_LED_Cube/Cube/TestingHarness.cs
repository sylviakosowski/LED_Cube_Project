﻿using System;
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

        private RGBColor red = new RGBColor(255, 0, 0);
        private RGBColor green = new RGBColor(0, 255, 0);
        private RGBColor yellow = new RGBColor(255, 255, 0);
        private RGBColor cyan = new RGBColor(0, 255, 255);
        private RGBColor purple = new RGBColor(255, 0, 255);

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

            ShiftBlockOnceDecreasingTest();

            /* BLINKING TESTS */
            //
            //BlinkLEDTest();
            //BlinkLEDsTest();

            //Need to keep this to send signal to visualization.
            //Won't need for actual cube.
            if(physical)
            {
                DataTransfer.SendImagesToCube(port, imageFrames);
            }
            else
            {
                tl.ReceiveAndSendSignal(imageFrames);
            }
        }

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

        /* Test shifting once. */
        /* TODO: Redo this section to use LightBlock instead of LightCrossSection */
        private void ShiftOnceTest(HypnocubeImpl.Direction d, bool b)
        {
            /*
            Coordinate c;
            RGBColor col = new RGBColor(255, 0, 0);
            if(b)
            {
                c = new Coordinate(7, 0, 0);
                hc.LightBlock(imageFrames, c, );
                hc.ShiftOnce(imageFrames, d, true);
            }
            else
            {
                c = new Coordinate(0, 0, 7);
                hc.LightCrossSection(imageFrames, col, c, d, false);
                hc.ShiftOnce(imageFrames, d, false);
            }
             */
        }

        /* Test shifting along cube. */
        /* TODO: Redo this test to use LightBlock instead of LightCrossSection */
        private void ShiftAlongCubeTest(HypnocubeImpl.Direction d, bool b)
        {
            /*
            if(b)
            {
                LightCrossSectionTest(d, new Coordinate(7,7,7));
            }
            else
            {
                LightCrossSectionTest(d, new Coordinate(0, 0, 0));
            }

            hc.ShiftAlongCube(imageFrames, d, b);
             */
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

            hc.LightLEDs(imageFrames, coords, colors, rates, fader);
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

            hc.LightLEDs(imageFrames, coords, colors, rates, fader);
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
                        colors.Add(blue);
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

            hc.LightLEDs(imageFrames, coords, colors, rates, fader);
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
            hc.LightLEDs(imageFrames, coords, colors, rates, blinker);
        }
        
        /* Test blinking multiple LEDs */
        private void BlinkLEDsTest()
        {
            List<Coordinate> coords = new List<Coordinate>();
            List<RGBColor> colors = new List<RGBColor>();
            List<int> rates = new List<int>();
            List<int> numBlinks = new List<int>();

            RGBColor red = new RGBColor(255,0,0);

            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    for (int k = 0; k < 8; k++)
                    {
                        coords.Add(new Coordinate(i, j, k));
                        colors.Add(red);
                        rates.Add((i + j + k));
                        numBlinks.Add(i + j+ k);
                    }
                }
            }

            LightingMethod blinker = new Blinker(hc, numBlinks);
            hc.LightLEDs(imageFrames, coords, colors, rates, blinker);
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

            hc.LightLEDs(imageFrames, coords, colors, rates, fader);
        }

        /* Test lighting up a block. */
        private void LightBlockSingleColorTest()
        {
            ColorFiller filler = new ColorFiller(hc);

            /*
            List<Coordinate> coords1 = hc.GenerateCoordBlock(new Coordinate(7, 7, 7), new Coordinate(5, 5, 5));
            List<Coordinate> coords2 = hc.GenerateCoordBlock(new Coordinate(0, 0, 0), new Coordinate(0, 7, 7));
            List<Coordinate> coords3 = hc.GenerateCoordBlock(new Coordinate(0, 0, 0), new Coordinate(7, 0, 0));
            List<Coordinate> coords4 = hc.GenerateCoordBlock(new Coordinate(7, 3, 4), new Coordinate(7, 3, 3));
            List<Coordinate> coords5 = hc.GenerateCoordBlock(new Coordinate(7, 7, 0), new Coordinate(7, 7, 0));
             */

            /*
            List<RGBColor> colors1 = new List<RGBColor>();
            List<int> rates1 = new List<int>();
            filler.UniformColorRate(coords1.Count, red, 20, colors1, rates1);

            List<RGBColor> colors2 = new List<RGBColor>();
            List<int> rates2 = new List<int>();
            filler.UniformColorRate(coords2.Count, green, 20, colors2, rates2);

            List<RGBColor> colors3 = new List<RGBColor>();
            List<int> rates3 = new List<int>();
            filler.UniformColorRate(coords3.Count, yellow, 20, colors3, rates3);

            List<RGBColor> colors4 = new List<RGBColor>();
            List<int> rates4 = new List<int>();
            filler.UniformColorRate(coords4.Count, cyan, 20, colors4, rates4);

            List<RGBColor> colors5 = new List<RGBColor>();
            List<int> rates5 = new List<int>();
            filler.UniformColorRate(coords5.Count, purple, 20, colors5, rates5);

            hc.LightBlock(imageFrames, new Coordinate(7, 7, 7), new Coordinate(5, 5, 5), colors1, rates1, filler);
            hc.LightBlock(imageFrames, new Coordinate(0, 0, 0), new Coordinate(0, 7, 7), colors2, rates2, filler);
            hc.LightBlock(imageFrames, new Coordinate(0, 0, 0), new Coordinate(7, 0, 0), colors3, rates3, filler);
            hc.LightBlock(imageFrames, new Coordinate(7, 3, 4), new Coordinate(7, 3, 3), colors4, rates4, filler);
            hc.LightBlock(imageFrames, new Coordinate(7, 7, 0), new Coordinate(7, 7, 0), colors5, rates5, filler);
             */

            filler.LightBlockUniform(imageFrames, new Coordinate(7, 7, 7), new Coordinate(5, 5, 5), red, 20);
            filler.LightBlockUniform(imageFrames, new Coordinate(0, 0, 0), new Coordinate(0, 7, 7), green, 20);
            filler.LightBlockUniform(imageFrames, new Coordinate(0, 0, 0), new Coordinate(7, 0, 0), yellow, 20);
            filler.LightBlockUniform(imageFrames, new Coordinate(7, 3, 4), new Coordinate(7, 3, 3), cyan, 20);
            filler.LightBlockUniform(imageFrames, new Coordinate(7, 7, 0), new Coordinate(7, 7, 0), purple, 20);
        }

        private void FadeBlockSingleColorRateTest()
        {
            Fader fader = new Fader(hc);

            fader.FadeBlockUniform(imageFrames, new Coordinate(7, 7, 7), new Coordinate(5, 5, 5), red, 20);
            fader.FadeBlockUniform(imageFrames, new Coordinate(0, 0, 0), new Coordinate(0, 7, 7), green, 20);
            fader.FadeBlockUniform(imageFrames, new Coordinate(0, 0, 0), new Coordinate(7, 0, 0), yellow, 20);
            fader.FadeBlockUniform(imageFrames, new Coordinate(7, 3, 4), new Coordinate(7, 3, 3), cyan, 20);
            fader.FadeBlockUniform(imageFrames, new Coordinate(7, 7, 0), new Coordinate(7, 7, 0), purple, 20);            
        }

        private void BlinkBlockSingleColorRateTest()
        {
            Blinker blinker = new Blinker(hc, null);

            blinker.BlinkBlockUniform(imageFrames, new Coordinate(7, 7, 7), new Coordinate(5, 5, 5), red, 10, 3);
            blinker.BlinkBlockUniform(imageFrames, new Coordinate(0, 0, 0), new Coordinate(0, 7, 7), green, 1, 10);
        }

        private void ShiftBlockOnceDecreasingTest()
        {
            ColorFiller filler = new ColorFiller(hc);
            Blinker blinker = new Blinker(hc, null);
            Fader fader = new Fader(hc);

            //filler.LightBlockUniform(imageFrames, new Coordinate(7, 7, 7), new Coordinate(5, 5, 5), red, 20);
            fader.FadeBlockUniform(imageFrames, new Coordinate(0, 0, 0), new Coordinate(0, 7, 7), green, 20);
            fader.FadeBlockUniform(imageFrames, new Coordinate(0, 0, 0), new Coordinate(7, 0, 0), yellow, 20);
            fader.FadeBlockUniform(imageFrames, new Coordinate(7, 3, 4), new Coordinate(7, 3, 3), cyan, 20);
            fader.FadeBlockUniform(imageFrames, new Coordinate(7, 7, 0), new Coordinate(7, 7, 0), purple, 20);
            fader.FadeBlockUniform(imageFrames, new Coordinate(7, 7, 7), new Coordinate(5, 5, 5), red, 40);

            hc.ShiftBlockOnceDecreasing(imageFrames, (imageFrames.Count - 35), HypnocubeImpl.Direction.X,
                new Coordinate(7, 7, 7), new Coordinate(5, 5, 5));

        }
    }
}
