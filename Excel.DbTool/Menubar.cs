using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Office.Tools.Ribbon;

namespace Excel.DbTool
{
    public enum HeadTexts
    {
        名称=1,
        类型=2,
        可空=3,
        备注=4,
        重命名=5
    }
    public partial class Menubar
    {
        const string ALL = "全部";

        IDbManager dbmanager;

        int pageIndex = 1;
        int pageSize = 27;
        int pageCount = 0;

        int maxPageCount = 6;

        List<string> lstTableName;
        private void Menubar_Load(object sender, RibbonUIEventArgs e)
        {
            this.InitDrpDBSource(null, null);
            this.btnDBSourceSetting.Click += btnDBSourceSetting_Click;
            this.drpDBlist.SelectionChanged += drpDBlist_SelectionChanged;
            this.drpTablelist.SelectionChanged += drpTablelist_SelectionChanged;
            this.btnTableSchemal.Click += DisplayTableMeta;
            this.btnTableSave.Click += SaveTableMeta;
            Globals.ThisAddIn.Application.SheetBeforeDoubleClick += Application_SheetBeforeDoubleClick;
            this.btnLast.Click += this.BtnLast_Click;
            this.btnNext.Click += this.BtnNext_Click;
        }
        private void BtnNext_Click(object sender, RibbonControlEventArgs e)
        {
            if (pageIndex >= this.pageCount)
                return;
            pageIndex += 1;
            PageHandler();
        }

        private void BtnLast_Click(object sender, RibbonControlEventArgs e)
        {
            if (pageIndex <= 1)
                return;
            pageIndex -= 1;
            PageHandler();
        }
        private void Application_SheetBeforeDoubleClick(object Sh, Microsoft.Office.Interop.Excel.Range target, ref bool Cancel)
        {
            var lstbutton= this.tableCollection.Items.Cast<RibbonCheckBox>().Where(x => x.Checked).ToList();
            if (lstbutton.Count<1)
                return;

            if (!CheckDrpSelectedValue(drpDBlist, drpTablelist))
                return;
            Cancel = true;
            string cnnstr = ((Tuple<string, string>)drpDBlist.SelectedItem.Tag).Item2;
            lstbutton.ForEach(x=> {
                x.Checked = false;
                target= this.DisplayTableMeta(x.Label, target);
            });
            //string tableName = lstbutton.Label;
            //this.DisplayTableMeta(tableName, Target);
            //lstbutton.Checked = false;
        }

        private void FocusButton(object sender, RibbonControlEventArgs e)
        {
            //RibbonButton button = sender as RibbonButton;
            //var lst= this.tableCollection.Items.Cast<RibbonButton>().Except(new List<RibbonButton>() { button}).ToList();
            //lst.ForEach(x=>x.ShowImage=false);
            //button.ShowImage = !button.ShowImage;
        }
        /// <summary>
        /// 初始化数据库下拉列表
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
        private void PageHandler()
        {
            var lstbutton = this.tableCollection.Items.Cast<RibbonCheckBox>().ToList();
            lstbutton.ForEach(x => x.Visible = false);
            for (int i = pageIndex * pageSize - pageSize; i < pageIndex * pageSize; i++)
            {
                lstbutton[i].Label = lstTableName[i];
                lstbutton[i].Visible = true;

            }
            //var lstTableNameTmp = lstTableName.Skip(this.pageIndex * pageSize - pageSize).Take(pageSize).ToList();
            //for (int i = 0; i < lstTableNameTmp.Count; i++)
            //{
            //    lstbutton[i].Label = lstTableNameTmp[i];
            //    lstbutton[i].Visible = true;
            //}
            this.tableCollection.Label = string.Format("第{0}/{1}页，共{2}个表", pageIndex, pageCount,lstTableName.Count);
        }
        /// <summary>
        /// 切换表格下拉列表的响应方法
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void drpTablelist_SelectionChanged(object sender, RibbonControlEventArgs e)
        {
            if (this.drpTablelist.SelectedItem == null)
                return;
            var range= Globals.ThisAddIn.Application.Cells.Find(this.drpTablelist.SelectedItem.Label,MatchByte:true);
            if (range != null)
                range.Activate();
        }

