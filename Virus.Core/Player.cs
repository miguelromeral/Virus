using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Virus.Core
{
    public class Player
    {
        private ArtificialIntelligence.AICategory ai;
        private ArtificialIntelligence computer = null;
        private List<Card> hand;
        private Body body;
        private int id;

        public ArtificialIntelligence.AICategory Ai {
            get { return ai; }
        }
        public ArtificialIntelligence Computer
        {
            get { return computer; }
        }

        public string ShortDescription
        {
            get { return "Player " + id; }
        }


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
            set { body = value; }
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
        

        public Player(Game game, bool human = false)
        {
            Console.WriteLine("Creating new player.");
            body = new Body();
            if (human)
            {
                ai = ArtificialIntelligence.AICategory.Human;
            }
            else
            {
                computer = new ArtificialIntelligence(game, this);
                ai = computer.RandomIA();
            }
        }

        public void NewHand(List<Card> h)
        {
            hand = h;
        }

        public override string ToString()
        {
            string printed = String.Empty;

            printed += "* IA: " + ai.ToString() + "\n";
            printed += "* Body: \n";
            printed += body + "\n";
            
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
        
    }
}
