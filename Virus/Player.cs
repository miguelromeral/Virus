using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Virus.Core
{
    class Player
    {
        public enum IA
        {
            Human,
            Easy,
            Medium,
            Hard
        }

        private List<Card> hand;
        private Body body;
        private IA ia;
        private int id;

        public int ID
        {
            get { return id; }
            set { id = value; }
        }
        public List<Card> Hand
        {
            get { return hand; }
        }

        public Body Body
        {
            get { return body; }
        }

        public int HealthyOrgans
        {
            get
            {
                int count = 0;
                foreach (var item in body.Organs)
                {
                    switch (item.Status)
                    {
                        case BodyItem.State.Free:
                        case BodyItem.State.Vaccinated:
                        case BodyItem.State.Immunized:
                            count++;
                            break;
                    }
                }
                return count;
            }
        }


        public List<string> GetMovesWildcardMedicine()
        {
            List<string> moves = new List<string>();
            BodyItem item;
            BodyItem.State status;

            for (int i = 0; i < body.Organs.Count; i++)
            {
                item = body.Organs.ElementAt(i);
                status = item.Status;
                if (status == BodyItem.State.Free ||
                    status == BodyItem.State.Vaccinated)
                {
                    moves.Add(Scheduler.GetMoveItem(id, i));
                }
            }

            return moves;
        }


        public Player(bool human = false)
        {
            Console.WriteLine("Creating new player.");
            body = new Body();
            if (human)
            {
                ia = IA.Human;
                Console.WriteLine("Human player created.");
            }
            else
            {
                ia = RandomIA();
                Console.WriteLine(String.Format("Player created with IA {0}.", ia.ToString()));
            }
        }

        public void NewHand(List<Card> h)
        {
            hand = h;
        }

        public static IA RandomIA()
        {
            return (IA)new Random().Next(1, Enum.GetValues(typeof(IA)).Length);
        }

        public override string ToString()
        {
            string printed = String.Empty;

            printed += "* IA: " + ia.ToString() + "\n";
            printed += "* Body: \n";
            printed += body + "\n";
            //printed += "* Current Hand:\n";
            //printed += PrintHand() + "\n";

            return printed;
        }


        // Return false if there is no cards.
        public void PrintMyOptions(bool discarding = false)
        {
            int i = 0;
            while (i < hand.Count)
            {
                Console.WriteLine("{0}.- {1}", (i + 1), hand[i]);
                i++;
            }
            if (discarding)
            {
                Console.WriteLine("0.- End discarding");
            }
            else
            {
                Console.WriteLine("0.- Discard");
            }

        }

        public string PlayCardPlayer(Card myCard, bool user = false)
        {
            switch (myCard.Face)
            {
                case Card.CardFace.Organ:
                    return body.SetOrgan(myCard);
                case Card.CardFace.Medicine:
                    return body.SetMedicine(myCard);

                default:
                    return " UNKNOWN CARD PLAYED IN PLAYER";
            }
        }
    }
}
