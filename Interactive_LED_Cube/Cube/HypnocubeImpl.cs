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

        /* Change the color of a single LED in a particular imageFrame of the animation. */
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
            if (index < 0)
            {
                Console.WriteLine(coord.ToString());
            }

            imageFrame[index] = newColor.R;
            imageFrame[index + 1] = newColor.G;
            imageFrame[index + 2] = newColor.B;

            return imageFrame;
        }

        /* Change the color of a single LED in multiple imageFrames of the animation. */
        public List<byte[]> changeColorLEDImages(List<byte[]> imageFrames, int imageIndex, Coordinate coord, RGBColor color)
        {
            int index = IndexFromCoord(coord);

            for (int i = imageIndex; i < imageFrames.Count; i++)
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
            if (physical)
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

        /* Obtain the color of a speicific coordinate in a particular imageFrame. */
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
         * This is an old method. Use a ColorFiller LightingMethod instead.
         * Don't use this.
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
            //List<byte[]> imageFrames = new List<byte[]>();
            //AddImageFrame(imageFrames);
        }

        /* Changes the whole cube to a random color.
         * 
         * This is an old method. Use a ColorFiller LightingMethod implementation
         * instead. Don't use this.
         */
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
                        coords.Add(new Coordinate(x, y, z));
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



        ///////////////////// OFFICIAL ANIMATIONS /////////////////////////

        /* Fade the cube using cp, starting from one corner and working to 
         * the other. */
        public void Fade(List<byte[]> imageFrames, ColorPalette cp)
        {
            List<Coordinate> coords = new List<Coordinate>();
            List<RGBColor> colors = new List<RGBColor>();
            List<int> rates = new List<int>();

            LightingMethod fader = new Fader(this);

            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    for (int k = 0; k < 8; k++)
                    {
                        coords.Add(new Coordinate(i, j, k));
                        colors.Add(cp.MapCoordToColor(new Coordinate(i, j, k)));
                        if (i == 0 && j == 0 && k == 0)
                        {
                            rates.Add(5);
                        }
                        else
                        {
                            rates.Add((i + j + k) * 10);
                        }
                    }
                }
            }

            LightLEDs(imageFrames, coords, colors, rates, fader, false);
        }

        /* Makes the cube blink in a fancy pattern. */
        public void Blink(List<byte[]> imageFrames, ColorPalette cp)
        {
            List<Coordinate> coords = new List<Coordinate>();
            List<RGBColor> colors = new List<RGBColor>();
            List<int> rates = new List<int>();
            List<int> numBlinks = new List<int>();

            RGBColor red = new RGBColor(255, 0, 0);

            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    for (int k = 0; k < 8; k++)
                    {
                        coords.Add(new Coordinate(i, j, k));
                        colors.Add(cp.MapCoordToColor(new Coordinate(i, j, k)));
                        rates.Add((i + j + k));
                        numBlinks.Add(i + j + k);
                    }
                }
            }
            LightingMethod blinker = new Blinker(this, numBlinks);
            LightLEDs(imageFrames, coords, colors, rates, blinker, false);
        }

        /* Fills the cube by filling one LED at a time, which is in a random position. 
         * 
         * If rand is true, will generate random color for each LED. Otherwise, will fill
         * with the specified color. 
         * 
         * Rate determines how fast the block will fill up. Should be a number that divides into 512.
         */
        public void RandomFill(List<byte[]> imageFrames, RGBColor color, bool rand, int rate, ColorPalette cp)
        {
            List<Coordinate> coords = new List<Coordinate>();

            for (int x = 0; x < 8; x++)
            {
                for (int y = 0; y < 8; y++)
                {
                    for (int z = 0; z < 8; z++)
                    {
                        coords.Add(new Coordinate(x, y, z));
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
                for (int i = 0; i < rate; i++)
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
                        changeColorLED(coords[randIndex], cp.MapCoordToColor(coords[randIndex]), false);
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

            //ColorFiller cf = new ColorFiller(color);
            Fader f = new Fader(this);

            switch (d)
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
        }

        /* Makes a miniature cube expand and contract. (1 cycle) */
        public void ExpandingSolidCube(List<byte[]> imageFrames, bool rand, int rate, LightingMethod lm, ColorPalette cp)
        {
            ColorPalette blackPalette = new SolidPalette(new RGBColor(0, 0, 0));
            for (int i = 0; i < 4; i++)
            {
                //Fade in
                lm.LightBlockUniform(imageFrames, new Coordinate(i, i, i), new Coordinate(7 - i, 7 - i, 7 - i), rate, false, cp);

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

        /* Make the cube fade in and fade out, simple blink of color. */
        public void SimpleLight(List<byte[]> imageFrames, bool rand, int rate, LightingMethod lm, ColorPalette cp)
        {
            ColorPalette blackPalette = new SolidPalette(new RGBColor(0, 0, 0));
            lm.LightBlockUniform(imageFrames, new Coordinate(0,0,0), new Coordinate(7,7,7), rate, false, cp);
            lm.LightBlockUniform(imageFrames, new Coordinate(0,0,0), new Coordinate(7,7,7), rate, false, blackPalette);
        }

        /* A little roamer, one LED which moves around randomly leaving a trail behind it if resetFrames is false. */
        public void LittleRoamer(List<byte[]> imageFrames, int rate, LightingMethod lm, bool resetFrames, ColorPalette cp,
            int duration)
        {
            Random rand = new Random();

            int startX = rand.Next(0, 8);
            int startY = rand.Next(0, 8);
            int startZ = rand.Next(0, 8);

            Coordinate c = new Coordinate(startX, startY, startZ);

            int nextDirection;

            for (int i = 0; i < duration; i++)
            {
                nextDirection = rand.Next(0, 6);

                switch (nextDirection)
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

        /* A roamer which can be any rectangular shape as indicated by the two coordinates. */
        public void Roamer(List<byte[]> imageFrames, int rate, LightingMethod lm, bool resetFrames, ColorPalette cp,
            Coordinate c1, Coordinate c2, int duration)
        {
            Random rand = new Random();

            int startX = rand.Next(0, 8);
            int startY = rand.Next(0, 8);
            int startZ = rand.Next(0, 8);

            //Coordinate c = new Coordinate(startX, startY, startZ);

            int xDiff = c1.X - c2.X;
            int yDiff = c1.Y - c2.Y;
            int zDiff = c1.Z - c2.Z;

            int nextDirection;

            for (int i = 0; i < duration; i++)
            {
                nextDirection = rand.Next(0, 6);

                switch (nextDirection)
                {
                    case 0:
                        {
                            //Increase x
                            int x1 = c1.IncDec(c1.X, 1);
                            int x2 = c2.IncDec(c2.X, 1);
                            if (!(c1.X == x1 || c2.X == x2))
                            {
                                c1.X = x1;
                                c2.X = x2;
                            }
                            break;
                        }
                    case 1:
                        {
                            //Decrease x
                            int x1 = c1.IncDec(c1.X, -1);
                            int x2 = c2.IncDec(c2.X, -1);
                            if (!(c1.X == x1 || c2.X == x2))
                            {
                                c1.X = x1;
                                c2.X = x2;
                            }
                            break;
                        }
                    case 2:
                        {
                            //Increase y
                            int y1 = c1.IncDec(c1.Y, 1);
                            int y2 = c2.IncDec(c2.Y, 1);
                            if (!(c1.Y == y1 || c2.Y == y2))
                            {
                                c1.Y = y1;
                                c2.Y = y2;
                            }
                            break;
                        }
                    case 3:
                        {
                            //Decrease y
                            int y1 = c1.IncDec(c1.Y, -1);
                            int y2 = c2.IncDec(c2.Y, -1);
                            if (!(c1.Y == y1 || c2.Y == y2))
                            {
                                c1.Y = y1;
                                c2.Y = y2;
                            }
                            break;
                        }
                    case 4:
                        {
                            //Increase z
                            int z1 = c1.IncDec(c1.Z, 1);
                            int z2 = c2.IncDec(c2.Z, 1);
                            if (!(c1.Z == z1 || c2.Z == z2))
                            {
                                c1.Z = z1;
                                c2.Z = z2;
                            }
                            break;
                        }
                    case 5:
                        {
                            //Decrease z
                            int z1 = c1.IncDec(c1.Z, -1);
                            int z2 = c2.IncDec(c2.Z, -1);
                            if (!(c1.Z == z1 || c2.Z == z2))
                            {
                                c1.Z = z1;
                                c2.Z = z2;
                            }
                            break;
                        }
                    default:
                        {
                            break;
                        }
                }

                lm.LightBlockUniform(imageFrames, c1, c2, rate, resetFrames, cp);

            }
            ColorPalette blackPalette = new SolidPalette(new RGBColor(0, 0, 0));
            lm.LightBlockUniform(imageFrames, new Coordinate(0, 0, 0), new Coordinate(7, 7, 7), 4, false, blackPalette);
        }

        /* Method for making many little roamers as described above. LittleRoamer is now obsolete since you can just
         * make the count here be 1 for a single little roamer.
         */
        public void ManyLittleRoamers(List<byte[]> imageFrames, int rate, LightingMethod lm, bool resetFrames, ColorPalette cp, int count,
            int duration)
        {
            Random rand = new Random();

            List<Coordinate> coords = new List<Coordinate>();
            List<int> directions = new List<int>();

            int startX;
            int startY;
            int startZ; 
            for(int i = 0; i < count; i++)
            {
                startX = rand.Next(0, 8);
                startY = rand.Next(0, 8);
                startZ = rand.Next(0, 8);

                coords.Add(new Coordinate(startX, startY, startZ));    
            }

            for (int i = 0; i < duration; i++)
            {
                for(int j = 0; j < count; j++)
                {
                    directions.Add(rand.Next(0, 6));
                    Shift(directions[j], coords[j]);
                    
                }

                directions = new List<int>();

                lm.LightManyBlocksUniform(imageFrames, coords, rate, resetFrames, cp);
            }

            ColorPalette blackPalette = new SolidPalette(new RGBColor(0, 0, 0));
            lm.LightBlockUniform(imageFrames, new Coordinate(0,0,0), new Coordinate(7,7,7), 4, false, blackPalette);
        }

        /* Helper method used in the roamer methods to shift a coordinate in a certain direction. */
        public void Shift(int nextDirection, Coordinate c1)
        {
            switch (nextDirection)
            {
                case 0:
                    {
                        //Increase x
                        c1.X = c1.IncDec(c1.X, 1);
                        break;
                    }
                case 1:
                    {
                        //Decrease x
                        c1.X = c1.IncDec(c1.X, -1);
                        break;
                    }
                case 2:
                    {
                        //Increase y
                        c1.Y = c1.IncDec(c1.Y, 1);
                        break;
                    }
                case 3:
                    {
                        //Decrease y
                        c1.Y = c1.IncDec(c1.Y, -1);
                        break;
                    }
                case 4:
                    {
                        //Increase z
                        c1.Z = c1.IncDec(c1.Z, 1);
                        break;
                    }
                case 5:
                    {
                        //Decrease z
                        c1.Z = c1.IncDec(c1.Z, -1);
                        break;
                    }
                default:
                    {
                        break;
                    }
            }
        }
    
    }
}
