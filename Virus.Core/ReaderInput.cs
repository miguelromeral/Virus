using System;
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

        public string RequestMovementChoosen(Player user, List<string> moves)
        {
            if (OnlyMyMoves(user, moves))
            {
                // PRINT ONLY USER ORGANS
                Console.WriteLine("- Please, enter the number of your organ you want to play this card.");
                for (int i = 0; i < moves.Count; i++)
                {
                    string move = moves[i];
                    Console.WriteLine("{0}. {1}",
                        (Scheduler.GetStringInt(move, 2) + 1),
                        user.Body.Organs[Scheduler.GetStringInt(move, 2)]);
                }
                int c;
                try
                {
                    c = Convert.ToInt32(Console.ReadLine()) - 1;

                    if (!Scheduler.IntInListString(moves, 2, c))
                        throw new Exception("You've not choosen a valid card number to put this card.");
                }
                catch (Exception)
                {
                    return null;
                }
                return Scheduler.GetMoveItem(user.ID, c - 1);
            }
            else
            {
                int currentPlayer = -1;
                int c = -1;
                    foreach (string m in moves)
                    {
                        int mNum = -1;
                        Int32.TryParse(m.Substring(0, 1), out mNum);
                        if (currentPlayer != mNum)
                        {
                            currentPlayer = mNum;
                            Console.WriteLine(String.Format("Player {0}:", mNum + 1));
                        }
                        Int32.TryParse(m.Substring(2, 1), out c);
                        Console.WriteLine("-" + (c + 1) + ". " + game.Players[mNum].Body.Organs[c]);
                    }

                    Console.WriteLine("- Please, select the number of player to use this card:");
                    int p = Convert.ToInt32(Console.ReadLine()) - 1;
                    if (!Scheduler.IntInListString(moves, 0, p))
                        throw new Exception("You've not choosen a valid player number to put this card.");

                    Console.WriteLine("- Please, select the number of card to use this card:");
                    c = Convert.ToInt32(Console.ReadLine()) - 1;
                    if (!Scheduler.IntInListString(moves, 2, c))
                        throw new Exception ("You've not choosen a valid card number to put this card.");


                    return Scheduler.GetMoveItem(p, c);


                
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
                    Console.WriteLine("{0}.     {1,20}       {2,20}", c, one.ShortDescription, two.ShortDescription);
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
