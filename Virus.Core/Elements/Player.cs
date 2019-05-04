using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
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

        public bool PlayedProtectiveSuit = false;


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
                    if (item != null && item.IsHealthy)
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
                //AI = ArtificialIntelligence.AICategory.Medium;
                AI = ArtificialIntelligence.AICategory.First;
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
                Console.Write("{0}.- ", (i + 1));
                Scheduler.ChangeConsoleOutput(Hand[i].Color);
                Hand[i].PrintCard();
                Console.WriteLine("");
                i++;
                Scheduler.ChangeConsoleOutput();
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


        public bool DoIHaveProtectiveSuit()
        {
            foreach(Card c in Hand)
            {
                if (c.Face == Card.CardFace.ProtectiveSuit)
                {
                    return true;
                }
            }
            return false;
        }


        public static Player DeepClone<Player>(Player obj)
        {
            using (var ms = new MemoryStream())
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(ms, obj);
                ms.Position = 0;
                Player p = (Player)formatter.Deserialize(ms);
                return p;
            }
        }
        
    }
}
