using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Excel.DbTool
{
    public class SettingController:Controller
    {
        List<int> lstDb = System.Linq.Enumerable.Range(1, 10).ToList();
        public int DbCount(Microsoft.Office.Core.IRibbonControl control)
        {
            return this.lstDb.Count;
        }

        public string DbName(Microsoft.Office.Core.IRibbonControl control, int index)
        {
            return this.lstDb[index].ToString();
        }

        public void ChangeDB(Microsoft.Office.Core.IRibbonControl control, string selectedId, int selectedIndex)
        {
            System.Windows.Forms.MessageBox.Show("selectedId=" + selectedId);
            System.Windows.Forms.MessageBox.Show("selectedIndex=" + selectedIndex);
        }
    }
}
