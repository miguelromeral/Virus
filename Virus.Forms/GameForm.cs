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
            
            foreach (var p in Game.Players)
            {
                FlowLayoutPanel flp = new FlowLayoutPanel()
                {
                    Name = "panel_user_" + p.ID,
                    FlowDirection = FlowDirection.LeftToRight,
                    AutoScroll = true
                };
                PlayerPanels.Add(flp);

                Label tb = new Label()
                {
                    Text = p.ShortDescription
                };

                PlayerPanels.Add(flp);

                List<Panel> bil = new List<Panel>();
                for (int i = 0; i < Game.Settings.NumberToWin; i++)
                {
                    FlowLayoutPanel pi = new FlowLayoutPanel();
                    pi.BorderStyle = BorderStyle.FixedSingle;
                    pi.BackColor = Color.AliceBlue;
                    flp.Controls.Add(pi);
                    bil.Add(pi);
                }
                BodyItemPanels.Add(p.ID, bil);
                MainLayout.Controls.Add(tb);
                MainLayout.Controls.Add(flp);


            }
            UpdateUserHand();
        }


        private Dictionary<int, List<Panel>> BodyItemPanels = new Dictionary<int, List<Panel>>();

        private List<Panel> PlayerPanels = new List<Panel>();


        private void UpdateUserHand()
        {
            foreach (var p in Game.Players)
            {
                if (p.AI != ArtificialIntelligence.AICategory.Human)
                    break;

                foreach (Card c in p.Hand)
                {
                    FlowLayoutPanel pc = new FlowLayoutPanel();
                    
                    pc.Controls.Add(CreateButtonCheckBoxCard(c, 75, 100));
                    pUserHand.Controls.Add(pc);
                }
            }
        }


        private void button1_Click(object sender, EventArgs e)
        {
            Game.PlayTurn();
            UpdateAllPlayerPanels();
            UpdateGamePanel();
        }

        private void UpdateAllPlayerPanels()
        {
            foreach (var p in Game.Players)
                UpdatePlayerPanel(p.ID);
        }

        private CheckBox CreateButtonCheckBoxCard(Card c, int width, int height)
        {
            Image myimage = new Bitmap(GetImageFromCard(c));
            CheckBox b = new CheckBox()
            {
                //Text = c.ToString(),
                BackgroundImage = myimage,
                Width = width,
                Height = height,
                Appearance = Appearance.Button
            };

            b.BackgroundImage = new Bitmap(b.BackgroundImage, b.Width, b.Height);
            return b;
        }

        private void UpdatePlayerPanel(int id)
        {
            Player p = Game.Players[id];

            for (int i = 0; i < p.Body.Items.Count; i++)
            {
                Panel panel = PlayerPanels[id];

                if (panel == null)
                    break;

                BodyItem item = p.Body.Items[i];

                panel.Controls.Clear();
                

                panel.Controls.Add(CreateButtonCheckBoxCard(item.Organ, 50, 100));

                for (int j = 0; j < item.Modifiers.Count; j++)
                {
                    panel.Controls.Add(CreateButtonCheckBoxCard(item.Modifiers[j], 25, 37));
                }

            }
        }

        private void UpdateGamePanel()
        {
            lTurns.Text = "Turn #" + Game.Turn;
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

    }
}
