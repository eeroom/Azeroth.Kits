using System;
using System.Linq;
using Microsoft.Office.Tools.Ribbon;

namespace Excel.DbTool
{
    partial class Menubar : Microsoft.Office.Tools.Ribbon.RibbonBase
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        public Menubar()
            : base(Globals.Factory.GetRibbonFactory())
        {
            InitializeComponent();
            var lstbutton = System.Linq.Enumerable.Range(0, maxTableCount).Select((x, i) => System.Tuple.Create(i, this.Factory.CreateRibbonButton())).ToList();
            lstbutton.ForEach(x => {
                x.Item2.Name = "table" + x.Item1.ToString();
                x.Item2.Label = "table" + x.Item1.ToString();
                x.Item2.Visible = false;
                x.Item2.Image = Excel.DbTool.Properties.Resources.AddTableHH;
                x.Item2.Click += this.FocusButton;
            });
            lstbutton.ForEach(x => this.tableCollection.Items.Add(x.Item2));
        }

       

        /// <summary> 
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 组件设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Menubar));
            this.container = this.Factory.CreateRibbonTab();
            this.datasourceCollection = this.Factory.CreateRibbonGroup();
            this.btnDBSourceSetting = this.Factory.CreateRibbonButton();
            this.drpDBlist = this.Factory.CreateRibbonDropDown();
            this.drpTablelist = this.Factory.CreateRibbonDropDown();
            this.handlerCollection = this.Factory.CreateRibbonGroup();
            this.btnTableSchemal = this.Factory.CreateRibbonButton();
            this.btnTableSave = this.Factory.CreateRibbonButton();
            this.tableCollection = this.Factory.CreateRibbonGroup();
            this.container.SuspendLayout();
            this.datasourceCollection.SuspendLayout();
            this.handlerCollection.SuspendLayout();
            this.SuspendLayout();
            // 
            // container
            // 
            this.container.Groups.Add(this.datasourceCollection);
            this.container.Groups.Add(this.handlerCollection);
            this.container.Groups.Add(this.tableCollection);
            this.container.Label = "StormwindDesigner";
            this.container.Name = "container";
            // 
            // datasourceCollection
            // 
            this.datasourceCollection.Items.Add(this.btnDBSourceSetting);
            this.datasourceCollection.Items.Add(this.drpDBlist);
            this.datasourceCollection.Items.Add(this.drpTablelist);
            this.datasourceCollection.Label = "数据源";
            this.datasourceCollection.Name = "datasourceCollection";
            // 
            // btnDBSourceSetting
            // 
            this.btnDBSourceSetting.ControlSize = Microsoft.Office.Core.RibbonControlSize.RibbonControlSizeLarge;
            this.btnDBSourceSetting.Image = ((System.Drawing.Image)(resources.GetObject("btnDBSourceSetting.Image")));
            this.btnDBSourceSetting.Label = "设置\r\n";
            this.btnDBSourceSetting.Name = "btnDBSourceSetting";
            this.btnDBSourceSetting.ShowImage = true;
            // 
            // drpDBlist
            // 
            this.drpDBlist.Label = "";
            this.drpDBlist.Name = "drpDBlist";
            // 
            // drpTablelist
            // 
            this.drpTablelist.Label = "";
            this.drpTablelist.Name = "drpTablelist";
            // 
            // handlerCollection
            // 
            this.handlerCollection.Items.Add(this.btnTableSchemal);
            this.handlerCollection.Items.Add(this.btnTableSave);
            this.handlerCollection.Label = "操作";
            this.handlerCollection.Name = "handlerCollection";
            // 
            // btnTableSchemal
            // 
            this.btnTableSchemal.ControlSize = Microsoft.Office.Core.RibbonControlSize.RibbonControlSizeLarge;
            this.btnTableSchemal.Image = global::Excel.DbTool.Properties.Resources.PropertiesHH;
            this.btnTableSchemal.Label = "查看\r\n";
            this.btnTableSchemal.Name = "btnTableSchemal";
            this.btnTableSchemal.ShowImage = true;
            // 
            // btnTableSave
            // 
            this.btnTableSave.ControlSize = Microsoft.Office.Core.RibbonControlSize.RibbonControlSizeLarge;
            this.btnTableSave.Image = global::Excel.DbTool.Properties.Resources.SaveHH;
            this.btnTableSave.Label = "保存\r\n";
            this.btnTableSave.Name = "btnTableSave";
            this.btnTableSave.ShowImage = true;
            // 
            // tableCollection
            // 
            this.tableCollection.Label = "数据表";
            this.tableCollection.Name = "tableCollection";
            // 
            // Menubar
            // 
            this.Name = "Menubar";
            this.RibbonType = "Microsoft.Excel.Workbook";
            this.Tabs.Add(this.container);
            this.Load += new Microsoft.Office.Tools.Ribbon.RibbonUIEventHandler(this.Menubar_Load);
            this.container.ResumeLayout(false);
            this.container.PerformLayout();
            this.datasourceCollection.ResumeLayout(false);
            this.datasourceCollection.PerformLayout();
            this.handlerCollection.ResumeLayout(false);
            this.handlerCollection.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        internal Microsoft.Office.Tools.Ribbon.RibbonTab container;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton btnTableSchemal;
        internal Microsoft.Office.Tools.Ribbon.RibbonGroup datasourceCollection;
        internal Microsoft.Office.Tools.Ribbon.RibbonDropDown drpDBlist;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton btnDBSourceSetting;
        internal Microsoft.Office.Tools.Ribbon.RibbonDropDown drpTablelist;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton btnTableSave;
        internal Microsoft.Office.Tools.Ribbon.RibbonGroup handlerCollection;
        internal RibbonGroup tableCollection;
    }

    partial class ThisRibbonCollection
    {
        internal Menubar ToolBar
        {
            get { return this.GetRibbon<Menubar>(); }
        }
    }
}
