using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Virus.Core
{
    public class Game
    {
        #region Variables
        public List<Player> Players;
        private List<Card> deck;
        private List<Card> discards;
        public int turns;
        public Logger logger { get; }

        public Settings Settings { get; set; }
        #endregion
        
        #region Properties
        public int Turn
        {
            get { return turns % Players.Count; }
        }

        public bool GameOver
        {
            get
            {
                foreach (var p in Players)
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
        public Game(int numPlayers, bool firstHuman = false)
        {
            logger = new Logger();
            logger.Write("We're getting ready Virus!", true);

            Settings = new Settings(this);
            Settings.LoadGamePreferences();
            
            deck = Shuffle(InitializeCards());
            logger.Write(deck.Count+" cards shuffled.", true);
            discards = new List<Card>();
            logger.Write("Discard stack created.");
            InitializePlayers(numPlayers, firstHuman);
        }

        #region Methods called by constructor

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
                    logger.Write(Settings.NumberOrgans +" "+color+" organs created.");

                    for (i = 0; i < Settings.NumberMedicines; i++)
                    {
                        deck.Add(new Card(color, Card.CardFace.Medicine));
                    }
                    logger.Write(Settings.NumberMedicines + " " + color + " medicines created.");

                    for (i = 0; i < Settings.NumberViruses; i++)
                    {
                        deck.Add(new Card(color, Card.CardFace.Virus));
                    }
                    logger.Write(Settings.NumberViruses + " " + color + " viruses created.");

                }
            }
            #endregion

            #region WILDCARD CARDS
            for (i = 0; i < Settings.NumberWildcardOrgans; i++)
            {
                deck.Add(new Card(Card.CardColor.Wildcard, Card.CardFace.Organ));
            }
            logger.Write(Settings.NumberWildcardOrgans + " wildcard organs created.");

            for (i = 0; i < Settings.NumberWildcardMedicines; i++)
            {
                deck.Add(new Card(Card.CardColor.Wildcard, Card.CardFace.Medicine));
            }
            logger.Write(Settings.NumberWildcardMedicines + " wildcard medicines created.");

            for (i = 0; i < Settings.NumberWildcardViruses; i++)
            {
                deck.Add(new Card(Card.CardColor.Wildcard, Card.CardFace.Virus));
            }
            logger.Write(Settings.NumberWildcardViruses + " wildcard viruses created.");
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
  
        private bool InitializePlayers(int numPlayers, bool firstHuman = false)
        {
            logger.Write("Creating players.", true);
            Players = new List<Player>();
            for (int i = 0; i < numPlayers; i++)
            {
                var p = new Player(this, (i == 0 && firstHuman)) { ID = i };
                Players.Add(p);
                logger.Write("Player with ID " + i + " created. " + ((i == 0 && firstHuman) ? "Human" : "IA: " + p.Ai));
            }
            logger.Write("Dealing cards.", true);
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

        #endregion


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

        #endregion

        #region Methods

        public Card DrawNewCard(Player me)
        {
            Card newCard = null;
            try
            {
                Random r = new Random();
                if(deck.Count == 0)
                {
                    logger.Write("Deck is already empty. We have to redraw the discards.");
                    deck = Shuffle(discards);
                    discards = new List<Card>();
                }
                newCard = deck[0];
                logger.Write(me.ShortDescription + " draws a new card: " + newCard);
                deck.RemoveAt(0);
            }
            catch (Exception)
            {
                logger.Write("** There's no cards in deck to pop. **", true);
                return null;
            }
            return newCard;
        }

        public void Start() {
            Console.WriteLine("In progress!");
        }

        public override string ToString()
        {
            string printed = String.Empty;
            printed += "Deck (" + deck.Count + ") | Discarding Stack (" + discards.Count + ")\n\n";

            printed += "Turn # " + (turns + 1) + "\n"; 
            for(int i=0; i<Players.Count; i++)
            {
                printed += Players[i];
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

        public void PlayTurn()
        {
            Player p = Players[Turn];
            logger.Write("It's "+ p.ShortDescription +" turn!");
            if (p.Hand.Count > 0)
            {
                //PrintGameState();
                Console.WriteLine("Press to continue (It's computer turn!)...");
                Console.ReadLine();
                p.Computer.PlayTurn();
            }
            else
            {
                logger.Write("The player has no cards in his hand. Pass the turn.");
            }
            DrawCardsToFill(p);
            turns++;
        }

        public void DrawCardsToFill(Player player)
        {
            for(int i=player.Hand.Count; i< Settings.NumberCardInHand; i++)
            {
                player.Hand.Add(DrawNewCard(player));
            }
        }

        public const string ACTION_PLAYING = "Playing";
        public const string ACTION_DISCARDING = "Discarding";
        public const string ACTION_CHOOSING = "ChoosingCars";

        
        
        

        public string DoSpreadingOneItem(string moves)
        {
            Player one, two;
            BodyItem bone, btwo;
            int p1, p2, o1, o2;

            p1 = Scheduler.GetStringInt(moves, 0);
            o1 = Scheduler.GetStringInt(moves, 2);
            one = Players[p1];
            bone = one.Body.Organs[o1];

            p2 = Scheduler.GetStringInt(moves, 4);
            o2 = Scheduler.GetStringInt(moves, 6);
            two = Players[p2];
            btwo = two.Body.Organs[o2];

            Card virus = bone.GetLastModifier();
            bone.Modifiers.Remove(virus);
            btwo.NewVirus(virus, this);

            logger.Write(one.ShortDescription+" has spread his "+virus+" from his "+bone+" to "+two.ShortDescription+"'s "+btwo);

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
                one = Players[p1];
                two = Players[p2];
                bone = one.Body.Organs[o1];
                btwo = two.Body.Organs[o2];

                Players[p1].Body.Organs[o1] = btwo;
                Players[p2].Body.Organs[o2] = bone;

                logger.Write(one.ShortDescription + " has transplantated his " + bone + " by the " + two.ShortDescription + "'s organ "+btwo);

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
                Player rival = Players[Scheduler.GetStringInt(move, 0)];
                BodyItem stealed = rival.Body.Organs[Scheduler.GetStringInt(move, 2)];
                rival.Body.Organs.Remove(stealed);

                me.Body.Organs.Add(stealed);

                logger.Write(me.ShortDescription+" has stealed the "+rival.ShortDescription+"'s organ "+stealed);

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
                Player toswitch = Players[Scheduler.GetStringInt(move, 0)];

                Body aux = me.Body;
                me.Body = toswitch.Body;
                toswitch.Body = aux;

                logger.Write(me.ShortDescription+" has switched his body by the "+toswitch.ShortDescription+"'s one.");

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
                    for (int i = 0; i < Players.Count; i++)
                    {
                        Player rival = Players[i];
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
                    for (int i = 0; i < Players.Count; i++)
                    {
                        for (int j = i + 1; j < Players.Count; j++)
                        {
                            one = Players[i];
                            two = Players[j];

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
                                foreach (Player rival in Players)
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
                    for (int i = 0; i < Players.Count; i++)
                    {
                        Player rival = Players[i];
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
                    for (int i = 0; i < Players.Count; i++)
                    {
                        Player rival = Players[i];
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
                        foreach (Player rival in Players)
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


        public void DiscardFromHand(Player player, int index)
        {
            Card card = player.Hand[index];
            DiscardFromHand(player, card);
        }

        public void DiscardFromHand(Player player, Card card)
        {
            discards.Add(card);
            player.Hand.Remove(card);

            logger.Write(player.ShortDescription + " has discarded a " + card + " from his hand.");
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
            logger.Write(card + " has been moved to discard stack.");
        }

        #endregion


    }
}