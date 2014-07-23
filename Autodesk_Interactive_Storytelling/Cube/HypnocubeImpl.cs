using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Autodesk_Interactive_Storytelling
{
    /* 
     * Class which represents the Hypnocube and provides methods to control
     * its behavior.
     */
    public class HypnocubeImpl
    {
        /*
         * Represents the current state of all LEDS in cube
         * Each byte in array is one RGB value for LED.
         * Therefore, each LED takes up 3 bytes and therefore, 3 positions in array.
         */
        private byte[] colorArray;
        private static int ARRAY_SIZE = 1536;

        /* Constructor */
        public HypnocubeImpl()
        {
            colorArray = new byte[ARRAY_SIZE];
        }

        /* Getter/setter for image */
        public byte[] ColorArray
        {
            get { return colorArray; }
            set { colorArray = value; }
        }

        /* Change color of a single LED at the position specified. */
        public byte[] changeColorLED(int x, int y, int z, byte red ,byte green, byte blue)
        {
            int index = IndexFromCoord(x,y,z);
            colorArray[index] = red;
            colorArray[index + 1] = green;
            colorArray[index + 2] = blue;
            return colorArray;
        }

        /* Method to obtain an index from a tuple of 3 dimensional coordinates. */
        public int IndexFromCoord(int x, int y, int z)
        {
            return (x + 8 * y + 64 * z) * 3;
        }


        //////////////////////// ANIMATION CODE ///////////////////////////////////////

        /* Adds an image frame to the image frame queue. */
        private void AddImageFrame(List<byte[]> imageFrames)
        {
            byte[] newImage = new byte[ColorArray.Length];
            Array.Copy(ColorArray, newImage, ColorArray.Length);
            imageFrames.Add(newImage);
        }

        /* Changes the whole cube to the specified color. */
        public void SpecificColorWholeCube(byte r, byte g, byte b)
        {
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    for (int k = 0; k < 8; k++)
                    {
                        changeColorLED(i, j, k, r, g, b);
                    }
                }
            }
        }

        /* Changes the whole cube to a random color. */
        public void ChangeToRandColor(Random rand)
        {
            byte randR = (byte)rand.Next(0, 255);
            byte randG = (byte)rand.Next(0, 255);
            byte randB = (byte)rand.Next(0, 255);


            SpecificColorWholeCube(randR, randG, randB);
        }

        /* Makes the cube blink quickly through 10 random colors */
        public void RandomFullCubeColorChange(int count, List<byte[]> imageFrames)
        {
            Random rand = new Random();

            for (int i = 0; i < count; i++)
            {
                ChangeToRandColor(rand);
                AddImageFrame(imageFrames);
                /*
                byte[] newImage = new byte[ColorArray.Length];
                Array.Copy(ColorArray, newImage, ColorArray.Length);
                imageFrames.Add(newImage);
                 */
            }
        }

        /* Blink a specific LED */
        public void BlinkLED(List<byte[]> imageFrames, int x, int y, int z)
        {
            SpecificColorWholeCube(56, 56, 56);
            int counter = 0;
            for (int i = 0; i < 40; i++ )
            {
                if(counter == 8)
                {
                    counter = 0;
                }
                
                if(counter < 4)
                {
                    changeColorLED(x, y, z, 255, 0, 0);
                    AddImageFrame(imageFrames);
                    counter++;
                }
                else
                {
                    changeColorLED(x, y, z, 56, 56, 56);
                    AddImageFrame(imageFrames);
                    counter++;
                }
            }
        }

        /* Light up a vertical strip of LEDs between the two coordinates specified */
        public void LightHorizontalStrip(List<byte[]> imageFrames, int x, int z, int y1, int y2, byte r, byte g, byte b)
        {
            int yMax = Math.Max(y1, y2);
            int yMin = Math.Min(y1, y2);

            for(int i = yMin; i <= yMax; i++)
            {
                changeColorLED(x, i, z, r, g, b);
            }

            AddImageFrame(imageFrames);
        }

        /* Light up a cross section of the cube */
        public void LightUpCrossSection()
        {

        }

        /* Horizontal slider */
        public void HorizontalSlider(List<byte[]> imageFrames)
        {

        }

    }
}
