using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Virus.Core;

namespace Virus.Forms
{
    [Serializable]
    public class CLogger : Logger
    {
        public TextBox TextBox;


        public CLogger(TextBox tb) : base()
        {
            TextBox = tb;
        }

        public override bool Write(string message, bool print = false)
        {
            bool res = base.Write(message, print);
            

                //if (print)
                //{
                if (TextBox != null)
                {
                    TextBox.AppendText(message + Environment.NewLine);

                    TextBox.SelectionStart = TextBox.Text.Length;
                    TextBox.SelectionLength = 0;
                    TextBox.ScrollToCaret();
                }
                //}

            
            return res;
        }
    }
}
