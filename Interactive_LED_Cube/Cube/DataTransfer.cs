using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interactive_LED_Cube
{
    /* Used to send animations to the physical cube (not the OpenGL simulation,
     * that uses the TweetListener class in Cube_Visualization. */
    public static class DataTransfer
    {
        /* Port is an already opened port. */
        public static void SendImagesToCube(PIC32 port, List<byte[]> imageFrames)
        {
            var imageQueue = new Queue<byte[]>(imageFrames);

            while (imageQueue.Count > 0)
            {
                byte[] currentImage = imageQueue.Dequeue();
                port.WriteImage(currentImage, false);
            }
        }


    }
}
