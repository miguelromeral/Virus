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
            List<List<string>> movesByCard = new List<List<string>>();
            Card card;

            for(int i=0; i< Me.Hand.Count; i++)
            {
                movesByCard.Add(Game.Referee.GetListMovements(Me, Me.Hand[i]));
            }

            switch (Me.Ai)
            {
                case AICategory.Easy:
                case AICategory.Medium:
                case AICategory.Hard:

                    for(int i=0; i<Me.Hand.Count; i++)
                    {
                        if(movesByCard[i].Count > 0)
                        {
                            Game.PlayCardByMove(Me, Me.Hand[i], movesByCard[i].ElementAt(0));
                            return null;
                        }
                        else
                        {
                            // Testing Operation
                            //Game.DiscardFromHand(Me, i);
                        }
                    }
                    Game.DiscardAllHand(Me);
                    return null;
                    
            }
            
            return null;
        }
    }
}
