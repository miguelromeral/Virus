using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Virus.Core;

namespace Virus.Forms
{
    public class FormUtilities
    {
        private Game Game;
        private GameForm Form;

        public Label LTurns { get; set; }
        public TextBox TBState { get; set; }
        public Button BDiscards { get; set; }

        public Player Me { get; }



        private List<Panel> PlayerPanels = new List<Panel>();
        private Panel UserHandPanel;


        private Dictionary<int, List<Panel>> BodyItemPanels = new Dictionary<int, List<Panel>>();
        private List<Panel> UserHandPanels = new List<Panel>();
        private List<CCheckBox> UserHandCards = new List<CCheckBox>();


        public enum Action
        {
            None,
            Selecting,
            Discarding
        }

        private List<CCheckBox> SelectedCards = new List<CCheckBox>();
        private Action action = Action.None;


        public FormUtilities(GameForm form, Game g, Player m, TableLayoutPanel layout, Panel UserHand)
        {
            Form = form;
            Game = g;
            Me = m;
            UserHandPanel = UserHand;
            InitPanels(layout);
        }

        #region GUI
        public void UpdateGUI()
        {
            UpdateAllPlayerPanels();
            UpdateGamePanel();
            UpdateUserHand();
        }

        private void UpdateAllPlayerPanels()
        {
            foreach (var p in Game.Players)
                UpdatePlayerPanel(p.ID);
        }
        
        private Panel GetBodyItemPanel(int id, int item)
        {
            List<Panel> list = null;
            BodyItemPanels.TryGetValue(id, out list);
            if (list == null)
            {
                return null;
            }

            return list[item];
        }

        private void UpdatePlayerPanel(int id)
        {
            Player p = Game.Players[id];
            Panel pItem;
            int i = 0;
            for (i = 0; i < p.Body.Items.Count; i++)
            {
                pItem = GetBodyItemPanel(id, i);

                pItem.Controls.Clear();

                BodyItem item = p.Body.Items[i];

                CCheckBox cb = FormUtilities.CreateButtonCheckBoxCard(item.Organ, 0.4, id, i, false);

                //cb.Click += CardClicked;
                cb.Click += CardClicked2;

                pItem.Controls.Add(cb);

                for (int j = 0; j < item.Modifiers.Count; j++)
                {
                    cb = FormUtilities.CreateButtonCheckBoxCard(item.Modifiers[j], 0.25, id, i, false);
                    //cb.Click += CardClicked;
                    cb.Click += CardClicked2;

                    pItem.Controls.Add(cb);
                }

            }
            while (i < Game.Settings.NumberToWin)
            {
                pItem = GetBodyItemPanel(id, i);
                pItem.Controls.Clear();
                i++;
            }
        }
        
        private void UpdateGamePanel()
        {
            LTurns.Text = "Turn #" + Game.Turn;
            TBState.Text = Game.ToString();
        }
        #endregion

        public void CardClicked2(object sender, EventArgs e)
        {
            CCheckBox cb = (CCheckBox)sender;
            switch (action)
            {
                case Action.None:
                    {
                        //if (!cb.InHand)
                        //{
                        //    cb.Checked = false;
                        //    return;
                        //}
                        //else
                        //{
                        //    SelectedCards.Add(cb);


                        //}
                    }
                    break;
                case Action.Discarding:
                    {
                        // Check or uncheck card to discard or not.
                        if (cb.Checked)
                        {
                            SelectedCards.Add(cb);
                            //if (SelectedCards.Count == Game.Settings.NumberCardInHand)
                            //{
                            //    // Automatic click to discard all hand.
                            //    bDiscard_Click(BDiscards, e);
                            //}
                        }
                        else
                        {
                            SelectedCards.Remove(cb);
                        }
                    }
                    break;
                default:
                    MessageBox.Show("NO ACTIONS!");
                    return;
            }
        }





        public void CardClicked(object sender, EventArgs e)
        {
            //bool done = Form.EventCard((CCheckBox)sender);
            //if (done)
            //{
            //    DoMove();
            //}
        }

