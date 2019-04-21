using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Virus.Core
{
    /// <summary>
    /// Player. A user with his corresponding body.
    /// </summary>
    [Serializable]
    public class Player
    {
        #region PROPERTIES
        /// <summary>
        /// Artificial Intelligence of the player. Hard difficulity.
        /// </summary>
        public ArtificialIntelligence.AICategory AI;

        /// <summary>
        /// Artificial Intelligence entity (or "mind") that indicates how to play.
        /// </summary>
        public ArtificialIntelligence Computer;

        /// <summary>
        /// Nickname of the player.
        /// </summary>
        public string ShortDescription
        {
            // For the moment, it's only the ID.
            get { return "Player " + ID; }
        }

        /// <summary>
        /// ID corresponding to the current player.
        /// </summary>
        public int ID;

        /// <summary>
        /// List of cards that the user can play in this turn.
        /// </summary>
        public List<Card> Hand;

        /// <summary>
        /// Body of the player.
        /// </summary>
        public Body Body;

        /// <summary>
        /// Count of how many healthy organs has this user.
        /// </summary>
        public int HealthyOrgans
        {
            get
            {
                int count = 0;
                foreach (var item in Body.Items)
                {
                    if (item.IsHealthy)
                    {
                        count++;
                    }
                }
                return count;
            }
        }
        #endregion

        #region CONSTRUCTOR
        /// <summary>
        /// Player constructor.
        /// </summary>
        /// <param name="game">Game</param>
        /// <param name="human">Indicates if the player will be a human (true) or PC (false)</param>
        public Player(Game game, bool human = false)
        {
            Console.WriteLine("Creating new player.");
            Body = new Body();
            if (human)
            {
                AI = ArtificialIntelligence.AICategory.Human;
            }
            else
            {
                Computer = new ArtificialIntelligence(game, this);
                AI = Computer.RandomIA();
                //ai = ArtificialIntelligence.AICategory.Random;
                AI = ArtificialIntelligence.AICategory.Easy;
            }
        }
        #endregion

        /// <summary>
        /// Replace the current hand with the new one.
        /// </summary>
        /// <param name="h">New hand (list of cards)</param>
        public void NewHand(List<Card> h)
        {
            Hand = h;
        }

        /// <summary>
        /// Gets a review of the player.
        /// </summary>
        /// <returns>String with the info of the player.</returns>
        public override string ToString()
        {
            string printed = String.Empty;
            
            printed += String.Format("[{0,20} | IA: {1,10}]" + Environment.NewLine, ShortDescription, AI.ToString());
            printed += Body + Environment.NewLine;
            
            return printed;
        }

        /// <summary>
        /// Print the current hand of the player.
        /// </summary>
        /// <param name="discarding">Indicates if the player is currently discarding</param>
        public void PrintMyOptions(bool discarding = false)
        {
            int i = 0;
            while (i < Hand.Count)
            {
                Console.WriteLine("{0}.- {1}", (i + 1), Hand[i]);
                i++;
            }
            if (AI == ArtificialIntelligence.AICategory.Human)
            {

                if (discarding)
                {
                    Console.WriteLine("0.- End discarding");
                }
                else
                {
                    Console.WriteLine("0.- Discard");
                }
            }

        }

        /// <summary>
        /// Returns the index of the card in the hand.
        /// </summary>
        /// <param name="card">Card to find its index.</param>
        /// <returns>Index of the card in the player hand.</returns>
        public int GetIndexOfCardInHand(Card card)
        {
            int i = 0;
            foreach(var c in Hand)
            {
                if (c.Equals(card)){
                    return i;
                }
                i++;
            }
            return -1;
        }
        
    }
}
