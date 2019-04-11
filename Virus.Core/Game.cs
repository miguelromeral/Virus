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
            logger.Write("We're getting ready Virus!", true);
            deck = Shuffle(InitializeCards());
            logger.Write(deck.Count+" cards shuffled.", true);
            discards = new List<Card>();
            logger.Write("Discard stack created.");
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
                    logger.Write(Scheduler.NUM_ORGANS+" "+color+" organs created.");

                    for (i = 0; i < Scheduler.NUM_MEDICINES; i++)
                    {
                        deck.Add(new Card(color, Card.CardFace.Medicine));
                    }
                    logger.Write(Scheduler.NUM_MEDICINES + " " + color + " medicines created.");

                    for (i = 0; i < Scheduler.NUM_VIRUSES; i++)
                    {
                        deck.Add(new Card(color, Card.CardFace.Virus));
                    }
                    logger.Write(Scheduler.NUM_VIRUSES + " " + color + " viruses created.");

                }
            }
            #endregion

            #region WILDCARD CARDS
            for (i = 0; i < Scheduler.NUM_WILDCARD_ORGANS; i++)
            {
                deck.Add(new Card(Card.CardColor.Wildcard, Card.CardFace.Organ));
            }
            logger.Write(Scheduler.NUM_WILDCARD_ORGANS + " wildcard organs created.");

            for (i = 0; i < Scheduler.NUM_WILDCARD_MEDICINES; i++)
            {
                deck.Add(new Card(Card.CardColor.Wildcard, Card.CardFace.Medicine));
            }
            logger.Write(Scheduler.NUM_WILDCARD_MEDICINES + " wildcard medicines created.");

            for (i = 0; i < Scheduler.NUM_WILDCARD_VIRUSES; i++)
            {
                deck.Add(new Card(Card.CardColor.Wildcard, Card.CardFace.Virus));
            }
            logger.Write(Scheduler.NUM_WILDCARD_VIRUSES + " wildcard viruses created.");
            #endregion
            
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
            logger.Write("Creating players.", true);
            players = new List<Player>();
            for (int i = 0; i < numPlayers; i++)
            {
                var p = new Player(this, i == 0) { ID = i };
                players.Add(p);
                logger.Write("Player with ID "+i+" created. "+((i==0) ? "Human" : "IA: " + p.Ai));
            }
            logger.Write("Dealing cards.", true);
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
            players[0].Body.SetVirus(new Card(Card.CardColor.Red, Card.CardFace.Virus), 0, this);
            players[0].Body.SetOrgan(new Card(Card.CardColor.Yellow, Card.CardFace.Organ));
            players[0].Body.SetVirus(new Card(Card.CardColor.Yellow, Card.CardFace.Virus), 1, this);
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
            Player p = players[Turn];
            if (p.Hand.Count > 0)
            {
                // Human Turn
                if (p.Ai.Equals(ArtificialIntelligence.AICategory.Human))
                {
                    var pt = 1;
                    Console.WriteLine("Your turn (" + pt + ")");
                    ReadUserInput();
                    pt++;
                }
                // IA Turn
                else
                {
                    PrintGameState();
                    Console.WriteLine("Press to continue (It's computer turn!)...");
                    Console.ReadLine();
                    p.Computer.PlayTurn();
                }
            }
            DrawCardsToFill(p);
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
                    myCard = me.Hand[(myCardIndex - 1)];
                    message = PlayGameCardByUser(players[0], myCard);
                    ThrowExceptionIfMessage(message);
                    if(message == null)
                    {
                        DiscardFromHand(me, myCard);
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
            int p, c;
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

                #region PLAY VIRUS
                case Card.CardFace.Virus:
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
                #endregion
                    
                case Card.CardFace.Transplant:
                    moves = GetListMovements(player, myCard);
                    if (moves.Count == 0)
                    {
                        return "You currently can't swith any organ between you and your rivals.";
                    }
                    if (moves.Count == 1)
                    {
                        return PlayTransplant(moves[0]);
                    }
                    if (moves.Count > 1)
                    {
                        int opt = reader.RequestMovementChoosenTransplant(moves, this);
                        return PlayTransplant(moves[opt]);
                    }
                    break;

                #region PLAY ORGAN THIEF
                case Card.CardFace.OrganThief:
                    moves = GetListMovements(player, myCard);
                    if (moves.Count == 0)
                    {
                        return "You currently can't steal any body of your rivals.";
                    }
                    if (moves.Count == 1)
                    {
                        p = Scheduler.GetStringInt(moves[0], 0);
                        c = Scheduler.GetStringInt(moves[0], 2);

                        return PlayOrganThief(player, moves[0]);
                    }
                    if (moves.Count > 1)
                    {
                        string choosen = reader.RequestMovementChoosen(player, moves);

                        if (choosen == null)
                            throw new Exception("The input doesn't belong to any available move.");

                        p = Scheduler.GetStringInt(choosen, 0);
                        c = Scheduler.GetStringInt(choosen, 2);

                        return PlayOrganThief(player, choosen);
                    }
                    break;
                #endregion

                #region PLAY SPREADING
                case Card.CardFace.Spreading:
                    List<List<string>> wholeMoves = GetListMovementsSrepading(player);
                    if (wholeMoves.Count == 0)
                    {
                        return "You currently can't spread your virus to any free organ of your rival's bodies.";
                    }
                    if (wholeMoves.Count > 0)
                    {
                        List<string> choosen = new List<string>();
                        foreach(var move in wholeMoves)
                        {
                            string input = ProcessSpreadingItem(move);
                            if(input == null)
                            {
                                return "One or more input in spreading options is not valid.";
                            }
                            else
                            {
                                choosen.Add(input);
                            }
                        }
                        foreach(var move in choosen)
                        {
                            DoSpreadingOneItem(move);
                        }
                        return null;
                    }
                    break;
                #endregion

                #region PLAY LATEX GLOVE
                case Card.CardFace.LatexGlove:
                    foreach(Player rival in players)
                    {
                        if (!rival.Equals(player))
                        {
                            DiscardAllHand(rival);
                        }
                    }
                    return null;
                #endregion

                #region PLAY MEDICAL ERROR
                case Card.CardFace.MedicalError:
                    moves = GetListMovements(player, myCard);
                    if (moves.Count == 0)
                    {
                        return "You don't have any player to change yours bodies.";
                    }
                    if (moves.Count == 1)
                    {
                        return PlayMedicalError(player, moves[0]);
                    }
                    if (moves.Count > 1)
                    {
                        string choosen = reader.RequestMovementChoosenMedicalError(player, moves);
                        
                        if (choosen == null)
                            throw new Exception("The input doesn't belong to any available move.");

                        return PlayMedicalError(player, moves[0]);
                    }
                    break;
                #endregion

                default:
                    return " UNKNOWN CARD PLAYED IN GAME";
            }
            return "END OF SWITCH";
        }

        public string DoSpreadingOneItem(string moves)
        {
            Player one, two;
            BodyItem bone, btwo;
            int p1, p2, o1, o2;

            p1 = Scheduler.GetStringInt(moves, 0);
            o1 = Scheduler.GetStringInt(moves, 2);
            one = players[p1];
            bone = one.Body.Organs[o1];

            p2 = Scheduler.GetStringInt(moves, 4);
            o2 = Scheduler.GetStringInt(moves, 6);
            two = players[p2];
            btwo = two.Body.Organs[o2];

            Card virus = bone.GetLastModifier();
            bone.Modifiers.Remove(virus);
            btwo.NewVirus(virus, this);
            

            return null;
        }
            

        public string ProcessSpreadingItem(List<string> spreadmoves)
        {
            if(spreadmoves.Count == 1)
            {
                return spreadmoves[0];
            }
            if(spreadmoves.Count > 1)
            {
                return spreadmoves[reader.RequestMovementChoosenSpreading(spreadmoves, this)];
            }
            return null;
        }

        public string PlayTransplant(string move)
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
                one = players[p1];
                two = players[p2];
                bone = one.Body.Organs[o1];
                btwo = two.Body.Organs[o2];

                players[p1].Body.Organs[o1] = btwo;
                players[p2].Body.Organs[o2] = bone;

                return null;
            }
            catch (Exception)
            {
                return "EXCEPTION: CAN'T ABLE TO TRANSPLANT ORGANS.";
            }
        }

        public bool SameColorOrWildcard(Card.CardColor color1, Card.CardColor color2)
        {
            if(color1 == color2 || 
                color1 == Card.CardColor.Wildcard ||
                color2 == Card.CardColor.Wildcard)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public string PlayOrganThief(Player me, string move)
        {
            try
            {
                Player rival = players[Scheduler.GetStringInt(move, 0)];
                BodyItem stealed = rival.Body.Organs[Scheduler.GetStringInt(move, 2)];
                rival.Body.Organs.Remove(stealed);

                me.Body.Organs.Add(stealed);

                return null;
            }
            catch (Exception)
            {
                return "EXCEPTION: CAN'T ABLE TO STEAL ORGAN.";
            }
        }


        public string PlayMedicalError(Player me, string move)
        {
            try
            {
                Player toswitch = players[Scheduler.GetStringInt(move, 0)];

                Body aux = me.Body;
                me.Body = toswitch.Body;
                toswitch.Body = aux;

                return null;
            }
            catch (Exception)
            {
                return "EXCEPTION: CAN'T ABLE TO SWITCH BODIES.";
            }
        }

        public List<string> GetListMovements(Player me, Card myCard)
        {
            List<string> moves = new List<string>();
            Body body;
            int myId;

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
                    myId = me.ID;
                    for (int i = 0; i < players.Count; i++)
                    {
                        Player rival = players[i];
                        if (rival.ID != me.ID)
                        {
                            body = rival.Body;
                            for (int j = 0; j < body.Organs.Count; j++)
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

                case Card.CardFace.Transplant:
                    Player one, two;
                    BodyItem bone, btwo;
                    for (int i = 0; i < players.Count; i++)
                    {
                        for (int j = i + 1; j < players.Count; j++)
                        {
                            one = players[i];
                            two = players[j];

                            for (int x = 0; x < one.Body.Organs.Count; x++)
                            {
                                for (int y = 0; y < two.Body.Organs.Count; y++)
                                {
                                    bone = one.Body.Organs[x];
                                    btwo = two.Body.Organs[y];
                                    if (!one.Body.HaveThisOrgan(btwo.Organ.Color) &&
                                        !two.Body.HaveThisOrgan(bone.Organ.Color))
                                    {
                                        moves.Add(Scheduler.GetManyMoveItem(new string[]
                                        {
                                            Scheduler.GetMoveItem(i, x),
                                            Scheduler.GetMoveItem(j, y)
                                        }));

                                    }
                                }
                            }
                        }
                    }
                    break;

                case Card.CardFace.Spreading:
                    int myCardIndex = 0;
                    Card modifier;
                    foreach (BodyItem item in me.Body.Organs)
                    {
                        modifier = item.GetLastModifier();
                        if (modifier != null)
                        {
                            if (item.Status.Equals(BodyItem.State.Infected))
                            {
                                int j = 0;
                                foreach (Player rival in players)
                                {
                                    if (rival.ID != me.ID)
                                    {
                                        int k = 0;
                                        foreach (BodyItem ri in rival.Body.Organs)
                                        {
                                            if (SameColorOrWildcard(modifier.Color, ri.Organ.Color) &&
                                                ri.Status.Equals(BodyItem.State.Free))
                                            {
                                                moves.Add(Scheduler.GetManyMoveItem(new string[] {
                                                Scheduler.GetMoveItem(me.ID, myCardIndex),
                                                Scheduler.GetMoveItem(j, k)
                                            }));
                                            }
                                            k++;
                                        }
                                    }
                                    j++;
                                }
                            }
                        }
                        myCardIndex++;
                    }
                    break;

                case Card.CardFace.OrganThief:
                    myId = me.ID;
                    for (int i = 0; i < players.Count; i++)
                    {
                        Player rival = players[i];
                        if (rival.ID != me.ID)
                        {
                            body = rival.Body;
                            for (int j = 0; j < body.Organs.Count; j++)
                            {
                                var item = body.Organs[j];
                                if (!me.Body.HaveThisOrgan(item.Organ.Color) && !item.Status.Equals(BodyItem.State.Immunized))
                                {
                                    moves.Add(Scheduler.GetMoveItem(rival.ID, j));
                                }
                            }
                        }
                    }
                    break;

                case Card.CardFace.MedicalError:
                    myId = me.ID;
                    for (int i = 0; i < players.Count; i++)
                    {
                        Player rival = players[i];
                        if (rival.ID != me.ID)
                        {
                            moves.Add(Scheduler.GetMoveItem(rival.ID, 0));
                        }
                    }
                    return moves;
            }


            return moves;
        }


        public List<List<string>> GetListMovementsSrepading(Player me)
        {
            List<List<string>> wholeMoves = new List<List<string>>();
            List<string> moves;

            int myCardIndex = 0;
            Card modifier;
            foreach (BodyItem item in me.Body.Organs)
            {
                modifier = item.GetLastModifier();
                if (modifier != null)
                {
                    if (item.Status.Equals(BodyItem.State.Infected))
                    {
                        moves = new List<string>();
                        int j = 0;
                        foreach (Player rival in players)
                        {
                            if (rival.ID != me.ID)
                            {
                                int k = 0;
                                foreach (BodyItem ri in rival.Body.Organs)
                                {
                                    if (SameColorOrWildcard(modifier.Color, ri.Organ.Color) &&
                                        ri.Status.Equals(BodyItem.State.Free))
                                    {
                                        moves.Add(Scheduler.GetManyMoveItem(new string[] {
                                                Scheduler.GetMoveItem(me.ID, myCardIndex),
                                                Scheduler.GetMoveItem(j, k)
                                            }));
                                    }
                                    k++;
                                }
                            }
                            j++;
                        }
                        if(moves.Count > 0)
                        {
                            wholeMoves.Add(moves);
                        }
                    }
                }
                myCardIndex++;
            }

            return wholeMoves;
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

        public void DiscardFromHand(Player player, Card card)
        {
            discards.Add(card);
            player.Hand.Remove(card);
        }

        public void DiscardAllHand(Player player)
        {
            for(int i=player.Hand.Count - 1; i>=0; i--)
            {
                DiscardFromHand(player, i);
            }
        }
        
        public void MoveToDiscards(Card card)
        {
            discards.Add(card);
        }
        
        #endregion
    }
}