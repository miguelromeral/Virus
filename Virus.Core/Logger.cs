using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Virus.Core
{
    /// <summary>
    /// Logger to register the actions that have been taken in the game.
    /// </summary>
    [Serializable]
    public class Logger
    {
        #region PROPERTIES
        /// <summary>
        /// Full filename of the logger.
        /// </summary>
        private string filename;
        /// <summary>
        /// Current date (in string)
        /// </summary>
        private string date;
        /// <summary>
        /// Beggining of the file name.
        /// </summary>
        private const string NAME_FILE_FORMAT = "Virus! (by Tranjis Games)";
        /// <summary>
        /// File extension.
        /// </summary>
        private const string NAME_EXTENSION = ".txt";
        /// <summary>
        /// Format of the date in the file.
        /// </summary>
        //private const string DATEFORMAT = "yyyyMMdd HHmm";
        private const string DATEFORMAT = "yyyyMMdd";
        #endregion

        #region CONSTRUCTOR
        /// <summary>
        /// Logger constructor. It generates the file and writes for first time.
        /// </summary>
        public Logger() {
            date = DateTime.Now.ToString(DATEFORMAT);
            filename = NAME_FILE_FORMAT + " " + date + NAME_EXTENSION;
            FirstLogMessage();
        }
        #endregion

        /// <summary>
        /// Write some text in the logger file.
        /// </summary>
        /// <param name="message">Text to be printed in the logger</param>
        /// <param name="print">True if want to also redirect this text to console output</param>
        /// <returns>True if it could be written to the logger.</returns>
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
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Writes the first message of the game to the logger.
        /// </summary>
        /// <returns>True if it could be completed</returns>
        public bool FirstLogMessage()
        {
            Write("-----------------------------------------------");
            Write("- New Virus! Game at " + date);
            Write("-----------------------------------------------");
            return true;
        }
    }
}
