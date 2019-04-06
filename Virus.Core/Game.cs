using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Virus.Core
{
    public class Game
    {
        #region Variables
        private List<Player> players;
        private List<Card> deck;
        private List<Card> discards;
        private int turns;
        public Logger logger;
        private ReaderInput reader;
        #endregion

        #region Properties
        public List<Player> Players
        {
            get { return players; }
        }

        public int Turn
        {
            get { return turns % players.Count; }
        }

        public bool GameOver
        {
            get
            {
                foreach (var p in players)
                {
                    if (p.HealthyOrgans == 4)
                    {
                        return true;
                    }
                }
                return false;
            }
        }
        #endregion
        
        #region Initializers
        public Game(int numPlayers)
        {
            logger = new Logger();
            Console.WriteLine("We're getting ready Virus!");
            Console.WriteLine("Shuffling cards.");
            deck = Shuffle(InitializeCards());
            discards = new List<Card>();
            Console.WriteLine("Initializing players.");
            InitializePlayers(numPlayers);
            reader = new ReaderInput(this);
        }

        public List<Card> InitializeCards()
        {
            List<Card> deck = new List<Card>();
            int i;

            #region ORGANS. MEDICINES & VIRUS BY COLOR
            foreach (Card.CardColor color in (Card.CardColor[])Enum.GetValues(typeof(Card.CardColor)))
            {
                if (color != Card.CardColor.Purple && color != Card.CardColor.Wildcard)
                {
                    for (i = 0; i < Scheduler.NUM_ORGANS; i++)
                    {
                        deck.Add(new Card(color, Card.CardFace.Organ));
                    }
                    for (i = 0; i < Scheduler.NUM_MEDICINES; i++)
                    {
                        deck.Add(new Card(color, Card.CardFace.Medicine));
                    }
                    for (i = 0; i < Scheduler.NUM_VIRUSES; i++)
                    {
                        deck.Add(new Card(color, Card.CardFace.Virus));
                    }
                }
            }
            #endregion

            #region WILDCARD CARDS
            for (i = 0; i < Scheduler.NUM_WILDCARD_ORGANS; i++)
            {
                deck.Add(new Card(Card.CardColor.Wildcard, Card.CardFace.Organ));
            }
            
            for (i = 0; i < Scheduler.NUM_WILDCARD_MEDICINES; i++)
            {
                deck.Add(new Card(Card.CardColor.Wildcard, Card.CardFace.Medicine));
            }
            for (i = 0; i < Scheduler.NUM_WILDCARD_VIRUSES; i++)
            {
                deck.Add(new Card(Card.CardColor.Wildcard, Card.CardFace.Virus));
            }

                     #endregion
                     /*
                     #region THREATMENTS
                     for (i = 0; i < Scheduler.NUM_THREATMENT_SPREADING; i++)
                     {
                         deck.Add(new Card(Card.CardColor.Purple, Card.CardFace.Spreading));
                     }
                     for (i = 0; i < Scheduler.NUM_THREATMENT_ORGANTHIEF; i++)
                     {
                         deck.Add(new Card(Card.CardColor.Purple, Card.CardFace.OrganThief));
                     }
                     for (i = 0; i < Scheduler.NUM_THREATMENT_TRANSPLANT; i++)
                     {
                         deck.Add(new Card(Card.CardColor.Purple, Card.CardFace.Transplant));
                     }
                     for (i = 0; i < Scheduler.NUM_THREATMENT_LATEXGLOVE; i++)
                     {
                         deck.Add(new Card(Card.CardColor.Purple, Card.CardFace.LatexGlove));
                     }
                     for (i = 0; i < Scheduler.NUM_THREATMENT_MEDICALERROR; i++)
                     {
                         deck.Add(new Card(Card.CardColor.Purple, Card.CardFace.MedicalError));
                     }
                     #endregion
         */
            return deck;
        }

        // Source: http://www.vcskicks.com/randomize_array.php
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

        public bool InitializePlayers(int numPlayers)
        {
            Console.WriteLine("Creating players.");
            players = new List<Player>();
            for (int i = 0; i < numPlayers; i++)
            {
                players.Add(new Player(i == 0) { ID = i });
            }
            Console.WriteLine("Dealing cards.");
            for (int i = 0; i < numPlayers; i++)
            {
                List<Card> hand = new List<Card>();
                for (int j = 0; j < Scheduler.NUM_CARDS_HAND; j++)
                {
                    hand.Add(DrawNewCard());
                }

                players[i].NewHand(hand);

            }
            return true;
        }

        #endregion

        #region Methods

        public Card DrawNewCard()
        {
            Card newCard = null;
            try
            {
                Random r = new Random();
                if(deck.Count == 0)
                {
                    deck = Shuffle(discards);
                    discards = new List<Card>();
                }
                if(deck.Count == 0)
                {
                    Console.WriteLine("**** BUG: THERE IS NO ENOUGH CARDS TO DEAL **** ");
                }
                newCard = deck[0];
                deck.RemoveAt(0);
            }
            catch (Exception)
            {
                Console.WriteLine("** There's no cards in deck to pop. **");
                return null;
            }
            return newCard;
        }

        public override string ToString()
        {
            string printed = String.Empty;
            printed += "Deck (" + deck.Count + ") | Discarding Stack (" + discards.Count + ")\n\n";

            printed += "Turn # " + (turns + 1) + "\n"; 
            for(int i=0; i<players.Count; i++)
            {
                printed += " ** Player " + (i + 1)+" **\n";
                printed += players[i];
            }

            return printed;

          /*  Console.WriteLine("-------------------------------");
            Console.WriteLine("First Name | Last Name  |   Age");
            Console.WriteLine("-------------------------------");
            Console.WriteLine(String.Format("{0,-10} | {1,-10} | {2,5}", "Bill", "Gates", 51));
            Console.WriteLine(String.Format("{0,-10} | {1,-10} | {2,5}", "Edna", "Parker", 114));
            Console.WriteLine(String.Format("{0,-10} | {1,-10} | {2,5}", "Johnny", "Depp", 44));
            Console.WriteLine("-------------------------------");*/
        }
        
        public void PassTurn()
        {
            turns++;
        }

        public void Play()
        {
            Console.WriteLine("Press any key to begin the Virus!");
            Console.ReadLine();
            // DATA TO TEST:
            players[0].Body.SetOrgan(new Card(Card.CardColor.Wildcard, Card.CardFace.Organ));
            players[1].Body.SetOrgan(new Card(Card.CardColor.Blue, Card.CardFace.Organ));
            players[1].Body.SetOrgan(new Card(Card.CardColor.Red, Card.CardFace.Organ));
            players[1].Body.SetOrgan(new Card(Card.CardColor.Green, Card.CardFace.Organ));
            players[2].Body.SetOrgan(new Card(Card.CardColor.Yellow, Card.CardFace.Organ));
            players[2].Body.SetOrgan(new Card(Card.CardColor.Red, Card.CardFace.Organ));
            players[2].Body.SetOrgan(new Card(Card.CardColor.Green, Card.CardFace.Organ));

            while (!GameOver)
            {
                PlayTurn();
                PassTurn();
            }
            Console.WriteLine();
        }

        public void PlayTurn()
        {
            // Human Turn
            if(Turn == 0)
            {
                var pt = 1;
                Console.WriteLine("Your turn ("+pt+")");
                ReadUserInput();
                pt++;
            }
            // IA Turn
            else
            {
                PrintGameState();
                Console.WriteLine("Press to continue...");
                //Console.ReadLine();
            }
            DrawCardsToFill(players[Turn]);
        }

        public void DrawCardsToFill(Player player)
        {
            for(int i=player.Hand.Count; i< Scheduler.NUM_CARDS_HAND; i++)
            {
                player.Hand.Add(DrawNewCard());
            }
        }

        public const string ACTION_PLAYING = "Playing";
        public const string ACTION_DISCARDING = "Discarding";
        public const string ACTION_CHOOSING = "ChoosingCars";

        
        public void PrintGameState(string message = null, bool user = false, string action = ACTION_PLAYING, List<string>moves = null)
        {
            //Console.Clear();
            Console.WriteLine("------------------------------------------------------------");

            if (user)
            {
                if(!action.Equals(ACTION_CHOOSING))
                    Console.WriteLine(this);

                if (message != null)
                {
                    Console.WriteLine(new string('*', 70));
                    Console.WriteLine("** MOVEMENT NOT ALLOWED: "+new string(' ', 43)+"**");
                    Console.WriteLine(String.Format("** {0,63} **", message));
                    Console.WriteLine(new string('*', 70));

                }


                switch (action)
                {
                    case ACTION_PLAYING:
                        Console.WriteLine("- Please, press the number of card to play:");
                        players[0].PrintMyOptions(false);
                        break;
                    case ACTION_DISCARDING:
                        Console.WriteLine("- Please, press the number of card to discard:");
                        players[0].PrintMyOptions(true);
                        break;
                    case ACTION_CHOOSING:

                        break;
                }


            }
            else
            {

            }
        }



        public bool ReadUserInput(string message = null, bool moveDone = false)
        {
            // IDEA: if the user doesn't input the data right then 10 times, make random turn (via IA)
            try
            {
                Player me = players[0];
                Card myCard;
                PrintGameState(message, true);
                int myCardIndex = Convert.ToInt32(Console.ReadLine());
                if(myCardIndex < 0 || myCardIndex > Scheduler.NUM_CARDS_HAND)
                {
                    throw new Exception("The number of card is not in range.");
                }
                // Discard
                if (myCardIndex == 0)
                {
                    int todiscard = -1;
                    while(todiscard != 0)
                    {
                        PrintGameState(message, true, ACTION_DISCARDING);
                        if (me.Hand.Count > 0)
                        {
                            todiscard = Convert.ToInt32(Console.ReadLine());
                            if (todiscard > 0 && todiscard <= me.Hand.Count)
                            {
                                DiscardFromHand(me, (todiscard - 1));
                                moveDone = true;
                            }
                            else if (todiscard == 0)
                            {
                                if (me.Hand.Count == Scheduler.NUM_CARDS_HAND)
                                {
                                    throw new Exception("You have to discard, at least, one card. Discarding cancelled");
                                }
                                else
                                {
                                    return true;
                                }
                            }
                            else
                            {
                                throw new Exception("The number doesn't belong to any card of your hand.");
                            }
                        }
                        else
                        {
                            return true;
                        }
                    }
                }
                else
                {
                    myCard = players[0].Hand[(myCardIndex - 1)];
                    message = PlayGameCardByUser(players[0], myCard);
                    ThrowExceptionIfMessage(message);
                    if(message == null)
                    {
                        me.Hand.Remove(myCard);
                    }
                }
                return true;
            }
            catch (Exception ex) 
            {
                if (!moveDone)
                    return ReadUserInput(ex.Message, moveDone);
                else
                    return true;
            }
            
        }
        

        //public string PlayGameCardUser(Player player, Card myCard)
        //{
        //    string message = null;

        //    List<string> moves = GetListMovements(player, myCard);

        //    if(moves.Count == 0)
        //    {
        //        return "Movement not allowed";
        //    }
        //    if(moves.Count == 1)
        //    {
        //        int ip = Scheduler.GetStringInt(moves[0], 0);
        //        int ic = Scheduler.GetStringInt(moves[0], 2);
        //        players[]
        //    }


        //    return message;
        //}




        public string PlayGameCardByUser(Player player, Card myCard)
        {
            List<string> moves = new List<string>();
            switch (myCard.Face)
            {
                #region PLAY ORGAN
                case Card.CardFace.Organ:
                    return player.Body.SetOrgan(myCard);
                #endregion

                #region PLAY MEDICINE
                case Card.CardFace.Medicine:
                    moves = GetListMovements(player, myCard);
                    if(moves.Count == 0)
                    {
                        return "You don't have any organ available to play this medicine.";
                    }
                    if(moves.Count == 1)
                    {
                        return player.Body.SetMedicine(myCard, Scheduler.GetStringInt(moves[0],2));
                    }
                    if (moves.Count > 1)
                    {
                        string choosen = reader.RequestMovementChoosen(player, moves);

                        if (choosen == null)
                            throw new Exception("The input doesn't belong to any available move.");

                        return player.Body.SetMedicine(myCard, Scheduler.GetStringInt(choosen, 2));
                    }
                    break;
                #endregion


                case Card.CardFace.Virus:
                    int p, c;
                    moves = GetListMovements(player, myCard);
                    if (moves.Count == 0)
                    {
                        return "You don't have any organ available to play this virus.";
                    }
                    if (moves.Count == 1)
                    {
                        p = Scheduler.GetStringInt(moves[0], 0);
                        c = Scheduler.GetStringInt(moves[0], 2);
                        return players[p].Body.SetVirus(myCard, c, this);
                    }
                    if (moves.Count > 1)
                    {
                        string choosen = reader.RequestMovementChoosen(player, moves);

                        if (choosen == null)
                            throw new Exception("The input doesn't belong to any available move.");

                        p = Scheduler.GetStringInt(choosen, 0);
                        c = Scheduler.GetStringInt(choosen, 2);
                        
                        return players[p].Body.SetVirus(myCard, c, this);
                    }
                    break;

                #region TO BE DEVELOPED
                case Card.CardFace.Transplant:
                    return "NOT IMPLEMENTED YET";

                case Card.CardFace.OrganThief:
                    return "NOT IMPLEMENTED YET";

                case Card.CardFace.Spreading:
                    return "NOT IMPLEMENTED YET";

                case Card.CardFace.LatexGlove:
                    return "NOT IMPLEMENTED YET";

                case Card.CardFace.MedicalError:
                    return "NOT IMPLEMENTED YET";
                #endregion
                default:
                    return " UNKNOWN CARD PLAYED IN GAME";
            }
            return "END OF SWITCH";
        }
    
        public List<string> GetListMovements(Player me, Card myCard)
        {
            List<string> moves = new List<string>();
            Body body;

            switch (myCard.Face)
            {
                case Card.CardFace.Medicine:
                    body = me.Body;
                    for (int i = 0; i < body.Organs.Count; i++)
                    {
                        var item = body.Organs[i];
                        if (item.CanPlayMedicine(myCard))
                        {
                            moves.Add(Scheduler.GetMoveItem(me.ID, i));
                        }
                    }
                    break;

                case Card.CardFace.Virus:
                    int myId = me.ID;
                    for(int i=0; i<players.Count; i++)
                    {
                        Player rival = players[i];
                        if (rival.ID != me.ID)
                        {
                            body = rival.Body;
                            for(int j=0; j<body.Organs.Count; j++)
                            {
                                var item = body.Organs[j];
                                if (item.CanPlayVirus(myCard))
                                {
                                    moves.Add(Scheduler.GetMoveItem(rival.ID, j));
                                }
                            }
                        }
                    }
                    break;
            }


            return moves;
        }


        private void ThrowExceptionIfMessage(string message = null)
        {
            try
            {
                if (message != null)
                {
                    throw new Exception("** "+message+" **");
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void DiscardFromHand(Player player, int index)
        {
            Card card = player.Hand[index];
            discards.Add(card);
            player.Hand.Remove(card);
        }
        
        public void MoveToDiscards(Card card)
        {
            discards.Add(card);
        }
        
        #endregion
    }
}