using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Office.Tools.Ribbon;

namespace Excel.Extension
{
    public partial class ToolBar
    {
        
        string[] HeardTexts=new string[5]{"名称","类型","长度","可空","备注"};
        const string ALL = "全部";
        const string TableRemarkColName = "_TableName_";

        IDBHelper dbHelper;
        private void InitToolBar(object sender, RibbonUIEventArgs e)
        {
            
            
            this.InitDrpDBSource(null,null);
            this.drpDBlist.SelectionChanged += drpDBlist_SelectionChanged;
            this.drpTablelist.SelectionChanged += drpTablelist_SelectionChanged;
            this.btnDBSourceSetting.Click+=btnDBSourceSetting_Click;
            this.btnTableSchemal.Click += btnTableSchemal_Click;
            this.btnTableSave.Click += btnTableSave_Click;
            this.btnVisioSchemal.Click += BtnVisioSchemal_Click;
        }

        MouseHookWrapper hookWrapper;
        private void BtnVisioSchemal_Click(object sender, RibbonControlEventArgs e)
        {
            if(hookWrapper==null)
            {
                this.hookWrapper = new MouseHookWrapper();
                hookWrapper.Hook(OnMouseWheel, HookLevel.WH_MOUSE);
            }
            else
            {
                this.hookWrapper.UnHook();
                this.hookWrapper = null;
            }
            var tmp = this.btnVisioSchemal.Label;
            this.btnVisioSchemal.Label = this.btnVisioSchemal.Tag.ToString();
            this.btnVisioSchemal.Tag = tmp;
        }

        Microsoft.VisualBasic.Devices.Keyboard keyBoard = new Microsoft.VisualBasic.Devices.Keyboard();
        private void OnMouseWheel(int nCode, IntPtr wParam, IntPtr lParam)
        {
            MouseButtonAction at = (MouseButtonAction)(int)wParam;
            if (at != MouseButtonAction.WM_Wheel ||!keyBoard.ShiftKeyDown)
                return;
            MouseHookWrapper.LParamStruct rst =
                (MouseHookWrapper.LParamStruct)System.Runtime.InteropServices.Marshal.PtrToStructure(lParam,typeof(MouseHookWrapper.LParamStruct));
            Globals.ThisAddIn.Application.ActiveWindow.SmallScroll(ToRight: keyBoard.CtrlKeyDown?-1:1);

        }

        void InitDrpDBSource(object sender, System.Windows.Forms.FormClosedEventArgs e)
        {
            this.drpDBlist.Items.Clear();
            var tmp = this.Factory.CreateRibbonDropDownItem();
            tmp.Label = "请选择数据库";
            tmp.Tag = string.Empty;
            this.drpDBlist.Items.Add(tmp);
            var lstDB = System.Configuration.ConfigurationManager.OpenExeConfiguration(System.Configuration.ConfigurationUserLevel.None).ConnectionStrings
                .ConnectionStrings.Cast<System.Configuration.ConnectionStringSettings>().Where(x => x.Name != "LocalSqlServer").ToList();
            foreach (var db in lstDB)
            {
                tmp = this.Factory.CreateRibbonDropDownItem();
                tmp.Label = db.Name;
                tmp.Tag = Tuple.Create(db.ProviderName, db.ConnectionString);
                this.drpDBlist.Items.Add(tmp);
            }
        }

        void drpTablelist_SelectionChanged(object sender, RibbonControlEventArgs e)
        {
            if (this.drpTablelist.SelectedItem == null)
                return;
            var range= Globals.ThisAddIn.Application.Cells.Find(this.drpTablelist.SelectedItem.Label,MatchByte:true);
            if (range != null)
                range.Activate();
        }

