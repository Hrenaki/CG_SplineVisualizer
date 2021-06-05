using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CG_SplineVisualizer
{
    class Program
    {
        
        static void Main(string[] args)
        {
            using(Window win = new Window(1440, 800, "Spline"))
            {
                win.Run(60.0);
            }

            //Console.ReadLine();
            //
        }
    }
}
