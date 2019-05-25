using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Virus.Core;
using Virus.Universal.Pages;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Virus.Universal.Classes
{
    public class UserHandler
    {
        public Game Game;
        public Player Me;
        public GamePage Page;
        public Dictionary<int, Button> Buttons;
        public Dictionary<int, CheckBox> Checkers;

        public UserHandler(Game g, Player m, GamePage p)
        {
            Game = g;
            Me = m;
            Page = p;
        }

        public void InitializeUserPanels()
        {
            StackPanel panelHands = new StackPanel();
            panelHands.Orientation = Orientation.Horizontal;
            Page.GameContent.Children.Add(panelHands);

            Checkers = new Dictionary<int, CheckBox>();
            Buttons = new Dictionary<int, Button>();

            for (int i = 0; i < Game.Players[0].Hand.Count; i++)
            {
                StackPanel spCard = new StackPanel()
                {
                    Orientation = Orientation.Vertical
                };

                //var outPutDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase);
                //var logoimage = Path.Combine(outPutDirectory, "Image\\Botiqun_500.jpg");

                //Image ci = new Image()
                //{
                //    Source = new BitmapImage(new Uri(logoimage))
                //};

                Card c = Game.Players[0].Hand[i];
                Button b = new Button()
                {
                    Name = "hand_" + i,
                    Content = c.ToString(),
                    Margin = new Thickness(20)
                };
                //b.Style = Resources["CardButton"] as Style;
                b.Click += Card_Clicked;

                Buttons.Add(i, b);

                CheckBox cb = new CheckBox()
                {
                    Name = "check_" + i
                };

                Checkers.Add(i, cb);

                //spCard.Children.Add(ci);
                spCard.Children.Add(b);
                spCard.Children.Add(cb);
                panelHands.Children.Add(spCard);
            }

            Button bd = new Button()
            {
                Name = "discardButton",
                Content = "Discard selected cards",
                Margin = new Thickness(20)
            };
            bd.Click += Discard_Selected_Cards;

            panelHands.Children.Add(bd);

        }





        public void Card_Clicked(object sender, RoutedEventArgs e)
        {
            ((Button)sender).Content = "Clicked!";

            //int index = -1;
            //foreach(KeyValuePair<int, Button> item in Buttons)
            //{
            //    if (item.Value.Equals((Button)sender){
            //        index = item.Key;
            //        break;
            //    }
            //}
            
            //if(index != -1)
            //{

            //}
        }

        public void Discard_Selected_Cards(object sender, RoutedEventArgs e)
        {
            ((Button)sender).Content = "Discarding!";

            List<Card> toDiscard = new List<Card>();

            foreach(KeyValuePair<int, CheckBox> item in Checkers)
            {
                if (item.Value.IsChecked != null && item.Value.IsChecked == true)
                {
                    toDiscard.Add(Me.Hand[item.Key]);
                }
            }

            foreach(Card c in toDiscard)
            {
                Game.MoveToDiscards(c);
            }

        }
    }
}
