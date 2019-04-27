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
        public List<Player> Players;
        /// <summary>
        /// Stack of cards without been played yet.
        /// </summary>
        private List<Card> Deck;
        /// <summary>
        /// Stack of cards that have been already played.
        /// </summary>
        private List<Card> Discards;
        /// <summary>
        /// Number of turns in the current game.
        /// </summary>
        public int Turn { get; set; }
        /// <summary>
        /// Logger class to register every action in the game.
        /// </summary>
        public Logger Logger { get; set; }
        /// <summary>
        /// Referee that indicates all availables moves.
        /// </summary>
        public Referee Referee { get; }
        /// <summary>
        /// Settings parameters of the game.
        /// </summary>
        public Settings Settings { get; set; }
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
        /// Total cards in the game.
        /// </summary>
        public int TotalCardsInGame
        {
            get
            {
                int count = 0;
                foreach (Player p in Players)
                {
                    count += p.Hand.Count;
                    foreach (BodyItem item in p.Body.Items)
                    {
                        count++;
                        count += item.Modifiers.Count;
                    }
                }
                count += Deck.Count;
                count += Discards.Count;
                return count;
            }
        }
#endregion

        #region INITIALIZERS
        /// <summary>
        /// Game constructor. It inits the current game and its parameters and config.
        /// </summary>
        /// <param name="numPlayers">Number of players in the game</param>
        /// <param name="firstHuman">First player is a human</param>
        public Game(int numPlayers, bool firstHuman = false)
        {
            Logger = new Logger();
            WriteToLog("We're getting ready Virus!", true);

            Settings = new Settings(this);
            Settings.LoadGamePreferences();

            Referee = new Referee(this);

            Turn = 1;

            Deck = Shuffle(InitializeCards());
            WriteToLog(Deck.Count+" cards shuffled.", true);
            Discards = new List<Card>();
            WriteToLog("Discard stack created.");
            InitializePlayers(numPlayers, firstHuman);
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
                if (color != Card.CardColor.Purple && color != Card.CardColor.Wildcard)
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

                }
            }
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
            #endregion
            
            #region THREATMENTS
            for (i = 0; i < Settings.NumberThreatmentsSpreading; i++)
            {
                deck.Add(new Card(Card.CardColor.Purple, Card.CardFace.Spreading));
            }
            for (i = 0; i < Settings.NumberThreatmentsOrganThief; i++)
            {
                deck.Add(new Card(Card.CardColor.Purple, Card.CardFace.OrganThief));
            }
            for (i = 0; i < Settings.NumberThreatmentsTransplant; i++)
            {
                deck.Add(new Card(Card.CardColor.Purple, Card.CardFace.Transplant));
            }
            for (i = 0; i < Settings.NumberThreatmentsLatexGlove; i++)
            {
                deck.Add(new Card(Card.CardColor.Purple, Card.CardFace.LatexGlove));
            }
            for (i = 0; i < Settings.NumberThreatmentsMedicalError; i++)
            {
                deck.Add(new Card(Card.CardColor.Purple, Card.CardFace.MedicalError));
            }
            #endregion

            return deck;
        }
  
        /// <summary>
        /// Inits all the players and set the main values.
        /// </summary>
        /// <param name="numPlayers">Number of players in the game.</param>
        /// <param name="firstHuman">First player is human</param>
        /// <returns></returns>
        private bool InitializePlayers(int numPlayers, bool firstHuman = false)
        {
            WriteToLog("Creating players.", true);
            Players = new List<Player>();
            for (int i = 0; i < numPlayers; i++)
            {
                var p = new Player(this, (i == 0 && firstHuman)) { ID = i };
                Players.Add(p);
                WriteToLog("Player with ID " + i + " created. " + ((i == 0 && firstHuman) ? "Human" : "IA: " + p.AI));
            }
            WriteToLog("Dealing cards.", true);
            for (int i = 0; i < numPlayers; i++)
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
        /// Shuffle the whole deck stack.
        /// </summary>
        /// <see cref="http://www.vcskicks.com/randomize_array.php"/>
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
        /// <returns>Card recovered from the deck</returns>
        public Card DrawNewCard(Player me)
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
                newCard = Deck[0];
                WriteToLog(me.ShortDescription + " draws a new card: " + newCard);
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
        /// Start the current game.
        /// </summary>
        /// <param name="milis">Number of miliseconds to wait for the next move automatically. If its 0, you'll have to press a key</param>
        public void Start(int milis = 0) {
            Console.WriteLine("Press any key to begin the Virus!");
            Console.ReadLine();

            //Players[1].Body.SetOrgan(new Card(Card.CardColor.Red, Card.CardFace.Organ));
            //Players[1].Body.SetMedicine(this, new Card(Card.CardColor.Red, Card.CardFace.Medicine));
            //Players[0].Hand[0] = new Card(Card.CardColor.Red, Card.CardFace.Virus);

            while (!GameOver)
            {
                Console.Clear();
                PlayTurn(milis == 0, true);
                if(milis != 0)
                {
                    System.Threading.Thread.Sleep(milis);
                }
            }

            WriteToLog("The game has been finished.", true);

            PrintCurrentGameState();

            //WriteToLog(ToString(), true);
        }


        /// <summary>
        /// Gets a string overview of the game.
        /// </summary>
        /// <returns>String overview of the game.</returns>
        public override string ToString()
        {
            return "Here it'll come the current state!";
        }

        public void PrintCurrentGameState()
        {

            int j = 1;
            foreach (var p in TopPlayers())
            {
                Console.Write("[" + j + "][" + String.Format("{0,20}", p.ShortDescription) + "]" + Environment.NewLine);
                j++;
            }
            Console.Write(Environment.NewLine);

            Console.Write("+------------+------------+----------------+-------------------------------+" + Environment.NewLine);

            Console.Write(String.Format("| #Turn: {0,3} | #Deck: {1,3} | #Discards: {2,3} | #Total Cards: {3,3}             |",
                Turn, Deck.Count, Discards.Count, TotalCardsInGame) + Environment.NewLine);
            Console.Write("+------------+------------+----------------+-------------------------------+" + Environment.NewLine);


            for (int x = 0; x < Players.Count; x++)
            {
                Player p = Players[x];

                Console.Write(String.Format("| {0,10} | {1,20} | AI: {2,11} | Body Pts: <{3,6}> |",
                    (CurrentTurn == x ? "--------->" : ""),
                    p.ShortDescription,
                    p.AI.ToString(),
                    p.Body.Points) + Environment.NewLine);

                for (int y = 0; y < p.Body.Items.Count; y++)
                {
                    BodyItem item = p.Body.Items[y];
                    //printed += "+------------+------------+----------------+-------------------------------+" + Environment.NewLine;
                    Console.Write(String.Format("| {0,1}.   ", (y + 1)));

                    ChangeConsoleOutput(item.Organ.Color);
                    Console.Write(String.Format("{0,14}: ", item.Organ.ToString()));


                    ChangeConsoleOutput(item.Organ.Color);

                    int padd = 0;
                    foreach(Card mod in item.Modifiers)
                    {
                        ChangeConsoleOutput(mod.Color);
                        Console.Write("{0}", mod.ToStringShort());
                        padd+=4;
                    }

                    ChangeConsoleOutput(null);
                    while (padd < 38)
                    {
                        Console.Write(" ");
                        padd++;
                    }

                    Console.Write(String.Format("     +={0,6} |", item.Points) + Environment.NewLine);

                }


                Console.Write("+------------+------------+----------------+-------------------------------+" + Environment.NewLine);
            }
        }

        public void ChangeConsoleOutput(Card.CardColor? color)
        {
            if(color == null)
            {
                Console.BackgroundColor = ConsoleColor.Black;
                Console.ForegroundColor = ConsoleColor.White;
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
                    Console.BackgroundColor = ConsoleColor.Blue;
                    Console.ForegroundColor = ConsoleColor.White;
                    break;
            }
        }

        /// <summary>
        /// Play turn by the computer
        /// </summary>
        /// <param name="wait">True if the user will have to press a key to continue with the move.</param>
        /// <param name="printHand">True if print the player hand.</param>
        public void PlayTurn(bool wait = false, bool printHand = false)
        {
            Player p = Players[CurrentTurn];

            PrintCurrentGameState();

            if (printHand)
            {
                p.PrintMyOptions();
            }

            Console.WriteLine();
            WriteToLog("Turn #" + Turn + " (" + p.ShortDescription + ").", true);
            if (wait)
            {
                Console.WriteLine("Press any key to continue.");
                Console.ReadLine();
            }

            if (p.Hand.Count > 0)
            {
                p.Computer.PlayTurn();
            }
            else
            {
                WriteToLog("The player has no cards in his hand. Pass the turn.");
            }
            // Once the player has used (or discarded) cards, fill the hand to the number.
            DrawCardsToFill(p);
            Turn++;
        }

        /// <summary>
        /// Draws as many cards needed until fill the player hand.
        /// </summary>
        /// <param name="player"></param>
        public void DrawCardsToFill(Player player)
        {
            for(int i=player.Hand.Count; i< Settings.NumberCardInHand; i++)
            {
                player.Hand.Add(DrawNewCard(player));
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

            WriteToLog(one.ShortDescription+" has spread his "+virus+" from his "+bone+" to "+two.ShortDescription+"'s "+btwo);

            return null;
        }
        
        /// <summary>
        /// Do a transplant in the game. Switch two body items.
        /// </summary>
        /// <param name="move">Move to spread</param>
        /// <returns>Error message if couldn't be switched</returns>
        public string PlayGameCardTransplant(string move)
        {
            try
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

                WriteToLog(one.ShortDescription + " has transplantated his " + bone + " by the " + two.ShortDescription + "'s organ "+btwo);

                return null;
            }
            catch (Exception)
            {
                return "EXCEPTION: CAN'T ABLE TO TRANSPLANT ORGANS.";
            }
        }
        
        /// <summary>
        /// Steal one body item to one player.
        /// </summary>
        /// <param name="me">Player to receive the new body item</param>
        /// <param name="move">Move that indicates who and what to steal</param>
        /// <returns>Error message if there is</returns>
        public string PlayOrganThief(Player me, string move)
        {
            try
            {
                Player rival = Players[Scheduler.GetStringInt(move, 0)];
                BodyItem stealed = rival.Body.Items[Scheduler.GetStringInt(move, 2)];
                rival.Body.Items.Remove(stealed);

                me.Body.Items.Add(stealed);

                WriteToLog(me.ShortDescription+" has stealed the "+rival.ShortDescription+"'s organ "+stealed);

                return null;
            }
            catch (Exception)
            {
                return "EXCEPTION: CAN'T ABLE TO STEAL ORGAN.";
            }
        }
        
        /// <summary>
        /// Remove any hand of the players but me.
        /// </summary>
        /// <param name="me">Player who has used the card.</param>
        /// <returns>Error message if it is</returns>
        public string PlayLatexGlove(Player me)
        {
            foreach (Player rival in Players)
            {
                if (rival.ID != me.ID)
                {
                    WriteToLog(rival.ShortDescription + " is going to lost his hand due to a latex glove played by " + me.ShortDescription);
                    DiscardAllHand(rival);
                }
            }
            return null;
        }

        /// <summary>
        /// Switch two bodies.
        /// </summary>
        /// <param name="me">Player who has used the card.</param>
        /// <param name="move">Move that indicates who switch the whole body</param>
        /// <returns></returns>
        public string PlayMedicalError(Player me, string move)
        {
            try
            {
                Player toswitch = Players[Scheduler.GetStringInt(move, 0)];

                Body aux = me.Body;
                me.Body = toswitch.Body;
                toswitch.Body = aux;

                WriteToLog(me.ShortDescription+" has switched his body by the "+toswitch.ShortDescription+"'s one.");

                return null;
            }
            catch (Exception)
            {
                return "EXCEPTION: CAN'T ABLE TO SWITCH BODIES.";
            }
        }

        /// <summary>
        /// Play a card in the game given the move choosen.
        /// </summary>
        /// <param name="player">Player who plays the card</param>
        /// <param name="myCard">Card used</param>
        /// <param name="move">Move with the option selected</param>
        /// <returns></returns>
        public string PlayCardByMove(Player player, Card myCard, string move)
        {
            switch (myCard.Face)
            {
                case Card.CardFace.Organ:
                    RemoveCardFromHand(player, myCard);
                    return PlayGameCardOrgan(player, myCard);

                case Card.CardFace.Medicine:
                    RemoveCardFromHand(player, myCard);
                    return PlayGameCardMedicine(player, myCard, move);

                case Card.CardFace.Virus:
                    //p = Scheduler.GetStringInt(move, 0);
                    //c = Scheduler.GetStringInt(move, 2);
                    //if(Players[p].Body.Items[c].Modifiers.Count == 0)
                    //{
                        RemoveCardFromHand(player, myCard);
                    //}
                    return PlayGameCardVirus(player, myCard, move);

                case Card.CardFace.Transplant:
                    DiscardFromHand(player, myCard);
                    return PlayGameCardTransplant(move);

                case Card.CardFace.OrganThief:
                    DiscardFromHand(player, myCard);
                    return PlayOrganThief(player, move);

                case Card.CardFace.Spreading:
                    DiscardFromHand(player, myCard);
                    return PlayGameCardSpreading(move);

                case Card.CardFace.LatexGlove:
                    DiscardFromHand(player, myCard);
                    return PlayLatexGlove(player);

                case Card.CardFace.MedicalError:
                    DiscardFromHand(player, myCard);
                    return PlayMedicalError(player, move);
            }
            
            return null;
        }

        /// <summary>
        /// Play a spreading card given the move selected.
        /// </summary>
        /// <param name="move">All moves to spreading in only one string</param>
        /// <returns>Error message if its</returns>
        public string PlayGameCardSpreading(string move)
        {
            // All move is in the same string. Here we split and process each one.
            string[] choosen = move.Split(Scheduler.MULTI_MOVE_SEPARATOR);
            for(int i=0; i <= (choosen.Length / 2); i+=2){
                string m = Scheduler.GetManyMoveItem(new string[] { choosen[i], choosen[i + 1] });
                DoSpreadingOneItem(m);
            }
            return null;
        }

        /// <summary>
        /// Plays an organ card to the player.
        /// </summary>
        /// <param name="player">Player who uses it</param>
        /// <param name="myCard">Organ card</param>
        /// <returns>Error message if its</returns>
        public string PlayGameCardOrgan(Player player, Card myCard)
        {
            WriteToLog(player.ShortDescription + " has played a " + myCard);
            return player.Body.SetOrgan(myCard);
        }

        /// <summary>
        /// Play medicine card to one player.
        /// </summary>
        /// <param name="player">Player to use the card</param>
        /// <param name="myCard">Medicine card</param>
        /// <param name="move">Move to indicates in which organ uses it</param>
        /// <returns></returns>
        public string PlayGameCardMedicine(Player player, Card myCard, string move)
        {
            WriteToLog(player.ShortDescription + " has used a "+myCard+" in his " + player.Body.Items[Scheduler.GetStringInt(move, 2)]);
            return player.Body.SetMedicine(this, myCard, Scheduler.GetStringInt(move, 2));
        }

        /// <summary>
        /// Play virus from one player.
        /// </summary>
        /// <param name="player">Player who uses this card.</param>
        /// <param name="myCard">Virus card</param>
        /// <param name="move">Move to put this virus</param>
        /// <returns>Error message if its</returns>
        public string PlayGameCardVirus(Player player, Card myCard, string move)
        {
            WriteToLog(player.ShortDescription + " has used a "+myCard+" to " + Players[Scheduler.GetStringInt(move, 0)].ShortDescription+"'s "+
                Players[Scheduler.GetStringInt(move, 0)].Body.Items[Scheduler.GetStringInt(move, 2)]);
            
            return Players[Scheduler.GetStringInt(move, 0)].Body.SetVirus(myCard, Scheduler.GetStringInt(move, 2), this);
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

            WriteToLog(player.ShortDescription + " has put a " + card + " from his hand to deck.");
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


        public bool WriteToLog(string message, bool print = false)
        {
            if (Logger == null)
                return false;
            return Logger.Write(message, print);
        }


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
    }
}