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




    }
}
