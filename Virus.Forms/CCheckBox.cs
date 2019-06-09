using System;
using System.Collections.Generic;
using System.Drawing;
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
        
        public Image CardImage { get; set; }
        public double Percentage { get; set; }

        public bool InHand { get; set; }


        public void SetTransparency(object sender, EventArgs e)
        {
            CCheckBox cb = (CCheckBox)sender;

            if (cb.Checked)
            {
                cb.BackColor = Color.Transparent;
                cb.BackgroundImage = FormUtilities.SetImageOpacity(CardImage, Percentage, 0.5F);
            }
            else
            {
                cb.BackgroundImage = FormUtilities.SetImageOpacity(CardImage, Percentage, 1F);
            }
        }

        public string ToString()
        {
            return String.Format("C:{0} | P:{1} | I:{2} | Hand:{3}", Card.ToString(), PlayerId, Index, InHand);
        }
    }
}
