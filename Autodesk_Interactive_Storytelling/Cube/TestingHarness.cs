using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CubeVisualization;

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
            //List<byte[]> imageFrames = new List<byte[]>();

            /* Tests you want to perform go here. */
            RandomFullColorCubeChangeTest();
            //BlinkLEDTest();
            //LightIntersectionTest();

            LightCrossSectionTest(HypnocubeImpl.Direction.X, new Coordinate(7,7,7));
            ShiftAlongCubeDecreasingTest(HypnocubeImpl.Direction.X);

            //LightCrossSectionTest(HypnocubeImpl.Direction.X, new Coordinate(0, 0, 0));
            //ShiftAlongCubeIncreasingTest(HypnocubeImpl.Direction.X);

            tl.ReceiveAndSendSignal(imageFrames);
        }

        /* Test RandomFullColorCubeChange */
        private void RandomFullColorCubeChangeTest()
        {
            hc.RandomFullCubeColorChange(imageFrames, 20, true);
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
        private void ShiftOnceDecreasingTest(HypnocubeImpl.Direction d)
        {
            Coordinate c = new Coordinate(7,0,0);
            RGBColor col = new RGBColor(255,0,0);
            hc.LightCrossSection(imageFrames, col, c, d, false);
            hc.ShiftOnceDecreasing(imageFrames, d, true);
        }

        private void ShiftOnceIncreasingTest(HypnocubeImpl.Direction d)
        {
            Coordinate c = new Coordinate(0, 0, 7);
            RGBColor col = new RGBColor(255, 0, 0);
            hc.LightCrossSection(imageFrames, col, c, d, false);
            hc.ShiftOnceIncreasing(imageFrames, d, true);
        }

        /* Test shifting along cube */
        private void ShiftAlongCubeDecreasingTest(HypnocubeImpl.Direction d)
        {
            hc.ShiftAlongCubeDecreasing(imageFrames, d, true);
        }

        private void ShiftAlongCubeIncreasingTest(HypnocubeImpl.Direction d)
        {
            hc.ShiftAlongCubeIncreasing(imageFrames, d, true);
        }

    }
}
