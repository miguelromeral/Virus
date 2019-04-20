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

        public GamePage(MainPage m)
        {
            this.InitializeComponent();
            Game = new Game(3, true);
            DataContext = Game;
            Main = m;
            
            Handler = new UserHandler(Game, Game.Players[0], this);

            foreach(var p in Game.Players)
            {
                Binding myBinding = new Binding("MyDataProperty")
                {
                    Source = Game
                };
                BindingOperations.SetBinding(myText, TextBlock.TextProperty, myBinding);

                StackPanel sp = new StackPanel
                {
                    Name = "sp" + p.ID
                };
                TextBlock tb = new TextBlock();
                tb.Text = p.ToString();

                sp.Children.Add(tb);
                GameContent.Children.Add(sp);
            }

            Handler.InitializeUserPanels();


            
            }

    }
}
