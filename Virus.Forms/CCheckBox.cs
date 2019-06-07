using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Virus.Core;

namespace Virus.Forms
{
    public class CCheckBox : CheckBox
    {
        public Card Card { get; set; }
        public int PlayerId { get; set; }
        public int Index { get; set; }
        
        public bool InHand { get; set; }


        public string ToString()
        {
            return String.Format("C:{0} | P:{1} | I:{2} | Hand:{3}", Card.ToString(), PlayerId, Index, InHand);
        }
    }
}
