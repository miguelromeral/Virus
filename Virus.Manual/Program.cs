﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Virus.Core;

namespace Virus.ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            GameConsoleApp game = new GameConsoleApp(2000, firstHuman:true);
            game.Play();
            Console.WriteLine("Press any key to exit");
            Console.ReadLine();
        }
    }
}
