using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using Office = Microsoft.Office.Core;

// TODO:   按照以下步骤启用功能区(XML)项: 

// 1. 将以下代码块复制到 ThisAddin、ThisWorkbook 或 ThisDocument 类中。

//  protected override Microsoft.Office.Core.IRibbonExtensibility CreateRibbonExtensibilityObject()
//  {
//      return new Ribbon();
//  }

// 2. 在此类的“功能区回调”区域中创建回调方法，以处理用户
//    操作(如单击某个按钮)。注意: 如果已经从功能区设计器中导出此功能区，
//    则将事件处理程序中的代码移动到回调方法并修改该代码以用于
//    功能区扩展性(RibbonX)编程模型。

// 3. 向功能区 XML 文件中的控制标记分配特性，以标识代码中的相应回调方法。  

// 有关详细信息，请参见 Visual Studio Tools for Office 帮助中的功能区 XML 文档。


namespace Excel.DbTool
{
    [ComVisible(true)]
    public class DkRibbon : Office.IRibbonExtensibility,IController
    {
        private Office.IRibbonUI ribbon;

        public DkRibbon()
        {
        }

        #region IRibbonExtensibility 成员

        public string GetCustomUI(string ribbonID)
        {
            return GetResourceText("Excel.DbTool.DkRibbon.xml");
        }

        #endregion

        #region 功能区回调
        //在此创建回调方法。有关添加回调方法的详细信息，请访问 http://go.microsoft.com/fwlink/?LinkID=271226

        

        #endregion

        #region 帮助器

        private static string GetResourceText(string resourceName)
        {
            Assembly asm = Assembly.GetExecutingAssembly();
            string[] resourceNames = asm.GetManifestResourceNames();
            for (int i = 0; i < resourceNames.Length; ++i)
            {
                if (string.Compare(resourceName, resourceNames[i], StringComparison.OrdinalIgnoreCase) == 0)
                {
                    using (StreamReader resourceReader = new StreamReader(asm.GetManifestResourceStream(resourceNames[i])))
                    {
                        if (resourceReader != null)
                        {
                            return resourceReader.ReadToEnd();
                        }
                    }
                }
            }
            return null;
        }

        #endregion
        #region 原来的参考代码

        //const string ALL = "全部";

        //IDbManager dbManager;

        //int pageIndex = 1;
        //int pageSize = 27;
        //int pageCount = 0;
        //int maxPageCount = 6;

        //List<string> lstTableName;
        //private void Menubar_Load(object sender, RibbonUIEventArgs e)
        //{
        //    this.InitDrpDBSource(null, null);
        //    this.btnDBSourceSetting.Click += btnDBSourceSetting_Click;
        //    this.drpDBlist.SelectionChanged += drpDBlist_SelectionChanged;
        //    this.drpTablelist.SelectionChanged += drpTablelist_SelectionChanged;
        //    this.btnTableSchemal.Click += DisplayTableMeta;
        //    this.btnTableSave.Click += SaveTableMeta;
        //    Globals.ThisAddIn.Application.SheetBeforeDoubleClick += Application_SheetBeforeDoubleClick;
        //    this.btnLast.Click += this.BtnLast_Click;
        //    this.btnNext.Click += this.BtnNext_Click;

        //    var lstitem = System.Linq.Enumerable.Range(5, 5).Select(x => Tuple.Create(x, Globals.Factory.GetRibbonFactory().CreateRibbonDropDownItem())).ToList();
        //    lstitem.ForEach(x => x.Item2.Label = x.Item1.ToString());
        //    lstitem.ForEach(x => this.drpPageSize.Items.Add(x.Item2));
        //    this.drpPageSize.SelectedItemIndex = 1;
        //    drpPageSize.SelectionChanged += DrpPageSize_SelectionChanged;
        //    this.pageSize = int.Parse(this.drpPageSize.SelectedItem.Label) * 3;

        //    //((Microsoft.Office.Interop.Excel.AppEvents_Event)Globals.ThisAddIn.Application).SheetBeforeRightClick += ToolBar_SheetBeforeRightClick;
        //}

        //private void ToolBar_SheetBeforeRightClick(object Sh, Microsoft.Office.Interop.Excel.Range Target, ref bool Cancel)
        //{
        //    System.Windows.Forms.MessageBox.Show("右键");
        //}

