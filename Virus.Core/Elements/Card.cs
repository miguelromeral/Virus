using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Virus.Core
{
    public class Card
    {

        public enum CardColor
        {
            Red,
            Yellow,
            Green,
            Blue,
            Purple,
            Wildcard
        }

        public enum CardFace
        {
            Organ,
            Virus,
            Medicine,
            Transplant,
            OrganThief,
            Spreading,
            LatexGlove,
            MedicalError
        }

        public const int NUM_COLORS = 6;

        private CardColor color;
        private CardFace face;

        public CardColor Color
        {
            get { return color; }
        }
        public CardFace Face
        {
            get { return face; }
        }

        public int Value
        {
            get {
                // TODO
                return 0;
            }
        }

        // Constructor
        public Card(CardColor c, CardFace f)
        {
            color = c;
            face = f;
        }


        public override string ToString()
        {
            string value = "{0} {1}";
            
            if (color != CardColor.Purple)
            {
                if (color == CardColor.Wildcard)
                {
                    value = String.Format(value, ToStringShortColor(), face.ToString());
                }
                else
                {
                    value = String.Format(value,
                        color.ToString(),
                        face.ToString());
                }
            }
            else
            {
                value = String.Format(value, ToStringShort(), face);
            }

            return value;
        }
        
        public string ToStringShort()
        {   
            return String.Format("({0}{1})", ToStringShortColor(), ToStringShortFace());   
        }
        
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
            }
            return charColor;
        }

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

    }
}
