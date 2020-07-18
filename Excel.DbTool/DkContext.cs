using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Excel.DbTool
{
    public class DkContext
    {
        public Microsoft.Office.Core.IRibbonControl Sender { set; get; }

        public int Index { get; set; }

        public string Text { get; set; }



    }
}
