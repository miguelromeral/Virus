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
            : base(numPlayers, waitingtime, firstHuman, new CLogger(log))
        {
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
    }
}
