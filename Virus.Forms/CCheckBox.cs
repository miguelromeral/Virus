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
    }
}
