﻿using System;
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
    public class Hypnocube
    {
        /*
         * Represents the current state of all LEDS in cube
         * Each byte in array is one RGB value for LED.
         * Therefore, each LED takes up 3 bytes and therefore, 3 positions in array.
         */
        private byte[] colorArray;
        private static int ARRAY_SIZE = 1536;

        /* Constructor */
        public Hypnocube()
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
        public byte[] changeColorLED(int x, int y, int z, byte red ,byte green, byte blue)
        {
            int index = IndexFromCoord(x,y,z);
            colorArray[index] = red;
            colorArray[index + 1] = green;
            colorArray[index + 2] = blue;
            return colorArray;
        }

        /* Method to obtain an index from a tuple of 3 dimensional coordinates. */
        public int IndexFromCoord(int x, int y, int z)
        {
            return (x + 8 * y + 64 * z) * 3;
        }

    }
}