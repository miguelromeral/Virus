using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Virus.Core
{
    /// <summary>
    /// Referee of the game. It allows players to do some actions based on the game state.
    /// </summary>
    [Serializable]
    public class Referee
    {
        /// <summary>
        /// Current game.
        /// </summary>
        public Game Game { get; set; }

        /// <summary>
        /// Referee constructor.
        /// </summary>
        /// <param name="g">Current game</param>
        public Referee(Game g)
        {
            Game = g;
        }

        /// <summary>
        /// Get a whole list of movements allowed if the player uses this card.
        /// </summary>
        /// <param name="me">Player who is gonna play</param>
        /// <param name="myCard">Card to play</param>
        /// <returns>List of possible moves with that card</returns>
        public List<string> GetListMovements(Player me, Card myCard, bool countme = false)
        {
            List<string> moves = new List<string>();
            Body body;
            int myId;

            switch (myCard.Face)
            {
                #region PLAY ORGAN
                case Card.CardFace.Organ:
                    // Check I haven't already a organ of this color
                    if (CanPlayOrgan(me, myCard))
                    {
                        moves.Add(Scheduler.GenerateMove(me.ID, me.GetIndexOfCardInHand(myCard)));
                    }

                    break;
                #endregion

                #region PLAY MEDICINE
                case Card.CardFace.Medicine:
                    body = me.Body;
                    for (int i = 0; i < body.Items.Count; i++)
                    {
                        if (CanPlayMedicine(body.Items[i], myCard))
                        {
                            moves.Add(Scheduler.GenerateMove(me.ID, i));
                        }
                    }
                    break;
                #endregion

                #region PLAY VIRUS
                case Card.CardFace.Virus:
                    myId = me.ID;
                    for (int i = 0; i < Game.Players.Count; i++)
                    {
                        Player rival = Game.Players[i];
                        // Can't play virus against myself.
                        
                        if (rival.ID != me.ID || countme)
                        {
                            body = rival.Body;
                            for (int j = 0; j < body.Items.Count; j++)
                            {
                                var item = body.Items[j];
                                if (CanPlayVirus(item, myCard))
                                {
                                    moves.Add(Scheduler.GenerateMove(rival.ID, j));
                                }
                            }
                        }
                    }
                    break;
                #endregion

                #region PLAY LATEX GLOVE OR OVERTIME
                case Card.CardFace.LatexGlove:
                case Card.CardFace.Overtime:
                    moves.Add(Scheduler.GenerateMove(me.ID, 0));
                    break;
                #endregion

                #region PLAY TRANSPLANT
                case Card.CardFace.Transplant:
                    Player one, two;
                    BodyItem bone, btwo;
                    // Recover any possible player combination
                    for (int p1 = 0; p1 < Game.Players.Count; p1++)
                    {
                        for (int p2 = p1 + 1; p2 < Game.Players.Count; p2++)
                        {
                            one = Game.Players[p1];
                            two = Game.Players[p2];

                            // Recover any body items combinations between selected players.
                            for (int bi1 = 0; bi1 < one.Body.Items.Count; bi1++)
                            {
                                for (int bi2 = 0; bi2 < two.Body.Items.Count; bi2++)
                                {
                                    bone = one.Body.Items[bi1];
                                    btwo = two.Body.Items[bi2];

                                    // Check if we can do the switch.
                                    if ((bone.Organ.Color == btwo.Organ.Color || 
                                        (!one.Body.HaveThisOrgan(btwo.Organ.Color) &&
                                        !two.Body.HaveThisOrgan(bone.Organ.Color))) &&
                                        bone.Status != BodyItem.State.Immunized &&
                                        btwo.Status != BodyItem.State.Immunized)
                                    {
                                        moves.Add(Scheduler.GetManyMoveItem(new string[]
                                        {
                                                Scheduler.GenerateMove(p1, bi1),
                                                Scheduler.GenerateMove(p2, bi2)
                                        }));
                                    }
                                }
                            }
                        }
                    }
                    break;
                #endregion

                #region PLAY SPREADING
                case Card.CardFace.Spreading:
                    int myCardIndex = 0;
                    List<List<string>> ListOfLists = new List<List<string>>();
                    Card modifier;
                    // Check every body item of our body.
                    foreach (BodyItem item in me.Body.Items)
                    {
                        // Check if we have a virus in our body item.
                        modifier = item.GetLastModifier();
                        if (modifier != null)
                        {
                            if (item.Status.Equals(BodyItem.State.Infected))
                            {
                                moves = new List<string>();

                                // Recover every player in the game.
                                int rivalid = 0;
                                foreach (Player rival in Game.Players)
                                {
                                    if (rival.ID != me.ID && !rival.PlayedProtectiveSuit)
                                    {
                                        // Check every body item for this rival.
                                        int rivalBodyItemIndex = 0;
                                        foreach (BodyItem ri in rival.Body.Items)
                                        {
                                            // Can spread only if it has the same color (or wild) and it's free.
                                            if (SameColorOrWildcard(modifier.Color, ri.Organ.Color) &&
                                                ri.Status.Equals(BodyItem.State.Free))
                                            {
                                                moves.Add(Scheduler.GetManyMoveItem(new string[] {
                                                    Scheduler.GenerateMove(me.ID, myCardIndex),
                                                    Scheduler.GenerateMove(rivalid, rivalBodyItemIndex)
                                                }));
                                            }
                                            rivalBodyItemIndex++;
                                        }
                                    }
                                    rivalid++;
                                }

                                ListOfLists.Add(moves);
                            }
                        }
                        myCardIndex++;
                    }
                    
                    return Scheduler.GetAllMovesSpreading(ListOfLists);
                #endregion

                #region PLAY ORGAN THIEF
                case Card.CardFace.OrganThief:
                    myId = me.ID;
                    for (int p = 0; p < Game.Players.Count; p++)
                    {
                        Player rival = Game.Players[p];
                        if (rival.ID != me.ID)
                        {
                            body = rival.Body;
                            for (int j = 0; j < body.Items.Count; j++)
                            {
                                var item = body.Items[j];
                                // If I haven't this organ yet and the rival's isn't immunized.
                                if (!me.Body.HaveThisOrgan(item.Organ.Color) && !item.Status.Equals(BodyItem.State.Immunized))
                                {
                                    moves.Add(Scheduler.GenerateMove(rival.ID, j));
                                }
                            }
                        }
                    }
                    break;
                #endregion

                #region PLAY MEDICAL ERROR OR SECOND OPINION
                case Card.CardFace.MedicalError:
                case Card.CardFace.SecondOpinion:
                    myId = me.ID;
                    for (int i = 0; i < Game.Players.Count; i++)
                    {
                        Player rival = Game.Players[i];
                        if (rival.ID != me.ID)
                        {
                            moves.Add(Scheduler.GenerateMove(rival.ID, 0));
                        }
                    }
                    return moves;
                #endregion

                #region PLAY EVOLVED MEDICINE
                case Card.CardFace.EvolvedMedicine:
                    body = me.Body;
                    for (int i = 0; i < body.Items.Count; i++)
                    {
                        if (CanPlayEvolvedMedicine(body.Items[i], myCard))
                        {
                            moves.Add(Scheduler.GenerateMove(me.ID, i));
                        }
                    }
                    break;
                #endregion

                #region PLAY EVOLVED VIRUS
                case Card.CardFace.EvolvedVirus:
                    myId = me.ID;
                    for (int i = 0; i < Game.Players.Count; i++)
                    {
                        Player rival = Game.Players[i];
                        // Can't play virus against myself.
                        if (rival.ID != me.ID || countme)
                        {
                            body = rival.Body;
                            for (int j = 0; j < body.Items.Count; j++)
                            {
                                var item = body.Items[j];
                                if (CanPlayVirus(item, myCard))
                                {
                                    moves.Add(Scheduler.GenerateMove(rival.ID, j));
                                }
                            }
                        }
                    }
                    break;
                #endregion


                #region QUARANTINE
                case Card.CardFace.Quarantine:
                    body = me.Body;
                    for (int j = 0; j < body.Items.Count; j++)
                    {
                        var item = body.Items[j];
                        // If I haven't this organ yet and the rival's isn't immunized.
                        if (item.Status.Equals(BodyItem.State.Infected))
                        {
                            moves.Add(Scheduler.GenerateMove(me.ID, j));
                        }
                    }
                    break;
                    #endregion

            }
            return moves;
        }
        

        public List<string> AddMyOwnMoves(List<string> moves, Card card)
        {
            // TODO, set my own moves.
            return moves;
        }


        /// <summary>
        /// Check if the player already hasn't an organ of this color.
        /// </summary>
        /// <param name="player">Player who is going to play this organ.</param>
        /// <param name="organ">Card organ to play.</param>
        /// <returns>True if the player hasn't the organ yet in his body.</returns>
        public bool CanPlayOrgan(Player player, Card organ)
        {
            Body body = player.Body;
            foreach(var item in body.Items)
            {
                if(item.Organ.Color == organ.Color)
                {
                    return false;
                }
            }
            return true;
        }

        public List<string> RemoveMovesPlayer(List<string> moves, int playerid, Card card, Player me)
        {
            List<string> filtered = new List<string>();
            
                switch (card.Face)
                {
                    case Card.CardFace.Transplant:
                        foreach (string m in moves)
                        {
                            if (Scheduler.GetStringInt(m, 0) != playerid &&
                                    Scheduler.GetStringInt(m, 4) != playerid)
                            {
                                filtered.Add(m);
                            }
                        }

                    break;
                    case Card.CardFace.Spreading:

                    foreach (var move in moves)
                    {
                        string[] choosen = move.Split(Scheduler.MULTI_MOVE_SEPARATOR);
                        for (int i = 0; i <= (choosen.Length / 2); i += 2)
                        {
                            string m = Scheduler.GetManyMoveItem(new string[] { choosen[i], choosen[i + 1] });
                            if (Char.GetNumericValue(m.ToCharArray()[4]) != playerid)
                            {
                                filtered.Add(m);
                            }
                        }
                    }

                    return GetListMovements(me, card, false);

                    break;
                    case Card.CardFace.Virus:
                    case Card.CardFace.EvolvedVirus:
                    case Card.CardFace.OrganThief:
                    case Card.CardFace.MedicalError:
                    case Card.CardFace.SecondOpinion:

                        foreach(string m in moves)
                        {
                            if (Scheduler.GetStringInt(m, 0) != playerid)
                            {
                                filtered.Add(m);
                            }
                        }

                        break;
                    case Card.CardFace.LatexGlove:
                        break;
                }
        
            return filtered;
        }


        /// <summary>
        /// Check if a medicine could be used in a body item.
        /// </summary>
        /// <param name="item">Body item to play this medicine.</param>
        /// <param name="medicine">Medicine to use.</param>
        /// <returns>True if the body item acepts this medicine</returns>
        public bool CanPlayMedicine(BodyItem item, Card medicine)
        {
            if (item.Organ.Color == Card.CardColor.Bionic)
            {
                return false;
            }

            if (Referee.SameColorOrWildcard(item.Organ.Color, medicine.Color) ||
                (item.Status == BodyItem.State.Infected &&
                Referee.SameColorOrWildcard(item.Modifiers[0].Color, medicine.Color)))
            {
                switch (item.Status)
                {
                    case BodyItem.State.Free:
                    case BodyItem.State.Vaccinated:
                        return true;
                    case BodyItem.State.Infected:
                        
                        if(item.Modifiers[0].Face == Card.CardFace.EvolvedVirus)
                        {
                            return false;
                        }
                        else{
                            return true;
                        }
                    default:
                        return false;
                }
            }
            return false;
        }
        public bool CanPlayEvolvedMedicine(BodyItem item, Card medicine)
        {
            if (item.Organ.Color == Card.CardColor.Bionic)
            {
                return false;
            }

            if (Referee.SameColorOrWildcard(item.Organ.Color, medicine.Color) ||
                (item.Status == BodyItem.State.Infected &&
                Referee.SameColorOrWildcard(item.Modifiers[0].Color, medicine.Color)))
            {
                switch (item.Status)
                {
                    case BodyItem.State.Free:
                    case BodyItem.State.Vaccinated:
                        return true;
                    case BodyItem.State.Infected:
                        return true;

                    default:
                        return false;
                }
            }
            return false;
        }

        /// <summary>
        /// Check if a body item can suffer a virus card.
        /// </summary>
        /// <param name="item">Body item to use the virus.</param>
        /// <param name="virus">Virs card to play.</param>
        /// <returns>True if the virus could be used in this body item</returns>
        public bool CanPlayVirus(BodyItem item, Card virus)
        {
            if(item.Organ.Color == Card.CardColor.Bionic)
            {
                return false;
            }

            // Same color of organn (or wildcard) and medicine of the same color of the mediccine (or wildcard)
            if (Referee.SameColorOrWildcard(item.Organ.Color, virus.Color) ||
                (item.Status == BodyItem.State.Vaccinated &&
                Referee.SameColorOrWildcard(item.Modifiers[0].Color, virus.Color)))
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

        public bool CanPlayEvolvedVirus(BodyItem item, Card virus)
        {
            if (item.Organ.Color == Card.CardColor.Bionic)
            {
                return false;
            }

            // Same color of organn (or wildcard) and medicine of the same color of the mediccine (or wildcard)
            if (Referee.SameColorOrWildcard(item.Organ.Color, virus.Color) ||
                (item.Status == BodyItem.State.Vaccinated &&
                Referee.SameColorOrWildcard(item.Modifiers[0].Color, virus.Color)))
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


        /// <summary>
        /// Check if two color are the same or wildcard color.
        /// </summary>
        /// <param name="color1">Color 1</param>
        /// <param name="color2">Color 2</param>
        /// <returns>True if they are equal or some of them is wildcard</returns>
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