        //private void DrpPageSize_SelectionChanged(object sender, RibbonControlEventArgs e)
        //{
        //    //a222
        //    this.pageSize = int.Parse(this.drpPageSize.SelectedItem.Label) * 3;
        //    this.pageIndex = 1;
        //    drpDBlist_SelectionChanged(sender, e);
        //}


        //private void BtnNext_Click(object sender, RibbonControlEventArgs e)
        //{
        //    if (pageIndex >= this.pageCount)
        //        return;
        //    pageIndex += 1;
        //    PageHandler();
        //}

        //private void BtnLast_Click(object sender, RibbonControlEventArgs e)
        //{
        //    if (pageIndex <= 1)
        //        return;
        //    pageIndex -= 1;
        //    PageHandler();
        //}
        //private void Application_SheetBeforeDoubleClick(object Sh, Microsoft.Office.Interop.Excel.Range target, ref bool Cancel)
        //{
        //    var lstbutton = this.tableCollection.Items.Cast<RibbonCheckBox>().Where(x => x.Checked).ToList();
        //    if (lstbutton.Count < 1)
        //        return;

        //    if (!CheckDrpSelectedValue(drpDBlist, drpTablelist))
        //        return;
        //    Cancel = true;
        //    string cnnstr = ((Tuple<string, string>)drpDBlist.SelectedItem.Tag).Item2;
        //    lstbutton.ForEach(x => {
        //        x.Checked = false;
        //        target = this.DisplayTableMeta(x.Label, target);
        //    });
        //    //string tableName = lstbutton.Label;
        //    //this.DisplayTableMeta(tableName, Target);
        //    //lstbutton.Checked = false;
        //}

        //private void FocusButton(object sender, RibbonControlEventArgs e)
        //{
        //    //RibbonButton button = sender as RibbonButton;
        //    //var lst= this.tableCollection.Items.Cast<RibbonButton>().Except(new List<RibbonButton>() { button}).ToList();
        //    //lst.ForEach(x=>x.ShowImage=false);
        //    //button.ShowImage = !button.ShowImage;
        //}
        ///// <summary>
        ///// 初始化数据库下拉列表
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="e"></param>
        //void InitDrpDBSource(object sender, System.Windows.Forms.FormClosedEventArgs e)
        //{
        //    this.drpDBlist.Items.Clear();
        //    var tmp = this.Factory.CreateRibbonDropDownItem();
        //    tmp.Label = "请选择数据库";
        //    tmp.Tag = string.Empty;
        //    this.drpDBlist.Items.Add(tmp);
        //    var lstDB = System.Configuration.ConfigurationManager.OpenExeConfiguration(System.Configuration.ConfigurationUserLevel.None).ConnectionStrings
        //        .ConnectionStrings.Cast<System.Configuration.ConnectionStringSettings>().Where(x => x.Name != "LocalSqlServer").ToList();
        //    foreach (var db in lstDB)
        //    {
        //        tmp = this.Factory.CreateRibbonDropDownItem();
        //        tmp.Label = db.Name;
        //        tmp.Tag = Tuple.Create(db.ProviderName, db.ConnectionString);
        //        this.drpDBlist.Items.Add(tmp);
        //    }
        //}
        //private void PageHandler()
        //{
        //    var lstbutton = this.tableCollection.Items.Cast<RibbonCheckBox>().ToList();
        //    lstbutton.ForEach(x => x.Visible = false);
        //    int maxindex = Math.Min(lstTableName.Count, pageIndex * pageSize);
        //    for (int i = pageIndex * pageSize - pageSize; i < maxindex; i++)
        //    {
        //        lstbutton[i].Label = lstTableName[i];
        //        lstbutton[i].Visible = true;

        //    }
        //    //var lstTableNameTmp = lstTableName.Skip(this.pageIndex * pageSize - pageSize).Take(pageSize).ToList();
        //    //for (int i = 0; i < lstTableNameTmp.Count; i++)
        //    //{
        //    //    lstbutton[i].Label = lstTableNameTmp[i];
        //    //    lstbutton[i].Visible = true;
        //    //}
        //    this.tableCollection.Label = string.Format("第{0}/{1}页，共{2}个表", pageIndex, pageCount, lstTableName.Count);
        //}
        ///// <summary>
        ///// 切换表格下拉列表的响应方法
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="e"></param>
        //void drpTablelist_SelectionChanged(object sender, RibbonControlEventArgs e)
        //{
        //    if (this.drpTablelist.SelectedItem == null)
        //        return;
        //    var range = Globals.ThisAddIn.Application.Cells.Find(this.drpTablelist.SelectedItem.Label, MatchByte: true);
        //    if (range != null)
        //        range.Activate();
        //}

