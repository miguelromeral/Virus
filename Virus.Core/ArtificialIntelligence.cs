using System;
using System.Collections.Generic;
using System.Diagnostics;
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
            List<List<string>> movesByCard = Game.GetListOfMovesWholeHand(Me);

            
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
                    PlayTurnAIMedium(movesByCard);
                    break;
                case AICategory.Hard:
                    PlayTurnAIHard(movesByCard);
                    //PlayTurnAIMedium(movesByCard);
                    break;
                default:
                    PlayTurnAIRandom(movesByCard);
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
                scen.eventWaitHandle.WaitOne();
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



        public void PlayTurnAIHard(List<List<string>> movesbycard)
        {
            Stopwatch timer = new Stopwatch();
            timer.Start();

            // List of scenarios with every card played.
            Scenario thebest = getTheBestPossibleScenario(Game, movesbycard, 2, Me.Hand.Count, null);

            if(thebest == null)
            {
                Game.DiscardAllHand(Me);
                return;
            }
            Scenario root = thebest.GetRoot();

            timer.Stop();
            Console.WriteLine("Time elapsed in choice: {0} ms", timer.ElapsedMilliseconds);

            Game.PlayCardByMove(Me, root.Card, root.Move, root.AllMoves);
        }

        

        public Scenario getTheBestPossibleScenario(Game original, List<List<string>> movesbycard, int steps, int cardsinhand, Scenario previous = null)
        {
            //Console.WriteLine("Entrando en Paso {0}", steps);
            
            Scenario[] bestByCard = new Scenario[cardsinhand];

            for (int i = 0; i < cardsinhand; i++)
            {
                Player otherme = original.GetPlayerByID(Me.ID);
                Card c = otherme.Hand[i];

                //Console.WriteLine("Paso {0}, Carta #{1} ({2})", steps, i, otherme.Hand[i]);
                
                bestByCard[i] = getTheBestPossibleScenarioByCard(original, movesbycard[i], c, steps, cardsinhand, previous);
            }

            Scenario best = bestByCard[0];

            for (int i = 1; i < cardsinhand; i++)
            {
                Scenario.GetBetter(best, bestByCard[i]);
            }

            if (best == null)
                return null;
            
            return best;
        }


        public Scenario getTheBestPossibleScenarioByCard(Game original, List<string> moves, Card c, int steps, int cardsinhand, Scenario first = null)
        {
            List<Scenario> bestInHand = new List<Scenario>();

            for (int i = 0; i < moves.Count; i++)
            {
                //Console.WriteLine("S:{0}, C:{1}, M:{2}", steps, c, moves[i]);

                Scenario newone = new Scenario(original, Me, moves[i], c, steps, moves, first);
                
                
                Player otherme = newone.Game.GetPlayerByID(Me.ID);
                c = otherme.Hand[otherme.GetIndexOfCardInHand(c)];
                // Substitute this with the async method:
                newone.Game.PlayCardByMove(newone.Game.GetPlayerByID(Me.ID), c, newone.Move, null);
                //---------------------------


                if (newone.AmITheWinner)
                    return newone;

                newone.Game.DrawCardsToFill(newone.Game.GetPlayerByID(Me.ID), cardsinhand, true);

                if (steps == 0)
                {
                    bestInHand.Add(newone);
                }
                else
                {
                    bestInHand.Add(getTheBestPossibleScenario(newone.Game, newone.Game.GetListOfMovesWholeHand(otherme), steps - 1, otherme.Hand.Count, newone));
                }

            }

            if (bestInHand.Count == 0)
                return first;
                //return null;

            Scenario best = bestInHand[0];

            for (int i = 1; i < bestInHand.Count; i++)
            {
                Scenario.GetBetter(best, bestInHand[i]);
            }

            //Console.WriteLine("Selected - S:{0}, C:{1} --> M:{2}", steps, c, best.Move);


            return best;

        }
        


        /// <summary>
        /// Gets a list of every Game state with the moves passed.
        /// </summary>
        /// <param name="movesbycard">List of lists of moves.</param>
        /// <returns>List of Scenarios</returns>
        public List<Scenario> AllScenariosByLists(List<List<string>> movesbycard)
        {
            List<Scenario> scenarios = new List<Scenario>();
            // For every card in hand:
            for (int i = 0; i < Me.Hand.Count; i++)
            {
                Card card = Me.Hand[i];
                var list = movesbycard[i];
                // For every move in the list for this card.
                for (int j = 0; j < movesbycard[i].Count; j++)
                {
                    // Create and plays the card asynchronously
                    Scenario scen = new Scenario(Game, Me, list[j], card, 0, list);
                    ThreadPool.QueueUserWorkItem(Scenario.PerformUserWorkItem, scen);
                    scenarios.Add(scen);
                }
            }
            return scenarios;
        }
    }
}
