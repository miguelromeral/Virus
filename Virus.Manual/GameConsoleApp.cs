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
        public ReaderInput reader;

        #region Constructor
        public GameConsoleApp(
            int wa,
            bool firstHuman = false) : base(wa, firstHuman)
        {
            reader = new ReaderInput(this);
        }
        #endregion


        public void Play()
        {
            Console.WriteLine("Press any key to begin the Virus!");
            Console.ReadLine();


            //Players[0].Hand[0] = new Card(Card.CardColor.Purple, Card.CardFace.Overtime);
            //Players[0].Hand[1] = new Card(Card.CardColor.Purple, Card.CardFace.ProtectiveSuit);
            //Players[0].Hand[2] = new Card(Card.CardColor.Red, Card.CardFace.Organ);





            //Players[1].Hand[0] = new Card(Card.CardColor.Purple, Card.CardFace.ProtectiveSuit);
            //Players[1].Hand[0] = new Card(Card.CardColor.Purple, Card.CardFace.OrganThief);
            //Players[1].Hand[1] = new Card(Card.CardColor.Yellow, Card.CardFace.Organ);
            //Players[1].Hand[2] = new Card(Card.CardColor.Yellow, Card.CardFace.Organ);

            //Players[1].AI = ArtificialIntelligence.AICategory.Easy;

            //Players[0].Body.SetOrgan(new Card(Card.CardColor.Red, Card.CardFace.Organ));
            ////Players[0].Body.Items[0].NewVirus(new Card(Card.CardColor.Red, Card.CardFace.Virus), this);
            //Players[1].Body.SetOrgan(new Card(Card.CardColor.Red, Card.CardFace.Organ));
            //Players[2].Body.SetOrgan(new Card(Card.CardColor.Red, Card.CardFace.Organ));



            while (!GameOver)
            {
                Player p = Players[CurrentTurn];
                if (p.AI.Equals(ArtificialIntelligence.AICategory.Human))
                {
                    WriteToLog("Turn #" + Turn + " (" + p.Nickname + ").");
                    if (p.Hand.Count > 0)
                    {
                        ReadUserInput();
                    }
                    else
                    {
                        if (PlayerInOvertime != null && PlayerInOvertime == p.ID)
                        {
                            PlayerInOvertime = null;
                        }
                        WriteToLog("The player has no cards in his hand. Pass the turn.");
                    }
                    if (PlayerInOvertime == null || PlayerInOvertime != p.ID)
                    {
                        // Once the player has used (or discarded) cards, fill the hand to the number.
                        DrawCardsToFill(p);
                        Turn++;
                    }
                }
                // IA Turn
                else
                {
                    PlayTurn(WaitingTime == 0, true);
                    //PlayTurn(milis == 0, false);
                    if (WaitingTime != 0)
                    {
                        System.Threading.Thread.Sleep(WaitingTime);
                    }
                }
            }
            Console.WriteLine();

            PrintCurrentGameState();
            WriteToLog("The game has been finished.", true);
            WriteToLog(ToString(), false);
        }

        public void PrintGameState(bool fail = false, bool user = false, string action = Scheduler.ACTION_PLAYING, List<string> moves = null)
        {
            //Console.Clear();
            if (user)
            {
                if (!action.Equals(Scheduler.ACTION_CHOOSING))
                    PrintCurrentGameState();

                if (fail)
                {
                    Console.WriteLine(new string('*', 70));
                    Console.WriteLine("** MOVEMENT NOT ALLOWED. Please, try again. " + new string(' ', 30) + "**");
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
        
        public bool ReadUserInput(bool moveDone = false)
        {
            // IDEA: if the user doesn't input the data right then 10 times, make random turn (via IA)
            try
            {
                Player me = Players[0];
                PrintGameState(moveDone, true);
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
                        PrintGameState(user: true, action: Scheduler.ACTION_DISCARDING);
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
                    if(!PlayGameCardByUser(Players[0], myCardIndex - 1))
                    {
                        throw new Exception();
                    }
                }
                return true;
            }
            catch (Exception e)
            {
                if (!moveDone)
                    return ReadUserInput(false);
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
        
        public bool PlayGameCardByUser(Player player, int index, List<string> moves = null, Card myCard = null)
        {
            if(myCard == null)
            {
                myCard = player.Hand[index];
            }

            if (moves == null)
            {
                moves = Referee.GetListMovements(player, myCard);
            }

            switch (moves.Count)
            {
                case 0: return false;
                case 1:
                    PlayCardByMove(player, myCard, moves[0], moves);
                    return true;
                default:
                    string choosen = null;
                    switch (myCard.Face)
                    {
                        case Card.CardFace.Transplant:
                            choosen = reader.RequestMovementChoosenTransplant(moves, this);
                            break;
                            
                        case Card.CardFace.Spreading:
                            List<List<string>> wholeMoves = Scheduler.GetListOfListsSpreadingMoves(Referee.GetListMovements(player, myCard));
                            if (wholeMoves.Count == 0)
                            {
                                return false;
                            }
                            if (wholeMoves.Count > 0)
                            {
                                List<string> choosenlist = new List<string>();
                                foreach (var move in wholeMoves)
                                {
                                    string input = ProcessSpreadingItem(move);
                                    if (input == null)
                                    {
                                        return false;
                                    }
                                    else
                                    {
                                        choosenlist.Add(input);
                                    }
                                }
                                PlayGameCardSpreading(player, myCard, Scheduler.GetMoveByMultiple(choosenlist), moves);
                                return true;

                            }
                            break;

                        case Card.CardFace.MedicalError:
                        case Card.CardFace.SecondOpinion:
                            choosen = reader.RequestMovementChoosenMedicalError(player, moves, this);
                            break;

                        default:
                            choosen = reader.RequestMovementChoosen(player, moves);
                            break;
                    }

                    if (choosen == null)
                        return false;

                    PlayCardByMove(player, myCard, choosen, moves);
                    return true;
            }
        }


        
        public override bool ProceedProtectiveSuit(Player player, Player rival, Card myCard, string move, List<string> wholemoves)
        {
            bool psused = SomeoneHasDefend();

            bool play = rival.DoIHaveProtectiveSuit();

            if (play) {
                if (rival.AI == ArtificialIntelligence.AICategory.Human)
                {
                    play = reader.ReadDefendFromCard(player, myCard, move, rival);
                }
                else
                {
                    play = rival.Computer.DefendFromCard(player, myCard, move);
                }

                if (play)
                {

                    WriteToLog(rival.Nickname + " has protected with a Protective Suit.", true);



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

                        if (player.AI == ArtificialIntelligence.AICategory.Human)
                        {
                            if(wholemoves.Count == 0) {
                                return true;
                            }
                            return PlayGameCardByUser(player, -1, wholemoves, myCard);
                        }
                        else
                        {
                            move = player.Computer.ChooseBestOptionProtectiveSuit(wholemoves);
                            if (move != null)
                            {
                                PlayCardByMove(player, myCard, move, wholemoves, false);
                            }
                        }
                        
                    }

                    return true;
                }
            }
            return false;
        }
    }
}
