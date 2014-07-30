﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Autodesk_Interactive_Storytelling
{
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
            List<Coordinate> coords, List<RGBColor> colors, List<int> rates);

        /* 
         * Overloaded method. This overload used primarily if the LightingMethod
         * is BLINKING as there is an extra list of ints, numBlinks, to be used
         * in implementing blinking.
         */
        Dictionary<Coordinate, List<RGBColor>> CreateAnimation(
            List<Coordinate> coords, List<RGBColor> colors, List<int> rates,
            List<int> numBlinks);
    }
}
