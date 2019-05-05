using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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

        public void PlayTurn()
        {
            List<List<string>> movesByCard = new List<List<string>>();

            for(int i=0; i< Me.Hand.Count; i++)
            {

                movesByCard.Add(Game.Referee.GetListMovements(Me, Me.Hand[i]));
            }

            switch (Me.AI)
            {
                case AICategory.First:
                    ChooseFirstMove(movesByCard);
                    break;
                case AICategory.Random:
                    ChooseRandom(movesByCard);
                    break;
                case AICategory.Easy:
                    ChooseEasy(movesByCard);
                    break;
                case AICategory.Medium:
                case AICategory.Hard:
                default:
                    ChooseMedium(movesByCard);
                    break;
            }
        }

        public bool DefendFromCard(Player rival, Card c, string move)
        {
            if(rival.ID == Me.ID)
            {
                return false;
            }
            
            //bool shouldi = ShouldIDefend(rival, c, move);
            bool shouldi = true;

            if (shouldi)
            {
                int index = -1;
                for(int i=0; i<Me.Hand.Count; i++)
                {
                    if(Me.Hand[i].Face == Card.CardFace.ProtectiveSuit)
                    {
                        index = i;
                    }
                }
                Game.DiscardFromHand(Me, index);
                Me.PlayedProtectiveSuit = true;
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool ShouldIDefend(Player rival, Card card, string move)
        {
            Scenario scen = new Scenario(Game, rival, move, card, 1, null);

            // Develop it in the future with more enthusiasm.

            if(scen.Game.TopPlayers()[0].ID != Me.ID)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public string ChooseBestOptionProtectiveSuit(List<string> moves)
        {
            List<List<string>> aux = new List<List<string>>();

            if (moves.Count > 0)
            {
                switch (Me.AI)
                {
                    case AICategory.First:
                        return moves[0];
                        
                    case AICategory.Random:
                    default:
                        return moves[random.Next(0, moves.Count)];
                        
                    //case AICategory.Easy:
                    //    aux.Add(moves);
                    //    ChooseEasy(aux);
                    //    break;
                    //case AICategory.Medium:
                    //case AICategory.Hard:
                    //default:
                    //    aux.Add(moves);
                    //    ChooseMedium(aux);
                    //    break;
                }
                return null;
            }
            else
            {
                return null;
            }
        }
        

        public void ChooseFirstMove(List<List<string>> movesByCard)
        {
            for (int i = 0; i < Me.Hand.Count; i++)
            {
                if (movesByCard[i].Count > 0)
                {
                    string bestMove = movesByCard[i].ElementAt(0);
                    Game.PlayCardByMove(Me, Me.Hand[i], bestMove, movesByCard[i]);
                    return;
                }
            }
            Game.DiscardAllHand(Me);
            return;
            
        }
        
        public void ChooseRandom(List<List<string>> movesByCard)
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
                        Game.PlayCardByMove(Me, Me.Hand[sel], move, movesByCard[sel]);
                        return;
                    }
                } while (visited.Count < movesByCard.Count);

                sel = random.Next(-1, movesByCard.Count);
                if (sel == -1)
                {
                    Game.DiscardAllHand(Me);
                }
                else
                {
                    Game.DiscardFromHand(Me, Me.Hand[sel]);
                }
            }
        }

        public void ChooseEasy(List<List<string>> movesbyCard)
        {
            List<Scenario> scenarios = AllScenariosByLists(movesbyCard);
            List<string> listmoves = null;
            int maxPoints = 0, current;
            string best = null;
            Card card = null;

            foreach (var scen in scenarios)
            {
                scen.eventWaitHandle.WaitOne();
                current = scen.Game.GetPlayerByID(Me.ID).Body.Points;
                if (current > maxPoints)
                {
                    maxPoints = current;
                    best = scen.Move;
                    card = scen.Card;
                    listmoves = scen.AllMoves;
                }
            }

            if (best == null)
            {
                Game.DiscardAllHand(Me);
                return;
            }
            Game.PlayCardByMove(Me, card, best, listmoves);
        }
        
        public void ChooseMedium(List<List<string>> movesbycard)
        {
            List<Scenario> scenarios = AllScenariosByLists(movesbycard);
            int maxPoints = 99999, current;
            List<string> list = null;
            string bestmove = null;
            Card card = null;
            bool amiwinning = false;
            Game aux;

            foreach (Scenario scen in scenarios)
            {
                aux = scen.Game;
                var qualy = aux.TopPlayers();
                Player top = qualy[0];
                Player follower = qualy[1];

                if (top.ID == Me.ID)
                {
                    if (!amiwinning)
                    {
                        amiwinning = true;
                        maxPoints = 0;
                    }
                    current = top.Body.Points - follower.Body.Points;
                    if (current > maxPoints)
                    {
                        bestmove = scen.Move;
                        maxPoints = current;
                        card = scen.Card;
                        list = scen.AllMoves;
                    }
                }
                else
                {
                    if (!amiwinning)
                    {
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
                            bestmove = scen.Move;
                            maxPoints = current;
                            card = scen.Card;
                            list = scen.AllMoves;
                        }
                    }
                }
            }

            // CHECK IF NULL!
            if (bestmove == null)
            {
                Game.DiscardAllHand(Me);
                return;
            }
            Game.PlayCardByMove(Me, card, bestmove, list);
        }
        

        public List<Scenario> AllScenariosByLists(List<List<string>> movesbycard)
        {
            List<Scenario> scenarios = new List<Scenario>();
            for (int i = 0; i < Me.Hand.Count; i++)
            {
                Card card = Me.Hand[i];
                List<Game> scenByCard = new List<Game>();
                var list = movesbycard[i];
                for (int j = 0; j < movesbycard[i].Count; j++)
                {
                    Scenario scen = new Scenario(Game, Me, list[j], card, 1, list);
                    ThreadPool.QueueUserWorkItem(Scenario.PerformUserWorkItem, scen);
                    scenarios.Add(scen);
                }
            }
            return scenarios;
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
