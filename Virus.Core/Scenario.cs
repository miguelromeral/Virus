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
        
    }
}
