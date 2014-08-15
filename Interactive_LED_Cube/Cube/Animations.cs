using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cube_Visualization;

namespace Interactive_LED_Cube
{
    /* Class which stores the different animations which are currently implemented for
     * the hypnocube. Call these functions to perform these animations on the cube.
     */
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

        /* Sends imageFrames to cube or to OpenGL visualization 
         * depending on weather physical is true or not.
         */
        private void sendFrames()
        {
            if (physical)
            {
                DataTransfer.SendImagesToCube(port, imageFrames);
                imageFrames = new List<byte[]>();
            }
            else
            {
                tl.ReceiveAndSendSignal(imageFrames);
                //imageFrames = new List<byte[]>();
            }
        }

        ////////////////////////* ANIMATION POOL *///////////////////////////

        /* Fade the cube using cp, starting from one corner and working to 
         * the other. */
        public void Fade(ColorPalette cp)
        {
            hc.SpecificColorWholeCube(black, false);

            List<Coordinate> coords = new List<Coordinate>();
            List<RGBColor> colors = new List<RGBColor>();
            List<int> rates = new List<int>();

            LightingMethod fader = new Fader(hc);

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

            hc.LightLEDs(imageFrames, coords, colors, rates, fader, false);

            sendFrames();
        }

        /* Makes the cube blink in a fancy pattern, starts with whole cube
         * and then recedes into the corner. */
        public void Blink(ColorPalette cp)
        {
            hc.SpecificColorWholeCube(black, false);

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

            LightingMethod blinker = new Blinker(hc, numBlinks);
            hc.LightLEDs(imageFrames, coords, colors, rates, blinker, false);

            sendFrames();
        }

        /* Randomly fills the cube, then fades it out. */
        public void RandomFill(ColorPalette cp)
        {
            hc.SpecificColorWholeCube(black, false);

            ColorPalette bp = new SolidPalette(new RGBColor(0,0,0));
            hc.RandomFill(imageFrames, black, false, 4, cp);
            hc.RandomFill(imageFrames, black, false, 8, bp);
            sendFrames();
        }

        /* Fill the cube in a zig-zag pattern. */
        public void ZigZagFill(ColorPalette cp)
        {
            hc.SpecificColorWholeCube(black, false);

            ColorPalette bp = new SolidPalette(black);

            hc.ZigZagFill(imageFrames, true, 1, HypnocubeImpl.Direction.X, cp);
            hc.ZigZagFill(imageFrames, true, 1, HypnocubeImpl.Direction.Y, bp);
            hc.ZigZagFill(imageFrames, true, 1, HypnocubeImpl.Direction.Z, cp);
            hc.ZigZagFill(imageFrames, true, 1, HypnocubeImpl.Direction.Y, bp);

            sendFrames();
        }

        /* Make a sub-cube inside the cube expand and contract. */
        public void ExpandingCube(ColorPalette cp)
        {
            hc.SpecificColorWholeCube(black, false);

            ColorFiller cf = new ColorFiller(hc);
            Fader f = new Fader(hc);

            hc.ExpandingSolidCube(imageFrames, false, 40, f, cp);

            sendFrames();
        }

        /* Make the cube fade in uniformly. */
        public void SimpleLight(ColorPalette cp)
        {
            hc.SpecificColorWholeCube(black, false);

            Fader f = new Fader(hc);


            hc.SimpleLight(imageFrames, false, 40, f, cp);
            sendFrames();
        }

        /* Make a single LED in the cube run around the cube in a random path. 
         * THIS METHOD IS NO LONGER USED
         */
        public void LittleRoamer(RGBColor color)
        {
            hc.SpecificColorWholeCube(black, false);

            ColorFiller cf = new ColorFiller(hc);
            ColorPalette cp = new SolidPalette(color);

            hc.LittleRoamer(imageFrames, 4, cf, true, cp, 200);

            ColorPalette blackPalette = new SolidPalette(new RGBColor(0, 0, 0));
            
            sendFrames();
        }

        /* The multiple version of LittleRoamer. */
        public void Fireflies(ColorPalette cp)
        {
            hc.SpecificColorWholeCube(black, false);

            ColorFiller cf = new ColorFiller(hc);

            hc.ManyLittleRoamers(imageFrames, 4, cf, true, cp, 20, 200);
            sendFrames();
        }

        /* Make a single block roam around the cube. 
         *
         * TODO: Make multiple blocks roam around the cube.
         */
        public void Blocks(ColorPalette cp)
        {
            hc.SpecificColorWholeCube(black, false);

            ColorFiller cf = new ColorFiller(hc);
            Coordinate c1 = new Coordinate(3, 3, 3);
            Coordinate c2 = new Coordinate(5, 5, 5);

            hc.Roamer(imageFrames, 4, cf, true, cp, c1, c2, 200);

            sendFrames();
        }

        /* Make a random trail appear through the cube. */
        public void Trail(ColorPalette cp)
        {
            hc.SpecificColorWholeCube(black, false);

            ColorFiller cf = new ColorFiller(hc);

            hc.ManyLittleRoamers(imageFrames, 4, cf, false, cp, 2, 200);

            sendFrames();
        }

        /* Do all of the animations, useful for testing. */
        public void DoAll(ColorPalette cp)
        {
            Fade(cp);
            Blink(cp);
            RandomFill(cp);
            ZigZagFill(cp);
            ExpandingCube(cp);
            Fireflies(cp);
            Blocks(cp);
            Trail(cp);
        }
    }
}
