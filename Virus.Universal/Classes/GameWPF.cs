using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Virus.Core;

namespace Virus.Universal.Classes
{
    public class GameWPF : INotifyPropertyChanged
    {
        public Game Game;

        public GameWPF(int numPlayers, bool firstHuman = false)
        {
            Game = new Game(numPlayers, firstHuman);
        }


        public void Start(int milis = 0)
        {
            while (!Game.GameOver)
            {
                Game.PlayTurn(milis == 0, true);
                if (milis != 0)
                {
                    System.Threading.Thread.Sleep(milis);
                }
                OnPropertyChanged("Game");
            }
        }



        public event PropertyChangedEventHandler PropertyChanged;


        // Create the OnPropertyChanged method to raise the event
        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }
    }
}
