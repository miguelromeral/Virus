﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Virus.Core;

namespace Virus.Core
{
    public class ReaderInput
    {
        private Game game;

        public ReaderInput(Game g)
        {
            game = g;
        }

        public string RequestMovementChoosen(Player player, List<string> moves)
        {
            Console.WriteLine("- Please, enter the number of your organ you want to play this card.");
            BodyItem item;
            int c, o1;
            if (OnlyMyMoves(player, moves))
            {
                c = 0;
                foreach (string move in moves)
                {
                    o1 = Scheduler.GetStringInt(move, 2);
                    item = player.Body.Organs[o1];
                    Console.WriteLine("{0}.- {1,20}", (c+1), item);
                    c++;
                }

                int p = Convert.ToInt32(Console.ReadLine()) - 1;

                if (p < 0 || p >= moves.Count)
                    throw new Exception("You've not choosen a valid option.");

                return moves[p];

            }
            else
            {
                int p1;
                c = 0;
                foreach (string move in moves)
                {
                    p1 = Scheduler.GetStringInt(move, 0);
                    o1 = Scheduler.GetStringInt(move, 2);
                    player = game.Players[p1];
                    item = player.Body.Organs[o1];
                    Console.WriteLine("{0}.     {1,20} : {2,20}", (c+1), player.ShortDescription, item);
                    c++;
                }

                int p = Convert.ToInt32(Console.ReadLine()) - 1;

                if (p < 0 || p >= moves.Count)
                    throw new Exception("You've not choosen a valid option.");

                return moves[p];
            }
        }

        public int RequestMovementChoosenTransplant(List<string> moves, Game game)
        {
            try
            {
                Player one, two;
                BodyItem bone, btwo;
                int p1, p2, o1, o2, c = 1;

                Console.WriteLine("- Please, type the number of the combination to switch the appropiate organs.");
                foreach (string move in moves)
                {
                    p1 = Scheduler.GetStringInt(move, 0);
                    o1 = Scheduler.GetStringInt(move, 2);
                    p2 = Scheduler.GetStringInt(move, 4);
                    o2 = Scheduler.GetStringInt(move, 6);
                    one = game.Players[p1];
                    two = game.Players[p2];
                    bone = one.Body.Organs[o1];
                    btwo = two.Body.Organs[o2];
                    Console.WriteLine("{0}.     {1,20}       {2,20}", (c+1), one.ShortDescription, two.ShortDescription);
                    Console.WriteLine("        [{0,20}] <---> [{1,20}]\n", bone, btwo);
                    c++;
                }

                int p = Convert.ToInt32(Console.ReadLine()) - 1;

                if (p < 0 || p >= moves.Count)
                    throw new Exception("You've not choosen a valid combination to transplant organs.");
                
                return p;
            }
            catch (Exception)
            {
                throw new Exception("THE INPUT IS NOT VALID.");
            }


        }


        public int RequestMovementChoosenSpreading(List<string> moves, Game game)
        {
            try
            {
                Player one, two;
                BodyItem bone, btwo;
                int p1, p2, o1, o2, c = 1;

                p1 = Scheduler.GetStringInt(moves[0], 0);
                o1 = Scheduler.GetStringInt(moves[0], 2);
                one = game.Players[p1];
                bone = one.Body.Organs[o1];

                Console.WriteLine("- Please, type the number of the combination to spread your {0} virus.", bone.GetLastModifier());
                foreach (string move in moves)
                {
                    p2 = Scheduler.GetStringInt(move, 4);
                    o2 = Scheduler.GetStringInt(move, 6);
                    two = game.Players[p2];
                    btwo = two.Body.Organs[o2];
                    Console.WriteLine("{0}.     {1,20}: [{2,30}]", c, two.ShortDescription, btwo);
                    c++;
                }

                int p = Convert.ToInt32(Console.ReadLine()) - 1;

                if (p < 0 || p >= moves.Count)
                    throw new Exception("You've not choosen a valid combination to spread your virus.");

                return p;
            }
            catch (Exception)
            {
                throw new Exception("THE INPUT IS NOT VALID.");
            }


        }


        public string RequestMovementChoosenMedicalError(Player user, List<string> moves)
        {
            try
            {
                Console.WriteLine("- Please, type the player number to switch the body.");
                foreach (string m in moves)
                {
                    int mNum = -1;
                    Int32.TryParse(m.Substring(0, 1), out mNum);
                    Console.WriteLine(String.Format("- Player {0}.", mNum + 1));
                }

                int p = Convert.ToInt32(Console.ReadLine()) - 1;

                if (!Scheduler.IntInListString(moves, 0, p))
                    throw new Exception("You've not choosen a valid player number to switch your bodies.");

                return Scheduler.GetMoveItem(p, 0);
            }
            catch (Exception)
            {
                throw new Exception("THE INPUT IS NOT VALID.");
            }


        }

        public bool OnlyMyMoves(Player user, List<string> moves)
        {
            foreach(var move in moves)
            {
                if (Scheduler.GetStringInt(move, 0) != user.ID)
                {
                    return false;
                }
            }

            return true;
        }
        
    }
}