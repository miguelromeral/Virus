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
        public Game Game;

        public GameForm()
        {
            InitializeComponent();
            InitGame();
        }

        private void InitGame()
        {
            Game = new Game(3, 5000, true);

            InitPanels();
            UpdateUserHand();
        }


        private Dictionary<int, List<Panel>> BodyItemPanels = new Dictionary<int, List<Panel>>();

        private List<Panel> PlayerPanels = new List<Panel>();

        private List<Panel> UserHandPanels = new List<Panel>();
        private List<CCheckBox> UserHandCards = new List<CCheckBox>();



        private void InitPanels()
        {
            foreach (var p in Game.Players)
            {
                Label label = new Label()
                {
                    Text = p.ShortDescription
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
                    FlowLayoutPanel pItem = new FlowLayoutPanel() {
                        FlowDirection = FlowDirection.LeftToRight,
                        AutoScroll = false,
                        AutoSize = true,
                        AutoSizeMode = AutoSizeMode.GrowAndShrink,
                    };
                    bil.Add(pItem);
                    
                    pBody.Controls.Add(pItem);
                }
                BodyItemPanels.Add(p.ID, bil);

                MainLayout.Controls.Add(label);
                MainLayout.Controls.Add(pBody);
            }
            for(int i=0; i<Game.Settings.NumberCardInHand; i++)
            {
                FlowLayoutPanel pHand = new FlowLayoutPanel()
                {
                    FlowDirection = FlowDirection.LeftToRight,
                    AutoScroll = true,
                    AutoSize = true,
                    AutoSizeMode = AutoSizeMode.GrowAndShrink,
                };
                UserHandPanels.Add(pHand);
                pUserHand.Controls.Add(pHand);
            }
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
                for (i=0; i<p.Hand.Count; i++)
                {
                    Card c = p.Hand[i];
                    pc = UserHandPanels[i];

                    var cb = CreateButtonCheckBoxCard(c, 75, 100, p.ID, i);
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
        }


        private void button1_Click(object sender, EventArgs e)
        {
            Game.PlayTurn();
            UpdateGUI();
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
            if(list == null)
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

                pItem.Controls.Add(CreateButtonCheckBoxCard(item.Organ, 75, 100, id, i));

                for (int j = 0; j < item.Modifiers.Count; j++)
                {
                    pItem.Controls.Add(CreateButtonCheckBoxCard(item.Modifiers[j], 25, 37, id, i));
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
            lTurns.Text = "Turn #" + Game.Turn;
            tbGame.Text = Game.ToString();
        }

        private void UpdateGUI()
        {
            UpdateAllPlayerPanels();
            UpdateGamePanel();
            UpdateUserHand();
        }

        private CCheckBox CreateButtonCheckBoxCard(Card c, int width, int height, int playerid, int index)
        {
            Image myimage = new Bitmap(GetImageFromCard(c));
            CCheckBox b = new CCheckBox()
            {
                //Text = c.ToString(),
                Card = c,
                PlayerId = playerid,
                Index = index,
                BackgroundImage = myimage,
                Width = width,
                Height = height,
                Appearance = Appearance.Button
            };

            b.BackgroundImage = new Bitmap(b.BackgroundImage, b.Width, b.Height);
            return b;
        }

        public Image GetImageFromCard(Card c)
        {
            return new Bitmap(GetImageFromCardString(c));
        }

        public string GetImageFromCardString(Card c)
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

        private void bDiscard_Click(object sender, EventArgs e)
        {
            bool discarded = false;
            Player p = null;
            foreach(var cb in UserHandCards)
            {
                if (cb.Checked)
                {
                    if (p == null)
                        p = Game.GetPlayerByID(cb.PlayerId);

                    Game.DiscardFromHand(p, cb.Card);
                    discarded = true;
                }
            }
            if (discarded)
            {
                Game.DrawCardsToFill(p);
                Game.Turn++;
            }
            UpdateGUI();

        }
    }
}
