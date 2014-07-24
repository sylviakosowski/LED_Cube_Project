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

        public enum Direction { X, Y, Z };

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


        //////////////////////  BASIC HELPER METHODS //////////////////////

        /* Change color of a single LED at the position specified. */
        private byte[] changeColorLED(Coordinate coord, RGBColor color, bool blend)
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

        /* Method to obtain an index from a coordinate. */
        private int IndexFromCoord(Coordinate c)
        {
            return (c.X + 8 * c.Y + 64 * c.Z) * 3;
        }

        /* Method to get the color at a specific coordinate. */
        private RGBColor ColorFromCoord(Coordinate c)
        {
            int index = IndexFromCoord(c);
            byte red = colorArray[index];
            byte green = colorArray[index + 1];
            byte blue = colorArray[index + 2];

            return new RGBColor(red, green, blue);
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

        /* Adds an image frame to the image frame queue. */
        private void AddImageFrame(List<byte[]> imageFrames)
        {
            byte[] newImage = new byte[ColorArray.Length];
            Array.Copy(ColorArray, newImage, ColorArray.Length);
            imageFrames.Add(newImage);
        }

        //////////////////////// MORE INVOLVED HELPERS //////////////////////////

        /* Changes the whole cube to the specified color. */
        private void SpecificColorWholeCube(RGBColor color, bool blend)
        {
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    for (int k = 0; k < 8; k++)
                    {
                        changeColorLED(new Coordinate(i,j,k), color, blend);
                    }
                }
            }
        }

        /* Changes the whole cube to a random color. */
        private void ChangeToRandColor(Random rand, bool blend)
        {
            byte randR = (byte)rand.Next(0, 255);
            byte randG = (byte)rand.Next(0, 255);
            byte randB = (byte)rand.Next(0, 255);

            SpecificColorWholeCube(new RGBColor(randR, randG, randB), blend);
        }

        /* Light up a vertical strip of LEDs between the two coordinates specified */
        private void LightVerticalStrip(List<byte[]> imageFrames, Coordinate c, int y2, RGBColor color)
        {
            int yMax = Math.Max(c.Y, y2);
            int yMin = Math.Min(c.Y, y2);

            for (int i = yMin; i <= yMax; i++)
            {
                changeColorLED(new Coordinate(c.X, i, c.Z), color, false);
            }
        }

        /* Light up a horizontal strip of LEDs between the two coordinates specified. */
        private void LightHorizontalStrip(List<byte[]> imageFrames, Coordinate c, int z2, RGBColor color)
        {
            int zMax = Math.Max(c.Z, z2);
            int zMin = Math.Min(c.Z, z2);

            for (int i = zMin; i <= zMax; i++)
            {
                changeColorLED(new Coordinate(c.X, c.Y, i), color, true);
            }
        }

        /* Light up a cross section, given a coordinate and a plane which the cross section
         * should be on. Direction is an enum and can be x, y, or z.
         * */
        public void LightCrossSection(List<byte[]> imageFrames, RGBColor col, Coordinate c, Direction d, bool blend)
        {
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    switch (d)
                    {
                        case Direction.X:
                            {
                                changeColorLED(new Coordinate(c.X, i, j), col, blend);
                                break;
                            }
                        case Direction.Y:
                            {
                                changeColorLED(new Coordinate(i, c.Y, j), col, blend);
                                break;
                            }
                        case Direction.Z:
                            {
                                changeColorLED(new Coordinate(i, j, c.Z), col, blend);
                                break;
                            }
                        default:
                            {
                                break;
                            }

                    }
                }
            }

            AddImageFrame(imageFrames);
        }

        /////////////////////////// ANIMATION OPTIONS ////////////////////////////
        /* Distinguished by using the AddImageFrame method and therefore actually adding 
         * frames to the animation.
         */

        /* Makes the cube blink quickly through count number of random colors */
        public void RandomFullCubeColorChange(List<byte[]> imageFrames, int count, bool blend)
        {
            Random rand = new Random();

            for (int i = 0; i < count; i++)
            {
                ChangeToRandColor(rand, blend);
                AddImageFrame(imageFrames);
            }
        }

        /* Blink a specific LED */
        public void BlinkLED(List<byte[]> imageFrames, Coordinate c)
        {
            SpecificColorWholeCube(new RGBColor(56, 56, 56), false);
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

        /* Light up a cross section of the cube
         * 
         * TODO: MAKE GENERIC
         */
        public void LightIntersection(List<byte[]> imageFrames)
        {
            Coordinate c = new Coordinate(7,0,0);
            RGBColor red = new RGBColor(255,0,0);
            RGBColor blue = new RGBColor(0,0,255);
            LightVerticalStrip(imageFrames, c, 7, red);
            LightHorizontalStrip(imageFrames, c, 7, blue);

            AddImageFrame(imageFrames);
        }

        /* Shifts the entire image on the plane x. 
         * 
         * TODO: make more generic
         */
        public void ShiftOnce(List<byte[]> imageFrames, Direction d, bool decreasing)
        {
            int xLowerBound = 0;
            int yLowerBound = 0;
            int zLowerBound = 0;

            switch(d)
            {
                case Direction.X:
                    {
                        xLowerBound = 1;
                        break;
                    }
                case Direction.Y:
                    {
                        yLowerBound = 1;
                        break;
                    }
                case Direction.Z:
                    {
                        zLowerBound = 1;
                        break;
                    }
            }

            //Shifting back in x direction
            for (int x = xLowerBound; x < 8; x++ )
            {
                for (int y = yLowerBound; y < 8; y++ )
                {
                    for(int z = zLowerBound; z < 8; z++)
                    {
                        //Get color of initial coordinate
                        RGBColor color = ColorFromCoord(new Coordinate(x, y, z));
                        switch(d)
                        {
                            case Direction.X:
                                {
                                    //Put whatever color was there into the space one x-value away from initial coordinate.
                                    changeColorLED(new Coordinate((x-1), y, z), color, false);
                                    break;
                                }
                            case Direction.Y:
                                {
                                    changeColorLED(new Coordinate(x, (y-1), z), color, false);
                                    break;
                                }
                            case Direction.Z:
                                {
                                    changeColorLED(new Coordinate(x, y, (z-1)), color, false);
                                    break;
                                }
                            default:
                                {
                                    //Do nothing
                                    break;
                                }
                        }
                    }
                }
            }

            LightCrossSection(imageFrames, new RGBColor(50,50,50), new Coordinate(7,7,7), d, false);


            AddImageFrame(imageFrames);
            
        }

        /* Shift the LEDs of the cube according to d and decreasing. */
        public void ShiftAlongCube(List<byte[]> imageFrames, Direction d, bool decreasing)
        {
            for(int i = 0; i < 8; i++)
            {
                ShiftOnce(imageFrames, d, decreasing);
            }
        }
    }
}
