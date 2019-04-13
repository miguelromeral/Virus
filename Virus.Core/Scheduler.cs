using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Virus.Core
{
    public static class Scheduler
    {
        public const char MOVE_SEPARATOR = '-';
        public const char MULTI_MOVE_SEPARATOR = ',';

        public const string CHARS_MEDICINE = "({0}*)";
        public const string CHARS_VIRUS = "({0}@)";
        
        public const string ACTION_PLAYING = "Playing";
        public const string ACTION_DISCARDING = "Discarding";
        public const string ACTION_CHOOSING = "ChoosingCars";



        public static string GetMoveItem(int playerid, int cardnum)
        {
            try
            {
                return String.Format("{0}{1}{2}", playerid, MOVE_SEPARATOR, cardnum);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static string GetManyMoveItem(string[] moves)
        {
            string res = moves[0];
            for(int i=1; i<moves.Length; i++)
            {
                res += MULTI_MOVE_SEPARATOR + moves[i];
            }
            return res;
        }

        public static List<List<string>> GetListOfListsSpreadingMoves(List<string> moves)
        {
            List<List<string>> wholeMoves = new List<List<string>>();
            int lastPlayerIndex = -1;
            List<string> movesOnePlayer = new List<string>();
            string move;

            int index = 0;
            do
            {
                move = moves[index];
                int p = Scheduler.GetStringInt(move, 2);
                if (p != lastPlayerIndex)
                {
                    if (lastPlayerIndex != -1)
                    {
                        wholeMoves.Add(movesOnePlayer);
                    }
                    lastPlayerIndex = p;
                    movesOnePlayer = new List<string>();
                }
                movesOnePlayer.Add(move);
                index++;
            }
            while (index < moves.Count);
            if (lastPlayerIndex != -1)
            {
                wholeMoves.Add(movesOnePlayer);
            }

            return wholeMoves;
        }

        public static bool IntInListString(List<string> list, int index, int i)
        {
            try
            {
                foreach (var m in list)
                {
                    if (IntInString(m, index, i))
                    {
                        return true;
                    }
                }
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }
        

        public static bool IntInString(string text, int index, int i)
        {
            try
            {
                int res = -1;
                Int32.TryParse(text.Substring(index, 1), out res);
                return res == i;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public static int GetStringInt(string text, int index)
        {
            try
            {
                int i = -1;
                Int32.TryParse(text.Substring(index, 1), out i);
                return i;
            }
            catch (Exception)
            {
                return -1;
            }
        }
        

    }
}
