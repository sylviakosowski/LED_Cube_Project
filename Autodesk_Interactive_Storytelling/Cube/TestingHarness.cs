using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cube_Visualization;

namespace Autodesk_Interactive_Storytelling.Cube
{
    /* Tests various HypnocubeImpl animations. */
    public class TestingHarness
    {
        private HypnocubeImpl hc;
        private TweetListener tl;
        private List<byte[]> imageFrames;

        public TestingHarness(HypnocubeImpl hc, TweetListener tl)
        {
            this.hc = hc;
            this.tl = tl;
            imageFrames = new List<byte[]>();
        }

        /* Method to begin all the tests. */
        public void BeginTests()
        {
            /* Tests you want to perform go here. */
            RandomFullColorCubeChangeRepeatTest();
            //BlinkLEDTest();
            //LightIntersectionTest();

            //ShiftOnceTest(HypnocubeImpl.Direction.X, true);
            //ShiftAlongCubeTest(HypnocubeImpl.Direction.Z, false);

            //FadeLEDTest();
            //FadeLEDsSameRateTest();
            FadeLEDsDiffRatesTest();

            //Need to keep this to send signal to visualization.
            tl.ReceiveAndSendSignal(imageFrames);
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
            hc.RepeatFrames(test, imageFrames, 3);
        }

        /* Test BlinkLED */
        private void BlinkLEDTest()
        {
            hc.BlinkLED(imageFrames, new Coordinate(7,0,0));
        }

        /* Test LightIntersection */
        private void LightIntersectionTest()
        {
            hc.LightIntersection(imageFrames);
        }

        /* Test lighting cross section. */
        private void LightCrossSectionTest(HypnocubeImpl.Direction d, Coordinate c)
        {
            RGBColor col = new RGBColor(255, 0, 0);
            hc.LightCrossSection(imageFrames, col, c, d, false);
        }

        /* Test shifting once. */
        private void ShiftOnceTest(HypnocubeImpl.Direction d, bool b)
        {
            Coordinate c;
            RGBColor col = new RGBColor(255, 0, 0);
            if(b)
            {
                c = new Coordinate(7, 0, 0);
                hc.LightCrossSection(imageFrames, col, c, d, false);
                hc.ShiftOnce(imageFrames, d, true);
            }
            else
            {
                c = new Coordinate(0, 0, 7);
                hc.LightCrossSection(imageFrames, col, c, d, false);
                hc.ShiftOnce(imageFrames, d, false);
            }
        }

        /* Test shifting along cube. */
        private void ShiftAlongCubeTest(HypnocubeImpl.Direction d, bool b)
        {
            if(b)
            {
                LightCrossSectionTest(d, new Coordinate(7,7,7));
            }
            else
            {
                LightCrossSectionTest(d, new Coordinate(0, 0, 0));
            }

            hc.ShiftAlongCube(imageFrames, d, b);
        }

        /* Test fading a single LED. */
        private void FadeLEDTest()
        {
            List<Coordinate> coords = new List<Coordinate>();
            List<RGBColor> colors = new List<RGBColor>();
            List<int> rates = new List<int>();

            Coordinate c = new Coordinate(7,7,7);
            coords.Add(c);

            RGBColor red = new RGBColor(255, 0, 0);
            hc.changeColorLED(c, red, false);

            RGBColor blue = new RGBColor(0, 0, 255);
            colors.Add(blue);

            rates.Add(50);

            hc.fadeLEDs(imageFrames, coords, colors, rates);
        }

        /* Test fading multiple LEDs with the same rate. */
        private void FadeLEDsSameRateTest()
        {
            List<Coordinate> coords = new List<Coordinate>();
            List<RGBColor> colors = new List<RGBColor>();
            List<int> rates = new List<int>();
            RGBColor blue = new RGBColor(0, 0, 255);

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

            hc.fadeLEDs(imageFrames, coords, colors, rates);
        }

        /* Test fading multiple LEDs with different rates. */
        private void FadeLEDsDiffRatesTest()
        {
            List<Coordinate> coords = new List<Coordinate>();
            List<RGBColor> colors = new List<RGBColor>();
            List<int> rates = new List<int>();
            RGBColor blue = new RGBColor(0, 0, 255);

            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    for (int k = 0; k < 8; k++)
                    {
                        coords.Add(new Coordinate(i, j, k));
                        colors.Add(blue);
                        rates.Add((i+j+k) * 10);
                    }
                }
            }

            hc.fadeLEDs(imageFrames, coords, colors, rates);
        }

    }
}
