﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interactive_LED_Cube
{
    public class ColorDict
    {
        private Dictionary<string, ColorPalette> dict;

        /* Initializes a dictionary with string-RGBColor pairs, to represent
         * what color each color string corresponds to.
         */
        public ColorDict()
        {
            dict = new Dictionary<string, ColorPalette>();
            dict.Add("rainbow", new RainbowPalette());
            dict.Add("green", new AutodeskPalette());
            dict.Add("red", new RedBluePalette());

            //dict.Add("red", new RGBColor(255, 0, 0));
            //dict.Add("green", new RGBColor(0, 255, 0));
            //dict.Add("blue", new RGBColor(0, 0, 255));
            /* Add more colors as you see fit. */
        }

        public Dictionary<string, ColorPalette> Dict
        {
            get { return dict; }
        }
    }
}
