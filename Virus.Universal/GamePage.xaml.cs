using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using System.Windows;
using Virus.Core;
using Windows.UI.Xaml.Media.Imaging;
using System.Reflection;
using Virus.Universal.Classes;
using System.Threading.Tasks;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Virus.Universal.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class GamePage : Page
    {
        private MainPage Main;
        private Game Game;
        private UserHandler Handler;
        public List<Panel> PlayerPanels;
        public Dictionary<int, List<Panel>> BIPanels;

        public GamePage(MainPage m)
        {
            this.InitializeComponent();
            Main = m;
            
            PlayerPanels = new List<Panel>();
            Game = new Game(3, 0, true);
            BIPanels = new Dictionary<int, List<Panel>>();
            
            Handler = new UserHandler(Game, Game.Players[0], this);

            foreach(var p in Game.Players)
            {
                //    Binding myBinding = new Binding()
                //    {
                //        Mode = BindingMode.OneWay,
                //        Source = GameWPF.Game;
                //    };


                StackPanel sp = new StackPanel
                {
                    Name = "sp" + p.ID,
                    Orientation = Orientation.Horizontal
                };
                TextBlock tb = new TextBlock()
                {
                    Text = p.ShortDescription
                };


                //BindingOperations.SetBinding(sp, TextBlock.TextProperty, myBinding);

                sp.Children.Add(tb);


                List<Panel> list = new List<Panel>();
                for (int i = 0; i < Game.Settings.NumberToWin; i++)
                {
                    StackPanel spl = new StackPanel() { Orientation = Orientation.Horizontal };

                    list.Add(spl);
                    sp.Children.Add(spl);
                }
                BIPanels.Add(p.ID, list);

                PlayerPanels.Add(sp);
                GameContent.Children.Add(sp);
            }

            Handler.InitializeUserPanels();

            //Task taskA = new Task(() => GameWPF.Start(2000));
            //// Start the task.
            //taskA.Start();

            Button b = new Button()
            {
                Content = "Pass Turn"
            };
            b.Click += Button_Click;

            GameContent.Children.Add(b);
        }

        private Panel GetPanelByPlayerPosition(int id, int item)
        {
            List<Panel> list = null;
            BIPanels.TryGetValue(id, out list);

            if (list == null)
                return null;
            if (item > list.Count || item < 0)
                return null;

            return list[item];
        }

        private void UpdatePlayerPanel(int id)
        {
            Player p = Game.Players[id];

            for(int i=0; i<p.Body.Items.Count; i++)
            {
                Panel panel = GetPanelByPlayerPosition(id, i);
                if (panel == null)
                    break;
                BodyItem item = p.Body.Items[i];

                panel.Children.Clear();

                //TextBlock tb = new TextBlock() { Text = item.ToString() };

                Image im = new Image()
                {
                    Source = new BitmapImage(new Uri(Directory.GetCurrentDirectory()+"/Images/corazn_500.jpg", UriKind.Absolute)),
                    Height = 100,
                    Width = 200
                };

                panel.Children.Add(im);

            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //if(Game.CurrentTurn == 0)
            //    Game.Turn++;

            Game.PlayTurn();
            Player p = Game.Players[Game.PreviousTurn];

            UpdatePlayerPanel(p.ID);
        }


    }
}
