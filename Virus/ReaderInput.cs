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
            if(OnlyMyMoves(user, moves))
            {
                // PRINT ONLY USER ORGANS
                Console.WriteLine("- Please, enter the number of your organ you want to play this card.");
                for(int i=0; i<moves.Count; i++)
                {
                    string move = moves[i];
                    Console.WriteLine("{0}. {1}",
                        (Scheduler.GetStringInt(move, 2) + 1),
                        user.Body.Organs[Scheduler.GetStringInt(move, 2)]);
                }
                int c;
                try
                {
                    c = Convert.ToInt32(Console.ReadLine());
                }
                catch (Exception)
                {
                    return null;
                }
                return Scheduler.GetMoveItem(user.ID, c-1);
            }
            else
            {
                // PRINT RIVALS ORGANS
            }

            return null;
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
