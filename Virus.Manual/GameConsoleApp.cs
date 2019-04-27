﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Virus.Core;

namespace Virus.ConsoleApp
{
    [Serializable]
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


        public void Play(int milis)
        {
            Console.WriteLine("Press any key to begin the Virus!");
            Console.ReadLine();


            //Players[1].Body.SetOrgan(new Card(Card.CardColor.Red, Card.CardFace.Organ));
            //Players[1].Body.SetOrgan(new Card(Card.CardColor.Yellow, Card.CardFace.Organ));
            //Players[1].Body.SetMedicine(this, new Card(Card.CardColor.Wildcard, Card.CardFace.Medicine), 1);
            //Players[0].Hand[0] = new Card(Card.CardColor.Red, Card.CardFace.Virus);

            while (!GameOver)
            {
                Player p = Players[CurrentTurn];
                if (p.AI.Equals(ArtificialIntelligence.AICategory.Human))
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
                    PlayTurn(milis == 0, true);
                    if (milis != 0)
                    {
                        System.Threading.Thread.Sleep(milis);
                    }
                    else
                    {
                        Console.WriteLine("Press any key to begin the Virus!");
                        Console.ReadLine();
                    }
                }
            }
            Console.WriteLine();
            
            WriteToLog("The game has been finished.", true);
            WriteToLog(ToString(), true);
        }

        public void PrintGameState(string message = null, bool user = false, string action = Scheduler.ACTION_PLAYING, List<string> moves = null)
        {
            //Console.Clear();
            Console.WriteLine("------------------------------------------------------------");


            if (user)
            {
                if (!action.Equals(Scheduler.ACTION_CHOOSING))
                    PrintCurrentGameState();

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
                Players[CurrentTurn].PrintMyOptions(true);
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
            switch (myCard.Face)
            {
                #region PLAY ORGAN
                case Card.CardFace.Organ:

                    return PlayCardByMove(player, myCard, null);
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
                        return PlayCardByMove(player, myCard, moves[0]);
                    }
                    if (moves.Count > 1)
                    {
                        string choosen = reader.RequestMovementChoosen(player, moves);

                        if (choosen == null)
                            throw new Exception("The input doesn't belong to any available move.");

                        return PlayCardByMove(player, myCard, choosen);
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
                        return PlayCardByMove(player, myCard, moves[0]);
                    }
                    if (moves.Count > 1)
                    {
                        string choosen = reader.RequestMovementChoosen(player, moves);

                        if (choosen == null)
                            throw new Exception("The input doesn't belong to any available move.");
                        
                        return PlayCardByMove(player, myCard, choosen);
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
                        return PlayCardByMove(player, myCard, moves[0]);
                    }
                    if (moves.Count > 1)
                    {
                        int opt = reader.RequestMovementChoosenTransplant(moves, this);
                        return PlayCardByMove(player, myCard, moves[opt]);
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
                        return PlayCardByMove(player, myCard, moves[0]);
                    }
                    if (moves.Count > 1)
                    {
                        string choosen = reader.RequestMovementChoosen(player, moves);

                        if (choosen == null)
                            throw new Exception("The input doesn't belong to any available move.");

                        return PlayCardByMove(player, myCard, choosen);
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
                    return PlayCardByMove(player, myCard, null);
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
                        return PlayCardByMove(player, myCard, moves[0]);
                    }
                    if (moves.Count > 1)
                    {
                        string choosen = reader.RequestMovementChoosenMedicalError(player, moves, this);

                        if (choosen == null)
                            throw new Exception("The input doesn't belong to any available move.");

                        return PlayCardByMove(player, myCard, choosen);
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
