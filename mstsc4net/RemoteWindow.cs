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
            this.jingdutiao.Step = 3;
            this.jingdutiao.Style = ProgressBarStyle.Continuous;
            System.Threading.ThreadPool.QueueUserWorkItem(x =>
            {
                var timesOut = 0;
                while (timesOut++<100)
                {
                    System.Threading.Thread.Sleep(50);
                    if (this.rdpClient.Connected !=1)
                    {
                        this.BeginInvoke(new MethodInvoker(()=> {
                            this.jingdutiao.PerformStep();
                            if (this.jingdutiao.Value >= 100)
                                this.jingdutiao.Value = 0;
                        }));
                    }
                    else
                    {
                        this.BeginInvoke(new MethodInvoker(() => this.jingdutiao.Value = 0));
                        return;
                    }
                }
                this.BeginInvoke(new MethodInvoker(() => {
                    this.rdpClient.Disconnect();
                    this.jingdutiao.Value = 0;
                    this.btnOk.Enabled = true;
                    MessageBox.Show("连接失败！,请检查网络、目标机的地址及登陆密码等");
                }));

            });
            
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
            this.btnDisConnect.Enabled = false;
            this.rdpClient.Dock = DockStyle.Fill;
            this.FormClosing += (el, msg) => {
                if (this.rdpClient.Connected != 0)
                    this.rdpClient.Disconnect();
                if (!this.rdpClient.IsDisposed)
                    this.rdpClient.Dispose();
            };
            this.rdpClient.OnConnecting += (el,msg)=> this.btnOk.Enabled = false;
            this.rdpClient.OnConnected += (el, msg) =>
            {
                this.btnMax.Enabled = true;
                this.btnDisConnect.Enabled = true;
            };
            this.rdpClient.OnDisconnected += (el, msg) =>
            {
                this.btnOk.Enabled = true;
                this.btnMax.Enabled = false;
                this.btnDisConnect.Enabled = false;
            };
            this.rdpClient.OnEnterFullScreenMode += (el, msg) =>
            {
                this.Visible = false;
                //下面的代码不起作用，9版本的rdpclient有updateSessionDisplaySettings方法可以调整全屏后的窗口大小
                var bd = Screen.FromControl(this).Bounds;
                this.rdpClient.Height = bd.Height;
                this.rdpClient.Height = bd.Width;
                this.rdpClient.Update();
            };
            this.rdpClient.OnLeaveFullScreenMode += (el, msg) =>
            {
                this.Visible = true;
            };
        }

        private void btnDisConnect_Click(object sender, EventArgs e)
        {
            this.rdpClient.Disconnect();
        }
    }
}
