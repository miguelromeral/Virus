using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Virus.Core
{
    public class Logger
    {
        private string filename = "Log.txt";
        private string date;
        private const string NAME_FILE_FORMAT = "Virus! (by Tranjis Games)";
        private const string NAME_EXTENSION = ".txt";
        private const string DATEFORMAT = "yyyyMMdd HHmm";

        public Logger() {
            date = DateTime.Now.ToString(DATEFORMAT);
            filename = NAME_FILE_FORMAT + " " + date + NAME_EXTENSION;
            FirstLogMessage();
        }
        
        public bool Write(string message, bool print = false)
        {
            try
            {
                if (print)
                {
                    Console.WriteLine(message);
                }
                using (System.IO.StreamWriter file =
                new System.IO.StreamWriter(filename, true))
                {
                    file.WriteLine(message);
                }
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public bool FirstLogMessage()
        {
            Write("-----------------------------------------------");
            Write("- New Virus! Game at " + date + "            -\n");
            Write("-----------------------------------------------");
            return true;
        }
    }
}