        private void UpdateUserHand()
        {
            UserHandCards.Clear();
            foreach (var p in Game.Players)
            {
                if (p.AI != ArtificialIntelligence.AICategory.Human)
                    break;

                int i;
                Panel pc;
                for (i = 0; i < p.Hand.Count; i++)
                {
                    Card c = p.Hand[i];
                    pc = UserHandPanels[i];
                    var cb = FormUtilities.CreateButtonCheckBoxCard(c, 0.4, p.ID, i, true);
                    //cb.Click += CardClicked;
                    cb.Click += CardClicked2;
                    UserHandCards.Add(cb);
                    pc.Controls.Clear();
                    pc.Controls.Add(cb);
                }
                while (i < Game.Settings.NumberCardInHand)
                {
                    pc = UserHandPanels[i];
                    pc.Controls.Clear();
                    i++;

                }
            }

            if (Game.IsMyTurn(Me.ID))
            {
                ChangeUserButtons(true);
            }
            else
            {
                ChangeUserButtons(false);
            }

        }




        private void ChangeUserButtons(bool enable)
        {
            foreach (var cb in UserHandCards)
            {
                cb.Enabled = enable;
            }
            BDiscards.Enabled = enable;
        }

        bool Discarding;

        public void bDiscard_Click(object sender, EventArgs e)
        {
            Button b = (Button)sender;

            if(action != Action.Discarding)
            {
                ClearAllCheckedCardHand();
                action = Action.Discarding;
                b.Text = "End discarding";
            }
            else
            {
                action = Action.None;
                b.Text = "Discard";

                if (SelectedCards.Count > 0)
                {
                    foreach (var cb in SelectedCards)
                    {
                        Game.DiscardFromHand(Me, cb.Card);
                    }
                    SelectedCards.Clear();
                    Game.DrawCardsToFill(Me);
                    Game.Turn++;
                    UpdateGUI();
                }
            }



            //Button b = (Button)sender;
            //if (Discarding)
            //{
            //    bool discarded = false;
            //    Player p = null;
            //    foreach (var cb in UserHandCards)
            //    {
            //        if (cb.Checked)
            //        {
            //            if (p == null)
            //                p = Game.GetPlayerByID(cb.PlayerId);

            //            Game.DiscardFromHand(p, cb.Card);
            //            discarded = true;
            //        }
            //    }
            //    if (discarded)
            //    {
            //        Game.DrawCardsToFill(p);
            //        Game.Turn++;
            //    }


            //    Discarding = false;
            //    b.Text = "Begin to discard";

            //    Form.DoMove();

            //}
            //else
            //{
            //    Discarding = true;
            //    b.Text = "Discard selected";
            //    ClearAllCheckedCardHand();
            //}
        }


        private void ClearAllCheckedCardHand()
        {
            foreach (var cb in UserHandCards)
            {
                cb.Checked = false;
            }
        }

