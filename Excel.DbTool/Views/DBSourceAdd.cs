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
    public partial class DBSourceAdd : Form
    {
        public DBSourceAdd()
        {
            InitializeComponent();
        }

        public ConnectionStringWrapper ConnectionStringWrapper { get; set; }

        public event Action OnButtonOKClicked;

        bool isAdd;

        protected override void OnLoad(EventArgs e)
        {
            var lst = System.Enum.GetNames(typeof(DbCategory)).Select(x => new ConnectionStringWrapper() { Name = x, Value = x }).ToList();
            this.cmbDbProvider.DisplayMember = "Name";
            this.cmbDbProvider.ValueMember = "Value";
            this.cmbDbProvider.DataSource = lst;
            this.isAdd = ConnectionStringWrapper == null;
            this.btnOK.Click += btnOK_Click;
            this.ConnectionStringWrapper = this.ConnectionStringWrapper ?? new ConnectionStringWrapper();
            this.txtName.Text = ConnectionStringWrapper.Name;
            this.txtValue.Text = ConnectionStringWrapper.Value;
            this.cmbDbProvider.SelectedValue = this.ConnectionStringWrapper.Provider ?? string.Empty;
        }

        private void InitEdit()
        {
           
        }

        void btnOK_Click(object sender, EventArgs e)
        {
            this.ConnectionStringWrapper.Name = this.txtName.Text;
            this.ConnectionStringWrapper.Value = this.txtValue.Text;
            this.ConnectionStringWrapper.Provider = ((ConnectionStringWrapper)this.cmbDbProvider.SelectedItem).Value;
            var config = System.Configuration.ConfigurationManager.OpenExeConfiguration(System.Configuration.ConfigurationUserLevel.None);
            if (this.isAdd)
                config.ConnectionStrings.ConnectionStrings.Add(new System.Configuration.ConnectionStringSettings(this.ConnectionStringWrapper.Name, this.ConnectionStringWrapper.Value, this.ConnectionStringWrapper.Provider));
            else
            {
                config.ConnectionStrings.ConnectionStrings[this.ConnectionStringWrapper.Name].ConnectionString = this.ConnectionStringWrapper.Value;
                config.ConnectionStrings.ConnectionStrings[this.ConnectionStringWrapper.Name].ProviderName = this.ConnectionStringWrapper.Provider;
            }
            config.Save();
            if (this.OnButtonOKClicked != null)
                this.OnButtonOKClicked();
            this.Close();
        }
    }
}
