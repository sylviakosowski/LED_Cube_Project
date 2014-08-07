using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cube_Visualization;

namespace Interactive_LED_Cube
{
    public class Animations
    {
        private static RGBColor black = new RGBColor(0, 0, 0);

        private HypnocubeImpl hc;
        private TweetListener tl;
        private PIC32 port;
        private List<byte[]> imageFrames;
        private bool physical;

        public Animations(HypnocubeImpl hc, TweetListener tl, PIC32 port, bool physical)
        {
            this.hc = hc;
            this.tl = tl;
            this.port = port;
            imageFrames = new List<byte[]>();
            this.physical = physical;
        }

        private void sendFrames()
        {
            if (physical)
            {
                DataTransfer.SendImagesToCube(port, imageFrames);
            }
            else
            {
                tl.ReceiveAndSendSignal(imageFrames);
            }
        }

        public void RandomFill(RGBColor color)
        {
            hc.RandomFill(imageFrames, color, true, 4);
            hc.RandomFill(imageFrames, black, false, 8);
            sendFrames();
        }
    }
}
