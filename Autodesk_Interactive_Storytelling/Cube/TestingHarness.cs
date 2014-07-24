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
            LightCrossSectionTest(HypnocubeImpl.Direction.Z, new Coordinate(7,7,7));
            //ShiftOnceTest(HypnocubeImpl.Direction.Z);
            //BlinkLEDTest();
            //LightIntersectionTest();
            ShiftAlongCubeTest(HypnocubeImpl.Direction.Z);

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
        private void ShiftOnceTest(HypnocubeImpl.Direction d)
        {
            Coordinate c = new Coordinate(7,0,0);
            RGBColor col = new RGBColor(255,0,0);
            hc.LightCrossSection(imageFrames, col, c, d, false);
            hc.ShiftOnce(imageFrames, d, true);
        }

        /* Test shifting along cube */
        private void ShiftAlongCubeTest(HypnocubeImpl.Direction d)
        {
            hc.ShiftAlongCube(imageFrames, d, true);
        }

    }
}
