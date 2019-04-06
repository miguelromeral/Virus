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

            if(color != CardColor.Purple)
            {
                if(color == CardColor.Wildcard)
                {
                    value = String.Format(String.Format("{0} {1}", Scheduler.CHARS_WILDCARD, value),
                        color.ToString(),
                        face.ToString());
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
                value = String.Format(value, face, null);
            }

            return value;
        }
        

    }
}
