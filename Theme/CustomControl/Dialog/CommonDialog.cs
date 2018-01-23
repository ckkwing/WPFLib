using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Theme.CustomControl.Dialog
{
    public class CommonDialog : BasicWindow
    {
        public CommonDialog()
            :base()
        {
            
        }

        public CommonDialog(Window owner)
            :base(owner)
        {
            
        }

        public enum ResultButtonType
        {
            OK,
            Cancel,
        }
    }
}
