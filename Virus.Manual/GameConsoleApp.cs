﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Virus.Core;

namespace Virus.ConsoleApp
{
    class GameConsoleApp : Game
    {
        private ReaderInput reader;

        #region Constructor
        public GameConsoleApp(
            int numPlayers,
            bool firstHuman = false) : base(numPlayers, firstHuman)
        {
            reader = new ReaderInput(this);
        }
        #endregion


        public void Play()
        {
            Console.WriteLine("Press any key to begin the Virus!");
            Console.ReadLine();
            // DATA TO TEST:
            Players[0].Hand[0] = new Card(Card.CardColor.Purple, Card.CardFace.Spreading);

            Players[0].Body.SetOrgan(new Card(Card.CardColor.Wildcard, Card.CardFace.Organ));
            Players[0].Body.SetVirus(new Card(Card.CardColor.Red, Card.CardFace.Virus), 0, this);
            Players[0].Body.SetOrgan(new Card(Card.CardColor.Yellow, Card.CardFace.Organ));
            Players[0].Body.SetVirus(new Card(Card.CardColor.Yellow, Card.CardFace.Virus), 1, this);
            Players[1].Body.SetOrgan(new Card(Card.CardColor.Blue, Card.CardFace.Organ));
            Players[1].Body.SetOrgan(new Card(Card.CardColor.Red, Card.CardFace.Organ));
            Players[1].Body.SetOrgan(new Card(Card.CardColor.Green, Card.CardFace.Organ));
            Players[2].Body.SetOrgan(new Card(Card.CardColor.Yellow, Card.CardFace.Organ));
            Players[2].Body.SetOrgan(new Card(Card.CardColor.Red, Card.CardFace.Organ));
            Players[2].Body.SetOrgan(new Card(Card.CardColor.Green, Card.CardFace.Organ));

            while (!GameOver)
            {
                Player p = Players[CurrentTurn];
                if (p.Ai.Equals(ArtificialIntelligence.AICategory.Human))
                {
                    if (p.Hand.Count > 0)
                    {
                        var pt = 1;
                        Console.WriteLine("Your turn (" + pt + ")");
                        ReadUserInput();
                        pt++;
                    }
                    DrawCardsToFill(p);
                    Turn++;
                }
                // IA Turn
                else
                {
                    PlayTurn();
                }
            }
            Console.WriteLine();
        }

