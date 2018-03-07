namespace Excel.Extension
{
    partial class ToolBar : Microsoft.Office.Tools.Ribbon.RibbonBase
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        public ToolBar()
            : base(Globals.Factory.GetRibbonFactory())
        {
            InitializeComponent();
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
            this.tab1 = this.Factory.CreateRibbonTab();
            this.group2 = this.Factory.CreateRibbonGroup();
            this.btnDBSourceSetting = this.Factory.CreateRibbonButton();
            this.drpDBlist = this.Factory.CreateRibbonDropDown();
            this.drpTablelist = this.Factory.CreateRibbonDropDown();
            this.btnTableSchemal = this.Factory.CreateRibbonButton();
            this.btnTableSave = this.Factory.CreateRibbonButton();
            this.tab1.SuspendLayout();
            this.group2.SuspendLayout();
            // 
            // tab1
            // 
            this.tab1.Groups.Add(this.group2);
            this.tab1.Label = "Azeroth-Tools";
            this.tab1.Name = "tab1";
            // 
            // group2
            // 
            this.group2.Items.Add(this.btnDBSourceSetting);
            this.group2.Items.Add(this.drpDBlist);
            this.group2.Items.Add(this.drpTablelist);
            this.group2.Items.Add(this.btnTableSchemal);
            this.group2.Items.Add(this.btnTableSave);
            this.group2.Label = "数据库助手";
            this.group2.Name = "group2";
            // 
            // btnDBSourceSetting
            // 
            this.btnDBSourceSetting.ControlSize = Microsoft.Office.Core.RibbonControlSize.RibbonControlSizeLarge;
            this.btnDBSourceSetting.Image = global::Excel.Extension.Properties.Resources.Gear;
            this.btnDBSourceSetting.Label = "设置\r\n数据源";
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
            // btnTableSchemal
            // 
            this.btnTableSchemal.ControlSize = Microsoft.Office.Core.RibbonControlSize.RibbonControlSizeLarge;
            this.btnTableSchemal.Image = global::Excel.Extension.Properties.Resources.PropertiesHH;
            this.btnTableSchemal.Label = "查看\r\n设计";
            this.btnTableSchemal.Name = "btnTableSchemal";
            this.btnTableSchemal.ShowImage = true;
            // 
            // btnTableSave
            // 
            this.btnTableSave.ControlSize = Microsoft.Office.Core.RibbonControlSize.RibbonControlSizeLarge;
            this.btnTableSave.Image = global::Excel.Extension.Properties.Resources.SaveHH;
            this.btnTableSave.Label = "保存\r\n设计";
            this.btnTableSave.Name = "btnTableSave";
            this.btnTableSave.ShowImage = true;
            // 
            // ToolBar
            // 
            this.Name = "ToolBar";
            this.RibbonType = "Microsoft.Excel.Workbook";
            this.Tabs.Add(this.tab1);
            this.Load += new Microsoft.Office.Tools.Ribbon.RibbonUIEventHandler(this.ToolBar_Load);
            this.tab1.ResumeLayout(false);
            this.tab1.PerformLayout();
            this.group2.ResumeLayout(false);
            this.group2.PerformLayout();

        }

        #endregion

        internal Microsoft.Office.Tools.Ribbon.RibbonTab tab1;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton btnTableSchemal;
        internal Microsoft.Office.Tools.Ribbon.RibbonGroup group2;
        internal Microsoft.Office.Tools.Ribbon.RibbonDropDown drpDBlist;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton btnDBSourceSetting;
        internal Microsoft.Office.Tools.Ribbon.RibbonDropDown drpTablelist;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton btnTableSave;
    }

    partial class ThisRibbonCollection
    {
        internal ToolBar ToolBar
        {
            get { return this.GetRibbon<ToolBar>(); }
        }
    }
}
