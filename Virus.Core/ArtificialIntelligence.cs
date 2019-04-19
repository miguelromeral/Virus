using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Virus.Core
{
    public class ArtificialIntelligence
    {
        public enum AICategory
        {
            Human,
            First,
            Random,
            Easy,
            Medium,
            Hard
        }

        private Game Game;
        private Player Me;
        private static Random random;

        private List<Card> Discardables;

        public ArtificialIntelligence(Game g, Player p)
        {
            Game = g;
            Me = p;
            Discardables = new List<Card>();
            random = new Random();
        }
        
        public AICategory RandomIA()
        {
            return (AICategory) random.Next(1, Enum.GetValues(typeof(AICategory)).Length);
        }

        public string PlayTurn()
        {
            List<List<string>> movesByCard = new List<List<string>>();

            for(int i=0; i< Me.Hand.Count; i++)
            {
                movesByCard.Add(Game.Referee.GetListMovements(Me, Me.Hand[i]));
            }

            return DoMoveByAI(movesByCard);
        }

        public string DoMoveByAI(List<List<string>> movesByCard)
        {
            switch (Me.Ai)
            {
                case AICategory.First:
                    return ChooseFirstMove(movesByCard);
                case AICategory.Random:
                    return ChooseRandom(movesByCard);
                case AICategory.Easy:
                case AICategory.Medium:
                case AICategory.Hard:
                default:
                    return ChooseFirstMove(movesByCard);
            }
        }

        public string ChooseFirstMove(List<List<string>> movesByCard)
        {
            for (int i = 0; i < Me.Hand.Count; i++)
            {
                if (movesByCard[i].Count > 0)
                {
                    string bestMove = movesByCard[i].ElementAt(0);
                    return Game.PlayCardByMove(Me, Me.Hand[i], bestMove);
                }
            }
            Game.DiscardAllHand(Me);
            return null;
            
        }

        public string ChooseRandom(List<List<string>> movesByCard)
        {
            int sel;
            if (movesByCard.Count > 0)
            {
                List<int> visited = new List<int>();
                do
                {
                    sel = random.Next(0, movesByCard.Count);
                    if (!visited.Contains(sel))
                    {
                        visited.Add(sel);
                    }
                    if(movesByCard[sel].Count > 0)
                    {
                        string move = movesByCard[sel].ElementAt(random.Next(0, movesByCard[sel].Count));
                        return Game.PlayCardByMove(Me, Me.Hand[sel], move);
                    }
                } while (visited.Count < movesByCard.Count);

                sel = random.Next(-1, movesByCard.Count);
                if (sel == -1)
                {
                    Game.DiscardAllHand(Me);
                    return null;
                }
                else
                {
                    Game.DiscardFromHand(Me, Me.Hand[sel]);
                    return null;
                }
            }
            return "END RANDOM";
        }

        public string ChooseEasy(Card myCard)
        {
            /*
            switch (myCard.Face)
            {
                case Card.CardFace.Organ:
                case Card.CardFace.Medicine:
                case Card.CardFace.Virus:
                case Card.CardFace.Transplant:
                case Card.CardFace.OrganThief:
                case Card.CardFace.LatexGlove:
                case Card.CardFace.MedicalError:
                case Card.CardFace.Spreading:
                default:
                    return moves[0];
            }
            */
            return null;
        }
    }
}
