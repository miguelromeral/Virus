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
        public List<string> GetListMovements(Player me, Card myCard)
        {
            List<string> moves = new List<string>();
            Body body;
            int myId;

            switch (myCard.Face)
            {
                case Card.CardFace.Organ:


                    if (CanPlayOrgan(me, myCard))
                    {
                        moves.Add(Scheduler.GetMoveItem(me.ID, me.GetIndexOfCardInHand(myCard)));
                    }
                    break;


                case Card.CardFace.Medicine:
                    body = me.Body;
                    for (int i = 0; i < body.Organs.Count; i++)
                    {
                        if (CanPlayMedicine(body.Organs[i], myCard))
                        {
                            moves.Add(Scheduler.GetMoveItem(me.ID, i));
                        }
                    }
                    break;

                case Card.CardFace.Virus:
                    myId = me.ID;
                    for (int i = 0; i < Game.Players.Count; i++)
                    {
                        Player rival = Game.Players[i];
                        if (rival.ID != me.ID)
                        {
                            body = rival.Body;
                            for (int j = 0; j < body.Organs.Count; j++)
                            {
                                var item = body.Organs[j];
                                if (CanPlayVirus(item, myCard))
                                {
                                    moves.Add(Scheduler.GetMoveItem(rival.ID, j));
                                }
                            }
                        }
                    }
                    break;

                //    case Card.CardFace.Transplant:
                //        Player one, two;
                //        BodyItem bone, btwo;
                //        for (int i = 0; i < Players.Count; i++)
                //        {
                //            for (int j = i + 1; j < Players.Count; j++)
                //            {
                //                one = Players[i];
                //                two = Players[j];

                //                for (int x = 0; x < one.Body.Organs.Count; x++)
                //                {
                //                    for (int y = 0; y < two.Body.Organs.Count; y++)
                //                    {
                //                        bone = one.Body.Organs[x];
                //                        btwo = two.Body.Organs[y];
                //                        if (!one.Body.HaveThisOrgan(btwo.Organ.Color) &&
                //                            !two.Body.HaveThisOrgan(bone.Organ.Color))
                //                        {
                //                            moves.Add(Scheduler.GetManyMoveItem(new string[]
                //                            {
                //                                Scheduler.GetMoveItem(i, x),
                //                                Scheduler.GetMoveItem(j, y)
                //                            }));

                //                        }
                //                    }
                //                }
                //            }
                //        }
                //        break;

                //    case Card.CardFace.Spreading:
                //        int myCardIndex = 0;
                //        Card modifier;
                //        foreach (BodyItem item in me.Body.Organs)
                //        {
                //            modifier = item.GetLastModifier();
                //            if (modifier != null)
                //            {
                //                if (item.Status.Equals(BodyItem.State.Infected))
                //                {
                //                    int j = 0;
                //                    foreach (Player rival in Players)
                //                    {
                //                        if (rival.ID != me.ID)
                //                        {
                //                            int k = 0;
                //                            foreach (BodyItem ri in rival.Body.Organs)
                //                            {
                //                                if (SameColorOrWildcard(modifier.Color, ri.Organ.Color) &&
                //                                    ri.Status.Equals(BodyItem.State.Free))
                //                                {
                //                                    moves.Add(Scheduler.GetManyMoveItem(new string[] {
                //                                    Scheduler.GetMoveItem(me.ID, myCardIndex),
                //                                    Scheduler.GetMoveItem(j, k)
                //                                }));
                //                                }
                //                                k++;
                //                            }
                //                        }
                //                        j++;
                //                    }
                //                }
                //            }
                //            myCardIndex++;
                //        }
                //        return moves;

                //    case Card.CardFace.OrganThief:
                //        myId = me.ID;
                //        for (int i = 0; i < Players.Count; i++)
                //        {
                //            Player rival = Players[i];
                //            if (rival.ID != me.ID)
                //            {
                //                body = rival.Body;
                //                for (int j = 0; j < body.Organs.Count; j++)
                //                {
                //                    var item = body.Organs[j];
                //                    if (!me.Body.HaveThisOrgan(item.Organ.Color) && !item.Status.Equals(BodyItem.State.Immunized))
                //                    {
                //                        moves.Add(Scheduler.GetMoveItem(rival.ID, j));
                //                    }
                //                }
                //            }
                //        }
                //        break;

                case Card.CardFace.MedicalError:
                    myId = me.ID;
                    for (int i = 0; i < Game.Players.Count; i++)
                    {
                        Player rival = Game.Players[i];
                        if (rival.ID != me.ID)
                        {
                            moves.Add(Scheduler.GetMoveItem(rival.ID, 0));
                        }
                    }
                    return moves;
            }


            return moves;
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

        public bool CanPlayVirus(BodyItem item, Card virus)
        {
            if (Referee.SameColorOrWildcard(item.Organ.Color, virus.Color))
            {
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
            return false;
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
