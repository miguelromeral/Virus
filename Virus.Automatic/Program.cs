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
            Game virus = new Game(0, firstHuman:false);
            virus.Start();
            Console.WriteLine("Press any key to exit");
            Console.ReadLine();
        }
    }
}
