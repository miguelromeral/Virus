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
        public int Index;
        public int Step;

        public Scenario(Game g, Player p, string m, Card c, int i, int s)
        {
            Game = Game.DeepClone(g);
            Game.Logger = null;
            Player = p;
            Move = m;
            Card = c;
            Index = i;
            Step = s;
        }

        public void Run()
        {
            Game.PlayCardByMove(Game.GetPlayerByID(Player.ID), Card, Move);
        }
    }
}
