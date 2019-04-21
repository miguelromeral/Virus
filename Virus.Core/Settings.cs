using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Virus.Core
{
    /// <summary>
    /// Settings parameters of the game.
    /// </summary>
    [Serializable]
    public class Settings
    {
        /// <summary>
        /// Game instance.
        /// </summary>
        public Game Game { get; set; }
        
        /// <summary>
        /// Filename from load / to save parameters.
        /// </summary>
        public const string FILE_PREFERENCES = "virus settings.conf";

        #region Strings labels for file
        private const string strNumberOrgans = "NumberOrgans";
        private const string strNumberMedicines = "NumberMedicines";
        private const string strNumberViruses = "NumberViruses";
        private const string strNumberThreatmentsSpreading = "NumberThreatmentsSpreading";
        private const string strNumberThreatmentsTransplant = "NumberThreatmentsTransplant";
        private const string strNumberThreatmentsOrganThief = "NumberThreatmentsOrganThief";
        private const string strNumberThreatmentsLatexGlove = "NumberThreatmentsLatexGlove";
        private const string strNumberThreatmentsMedicalError = "NumberThreatmentsMedicalError";
        private const string strNumberWildcardOrgans = "NumberWildcardOrgans";
        private const string strNumberWildcardViruses = "NumberWildcardViruses";
        private const string strNumberWildcardMedicines = "NumberWildcardMedicines";
        private const string strNumberCardInHand = "NumberCardInHand";
        private const string strNumberToWin = "NumberOrgansToWin";
        private const char splitter = ' ';
        #endregion

        #region Default settings
        /// <summary>
        /// Default number of organs in a game.
        /// </summary>
        private const int nNumberOrgans = 5;
        /// <summary>
        /// Default number of medicines in a game.
        /// </summary>
        private const int nNumberMedicines = 5;
        /// <summary>
        /// Default number of viruses in a game.
        /// </summary>
        private const int nNumberViruses = 4;
        /// <summary>
        /// Default number of spreading cards in a game.
        /// </summary>
        private const int nNumberThreatmentsSpreading = 2;
        /// <summary>
        /// Default number of transplant cards in a game.
        /// </summary>
        private const int nNumberThreatmentsTransplant = 3;
        /// <summary>
        /// Default number of organ thiefs card in a game.
        /// </summary>
        private const int nNumberThreatmentsOrganThief = 3;
        /// <summary>
        /// Default number of latex glove cards in a game.
        /// </summary>
        private const int nNumberThreatmentsLatexGlove = 1;
        /// <summary>
        /// Default number of medical error in a game.
        /// </summary>
        private const int nNumberThreatmentsMedicalError = 1;
        /// <summary>
        /// Default number of wildcard organs in a game.
        /// </summary>
        private const int nNumberWildcardOrgans = 1;
        /// <summary>
        /// Default number of wildcard viruses in a game.
        /// </summary>
        private const int nNumberWildcardViruses = 1;
        /// <summary>
        /// Default number of wildcard medicines in a game.
        /// </summary>
        private const int nNumberWildcardMedicines = 4;
        /// <summary>
        /// Default number of cards in a hand of a player.
        /// </summary>
        private const int nNumberCardInHand = 3;
        /// <summary>
        /// Default number of healthy organs to win.
        /// </summary>
        private const int nNumberToWin = 4;
        #endregion

        #region Properties of the current game
        /// <summary>
        /// Number of organs of each color in the game.
        /// </summary>
        public int NumberOrgans { get; set; }
        /// <summary>
        /// Number of medicines of each color in the game.
        /// </summary>
        public int NumberMedicines { get; set; }
        /// <summary>
        /// Number of viruses of each color in the game.
        /// </summary>
        public int NumberViruses { get; set; }
        /// <summary>
        /// Number of spreading cards available in the game.
        /// </summary>
        public int NumberThreatmentsSpreading { get; set; }
        /// <summary>
        /// Number of transplant cards availables in the game.
        /// </summary>
        public int NumberThreatmentsTransplant { get; set; }
        /// <summary>
        /// Number of organ thief cards availables in the game.
        /// </summary>
        public int NumberThreatmentsOrganThief { get; set; }
        /// <summary>
        /// Number of latex glove cards availables in the game.
        /// </summary>
        public int NumberThreatmentsLatexGlove { get; set; }
        /// <summary>
        /// Number of medical error cards availables inn the game.
        /// </summary>
        public int NumberThreatmentsMedicalError { get; set; }
        /// <summary>
        /// Number of wildcard organs in the game.
        /// </summary>
        public int NumberWildcardOrgans { get; set; }
        /// <summary>
        /// Number of wildcard medicines in the game.
        /// </summary>
        public int NumberWildcardMedicines { get; set; }
        /// <summary>
        /// Number of wildcard viruses in the game.
        /// </summary>
        public int NumberWildcardViruses { get; set; }
        /// <summary>
        /// Number of cards in a hand of a player in the game.
        /// </summary>
        public int NumberCardInHand { get; set; }
        /// <summary>
        /// Number of healthy organs required to win the game.
        /// </summary>
        public int NumberToWin { get; set; }
        #endregion

        /// <summary>
        /// Constructor of the settings.
        /// </summary>
        /// <param name="g"></param>
        public Settings(Game g)
        {
            Game = g;
        }

        /// <summary>
        /// Set default parameters of the game.
        /// </summary>
        public void SetDefaultParameters()
        {
            NumberOrgans = nNumberOrgans;
            NumberMedicines = nNumberMedicines;
            NumberViruses = nNumberViruses;
            NumberThreatmentsSpreading = nNumberThreatmentsSpreading;
            NumberThreatmentsTransplant = nNumberThreatmentsTransplant;
            NumberThreatmentsOrganThief = nNumberThreatmentsOrganThief;
            NumberThreatmentsLatexGlove = nNumberThreatmentsLatexGlove;
            NumberThreatmentsMedicalError = nNumberThreatmentsMedicalError;
            NumberWildcardOrgans = NumberWildcardOrgans;
            NumberWildcardViruses = nNumberViruses;
            NumberWildcardMedicines = nNumberWildcardMedicines;
            NumberCardInHand = nNumberCardInHand;
            NumberToWin = nNumberToWin;
        }

        /// <summary>
        /// Load the game preferences from the settings file.
        /// </summary>
        /// <returns>True if it could have been loaded successfully</returns>
        public bool LoadGamePreferences()
        {
            // If the file settins doesn't exist, it's generated rigth here.
            if (!File.Exists(FILE_PREFERENCES))
            {
                CreateFilePreferencesDefault();
            }
            try
            {
                // Set default parameters, perhaps it couldn't be loaded.
                SetDefaultParameters();

                using (StreamReader sr = new StreamReader(FILE_PREFERENCES))
                {
                    int counter = 1, value;
                    string line;
                    string[] args;

                    while ((line = sr.ReadLine()) != null)
                    {
                        try
                        {
                            // Avoid white lines and comments 
                            if (line.Length > 0 && line[0] != '#')
                            {
                                args = line.Split(' ');

                                if (args.Length > 1)
                                {
                                    switch (args[0])
                                    {
                                        case strNumberOrgans:
                                            value = Convert.ToInt32(args[1]);
                                            if (value > 0)
                                            {
                                                NumberOrgans = value;
                                            }
                                            break;
                                        case strNumberMedicines:
                                            value = Convert.ToInt32(args[1]);
                                            if (value > 0)
                                            {
                                                NumberMedicines = value;
                                            }
                                            break;
                                        case strNumberViruses:
                                            value = Convert.ToInt32(args[1]);
                                            if (value > 0)
                                            {
                                                NumberViruses = value;
                                            }
                                            break;
                                        case strNumberThreatmentsSpreading:
                                            value = Convert.ToInt32(args[1]);
                                            if (value >= 0)
                                            {
                                                NumberThreatmentsSpreading = value;
                                            }
                                            break;
                                        case strNumberThreatmentsTransplant:
                                            value = Convert.ToInt32(args[1]);
                                            if (value >= 0)
                                            {
                                                NumberThreatmentsTransplant = value;
                                            }
                                            break;
                                        case strNumberThreatmentsOrganThief:
                                            value = Convert.ToInt32(args[1]);
                                            if (value >= 0)
                                            {
                                                NumberThreatmentsOrganThief = value;
                                            }
                                            break;
                                        case strNumberThreatmentsLatexGlove:
                                            value = Convert.ToInt32(args[1]);
                                            if (value >= 0)
                                            {
                                                NumberThreatmentsLatexGlove = value;
                                            }
                                            break;
                                        case strNumberThreatmentsMedicalError:
                                            value = Convert.ToInt32(args[1]);
                                            if (value >= 0)
                                            {
                                                NumberThreatmentsMedicalError = value;
                                            }
                                            break;
                                        case strNumberWildcardOrgans:
                                            value = Convert.ToInt32(args[1]);
                                            if (value >= 0)
                                            {
                                                NumberWildcardOrgans = value;
                                            }
                                            break;
                                        case strNumberWildcardViruses:
                                            value = Convert.ToInt32(args[1]);
                                            if (value >= 0)
                                            {
                                                NumberWildcardViruses = value;
                                            }
                                            break;
                                        case strNumberWildcardMedicines:
                                            value = Convert.ToInt32(args[1]);
                                            if (value >= 0)
                                            {
                                                NumberWildcardMedicines = value;
                                            }
                                            break;
                                        case strNumberCardInHand:
                                            value = Convert.ToInt32(args[1]);
                                            if (value > 0)
                                            {
                                                NumberCardInHand = value;
                                            }
                                            break;
                                        case strNumberToWin:
                                            value = Convert.ToInt32(args[1]);
                                            if (value > 1 && value <= 5)
                                            {
                                                NumberToWin = Convert.ToInt32(args[1]);
                                            }
                                            break;
                                    }
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("Error processing {0} in line {1}: {2}", FILE_PREFERENCES, counter, e.Message);
                        }
                        counter++;
                    }
                }
                Console.WriteLine("Game settings loaded successfully.");
                return true;
            }
            catch (Exception e)
            {
                Console.Error.WriteLine("Game settings failed when loading.");
                //System.Environment.Exit(2);
                return false;
            }
        }

        /// <summary>
        /// Generate the file preferences with default values
        /// </summary>
        /// <returns>True if the file could generate.</returns>
        public bool CreateFilePreferencesDefault()
        {
            try
            {
                #region File Settings Definition
                string defaultSettings =
                    @"
#####################################################
#
# Virus! (by Tranjis Games)
# 
# Developed by MiguelRomeral
#
#####################################################

# This is the file preferences.
# Here is where you can change game settings.
#
# PLEASE, DON'T CHANGE LABELS, ONLY NUMBERS,
# The labels are used to tell the game which
# cards are dealed.
# If one label is forgotten, the game will
# use default settings.
#
# Note that the lines which begin with a hashtag
# is a comment, and the game will ignore it.

";
                defaultSettings += "# Number of organs by color (default: 5)." + Environment.NewLine;
                defaultSettings += String.Format("{0}{1}{2}" + Environment.NewLine, strNumberOrgans, splitter, nNumberOrgans);
                defaultSettings += Environment.NewLine;

                defaultSettings += "# Number of medicines by color (default: 5)." + Environment.NewLine;
                defaultSettings += String.Format("{0}{1}{2}" + Environment.NewLine, strNumberMedicines, splitter, nNumberMedicines);
                defaultSettings += Environment.NewLine;

                defaultSettings += "# Number of viruses by color (default: 4)." + Environment.NewLine;
                defaultSettings += String.Format("{0}{1}{2}" + Environment.NewLine, strNumberViruses, splitter, nNumberViruses);
                defaultSettings += Environment.NewLine;

                defaultSettings += "# Number of threatment spreading (default: 2)." + Environment.NewLine;
                defaultSettings += String.Format("{0}{1}{2}" + Environment.NewLine, strNumberThreatmentsSpreading, splitter, nNumberThreatmentsSpreading);
                defaultSettings += Environment.NewLine;

                defaultSettings += "# Number of threatment transplant (default: 3)." + Environment.NewLine;
                defaultSettings += String.Format("{0}{1}{2}" + Environment.NewLine, strNumberThreatmentsTransplant, splitter, nNumberThreatmentsTransplant);
                defaultSettings += Environment.NewLine;

                defaultSettings += "# Number of threatment organ thief (default: 3)." + Environment.NewLine;
                defaultSettings += String.Format("{0}{1}{2}" + Environment.NewLine, strNumberThreatmentsOrganThief, splitter, nNumberThreatmentsOrganThief);
                defaultSettings += Environment.NewLine;

                defaultSettings += "# Number of threatment latex glove (default: 1)." + Environment.NewLine;
                defaultSettings += String.Format("{0}{1}{2}" + Environment.NewLine, strNumberThreatmentsLatexGlove, splitter, nNumberThreatmentsLatexGlove);
                defaultSettings += Environment.NewLine;

                defaultSettings += "# Number of threatment medical error (default: 1)." + Environment.NewLine;
                defaultSettings += String.Format("{0}{1}{2}" + Environment.NewLine, strNumberThreatmentsMedicalError, splitter, nNumberThreatmentsMedicalError);
                defaultSettings += Environment.NewLine;

                defaultSettings += "# Number of wildcard organs (default: 1)." + Environment.NewLine;
                defaultSettings += String.Format("{0}{1}{2}" + Environment.NewLine, strNumberWildcardOrgans, splitter, nNumberWildcardOrgans);
                defaultSettings += Environment.NewLine;

                defaultSettings += "# Number of wildcard viruses (default: 1)." + Environment.NewLine;
                defaultSettings += String.Format("{0}{1}{2}" + Environment.NewLine, strNumberWildcardViruses, splitter, nNumberWildcardViruses);
                defaultSettings += Environment.NewLine;

                defaultSettings += "# Number of wildcard medicines (default: 4)."+Environment.NewLine;
                defaultSettings += String.Format("{0}{1}{2}" + Environment.NewLine, strNumberWildcardMedicines, splitter, nNumberWildcardMedicines);
                defaultSettings += Environment.NewLine;

                defaultSettings += "# Number of cards in hand (default: 3)." + Environment.NewLine;
                defaultSettings += String.Format("{0}{1}{2}" + Environment.NewLine, strNumberCardInHand, splitter, nNumberCardInHand);
                defaultSettings += Environment.NewLine;

                defaultSettings += "# Number of organs in the body to win (default: 4)." + Environment.NewLine;
                defaultSettings += String.Format("{0}{1}{2}" + Environment.NewLine, strNumberToWin, splitter, nNumberToWin);
                defaultSettings += Environment.NewLine;

                #endregion
                using (StreamWriter sw = File.AppendText(FILE_PREFERENCES))
                {
                    sw.Write(defaultSettings);
                }
                Console.WriteLine("Default file preferences created.");
                return true;
            }
            catch (Exception)
            {
                Console.WriteLine("Default file preferences couldn't be created.");
                return false;
            }
        }
    }
}
