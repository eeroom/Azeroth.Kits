using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Excel.Extension
{
    public partial class FrmDBSourceAdd : Form
    {
        public FrmDBSourceAdd()
        {
            InitializeComponent();
        }

        public KV DBInfo { get; set; }

        public event Action OnButtonOKClicked;

        bool isAdd;

        protected override void OnLoad(EventArgs e)
        {
            var lst = System.Enum.GetNames(typeof(DBProvider)).Select(x => new KV() { Name = x, Value = x }).ToList();
            this.cmbDbProvider.DisplayMember = "Name";
            this.cmbDbProvider.ValueMember = "Value";
            this.cmbDbProvider.DataSource = lst;
            this.isAdd = DBInfo == null;
            this.btnOK.Click += btnOK_Click;
            this.DBInfo = this.DBInfo ?? new KV();
            this.txtName.Text = DBInfo.Name;
            this.txtValue.Text = DBInfo.Value;
            this.cmbDbProvider.SelectedValue = this.DBInfo.Provider ?? string.Empty;
        }

        private void InitEdit()
        {
           
        }

        void btnOK_Click(object sender, EventArgs e)
        {
            this.DBInfo.Name = this.txtName.Text;
            this.DBInfo.Value = this.txtValue.Text;
            this.DBInfo.Provider = ((KV)this.cmbDbProvider.SelectedItem).Value;
            var config = System.Configuration.ConfigurationManager.OpenExeConfiguration(System.Configuration.ConfigurationUserLevel.None);
            if (this.isAdd)
                config.ConnectionStrings.ConnectionStrings.Add(new System.Configuration.ConnectionStringSettings(this.DBInfo.Name, this.DBInfo.Value, this.DBInfo.Provider));
            else
            {
                config.ConnectionStrings.ConnectionStrings[this.DBInfo.Name].ConnectionString = this.DBInfo.Value;
                config.ConnectionStrings.ConnectionStrings[this.DBInfo.Name].ProviderName = this.DBInfo.Provider;
            }
            config.Save();
            if (this.OnButtonOKClicked != null)
                this.OnButtonOKClicked();
            this.Close();
        }
    }
}
