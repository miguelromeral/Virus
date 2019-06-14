using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Virus.Core
{
    /// <summary>
    /// Scheduler. A class to make order in the calls, messages and parameters that takes in part of the whole game.
    /// 
    /// The moves are written like: PLAYERID-ORGAN_OR_CARD_INDEX.
    /// Many moves are written like: MOVE,MOVE,MOVE...
    /// </summary>
    public static class Scheduler
    {
        #region PROPERTIES AND STATIC RESOURCES
        /// <summary>
        /// Separator in a move.
        /// </summary>
        public const char MOVE_SEPARATOR = '-';
        /// <summary>
        /// Separator of many moves in a same move. (f.e.: transplant requires two moves to anyone: the two body items to switch).
        /// </summary>
        public const char MULTI_MOVE_SEPARATOR = ',';

        /// <summary>
        /// String to print medicines.
        /// </summary>
        public const string CHARS_MEDICINE = "({0}*)";
        /// <summary>
        /// String to print viruses.
        /// </summary>
        public const string CHARS_VIRUS = "({0}@)";

        /// <summary>
        /// Message that indicates if the user is playing.
        /// </summary>
        public const string ACTION_PLAYING = "Playing";
        /// <summary>
        /// Message that indicates if the user is discarding.
        /// </summary>
        public const string ACTION_DISCARDING = "Discarding";
        /// <summary>
        /// Message that indicates if the user is choosing cards.
        /// </summary>
        public const string ACTION_CHOOSING = "ChoosingCars";

        #endregion

        /// <summary>
        /// Generate a move string given the Player ID and the card (or organ) number
        /// </summary>
        /// <param name="playerid">Player ID</param>
        /// <param name="cardnum">Index of card in hand or organ in body.</param>
        /// <returns>String with the move.</returns>
        public static string GenerateMove(int playerid, int cardnum)
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

        /// <summary>
        /// Get many moves provided in the same string.
        /// </summary>
        /// <param name="moves">array with all the moves </param>
        /// <returns>Single string with many moves provided</returns>
        public static string GetManyMoveItem(string[] moves)
        {
            string res = moves[0];
            for (int i = 1; i < moves.Length; i++)
            {
                res += MULTI_MOVE_SEPARATOR + moves[i];
            }
            return res;
        }

        /// <summary>
        /// Get many moves provided in the same string.
        /// </summary>
        /// <param name="moves">List of moves to be added.</param>
        /// <returns>One move with all moves indeed.</returns>
        public static string GetMoveByMultiple(List<string> moves)
        {
            if (moves.Count > 0)
            {
                string move = moves[0];
                for (int i = 1; i < moves.Count; i++)
                {
                    move += MULTI_MOVE_SEPARATOR + moves[i];
                }
                return move;
            }
            else
            {
                return String.Empty;
            }

        }

        /// <summary>
        /// Gets a list of list of moves to a whole list of moves.
        /// 
        /// This is usefull to separate the moves when spreading.
        /// <example>
        /// A spreading card can spread two viruses from my body, then the list of lists will have 2 list (one to each virus to spread).
        /// The list indeed of moves will have a list of possibles cards to this virus could be spread in.
        /// </example>
        /// The method gets all possibles moves and this have to separate between appropiates virus.
        /// The main reason to do that is because every move in the game is formatted as string, but the spreading move requires more
        /// complexity that the others.
        /// </summary>
        /// <param name="moves">Whole list of moves in spreading to be ordered appropiately</param>
        /// <returns>List ordered with all the moves provided</returns>
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
                // Gets the player referenced in the last move.
                int p = Scheduler.GetStringInt(move, 2);
                // if doesn't  match with the last one, we have to put it in a new list.
                if (p != lastPlayerIndex)
                {
                    if (lastPlayerIndex != -1)
                    {
                        // Add the move to the list.
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

        /// <summary>
        /// Indicates if a List of moves as string contains a specific int in a specific position.
        /// </summary>
        /// <param name="list">List of moves</param>
        /// <param name="index">Index of the character to compare</param>
        /// <param name="i">Int to search in the list.</param>
        /// <returns>True if the int is the list at this specified index.</returns>
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

        /// <summary>
        /// Incidates if a move has a specific int at a specific index.
        /// </summary>
        /// <param name="text">Move to search in</param>
        /// <param name="index">Position of character to compare</param>
        /// <param name="i">Int to compare</param>
        /// <returns>True if the int is in the index.</returns>
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

        /// <summary>
        /// Get in the move the int which is at specified index.
        /// </summary>
        /// <param name="text">Move to search in</param>
        /// <param name="index">Index of the int to retrieve.</param>
        /// <returns>Int at the specified position</returns>
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
        

        public static int RandomIndex(int end, int begin = 0)
        {
            return new Random().Next(begin, end);
        }
        
        public static void ChangeConsoleOutput(Card.CardColor? color = null, ConsoleColor background = ConsoleColor.Black, ConsoleColor foreground = ConsoleColor.White)
        {
            if (color == null)
            {
                Console.BackgroundColor = background;
                Console.ForegroundColor = foreground;
                return;
            }

            switch (color)
            {
                case Card.CardColor.Blue:
                    Console.BackgroundColor = ConsoleColor.Blue;
                    Console.ForegroundColor = ConsoleColor.White;
                    break;
                case Card.CardColor.Green:
                    Console.BackgroundColor = ConsoleColor.Green;
                    Console.ForegroundColor = ConsoleColor.Black;
                    break;
                case Card.CardColor.Red:
                    Console.BackgroundColor = ConsoleColor.Red;
                    Console.ForegroundColor = ConsoleColor.White;
                    break;
                case Card.CardColor.Yellow:
                    Console.BackgroundColor = ConsoleColor.Yellow;
                    Console.ForegroundColor = ConsoleColor.Black;
                    break;
                case Card.CardColor.Wildcard:
                    Console.BackgroundColor = ConsoleColor.White;
                    Console.ForegroundColor = ConsoleColor.Black;
                    break;
                case Card.CardColor.Purple:
                    Console.BackgroundColor = ConsoleColor.DarkMagenta;
                    Console.ForegroundColor = ConsoleColor.White;
                    break;
                case Card.CardColor.Bionic:
                    Console.BackgroundColor = ConsoleColor.Gray;
                    Console.ForegroundColor = ConsoleColor.Black;
                    break;
            }

        }


        public static List<string> GetAllMovesSpreading(List<List<string>> whole)
        {
            switch (whole.Count)
            {
                case 0:
                    return new List<string>();
                case 1:
                    return whole[0];
                default:
                    List<string> all = whole[0];
                    List<string> aux = new List<string>();
                    for(int i=1; i<whole.Count; i++)
                    {
                        List<string> l = whole[i];
                        if(all.Count == 0)
                        {
                            all = l;
                        }
                        else
                        {
                            foreach (string eli in all)
                            {
                                foreach (string elf in l)
                                {
                                    aux.Add(GetManyMoveItem(new string[] { eli, elf }));
                                }
                            }
                            all = aux;
                        }
                        aux = new List<string>();
                    }

                    return all;
            }
        }

        

    }
}
