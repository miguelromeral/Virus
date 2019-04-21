using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Virus.Core
{
    /// <summary>
    /// A body item, composed by, at least, on Organ and can store the medicines and virus played in that Organ.
    /// </summary>
    [Serializable]
    public class BodyItem
    {
        #region ENUMS
        /// <summary>
        /// Possibles states for the body item based on the cards played.
        /// </summary>
        public enum State
        {
            /// <summary>
            /// Free. Any card played on that organ.
            /// </summary>
            Free,
            /// <summary>
            /// Infected. More virus played than medicines.
            /// </summary>
            Infected,
            /// <summary>
            /// Vaccinated. More medicines played that viruses.
            /// </summary>
            Vaccinated,
            /// <summary>
            /// Immunized. Two medicines played on the organ.
            /// </summary>
            Immunized,
            /// <summary>
            /// Not in game state.
            /// </summary>
            NOTINGAME
        }
        #endregion

        #region PROPERTIES
        /// <summary>
        /// Main card of the item: the Organ.
        /// </summary>
        public Card Organ;
        /// <summary>
        /// List of cards that have been played to its Organ.
        /// </summary>
        public List<Card> Modifiers;

        /// <summary>
        /// Valuable criteria to clasify this body item. Usefull for AI.
        /// </summary>
        public int Points = Scheduler.POINTS_ORGAN;
        
        /// <summary>
        /// Current status of the body item in function of the cards played on its Organ.
        /// </summary>
        public State Status
        {
            get
            {
                switch (Modifiers.Count)
                {
                    case 0:
                        // Free Organ
                        // No medicines or virus played.
                        return State.Free;
                    case 1:
                        switch (Modifiers[0].Face)
                        {
                            // Vaccinated Organ
                            case Card.CardFace.Medicine:
                                return State.Vaccinated;
                            // Infected Organ
                            case Card.CardFace.Virus:
                                return State.Infected;
                            default:
                                return State.NOTINGAME;
                        }
                    case 2:
                        if (Modifiers[0].Face == Card.CardFace.Medicine)
                        {
                            return State.Immunized;
                        }
                        else
                        {
                            return State.NOTINGAME;
                        }
                    default:
                        return State.NOTINGAME;
                }
            }
        }

        public bool IsHealthy
        {
            get
            {
                switch (Status)
                {
                    // Only free, vaccinated and immunized are healthy organs (no virus affected).
                    case BodyItem.State.Free:
                    case BodyItem.State.Vaccinated:
                    case BodyItem.State.Immunized:
                        return true;
                    default:
                        return false;
                }
            }
        }
        #endregion

        #region CONSTRUCTOR
        /// <summary>
        /// Constructor of the body item.
        /// </summary>
        /// <param name="o">Card corresponding to the Organn.</param>
        public BodyItem(Card o)
        {
            Organ = o;
            Modifiers = new List<Card>();
        }
        #endregion

        /// <summary>
        /// Set a new medicine to the organ.
        /// </summary>
        /// <param name="game">Whole game. It's needed to remove cards when virus are played before.</param>
        /// <param name="medicine">Medicine card.</param>
        /// <returns>String with a error message if it cannot be added. Null if all right.</returns>
        public string NewMedicine(Game game, Card medicine)
        {
            if (medicine.Color == Card.CardColor.Wildcard)
            {
                Points += (Scheduler.POINTS_MEDICINE / 2);
            }
            else
            {
                Points += Scheduler.POINTS_MEDICINE;
            }

            switch (Status)
            {
                case State.Free:
                case State.Vaccinated:
                    Modifiers.Add(medicine);
                    return null;
                case State.Infected:
                    // If the item has a virus, one medicine avoids its effect, and both
                    // cards are moved to discards stack.
                    game.MoveToDiscards(Modifiers.ElementAt(0));
                    Modifiers.RemoveAt(0);
                    game.MoveToDiscards(medicine);
                    return null;
                case State.Immunized:
                    // Player can't play more medicines into a immunized organ.
                    return String.Format("Your {0} is already immunized.", Organ);
                default:
                    return "UNKNOWN STATE PUTTING THE MEDICINE.";
            }
            
        }

        /// <summary>
        /// Returns the last modifier card played on this item.
        /// </summary>
        /// <returns>Last card played on item. Null if anyone.</returns>
        public Card GetLastModifier()
        {
            if(Modifiers.Count > 0)
            {
                return Modifiers.ElementAt(Modifiers.Count - 1);
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Return all cards that makes this body item.
        /// </summary>
        /// <returns>List of cards with every card on this item.</returns>
        public List<Card> GetAllCardsInBody()
        {
            List<Card> list = new List<Card>();

            if (Organ != null)
                list.Add(Organ);

            foreach(var c in Modifiers)
            {
                list.Add(c);
            }

            return list;
        }

        /// <summary>
        /// Static string that indicates computer to remove all the body item.
        /// </summary>
        public const string RULE_DELETEBODY = "--DELETE BODY ITEM--";

        /// <summary>
        /// Adds a new virus into this body item.
        /// </summary>
        /// <param name="virus">Virus card.</param>
        /// <param name="game">Full game.</param>
        /// <returns>Error message if it can't be added. Null in other case.</returns>
        public string NewVirus(Card virus, Game game)
        {
            if (Status != State.Immunized)
            {
                // It's easier to remove a wildcard virus!
                if (virus.Color == Card.CardColor.Wildcard)
                {
                    Points -= (Scheduler.POINTS_VIRUS / 2);
                }
                else
                {
                    Points -= Scheduler.POINTS_VIRUS;
                }
            }

            switch (Status)
            {
                case State.Free:
                    Modifiers.Add(virus);
                    return null;
                case State.Vaccinated:
                    game.MoveToDiscards(virus);
                    Card medicine = Modifiers.ElementAt(0);
                    game.MoveToDiscards(medicine);
                    Modifiers.Remove(medicine);
                    return null;
                case State.Infected:
                    // If the item is already infected, another virus will make
                    // needed to discard every card on this body item.
                    game.MoveToDiscards(virus);
                    foreach (var c in GetAllCardsInBody())
                    {
                        game.MoveToDiscards(c);
                    }
                    // Return to the game that its necessary to remove the whole body item (now it hasn't any cards)
                    return RULE_DELETEBODY;
                case State.Immunized:
                    return String.Format("The {0} is immunized. You cannot put the virus.", Organ);
                default:
                    return "UNKNOWN STATE PUTTING THE VIRUS.";
            }
        }

        /// <summary>
        /// Returns a body item description.
        /// </summary>
        /// <returns>String with the body item info.</returns>
        public override string ToString()
        {
            string printed = String.Empty;

            foreach(var mod in Modifiers)
            {
                printed += mod.ToStringShort();
            }

            return String.Format("({0,14}:{1,9}) [Pts: {2,4}]", Organ.ToString(), printed, Points);
        }

    }
}
