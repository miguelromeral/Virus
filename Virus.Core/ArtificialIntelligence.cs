﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Virus.Core
{
    /// <summary>
    /// Class that has the function of take the best choice in order to win the game.
    /// </summary>
    [Serializable]
    public class ArtificialIntelligence
    {
        /// <summary>
        /// Different AI levels of difficulty.
        /// </summary>
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
            /// <summary>
            /// Looks for being the leader. If its the leader, it'll choose the best move to become more leader if its possible.
            /// If its not the leader, a follower, it'll try to be as close as it can.
            /// </summary>
            Medium,
            /// <summary>
            /// To Be Developed.
            /// </summary>
            Hard
        }

        /// <summary>
        /// Current game state.
        /// </summary>
        private Game Game;
        /// <summary>
        /// Player that this AI belongs to.
        /// </summary>
        private Player Me;
        /// <summary>
        /// Random seed to get random results.
        /// </summary>
        private static Random random;
        
        /// <summary>
        /// Constructor of AI.
        /// </summary>
        /// <param name="g">Current game.</param>
        /// <param name="p">Player that has this AI.</param>
        public ArtificialIntelligence(Game g, Player p)
        {
            Game = g;
            Me = p;
            random = new Random();
        }
        
        /// <summary>
        /// Gets a random AI level except Human.
        /// </summary>
        /// <returns>Random AI level except Human.</returns>
        public AICategory RandomIA()
        {
            return (AICategory) random.Next(1, Enum.GetValues(typeof(AICategory)).Length);
        }

        /// <summary>
        /// Do the move of this player.
        /// </summary>
        public void PlayTurn()
        {
            // List of every possible move with each card.
            // Each list (of lists) contains the list of possible moves for the Card with index X.
            List<List<string>> movesByCard = new List<List<string>>();

            // Get all availables moves by card.
            for(int i=0; i< Me.Hand.Count; i++)
            {
                movesByCard.Add(Game.Referee.GetListMovements(Me, Me.Hand[i]));
            }

            // It'll do the appropiate move in function its AI level.
            switch (Me.AI)
            {
                case AICategory.First:
                    PlayTurnAIFirst(movesByCard);
                    break;
                case AICategory.Random:
                    PlayTurnAIRandom(movesByCard);
                    break;
                case AICategory.Easy:
                    PlayTurnAIEasy(movesByCard);
                    break;
                case AICategory.Medium:
                case AICategory.Hard:
                default:
                    PlayTurnAIMedium(movesByCard);
                    break;
            }
        }

        /// <summary>
        /// Defend itself (or not) from a rival Card payed (Protective Suit functionality).
        /// </summary>
        /// <param name="rival">Player who has used this card against this player.</param>
        /// <param name="c">Card which has been used in this move.</param>
        /// <param name="move">Current move code.</param>
        /// <returns>True if the player has used his Protective Suit to defend. False if not.</returns>
        public bool DefendFromCard(Player rival, Card c, string move)
        {
            // The player who has used this card cannot defend itself.
            if(rival.ID == Me.ID)
            {
                return false;
            }
            
            // Makes the choice of defend or not.
            bool shouldi = ShouldIDefend(rival, c, move);
            
            if (shouldi)
            {
                // Discards its protective suit card now it has used.
                int index = -1;
                for(int i=0; i<Me.Hand.Count; i++)
                {
                    if(Me.Hand[i].Face == Card.CardFace.ProtectiveSuit)
                    {
                        index = i;
                    }
                }
                Game.DiscardFromHand(Me, index);
                // Change the flag of protective used. (It tells the Referee to not include him getting all possible moves).
                // The game will clear this flag when the turn finishes.
                Me.PlayedProtectiveSuit = true;
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Check if it would be good to protect or not, looking at the possible scenario if not.
        /// </summary>
        /// <param name="rival">Player who has used this card.</param>
        /// <param name="card">Card used.</param>
        /// <param name="move">Move played.</param>
        /// <returns></returns>
        public bool ShouldIDefend(Player rival, Card card, string move)
        {
            Scenario scen = new Scenario(Game, rival, move, card, 1, null);
            
            /*
             * 
             * 
            // Develop it in the future with more enthusiasm.
            *
            * 
            */

            // For the moment, only check if the leader is not him.
            if(scen.Game.TopPlayers()[0].ID != Me.ID)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Choose the best possible move when a Protective Suit has been played.
        /// </summary>
        /// <param name="moves">All possible moves</param>
        /// <returns>New move played.</returns>
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
        
        /// <summary>
        /// Plays  the first of all possible moves.
        /// </summary>
        /// <param name="movesByCard">List of lists of moves.</param>
        public void PlayTurnAIFirst(List<List<string>> movesByCard)
        {
            for (int i = 0; i < Me.Hand.Count; i++)
            {
                if (movesByCard[i].Count > 0)
                {
                    // There's a move then play the first one.
                    string bestMove = movesByCard[i].ElementAt(0);
                    Game.PlayCardByMove(Me, Me.Hand[i], bestMove, movesByCard[i]);
                    return;
                }
            }
            // If there is any move in the list, just discard all the hand.
            Game.DiscardAllHand(Me);
            return;
        }
        
        /// <summary>
        /// Play the turn with Random AI.
        /// </summary>
        /// <param name="movesByCard">List of lists of moves.</param>
        public void PlayTurnAIRandom(List<List<string>> movesByCard)
        {
            int sel;
            if (movesByCard.Count > 0)
            {
                // List of number of lists that we've visited so far.
                List<int> visited = new List<int>();
                do
                {
                    // Gets a new random int and add it to the visited list.
                    sel = random.Next(0, movesByCard.Count);
                    if (!visited.Contains(sel))
                    {
                        visited.Add(sel);
                    }
                    // If there is at least one move, lets play with that.
                    if(movesByCard[sel].Count > 0)
                    {
                        string move = movesByCard[sel].ElementAt(random.Next(0, movesByCard[sel].Count));
                        Game.PlayCardByMove(Me, Me.Hand[sel], move, movesByCard[sel]);
                        return;
                    }
                    //... if not, it will get a new random element if there are list to be visited yet.
                    // We'll be visiting lists while there would be lists to be visited.
                } while (visited.Count < movesByCard.Count);

                // If there is no moves at all, discard one (or all cards) by random.
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

        /// <summary>
        /// Play the turn in Easy AI level.
        /// </summary>
        /// <param name="movesbyCard">List of lists of moves</param>
        public void PlayTurnAIEasy(List<List<string>> movesbyCard)
        {
            // Get every game state with all of this moves.
            List<Scenario> scenarios = AllScenariosByLists(movesbyCard);
            List<string> listmoves = null;  // All list of moves with the best option.
            int maxPoints = 0, current;     // The indicatos of maximum points in all of that scenarios and the aux current value
            string best = null;             // The best move of them all.
            Card card = null;               // The best card to play.

            // For every scenario, check for what of them makes our body with more points.
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

            // If there is no best option, just discard every card.
            if (best == null)
            {
                Game.DiscardAllHand(Me);
                return;
            }

            // If there is one best move, play it.
            Game.PlayCardByMove(Me, card, best, listmoves);
        }
        
        /// <summary>
        /// Play the turn in Medium AI level.
        /// </summary>
        /// <param name="movesbycard">List of lists of moves</param>
        public void PlayTurnAIMedium(List<List<string>> movesbycard)
        {
            // List of scenarios with every card played.
            List<Scenario> scenarios = AllScenariosByLists(movesbycard);
            int maxPoints = 99999, current;     // Flags to check scenarios.
            List<string> list = null;           // List of moves with the best card possible
            string bestmove = null;             // Best move to play.
            Card card = null;                   // Best card.
            bool amiwinning = false;            // Indicates that we are leaders of the game.
            Game aux;

            // For each scenario, check if we are leader of one of them. If its of this way, finds the
            // one which makes us more leader than possible. If we are not the laeaders, find for the one who
            // makes us more closer than possible.
            foreach (Scenario scen in scenarios)
            {
                aux = scen.Game;
                var qualy = aux.TopPlayers();
                Player top = qualy[0];
                Player follower = qualy[1];

                // If we are the leader.
                if (top.ID == Me.ID)
                {
                    // If its the first time we are the leader, avoid check the scenarios that we aren't.
                    if (!amiwinning)
                    {
                        amiwinning = true;
                        // The flag changes. Now we check the maximum of points of difference, not the minimum.
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
                    // If we are not the leader, a follower.
                    // Only check if we are not the leader in any moment.
                    if (!amiwinning)
                    {
                        // Retrieves our position in scenario qualy.
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

            // If there is no move available, discard all hand.
            if (bestmove == null)
            {
                Game.DiscardAllHand(Me);
                return;
            }
            // if there is a best move, play it.
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
