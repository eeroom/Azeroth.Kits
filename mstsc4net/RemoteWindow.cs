using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace mstsc4net
{
    public partial class RemoteWindow : Form
    {
        public RemoteWindow()
        {
            InitializeComponent();
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            this.rdpClient.Server = this.txtIp.Text;
            this.rdpClient.AdvancedSettings7.RDPPort = int.Parse(this.txtPort.Text);
            this.rdpClient.UserName = this.txtUserName.Text;
            this.rdpClient.AdvancedSettings7.ClearTextPassword = this.txtPwd.Text;
            this.rdpClient.AdvancedSettings7.EnableCredSspSupport = this.ckEnableCredSspSupport.Checked;
            this.rdpClient.Connect();
            
        }

        private void btnMax_Click(object sender, EventArgs e)
        {
            this.rdpClient.Bounds = Screen.FromControl(this).Bounds;
            this.Visible = false;
            this.rdpClient.FullScreen = true;
        }

        private void RemoteWindow_Load(object sender, EventArgs e)
        {
            this.btnMax.Enabled = false; 
            this.rdpClient.Dock = DockStyle.Fill;
            this.rdpClient.OnConnecting += (el,msg)=> this.btnOk.Enabled = false;
            this.rdpClient.OnConnected += (el, msg) => this.btnMax.Enabled = true;
            this.rdpClient.OnDisconnected += (el, msg) =>
            {
                this.btnOk.Enabled = true;
                this.btnMax.Enabled = false;
            };
            this.rdpClient.OnEnterFullScreenMode += (el, msg) =>
            {
                this.Visible = false;
                var bd = Screen.FromControl(this).Bounds;
                this.rdpClient.Height = bd.Height;
                this.rdpClient.Height = bd.Width;
            };
            this.rdpClient.OnLeaveFullScreenMode += (el, msg) =>
            {
                this.Visible = true;
            };
            
            
        }

        
    }
}
