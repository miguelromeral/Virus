using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Virus.Core
{
    public class Scenario
    {
        public Game Game;
        public Player Player;
        public string Move;
        public Card Card;
        public int Step;
        //public State State;
        public EventWaitHandle eventWaitHandle;

        public Scenario(Game g, Player p, string m, Card c, int s)
        {
            Game = Game.DeepClone(g);
            Game.Logger = null;
            eventWaitHandle = new ManualResetEvent(false);
            Move = m;
            Player = p;
            Card = c;
            Step = s;
        }

        
        public static void PerformUserWorkItem(Object args)
        {
            Scenario scen = args as Scenario;
            Player p = null;
            foreach(var pl in scen.Game.Players)
            {
                if (pl.ID == scen.Player.ID)
                    p = pl;
            }
            scen.Game.PlayCardByMove(p, scen.Card, scen.Move);
            scen.eventWaitHandle.Set();
        }
        
    }
}
