using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Virus.Core
{
    /// <summary>
    /// Class that stores the game state when looking for the best possible move.
    /// </summary>
    public class Scenario
    {
        /// <summary>
        /// Original game state.
        /// </summary>
        public Scenario Previous;
        /// <summary>
        /// Game state with the move played.
        /// </summary>
        public Game Game;
        /// <summary>
        /// Player who has moved latest.
        /// </summary>
        public Player Player;
        /// <summary>
        /// Last move played in this game.
        /// </summary>
        public string Move;
        /// <summary>
        /// List of all moves with this card.
        /// </summary>
        public List<string> AllMoves;
        /// <summary>
        /// Last card used.
        /// </summary>
        public Card Card;
        /// <summary>
        /// Number of step in the search. (TBD).
        /// </summary>
        public int Step;
        /// <summary>
        /// Sempahore that allows the AI to check this game state.
        /// </summary>
        public EventWaitHandle eventWaitHandle;
        public bool AmITheWinner {
            get {
                if (Game == null)
                    return false;
                return Game.AmITheWinner(Player.ID);
            }
        }
        public int Points { get { return Player.Body.Points + (AmITheWinner ? 99999 : 0); } }

        /// <summary>
        /// Scenario's constructor.
        /// </summary>
        /// <param name="g">Original game. It'll make a copy.</param>
        /// <param name="p">Player who will play.</param>
        /// <param name="m">Move to be played.</param>
        /// <param name="c">Card to be played.</param>
        /// <param name="s">Int number of step.</param>
        /// <param name="list">List of whole moves with this card.</param>
        public Scenario(Game g, Player p, string m, Card c, int s, List<string> list, Scenario r = null)
        {
            // Creates a instance copy.
            Game = Game.DeepClone(g);
            Game.Logger = null;
            Game.IsInScenario = true;
            eventWaitHandle = new ManualResetEvent(false);
            Move = m;
            Player = p;
            Card = c;
            Step = s;
            AllMoves = list;
            Previous = r;
        }

        public void SetRootGame()
        {
            Previous = this;
        }

        /// <summary>
        /// Performs the movement in the scenario.
        /// </summary>
        /// <param name="args">The scenario initialized as object.</param>
        public static void PerformUserWorkItem(Object args)
        {
            Scenario scen = args as Scenario;
            Player p = null;
            foreach(var pl in scen.Game.Players)
            {
                if (pl.ID == scen.Player.ID)
                    p = pl;
            }
            scen.Game.PlayCardByMove(p, scen.Card, scen.Move, null);
            scen.eventWaitHandle.Set();
        }

        public Scenario GetRoot()
        {
            if(Previous == null)
            {
                return this;
            }
            else
            {
                return Previous.GetRoot();
            }
        }
        

        public static Scenario GetBetter(Scenario one, Scenario two)
        {
            if (one == null)
                return two;
            if (two == null)
                return one;

            // Compare steps. If less steps, it means less moves to become the winner.
            // Note: the steps go in descendent order, so 3 as step is better than 0.
            if (one.Step > two.Step)
                return one;
            if (one.Step < two.Step)
                return two;
            // Same number of steps


            Player po = one.Player;
            Player pt = two.Player;
            var topone = one.Game.TopPlayers();
            var toptwo = two.Game.TopPlayers();

            int posone = topone.IndexOf(one.Game.GetPlayerByID(po.ID));
            int postwo = topone.IndexOf(two.Game.GetPlayerByID(pt.ID));




            // Now, check the healthy organs

            if (po.HealthyOrgans > pt.HealthyOrgans)
                return one;
            if (po.HealthyOrgans < pt.HealthyOrgans)
                return two;
            // Also same number of healthy organs

            // Now, check the points of its body.
            Body bo = po.Body;
            Body bt = pt.Body;

            if (bo.Points > bt.Points)
                return one;
            if (bo.Points < bt.Points)
                return two;
            // Same number of points.

            // MAKE IN THE FUTURE  MORE OPTIONS
            return one;
        }

    }
}
