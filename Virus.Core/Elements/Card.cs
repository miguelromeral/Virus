using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Virus.Core
{
    /// <summary>
    /// Card of the game.
    /// </summary>
    [Serializable]
    public class Card
    {
        #region ENUMARATIONS
        /// <summary>
        /// All color that a card can have.
        /// </summary>
        public enum CardColor
        {
            /// <summary>
            /// Red
            /// </summary>
            Red,
            /// <summary>
            /// Yellow
            /// </summary>
            Yellow,
            /// <summary>
            /// Green
            /// </summary>
            Green,
            /// <summary>
            /// Blue
            /// </summary>
            Blue,
            /// <summary>
            /// Purple. All threatments have a purple color.
            /// </summary>
            Purple,
            /// <summary>
            /// Wildcard color. It can be used like a red, yellow, green or blue.
            /// </summary>
            Wildcard,
            Bionic
        }

        /// <summary>
        /// Face of the card. In function of the face, the card could be used of different ways.
        /// </summary>
        public enum CardFace
        {
            /// <summary>
            /// Organ. It makes a body item.
            /// </summary>
            /// <see cref="Virus.Core.BodyItem"/>
            Organ,
            /// <summary>
            /// Virus. It can infect free organs, infected or vaccunated.
            /// </summary>
            Virus,
            /// <summary>
            /// Medicine. It can remove a virus from an organ or can inmunize a body item.
            /// </summary>
            Medicine,
            /// <summary>
            /// Transplant is a threatment that is used to switch between two body items of the game (no matter if any of they are yours).
            /// </summary>
            Transplant,
            /// <summary>
            /// Organ thief allows a player to steal a rival organ and put it on their own body.
            /// </summary>
            OrganThief,
            /// <summary>
            /// Spreading is used to move from our own virus' organ to some other rival free organ.
            /// </summary>
            Spreading,
            /// <summary>
            /// Latex glove force rivals to discard all their hand.
            /// </summary>
            LatexGlove,
            /// <summary>
            /// Medical error is used to switch your whole body to ahy other of your rivals.
            /// </summary>
            MedicalError
        }
        #endregion

        #region PROPIERTIES
        /// <summary>
        /// Color of the card.
        /// </summary>
        public CardColor Color;
        /// <summary>
        /// Face of the card.
        /// </summary>
        public CardFace Face;

        /// <summary>
        /// Value. A qualified value to allow AI choose between the best move possible.
        /// </summary>
        public int Value
        {
            get {
                // TODO
                return 0;
            }
        }
        #endregion

        #region CONSTRUCTOR
        /// <summary>
        /// Card constructor.
        /// </summary>
        /// <param name="c">Color of the card.</param>
        /// <param name="f">Face of the card.</param>
        public Card(CardColor c, CardFace f)
        {
            Color = c;
            Face = f;
        }
        #endregion

        #region PRINTABLE METHODS
        /// <summary>
        /// Description of the card.
        /// </summary>
        /// <returns>String with the card inf</returns>
        public override string ToString()
        {
            string value = "{0} {1}";

            switch (Color)
            {
                case CardColor.Purple:
                    value = String.Format(value, ToStringShort(), Face);
                    break;
                case CardColor.Wildcard:
                    value = String.Format(value, ToStringShortColor(), Face.ToString());
                    break;
                case CardColor.Red:
                case CardColor.Yellow:
                case CardColor.Blue:
                case CardColor.Green:
                case CardColor.Bionic:
                    // Normal format.
                    value = String.Format(value,
                        Color.ToString(),
                        Face.ToString());
                    break;
            }

            return value;
        }

        public void PrintCard()
        {
            string value = "{0,8} {1,8}";

            switch (Color)
            {
                case CardColor.Purple:
                    value = String.Format(value, ToStringShort(), Face);
                    break;
                case CardColor.Wildcard:
                    value = String.Format(value, ToStringShortColor(), Face.ToString());
                    break;
                case CardColor.Red:
                case CardColor.Yellow:
                case CardColor.Blue:
                case CardColor.Green:
                case CardColor.Bionic:
                    // Normal format.
                    value = String.Format(value,
                        Color.ToString(),
                        Face.ToString());
                    break;
            }

            Console.Write(value);
        }
        
        /// <summary>
        /// 2 characters info for this card.
        /// </summary>
        /// <returns>2 characters info for this card.</returns>
        public string ToStringShort()
        {   
            return String.Format("({0}{1,2})", ToStringShortColor(), ToStringShortFace());   
        }
        
        /// <summary>
        /// Get a single character in function of its color.
        /// </summary>
        /// <returns>Char with the code.</returns>
        public char? ToStringShortColor()
        {
            char? charColor = null;
            switch (Color)
            {
                case CardColor.Red: charColor = 'R'; break;
                case CardColor.Yellow: charColor = 'Y'; break;
                case CardColor.Green: charColor = 'G'; break;
                case CardColor.Blue: charColor = 'B'; break;
                case CardColor.Purple: charColor = null; break;
                case CardColor.Wildcard: charColor = '^'; break;
                case CardColor.Bionic: charColor = '<'; break;
            }
            return charColor;
        }

        /// <summary>
        /// Get a single character in function of its face.
        /// </summary>
        /// <returns>Char with the face</returns>
        public char ToStringShortFace()
        {
            char charFace;
            switch (Face)
            {
                case CardFace.Organ: charFace = 'O'; break;
                case CardFace.Medicine: charFace = '*'; break;
                case CardFace.Virus: charFace = '@'; break;
                default: charFace = '+'; break;
            }
            return charFace;
        }
        #endregion
    }
}
