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

        /////////////////////////// PUBLIC INTERFACE ////////////////////////////

        /* Makes the cube blink quickly through count number of random colors, with option
         * to make colors blend.
         */
        public List<byte[]> RandomFullCubeColorChange(List<byte[]> imageFrames, int count, bool blend)
        {
            List<byte[]> newFrames = new List<byte[]>(0);

            Random rand = new Random();

            for (int i = 0; i < count; i++)
            {
                ChangeToRandColor(rand, blend);
                AddImageFrame(imageFrames);
                AddImageFrame(newFrames);
            }

            return newFrames;
        }

        /* Light up an intersection of the cube.
         * 
         * TODO: MAKE GENERIC
         */
        public void LightIntersection(List<byte[]> imageFrames)
        {
            Coordinate c = new Coordinate(7, 0, 0);
            RGBColor red = new RGBColor(255, 0, 0);
            RGBColor blue = new RGBColor(0, 0, 255);
            LightVerticalStrip(imageFrames, c, 7, red);
            LightHorizontalStrip(imageFrames, c, 7, blue);

            AddImageFrame(imageFrames);
        }

        /* 
         * Light up a cross section of the cube.
         * 
         * col = the color the cross section should be
         * c = a single coordinate belonging to the desired cross section. Can be any coordinate in the cross section.
         * d = X, Y, or Z (enum). Specifies the value which does not change among all the coordinates in this cross-
         *      section. X will make the cross-section be on the YZ plane. Y will make the cross-section be on the XZ
         *      plane. Z will make the cross-section be on the XY plane.
         *      
         *      TODO: A DIAGRAM WILL BE PROVIDED FOR POTENTIAL FUTURE USERS
         */
        public void LightCrossSection(List<byte[]> imageFrames, RGBColor col, 
            Coordinate c, Direction d, bool blend)
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

        /*
         * Shift the colors of the cube by one in a specific direction.
         * 
         * d = specifies in what plane to shift the cube. If X is specified, all the x 
         *      coordinates will be shifted by one as indicated by the bool decreasing.
         *      If decreasing is true, x will be shifted down and if decreasing is 
         *      false x will be shifted up.
         *      
         *      For example: if d = X and decreasing = true, then the color of the
         *      LED at coordinate (1,2,3) will be copied to the LED at coordinate
         *      (0,2,3). If decreasing = false, then the color at (1,2,3) will be 
         *      copied to (2,2,3).
         *      
         *      The same applies for the direction Y and Z.
         *      
         * decreasing = true if we are decreasing the d-coordinates specified,
         *      false if we are increasing the d-coordinates specified.
         */
        public void ShiftOnce(List<byte[]> imageFrames, Direction d, bool decreasing)
        {
            if (decreasing)
            {
                ShiftOnceDecreasing(imageFrames, d);
            }
            else
            {
                ShiftOnceIncreasing(imageFrames, d);
            }
        }

        /* 
         * Iterate ShiftOnce 8 times so that the lights in the cube pass through the
         * entire cube from one side to another. d and decreasing behave as described
         * in ShiftOnce.
         */
        public void ShiftAlongCube(List<byte[]> imageFrames, Direction d, bool decreasing)
        {
            for (int i = 0; i < 8; i++)
            {
                ShiftOnce(imageFrames, d, decreasing);
            }
        }

        /* Given a list of frames to repeat and an int count which determines the number of
         * times we repeat, repeat an animation.
         */
        public void RepeatFrames(List<byte[]> framesToRepeat, List<byte[]> imageFrames, int count)
        {
            for(int i = 0; i < count; i++)
            {
                foreach(byte[] frame in framesToRepeat)
                {
                    imageFrames.Add(frame);
                }
            }
        }

        /* Precondition: coords, endColors, and rates are all the same length. */
        public void FadeLEDs(List<byte[]> imageFrames, List<Coordinate> coords,
            List<RGBColor> endColors, List<int> rates)
        {
            List<RGBColor> fadeAnimation = new List<RGBColor>();
            Dictionary<Coordinate, List<RGBColor>> animDict =
                new Dictionary<Coordinate, List<RGBColor>>();
            int longestAnim = 0;

            /* Create fading animation for each specific LED in coords, map it
             * to each coordinate in a dictionary.
             */
            for (int i = 0; i < coords.Count; i++)
            {
                fadeAnimation = fadeLED(coords[i], endColors[i], rates[i]);
                animDict.Add(coords[i], fadeAnimation);
                longestAnim = Math.Max(longestAnim, fadeAnimation.Count);
            }

            int index;
            RGBColor color;

            /* For each animation frame, update the behavior of each LED in coords. */
            for (int i = 0; i < longestAnim; i++)
            {
                foreach (KeyValuePair<Coordinate, List<RGBColor>> entry in animDict)
                {
                    if (i < entry.Value.Count)
                    {
                        index = IndexFromCoord(entry.Key);
                        color = entry.Value[i];
                        colorArray[index] = color.R;
                        colorArray[index + 1] = color.G;
                        colorArray[index + 2] = color.B;
                    }
                }
                AddImageFrame(imageFrames);
            }
        }

        public void BlinkLEDs(List<byte[]> imageFrames, List<Coordinate> coords,
            List<RGBColor> colors, List<int> rates, List<int> numBlinks)
        {
            List<RGBColor> blinkAnimation = new List<RGBColor>();
            Dictionary<Coordinate, List<RGBColor>> animDict =
                new Dictionary<Coordinate, List<RGBColor>>();
            int longestAnim = 0;

            /* Create blinking animation for each LED in the coords. */
            for (int i = 0; i < coords.Count; i++)
            {
                blinkAnimation = BlinkLED(coords[i], colors[i], rates[i], numBlinks[i]);
                animDict.Add(coords[i], blinkAnimation);
                longestAnim = Math.Max(longestAnim, blinkAnimation.Count);
            }

            int index;
            RGBColor color;

            /* For each animation frame, update the behavior of each LED in coords. */
            for (int i = 0; i < longestAnim; i++)
            {
                foreach (KeyValuePair<Coordinate, List<RGBColor>> entry in animDict)
                {
                    if (i < entry.Value.Count)
                    {
                        index = IndexFromCoord(entry.Key);
                        color = entry.Value[i];
                        colorArray[index] = color.R;
                        colorArray[index + 1] = color.G;
                        colorArray[index + 2] = color.B;
                    }
                }
                AddImageFrame(imageFrames);
            }
        }

        ////////////////////// PRIVATE IMPLEMENTATION //////////////////////

        //////////////////////  BASIC HELPER METHODS //////////////////////

        /* Change color of a single LED at the position specified, with option to blend.*/
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

        /* Method to obtain an index from a coordinate. */
        public int IndexFromCoord(Coordinate c)
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

        /* Adds an image frame to the image frame queue. Every
         * frame of animation that you want sent to the cube 
         * must be added to imageFrames using this method, or it
         * will not be sent to the cube.
         */
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

        /* Fade a single LED from one color to another, using Linear
         * Interpolation.
         * 
         * c is the coordinate of the LED we want to change.
         * endColor is the final color we want the LED to be.
         * Rate represents how fast it should fade from one color to
         * another, and is given as the number of frames the fading
         * should span across.
         * 
         * Returns a list of colors of length rate, which represents the
         * color the LED will be at each of these frames.
         */
        private List<RGBColor> fadeLED(Coordinate c, RGBColor endColor, int rate)
        {
            List<RGBColor> fadeAnimation = new List<RGBColor>();
            //Get old RGB value at c.
            int index = IndexFromCoord(c);

            int oldR = colorArray[index];
            int oldG = colorArray[index + 1];
            int oldB = colorArray[index + 2];

            //Calculate difference between new color and old color.
            int rDiff = endColor.R - oldR;
            int gDiff = endColor.G - oldG;
            int bDiff = endColor.B - oldB;


            //Calculate increment for each frame
            double increment = 1.0 / ((double)rate);

            byte newR = 0;
            byte newG = 0;
            byte newB = 0;

            for (int i = 0; i < rate; i++)
            {
                newR = (byte)(oldR + (increment * (i + 1)) * rDiff);
                newG = (byte)(oldG + (increment * (i + 1)) * gDiff);
                newB = (byte)(oldB + (increment * (i + 1)) * bDiff);

                fadeAnimation.Add(new RGBColor(newR, newG, newB));
            }

            return fadeAnimation;
        }

        /* Blink a specific LED at Coordinate c, with speed of blink
         * determined by rate. (rate = the number of frames the LED
         * should be lighted up in the blink)
         */
        public List<RGBColor> BlinkLED(Coordinate c, RGBColor color, int rate, int numBlinks)
        {
            List<RGBColor> colors = new List<RGBColor>();
            RGBColor black = new RGBColor(0, 0, 0);

            for (int j = 0; j < numBlinks; j++)
            {
                for (int i = 0; i < rate * 2; i++)
                {
                    if (i < rate)
                    {
                        colors.Add(color);
                    }
                    else
                    {
                        colors.Add(black);
                    }
                }
            }

            return colors;
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

        /* Light up a rectangular sub-block of the cube in a specific color.
         *
         * The rectangular block is specified by two coordinates, which are the corners which
         * are diagonally across from each other in the block.
         * 
         * TODO: Light up whole cube could be seen as LightBlock from (0,0,0) to (7,7,7)
         *       Make this change for better implementation code reuse, but still provide
         *       changeFullColor in code interface for user use.
         */
        public void LightBlock(List<byte[]> imageFrames, Coordinate c1, Coordinate c2, RGBColor color)
        {
            int xMax = Math.Max(c1.X, c2.X);
            int xMin = Math.Min(c1.X, c2.X);

            int yMax = Math.Max(c1.Y, c2.Y);
            int yMin = Math.Min(c1.Y, c2.Y);

            int zMax = Math.Max(c1.Z, c2.Z);
            int zMin = Math.Min(c1.Z, c2.Z);

            for(int x = xMin; x <= xMax; x++)
            {
                for(int y = yMin; y <= yMax; y++)
                {
                    for(int z = zMin; z <= zMax; z++)
                    {
                        changeColorLED(new Coordinate(x,y,z), color, false);
                    }
                }
            }

            AddImageFrame(imageFrames);
        }

        /* 
         * Makes the colors of the cube be shifted so that the d-coordinates
         * decrease by one.
         */
        private void ShiftOnceDecreasing(List<byte[]> imageFrames, Direction d)
        {
            /* Depending on d, one cross-section of the cube will be 
             * effectively "pushed off" the cube as the color data is
             * shifted by one. This switch statement is necessary to 
             * determine correct pushing off behavior in the for loops
             * below.
             */
            int xLowerBound = 0;
            int yLowerBound = 0;
            int zLowerBound = 0;

            switch (d)
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

            for (int x = xLowerBound; x < 8; x++)
            {
                for (int y = yLowerBound; y < 8; y++)
                {
                    for (int z = zLowerBound; z < 8; z++)
                    {
                        //Get color of initial coordinate
                        RGBColor color = ColorFromCoord(new Coordinate(x, y, z));
                        switch (d)
                        {
                            //Put whatever color was there into the space one d-value away from initial coordinate.
                            case Direction.X:
                                {
                                    changeColorLED(new Coordinate((x - 1), y, z), color, false);
                                    break;
                                }
                            case Direction.Y:
                                {
                                    changeColorLED(new Coordinate(x, (y - 1), z), color, false);
                                    break;
                                }
                            case Direction.Z:
                                {
                                    changeColorLED(new Coordinate(x, y, (z - 1)), color, false);
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

            /* Since the cube shifted, one cross-section has been shifted off the cube, and an
             * empty cross section has been added on to the other end. For now, fill with blank vals.
             * 
             * TODO: Make it so this behavior can be easily changed
             */
            LightCrossSection(imageFrames, new RGBColor(50, 50, 50), new Coordinate(7, 7, 7), d, false);

            AddImageFrame(imageFrames);

        }

        /* 
         * Makes the colors of the cube be shifted so that the d-coordinates
         * increase by one.
         */
        private void ShiftOnceIncreasing(List<byte[]> imageFrames, Direction d)
        {
            /* Depending on d, one cross-section of the cube will be 
             * effectively "pushed off" the cube as the color data is
             * shifted by one. This switch statement is necessary to 
             * determine correct pushing off behavior in the for loops
             * below.
             */
            int xLowerBound = 0;
            int yLowerBound = 0;
            int zLowerBound = 0;

            switch (d)
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

            for (int x = 7; xLowerBound <= x; x--)
            {
                for (int y = 7; yLowerBound <= y; y--)
                {
                    for (int z = 7; zLowerBound <= z; z--)
                    {
                        //Get color of coordinate one away from desired coordinate.
                        RGBColor color;

                        switch (d)
                        {
                            case Direction.X:
                                {
                                    color = ColorFromCoord(new Coordinate((x - 1), y, z));
                                    break;
                                }
                            case Direction.Y:
                                {
                                    color = ColorFromCoord(new Coordinate(x, (y - 1), z));
                                    break;
                                }
                            case Direction.Z:
                                {
                                    color = ColorFromCoord(new Coordinate(x, y, (z - 1)));
                                    break;
                                }
                            default:
                                {
                                    //Do nothing
                                    color = new RGBColor(0, 0, 0);
                                    break;
                                }
                        }

                        //Replace this coordinate with prev coordinate's color
                        changeColorLED(new Coordinate(x, y, z), color, false);
                    }
                }
            }

            /* Since the cube shifted, one cross-section has been shifted off the cube, and an
             * empty cross section has been added on to the other end. For now, fill with blank vals.
             * 
             * TODO: Make it so this behavior can be easily changed
             */
            LightCrossSection(imageFrames, new RGBColor(50, 50, 50), new Coordinate(0, 0, 0), d, false);

            AddImageFrame(imageFrames);
        }
    }
}