        /// <summary>
        /// 切换数据库下拉列表的响应方法
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void drpDBlist_SelectionChanged(object sender, RibbonControlEventArgs e)
        {
            this.drpTablelist.Items.Clear();
            this.drpTablelist.Label = string.Empty;
            var tmp = this.drpDBlist.SelectedItem.Tag as Tuple<string, string>;
            DbIndex dbIndex;
            if (tmp == null || !System.Enum.TryParse<DbIndex>(tmp.Item1, out dbIndex))
                return;
            var dbHandlerMeta = typeof(IDbManager).FullName;
            var lstDBHelper = System.Reflection.Assembly.GetExecutingAssembly().GetTypes().Where(x => x.GetInterface(dbHandlerMeta)!=null)
                .Select(x => (IDbManager)System.Activator.CreateInstance(x)).ToList();
            this.dbmanager = lstDBHelper.FirstOrDefault(x => x.DbIndex == dbIndex);
            if (this.dbmanager == null)
                throw new ArgumentException("不支持指定的数据库");
            this.dbmanager.SetDbConnectionString(tmp.Item2);
            lstTableName = this.dbmanager.GetAllTableName();
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
            this.drpTablelist.SelectedItemIndex = 0;

            this.pageCount = (int)Math.Ceiling((1.0) * lstTableName.Count / this.pageSize);
            if (this.pageCount > this.maxPageCount)
                this.pageCount = this.maxPageCount;
            this.pageIndex = 1;
            PageHandler();

        }

        /// <summary>
        /// 设置数据源
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnDBSourceSetting_Click(object sender, RibbonControlEventArgs e)
        {
            Views.DBSourceSetting window = new Views.DBSourceSetting();
            window.FormClosed += InitDrpDBSource;
            window.ShowDialog();
        }

        /// <summary>
        /// 保存设计
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void SaveTableMeta(object sender, RibbonControlEventArgs e)
        {
            if (!CheckDrpSelectedValue(drpDBlist, drpTablelist))
                return;
            Microsoft.Office.Interop.Excel.Range range = Globals.ThisAddIn.Application.Selection;
            string cnnstr=((Tuple<string, string>)drpDBlist.SelectedItem.Tag).Item2;
            List<string> lstTableName = drpTablelist.Items.Select(x => ((object)(x.Tag)).ToString()).ToList();
            if (range.Rows.Count <= 2)
                throw new ArgumentException("请选中需要处理的表的区域");
            object[,] rangeValue = range.Value2;
            //这里把数组拆成多个数组
            //List<object> lstarry = new List<object>();
            //var arraytmp= System.Array.CreateInstance(typeof(object), new int[] { rangeValue.GetUpperBound(0), rangeValue.GetUpperBound(1) }, new int[] { 1, 1 });
            //for (int i = 0; i <= rangeValue.GetUpperBound(0); i++)
            //{

            //}
             





            TableMeta meta = new TableMeta();
            meta.Name= rangeValue[1, (int)HeadTexts.名称] as string;
            meta.Description = rangeValue[1, (int)HeadTexts.备注] as string;
            if (string.IsNullOrEmpty(meta.Name))
                throw new ArgumentException("必须填写表名称");
            var headNames= System.Enum.GetNames(typeof(HeadTexts)).Cast<string>().ToArray();
            if (rangeValue.GetUpperBound(1) != headNames.Length && rangeValue.GetUpperBound(1) != headNames.Length - 1)//因为 range.Value2里面的数组从1，1开始，+1的情况表示修改名称
                throw new ArgumentException("选择区域的数据与模版格式不一致！");
            var heardTextTmp = System.Linq.Enumerable.Range(1, rangeValue.GetUpperBound(1)).Select(x => rangeValue.GetValue(2, x) as string).ToArray();
            if (heardTextTmp.Except(headNames).Count()>0)
                throw new ArgumentException("选择区域的数据与模版格式不一致！");
            for (int i = 3; i <= rangeValue.GetUpperBound(0); i++)
            {
                ColumnMeta column = new ColumnMeta();
                column.ColumnName = rangeValue[i, (int)HeadTexts.名称] as string;
                column.DataTypeName = rangeValue[i, (int)HeadTexts.类型] as string;
                //column.ColumnSize = rangeValue[i, (int)HeadTexts.长度] ==null?double.MinValue:(double)rangeValue[i, (int)HeadTexts.长度];
                column.AllowDBNull = "是".Equals(rangeValue[i, (int)HeadTexts.可空])? true:false;
                column.Description = rangeValue[i, (int)HeadTexts.备注] as string;
                meta.Columns.Add(column);
            }
            if (!lstTableName.Contains(meta.Name))
            {
                this.dbmanager.TableAdd(meta);
                this.drpDBlist_SelectionChanged(null, null);
                this.drpTablelist.SelectedItem = this.drpTablelist.Items.First(x => x.Label==meta.Name);
                System.Windows.Forms.MessageBox.Show("添加成功");
            }
            else if (rangeValue.GetUpperBound(1) == headNames.Length-1)
            {
                this.dbmanager.TableDesigner(meta);
                System.Windows.Forms.MessageBox.Show("修改成功");
                
            }
            else if (rangeValue.GetUpperBound(1) == headNames.Length)
            {
                Dictionary<string, string> dict = new Dictionary<string, string>();
                meta.ReName = rangeValue[1, (int)HeadTexts.重命名] as string;
                for (int i = 3; i <= rangeValue.GetUpperBound(0); i++)
                { 
                    dict.Add(rangeValue[i,(int)HeadTexts.名称] as string,rangeValue[i,(int)HeadTexts.重命名] as string);
                }
                this.dbmanager.TableDesignerReName(meta, dict);
                this.drpDBlist_SelectionChanged(null, null);
                meta.ReName = meta.ReName ?? meta.Name;
                this.drpTablelist.SelectedItem = this.drpTablelist.Items.First(x => x.Label== meta.ReName);
                System.Windows.Forms.MessageBox.Show("重命名成功");
            }
        }

