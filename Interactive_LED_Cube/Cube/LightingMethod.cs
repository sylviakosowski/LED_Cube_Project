﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interactive_LED_Cube
{
    /* A LightingMethod determines the way in which LEDs are lit up. It determines
     * how animations for the cube are created. Most notable use is passing it into
     * the LightLEDS function. A LightingMethod can implement behaviors such as making
     * LEDS fade, blink, or even more simply, shine in a solid color.
     */
    public interface LightingMethod
    {
        /* 
         * Given a list of coordinates, colors, and ints (which can mean different
         * things depending on how the function is implemented but in general indicate
         * the speed/duration of the animation) generate an animation for the LEDs at
         * these coordinates. This animation is stored in a dictionary where the 
         * coordinate belonging to each LED is paired with a list of RGBColors where
         * each color in the list represents the color the LED will be at a single
         * frame in the animation.
         */
        Dictionary<Coordinate, List<RGBColor>> CreateAnimation(
            List<Coordinate> coords, List<RGBColor> colors, List<int> rates, bool resetFrames);

        /* Creates the frames of the animation which is specified in the Dictionary, and send
         * them off to the cube.
         */
        void CreateFrames(List<byte[]> imageFrames,
                Dictionary<Coordinate, List<RGBColor>> animDict, int longestAnim, bool resetFrames);

        /* Helper method to get the length (in number of animation frames) of the 
         * longest animation. */
        int GetLongestAnim();

        /* Light a block specified between coordinate c1 and coordinate c2, using this
         * LightingMethod and the specified color palette and rate. */
        void LightBlockUniform(List<byte[]> imageFrames, Coordinate c1, Coordinate c2,
            int rate, bool resetFrames, ColorPalette cp);

        /* Like LightBlockUniform except for many blocks. */
        void LightManyBlocksUniform(List<byte[]> imageFrames, List<Coordinate> coords,
            int rate, bool resetFrames, ColorPalette cp);
    }
}
