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
        public byte[] changeColorLED(Coordinate coord, RGBColor color, bool blend)
        {
            int index = IndexFromCoord(coord);

            RGBColor newColor = color;
            if(blend == true)
            {
                RGBColor existingColor = 
                    new RGBColor(colorArray[index], colorArray[index + 1], colorArray[index + 2]);
                newColor = Blend(existingColor, color);
            }

            colorArray[index] = newColor.R;
            colorArray[index + 1] = newColor.G;
            colorArray[index + 2] = newColor.B;

            return colorArray;
        }

        /* Method to obtain an index from a tuple of 3 dimensional coordinates. */
        private int IndexFromCoord(Coordinate c)
        {
            return (c.X + 8 * c.Y + 64 * c.Z) * 3;
        }

        /* Takes the average of two bytes, used in blending. */
        private byte AverageBytes(byte b1, byte b2)
        {
            return (byte)((b1+b2)/2);
        }

        /* Mixes two RGBColors, returning the new color. */
        private RGBColor Blend(RGBColor c1, RGBColor c2)
        {
            byte avgRed = AverageBytes(c1.R, c2.R);
            byte avgGreen = AverageBytes(c1.G, c2.G);
            byte avgBlue = AverageBytes(c1.B, c2.B);

            return new RGBColor(avgRed, avgGreen, avgBlue);
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
        public void SpecificColorWholeCube(RGBColor color)
        {
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    for (int k = 0; k < 8; k++)
                    {
                        changeColorLED(new Coordinate(i,j,k), color, false);
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


            SpecificColorWholeCube(new RGBColor(randR, randG, randB));
        }

        /* Makes the cube blink quickly through 10 random colors */
        public void RandomFullCubeColorChange(int count, List<byte[]> imageFrames)
        {
            Random rand = new Random();

            for (int i = 0; i < count; i++)
            {
                ChangeToRandColor(rand);
                AddImageFrame(imageFrames);
            }
        }

        /* Blink a specific LED */
        public void BlinkLED(List<byte[]> imageFrames, Coordinate c)
        {
            SpecificColorWholeCube(new RGBColor(56, 56, 56));
            int counter = 0;
            for (int i = 0; i < 40; i++ )
            {
                if(counter == 8)
                {
                    counter = 0;
                }
                
                if(counter < 4)
                {
                    changeColorLED(c, new RGBColor(255, 0, 0), false);
                    AddImageFrame(imageFrames);
                    counter++;
                }
                else
                {
                    changeColorLED(c, new RGBColor(56, 56, 56), false);
                    AddImageFrame(imageFrames);
                    counter++;
                }
            }
        }

        /* Light up a vertical strip of LEDs between the two coordinates specified */
        public void LightVerticalStrip(List<byte[]> imageFrames, Coordinate c, int y2, RGBColor color)
        {
            int yMax = Math.Max(c.Y, y2);
            int yMin = Math.Min(c.Y, y2);

            for(int i = yMin; i <= yMax; i++)
            {
                changeColorLED(new Coordinate(c.X, i, c.Z), color, false);
            }

            //AddImageFrame(imageFrames);
        }

        /* Light up a horizontal strip of LEDs between the two coordinates specified. */
        public void LightHorizontalStrip(List<byte[]> imageFrames, Coordinate c, int z2, RGBColor color)
        {
            int zMax = Math.Max(c.Z, z2);
            int zMin = Math.Min(c.Z, z2);

            for(int i = zMin; i <= zMax; i++)
            {
                changeColorLED(new Coordinate(c.X, c.Y, i), color, true);
            }

            //AddImageFrame(imageFrames);
        }

        /* Light up a cross section of the cube */
        public void LightIntersection(List<byte[]> imageFrames)
        {
            Coordinate c = new Coordinate(7,0,0);
            RGBColor red = new RGBColor(255,0,0);
            RGBColor blue = new RGBColor(0,0,255);
            LightVerticalStrip(imageFrames, c, 7, red);
            LightHorizontalStrip(imageFrames, c, 7, blue);

            AddImageFrame(imageFrames);
        }

        /* Horizontal slider */
        public void HorizontalSlider(List<byte[]> imageFrames)
        {

        }

    }
}
