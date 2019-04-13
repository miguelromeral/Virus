using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Virus.Core
{
    public class Settings
    {
        public Game Game { get; set; }
        
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
        private const char splitter = ' ';
        #endregion

        #region Default settings
        private const int nNumberOrgans = 5;
        private const int nNumberMedicines = 5;
        private const int nNumberViruses = 4;
        private const int nNumberThreatmentsSpreading = 2;
        private const int nNumberThreatmentsTransplant = 3;
        private const int nNumberThreatmentsOrganThief = 3;
        private const int nNumberThreatmentsLatexGlove = 1;
        private const int nNumberThreatmentsMedicalError = 1;
        private const int nNumberWildcardOrgans = 1;
        private const int nNumberWildcardViruses = 1;
        private const int nNumberWildcardMedicines = 4;
        private const int nNumberCardInHand = 3;
        #endregion

        #region Properties of the current game
        public int NumberOrgans { get; set; }
        public int NumberMedicines { get; set; }
        public int NumberViruses { get; set; }
        public int NumberThreatmentsSpreading { get; set; }
        public int NumberThreatmentsTransplant { get; set; }
        public int NumberThreatmentsOrganThief { get; set; }
        public int NumberThreatmentsLatexGlove { get; set; }
        public int NumberThreatmentsMedicalError { get; set; }
        public int NumberWildcardOrgans { get; set; }
        public int NumberWildcardMedicines { get; set; }
        public int NumberWildcardViruses { get; set; }
        public int NumberCardInHand { get; set; }
        #endregion

        public Settings(Game g)
        {
            Game = g;
        }

        public bool LoadGamePreferences()
        {
            if (!File.Exists(FILE_PREFERENCES))
            {
                CreateFilePreferencesDefault();
            }
            try
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

                using (StreamReader sr = new StreamReader(FILE_PREFERENCES))
                {
                    int counter = 1;
                    string line;
                    string[] args;

                    while ((line = sr.ReadLine()) != null)
                    {
                        try
                        {
                            if (line.Length > 0 && line[0] != '#')
                            {
                                args = line.Split(' ');

                                if (args.Length > 1)
                                {
                                    switch (args[0])
                                    {
                                        case strNumberOrgans:
                                            NumberOrgans = Convert.ToInt32(args[1]);
                                            break;
                                        case strNumberMedicines:
                                            NumberMedicines = Convert.ToInt32(args[1]);
                                            break;
                                        case strNumberViruses:
                                            NumberViruses = Convert.ToInt32(args[1]);
                                            break;
                                        case strNumberThreatmentsSpreading:
                                            NumberThreatmentsSpreading = Convert.ToInt32(args[1]);
                                            break;
                                        case strNumberThreatmentsTransplant:
                                            NumberThreatmentsTransplant = Convert.ToInt32(args[1]);
                                            break;
                                        case strNumberThreatmentsOrganThief:
                                            NumberThreatmentsOrganThief = Convert.ToInt32(args[1]);
                                            break;
                                        case strNumberThreatmentsLatexGlove:
                                             NumberThreatmentsLatexGlove = Convert.ToInt32(args[1]);
                                            break;
                                        case strNumberThreatmentsMedicalError:
                                             NumberThreatmentsMedicalError= Convert.ToInt32(args[1]);
                                            break;
                                        case strNumberWildcardOrgans:
                                             NumberWildcardOrgans= Convert.ToInt32(args[1]);
                                            break;
                                        case strNumberWildcardViruses:
                                             NumberWildcardViruses= Convert.ToInt32(args[1]);
                                            break;
                                        case strNumberWildcardMedicines:
                                             NumberWildcardMedicines= Convert.ToInt32(args[1]);
                                            break;
                                        case strNumberCardInHand:
                                             NumberCardInHand = Convert.ToInt32(args[1]);
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
            catch (Exception)
            {
                Console.WriteLine("Game settings failed when loading. Exiting game.");
                System.Environment.Exit(1);
                return false;
            }
        }

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
                defaultSettings += "# Number of organs by color (default: 5).\n";
                defaultSettings += String.Format("{0}{1}{2}\n", strNumberOrgans, splitter, nNumberOrgans);
                defaultSettings += "\n";

                defaultSettings += "# Number of medicines by color (default: 5).\n";
                defaultSettings += String.Format("{0}{1}{2}\n", strNumberMedicines, splitter, nNumberMedicines);
                defaultSettings += "\n";

                defaultSettings += "# Number of viruses by color (default: 4).\n";
                defaultSettings += String.Format("{0}{1}{2}\n", strNumberViruses, splitter, nNumberViruses);
                defaultSettings += "\n";

                defaultSettings += "# Number of threatment spreading (default: 2).\n";
                defaultSettings += String.Format("{0}{1}{2}\n", strNumberThreatmentsSpreading, splitter, nNumberThreatmentsSpreading);
                defaultSettings += "\n";

                defaultSettings += "# Number of threatment transplant (default: 3).\n";
                defaultSettings += String.Format("{0}{1}{2}\n", strNumberThreatmentsTransplant, splitter, nNumberThreatmentsTransplant);
                defaultSettings += "\n";

                defaultSettings += "# Number of threatment organ thief (default: 3).\n";
                defaultSettings += String.Format("{0}{1}{2}\n", strNumberThreatmentsOrganThief, splitter, nNumberThreatmentsOrganThief);
                defaultSettings += "\n";

                defaultSettings += "# Number of threatment latex glove (default: 1).\n";
                defaultSettings += String.Format("{0}{1}{2}\n", strNumberThreatmentsLatexGlove, splitter, nNumberThreatmentsLatexGlove);
                defaultSettings += "\n";

                defaultSettings += "# Number of threatment medical error (default: 1).\n";
                defaultSettings += String.Format("{0}{1}{2}\n", strNumberThreatmentsMedicalError, splitter, nNumberThreatmentsMedicalError);
                defaultSettings += "\n";

                defaultSettings += "# Number of wildcard organs (default: 1).\n";
                defaultSettings += String.Format("{0}{1}{2}\n", strNumberWildcardOrgans, splitter, nNumberWildcardOrgans);
                defaultSettings += "\n";

                defaultSettings += "# Number of wildcard viruses (default: 1).\n";
                defaultSettings += String.Format("{0}{1}{2}\n", strNumberWildcardViruses, splitter, nNumberWildcardViruses);
                defaultSettings += "\n";

                defaultSettings += "# Number of wildcard medicines (default: 4).\n";
                defaultSettings += String.Format("{0}{1}{2}\n", strNumberWildcardMedicines, splitter, nNumberWildcardMedicines);
                defaultSettings += "\n";

                defaultSettings += "# Number of cards in hand (default: 3).\n";
                defaultSettings += String.Format("{0}{1}{2}\n", strNumberCardInHand, splitter, nNumberCardInHand);
                defaultSettings += "\n";
                
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
