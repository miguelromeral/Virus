using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Virus.Core
{
    public class Referee
    {
        public Game Game { get; set; }

        public Referee(Game g)
        {
            Game = g;
        }


        public bool CanPlayOrgan(Player player, Card organ)
        {
            Body body = player.Body;
            foreach(var item in body.Organs)
            {
                if(item.Organ.Color == organ.Color)
                {
                    return false;
                }
            }
            return true;
        }


        public bool CanPlayMedicine(BodyItem item, Card medicine)
        {
            if (!item.Organ.Color.Equals(Card.CardColor.Wildcard) &&
                !medicine.Color.Equals(item.Organ.Color) &&
                !medicine.Color.Equals(Card.CardColor.Wildcard))
            {
                return false;
            }

            switch (item.Status)
            {
                case BodyItem.State.Free:
                case BodyItem.State.Vaccinated:
                case BodyItem.State.Infected:
                    return true;
                default:
                    return false;
            }
        }


        public static bool SameColorOrWildcard(Card.CardColor color1, Card.CardColor color2)
        {
            if (color1 == color2 ||
                color1 == Card.CardColor.Wildcard ||
                color2 == Card.CardColor.Wildcard)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
