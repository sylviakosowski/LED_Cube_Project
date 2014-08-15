using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cube_Visualization
{
    /* 
     * Run the cube_visualization program independently of the
     * Interactive_LED_Cube program.
     */
    public class Program
    {
        public static void Main()
        {
            Game game = new Game();
            game.Run(30, 30);
        }
    }
}