        ///// <summary>
        ///// 切换数据库下拉列表的响应方法
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="e"></param>
        //void drpDBlist_SelectionChanged(object sender, RibbonControlEventArgs e)
        //{
        //    this.drpTablelist.Items.Clear();
        //    this.drpTablelist.Label = string.Empty;
        //    var tmp = this.drpDBlist.SelectedItem.Tag as Tuple<string, string>;
        //    DbType dbType;
        //    if (tmp == null || !System.Enum.TryParse<DbType>(tmp.Item1, out dbType))
        //        return;
        //    var flagType = typeof(DbManager);
        //    var lstDbManger = System.Reflection.Assembly.GetExecutingAssembly().GetTypes()
        //        .Where(x => flagType.IsAssignableFrom(x) && flagType.Name != x.Name)
        //        .Select(x => (DbManager)System.Activator.CreateInstance(x)).ToList();
        //    this.dbManager = lstDbManger.FirstOrDefault(x => x.GetDbType() == dbType);
        //    if (this.dbManager == null)
        //        throw new ArgumentException("不支持指定的数据库");
        //    this.dbManager.SetConnectionString(tmp.Item2);
        //    lstTableName = this.dbManager.GetAllTableName();
        //    var drpItem = this.Factory.CreateRibbonDropDownItem();
        //    drpItem.Label = ALL;
        //    drpItem.Tag = ALL;
        //    this.drpTablelist.Items.Add(drpItem);
        //    foreach (var tname in lstTableName)
        //    {
        //        drpItem = this.Factory.CreateRibbonDropDownItem();
        //        drpItem.Label = tname;
        //        drpItem.Tag = tname;
        //        this.drpTablelist.Items.Add(drpItem);
        //    }
        //    this.drpTablelist.SelectedItemIndex = 0;

        //    this.pageCount = (int)Math.Ceiling((1.0) * lstTableName.Count / this.pageSize);
        //    if (this.pageCount > this.maxPageCount)
        //        this.pageCount = this.maxPageCount;
        //    this.pageIndex = 1;
        //    PageHandler();

        //}

        ///// <summary>
        ///// 设置数据源
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="e"></param>
        //void btnDBSourceSetting_Click(object sender, RibbonControlEventArgs e)
        //{
        //    Views.DBSourceSetting window = new Views.DBSourceSetting();
        //    window.FormClosed += InitDrpDBSource;
        //    window.ShowDialog();
        //}

        ///// <summary>
        ///// 保存设计
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="e"></param>
        //void SaveTableMeta(object sender, RibbonControlEventArgs e)
        //{
        //    if (!CheckDrpSelectedValue(drpDBlist, drpTablelist))
        //        return;
        //    Microsoft.Office.Interop.Excel.Range range = Globals.ThisAddIn.Application.Selection;
        //    string cnnstr = ((Tuple<string, string>)drpDBlist.SelectedItem.Tag).Item2;

        //    if (range.Rows.Count <= 2)
        //        throw new ArgumentException("请选中需要处理的表的区域");

        //    object[,] rangeValue = range.Value2;

        //    var lstHeaderTemplate = System.Enum.GetNames(typeof(RangeTitle)).Cast<string>().ToList();
        //    if (rangeValue.GetUpperBound(1) != lstHeaderTemplate.Count && rangeValue.GetUpperBound(1) != lstHeaderTemplate.Count - 1)//因为 range.Value2里面的数组从1，1开始，+1的情况表示修改名称
        //        throw new ArgumentException("选择区域的数据与模版格式不一致！");
        //    var lstHeaderCell = System.Linq.Enumerable.Range(1, rangeValue.GetUpperBound(1)).Select(x => rangeValue.GetValue(2, x) as string).ToList();
        //    if (lstHeaderCell.Except(lstHeaderTemplate).Count() > 0)
        //        throw new ArgumentException("选择区域的数据与模版格式不一致！");

