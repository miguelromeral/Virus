using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Virus.Core
{
    public class Body
    {
        private List<BodyItem> organs;

        
        public int Points
        {
            get {
                int p = 0;
                foreach(var item in organs)
                {
                    p += item.Points;
                }
                return p;
            }
        }
        public List<BodyItem> Organs
        {
            get { return organs; }
        }

        
        public Body()
        {
            organs = new List<BodyItem>();
        }


        public override string ToString()
        {
            string printed = String.Empty;

            int i = 1;
            foreach(BodyItem item in Organs)
            {
                printed += i +".     "+ item.ToString() + Environment.NewLine;
                i++;
            }
            return printed;
        }

        public bool HaveThisOrgan(Card.CardColor color)
        {
            foreach(var item in organs)
            {
                if (item.Organ.Color.Equals(color))
                {
                    return true;
                }
            }
            return false;
        }

        public BodyItem GetOrganByColor(Card.CardColor color)
        {
            foreach(var i in organs)
            {
                if(i.Organ.Color == color)
                {
                    return i;
                }
            }
            return null;
        }

        public string SetOrgan(Card organ)
        {
            if (HaveThisOrgan(organ.Color))
            {
                return "You already have this organ in your body.";
            }
            else
            {
                organs.Add(new BodyItem(organ));
                return null;
            }
        }

        public string SetVirus(Card virus, int index, Game game)
        {
            BodyItem item = organs[index];

            if (virus.Color == item.Organ.Color ||
                virus.Color == Card.CardColor.Wildcard ||
                item.Organ.Color == Card.CardColor.Wildcard)
            {
                string message = item.NewVirus(virus, game);
                if (message != null && message.Equals(BodyItem.RULE_DELETEBODY))
                {
                    organs.Remove(item);
                    game.logger.Write("The "+item+" has been removed from the body.");
                }
                return message;
            }
            else
            {
                return "The virus and organ color don't match.";
            }
        }

        public string SetMedicine(Game game, Card medicine, int index = 0)
        {
            BodyItem item = organs[index];

            if (medicine.Color == item.Organ.Color ||
                medicine.Color == Card.CardColor.Wildcard ||
                item.Organ.Color == Card.CardColor.Wildcard)
            {
                return organs[index].NewMedicine(game, medicine);
            }
            else
            {
                return "The medicine and organ color don't match.";
            }
        }
        
    }
}
