using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Virus.Core;

namespace Virus.Forms
{
    public partial class GameForm : Form
    {
        public CGame Game;
        public CCheckBox CardSelected;
        private FormUtilities Utilities;

        public GameForm()
        {
            InitializeComponent();



            Game = new CGame(3, 5000, tbLog, true);


            Utilities = new FormUtilities(this, Game, Game.Players[0], MainLayout, pUserHand)
            {
                LTurns = lTurns,
                TBState = tbGame,
                BDiscards = bDiscard,
            };
            //this.bDiscard.Click += new System.EventHandler(Utilities.CardClicked);
            this.bDiscard.Click += new System.EventHandler(Utilities.bDiscard_Click);


            Utilities.UpdateGUI();
        }




        public void DoMove()
        {
            //UpdateGUI();
            //CheckGameOver();
            //while (!Game.IsMyTurn(Me))
            //{

                Game.PlayTurn();
                Utilities.UpdateGUI();
                //CheckGameOver();
                
            
                //System.Threading.Thread.Sleep(2000);
            //}
        }

        private void button1_Click(object sender, EventArgs e)
        {

            //new Task(delegate { DoMove(); }).Start();
            DoMove();

        }

        



        

        //private List<string> CurrentMoves;

        //public const string ACTION_TRANSPLANT = "Transplanting";
        //private string action = null;

        //public bool EventCard(CCheckBox cb)
        //{
        //    if (cb.InHand)
        //    {
        //        // No actions when choosing the discards
        //        if (Discarding)
        //        {
        //            return false;
        //        }

        //        if (CardSelected != null)
        //            CardSelected.Checked = false;

        //        Player me = Game.GetPlayerByID(cb.PlayerId);

        //        List<string> moves = Game.Referee.GetListMovements(me, cb.Card, false);

        //        if (moves.Count == 1)
        //        {
        //            Game.PlayGameCardByUser(me, cb.Index, moves, cb.Card);
        //            Game.DrawCardsToFill(me);
        //            Game.Turn++;
        //            Utilities.UpdateGUI();
        //            CurrentMoves = null;
        //            return true;
        //        }
        //        else
        //        {
        //            Card.CardFace face = cb.Card.Face;
        //            switch (cb.Card.Face)
        //            {
        //                // Cards to only show a inputbox:
        //                case Card.CardFace.SecondOpinion:
        //                case Card.CardFace.MedicalError:

        //                    string[] options = new string[Game.Players.Count - 1];
        //                    int i = 0;
        //                    foreach (Player p in Game.Players)
        //                    {
        //                        if (p.ID != Me.ID)
        //                        {
        //                            options[i] = p.Nickname;
        //                            i++;
        //                        }
        //                    }

        //                    #region DIALOG
        //                    //Set buttons language Czech/English/German/Slovakian/Spanish (default English)
        //                    InputBox.SetLanguage(InputBox.Language.English);

        //                    //Save the DialogResult as res
        //                    DialogResult res = InputBox.ShowDialog("To which player do you want to use this card?",
        //                    "Play this card",   //Text message (mandatory), Title (optional)
        //                        InputBox.Icon.Question, //Set icon type (default info)
        //                                                //InputBox.Icon.Information, //Set icon type (default info)
        //                        InputBox.Buttons.OkCancel, //Set buttons (default ok)
        //                        InputBox.Type.ComboBox, //Set type (default nothing)
        //                        options, //String field as ComboBox items (default null)
        //                        true //Set visible in taskbar (default false)
        //                        );
        //                    #endregion

        //                    //Check InputBox result
        //                    if (res == System.Windows.Forms.DialogResult.OK || res == System.Windows.Forms.DialogResult.Yes)
        //                    {
        //                        string user = InputBox.ResultValue;
        //                        string move;

        //                        foreach (Player p in Game.Players)
        //                        {
        //                            if (p.Nickname == user)
        //                            {
        //                                move = Scheduler.GenerateMove(p.ID, 0);
        //                                if (Game.PlayUserCardByMove(Me, cb.Card, move, moves))
        //                                {
        //                                    CurrentMoves = null;
        //                                    return true;
        //                                }
        //                                else
        //                                {
        //                                    // TODO
        //                                    return false;
        //                                }
        //                            }
        //                        }
        //                    }
        //                    break;

