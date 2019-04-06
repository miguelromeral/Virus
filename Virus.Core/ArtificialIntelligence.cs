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
            Easy,
            Medium,
            Hard
        }

        private Game Game;
        private Player Player;

        public ArtificialIntelligence(Game g, Player p)
        {
            Game = g;
            Player = p;
        }
        
        public AICategory RandomIA()
        {
            return (AICategory) new Random().Next(1, Enum.GetValues(typeof(AICategory)).Length);
        }

        public string PlayTurn()
        {
            Console.WriteLine("Player with ID " + Player.ID + " is going to play turn.");
            return null;
        }
    }
}