        //    TableMeta tableMeta = SaveTableMeta(rangeValue);

        //    List<string> lstTableName = drpTablelist.Items.Select(x => ((object)(x.Tag)).ToString()).ToList();
        //    bool isNewTable = !lstTableName.Contains(tableMeta.Name);
        //    bool isColumnRename = rangeValue.GetUpperBound(1) == lstHeaderTemplate.Count;
        //    if (isNewTable)
        //    {
        //        this.dbManager.CreateTable(tableMeta);
        //        this.drpDBlist_SelectionChanged(null, null);
        //        this.drpTablelist.SelectedItem = this.drpTablelist.Items.First(x => x.Label == tableMeta.Name);
        //        System.Windows.Forms.MessageBox.Show("添加成功");
        //        return;
        //    }
        //    if (isColumnRename)
        //    {
        //        Dictionary<string, string> dict = new Dictionary<string, string>();
        //        tableMeta.NewName = rangeValue[1, (int)RangeTitle.重命名] as string;
        //        for (int i = 3; i <= rangeValue.GetUpperBound(0); i++)
        //        {
        //            dict.Add(rangeValue[i, (int)RangeTitle.名称] as string, rangeValue[i, (int)RangeTitle.重命名] as string);
        //        }
        //        this.dbManager.AlterTableWithRename(tableMeta, dict);
        //        this.drpDBlist_SelectionChanged(null, null);
        //        tableMeta.NewName = tableMeta.NewName ?? tableMeta.Name;
        //        this.drpTablelist.SelectedItem = this.drpTablelist.Items.First(x => x.Label == tableMeta.NewName);
        //        System.Windows.Forms.MessageBox.Show("重命名成功");
        //    }
        //    this.dbManager.AlterTable(tableMeta);
        //    System.Windows.Forms.MessageBox.Show("修改成功");
        //}

        //private TableMeta SaveTableMeta(object[,] rangeValue)
        //{
        //    TableMeta tableMeta = new TableMeta();
        //    tableMeta.Name = rangeValue[1, (int)RangeTitle.名称] as string;
        //    if (string.IsNullOrEmpty(tableMeta.Name))
        //        throw new ArgumentException("必须填写表名称");
        //    tableMeta.Remark = rangeValue[1, (int)RangeTitle.备注] as string;

        //    Dictionary<string, ConstraintType> dictConstraint = System.Enum.GetValues(typeof(ConstraintType))
        //        .Cast<ConstraintType>()
        //        .Where(x => x != ConstraintType.未识别)
        //        .ToDictionary(x => x.ToString(), x => x);
        //    for (int i = 3; i <= rangeValue.GetUpperBound(0); i++)
        //    {
        //        ColumnMeta column = new ColumnMeta();
        //        column.Name = rangeValue[i, (int)RangeTitle.名称] as string;
        //        column.DataType = rangeValue[i, (int)RangeTitle.类型] as string;
        //        string tmp = rangeValue[i, (int)RangeTitle.约束]?.ToString();
        //        column.UnNullable = dictConstraint.ContainsKey(tmp ?? string.Empty);
        //        if (column.UnNullable)
        //        {
        //            column.Constraints.Add(new ColumnConstraint()
        //            {
        //                ColumnName = column.Name,
        //                TableName = tableMeta.Name,
        //                Type = dictConstraint[tmp ?? string.Empty]
        //            });
        //        }
        //        column.Comment = new Comment() { Remark = rangeValue[i, (int)RangeTitle.备注] as string };
        //        tableMeta.Columns.Add(column);
        //    }
        //    return tableMeta;
        //}

        ///// <summary>
        ///// 查看设计
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="e"></param>
        //void DisplayTableMeta(object sender, RibbonControlEventArgs e)
        //{
        //    if (!CheckDrpSelectedValue(drpDBlist, drpTablelist))
        //        return;
        //    string cnnstr = ((Tuple<string, string>)drpDBlist.SelectedItem.Tag).Item2;
        //    string tableName = this.drpTablelist.SelectedItem.Tag.ToString();
        //    var range = Globals.ThisAddIn.Application.ActiveCell;
        //    if (!ALL.Equals(tableName))
        //    {
        //        this.DisplayTableMeta(tableName, range);
        //        return;
        //    }
        //    Views.ProcessDashboard pd = new Views.ProcessDashboard();
        //    pd.TableMetaHandler = () =>
        //    {
        //        foreach (var item in this.drpTablelist.Items)
        //        {
        //            if (ALL.Equals(item.Label))
        //                continue;
        //            range = this.DisplayTableMeta(item.Label, range);
        //        }
        //    };
        //    pd.ShowDialog();
        //}