        public void PrintGameState(string message = null, bool user = false, string action = Scheduler.ACTION_PLAYING, List<string> moves = null)
        {
            //Console.Clear();
            Console.WriteLine("------------------------------------------------------------");

            if (user)
            {
                if (!action.Equals(Scheduler.ACTION_CHOOSING))
                    Console.WriteLine(this);

                if (message != null)
                {
                    Console.WriteLine(new string('*', 70));
                    Console.WriteLine("** MOVEMENT NOT ALLOWED: " + new string(' ', 43) + "**");
                    Console.WriteLine(String.Format("** {0,63} **", message));
                    Console.WriteLine(new string('*', 70));

                }

                switch (action)
                {
                    case Scheduler.ACTION_PLAYING:
                        Console.WriteLine("- Please, press the number of card to play:");
                        Players[0].PrintMyOptions(false);
                        break;
                    case Scheduler.ACTION_DISCARDING:
                        Console.WriteLine("- Please, press the number of card to discard:");
                        Players[0].PrintMyOptions(true);
                        break;
                    case Scheduler.ACTION_CHOOSING:

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
                Player me = Players[0];
                Card myCard;
                PrintGameState(message, true);
                int myCardIndex = Convert.ToInt32(Console.ReadLine());
                if (myCardIndex < 0 || myCardIndex > Settings.NumberCardInHand)
                {
                    throw new Exception("The number of card is not in range.");
                }
                // Discard
                if (myCardIndex == 0)
                {
                    int todiscard = -1;
                    while (todiscard != 0)
                    {
                        PrintGameState(message, true, Scheduler.ACTION_DISCARDING);
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
                                if (me.Hand.Count == Settings.NumberCardInHand)
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
                    message = PlayGameCardByUser(Players[0], myCard);
                    ThrowExceptionIfMessage(message);
                    if (message == null)
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


        public string ProcessSpreadingItem(List<string> spreadmoves)
        {
            if (spreadmoves.Count == 1)
            {
                return spreadmoves[0];
            }
            if (spreadmoves.Count > 1)
            {
                return spreadmoves[reader.RequestMovementChoosenSpreading(spreadmoves, this)];
            }
            return null;
        }


        private void ThrowExceptionIfMessage(string message = null)
        {
            try
            {
                if (message != null)
                {
                    throw new Exception("** " + message + " **");
                }
            }
            catch (Exception)
            {
                throw;
            }
        }


        public string PlayGameCardByUser(Player player, Card myCard)
        {
            List<string> moves = new List<string>();
            int p, c;
            switch (myCard.Face)
            {
                #region PLAY ORGAN
                case Card.CardFace.Organ:
                    return PlayGameCardOrgan(player, myCard);
                #endregion

                #region PLAY MEDICINE
                case Card.CardFace.Medicine:
                    moves = Referee.GetListMovements(player, myCard);
                    if (moves.Count == 0)
                    {
                        return "You don't have any organ available to play this medicine.";
                    }
                    if (moves.Count == 1)
                    {
                        return PlayGameCardMedicine(player, myCard, moves[0]);
                    }
                    if (moves.Count > 1)
                    {
                        string choosen = reader.RequestMovementChoosen(player, moves);

                        if (choosen == null)
                            throw new Exception("The input doesn't belong to any available move.");

                        return PlayGameCardMedicine(player, myCard, choosen);
                    }

                    break;
                #endregion

                #region PLAY VIRUS
                case Card.CardFace.Virus:
                    moves = Referee.GetListMovements(player, myCard);
                    if (moves.Count == 0)
                    {
                        return "You don't have any organ available to play this virus.";
                    }
                    if (moves.Count == 1)
                    {
                        p = Scheduler.GetStringInt(moves[0], 0);
                        c = Scheduler.GetStringInt(moves[0], 2);
                        return Players[p].Body.SetVirus(myCard, c, this);
                    }
                    if (moves.Count > 1)
                    {
                        string choosen = reader.RequestMovementChoosen(player, moves);

                        if (choosen == null)
                            throw new Exception("The input doesn't belong to any available move.");

                        p = Scheduler.GetStringInt(choosen, 0);
                        c = Scheduler.GetStringInt(choosen, 2);

                        return Players[p].Body.SetVirus(myCard, c, this);
                    }
                    break;
                #endregion

                #region PLAY TRANSPLANT
                case Card.CardFace.Transplant:
                    moves = Referee.GetListMovements(player, myCard);
                    if (moves.Count == 0)
                    {
                        return "You currently can't swith any organ between you and your rivals.";
                    }
                    if (moves.Count == 1)
                    {
                        return PlayGameCardTransplant(moves[0]);
                    }
                    if (moves.Count > 1)
                    {
                        int opt = reader.RequestMovementChoosenTransplant(moves, this);
                        return PlayGameCardTransplant(moves[opt]);
                    }
                    break;
                #endregion

                #region PLAY ORGAN THIEF
                case Card.CardFace.OrganThief:
                    moves = Referee.GetListMovements(player, myCard);
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
                        
                        return PlayOrganThief(player, choosen);
                    }
                    break;
                #endregion

                #region PLAY SPREADING
                case Card.CardFace.Spreading:
                    List<List<string>> wholeMoves = Scheduler.GetListOfListsSpreadingMoves(Referee.GetListMovements(player, myCard));
                    if (wholeMoves.Count == 0)
                    {
                        return "You currently can't spread your virus to any free organ of your rival's bodies.";
                    }
                    if (wholeMoves.Count > 0)
                    {
                        List<string> choosen = new List<string>();
                        foreach (var move in wholeMoves)
                        {
                            string input = ProcessSpreadingItem(move);
                            if (input == null)
                            {
                                return "One or more input in spreading options is not valid.";
                            }
                            else
                            {
                                choosen.Add(input);
                            }
                        }
                        return PlayGameCardSpreading(Scheduler.GetMoveByMultiple(choosen));
                        
                    }
                    break;
                #endregion

                #region PLAY LATEX GLOVE
                case Card.CardFace.LatexGlove:
                    return PlayLatexGlove(player);
                #endregion

                #region PLAY MEDICAL ERROR
                case Card.CardFace.MedicalError:
                    moves = Referee.GetListMovements(player, myCard);
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


    }
}