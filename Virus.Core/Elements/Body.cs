using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Virus.Core
{
    /// <summary>
    /// Body. It stores all body items for each organ and modifiers played by a single player.
    /// </summary>
    [Serializable]
    public class Body
    {
        #region PROPERTIES
        /// <summary>
        /// Points corresponding to the whole body (sum of any body items).
        /// </summary>
        public int Points
        {
            get {

                int p = 0;
                foreach(var item in Items)
                {
                    if(item != null)
                        p += item.Points;
                }
                return p;
            }
        }

        /// <summary>
        /// List of body items played by the user.
        /// </summary>
        public List<BodyItem> Items;
        #endregion

        #region CONSTRUCTOR
        /// <summary>
        /// Body constructor.
        /// </summary>
        public Body()
        {
            Items = new List<BodyItem>();
        }
        #endregion

        /// <summary>
        /// Gets a review of their body items.
        /// </summary>
        /// <returns>String with the body items</returns>
        public override string ToString()
        {
            //string printed = String.Empty;
            string printed = "<Total Pts: "+Points+">" + Environment.NewLine;

            int i = 1;
            foreach(BodyItem item in Items)
            {
                printed += i +".  "+ item.ToString() + Environment.NewLine;
                i++;
            }
            return printed;
        }

        /// <summary>
        /// Check if the body has a body item corresponding to this color.
        /// </summary>
        /// <param name="color">Color that want to check</param>
        /// <returns>True if the body has a body item of this color.</returns>
        public bool HaveThisOrgan(Card.CardColor color)
        {
            foreach(var item in Items)
            {
                if (item.Organ.Color.Equals(color))
                {
                    return true;
                }
            }
            return false;
        }
        
        /// <summary>
        /// Creates a new body item if the body doesn't contain a body item of the appropiate color.
        /// </summary>
        /// <param name="organ">Card of the organ.</param>
        /// <returns>String with the error message if it couldn't be added. Null in other case.</returns>
        public bool SetOrgan(Card organ)
        {
            if (HaveThisOrgan(organ.Color))
            {
                return false;
            }
            else
            {
                BodyItem item = new BodyItem(organ);
                if (organ.Color == Card.CardColor.Wildcard)
                {
                    // A Wildcard organ could substitute anyone. It's better valued.
                    item.Points += (Scheduler.POINTS_ORGAN / 5);
                }
                if (organ.Color == Card.CardColor.Bionic)
                {
                    item.Points += (Scheduler.POINTS_ORGAN / 4);
                }
                Items.Add(item);

                return true;
            }
        }

        /// <summary>
        /// Set a new virus to one of the body items.
        /// </summary>
        /// <param name="virus">Card with the virus.</param>
        /// <param name="index">Index of the body item to use this body iitem.</param>
        /// <param name="game">Game.</param>
        /// <returns>Error message if it couldn't have been played.</returns>
        public void SetVirus(Card virus, int index, Game game)
        {
            BodyItem item = Items[index];

            string message = item.NewVirus(virus, game);
            // if when we added the virus, the body item has more than one virus, we have to
            // remove them from the whole body.
            if (message != null && message.Equals(BodyItem.RULE_DELETEBODY))
            {
                Items.Remove(item);
                game.WriteToLog("The " + item + " has been removed from the body.");
            }
        }

        public void SetEvolvedVirus(Card virus, int index, Game game)
        {
            BodyItem item = Items[index];

            string message = item.NewEvolvedVirus(virus, game);
            // if when we added the virus, the body item has more than one virus, we have to
            // remove them from the whole body.
            if (message != null && message.Equals(BodyItem.RULE_DELETEBODY))
            {
                Items.Remove(item);
                game.WriteToLog("The " + item + " has been removed from the body.");
            }
        }

        public int OrgansLeftToWin(Game game)
        {
            int count = game.Settings.NumberToWin;
            foreach(var item in Items)
            {
                if (item.IsHealthy)
                {
                    count--;
                }
            }
            return count;
        }

    }
}
