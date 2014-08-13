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

        /* Animation pool */
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
            /*
            List<Coordinate> coords = new List<Coordinate>();
            List<RGBColor> colors = new List<RGBColor>();
            List<int> rates = new List<int>();
            RGBColor blue = new RGBColor(0, 0, 255);
            LightingMethod fader = new Fader(hc);

            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    for (int k = 0; k < 8; k++)
                    {
                        coords.Add(new Coordinate(i, j, k));
                        colors.Add(blue);
                        rates.Add(50);
                    }
                }
            }

            hc.LightLEDs(imageFrames, coords, colors, rates, fader, false);

            List<Coordinate> coords2 = new List<Coordinate>();
            List<RGBColor> colors2 = new List<RGBColor>();
            List<int> rates2 = new List<int>();
            //RGBColor blue = new RGBColor(0, 0, 255);
            LightingMethod fader2 = new Fader(hc);

            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    for (int k = 0; k < 8; k++)
                    {
                        coords2.Add(new Coordinate(i, j, k));
                        colors2.Add(color);
                        if (i == 0 && j == 0 && k == 0)
                        {
                            rates2.Add(5);
                        }
                        else
                        {
                            rates2.Add((i + j + k) * 10);
                        }
                    }
                }
            }

            hc.LightLEDs(imageFrames, coords2, colors2, rates2, fader2, false);

             * */
            sendFrames();
        }

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

            /*
            List<Coordinate> coords = new List<Coordinate>();
            List<RGBColor> colors = new List<RGBColor>();
            List<int> rates = new List<int>();
            List<int> numBlinks = new List<int>();

            //RGBColor red = new RGBColor(255, 0, 0);

            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    for (int k = 0; k < 8; k++)
                    {
                        coords.Add(new Coordinate(i, j, k));
                        colors.Add(color);
                        rates.Add((i + j + k));
                        numBlinks.Add(i + j + k);
                    }
                }
            }

            LightingMethod blinker = new Blinker(hc, numBlinks);
            hc.LightLEDs(imageFrames, coords, colors, rates, blinker, false);
             * */
            sendFrames();
        }

        public void RandomFill(ColorPalette cp)
        {
            hc.SpecificColorWholeCube(black, false);

            hc.RandomFill(imageFrames, black, true, 4);
            hc.RandomFill(imageFrames, black, false, 8);
            sendFrames();
        }

        public void ZigZagFill(ColorPalette cp)
        {

            hc.SpecificColorWholeCube(black, false);

            /*
            ColorPalette cp = new SolidPalette(color);
            ColorPalette bp = new SolidPalette(black);

            hc.ZigZagFill(imageFrames, true, 1, HypnocubeImpl.Direction.X, cp);
            hc.ZigZagFill(imageFrames, true, 1, HypnocubeImpl.Direction.Y, bp);
            hc.ZigZagFill(imageFrames, true, 1, HypnocubeImpl.Direction.Z, cp);
            hc.ZigZagFill(imageFrames, true, 1, HypnocubeImpl.Direction.Y, bp);
            */
            //ColorPalette cp = new RainbowPalette();

            ColorPalette bp = new SolidPalette(black);

            hc.ZigZagFill(imageFrames, true, 1, HypnocubeImpl.Direction.X, cp);
            hc.ZigZagFill(imageFrames, true, 1, HypnocubeImpl.Direction.Y, bp);
            hc.ZigZagFill(imageFrames, true, 1, HypnocubeImpl.Direction.Z, cp);
            hc.ZigZagFill(imageFrames, true, 1, HypnocubeImpl.Direction.Y, bp);

            sendFrames();
        }

        public void ExpandingCube(ColorPalette cp)
        {
            hc.SpecificColorWholeCube(black, false);

            /*
            ColorFiller cf = new ColorFiller(hc);
            Fader f = new Fader(hc);
            ColorPalette cp = new SolidPalette(color);

            hc.ExpandingSolidCube(imageFrames, false, 20, cf, cp);
             * */

            ColorFiller cf = new ColorFiller(hc);
            Fader f = new Fader(hc);

            hc.ExpandingSolidCube(imageFrames, false, 40, f, cp);

            sendFrames();
        }

        public void SimpleLight(ColorPalette cp)
        {
            hc.SpecificColorWholeCube(black, false);

            Fader f = new Fader(hc);


            hc.SimpleLight(imageFrames, false, 40, f, cp);
            sendFrames();
        }

        public void LittleRoamer(RGBColor color)
        {
            hc.SpecificColorWholeCube(black, false);

            ColorFiller cf = new ColorFiller(hc);
            ColorPalette cp = new SolidPalette(color);

            hc.LittleRoamer(imageFrames, 4, cf, true, cp);
            sendFrames();
        }

        public void Fireflies(ColorPalette cp)
        {
            hc.SpecificColorWholeCube(black, false);

            ColorFiller cf = new ColorFiller(hc);
            //ColorPalette cp = new RainbowPalette();

            hc.ManyLittleRoamers(imageFrames, 4, cf, true, cp, 20);
            sendFrames();
        }

        public void Blocks(ColorPalette cp)
        {
            hc.SpecificColorWholeCube(black, false);

            ColorFiller cf = new ColorFiller(hc);
            Coordinate c1 = new Coordinate(3, 3, 3);
            Coordinate c2 = new Coordinate(5, 5, 5);

            hc.Roamer(imageFrames, 4, cf, true, cp, c1, c2);

            sendFrames();
        }

        public void Trail(ColorPalette cp)
        {
            hc.SpecificColorWholeCube(black, false);

            ColorFiller cf = new ColorFiller(hc);

            hc.ManyLittleRoamers(imageFrames, 4, cf, false, cp, 1);

            sendFrames();
        }

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
