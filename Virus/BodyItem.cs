using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Virus.Core
{
    class BodyItem
    {
        private Card organ;
        private List<Card> modifiers;
        
        public Card Organ
        {
            get { return organ; }
        }
        public List<Card> Modifiers
        {
            get { return modifiers; }
        }
        public int Points
        {
            // Return points based on a IA function (TO DO)
            get { return 1; }
        }


        public BodyItem(Card o)
        {
            organ = o;
            modifiers = new List<Card>();
        }

        public enum State
        {
            Free,
            Infected,
            Vaccinated,
            Immunized,
            NOTINGAME
        }

        public State Status
        {
            get
            {
                switch (modifiers.Count)
                {
                    case 0:
                        // Free Organ
                        return State.Free;
                    case 1:
                        switch (modifiers[0].Face)
                        {
                            // Vaccinated organ
                            case Card.CardFace.Medicine:
                                return State.Vaccinated;
                            // Infected Organ
                            case Card.CardFace.Virus:
                                return State.Infected;
                            default:
                                return State.NOTINGAME;
                        }
                    case 2:
                        if (modifiers[0].Face == Card.CardFace.Medicine)
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


        public string NewMedicine(Card medicine)
        {
            switch (Status)
            {
                case State.Free:
                case State.Vaccinated:
                    modifiers.Add(medicine);
                    return null;
                case State.Infected:
                    modifiers.RemoveAt(0);
                    return null;
                case State.Immunized:
                    return String.Format("Your {0} is already immunized.", organ);
                default:
                    return "UNKNOWN STATE PUTTING THE MEDICINE.";
            }
            
        }

        public List<Card> GetAllCardsInBody()
        {
            List<Card> list = new List<Card>();

            if (organ != null)
                list.Add(organ);

            foreach(var c in modifiers)
            {
                list.Add(c);
            }

            return list;
        }

        public const string RULE_DELETEBODY = "--DELETE BODY ITEM--";

        public string NewVirus(Card virus, Game game)
        {
            switch (Status)
            {
                case State.Free:
                    modifiers.Add(virus);
                    return null;
                case State.Vaccinated:
                    game.MoveToDiscards(virus);
                    Card medicine = modifiers.ElementAt(0);
                    game.MoveToDiscards(medicine);
                    modifiers.Remove(medicine);
                    return null;
                case State.Infected:
                    game.MoveToDiscards(virus);
                    foreach (var c in GetAllCardsInBody())
                    {
                        game.MoveToDiscards(c);
                    }
                    return RULE_DELETEBODY;
                case State.Immunized:
                    return String.Format("The {0} immunized. You cannot put the virus.", organ);
                default:
                    return "UNKNOWN STATE PUTTING THE VIRUS.";
            }
        }




        public override string ToString()
        {
            //string printed = String.Format(" M:"+modifiers.Count+"    {0}: ", organ.ToString());
            string printed = String.Format("    {0}: ", organ.ToString());

            foreach(var mod in modifiers)
            {
                switch (mod.Face)
                {
                    case Card.CardFace.Virus:
                        printed += "(@)";
                        break;
                    case Card.CardFace.Medicine:
                        printed += "(+)";
                        break;
                }
            }
            return printed;
        }

    }
}