        //Microsoft.Office.Interop.Excel.Range DisplayTableMeta(string tableName, Microsoft.Office.Interop.Excel.Range cell)
        //{
        //    var tableMeta = this.dbManager.GetTableMetaByName(tableName);
        //    int rowIndex = 0;
        //    cell.UnMerge();
        //    cell.Offset[rowIndex, 0].Value2 = tableMeta.Name;
        //    var lstHeader = System.Enum.GetValues(typeof(RangeTitle)).Cast<int>().ToList();
        //    cell.Offset[rowIndex++, lstHeader.Count - 2].Value2 = tableMeta.Remark;
        //    lstHeader.ForEach(index => cell.Offset[rowIndex, index - 1].Value2 = ((RangeTitle)index).ToString());
        //    tableMeta.Columns.ForEach(column =>
        //    {
        //        rowIndex++;
        //        cell.Offset[rowIndex, (int)RangeTitle.名称 - 1].Value2 = column.Name;
        //        cell.Offset[rowIndex, (int)RangeTitle.类型 - 1].Value2 = this.DisplayTableMetaDataType(column);
        //        cell.Offset[rowIndex, (int)RangeTitle.约束 - 1].Value2 = this.DisplayTableMetaConstraints(column);
        //        cell.Offset[rowIndex, (int)RangeTitle.备注 - 1].Value2 = column.Comment?.Remark;
        //    });
        //    //处理样式
        //    var firstCell = cell.Offset[1, 0];
        //    var lastCell = cell.Offset[rowIndex, lstHeader.Count - 1];
        //    var range = Globals.ThisAddIn.Application.get_Range(cell, cell.Offset[0, lstHeader.Count - 3]);
        //    //range.Merge();
        //    range.HorizontalAlignment = Microsoft.Office.Interop.Excel.Constants.xlLeft;
        //    //range.Select();
        //    range = Globals.ThisAddIn.Application.get_Range(firstCell, lastCell);
        //    range.Columns.AutoFit();
        //    //range = Globals.ThisAddIn.Application.get_Range(cell,lastCell);
        //    System.Enum.GetValues(typeof(Microsoft.Office.Interop.Excel.XlBordersIndex))
        //        .Cast<Microsoft.Office.Interop.Excel.XlBordersIndex>()
        //        .Where(x => x != Microsoft.Office.Interop.Excel.XlBordersIndex.xlDiagonalDown && x != Microsoft.Office.Interop.Excel.XlBordersIndex.xlDiagonalUp)
        //        .ToList()
        //        .ForEach(x => range.Borders[x].LineStyle = Microsoft.Office.Interop.Excel.XlLineStyle.xlContinuous);
        //    firstCell.Select();
        //    return cell.Offset[rowIndex + 2, 0];
        //}

        //private string DisplayTableMetaDataType(ColumnMeta column)
        //{
        //    if (column.CharacterMaxLength.HasValue)
        //        return $"{column.DataType}({column.CharacterMaxLength.Value})";
        //    return column.DataType;
        //}

        //private string DisplayTableMetaConstraints(ColumnMeta column)
        //{
        //    var ct = column.Constraints.FirstOrDefault();
        //    if (ct != null)
        //        return ct.Type.ToString();
        //    if (column.UnNullable)
        //        return ConstraintType.非空.ToString();
        //    return string.Empty;
        //}

        //private bool CheckDrpSelectedValue(params RibbonDropDown[] drps)
        //{
        //    foreach (var item in drps)
        //    {

        //        if (item.SelectedItem == null || item.SelectedItem.Tag == null)
        //            return false;
        //    }
        //    return true;
        //}


        #endregion
        public void Start(Office.IRibbonUI ribbonUI)
        {
            this.ribbon = ribbonUI;
        }
    }
}