        void drpDBlist_SelectionChanged(object sender, RibbonControlEventArgs e)
        {
            this.drpTablelist.Items.Clear();
            this.drpTablelist.Label = string.Empty;
            var tmp = this.drpDBlist.SelectedItem.Tag as Tuple<string, string>;
            DBProvider provider;
            if (tmp == null || !System.Enum.TryParse<DBProvider>(tmp.Item1, out provider))
                return;
            var nameTmp = typeof(IDBHelper).Name;
            var lstDBHelper = System.Reflection.Assembly.GetExecutingAssembly().GetTypes().Where(x => x.GetInterfaces().Count(a => a.Name.Equals(nameTmp)) > 0)
                .Select(x => (IDBHelper)System.Activator.CreateInstance(x)).ToList();
            this.dbHelper = lstDBHelper.FirstOrDefault(x => x.GetProvider() == provider);
            if (this.dbHelper == null)
                throw new ArgumentException("不支持指定的数据库");
            List<string> lstTableName = this.dbHelper.InitDrpTableListByMssql(tmp.Item2);
            var drpItem = this.Factory.CreateRibbonDropDownItem();
            drpItem.Label = ALL;
            drpItem.Tag = ALL;
            this.drpTablelist.Items.Add(drpItem);
            foreach (var tname in lstTableName)
            {
                drpItem = this.Factory.CreateRibbonDropDownItem();
                drpItem.Label = tname;
                drpItem.Tag = tname;
                this.drpTablelist.Items.Add(drpItem);
            }
            this.drpTablelist.SelectedItemIndex = this.drpTablelist.Items.Count - 1;
        }

        void btnDBSourceSetting_Click(object sender, RibbonControlEventArgs e)
        {
            FrmDBSourceSetting window = new FrmDBSourceSetting();
            window.FormClosed += InitDrpDBSource;
            window.ShowDialog();
        }

        void btnTableSave_Click(object sender, RibbonControlEventArgs e)
        {
            if (!CheckDrpSelectedValue(drpDBlist, drpTablelist))
                return;
            var range = Globals.ThisAddIn.Application.Selection;
            string cnnstr=((Tuple<string, string>)drpDBlist.SelectedItem.Tag).Item2;
            List<string> lstTableName = drpTablelist.Items.Select(x => x.Tag.ToString()).ToList();
            if (range.Rows.Count <= 2)
                return;
            object[,] value = range.Value2;
            string tableName = value[1, 1] as string;
            if (string.IsNullOrEmpty(tableName))
                throw new ArgumentException("必须填写表名称");
            if (value.GetUpperBound(1) != HeardTexts.Length && value.GetUpperBound(1) != HeardTexts.Length + 1)//因为 range.Value2里面的数组从1，1开始，+1的情况表示修改名称
                throw new ArgumentException("选择区域的数据与模版格式不一致！");
            var heardTextTmp = System.Linq.Enumerable.Range(1, HeardTexts.Length).Select(x => value.GetValue(2, x)).ToList();
            if (this.HeardTexts.Where((x, i) => x.Equals(heardTextTmp[i])).Count() != this.HeardTexts.Length)
                throw new ArgumentException("选择区域的数据与模版格式不一致！");
            if (!lstTableName.Contains(tableName))
            {
                this.dbHelper.AddTableMetaByMssqlWithDDL(cnnstr, tableName, value);
                this.drpDBlist_SelectionChanged(null, null);
                this.drpTablelist.SelectedItem = this.drpTablelist.Items.First(x => x.Label.Equals(value[1, 1]));
                System.Windows.Forms.MessageBox.Show("添加成功");
            }
            else if (value.GetUpperBound(1) == HeardTexts.Length)
            {
                this.dbHelper.EditTableMetaByMssqlWithDDL(cnnstr, tableName, value);
                System.Windows.Forms.MessageBox.Show("修改成功");
            }
            else if (value.GetUpperBound(1) == HeardTexts.Length + 1)
            {
                this.dbHelper.EditTableMetaByMssqlWithReName(cnnstr, tableName, value);
                this.drpDBlist_SelectionChanged(null, null);
                this.drpTablelist.SelectedItem = this.drpTablelist.Items.First(x => x.Label.Equals(value[1, 6]));
                System.Windows.Forms.MessageBox.Show("重命名成功");
            }
        }
        void btnTableSchemal_Click(object sender, RibbonControlEventArgs e)
        {
            if (!CheckDrpSelectedValue(drpDBlist, drpTablelist))
                return;
            string cnnstr = ((Tuple<string, string>)drpDBlist.SelectedItem.Tag).Item2;
            string tableName = this.drpTablelist.SelectedItem.Tag.ToString();
            var range = Globals.ThisAddIn.Application.ActiveCell;
            if (!ALL.Equals(tableName))
            {
                this.GetTableSchemalByMssqlInternal(cnnstr, tableName, range);
                return;
            }
            foreach (var item in this.drpTablelist.Items)
            {
                if (ALL.Equals(item.Label))
                    continue;
                range = this.GetTableSchemalByMssqlInternal(cnnstr, item.Label, range);
            }
        }