        /// <summary>
        /// 查看设计
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void DisplayTableMeta(object sender, RibbonControlEventArgs e)
        {
            if (!CheckDrpSelectedValue(drpDBlist, drpTablelist))
                return;
            string cnnstr = ((Tuple<string, string>)drpDBlist.SelectedItem.Tag).Item2;
            string tableName = this.drpTablelist.SelectedItem.Tag.ToString();
            var range = Globals.ThisAddIn.Application.ActiveCell;
            if (!ALL.Equals(tableName))
            {
                this.DisplayTableMeta(tableName, range);
                return;
            }
            Views.ProcessDashboard pd = new Views.ProcessDashboard();
            pd.TableMetaHandler = () =>
            {
                foreach (var item in this.drpTablelist.Items)
                {
                    if (ALL.Equals(item.Label))
                        continue;
                    range = this.DisplayTableMeta(item.Label, range);
                }
            };
            pd.ShowDialog();
        }

        Microsoft.Office.Interop.Excel.Range DisplayTableMeta(string tableName, Microsoft.Office.Interop.Excel.Range cell)
        {
            var tableMeta=  this.dbmanager.GetTableMeta(tableName);
            int rowIndex=0;
            cell.UnMerge();
            cell.Offset[rowIndex, 0].Value2 = tableMeta.Name;
            var headValues= System.Enum.GetValues(typeof(HeadTexts)).Cast<int>().ToArray();
            cell.Offset[rowIndex++, headValues.Length - 2].Value2 = tableMeta.Description;
            foreach (var index in headValues)
            { 
                cell.Offset[rowIndex, index-1].Value2 = ((HeadTexts)index).ToString();
            }
            foreach (var column in tableMeta.Columns)
            {
                rowIndex++;
                cell.Offset[rowIndex, (int)HeadTexts.名称-1].Value2 = column.ColumnName;
                cell.Offset[rowIndex, (int)HeadTexts.类型 - 1].Value2 = column.DataTypeName;
                //cell.Offset[rowIndex, (int)HeadTexts.长度 - 1].Value2 = column.DataTypeName.Contains("char") ? column.ColumnSize.ToString(): string.Empty;
                cell.Offset[rowIndex, (int)HeadTexts.可空 - 1].Value2 = column.AllowDBNull ? "是" : string.Empty;
                cell.Offset[rowIndex, (int)HeadTexts.备注 - 1].Value2 = column.Description;
            }
            //处理样式
            var firstCell = cell.Offset[1, 0];
            var lastCell = cell.Offset[rowIndex, headValues.Length - 1];
            var range = Globals.ThisAddIn.Application.get_Range(cell, cell.Offset[0, headValues.Length-3]);
            //range.Merge();
            range.HorizontalAlignment = Microsoft.Office.Interop.Excel.Constants.xlLeft;
            //range.Select();
            range=  Globals.ThisAddIn.Application.get_Range(firstCell,lastCell);
            range.Columns.AutoFit();
            //range = Globals.ThisAddIn.Application.get_Range(cell,lastCell);
            System.Enum.GetValues(typeof(Microsoft.Office.Interop.Excel.XlBordersIndex)).Cast<Microsoft.Office.Interop.Excel.XlBordersIndex>()
                .Where(x => x != Microsoft.Office.Interop.Excel.XlBordersIndex.xlDiagonalDown && x != Microsoft.Office.Interop.Excel.XlBordersIndex.xlDiagonalUp)
                .ToList().ForEach(x => range.Borders[x].LineStyle = Microsoft.Office.Interop.Excel.XlLineStyle.xlContinuous);
            firstCell.Select();
            return cell.Offset[rowIndex + 2, 0];
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