        //                // ????????
        //                //case Card.CardFace.Spreading:

        //                case Card.CardFace.Transplant:
        //                    action = ACTION_TRANSPLANT;
        //                    CurrentMoves = moves;
        //                    break;

        //                default:
        //                    CardSelected = cb;
        //                    CurrentMoves = moves;
        //                    return false;
        //            }
        //            return false;
        //        }
        //    }
        //    else
        //    {
        //        if(action == null)
        //        {
        //            bool movedone = false;
        //            if (CardSelected != null && CurrentMoves != null)
        //            {
        //                bool allow = false;
        //                switch (CardSelected.Card.Face)
        //                {
        //                    case Card.CardFace.Medicine:
        //                    case Card.CardFace.EvolvedMedicine:
        //                    case Card.CardFace.Virus:
        //                    case Card.CardFace.EvolvedVirus:
        //                    case Card.CardFace.Quarantine:
        //                    case Card.CardFace.OrganThief:
        //                        if (cb.Card.Face == Card.CardFace.Organ)
        //                        {
        //                            allow = true;
        //                        }
        //                        break;
        //                    case Card.CardFace.Transplant:
        //                    case Card.CardFace.Spreading:
        //                    // Already threated:
        //                    case Card.CardFace.LatexGlove:
        //                    case Card.CardFace.Overtime:
        //                    case Card.CardFace.ProtectiveSuit:

        //                    default:
        //                        allow = false;
        //                        break;
        //                }
        //                if (allow)
        //                {
        //                    string move = Game.GetMoveGivenCCheckBox(CardSelected, cb);
        //                    if (move != null)
        //                    {
        //                        movedone = Game.PlayUserCardByMove(Me, CardSelected.Card, move, CurrentMoves);
        //                    }
        //                }
        //            }
        //            if (movedone)
        //            {
        //                return true;
        //            }
        //            else
        //            {
        //                cb.Checked = false;
        //                if (CardSelected != null)
        //                    CardSelected.Checked = false;
        //                CardSelected = null;
        //                CurrentMoves = null;
        //                return false;
        //            }
        //        }
        //        else if(action == ACTION_TRANSPLANT)
        //        {
        //            if(CardSelected == null)
        //            {
        //                if(cb.Card.Face == Card.CardFace.Organ)
        //                {
        //                    CardSelected = cb;
        //                }
        //                else
        //                {
        //                    cb.Checked = false;
        //                }
        //                return false;
        //            }
        //            else
        //            {
        //                if(cb.Card.Face == Card.CardFace.Organ)
        //                {
        //                    bool movedone = false;
        //                    string move = Game.GetMoveGivenCCheckBox(CardSelected, cb, action);
        //                    if (move != null)
        //                    {
        //                        movedone = Game.PlayUserCardByMove(Me, CardSelected.Card, move, CurrentMoves);
        //                    }
        //                    if (movedone)
        //                    {
        //                        action = null;
        //                        return true;
        //                    }
        //                    else
        //                    {
        //                        cb.Checked = false;
        //                        if (CardSelected != null)
        //                            CardSelected.Checked = false;
        //                        CardSelected = null;
        //                        CurrentMoves = null;
        //                    }
        //                }
        //                else
        //                {
        //                    CardSelected.Checked = false;
        //                    CardSelected = null;
        //                    cb.Checked = false;
        //                }
        //                return false;
        //            }
        //        }
        //        return false;
        //    }
        //}

        
        //private void CheckGameOver()
        //{
        //    if (Game.GameOver)
        //    {
        //        if (Game.AmITheWinner(Me.ID))
        //        {
        //            MessageBox.Show("Congratulations! You've won this game!", "You're the winner!");
        //        }
        //        else
        //        {
        //            Player p = Game.GetTheWinner();
        //            MessageBox.Show(p.Nickname+" has won this game. Try next time.", "Game over");
        //        }
        //        Application.Exit();

        //    }
        //}

        
        //private bool Discarding = false;

    }
}
