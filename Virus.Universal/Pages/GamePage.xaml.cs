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
        private GameWPF GameWPF;
        private UserHandler Handler;

        public GamePage(MainPage m)
        {
            this.InitializeComponent();
            GameWPF = new GameWPF(3);
            DataContext = GameWPF;
            Main = m;
            
            Handler = new UserHandler(GameWPF.Game, GameWPF.Game.Players[0], this);

            foreach(var p in GameWPF.Game.Players)
            {
            //    Binding myBinding = new Binding()
            //    {
            //        Mode = BindingMode.OneWay,
            //        Source = GameWPF.Game;
            //    };


                StackPanel sp = new StackPanel
                {
                    Name = "sp" + p.ID
                };
                TextBlock tb = new TextBlock();
                tb.Text = p.ToString();

                

                //BindingOperations.SetBinding(sp, TextBlock.TextProperty, myBinding);

                sp.Children.Add(tb);
                GameContent.Children.Add(sp);
            }

            Handler.InitializeUserPanels();

            Task taskA = new Task(() => GameWPF.Start(2000));
            // Start the task.
            taskA.Start();


            
            
            }

    }
}
