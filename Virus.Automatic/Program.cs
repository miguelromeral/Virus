using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Virus.Core;

namespace Virus.Automatic
{
    class Program
    {
        static void Main(string[] args)
        {
            Game virus = new Game(2);
            //virus.Start(2000);
            virus.Start(0);
            Console.WriteLine("Press, finally, any key to exit");
            Console.ReadLine();
        }
    }
}
