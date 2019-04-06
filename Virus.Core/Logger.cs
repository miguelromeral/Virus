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
        private const string NAME_FILE_FORMAT = "Virus! (by Tranjis Games) - {0}.txt";
        private const string DATEFORMAT = "dd/MM/yyyy, HH:mm";

        public Logger() {
            date = DateTime.Now.ToString(DATEFORMAT);
            filename = System.IO.Directory.GetCurrentDirectory() + "\\"+ String.Format(NAME_FILE_FORMAT, date);
            FirstLogMessage();
        }
        
        public bool WriteToLog(string message)
        {
            try
            {
                using (System.IO.StreamWriter file =
                new System.IO.StreamWriter(filename, true))
                {
                    file.WriteLine(message);
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool FirstLogMessage()
        {
            return WriteToLog(
                "-----------------------------------------------\n" +
                "- New Virus! Game at " +date+ "         \n" +
                "-----------------------------------------------\n"
                );
        }
    }
}
