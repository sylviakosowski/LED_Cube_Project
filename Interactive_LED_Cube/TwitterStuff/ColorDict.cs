using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interactive_LED_Cube
{
    /* Maps a color palette keyword to a color palette implementation. */
    public class ColorDict
    {
        private Dictionary<string, ColorPalette> dict;

        /* Initializes a dictionary with string-RGBColor pairs, to represent
         * what color each color string corresponds to.
         */
        public ColorDict()
        {
            Random r = new Random();
            dict = new Dictionary<string, ColorPalette>();
            dict.Add("rainbow", new RainbowPalette());
            dict.Add("mist", new AutodeskPalette());
            dict.Add("twilight", new RedBluePalette());
            dict.Add("sun", new SunrisePalette());
            dict.Add("random", new RandomPalette(r));

            /* Add more color palettes as you create them. */
        }

        public Dictionary<string, ColorPalette> Dict
        {
            get { return dict; }
        }
    }
}
