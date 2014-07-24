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

        public TestingHarness(HypnocubeImpl hc, TweetListener tl)
        {
            this.hc = hc;
            this.tl = tl;
        }

        /* Method to begin all the tests. */
        public void BeginTests()
        {
            List<byte[]> imageFrames = new List<byte[]>();

            /* Tests you want to perform go here. */
            RandomFullColorCubeChangeTest(imageFrames);
            ShiftOnceTest(imageFrames);

            tl.ReceiveAndSendSignal(imageFrames);
        }

        /* Test RandomFullColorCubeChange */
        public void RandomFullColorCubeChangeTest(List<byte[]> imageFrames)
        {
            hc.RandomFullCubeColorChange(imageFrames, 20, true);
        }

        /* Test shifting once. */
        public void ShiftOnceTest(List<byte[]> imageFrames)
        {
            Coordinate c = new Coordinate(7,0,0);
            RGBColor col = new RGBColor(255,0,0);
            hc.LightCrossSection(col, c, HypnocubeImpl.Direction.X, false);
            hc.ShiftOnce(imageFrames, HypnocubeImpl.Direction.X, true);
        }

    }
}
