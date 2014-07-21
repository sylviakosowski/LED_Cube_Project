using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using System.Xml;

namespace CubeVisualization
{
    class Program
    {
        public static void Main()
        {
            Game game = new Game();
            game.Run(30, 30);
        }

    }
 
}

