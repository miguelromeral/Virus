using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Virus.Core
{
    class Body
    {
        private int points;

        private List<BodyItem> organs;

        
        public int Points
        {
            get { return points; }
        }
        public List<BodyItem> Organs
        {
            get { return organs; }
        }

        
        public Body()
        {
            points = 0;
            organs = new List<BodyItem>();
        }


        public override string ToString()
        {
            string printed = String.Empty;

            foreach(BodyItem item in Organs)
            {
                printed += item.ToString() + "\n";
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

        public string SetVirus(Card virus, Game game)
        {
            if (virus.Color != Card.CardColor.Wildcard)
            {
                if (HaveThisOrgan(virus.Color))
                {
                    BodyItem item = GetOrganByColor(virus.Color);
                    string message = item.NewVirus(virus, game);
                    if (message != null && message.Equals(BodyItem.RULE_DELETEBODY))
                    {
                        organs.Remove(item);
                        message = null;
                    }
                    return message;
                }
                else
                {
                    return "PLAYER HAS NOT THIS ORGAN COLOR";
                }
            }
            else
            {
                // Wildcard virus (TODO)
                //return "WILDCARD VIRUS (TODO)";
            }
            return "UNKNOWN VIRUS";
        }

        public string SetMedicine(Card medicine)
        {
            if (medicine.Color != Card.CardColor.Wildcard)
            {
                if (HaveThisOrgan(medicine.Color))
                {
                    return GetOrganByColor(medicine.Color).NewMedicine(medicine);
                }
                else
                {
                    return "You don't have an organ available to put this medicine.";
                }
            }
            else
            {
                // Wildcard medicine (TODO)
                //return "WILDCARD MEDICINE (TODO)";
            }
            return "UNKNOWN MEDICINE";
        }
    }
}
