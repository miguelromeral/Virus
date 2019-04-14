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
        
        public int Turn { get; set; }

        public Logger logger { get; }
        public Referee Referee { get; }

        public Settings Settings { get; set; }
        #endregion
        
        #region Properties
        public int CurrentTurn
        {
            get { return Turn % Players.Count; }
        }

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
        #endregion

        #region Initializers
        public Game(int numPlayers, bool firstHuman = false)
        {
            logger = new Logger();
            logger.Write("We're getting ready Virus!", true);

            Settings = new Settings(this);
            Settings.LoadGamePreferences();
            Referee = new Referee(this);

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
            Console.WriteLine("Press any key to begin the Virus!");
            Console.ReadLine();

            while (!GameOver)
            {
                PlayTurn();
            }

            logger.Write("The game has been finished.", true);
            logger.Write(ToString(), true);
        }

        public override string ToString()
        {
            string printed = String.Empty;
            printed += "Deck (" + deck.Count + ") | Discarding Stack (" + discards.Count + ")" + Environment.NewLine + Environment.NewLine;

            printed += "Turn # " + (Turn + 1) + Environment.NewLine; 
            for(int i=0; i<Players.Count; i++)
            {
                printed += Players[i];
            }

            return printed;
        }

        public void PlayTurn()
        {
            Player p = Players[CurrentTurn];
            Console.WriteLine(this);
            p.PrintMyOptions();
            if (p.Hand.Count > 0)
            {
                //PrintGameState();
                Console.WriteLine();
                logger.Write("Turn #"+Turn+" (" + p.ShortDescription + ").", true);
                Console.WriteLine("Press any key to continue.");
                Console.ReadLine();
                p.Computer.PlayTurn();
            }
            else
            {
                logger.Write("The player has no cards in his hand. Pass the turn.");
            }
            DrawCardsToFill(p);
            Turn++;
        }

        public void DrawCardsToFill(Player player)
        {
            for(int i=player.Hand.Count; i< Settings.NumberCardInHand; i++)
            {
                player.Hand.Add(DrawNewCard(player));
            }
        }
        

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

        
        public string PlayCardByMove(Player player, Card myCard, string move)
        {
            player.Hand.Remove(myCard);

            switch (myCard.Face)
            {
                case Card.CardFace.Organ:
                    return PlayGameCardOrgan(player, myCard);
                case Card.CardFace.Medicine:
                    return PlayGameCardMedicine(player, myCard, move);
                case Card.CardFace.Virus:
                    return PlayGameCardVirus(Players[Scheduler.GetStringInt(move, 0)], myCard, move);
                case Card.CardFace.Transplant:
                case Card.CardFace.OrganThief:
                case Card.CardFace.Spreading:
                case Card.CardFace.LatexGlove:
                    return null;
                case Card.CardFace.MedicalError:
                    return PlayMedicalError(player, move);
            }
            
            return null;
        }


        public string PlayGameCardOrgan(Player player, Card myCard)
        {
            logger.Write(player.ShortDescription + " has played a " + myCard);
            return player.Body.SetOrgan(myCard);
        }

        public string PlayGameCardMedicine(Player player, Card myCard, string move)
        {
            logger.Write(player.ShortDescription + " has used a "+myCard+" in his " + player.Body.Organs[Scheduler.GetStringInt(move, 2)]);
            return player.Body.SetMedicine(myCard, Scheduler.GetStringInt(move, 2));
        }

        public string PlayGameCardVirus(Player player, Card myCard, string move)
        {
            logger.Write(player.ShortDescription + " has used a "+myCard+" to " + Players[Scheduler.GetStringInt(move, 0)].ShortDescription+"'s "+
                Players[Scheduler.GetStringInt(move, 0)].Body.Organs[Scheduler.GetStringInt(move, 2)]);
            return player.Body.SetVirus(myCard, Scheduler.GetStringInt(move, 2), this);
        }
        


        public string PlayGameCard(Player player, Card myCard)
        {
            List<string> moves = new List<string>();
            int p, c;
            switch (myCard.Face)
            {
                    //    #region PLAY MEDICINE
                    //    case Card.CardFace.Medicine:
                    //        moves = GetListMovements(player, myCard);
                    //        if (moves.Count == 0)
                    //        {
                    //            return "You don't have any organ available to play this medicine.";
                    //        }
                    //        if (moves.Count == 1)
                    //        {
                    //            return player.Body.SetMedicine(myCard, Scheduler.GetStringInt(moves[0], 2));
                    //        }
                    //        if (moves.Count > 1)
                    //        {
                    //            string choosen = reader.RequestMovementChoosen(player, moves);

                    //            if (choosen == null)
                    //                throw new Exception("The input doesn't belong to any available move.");

                    //            return player.Body.SetMedicine(myCard, Scheduler.GetStringInt(choosen, 2));
                    //        }

                    //        break;
                    //    #endregion

                    //    #region PLAY VIRUS
                    //    case Card.CardFace.Virus:
                    //        moves = GetListMovements(player, myCard);
                    //        if (moves.Count == 0)
                    //        {
                    //            return "You don't have any organ available to play this virus.";
                    //        }
                    //        if (moves.Count == 1)
                    //        {
                    //            p = Scheduler.GetStringInt(moves[0], 0);
                    //            c = Scheduler.GetStringInt(moves[0], 2);
                    //            return Players[p].Body.SetVirus(myCard, c, this);
                    //        }
                    //        if (moves.Count > 1)
                    //        {
                    //            string choosen = reader.RequestMovementChoosen(player, moves);

                    //            if (choosen == null)
                    //                throw new Exception("The input doesn't belong to any available move.");

                    //            p = Scheduler.GetStringInt(choosen, 0);
                    //            c = Scheduler.GetStringInt(choosen, 2);

                    //            return Players[p].Body.SetVirus(myCard, c, this);
                    //        }
                    //        break;
                    //    #endregion

                    //    case Card.CardFace.Transplant:
                    //        moves = GetListMovements(player, myCard);
                    //        if (moves.Count == 0)
                    //        {
                    //            return "You currently can't swith any organ between you and your rivals.";
                    //        }
                    //        if (moves.Count == 1)
                    //        {
                    //            return PlayTransplant(moves[0]);
                    //        }
                    //        if (moves.Count > 1)
                    //        {
                    //            int opt = reader.RequestMovementChoosenTransplant(moves, this);
                    //            return PlayTransplant(moves[opt]);
                    //        }
                    //        break;

                    //    #region PLAY ORGAN THIEF
                    //    case Card.CardFace.OrganThief:
                    //        moves = GetListMovements(player, myCard);
                    //        if (moves.Count == 0)
                    //        {
                    //            return "You currently can't steal any body of your rivals.";
                    //        }
                    //        if (moves.Count == 1)
                    //        {
                    //            p = Scheduler.GetStringInt(moves[0], 0);
                    //            c = Scheduler.GetStringInt(moves[0], 2);

                    //            return PlayOrganThief(player, moves[0]);
                    //        }
                    //        if (moves.Count > 1)
                    //        {
                    //            string choosen = reader.RequestMovementChoosen(player, moves);

                    //            if (choosen == null)
                    //                throw new Exception("The input doesn't belong to any available move.");

                    //            return PlayOrganThief(player, choosen);
                    //        }
                    //        break;
                    //    #endregion

                    //    #region PLAY SPREADING
                    //    case Card.CardFace.Spreading:
                    //        List<List<string>> wholeMoves = Scheduler.GetListOfListsSpreadingMoves(GetListMovements(player, myCard));
                    //        if (wholeMoves.Count == 0)
                    //        {
                    //            return "You currently can't spread your virus to any free organ of your rival's bodies.";
                    //        }
                    //        if (wholeMoves.Count > 0)
                    //        {
                    //            List<string> choosen = new List<string>();
                    //            foreach (var move in wholeMoves)
                    //            {
                    //                string input = ProcessSpreadingItem(move);
                    //                if (input == null)
                    //                {
                    //                    return "One or more input in spreading options is not valid.";
                    //                }
                    //                else
                    //                {
                    //                    choosen.Add(input);
                    //                }
                    //            }
                    //            foreach (var move in choosen)
                    //            {
                    //                DoSpreadingOneItem(move);
                    //            }
                    //            return null;
                    //        }
                    //        break;
                    //    #endregion

                    //    #region PLAY LATEX GLOVE
                    //    case Card.CardFace.LatexGlove:
                    //        foreach (Player rival in Players)
                    //        {
                    //            if (!rival.Equals(player))
                    //            {
                    //                DiscardAllHand(rival);
                    //            }
                    //        }
                    //        return null;
                    //    #endregion

                    //    #region PLAY MEDICAL ERROR
                    //    case Card.CardFace.MedicalError:
                    //        moves = GetListMovements(player, myCard);
                    //        if (moves.Count == 0)
                    //        {
                    //            return "You don't have any player to change yours bodies.";
                    //        }
                    //        if (moves.Count == 1)
                    //        {
                    //            return PlayMedicalError(player, moves[0]);
                    //        }
                    //        if (moves.Count > 1)
                    //        {
                    //            string choosen = reader.RequestMovementChoosenMedicalError(player, moves);

                    //            if (choosen == null)
                    //                throw new Exception("The input doesn't belong to any available move.");

                    //            return PlayMedicalError(player, moves[0]);
                    //        }
                    //        break;
                    //    #endregion

                    //    default:
                    //        return " UNKNOWN CARD PLAYED IN GAME";
            }
            return null;
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

            logger.Write(player.ShortDescription + " has put a " + card + " from his hand to deck.");
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