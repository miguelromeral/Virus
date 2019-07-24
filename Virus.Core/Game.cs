using System.Runtime.Serialization.Formatters.Binary;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Virus.Core
{
    /// <summary>
    /// Game. It has the players, bodies, cards and all functionality.
    /// </summary>
    [Serializable]
    public class Game
    {
        #region PROPERTIES
        /// <summary>
        /// List of players in this game.
        /// </summary>
        public List<Player> Players { get; private set; }
        /// <summary>
        /// Stack of cards without been played yet.
        /// </summary>
        private List<Card> Deck { get; set; }
        /// <summary>
        /// Stack of cards that have been already played.
        /// </summary>
        private List<Card> Discards { get; set; }
        /// <summary>
        /// Number of turns in the current game.
        /// </summary>
        public int Turn { get; protected set; }
        /// <summary>
        /// Logger class to register every action in the game.
        /// </summary>
        public Logger Logger { get; set; }
        /// <summary>
        /// Referee that indicates all availables moves.
        /// </summary>
        public Referee Referee { get; private set; }
        /// <summary>
        /// Specifies if the Game is the real (false) or it's in a scenario (true)
        /// </summary>
        public bool IsInScenario { get; set; }
        /// <summary>
        /// Index of current turn.
        /// </summary>
        public int CurrentTurn
        {
            get { return (Turn - 1) % Players.Count; }
        }
        
        
        /// <summary>
        /// Check if the game is over (any user have the healthy organs required).
        /// </summary>
        public bool GameOver
        {
            get
            {
                foreach (var p in Players)
                {
                    if (p.HealthyOrgans == Settings.NumberToWin)
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        /// <summary>
        /// Player's ID who has used in the current turn Overtime card.
        /// </summary>
        public int? PlayerInOvertime { get; set; }


        /// <summary>
        /// Total cards in the game.
        /// </summary>
        public List<Card> TotalCardsInGame { get; set; }

        /// <summary>
        /// Time to wait for the next turn. 0 for have to press a key to continue.
        /// </summary>
        public int WaitingTime { get; set; }
        #endregion

        #region INITIALIZERS
        /// <summary>
        /// Game constructor. It inits the current game and its parameters and config.
        /// </summary>
        /// <param name="firstHuman">First player is a human</param>
        /// <param name="l">Logger of the game. Null if its a copy</param>
        public Game(bool firstHuman = false, Logger l = null)
        {
            Logger = (l == null ? new Logger() : l);

            WriteToLog("We're getting ready Virus!", true);
            
            Settings.LoadGamePreferences();

            Referee = new Referee(this);

            Turn = 1;
            PlayerInOvertime = null;

            Deck = Shuffle(InitializeCards());
            TotalCardsInGame = new List<Card>();
            foreach(var c in Deck)
            {
                TotalCardsInGame.Add(c);
            }
            WriteToLog(Deck.Count+" cards shuffled.", true);
            Discards = new List<Card>();
            WriteToLog("Discard stack created.");
            InitializePlayers(Settings.PlayersNames, firstHuman);
        }
        
        /// <summary>
        /// Initializes the cards with the appropiate number of cards of each type.
        /// </summary>
        /// <returns>List of cards created</returns>
        private List<Card> InitializeCards()
        {
            List<Card> deck = new List<Card>();
            int i;

            #region ORGANS. MEDICINES & VIRUS BY COLOR
            foreach (Card.CardColor color in (Card.CardColor[])Enum.GetValues(typeof(Card.CardColor)))
            {
                if (color != Card.CardColor.Purple && color != Card.CardColor.Wildcard && color != Card.CardColor.Bionic)
                {
                    for (i = 0; i < Settings.NumberOrgans; i++)
                    {
                        deck.Add(new Card(color, Card.CardFace.Organ));
                    }
                    WriteToLog(Settings.NumberOrgans +" "+color+" organs created.");

                    for (i = 0; i < Settings.NumberMedicines; i++)
                    {
                        deck.Add(new Card(color, Card.CardFace.Medicine));
                    }
                    WriteToLog(Settings.NumberMedicines + " " + color + " medicines created.");

                    for (i = 0; i < Settings.NumberViruses; i++)
                    {
                        deck.Add(new Card(color, Card.CardFace.Virus));
                    }
                    WriteToLog(Settings.NumberViruses + " " + color + " viruses created.");

                    for (i = 0; i < Settings.NumberEvolvedMedicines; i++)
                    {
                        deck.Add(new Card(color, Card.CardFace.EvolvedMedicine));
                    }
                    WriteToLog(Settings.NumberEvolvedMedicines + " " + color + " evolved medicines created.");

                    for (i = 0; i < Settings.NumberEvolvedViruses; i++)
                    {
                        deck.Add(new Card(color, Card.CardFace.EvolvedVirus));
                    }
                    WriteToLog(Settings.NumberEvolvedViruses + " " + color + " evolved viruses created.");

                }
            }

            for (i = 0; i < Settings.NumberBionicOrgans; i++)
            {
                deck.Add(new Card(Card.CardColor.Bionic, Card.CardFace.Organ));
            }
            WriteToLog(Settings.NumberBionicOrgans + " bionic organs created.");

            #endregion

            #region WILDCARD CARDS
            for (i = 0; i < Settings.NumberWildcardOrgans; i++)
            {
                deck.Add(new Card(Card.CardColor.Wildcard, Card.CardFace.Organ));
            }
            WriteToLog(Settings.NumberWildcardOrgans + " wildcard organs created.");

            for (i = 0; i < Settings.NumberWildcardMedicines; i++)
            {
                deck.Add(new Card(Card.CardColor.Wildcard, Card.CardFace.Medicine));
            }
            WriteToLog(Settings.NumberWildcardMedicines + " wildcard medicines created.");

            for (i = 0; i < Settings.NumberWildcardViruses; i++)
            {
                deck.Add(new Card(Card.CardColor.Wildcard, Card.CardFace.Virus));
            }
            WriteToLog(Settings.NumberWildcardViruses + " wildcard viruses created.");

            for (i = 0; i < Settings.NumberWildcardEvolvedMedicines; i++)
            {
                deck.Add(new Card(Card.CardColor.Wildcard, Card.CardFace.EvolvedMedicine));
            }
            WriteToLog(Settings.NumberWildcardEvolvedMedicines + " wildcard evolved medicines created.");

            for (i = 0; i < Settings.NumberWildcardEvolvedViruses; i++)
            {
                deck.Add(new Card(Card.CardColor.Wildcard, Card.CardFace.EvolvedVirus));
            }
            WriteToLog(Settings.NumberWildcardEvolvedViruses + " wildcard evolved viruses created.");
            #endregion

            #region THREATMENTS
            for (i = 0; i < Settings.NumberThreatmentsSpreading; i++)
            {
                deck.Add(new Card(Card.CardColor.Purple, Card.CardFace.Spreading));
            }
            WriteToLog(Settings.NumberThreatmentsSpreading + " spreading cards created.");

            for (i = 0; i < Settings.NumberThreatmentsOrganThief; i++)
            {
                deck.Add(new Card(Card.CardColor.Purple, Card.CardFace.OrganThief));
            }
            WriteToLog(Settings.NumberThreatmentsOrganThief + " organ thief cards created.");

            for (i = 0; i < Settings.NumberThreatmentsTransplant; i++)
            {
                deck.Add(new Card(Card.CardColor.Purple, Card.CardFace.Transplant));
            }
            WriteToLog(Settings.NumberThreatmentsTransplant + " transplant cards created.");

            for (i = 0; i < Settings.NumberThreatmentsLatexGlove; i++)
            {
                deck.Add(new Card(Card.CardColor.Purple, Card.CardFace.LatexGlove));
            }
            WriteToLog(Settings.NumberThreatmentsLatexGlove + " latex glove cards created.");

            for (i = 0; i < Settings.NumberThreatmentsMedicalError; i++)
            {
                deck.Add(new Card(Card.CardColor.Purple, Card.CardFace.MedicalError));
            }
            WriteToLog(Settings.NumberThreatmentsMedicalError + " medical error cards created.");

            for (i = 0; i < Settings.NumberSecondOpinion; i++)
            {
                deck.Add(new Card(Card.CardColor.Purple, Card.CardFace.SecondOpinion));
            }
            WriteToLog(Settings.NumberSecondOpinion + " second opinion cards created.");

            for (i = 0; i < Settings.NumberQuarantine; i++)
            {
                deck.Add(new Card(Card.CardColor.Purple, Card.CardFace.Quarantine));
            }
            WriteToLog(Settings.NumberQuarantine + " quarantine cards created.");

            for (i = 0; i < Settings.NumberOvertime; i++)
            {
                deck.Add(new Card(Card.CardColor.Purple, Card.CardFace.Overtime));
            }
            WriteToLog(Settings.NumberOvertime + " overtime cards created.");

            for (i = 0; i < Settings.NumberProtectiveSuit; i++)
            {
                deck.Add(new Card(Card.CardColor.Purple, Card.CardFace.ProtectiveSuit));
            }
            WriteToLog(Settings.NumberProtectiveSuit + " protective suit cards created.");

            #endregion

            return deck;
        }

        /// <summary>
        /// Inits all the players and set the main values.
        /// </summary>
        /// <param name="PlayersNames">List of player's names</param>
        /// <param name="firstHuman">First player is human</param>
        /// <returns></returns>
        private bool InitializePlayers(List<string> PlayersNames, bool firstHuman = false)
        {
            WriteToLog("Creating players.", true);
            Players = new List<Player>();
            for (int i = 0; i < PlayersNames.Count; i++)
            {
                var p = new Player(this, PlayersNames[i] , (i == 0 && firstHuman)) { ID = i };
                Players.Add(p);

                System.Threading.Thread.Sleep(550);

                WriteToLog("Player with ID " + i + " created. " + ((i == 0 && firstHuman) ? "Human" : "IA: " + p.AI));
            }
            WriteToLog("Dealing cards.", true);
            for (int i = 0; i < PlayersNames.Count; i++)
            {
                List<Card> hand = new List<Card>();
                for (int j = 0; j < Settings.NumberCardInHand; j++)
                {
                    hand.Add(DrawNewCard(Players[i]));
                }

                Players[i].NewHand(hand);

            }
            return true;
        }

        /// <summary>
        /// Shuffle the whole deck stack. <see cref="http://www.vcskicks.com/randomize_array.php">Soyrce</see>
        /// </summary>
        /// <typeparam name="Card">Card</typeparam>
        /// <param name="inputList">List of cards to randomize</param>
        /// <returns>List of cards shuffled</returns>
        public List<Card> Shuffle<Card>(List<Card> inputList)
        {
            List<Card> randomList = new List<Card>();

            Random r = new Random();
            int randomIndex = 0;
            while (inputList.Count > 0)
            {
                randomIndex = r.Next(0, inputList.Count); //Choose a random object in the list
                randomList.Add(inputList[randomIndex]); //add it to the new, random list
                inputList.RemoveAt(randomIndex); //remove to avoid duplicates
            }

            return randomList; //return the new random list
        }
        #endregion

        /// <summary>
        /// Gets a new card from the deck stack and removes it.
        /// </summary>
        /// <param name="me">Player who is drawing a new card</param>
        /// <param name="inscenario">If the current game is in scenario of the AI. In that case, the card will automatically be random</param>
        /// <returns>Card recovered from the deck</returns>
        public Card DrawNewCard(Player me, bool inscenario = false)
        {
            Card newCard = null;
            try
            {
                Random r = new Random();
                // If there are no more cards in deck, shuffle the discard stacks and uses it
                if(Deck.Count == 0)
                {
                    WriteToLog("Deck is already empty. We have to redraw the discards.");
                    Deck = Shuffle(Discards);
                    Discards = new List<Card>();
                }

                if (inscenario)
                {
                    //newCard = new Card(Card.CardColor.Blue, Card.CardFace.Virus);
                    newCard = TotalCardsInGame[r.Next(0, TotalCardsInGame.Count)];
                }
                else
                {
                    newCard = Deck[0];
                }
                WriteToLog(me.Nickname + " draws a new card: " + newCard);
                Deck.RemoveAt(0);
            }
            catch (Exception)
            {
                WriteToLog("** There's no cards in deck to pop. **", true);
                return null;
            }
            return newCard;
        }

        /// <summary>
        /// Gets a list of a list with all the moves of a player's hand.
        /// </summary>
        /// <param name="p">Player to get all the moves allowed.</param>
        /// <returns></returns>
        public List<List<string>> GetListOfMovesWholeHand(Player p)
        {
            List<List<string>> wholeMoves = new List<List<string>>();

            foreach(Card c in p.Hand)
            {
                List<string> list = Referee.GetListMovements(p, c);
                wholeMoves.Add(list);
            }

            return wholeMoves;
        }

        /// <summary>
        /// Checks if the player's ID is the winner right now
        /// </summary>
        /// <param name="id">Player's ID to check.</param>
        /// <returns></returns>
        public bool AmITheWinner(int id)
        {
            if (GetPlayerByID(id).HealthyOrgans == Settings.NumberToWin)
                return true;
            return false;
        }

        /// <summary>
        /// Returns  the winner  of the game.
        /// </summary>
        /// <returns></returns>
        public Player GetTheWinner()
        {
            if (GameOver)
            {
                foreach(var p in Players)
                {
                    if(p.HealthyOrgans == Settings.NumberToWin)
                    {
                        return p;
                    }
                }
                return null;
            }
            return null;
        }


        /// <summary>
        /// Start the current game.
        /// </summary>
        public void Start() {
            Console.WriteLine("Press any key to begin the Virus!");
            Console.ReadLine();

            #region TESTING
            //Players[0].Hand[0] = new Card(Card.CardColor.Red, Card.CardFace.Organ);
            //Players[0].Hand[1] = new Card(Card.CardColor.Blue, Card.CardFace.Organ);
            //Players[0].Hand[2] = new Card(Card.CardColor.Yellow, Card.CardFace.Virus);



            //Players[0].Hand[0] = new Card(Card.CardColor.Wildcard, Card.CardFace.EvolvedMedicine);
            //Players[0].Hand[1] = new Card(Card.CardColor.Yellow, Card.CardFace.Organ);
            //Card c = new Card(Card.CardColor.Yellow, Card.CardFace.Organ);
            //Players[0].Hand[2] = c;
            //Players[0].Hand.Remove(c);

            //Players[0].Body.SetOrgan(new Card(Card.CardColor.Red, Card.CardFace.Organ));
            //Players[0].Body.SetOrgan(new Card(Card.CardColor.Bionic, Card.CardFace.Organ));

            //Players[0].Body.Items[0].NewEvolvedMedicine(this, 
            //    new Card(Card.CardColor.Red, Card.CardFace.EvolvedMedicine));

            //Players[1].Hand[0] = new Card(Card.CardColor.Red, Card.CardFace.Organ);
            //Players[1].Hand[1] = new Card(Card.CardColor.Red, Card.CardFace.Medicine);
            //Players[1].Hand[2] = new Card(Card.CardColor.Purple, Card.CardFace.SecondOpinion);






            //Players[0].Hand[0] = new Card(Card.CardColor.Purple, Card.CardFace.Overtime);
            //Players[0].Hand[1] = new Card(Card.CardColor.Purple, Card.CardFace.ProtectiveSuit);
            //Players[0].Hand[2] = new Card(Card.CardColor.Red, Card.CardFace.Organ);





            //Players[0].Body.SetOrgan(new Card(Card.CardColor.Red, Card.CardFace.Organ));
            //Players[0].Body.SetOrgan(new Card(Card.CardColor.Blue, Card.CardFace.Organ));

            //Players[0].Body.Items[0].NewVirus(new Card(Card.CardColor.Red, Card.CardFace.Virus), this);
            //Players[0].Body.Items[1].NewVirus(new Card(Card.CardColor.Blue, Card.CardFace.Virus), this);

            //Players[1].Body.SetOrgan(new Card(Card.CardColor.Blue, Card.CardFace.Organ));

            //Players[0].Hand[0] = new Card(Card.CardColor.Purple, Card.CardFace.Spreading);


            //Players[0].Hand[0] = new Card(Card.CardColor.Red, Card.CardFace.Medicine);
            //Players[0].Hand[1] = new Card(Card.CardColor.Blue, Card.CardFace.EvolvedMedicine);
            //Players[0].Hand[2] = new Card(Card.CardColor.Wildcard, Card.CardFace.EvolvedMedicine);

            //Players[1].Hand[0] = new Card(Card.CardColor.Red, Card.CardFace.Medicine);
            //Players[1].Hand[1] = new Card(Card.CardColor.Purple, Card.CardFace.Overtime);
            //Players[1].Hand[2] = new Card(Card.CardColor.Blue, Card.CardFace.Medicine);






            //Players[0].Hand[0] = new Card(Card.CardColor.Purple, Card.CardFace.Spreading);
            //Players[0].Body.SetOrgan(new Card(Card.CardColor.Red, Card.CardFace.Organ));
            //Players[0].Body.Items[0].NewEvolvedVirus(new Card(Card.CardColor.Red, Card.CardFace.EvolvedVirus), this);
            //Players[0].Body.SetOrgan(new Card(Card.CardColor.Blue, Card.CardFace.Organ));
            //Players[0].Body.Items[1].NewVirus(new Card(Card.CardColor.Blue, Card.CardFace.Virus), this);

            //Players[1].Hand[0] = new Card(Card.CardColor.Purple, Card.CardFace.ProtectiveSuit);
            //Players[2].Hand[0] = new Card(Card.CardColor.Purple, Card.CardFace.ProtectiveSuit);
            //Players[1].Body.SetOrgan(new Card(Card.CardColor.Red, Card.CardFace.Organ));
            //Players[1].Body.SetOrgan(new Card(Card.CardColor.Blue, Card.CardFace.Organ));

            //Players[2].Body.SetOrgan(new Card(Card.CardColor.Red, Card.CardFace.Organ));
            //Players[2].Body.SetOrgan(new Card(Card.CardColor.Blue, Card.CardFace.Organ));
            #endregion


            while (!GameOver)
            {
                //Console.Clear();
                PlayTurn(WaitingTime == 0, true);
                if(WaitingTime != 0)
                {
                    System.Threading.Thread.Sleep(WaitingTime);
                }
            }

            Console.Clear();
            
            PrintCurrentGameState();
            WriteToLog("The game has been finished.", true);
            WriteToLog(ToString(), false);
        }


        /// <summary>
        /// Gets a string overview of the game.
        /// </summary>
        /// <returns>String overview of the game.</returns>
        public override string ToString()
        {
            string res = String.Empty;

            res += "+------------+------------+----------------+-------------------------------+" + Environment.NewLine;

            res += String.Format("| #Turn: {0,3} | #Deck: {1,3} | #Discards: {2,3} | #Total Cards: {3,3}             |",
                Turn, Deck.Count, Discards.Count, TotalCardsInGame.Count) + Environment.NewLine;
            res += "+------------+------------+----------------+-------------------------------+" + Environment.NewLine;


            for (int x = 0; x < Players.Count; x++)
            {
                Player p = Players[x];

                res += String.Format("| {0,10} | {1,20} | AI: {2,11} | Body Pts: <{3,6}> |",
                    (CurrentTurn == x ? "<My Turn>" : ""),
                    p.Nickname,
                    p.AI.ToString(),
                    p.Body.Points) + Environment.NewLine;

                for (int y = 0; y < p.Body.Items.Count; y++)
                {
                    BodyItem item = p.Body.Items[y];
                    res += String.Format("| {0,1}.         | ", (y + 1));

                    if(item.Status == BodyItem.State.Immunized)
                    {
                        res += "[+] ";
                    }
                    else
                    {
                        res += "    ";
                    }

                    res += String.Format("{0,14}: ", item.Organ.ToString());
                    res += item.GetModifiers();

                    int padd = (5 * item.Modifiers.Count);

                    while (padd < 29)
                    {
                        res += " ";
                        padd++;
                    }

                    res += String.Format("  +={0,6} |", item.Points) + Environment.NewLine;
                }


                res += "+------------+-------------------------------------------------------------+" + Environment.NewLine;
            }

            return res;
        }

        /// <summary>
        /// Print in console the game state
        /// </summary>
        public void PrintCurrentGameState()
        {

            /*int j = 1;
            foreach (var p in TopPlayers())
            {
                Console.Write("[" + j + "][" + String.Format("{0,20}", p.ShortDescription) + "]" + Environment.NewLine);
                j++;
            }
            Console.Write(Environment.NewLine);*/

            Console.Write("+------------+------------+----------------+-------------------------------+" + Environment.NewLine);

            Console.Write(String.Format("| #Turn: {0,3} | #Deck: {1,3} | #Discards: {2,3} | #Total Cards: {3,3}             |",
                Turn, Deck.Count, Discards.Count, TotalCardsInGame.Count) + Environment.NewLine);
            Console.Write("+------------+------------+----------------+-------------------------------+" + Environment.NewLine);


            for (int x = 0; x < Players.Count; x++)
            {
                Player p = Players[x];
                
                ConsoleColor f = (CurrentTurn == x ? ConsoleColor.Yellow : ConsoleColor.White);

                Scheduler.ChangeConsoleOutput(foreground: f);

                Console.Write(String.Format("| {0,10} | {1,20} | AI: {2,11} | Body Pts: <{3,6}> |",
                    (CurrentTurn == x ? "<My Turn>"  : ""),
                    p.Nickname,
                    p.AI.ToString(),
                    p.Body.Points) + Environment.NewLine);

                for (int y = 0; y < p.Body.Items.Count; y++)
                {
                    BodyItem item = p.Body.Items[y];
                    //printed += "+------------+------------+----------------+-------------------------------+" + Environment.NewLine;
                    Console.Write(String.Format("| {0,1}.         | ", (y + 1)));


                    if (item.Status == BodyItem.State.Immunized)
                    {
                        Console.Write("[+] ");
                    }
                    else
                    {
                        Console.Write("    ");
                    }

                    Scheduler.ChangeConsoleOutput(item.Organ.Color);
                    Console.Write(String.Format("{0,14}: ", item.Organ.ToString()));
                    

                    item.PrintModifiers();
                    Scheduler.ChangeConsoleOutput(foreground: f);

                    int padd = (5 * item.Modifiers.Count);

                    Scheduler.ChangeConsoleOutput(foreground:f);
                    while (padd < 29)
                    {
                        Console.Write(" ");
                        padd++;
                    }

                    Console.Write(String.Format("  +={0,6} |", item.Points) + Environment.NewLine);

                }


                Console.Write("+------------+-------------------------------------------------------------+" + Environment.NewLine);

                Scheduler.ChangeConsoleOutput();
            }
        }
        
        /// <summary>
        /// Play turn by the computer
        /// </summary>
        /// <param name="wait">True if the user will have to press a key to continue with the move.</param>
        /// <param name="printHand">True if print the player hand.</param>
        public virtual void PlayTurn(bool wait = false, bool printHand = false)
        {
            Player p = Players[CurrentTurn];


            PrintCurrentGameState();

            if (printHand)
            {
                p.PrintMyOptions();
            }
            Console.WriteLine();
            WriteToLog("Turn #" + Turn + " (" + p.Nickname + ").");
            if (wait)
            {
                Console.WriteLine("Press any key to continue.");
                Console.ReadLine();
            }

            p.Computer.PlayTurn();
            PassTurn(p);
        }

        /// <summary>
        /// Clear the player's overtime flags for the current turn.
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        protected bool ClearOvertimeFlag(Player p)
        {
            if (PlayerInOvertime != null && PlayerInOvertime == p.ID)
            {
                PlayerInOvertime = null;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Pass the current turn
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public bool PassTurn(Player p)
        {
            if(p.Hand.Count > 0)
            {
                ClearOvertimeFlag(p);
                WriteToLog("The player "+p.Nickname+" has no cards in his hand. Pass the turn.");
            }

            if (PlayerInOvertime == null || PlayerInOvertime != p.ID)
            {
                // Once the player has used (or discarded) cards, fill the hand to the number.
                DrawCardsToFill(p);
                Turn++;
                ClearFlagsProtectiveSuite();
                return true;
            }
            return false;
        }


        private void ClearFlagsProtectiveSuite()
        {
            foreach(var p in Players)
            {
                p.PlayedProtectiveSuit = false;
            }
        }

        /// <summary>
        /// Draws as many cards needed until fill the player hand.
        /// </summary>
        /// <param name="player"></param>
        /// <param name="max"></param>
        /// <param name="insecenario"></param>
        public void DrawCardsToFill(Player player, int max = 0, bool insecenario = false)
        {
            if (max == 0)
                max = Settings.NumberCardInHand;

            for (int i = player.Hand.Count; i < max; i++)
            {
                player.Hand.Add(DrawNewCard(player, insecenario));
            }
        }
        
        /// <summary>
        /// Spread one virus from one free organ by only one move.
        /// </summary>
        /// <param name="moves">Move that indicates the spreading order</param>
        /// <returns>Error message if it couldn't be spreaded</returns>
        public string DoSpreadingOneItem(string moves)
        {
            Player one, two;
            BodyItem bone, btwo;
            int p1, p2, o1, o2;

            p1 = Scheduler.GetStringInt(moves, 0);
            o1 = Scheduler.GetStringInt(moves, 2);
            one = Players[p1];
            bone = one.Body.Items[o1];

            p2 = Scheduler.GetStringInt(moves, 4);
            o2 = Scheduler.GetStringInt(moves, 6);
            two = Players[p2];
            btwo = two.Body.Items[o2];

            Card virus = bone.GetLastModifier();
            bone.Modifiers.Remove(virus);
            btwo.NewVirus(virus, this);

            WriteToLog(one.Nickname+" has spread his "+virus+" from his "+bone+" to "+two.Nickname+"'s "+btwo);

            return null;
        }

        /// <summary>
        /// Do a transplant in the game. Switch two body items.
        /// </summary>
        /// <param name="me"></param>
        /// <param name="myCard"></param>
        /// <param name="move">Move to spread</param>
        /// <param name="wholemoves"></param>
        /// <returns>Error message if couldn't be switched</returns>
        public void PlayGameCardTransplant(Player me, Card myCard, string move, List<string> wholemoves)
        {
            try
            {
                if (!ProtectiveSuitScenario(me, myCard, move, wholemoves))
                {
                    Player one, two;
                    BodyItem bone, btwo;
                    int p1, p2, o1, o2;
                    p1 = Scheduler.GetStringInt(move, 0);
                    o1 = Scheduler.GetStringInt(move, 2);
                    p2 = Scheduler.GetStringInt(move, 4);
                    o2 = Scheduler.GetStringInt(move, 6);
                    one = Players[p1];
                    two = Players[p2];
                    bone = one.Body.Items[o1];
                    btwo = two.Body.Items[o2];

                    Players[p1].Body.Items[o1] = btwo;
                    Players[p2].Body.Items[o2] = bone;

                    WriteToLog(one.Nickname + " has transplantated his " + bone + " by the " + two.Nickname + "'s organ " + btwo);
                }
            }
            catch (Exception)
            {}
        }

        /// <summary>
        /// Steal one body item to one player.
        /// </summary>
        /// <param name="me">Player to receive the new body item</param>
        /// <param name="myCard"></param>
        /// <param name="move">Move that indicates who and what to steal</param>
        /// <param name="wholemoves"></param>
        /// <returns>Error message if there is</returns>
        public void PlayOrganThief(Player me, Card myCard, string move, List<string> wholemoves)
        {
            try
            {
                if (!ProtectiveSuitScenario(me, myCard, move, wholemoves))
                {
                    Player rival = Players[Scheduler.GetStringInt(move, 0)];
                    BodyItem stolen = rival.Body.Items[Scheduler.GetStringInt(move, 2)];
                    rival.Body.Items.Remove(stolen);

                    me.Body.Items.Add(stolen);

                    WriteToLog(me.Nickname + " has stolen the " + rival.Nickname + "'s organ " + stolen);
                }
                
            }
            catch (Exception)
            {}
        }

        /// <summary>
        /// Remove any hand of the players but me.
        /// </summary>
        /// <param name="me">Player who has used the card.</param>
        /// <param name="myCard"></param>
        /// <returns>Error message if it is</returns>
        public void PlayLatexGlove(Player me, Card myCard)
        {
            foreach (Player rival in Players)
            {
                if (rival.ID != me.ID)
                {

                    if (!ProtectiveSuitScenario(me, myCard, Scheduler.GenerateMove(rival.ID, 0), null))
                    {
                        WriteToLog(rival.Nickname + " is going to lost his hand due to a latex glove played by " + me.Nickname);
                        DiscardAllHand(rival);
                    }
                }
            }
        }

        /// <summary>
        /// Switch two bodies.
        /// </summary>
        /// <param name="me">Player who has used the card.</param>
        /// <param name="myCard"></param>
        /// <param name="move">Move that indicates who switch the whole body</param>
        /// <param name="wholemoves"></param>
        /// <returns></returns>
        protected void PlayMedicalError(Player me, Card myCard, string move, List<string> wholemoves)
        {
            try
            {
                if (!ProtectiveSuitScenario(me, myCard, move, wholemoves))
                {
                    Player toswitch = Players[Scheduler.GetStringInt(move, 0)];

                    Body aux = me.Body;
                    me.Body = toswitch.Body;
                    toswitch.Body = aux;

                    WriteToLog(me.Nickname + " has switched his body by the " + toswitch.Nickname + "'s one.");
                }
            }
            catch (Exception)
            {}
        }

        /// <summary>
        /// Switch hands between the current turn's player and other's one.
        /// </summary>
        /// <param name="me">Current player.</param>
        /// <param name="myCard">Card used.</param>
        /// <param name="move">Move to be performed.</param>
        /// <param name="wholemoves">List of whole possibly moves.</param>
        public void PlaySecondOpinion(Player me, Card myCard, string move, List<string> wholemoves)
        {
            try
            {
                if (!ProtectiveSuitScenario(me, myCard, move, wholemoves))
                {
                    Player toswitch = Players[Scheduler.GetStringInt(move, 0)];

                    List<Card> aux = me.Hand;
                    me.Hand = toswitch.Hand;
                    toswitch.Hand = aux;

                    WriteToLog(me.Nickname + " has switched his hand with the " + toswitch.Nickname + "'s one.");

                    // Avoids go to the previous turn when a Player in overtime plays this card.
                    if (PlayerInOvertime == null)
                    {
                        Turn--;
                    }

                    WriteToLog(me.Nickname + " can play again with his new hand.");
                }
            }
            catch (Exception)
            {}
        }

        /// <summary>
        /// Play a card in the game given the move choosen.
        /// </summary>
        /// <param name="player">Player who plays the card</param>
        /// <param name="myCard">Card used</param>
        /// <param name="move">Move with the option selected</param>
        /// <param name="wholemoves">List of whole moves allowed</param>
        /// <param name="discard">Indicates if we have to discard the card</param>
        /// <returns></returns>
        public void PlayCardByMove(Player player, Card myCard, string move, List<string> wholemoves, bool discard = true)
        {
            switch (myCard.Face)
            {
                case Card.CardFace.Organ:
                    RemoveCardFromHand(player, myCard);
                    PlayGameCardOrgan(player, myCard);
                    break;

                case Card.CardFace.Medicine:
                    RemoveCardFromHand(player, myCard);
                    PlayGameCardMedicine(player, myCard, move);
                    break;

                case Card.CardFace.Virus:
                    if(discard)
                        RemoveCardFromHand(player, myCard);
                    PlayGameCardVirus(player, myCard, move, wholemoves);
                    break;

                case Card.CardFace.Transplant:
                    if (discard)
                        DiscardFromHand(player, myCard);
                    PlayGameCardTransplant(player, myCard, move, wholemoves);   
                    break;

                case Card.CardFace.OrganThief:
                    if(discard)
                        DiscardFromHand(player, myCard);
                    PlayOrganThief(player, myCard, move, wholemoves);
                    break;

                case Card.CardFace.Spreading:
                    if (discard)
                        DiscardFromHand(player, myCard);
                    PlayGameCardSpreading(player, myCard, move, wholemoves);
                    break;

                case Card.CardFace.LatexGlove:
                    DiscardFromHand(player, myCard);
                    PlayLatexGlove(player, myCard);
                    break;

                case Card.CardFace.MedicalError:
                    if(discard)
                        DiscardFromHand(player, myCard);
                    PlayMedicalError(player, myCard, move, wholemoves);
                    break;

                case Card.CardFace.EvolvedMedicine:
                    RemoveCardFromHand(player, myCard);
                    PlayGameCardEvolvedMedicine(player, myCard, move);
                    break;

                case Card.CardFace.EvolvedVirus:
                    RemoveCardFromHand(player, myCard);
                    PlayGameCardEvolvedVirus(player, myCard, move, wholemoves);
                    break;

                case Card.CardFace.SecondOpinion:
                    if(discard)
                        DiscardFromHand(player, myCard);
                    PlaySecondOpinion(player, myCard, move, wholemoves);
                    break;

                case Card.CardFace.Quarantine:
                    DiscardFromHand(player, myCard);
                    PlayQuarantine(player, move);
                    break;

                case Card.CardFace.Overtime:
                    DiscardFromHand(player, myCard);
                    PlayOvertime(player);
                    break;
            }
        }

        /// <summary>
        /// Get the list of affected players when a spreading card is used.
        /// </summary>
        /// <param name="me"></param>
        /// <param name="move"></param>
        /// <returns></returns>
        public List<Player> GetListPlayesInSpreadingMove(Player me, string move)
        {
            List<Player> rivals = new List<Player>();

            string[] moves = move.Split(Scheduler.MULTI_MOVE_SEPARATOR);

            for (int i = 0; i < moves.Length; i ++)
            {
                Player p = Players[Scheduler.GetStringInt(moves[i],0)];
                if (!rivals.Contains(p) && me.ID != p.ID)
                {
                    rivals.Add(p);
                }
            }
            return rivals;
        }

        /// <summary>
        /// State of the game when a Protective Suit is being played.
        /// </summary>
        /// <param name="player"></param>
        /// <param name="myCard"></param>
        /// <param name="move"></param>
        /// <param name="wholemoves"></param>
        /// <returns></returns>
        public bool ProtectiveSuitScenario(Player player, Card myCard, string move, List<string> wholemoves)
        {
            if (!IsInScenario)
            {

                Player rival = GetPlayerByMove(player, myCard, move);
                if (rival == null)
                {
                    List<Player> rivals = GetListPlayesInSpreadingMove(player, move);
                    foreach (var r in rivals)
                    {
                        if (ProceedProtectiveSuit(player, r, myCard, move, wholemoves))
                        {
                            return true;
                        }
                    }
                    return false;
                }
                else
                {
                    return ProceedProtectiveSuit(player, rival, myCard, move, wholemoves);
                }
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Performs the operations when a Protective Suit has been played.
        /// </summary>
        /// <param name="player"></param>
        /// <param name="rival"></param>
        /// <param name="myCard"></param>
        /// <param name="move"></param>
        /// <param name="wholemoves"></param>
        /// <returns></returns>
        public virtual bool ProceedProtectiveSuit(Player player, Player rival, Card myCard, string move, List<string> wholemoves)
        {
            bool psused = SomeoneHasDefend();
            if (rival.DoIHaveProtectiveSuit() && rival.Computer.DefendFromCard(player, myCard, move))
            {

                WriteToLog(rival.Nickname + " has protected with a Protective Suit.", true);
                if (WaitingTime != 0)
                {
                    System.Threading.Thread.Sleep(WaitingTime);
                }
                else
                {
                    Console.ReadKey();
                }

                if (wholemoves == null)
                {
                    // Playable cards that doesn't require play a move.


                }
                else
                {

                    if (!psused)
                    {
                        wholemoves = Referee.GetListMovements(player, myCard, true);
                    }
                    wholemoves = Referee.RemoveMovesPlayer(wholemoves, rival.ID, myCard, player);
                    
                    move = player.Computer.ChooseBestOptionProtectiveSuit(wholemoves);

                    if (move != null)
                    {
                        PlayCardByMove(player, myCard, move, wholemoves, false);
                    }
                }

                return true;
            }
            return false;
        }

        /// <summary>
        /// Get the rival's player with the current move.
        /// </summary>
        /// <param name="me"></param>
        /// <param name="card"></param>
        /// <param name="move"></param>
        /// <returns></returns>
        public Player GetPlayerByMove(Player me, Card card, string move)
        {
            int index;
            switch (card.Face)
            {
                case Card.CardFace.Spreading:
                    return null;
                case Card.CardFace.Transplant:
                    index = Scheduler.GetStringInt(move, 0);
                    int i2 = Scheduler.GetStringInt(move, 4);
                    index = (index == me.ID ? i2 : index);
                    return Players[index];
                case Card.CardFace.Virus:
                case Card.CardFace.EvolvedVirus:
                case Card.CardFace.OrganThief:
                case Card.CardFace.MedicalError:
                case Card.CardFace.LatexGlove:
                case Card.CardFace.SecondOpinion:
                    index = Scheduler.GetStringInt(move, 0);
                    return Players[index];

                // TODO: the rest of cards.

                default: return null;
            }
        }

        /// <summary>
        /// Indicates if some player has used a Protective Suit in the current turn.
        /// </summary>
        /// <returns></returns>
        protected bool SomeoneHasDefend()
        {
            foreach(Player p in Players)
            {
                if (p.PlayedProtectiveSuit)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Play a spreading card given the move selected.
        /// </summary>
        /// <param name="player"></param>
        /// <param name="myCard"></param>
        /// <param name="move">All moves to spreading in only one string</param>
        /// <param name="wholemoves"></param>
        /// <returns>Error message if its</returns>
        public void PlayGameCardSpreading(Player player, Card myCard, string move, List<string> wholemoves)
        {

            if (!ProtectiveSuitScenario(player, myCard, move, wholemoves))
            {
                string[] choosen = move.Split(Scheduler.MULTI_MOVE_SEPARATOR);
                for (int i = 0; i <= (choosen.Length / 2); i += 2)
                {
                    string m = Scheduler.GetManyMoveItem(new string[] { choosen[i], choosen[i + 1] });
                    DoSpreadingOneItem(m);
                }
            }
        }

        /// <summary>
        /// Plays an organ card to the player.
        /// </summary>
        /// <param name="player">Player who uses it</param>
        /// <param name="myCard">Organ card</param>
        /// <returns>Error message if its</returns>
        public void PlayGameCardOrgan(Player player, Card myCard)
        {
            WriteToLog(player.Nickname + " has played a " + myCard);
            player.Body.SetOrgan(myCard);
        }

        /// <summary>
        /// Play medicine card to one player.
        /// </summary>
        /// <param name="player">Player to use the card</param>
        /// <param name="myCard">Medicine card</param>
        /// <param name="move">Move to indicates in which organ uses it</param>
        /// <returns></returns>
        protected void PlayGameCardMedicine(Player player, Card myCard, string move)
        {
            WriteToLog(player.Nickname + " has used a " + myCard + " in his " + player.Body.Items[Scheduler.GetStringInt(move, 2)]);
            player.Body.Items[Scheduler.GetStringInt(move, 2)].NewMedicine(this, myCard);
        }

        /// <summary>
        /// Performs evolved medicine move.
        /// </summary>
        /// <param name="player"></param>
        /// <param name="myCard"></param>
        /// <param name="move"></param>
        protected void PlayGameCardEvolvedMedicine(Player player, Card myCard, string move)
        {
            WriteToLog(player.Nickname + " has used a " + myCard + " in his " + player.Body.Items[Scheduler.GetStringInt(move, 2)]);
            player.Body.Items[Scheduler.GetStringInt(move, 2)].NewEvolvedMedicine(this, myCard);
        }

        /// <summary>
        /// Play virus from one player.
        /// </summary>
        /// <param name="player">Player who uses this card.</param>
        /// <param name="myCard">Virus card</param>
        /// <param name="move">Move to put this virus</param>
        /// <param name="wholemoves"></param>
        /// <returns>Error message if its</returns>
        protected void PlayGameCardVirus(Player player, Card myCard, string move, List<string> wholemoves)
        {
            if (!ProtectiveSuitScenario(player, myCard, move, wholemoves))
            {
                WriteToLog(player.Nickname + " has used a " + myCard + " to " + Players[Scheduler.GetStringInt(move, 0)].Nickname + "'s " +
                    Players[Scheduler.GetStringInt(move, 0)].Body.Items[Scheduler.GetStringInt(move, 2)]);

                Players[Scheduler.GetStringInt(move, 0)].Body.SetVirus(myCard, Scheduler.GetStringInt(move, 2), this);
            }
        }

        /// <summary>
        /// Perform evolved virus move given its card.
        /// </summary>
        /// <param name="player"></param>
        /// <param name="myCard"></param>
        /// <param name="move"></param>
        /// <param name="wholemoves"></param>
        public void PlayGameCardEvolvedVirus(Player player, Card myCard, string move, List<string> wholemoves)
        {
            if (!ProtectiveSuitScenario(player, myCard, move, wholemoves))
            {
                WriteToLog(player.Nickname + " has used a " + myCard + " to " + Players[Scheduler.GetStringInt(move, 0)].Nickname + "'s " +
                Players[Scheduler.GetStringInt(move, 0)].Body.Items[Scheduler.GetStringInt(move, 2)]);

                Players[Scheduler.GetStringInt(move, 0)].Body.SetEvolvedVirus(myCard, Scheduler.GetStringInt(move, 2), this);
            }
        }

        /// <summary>
        /// Performs the quarantine move.
        /// </summary>
        /// <param name="player"></param>
        /// <param name="move"></param>
        public void PlayQuarantine(Player player, string move)
        {
            BodyItem item = player.Body.Items[Scheduler.GetStringInt(move, 2)];
            Card virus = item.Modifiers[0];
            item.Modifiers.Remove(virus);

            WriteToLog(player.Nickname + " has set in quarantine the " + virus + " that belonged to his " + item.ToString());
        }

        /// <summary>
        /// Play the overtime card by a user.
        /// </summary>
        /// <param name="player"></param>
        public void PlayOvertime(Player player)
        {
            WriteToLog(player.Nickname + " has used Overtime.");
            PlayerInOvertime = player.ID;
        }

        /// <summary>
        /// Remove a card from the hand of a player. IT'S NOT GOING to the deck stack but will be in a body.
        /// </summary>
        /// <param name="player">Player who has the hand.</param>
        /// <param name="card">Card to remove</param>
        public void RemoveCardFromHand(Player player, Card card)
        {
            player.Hand.Remove(card);
        }

        /// <summary>
        /// Move one card (given its index) from the hand to the deck stack.
        /// </summary>
        /// <param name="player"></param>
        /// <param name="index"></param>
        public void DiscardFromHand(Player player, int index)
        {
            Card card = player.Hand[index];
            DiscardFromHand(player, card);
        }

        /// <summary>
        /// Move one card from the hand fo the deck stack
        /// </summary>
        /// <param name="player">Player who has the hand</param>
        /// <param name="card">Card to discard</param>
        public void DiscardFromHand(Player player, Card card)
        {
            Discards.Add(card);
            player.Hand.Remove(card);

            WriteToLog(player.Nickname + " has put a " + card + " from his hand to deck.");
        }

        /// <summary>
        /// Move all hand to discards stack.
        /// </summary>
        /// <param name="player">Player who remove its hand</param>
        public void DiscardAllHand(Player player)
        {
            for(int i=player.Hand.Count - 1; i>=0; i--)
            {
                DiscardFromHand(player, i);
            }
        }
        
        /// <summary>
        /// Move to discards one card.
        /// </summary>
        /// <param name="card">Card to move.</param>
        public void MoveToDiscards(Card card)
        {
            Discards.Add(card);
            WriteToLog(card + " has been moved to discard stack.");
        }
        
        /// <summary>
        /// Get a player given its ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Player GetPlayerByID(int id)
        {
            foreach(Player p in Players)
            {
                if(p.ID == id)
                {
                    return p;
                }
            }
            return null;
        }

        /// <summary>
        /// Gets a deep clone of the whole game instance.
        /// </summary>
        /// <typeparam name="Game"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static Game DeepClone<Game>(Game obj)
        {
            using (var ms = new MemoryStream())
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(ms, obj);
                ms.Position = 0;
                Game g = (Game)formatter.Deserialize(ms);
                return g;
            }
        }

        /// <summary>
        /// Writes messages in the logger.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="print"></param>
        /// <returns></returns>
        public bool WriteToLog(string message, bool print = false)
        {
            if (Logger == null)
                return false;
            return Logger.Write(message, print);
        }

        /// <summary>
        /// Returns the players ordered by points.
        /// </summary>
        /// <returns></returns>
        public List<Player> TopPlayers()
        {
            List<Player> copy = new List<Player>();

            foreach(Player p in Players)
            {
                copy.Add(Player.DeepClone(p));
            }

            copy.Sort(new PlayerComparer());

            return copy;
        }
        
        /// <summary>
        /// Indicates if is the turn of a given player.
        /// </summary>
        /// <param name="playerid"></param>
        /// <returns></returns>
        public bool IsMyTurn(int playerid)
        {
            if (CurrentTurn == playerid)
                return true;

            return false;
        }

        /// <summary>
        /// Return a string with the item affected by a player attack.
        /// </summary>
        /// <param name="Me"></param>
        /// <param name="card"></param>
        /// <param name="move"></param>
        /// <returns></returns>
        public string GetMyCardAffectedFromMove(Player Me, Card card, string move)
        {
            string res = "you";
            int index;
            switch (card.Face)
            {
                case Card.CardFace.OrganThief:
                case Card.CardFace.Transplant:
                case Card.CardFace.EvolvedVirus:
                case Card.CardFace.Virus:
                    index = Convert.ToInt32(move.ToCharArray()[2]);
                    res = "your "+Me.Body.Items[0].Organ.ToString();
                    break;
                case Card.CardFace.SecondOpinion:
                case Card.CardFace.LatexGlove:
                    res = "your hand";
                    break;
                case Card.CardFace.MedicalError:
                    res = "your whole body";
                    break;
                case Card.CardFace.Spreading:
                default:
                    res = "you";
                    break;
            }
            return res;
        }

    }
}