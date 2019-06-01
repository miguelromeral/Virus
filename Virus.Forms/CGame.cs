using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Virus.Core;

namespace Virus.Forms
{
    [Serializable]
    public class CGame : Game
    {
        public CGame(int numPlayers, int waitingtime, Logger log, bool firstHuman = false)
            : base(numPlayers, waitingtime, firstHuman, log)
        {
            //Players[0].Body.SetOrgan(new Card(Card.CardColor.Red, Card.CardFace.Organ));
            //Players[0].Body.SetOrgan(new Card(Card.CardColor.Blue, Card.CardFace.Organ));
            //Players[0].Body.SetOrgan(new Card(Card.CardColor.Green, Card.CardFace.Organ));
            //Players[0].Hand[0] = new Card(Card.CardColor.Purple, Card.CardFace.ProtectiveSuit);
            //Players[0].Hand[1] = new Card(Card.CardColor.Purple, Card.CardFace.ProtectiveSuit);
            //Players[0].Hand[2] = new Card(Card.CardColor.Purple, Card.CardFace.ProtectiveSuit);

        }

        public bool PlayGameCardByUser(Player player, int index,
            List<string> moves = null, Card myCard = null)
        {
            if (myCard == null)
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
                    return false;
                    //string choosen = null;
                    //switch (myCard.Face)
                    //{
                    //    case Card.CardFace.Transplant:
                    //        choosen = reader.RequestMovementChoosenTransplant(moves, this);
                    //        break;

                    //    case Card.CardFace.Spreading:
                    //        List<List<string>> wholeMoves = Scheduler.GetListOfListsSpreadingMoves(Referee.GetListMovements(player, myCard));
                    //        if (wholeMoves.Count == 0)
                    //        {
                    //            return false;
                    //        }
                    //        if (wholeMoves.Count > 0)
                    //        {
                    //            List<string> choosenlist = new List<string>();
                    //            foreach (var move in wholeMoves)
                    //            {
                    //                string input = ProcessSpreadingItem(move);
                    //                if (input == null)
                    //                {
                    //                    return false;
                    //                }
                    //                else
                    //                {
                    //                    choosenlist.Add(input);
                    //                }
                    //            }
                    //            PlayGameCardSpreading(player, myCard, Scheduler.GetMoveByMultiple(choosenlist), moves);
                    //            return true;

                    //        }
                    //        break;

                    //    case Card.CardFace.MedicalError:
                    //    case Card.CardFace.SecondOpinion:
                    //        choosen = reader.RequestMovementChoosenMedicalError(player, moves, this);
                    //        break;

                    //    default:
                    //        choosen = reader.RequestMovementChoosen(player, moves);
                    //        break;
                    //}

                    //if (choosen == null)
                    //    return false;

                    //PlayCardByMove(player, myCard, choosen, moves);
                    //return true;
            }
        }

        
        public override bool ProceedProtectiveSuit(Player player, Player rival, Card myCard, string move, List<string> wholemoves)
        {
            bool psused = SomeoneHasDefend();

            bool play = rival.DoIHaveProtectiveSuit();

            if (play)
            {
                if (rival.AI == ArtificialIntelligence.AICategory.Human)
                {
                    string warning = String.Format(
                        "{0} is trying to play a {1} against {2}. Do you want to protect?",
                        player.ShortDescription, myCard.ToString(), GetMyCardAffectedFromMove(rival, myCard, move));
                    
                    play = (MessageBox.Show(warning, "Protective Suit chance",
                        MessageBoxButtons.YesNo, MessageBoxIcon.Question,
                        MessageBoxDefaultButton.Button1) == System.Windows.Forms.DialogResult.Yes);
                    
                    if (play)
                    {
                        int index = -1;
                        for (int i = 0; i < rival.Hand.Count; i++)
                        {
                            if (rival.Hand[i].Face == Card.CardFace.ProtectiveSuit)
                            {
                                index = i;
                            }
                        }
                        DiscardFromHand(rival, index);
                        rival.PlayedProtectiveSuit = true;
                    }
                }
                else
                {
                    play = rival.Computer.DefendFromCard(player, myCard, move);
                }

                if (play)
                {

                    WriteToLog(rival.ShortDescription + " has protected with a Protective Suit.", true);



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
                            if (wholemoves.Count == 0)
                            {
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
