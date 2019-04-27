using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Virus.Core;

namespace Virus.Core
{
    [Serializable]
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
            int p1;
            c = 0;
            foreach (string move in moves)
            {
                p1 = Scheduler.GetStringInt(move, 0);
                o1 = Scheduler.GetStringInt(move, 2);
                player = game.Players[p1];
                item = player.Body.Items[o1];
                Console.Write("{0}. {1,20} : ", (c+1), player.ShortDescription);
                Scheduler.ChangeConsoleOutput(item.Organ.Color);
                item.Organ.PrintCard();
                item.PrintModifiers();
                Console.WriteLine("");
                Scheduler.ChangeConsoleOutput();
                c++;
            }

            int p = Convert.ToInt32(Console.ReadLine()) - 1;

            if (p < 0 || p >= moves.Count)
                throw new Exception("You've not choosen a valid option.");

            return moves[p];
            
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
                    bone = one.Body.Items[o1];
                    btwo = two.Body.Items[o2];
                    Console.WriteLine("{0}.", c);

                    Console.Write("   - {0,20}: ", one.ShortDescription);
                    bone.PrintBodyItem();
                    Console.WriteLine("");
                    Console.Write("   - {0,20}: ", two.ShortDescription);
                    btwo.PrintBodyItem();
                    Console.WriteLine("");
                    
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
                bone = one.Body.Items[o1];

                Console.WriteLine("- Please, type the number of the combination to spread your {0} virus.", bone.GetLastModifier());
                foreach (string move in moves)
                {
                    p2 = Scheduler.GetStringInt(move, 4);
                    o2 = Scheduler.GetStringInt(move, 6);
                    two = game.Players[p2];
                    btwo = two.Body.Items[o2];
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


        public string RequestMovementChoosenMedicalError(Player user, List<string> moves, Game game)
        {
            try
            {
                Console.WriteLine("- Please, type the player number to switch the body.");
                for(int i=0; i<moves.Count;i++)
                {
                    int mNum = -1;
                    Int32.TryParse(moves[i].Substring(0, 1), out mNum);
                    Console.WriteLine(String.Format("{0}.- {1}.", (i+ 1), game.Players[mNum].ShortDescription));
                }

                int p = Convert.ToInt32(Console.ReadLine()) - 1;

                if (!Scheduler.IntInListString(moves, 0, p))
                    throw new Exception("You've not choosen a valid player number to switch your bodies.");

                return moves[p];
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
