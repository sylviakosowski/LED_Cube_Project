using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace CubeVisualization
{
    public class TweetListener
    {
        private Game game;

        public TweetListener(Game game)
        {
            this.game = game;
        }

        /* Given three bytes containing red, green, and blue values, 
         * return a vector which can be used to set color in OpenGL.
         */
        private Vector3 convertToGLColor(byte r, byte g, byte b)
        {
            float red = (float)r / (float)255.0;
            float green = (float)g / (float)255.0;
            float blue = (float)b / (float)255.0;

            return new Vector3(red, green, blue);
        }

        /* Loop through the image array changing each set of three
         * byte values to a Vector3 GL value. */
        private Vector3[] convertToGLArray(byte[] colorArray)
        {
            Vector3[] glColorArray = new Vector3[colorArray.Length/3];

            int j = 0;

            for (int i = 0; i < colorArray.Length; i += 3 )
            {
                Vector3 newColor = convertToGLColor(colorArray[i], colorArray[i+1], colorArray[i+2]);
                glColorArray[j] = newColor;
                j++;
            }

            return glColorArray;
        }

        /* Given a list of iamgeFrames, converts each of them to a GL array of
         * Vector3 values.
         */
        public void ReceiveSignal(List<byte[]> imageFrames) 
        {
            foreach(byte[] image in imageFrames)
            {
                Vector3[] glImage = convertToGLArray(image);
                game.changeCubeColors(glImage);
            }

        }

    }
}