        private Microsoft.Office.Interop.Excel.Range GetTableSchemalByMssqlInternal(string cnnstr, string tableName, Microsoft.Office.Interop.Excel.Range cell)
        {
            
            System.Data.DataTable tableRemark;
             System.Data.DataTable table;
             this.dbHelper.GetTableSchemalByMssqlInternal(cnnstr, tableName, out  table, out  tableRemark);
            int colIndex = 0;
            int rowIndex = 0;
            cell.UnMerge();
            cell.Offset[rowIndex++, colIndex].Value2 = tableName;
            cell.Offset[rowIndex, colIndex++].Value2 = HeardTexts[colIndex-1];
            cell.Offset[rowIndex, colIndex++].Value2 = HeardTexts[colIndex-1];
            cell.Offset[rowIndex, colIndex++].Value2 = HeardTexts[colIndex-1];
            cell.Offset[rowIndex, colIndex++].Value2 = HeardTexts[colIndex-1];
            cell.Offset[rowIndex, colIndex++].Value2 = HeardTexts[colIndex-1];
            var tmp = tableRemark.Select().FirstOrDefault(x =>TableRemarkColName.Equals( x["name"]));//借用colIndex处理表的备注
            cell.Offset[rowIndex - 1, colIndex - 1].Value2 = tmp == null ? string.Empty : tmp["value"];
            var metaRows = table.Select().OrderBy(x => x["ColumnName"].ToString()).ToList();
            var remarkRows = tableRemark.Select();
            foreach (var row in metaRows)
            {
                rowIndex++;
                colIndex = 0;
                cell.Offset[rowIndex, colIndex++].Value2 = row["ColumnName"];
                cell.Offset[rowIndex, colIndex++].Value2 = row["DataTypeName"];
                cell.Offset[rowIndex, colIndex++].Value2 = row["DataTypeName"].ToString().Contains("char")?row["ColumnSize"]:string.Empty;
                cell.Offset[rowIndex, colIndex++].Value2 = (bool)row["AllowDBNull"]?"是":string.Empty;
                tmp = remarkRows.FirstOrDefault(x => row["ColumnName"].ToString().Equals(x["name"]));
                 cell.Offset[rowIndex, colIndex++].Value2 = tmp == null ? string.Empty : tmp["value"];
            }
            var firstCell = cell.Offset[1, 0];
            var lastCell = cell.Offset[rowIndex, colIndex - 1];
            var nextCell = cell.Offset[rowIndex + 2, 0];
            var range = Globals.ThisAddIn.Application.get_Range(cell, cell.Offset[0, HeardTexts.Length-2]);
            range.Merge();
            range.HorizontalAlignment = Microsoft.Office.Interop.Excel.Constants.xlLeft;
            range.Select();

           range=  Globals.ThisAddIn.Application.get_Range(firstCell,lastCell);
            range.Columns.AutoFit();
            range = Globals.ThisAddIn.Application.get_Range(cell,lastCell);
            System.Enum.GetValues(typeof(Microsoft.Office.Interop.Excel.XlBordersIndex)).Cast<Microsoft.Office.Interop.Excel.XlBordersIndex>()
                .Where(x => x != Microsoft.Office.Interop.Excel.XlBordersIndex.xlDiagonalDown && x != Microsoft.Office.Interop.Excel.XlBordersIndex.xlDiagonalUp)
                .ToList().ForEach(x => range.Borders[x].LineStyle = Microsoft.Office.Interop.Excel.XlLineStyle.xlContinuous);
            return nextCell;
        }

        private bool CheckDrpSelectedValue(params RibbonDropDown[] drps)
        {
            foreach (var item in drps)
            {
                
                if (item.SelectedItem == null || item.SelectedItem.Tag == null)
                    return false;
            }
            return true;
        }

    }
}
