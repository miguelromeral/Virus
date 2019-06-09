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
        public CGame(int numPlayers, int waitingtime, TextBox log, bool firstHuman = false)
            : base(waitingtime, firstHuman, new CLogger(log))
        {
            //Players[0].Body.SetOrgan(new Card(Card.CardColor.Red, Card.CardFace.Organ));
            //Players[0].Body.Items[0].NewEvolvedVirus(new Card(Card.CardColor.Red, Card.CardFace.EvolvedVirus), this);
            //Players[0].Body.SetOrgan(new Card(Card.CardColor.Blue, Card.CardFace.Organ));
            //Players[0].Body.Items[1].NewEvolvedVirus(new Card(Card.CardColor.Blue, Card.CardFace.EvolvedVirus), this);

            Players[0].Body.SetOrgan(new Card(Card.CardColor.Green, Card.CardFace.Organ));
            Players[0].Body.SetOrgan(new Card(Card.CardColor.Red, Card.CardFace.Organ));
            //Players[0].Body.SetOrgan(new Card(Card.CardColor.Blue, Card.CardFace.Organ));
            ////Players[0].Body.SetOrgan(new Card(Card.CardColor.Green, Card.CardFace.Organ));
            //Players[0].Hand[0] = new Card(Card.CardColor.Red, Card.CardFace.EvolvedVirus);
            ////Players[0].Hand[1] = new Card(Card.CardColor.Blue, Card.CardFace.EvolvedMedicine);
            ////Players[0].Hand[1] = new Card(Card.CardColor.Purple, Card.CardFace.SecondOpinion);
            ////Players[0].Hand[2] = new Card(Card.CardColor.Purple, Card.CardFace.ProtectiveSuit);

            Players[1].Body.SetOrgan(new Card(Card.CardColor.Red, Card.CardFace.Organ));
            Players[2].Body.SetOrgan(new Card(Card.CardColor.Blue, Card.CardFace.Organ));
            //Players[1].Body.SetOrgan(new Card(Card.CardColor.Red, Card.CardFace.Organ));
            ////Players[2].Body.SetOrgan(new Card(Card.CardColor.Red, Card.CardFace.Organ));
            Players[0].Hand[0] = new Card(Card.CardColor.Purple, Card.CardFace.Transplant);
            Players[0].Hand[1] = new Card(Card.CardColor.Wildcard, Card.CardFace.Medicine);
            ////Players[1].Body.SetOrgan(new Card(Card.CardColor.Yellow, Card.CardFace.Organ));
            //Players[2].Body.SetOrgan(new Card(Card.CardColor.Yellow, Card.CardFace.Organ));


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
            }
        }


        public bool PlayUserCardByMove(Player player, Card myCard, string move, List<string> wholemoves, bool discard = true)
        {
            if (!wholemoves.Contains(move))
            {
                return false;
            }

            bool nextturn = true;

            switch (myCard.Face)
            {
                case Card.CardFace.Medicine:
                    RemoveCardFromHand(player, myCard);
                    PlayGameCardMedicine(player, myCard, move);
                    break;
                case Card.CardFace.EvolvedMedicine:
                    RemoveCardFromHand(player, myCard);
                    PlayGameCardEvolvedMedicine(player, myCard, move);
                    break;


                case Card.CardFace.Virus:
                    if (discard)
                        RemoveCardFromHand(player, myCard);
                    PlayGameCardVirus(player, myCard, move, wholemoves);
                    break;
                case Card.CardFace.EvolvedVirus:
                    RemoveCardFromHand(player, myCard);
                    PlayGameCardEvolvedVirus(player, myCard, move, wholemoves);
                    break;

                case Card.CardFace.OrganThief:
                    if (discard)
                        DiscardFromHand(player, myCard);
                    PlayOrganThief(player, myCard, move, wholemoves);
                    break;
                    
                case Card.CardFace.Quarantine:
                    DiscardFromHand(player, myCard);
                    PlayQuarantine(player, move);
                    break;

                case Card.CardFace.Transplant:
                    if (discard)
                        DiscardFromHand(player, myCard);
                    PlayGameCardTransplant(player, myCard, move, wholemoves);
                    break;

                case Card.CardFace.MedicalError:
                    if (discard)
                        DiscardFromHand(player, myCard);
                    PlayMedicalError(player, myCard, move, wholemoves);
                    break;
                case Card.CardFace.SecondOpinion:
                    if (discard)
                        DiscardFromHand(player, myCard);
                    PlaySecondOpinion(player, myCard, move, wholemoves);
                    //nextturn = false;
                    break;
                default:
                    return false;
            }

            if (nextturn)
            {
                DrawCardsToFill(player);
                Turn++;
            }
            return true;
        }



        public string GetMoveGivenSelectedCards(List<CCheckBox> selected)
        {
            if (selected == null || selected.Count == 0) {
                return null;
            }

            Card first = selected[0].Card;
            selected.Remove(selected[0]);

            switch (first.Face)
            {
                case Card.CardFace.EvolvedMedicine:
                case Card.CardFace.EvolvedVirus:
                case Card.CardFace.Medicine:
                case Card.CardFace.OrganThief:
                case Card.CardFace.Quarantine:
                case Card.CardFace.Virus:
                    CCheckBox dest = selected[0];
                    return Scheduler.GenerateMove(dest.PlayerId, dest.Index);


                // Two cards interaction
                case Card.CardFace.Transplant:
                    //if (source == null || dest == null)
                    //    //            {
                    //    //                return null;
                    //    //            }
                    //    //            return Scheduler.GetManyMoveItem(new string[]
                    //    //                                {
                    //    //                                    Scheduler.GenerateMove(source.PlayerId, source.Index),
                    //    //                                    Scheduler.GenerateMove(dest.PlayerId, dest.Index)
                    //    //                                });
                        return null;

                // N cards interaction
                case Card.CardFace.Spreading:
                    return null;
                    
                //case Card.CardFace.MedicalError:
                //case Card.CardFace.SecondOpinion:
                //case Card.CardFace.LatexGlove: break;
                //case Card.CardFace.Organ: break;
                //case Card.CardFace.Overtime: break;
                //case Card.CardFace.ProtectiveSuit: break;
            }
            
            return null;
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
                        player.Nickname, myCard.ToString(), GetMyCardAffectedFromMove(rival, myCard, move));
                    
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
