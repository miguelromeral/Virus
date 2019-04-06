﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Virus.Core
{
    public static class Scheduler
    {
        #region Static Game Settings
        public const int NUM_ORGANS = 5;
        public const int NUM_VIRUSES = 4;
        public const int NUM_MEDICINES = 5;
        public const int NUM_WILDCARD_ORGANS = 1;
        public const int NUM_WILDCARD_VIRUSES = 1;
        public const int NUM_WILDCARD_MEDICINES = 4;
        public const int NUM_THREATMENT_TRANSPLANT = 3;
        public const int NUM_THREATMENT_ORGANTHIEF = 3;
        public const int NUM_THREATMENT_SPREADING = 2;
        public const int NUM_THREATMENT_LATEXGLOVE = 1;
        public const int NUM_THREATMENT_MEDICALERROR = 1;
        public const int NUM_CARDS_HAND = 3;
        #endregion

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
