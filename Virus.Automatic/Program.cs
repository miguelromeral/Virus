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
            Game virus = new Game(firstHuman: false)
            {
                WaitingTime = 0
            };
            virus.Start();
            Console.WriteLine("Press any key to exit");
            Console.ReadLine();
        }
    }
}
