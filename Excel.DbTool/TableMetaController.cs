using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Excel.DbTool
{
    public class TableMetaController:Controller
    {
        public void Show(Microsoft.Office.Core.IRibbonControl control)
        {
            System.Windows.Forms.MessageBox.Show(control.Id);
        }
    }
}
