using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Virus.Core;

namespace Virus.Core
{
    class Program
    {
        static void Main(string[] args)
        {
            Game virus = new Game(3);
            virus.Play();
        }
    }
}
