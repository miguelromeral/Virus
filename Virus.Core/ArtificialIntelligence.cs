using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Virus.Core
{
    [Serializable]
    public class ArtificialIntelligence
    {
        public enum AICategory
        {
            /// <summary>
            /// Human interaction
            /// </summary>
            Human,
            /// <summary>
            /// Choose every time the first card in his hand and the first available move (the Player 0 will suffer almost every time!)
            /// </summary>
            First,
            /// <summary>
            /// Choose randomly one card to play and one random move to this card.
            /// </summary>
            Random,
            /// <summary>
            /// Looks for the move with more points to him.
            /// </summary>
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
            switch (Me.AI)
            {
                case AICategory.First:
                    return ChooseFirstMove(movesByCard);
                case AICategory.Random:
                    return ChooseRandom(movesByCard);
                case AICategory.Easy:
                    return ChooseEasy(movesByCard);
                case AICategory.Medium:
                case AICategory.Hard:
                default:
                    //return ChooseEasy(movesByCard);
                    return ChooseMedium(movesByCard);
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

        public string ChooseEasy(List<List<string>> movesbyCard)
        {
            // Hand -> Game (by move)
            List<List<Game>> scenarios = AllScenariosByLists(movesbyCard);
            Dictionary<string, Card> best = GetMoveMorePoints(scenarios, movesbyCard);
            if (best == null)
            {
                Game.DiscardAllHand(Me);
                return null;
            }
            else
            {
                foreach (var i in best)
                {
                    return Game.PlayCardByMove(Me, i.Value, i.Key);
                }
            }

            return null;
        }


        public string ChooseMedium(List<List<string>> movesbycard)
        {
            // Hand -> Game (by move)
            List<List<Game>> scenarios = AllScenariosByLists(movesbycard);
            int maxPoints = 99999, current;
            Dictionary<string, Card> best;
            string bestmove = null;
            Card card = null;
            bool amiwinning = false;
            Game aux;
            int pts1, pts2, diff;

            for (int i = 0; i < movesbycard.Count; i++)
            {
                var games = scenarios[i];
                var moves = movesbycard[i];
                for (int j = 0; j < movesbycard[i].Count; j++)
                {
                    aux = games[j];
                    var qualy = aux.TopPlayers();
                    Player top = qualy[0];
                    Player follower = qualy[1];

                    if(top.ID == Me.ID)
                    {
                        if (!amiwinning)
                        {
                            amiwinning = true;
                            maxPoints = 0;
                        }
                        current = top.Body.Points - follower.Body.Points;
                        if(current > maxPoints)
                        {
                            bestmove = moves[j];
                            maxPoints = current;
                            card = Me.Hand[i];
                        }
                    }
                    else
                    {
                        if (!amiwinning) {
                            foreach (var p in qualy)
                            {
                                if (p.ID == Me.ID)
                                {
                                    follower = p;
                                }
                            }
                            current = top.Body.Points - follower.Body.Points;
                            if (current < maxPoints)
                            {
                                bestmove = moves[j];
                                maxPoints = current;
                                card = Me.Hand[i];
                            }
                        }
                    }
                }
            }

            // CHECK IF NULL!
            if (bestmove == null)
            {
                Game.DiscardAllHand(Me);
                return null;
            }

            best = new Dictionary<string, Card>
            {
                { bestmove, card }
            };
            
            foreach (var i in best)
            {
                return Game.PlayCardByMove(Me, i.Value, i.Key);
            }
            

            return null;
        }

        public List<List<Game>> AllScenariosByLists(List<List<string>> movesbycard)
        {
            // IMPLEMENT HERE WITH THREADS!

            List<List<Game>> scenarios = new List<List<Game>>();
            for (int i = 0; i < Me.Hand.Count; i++)
            {
                Card card = Me.Hand[i];
                List<Game> scenByCard = new List<Game>();
                for (int j = 0; j < movesbycard[i].Count; j++)
                {

                    // La carta que se juega se elimina del original.

                    Game aux = Game.DeepClone<Game>(Game);
                    aux.Logger = null;

                    var list = movesbycard[i];
                    aux.PlayCardByMove(aux.GetPlayerByID(Me.ID), card, list[j]);
                    scenByCard.Add(aux);
                }
                scenarios.Add(scenByCard);
            }
            return scenarios;
        }
        
        public Dictionary<string, Card> GetMoveMorePoints(List<List<Game>> scenarios, List<List<string>> movesbycard)
        {
            int maxPoints = 0, current;
            string best = null;
            Card card = null;
            Game aux;

            for (int i = 0; i < movesbycard.Count; i++)
            {
                for (int j = 0; j < movesbycard[i].Count; j++)
                {
                    var games = scenarios[i];
                    var moves = movesbycard[i];

                    aux = games[j];
                    current = aux.GetPlayerByID(Me.ID).Body.Points;
                    if (current > maxPoints)
                    {
                        maxPoints = current;
                        best = moves[j];
                        card = Me.Hand[i];
                    }
                }
            }

            // CHECK IF NULL!
            if (best == null)
                return null;

            return new Dictionary<string, Card>
            {
                { best, card }
            };
        }

        public bool CanPlayOrganFromHand(List<List<string>> wholeMoves)
        {
            for(int i=0; i<Me.Hand.Count; i++)
            {
                Card c = Me.Hand[i];
                if(c.Face == Card.CardFace.Organ)
                {
                    if(wholeMoves[i].Count > 0)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
