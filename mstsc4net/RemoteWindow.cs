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
            this.axMsRdpClient6NotSafeForScripting1.Height = 600;
            this.axMsRdpClient6NotSafeForScripting1.Width = 800;
            this.axMsRdpClient6NotSafeForScripting1.Server = this.txtIp.Text;
            this.axMsRdpClient6NotSafeForScripting1.AdvancedSettings7.RDPPort = int.Parse(this.txtPort.Text);
            this.axMsRdpClient6NotSafeForScripting1.UserName = this.txtUserName.Text;
            this.axMsRdpClient6NotSafeForScripting1.AdvancedSettings7.ClearTextPassword = this.txtPwd.Text;
            this.axMsRdpClient6NotSafeForScripting1.Domain = string.Empty;
            this.axMsRdpClient6NotSafeForScripting1.AdvancedSettings7.LoadBalanceInfo = string.Empty;
            this.axMsRdpClient6NotSafeForScripting1.Connect();

        }
    }
}