        private void InitPanels(TableLayoutPanel MainLayout)
        {
            MainLayout.ColumnCount = Game.Settings.NumberToWin;
            MainLayout.ColumnStyles.Clear();
            for (int i = 0; i < Game.Settings.NumberToWin; i++)
            {
                MainLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, (float)(1 / Game.Settings.NumberToWin)));
                //MainLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 100));

            }
            MainLayout.RowCount = Game.Players.Count * 2;
            MainLayout.RowStyles.Clear();
            for (int i = 0; i < Game.Players.Count; i++)
            {
                MainLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 50));
                MainLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 200));
            }
            int count = 0;
            foreach (var p in Game.Players)
            {
                Label label = new Label()
                {
                    Text = p.Nickname
                };

                FlowLayoutPanel pBody = new FlowLayoutPanel()
                {
                    Name = "panel_user_" + p.ID,
                    FlowDirection = FlowDirection.LeftToRight,
                    BorderStyle = BorderStyle.FixedSingle,
                    AutoScroll = false,
                    AutoSize = true,
                    AutoSizeMode = AutoSizeMode.GrowAndShrink,
                };
                PlayerPanels.Add(pBody);

                List<Panel> bil = new List<Panel>();
                for (int i = 0; i < Game.Settings.NumberToWin; i++)
                {
                    FlowLayoutPanel pItem = new FlowLayoutPanel()
                    {
                        FlowDirection = FlowDirection.LeftToRight,
                        AutoScroll = false,
                        AutoSize = true,
                        AutoSizeMode = AutoSizeMode.GrowAndShrink,
                    };
                    bil.Add(pItem);

                    MainLayout.Controls.Add(pItem, i, count + 1);
                }
                BodyItemPanels.Add(p.ID, bil);

                MainLayout.Controls.Add(label, 0, count);
                //MainLayout.Controls.Add(pBody);
                count += 2;
            }
            for (int i = 0; i < Game.Settings.NumberCardInHand; i++)
            {
                FlowLayoutPanel pHand = new FlowLayoutPanel()
                {
                    FlowDirection = FlowDirection.LeftToRight,
                    AutoScroll = true,
                    AutoSize = true,
                    AutoSizeMode = AutoSizeMode.GrowAndShrink,
                };
                UserHandPanels.Add(pHand);
                UserHandPanel.Controls.Add(pHand);
            }
        }




        public static CCheckBox CreateButtonCheckBoxCard(Card c, double perc, int playerid, int index, bool user)
        {
            int width = (int)(200 * perc);
            int height = (int)(279 * perc);

            Image myimage = GetImageFromCard(c);

            CCheckBox b = new CCheckBox()
            {
                //Text = c.ToString(),
                
                Card = c,
                InHand = user,
                PlayerId = playerid,
                Percentage = perc,
                Index = index,
                CardImage = myimage,
                BackgroundImage = myimage,
                Width = width,
                Height = height,
                Appearance = Appearance.Button
            };
            b.CheckedChanged += new System.EventHandler(b.SetTransparency);



            b.BackgroundImage = new Bitmap(b.BackgroundImage, b.Width, b.Height);
            

            return b;
        }



        public static Image SetImageOpacity(Image image, double per, float opacity)
        {
            Bitmap bmp = new Bitmap((int) (per * image.Width), (int) (per * image.Height));
            using (Graphics g = Graphics.FromImage(bmp))
            {
                ColorMatrix matrix = new ColorMatrix();
                matrix.Matrix33 = opacity;
                ImageAttributes attributes = new ImageAttributes();
                attributes.SetColorMatrix(matrix, ColorMatrixFlag.Default,
                                                  ColorAdjustType.Bitmap);
                g.DrawImage(image, new Rectangle(0, 0, bmp.Width, bmp.Height),
                                   0, 0, image.Width, image.Height,
                                   GraphicsUnit.Pixel, attributes);
            }
            return bmp;
        }


        protected static Image GetImageFromCard(Card c)
        {
            return new Bitmap(GetImageFromCardString(c));
        }

        protected static string GetImageFromCardString(Card c)
        {
            string path = Directory.GetCurrentDirectory() + "/Images/";

            switch (c.Color)
            {
                case Card.CardColor.Bionic:
                    path += "bionic";
                    break;
                case Card.CardColor.Blue:
                    path += "blue";
                    break;
                case Card.CardColor.Green:
                    path += "green";
                    break;
                case Card.CardColor.Red:
                    path += "red";
                    break;
                case Card.CardColor.Yellow:
                    path += "yellow";
                    break;
                case Card.CardColor.Wildcard:
                    path += "wild";
                    break;
                case Card.CardColor.Purple:
                    switch (c.Face)
                    {
                        case Card.CardFace.LatexGlove:
                            path += "latexglove.bmp";
                            break;
                        case Card.CardFace.MedicalError:
                            path += "medicalerror.bmp";
                            break;
                        case Card.CardFace.OrganThief:
                            path += "organthief.bmp";
                            break;
                        case Card.CardFace.Overtime:
                            path += "overtime.bmp";
                            break;
                        case Card.CardFace.ProtectiveSuit:
                            path += "protectivesuit.bmp";
                            break;
                        case Card.CardFace.Quarantine:
                            path += "quarantine.bmp";
                            break;
                        case Card.CardFace.SecondOpinion:
                            path += "secondopinion.bmp";
                            break;
                        case Card.CardFace.Spreading:
                            path += "spreading.bmp";
                            break;
                        case Card.CardFace.Transplant:
                            path += "transplant.bmp";
                            break;
                        default:
                            path += "none.bmp";
                            break;
                    }
                    return path;
                default:
                    path += "none.bmp";
                    return path;
            }

            switch (c.Face)
            {
                case Card.CardFace.Organ:
                    path += "_organ.bmp";
                    break;
                case Card.CardFace.Virus:
                    path += "_virus.bmp";
                    break;
                case Card.CardFace.Medicine:
                    path += "_medicine.bmp";
                    break;
                case Card.CardFace.EvolvedMedicine:
                    path += "_experimental.bmp";
                    break;
                case Card.CardFace.EvolvedVirus:
                    path += "_evolvedvirus.bmp";
                    break;
                default:
                    path += "none.bmp";
                    return path;
            }
            return path;
        }

    }
}
