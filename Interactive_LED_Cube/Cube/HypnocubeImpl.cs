using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interactive_LED_Cube
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

        //True if using physical cube, false if using OpenGL visualization.
        private bool physical;

        public enum Direction { X, Y, Z };

        /* Constructor */
        public HypnocubeImpl(bool physical)
        {
            colorArray = new byte[ARRAY_SIZE];
            this.physical = physical;
        }

        /* Getter/setter for image */
        public byte[] ColorArray
        {
            get { return colorArray; }
            set { colorArray = value; }
        }

        /////////////////////////// INITIAL TEST ////////////////////////////

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




        //////////////////////  BASIC HELPER METHODS //////////////////////

        /* Change color of a single LED at the position specified, with option to blend.*/
        public byte[] changeColorLED(Coordinate coord, RGBColor color, bool blend)
        {
            int index = IndexFromCoord(coord);

            RGBColor newColor = color;
            if (blend == true)
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

        public byte[] changeColorLEDImage(byte[] imageFrame, Coordinate coord, RGBColor color)
        {
            int index = IndexFromCoord(coord);

            RGBColor newColor = color;
            /*
            if (blend == true)
            {
                RGBColor existingColor =
                    new RGBColor(colorArray[index], colorArray[index + 1], colorArray[index + 2]);
                newColor = Blend(existingColor, color);
            }*/
            if(index < 0)
            {
                Console.WriteLine(coord.ToString());
            }

            imageFrame[index] = newColor.R;
            imageFrame[index + 1] = newColor.G;
            imageFrame[index + 2] = newColor.B;

            return imageFrame;
        }

        public List<byte[]> changeColorLEDImages(List<byte[]> imageFrames, int imageIndex, Coordinate coord, RGBColor color)
        {
            int index = IndexFromCoord(coord);

            for (int i = imageIndex; i < imageFrames.Count; i++ )
            {
                imageFrames[i][index] = color.R;
                imageFrames[i][index + 1] = color.G;
                imageFrames[i][index + 2] = color.B;
            }
            return imageFrames;
        }

        /* Method to obtain an index from a coordinate. */
        public int IndexFromCoord(Coordinate c)
        {
            /* Coordinates of visualization and actual cube are a  bit different,
             * need to test what mode we're in and return index accordingly. */
            if(physical)
            {
                //Using physical cube.
                return (c.Z + 8 * c.X + 64 * c.Y) * 3;
            }
            else
            {
                //Using OpenGL visualization
                return (c.X + 8 * c.Y + 64 * c.Z) * 3;
            }
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

       private RGBColor ImageColorFromCoord(byte[] imageFrame, Coordinate c)
        {
            int index = IndexFromCoord(c);
            byte red = imageFrame[index];
            byte green = imageFrame[index + 1];
            byte blue = imageFrame[index + 2];

            return new RGBColor(red, green, blue);
        }

        /* Takes the average of two bytes, used in blending. */
        private byte AverageBytes(byte b1, byte b2)
        {
            return (byte)((b1 + b2) / 2);
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
        public void AddImageFrame(List<byte[]> imageFrames)
        {
            byte[] newImage = new byte[ColorArray.Length];
            Array.Copy(ColorArray, newImage, ColorArray.Length);
            imageFrames.Add(newImage);
        }

        /* Given a list of frames to repeat and an int count which determines the number of
         * times we repeat, repeat an animation.
         */
        public void RepeatFrames(List<byte[]> framesToRepeat, List<byte[]> imageFrames, int count)
        {
            for (int i = 0; i < count; i++)
            {
                foreach (byte[] frame in framesToRepeat)
                {
                    imageFrames.Add(frame);
                }
            }
        }




        //////////////////////// MORE INVOLVED HELPERS //////////////////////////

        /* Changes the whole cube to the specified color. 
         *
         * TODO: Change to use LightBlock in implementation
         */
        public void SpecificColorWholeCube(RGBColor color, bool blend)
        {
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    for (int k = 0; k < 8; k++)
                    {
                        changeColorLED(new Coordinate(i, j, k), color, blend);
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

        /* Given two coordinates, generates a list of all the coordinates in the block
         * delimited by the coordinates.
         */
        public List<Coordinate> GenerateCoordBlock(Coordinate c1, Coordinate c2)
        {
            List<Coordinate> coords = new List<Coordinate>();

            int xMax = Math.Max(c1.X, c2.X);
            int xMin = Math.Min(c1.X, c2.X);

            int yMax = Math.Max(c1.Y, c2.Y);
            int yMin = Math.Min(c1.Y, c2.Y);

            int zMax = Math.Max(c1.Z, c2.Z);
            int zMin = Math.Min(c1.Z, c2.Z);

            for (int x = xMin; x <= xMax; x++)
            {
                for (int y = yMin; y <= yMax; y++)
                {
                    for (int z = zMin; z <= zMax; z++)
                    {
                        coords.Add(new Coordinate(x,y,z));
                    }
                }
            }

            return coords;
        }

        /* Generate an animation for the specified LEDs in coords, using the colors and
         * rates specified. Animation behavior is determined by the LightingMethod.
         */
        public void LightLEDs(List<byte[]> imageFrames, List<Coordinate> coords,
            List<RGBColor> colors, List<int> rates, LightingMethod lm, bool resetFrames)
        {
            Dictionary<Coordinate, List<RGBColor>> animDict = lm.CreateAnimation(coords, colors, rates, resetFrames);
            lm.CreateFrames(imageFrames, animDict, lm.GetLongestAnim(), resetFrames);
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
        public void LightBlock(List<byte[]> imageFrames, Coordinate c1, Coordinate c2, 
            List<RGBColor> colors, List<int> rates, LightingMethod lm)
        {
            List<Coordinate> coords = GenerateCoordBlock(c1, c2);

            /*
            foreach(Coordinate c in coords)
            {
                changeColorLED(c, color, false);
            }
             * */

            LightLEDs(imageFrames, coords, colors, rates, lm, false);

            //AddImageFrame(imageFrames);
        }





        //////////////////////   WORK IN PROGRESS DOESN'T WORK PROPERLY ////////////////////////

        /* Shift the colors of the cube by one in a specific direction.
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
        public Tuple<Coordinate, Coordinate> ShiftOnce(List<byte[]> imageFrames, int imageIndex,
            Direction d, bool decreasing, Coordinate c1, Coordinate c2, RGBColor eraseColor)
        {
            if (decreasing)
            {
                //ShiftOnceDecreasing(imageFrames, d);
                return ShiftBlockOnceDecreasing(imageFrames, imageIndex, d, c1, c2, eraseColor);
            }
            else
            {
                return ShiftBlockOnceIncreasing(imageFrames, imageIndex, d, c1, c2, eraseColor);
            }
        }

        /* Iterate ShiftOnce numShift times so that the block shifts multiple times.
         */
        public Tuple<Coordinate, Coordinate> ShiftAlongCube(List<byte[]> imageFrames, int imageIndex,
            Direction d, bool decreasing, Coordinate c1, Coordinate c2, int speedIncr, int numShift,
            RGBColor eraseColor)
        {
            Tuple<Coordinate, Coordinate> coords = new Tuple<Coordinate, Coordinate>(c1, c2);

            for (int i = 0; i < numShift; i++)
            {
                if (coords.Item1 == null && coords.Item2 == null)
                {
                    break;
                }
                coords = ShiftOnce(imageFrames, imageIndex, d, decreasing, coords.Item1, coords.Item2, eraseColor);

                Console.WriteLine("coord 1: " + coords.Item1.ToString() + "coord 2: " + coords.Item2.ToString());
                imageIndex = imageIndex + speedIncr;
            }
            return coords;
        }

        /* Shift a block of coordinates as determined by c1 and c2, in a direction d, filling in the 
         * empty space that is left by the shifting with eraseColor. Shifts the block in the imageFrame array
         * at index imageIndex.
         * 
         * TODO: If time, make this less hacky and cut & paste-y.
         */
        public Tuple<Coordinate, Coordinate> ShiftBlockOnceDecreasing(List<byte[]> imageFrames, int imageIndex, 
            Direction d, Coordinate c1, Coordinate c2, RGBColor eraseColor)
        {

            int xMax = Math.Max(c1.X, c2.X);
            int xMin = Math.Min(c1.X, c2.X);

            int yMax = Math.Max(c1.Y, c2.Y);
            int yMin = Math.Min(c1.Y, c2.Y);

            int zMax = Math.Max(c1.Z, c2.Z);
            int zMin = Math.Min(c1.Z, c2.Z);

            /* Depending on the direction we are shifting, the indices
             * of the nested for loops below this will need to be changed.
             * This switch statement takes care of this. It also determines 
             * the coordinates that indicate where the block was shifted to.
             */
            switch (d)
            {
                case Direction.X:
                    {
                        if(xMin == 0)
                        {
                            xMin = 1;
                        }

                        if(c1.X == 0 && c2.X == 0)
                        {
                            EraseLEDs(imageFrames, imageIndex, eraseColor, d, 0, xMax, yMin, yMax, zMin, zMax, true);
                            return new Tuple<Coordinate, Coordinate>(null, null);
                        }

                        if (c1.X == 0) { break; }
                        else { c1.X = c1.X - 1; }

                        if (c2.X == 0) { break; }
                        else { c2.X = c2.X - 1; }

                        break;
                    }
                case Direction.Y:
                    {
                        if(yMin == 0)
                        {
                            yMin = 1;
                        }

                        if (c1.Y == 0 && c2.Y == 0)
                        {
                            EraseLEDs(imageFrames, imageIndex, eraseColor, d, xMin, xMax, 0, yMax, zMin, zMax, true);
                            return new Tuple<Coordinate, Coordinate>(null, null);
                        }

                        if (c1.Y == 0) { break; }
                        else { c1.Y = c1.Y - 1; }

                        if (c2.Y == 0) { break; }
                        else { c2.Y = c2.Y - 1; }
                        break;
                    }
                case Direction.Z:
                    {
                        if(zMin == 0)
                        {
                            zMin = 1;
                        }

                        if (c1.Z == 0 && c2.Z == 0)
                        {
                            EraseLEDs(imageFrames, imageIndex, eraseColor, d, 0, xMax, yMin, yMax, 0, zMax, true);
                            return new Tuple<Coordinate, Coordinate>(null, null);
                        }

                        if (c1.Z == 0) { break; }
                        else { c1.Z = c1.Z - 1; }

                        if (c2.Z == 0) { break; }
                        else { c2.Z = c2.Z - 1; }
                        break;
                    }
            }


            for (int i = imageIndex; i < imageFrames.Count; i++)
            {
                for (int x = xMin; x <= xMax; x++)
                {
                    for (int y = yMin; y <= yMax; y++)
                    {
                        for (int z = zMin; z <= zMax; z++)
                        {
                            //Get color of initial coordinate
                            RGBColor color = ImageColorFromCoord(imageFrames[i], new Coordinate(x, y, z));
                            switch (d)
                            {
                                //Put whatever color was there into the space one d-value away from initial coordinate.
                                case Direction.X:
                                    {
                                        changeColorLEDImage(imageFrames[i], new Coordinate((x - 1), y, z), color);
                                        break;
                                    }
                                case Direction.Y:
                                    {
                                        changeColorLEDImage(imageFrames[i], new Coordinate(x, (y - 1), z), color);
                                        break;
                                    }
                                case Direction.Z:
                                    {
                                        changeColorLEDImage(imageFrames[i], new Coordinate(x, y, (z - 1)), color);
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
            }

            /* Erase the part of the block that was shifted.
             * 
             * TODO: Make it so this behavior can be easily changed
             */
            EraseLEDs(imageFrames, imageIndex, eraseColor, d, xMin, xMax, yMin, yMax, zMin, zMax, true);

            return Tuple.Create<Coordinate, Coordinate>(c1, c2);
        }

        public Tuple<Coordinate, Coordinate> ShiftBlockOnceIncreasing(List<byte[]> imageFrames, int imageIndex,
            Direction d, Coordinate c1, Coordinate c2, RGBColor eraseColor)
        {
            int xMax = Math.Max(c1.X, c2.X);
            int xMin = Math.Min(c1.X, c2.X);

            int yMax = Math.Max(c1.Y, c2.Y);
            int yMin = Math.Min(c1.Y, c2.Y);

            int zMax = Math.Max(c1.Z, c2.Z);
            int zMin = Math.Min(c1.Z, c2.Z);

            /* Depending on the direction we are shifting, the indices
             * of the nested for loops below this will need to be changed.
             * This switch statement takes care of this. It also determines 
             * the coordinates that indicate where the block was shifted to.
             * 
             */
            switch (d)
            {
                case Direction.X:
                    {
                        if (xMax == 7)
                        {
                            xMax = 6;
                        }

                        if (c1.X == 7 && c2.X == 7)
                        {
                            EraseLEDs(imageFrames, imageIndex, eraseColor, d, xMin, xMax, yMin, yMax, zMin, zMax, false);
                            return new Tuple<Coordinate, Coordinate>(null, null);
                        }

                        if (c1.X == 7) { break; }
                        else { c1.X = c1.X + 1; }

                        if (c2.X == 7) { break; }
                        else { c2.X = c2.X + 1; }

                        break;
                    }
                case Direction.Y:
                    {
                        if (yMax == 7)
                        {
                            yMax = 6;
                        }

                        if (c1.Y == 7 && c2.Y == 7)
                        {
                            EraseLEDs(imageFrames, imageIndex, eraseColor, d, xMin, xMax, yMin, yMax, zMin, zMax, false);
                            return new Tuple<Coordinate, Coordinate>(null, null);
                        }

                        if (c1.Y == 7) { break; }
                        else { c1.Y = c1.Y + 1; }

                        if (c2.Y == 7) { break; }
                        else { c2.Y = c2.Y + 1; }
                        break;
                    }
                case Direction.Z:
                    {
                        if (zMax == 7)
                        {
                            zMax = 6;
                        }

                        if (c1.Z == 7 && c2.Z == 7)
                        {
                            EraseLEDs(imageFrames, imageIndex, eraseColor, d, xMin, xMax, yMin, yMax, zMin, zMax, false);
                            return new Tuple<Coordinate, Coordinate>(null, null);
                        }

                        if (c1.Z == 7) { break; }
                        else { c1.Z = c1.Z + 1; }

                        if (c2.Z == 7) { break; }
                        else { c2.Z = c2.Z + 1; }
                        break;
                    }
            }


            for (int i = imageIndex; i < imageFrames.Count; i++)
            {
                for (int x = xMax; xMin <= x; x--)
                {
                    for (int y = yMax; yMin <= y; y--)
                    {
                        for (int z = zMax; xMin <= z; z--)
                        {
                            //Get color of initial coordinate
                            RGBColor color = ImageColorFromCoord(imageFrames[i], new Coordinate(x, y, z));
                            switch (d)
                            {
                                //Put whatever color was there into the space one d-value away from initial coordinate.
                                case Direction.X:
                                    {
                                        changeColorLEDImage(imageFrames[i], new Coordinate((x + 1), y, z), color);
                                        break;
                                    }
                                case Direction.Y:
                                    {
                                        changeColorLEDImage(imageFrames[i], new Coordinate(x, (y + 1), z), color);
                                        break;
                                    }
                                case Direction.Z:
                                    {
                                        changeColorLEDImage(imageFrames[i], new Coordinate(x, y, (z + 1)), color);
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
            }

            /* Erase the part of the block that was shifted.
             * 
             * TODO: Make it so this behavior can be easily changed
             */
            EraseLEDs(imageFrames, imageIndex, eraseColor, d, xMin, xMax, yMin, yMax, zMin, zMax, false);

            return Tuple.Create<Coordinate, Coordinate>(c1, c2);
        }

        /* Used when shifting blocks, fills the part of the block that was shifted with an erase color. */
        private void EraseLEDs(List<byte[]> imageFrames, int imageIndex, RGBColor eraseColor, Direction d, 
            int xMin, int xMax, int yMin, int yMax, int zMin, int zMax, bool decreasing)
        {
            int value = 0;

            switch (d)
            {
                case Direction.X:
                    {
                        if (decreasing) { value = xMax; }
                        else { value = xMin; }

                        for (int y = yMin; y <= yMax; y++)
                        {
                            for (int z = zMin; z <= zMax; z++)
                            {
                                changeColorLEDImages(imageFrames, imageIndex, new Coordinate(value, y, z), eraseColor);
                            }
                        }
                        break;
                    }
                case Direction.Y:
                    {
                        if (decreasing) { value = yMax; }
                        else { value = yMin; }

                        for (int x = xMin; x <= xMax; x++)
                        {
                            for (int z = zMin; z <= zMax; z++)
                            {
                                changeColorLEDImages(imageFrames, imageIndex, new Coordinate(x, value, z), eraseColor);
                            }
                        }
                        break;
                    }
                case Direction.Z:
                    {
                        if (decreasing) { value = zMax; }
                        else { value = zMin; }

                        for (int x = xMin; x <= xMax; x++)
                        {
                            for (int y = yMin; y <= yMax; y++)
                            {
                                changeColorLEDImages(imageFrames, imageIndex, new Coordinate(x, y, value), eraseColor);
                            }
                        }
                        break;
                    }
            }
        }

        /* Makes the colors of the cube be shifted so that the d-coordinates
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

        /* TODO: Get rid of this function.
         * 
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

        /* ugh */
        public List<byte[]> ConsolidateFrames(List<byte[]> frames1, List<byte[]> frames2, Coordinate c1, Coordinate c2,
            int offset)
        {
            List<Coordinate> coords = GenerateCoordBlock(c1, c2);
            List<byte[]> combinedFrames = new List<byte[]>();
            byte[] frame;

            int index = 0;

            for (int i = 0; i < frames2.Count - offset; i++)
            {
                foreach (Coordinate c in coords)
                {
                    frame = changeColorLEDImage(frames2[i + offset], c, ImageColorFromCoord(frames1[i], c));
                    combinedFrames.Add(frame);
                }
                index++;
            }

            byte[] finalFrame = frames2[frames2.Count - 1];

            for (int i = index; i < frames1.Count - index; i++)
            {
                foreach (Coordinate c in coords)
                {
                    frame = changeColorLEDImage(finalFrame, c, ImageColorFromCoord(frames1[i], c));
                    combinedFrames.Add(frame);
                }
            }

            return combinedFrames;

        }

        ///////////////////// OFFICIAL ANIMATIONS /////////////////////////

        /* Fills the cube by filling one LED at a time, which is in a random position. 
         * 
         * If rand is true, will generate random color for each LED. Otherwise, will fill
         * with the specified color. 
         * 
         * Rate determines how fast the block will fill up. Should be a number that divides into 512.
         */
        public void RandomFill(List<byte[]> imageFrames, RGBColor color, bool rand, int rate)
        {
            List<Coordinate> coords = new List<Coordinate>();

            for(int x = 0; x < 8; x++)
            {
                for(int y = 0; y < 8; y++)
                {
                    for(int z = 0; z < 8; z++)
                    {
                        coords.Add(new Coordinate(x,y,z));
                    }
                }
            }

            Random r = new Random();
            int counter = coords.Count;
            int randIndex;

            byte R;
            byte G;
            byte B;

            RGBColor randColor;

            while (counter > 0)
            {
                for (int i = 0; i < rate; i++ )
                {
                    randIndex = r.Next(0, counter);

                    if (rand)
                    {
                        R = (byte)r.Next(0, 255);
                        G = (byte)r.Next(0, 255);
                        B = (byte)r.Next(0, 255);

                        randColor = new RGBColor(R, G, B);

                        changeColorLED(coords[randIndex], randColor, false);
                    }
                    else
                    {
                        changeColorLED(coords[randIndex], color, false);
                    }
                    coords.RemoveAt(randIndex);
                    counter--;
                }

                AddImageFrame(imageFrames);
            }
         
        }

        /* Zig-zag fills the cube with color, starting from bottom and zig-zagging to top. */
        public void ZigZagFill(List<byte[]> imageFrames, bool rand, int rate, Direction d, ColorPalette cp)
        {
            bool turnSignal = false;

            //ColorFiller cf = new ColorFiller(this);
            Fader f = new Fader(this);

            switch(d)
            {
                case Direction.X:
                    {
                        for (int x = 0; x < 8; x++)
                        {
                            if (turnSignal)
                            {
                                for (int y = 7; y >= 0; y--)
                                {
                                    //cf.LightBlockUniform(imageFrames, new Coordinate(x, y, 0), new Coordinate(x, y, 7), color, 1);
                                    f.LightBlockUniform(imageFrames, new Coordinate(x, y, 0), new Coordinate(x, y, 7), rate, false, cp);
                                    AddImageFrame(imageFrames);
                                }
                                turnSignal = false;
                            }
                            else
                            {
                                for (int y = 0; y < 8; y++)
                                {
                                    f.LightBlockUniform(imageFrames, new Coordinate(x, y, 0), new Coordinate(x, y, 7), rate, false, cp);
                                    AddImageFrame(imageFrames);
                                }
                                turnSignal = true;
                            }
                        }
                        break;
                    }
                case Direction.Y:
                    {
                        for (int y = 0; y < 8; y++)
                        {
                            if (turnSignal)
                            {
                                for (int x = 7; x >= 0; x--)
                                {
                                    //cf.LightBlockUniform(imageFrames, new Coordinate(x, y, 0), new Coordinate(x, y, 7), color, 1);
                                    f.LightBlockUniform(imageFrames, new Coordinate(x, y, 0), new Coordinate(x, y, 7), rate, false, cp);
                                    AddImageFrame(imageFrames);
                                }
                                turnSignal = false;
                            }
                            else
                            {
                                for (int x = 0; x < 8; x++)
                                {
                                    f.LightBlockUniform(imageFrames, new Coordinate(x, y, 0), new Coordinate(x, y, 7), rate, false, cp);
                                    AddImageFrame(imageFrames);
                                }
                                turnSignal = true;
                            }
                        }
                        break;
                    }
                case Direction.Z:
                    {
                        for (int z = 0; z < 8; z++)
                        {
                            if (turnSignal)
                            {
                                for (int y = 7; y >= 0; y--)
                                {
                                    //cf.LightBlockUniform(imageFrames, new Coordinate(x, y, 0), new Coordinate(x, y, 7), color, 1);
                                    f.LightBlockUniform(imageFrames, new Coordinate(0, y, z), new Coordinate(7, y, z), rate, false, cp);
                                    AddImageFrame(imageFrames);
                                }
                                turnSignal = false;
                            }
                            else
                            {
                                for (int y = 0; y < 8; y++)
                                {
                                    f.LightBlockUniform(imageFrames, new Coordinate(7, y, z), new Coordinate(0, y, z), rate, false, cp);
                                    AddImageFrame(imageFrames);
                                }
                                turnSignal = true;
                            }
                        }
                        break;
                    }
                default:
                    {
                        break;
                    }
            }

            /*
            for(int y = 0; y < 8; y++)
            {
                if(turnSignal)
                {
                    for(int x = 7; x >= 0; x--)
                    {
                        //cf.LightBlockUniform(imageFrames, new Coordinate(x, y, 0), new Coordinate(x, y, 7), color, 1);
                        f.LightBlockUniform(imageFrames, new Coordinate(x, y, 0), new Coordinate(x, y, 7), color, rate, false);
                        AddImageFrame(imageFrames);
                    }
                    turnSignal = false;
                }
                else
                {
                    for (int x = 0; x < 8; x++)
                    {
                        f.LightBlockUniform(imageFrames, new Coordinate(x, y, 0), new Coordinate(x, y, 7), color, rate, false);
                        AddImageFrame(imageFrames);
                    }
                    turnSignal = true;
                }
            }
             * */
        }
    
        /* Makes a miniature cube expand and contract. (1 cycle) */
        public void ExpandingSolidCube(List<byte[]> imageFrames, bool rand, int rate, LightingMethod lm, ColorPalette cp)
        {
            ColorPalette blackPalette = new SolidPalette(new RGBColor(0, 0, 0));
            for(int i = 0; i < 4; i++)
            {
                //Fade in
                lm.LightBlockUniform(imageFrames,new Coordinate(i,i,i), new Coordinate(7-i, 7-i, 7-i), rate, false, cp);

                //Fade out
                lm.LightBlockUniform(imageFrames, new Coordinate(i, i, i), new Coordinate(7 - i, 7 - i, 7 - i), rate, false, blackPalette);
            }
            //lm.LightBlockUniform(imageFrames, new Coordinate(0,0,0), new Coordinate(0,0,0), new RGBColor(0,0,0), rate, false);


            for (int i = 3; i >= 0; i--)
            {
                //Fade in
                lm.LightBlockUniform(imageFrames, new Coordinate(i, i, i), new Coordinate(7 - i, 7 - i, 7 - i), rate, true, cp);

                //Fade out
                lm.LightBlockUniform(imageFrames, new Coordinate(i, i, i), new Coordinate(7 - i, 7 - i, 7 - i), rate, false, blackPalette);
            }
        }

        /* A little roamer, one LED which moves around randomly leaving a trail behind it if resetFrames is false. */
        public void LittleRoamer(List<byte[]> imageFrames, int rate, LightingMethod lm, bool resetFrames, ColorPalette cp)
        {
            Random rand = new Random();

            int startX = rand.Next(0,8);
            int startY = rand.Next(0,8);
            int startZ = rand.Next(0,8);

            Coordinate c = new Coordinate(startX, startY, startZ);

            int nextDirection;

            for(int i = 0; i < 500; i++)
            {
                nextDirection = rand.Next(0, 6);

                switch(nextDirection)
                {
                    case 0:
                        {
                            //Increase x
                            c.X = c.IncDec(c.X, 1);
                            break;
                        }
                    case 1:
                        {
                            //Decrease x
                            c.X = c.IncDec(c.X, -1);
                            break;
                        }
                    case 2:
                        {
                            //Increase y
                            c.Y = c.IncDec(c.Y, 1);
                            break;
                        }
                    case 3:
                        {
                            //Decrease y
                            c.Y = c.IncDec(c.Y, -1);
                            break;
                        }
                    case 4:
                        {
                            //Increase z
                            c.Z = c.IncDec(c.Z, 1);
                            break;
                        }
                    case 5:
                        {
                            //Decrease z
                            c.Z = c.IncDec(c.Z, -1);
                            break;
                        }
                    default:
                        {
                            break;
                        }
                }

                //RGBColor newColor = cp.MapCoordToColor(c);

                lm.LightBlockUniform(imageFrames, c, c, rate, resetFrames, cp);
            }

        }

        public RGBColor MapRainbow1(Coordinate c)
        {
            int R = 256 - 32 * (c.X + 1);
            int G = 256 - 32 * (c.Y + 1);
            int B = 256 - 32 * (c.Z + 1);

            return new RGBColor((byte)R, (byte)G, (byte)B);
        }

    }
}
