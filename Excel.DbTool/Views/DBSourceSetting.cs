using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Excel.DbTool.Views
{
    public partial class DBSourceSetting : Form
    {
        public DBSourceSetting()
        {
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            int index=0;
            this.dgDBlist.AutoGenerateColumns = false;
            this.dgDBlist.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            this.dgDBlist.EditMode = DataGridViewEditMode.EditProgrammatically;
            this.dgDBlist.Columns.Add(new DataGridViewCheckBoxColumn() { DisplayIndex = index++ });
            this.dgDBlist.Columns.Add(new System.Windows.Forms.DataGridViewTextBoxColumn() { DisplayIndex = index++, DataPropertyName = "Name", HeaderText = "名称",Width=200 });
            this.dgDBlist.Columns.Add(new System.Windows.Forms.DataGridViewTextBoxColumn() { DisplayIndex = index++, DataPropertyName = "Provider", HeaderText = "名称", Width = 200 });
            this.dgDBlist.Columns.Add(new System.Windows.Forms.DataGridViewTextBoxColumn() { DisplayIndex = index++, DataPropertyName = "Value", HeaderText = "数据库地址" ,Width=600});
            this.InitDBInfoList();
            this.btnAdd.Click += btnAdd_Click;
            this.btnEdit.Click += btnEdit_Click;
            this.btnDelete.Click += btnDelete_Click;
        }

        void btnDelete_Click(object sender, EventArgs e)
        {
            if (this.dgDBlist.SelectedRows.Count < 1)
                return;
            var config = System.Configuration.ConfigurationManager.OpenExeConfiguration(System.Configuration.ConfigurationUserLevel.None);
            var dbInfo= (KV)this.dgDBlist.SelectedRows[0].DataBoundItem;
            config.ConnectionStrings.ConnectionStrings.Remove(dbInfo.Name);
            config.Save();
            this.InitDBInfoList();
        }

        void btnEdit_Click(object sender, EventArgs e)
        {
            if (this.dgDBlist.SelectedRows.Count < 1)
                return;
            var window = new DBSourceAdd();
            window.OnButtonOKClicked += InitDBInfoList;
            window.Text = "修改";
            window.DBInfo= (KV)this.dgDBlist.SelectedRows[0].DataBoundItem;
            window.ShowDialog();
        }

        void btnAdd_Click(object sender, EventArgs e)
        {
            var window = new DBSourceAdd();
            window.OnButtonOKClicked += InitDBInfoList;
            window.Text = "添加";
            window.ShowDialog();
        }

        void InitDBInfoList()
        {
            var lst = System.Configuration.ConfigurationManager.OpenExeConfiguration(System.Configuration.ConfigurationUserLevel.None)
                .ConnectionStrings.ConnectionStrings.Cast<System.Configuration.ConnectionStringSettings>()
                .Select(x => new KV { Name = x.Name, Value = x.ConnectionString ,Provider=x.ProviderName}).ToList();
            this.dgDBlist.DataSource = lst;
        }


    }

    public class KV {
        public string Name { get; set; }
        public string Value { get; set; }
        public string Provider { get; set; }
    }
}
