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
        private Player Me;

        private List<Card> Discardables;

        public ArtificialIntelligence(Game g, Player p)
        {
            Game = g;
            Me = p;
            Discardables = new List<Card>();
        }
        
        public AICategory RandomIA()
        {
            return (AICategory) new Random().Next(1, Enum.GetValues(typeof(AICategory)).Length);
        }

        public string PlayTurn()
        {
            List<List<string>> messages = new List<List<string>>();
            List<string> currentMessage;
            Discardables.Clear();
            Game aux;

            foreach(var card in Me.Hand)
            {
                aux = Game;
                currentMessage = new List<string>();
                currentMessage = aux.GetListMovements(Me, card);
                if(currentMessage.Count != 0)
                {
                    messages.Add(currentMessage);
                }
                else
                {
                    Discardables.Add(card);
                }
            }

            switch (Me.Ai)
            {
                case AICategory.Easy:
                case AICategory.Medium:
                case AICategory.Hard:


                    if(Discardables.Count > (Game.Settings.NumberCardInHand / 2))
                    {
                        Game.DiscardAllHand(Me);
                    }
                    else{

                    }

                    break;
            }
            
            return null;
        }
    }
}
