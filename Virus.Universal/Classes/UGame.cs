using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Virus.Core;

namespace Virus.Universal.Classes
{
    [Serializable]
    public class UGame : Game
    {
        public UGame(int waitingtime, bool firstHuman = false)
            : base(waitingtime, firstHuman)
        {
        }
        

        public override bool ProceedProtectiveSuit(Player player, Player rival, Card myCard, string move, List<string> wholemoves)
        {
            // TODO
            return false;
        }
    }
}
