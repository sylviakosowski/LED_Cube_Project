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
            ShiftOnceTest();
            BlinkLEDTest();
            LightIntersectionTest();
            ShiftAlongCubeTest();

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

        /* Test shifting once. */
        private void ShiftOnceTest()
        {
            Coordinate c = new Coordinate(7,0,0);
            RGBColor col = new RGBColor(255,0,0);
            hc.LightCrossSection(col, c, HypnocubeImpl.Direction.X, false);
            hc.ShiftOnce(imageFrames, HypnocubeImpl.Direction.X, true);
        }

        /* Test shifting along cube */
        private void ShiftAlongCubeTest()
        {
            hc.ShiftAlongCube(imageFrames, HypnocubeImpl.Direction.X, true);
        }

    }
}
