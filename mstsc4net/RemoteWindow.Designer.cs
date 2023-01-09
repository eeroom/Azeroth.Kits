namespace mstsc4net
{
    partial class RemoteWindow
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

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

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RemoteWindow));
            this.rdpClient = new AxMSTSCLib.AxMsRdpClient6NotSafeForScripting();
            this.label1 = new System.Windows.Forms.Label();
            this.txtIp = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtUserName = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.txtPwd = new System.Windows.Forms.TextBox();
            this.btnOk = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.txtPort = new System.Windows.Forms.TextBox();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.ckEnableCredSspSupport = new System.Windows.Forms.CheckBox();
            this.btnMax = new System.Windows.Forms.Button();
            this.btnDisConnect = new System.Windows.Forms.Button();
            this.jingdutiao = new System.Windows.Forms.ProgressBar();
            this.label5 = new System.Windows.Forms.Label();
            this.textLoadBalanceInfo = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.rdpClient)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // rdpClient
            // 
            this.rdpClient.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rdpClient.Enabled = true;
            this.rdpClient.Location = new System.Drawing.Point(0, 0);
            this.rdpClient.Name = "rdpClient";
            this.rdpClient.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("rdpClient.OcxState")));
            this.rdpClient.Size = new System.Drawing.Size(665, 573);
            this.rdpClient.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 15);
            this.label1.Margin = new System.Windows.Forms.Padding(3, 15, 3, 2);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(29, 12);
            this.label1.TabIndex = 1;
            this.label1.Text = "地址";
            // 
            // txtIp
            // 
            this.txtIp.Location = new System.Drawing.Point(13, 32);
            this.txtIp.Name = "txtIp";
            this.txtIp.Size = new System.Drawing.Size(298, 21);
            this.txtIp.TabIndex = 1;
            this.txtIp.Text = "192.168.1.3";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(13, 127);
            this.label3.Margin = new System.Windows.Forms.Padding(3, 15, 3, 2);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(41, 12);
            this.label3.TabIndex = 1;
            this.label3.Text = "用户名";
            // 
            // txtUserName
            // 
            this.txtUserName.Dock = System.Windows.Forms.DockStyle.Top;
            this.txtUserName.Location = new System.Drawing.Point(13, 144);
            this.txtUserName.Name = "txtUserName";
            this.txtUserName.Size = new System.Drawing.Size(298, 21);
            this.txtUserName.TabIndex = 3;
            this.txtUserName.Text = "Administrator";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(13, 183);
            this.label4.Margin = new System.Windows.Forms.Padding(3, 15, 3, 2);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(29, 12);
            this.label4.TabIndex = 1;
            this.label4.Text = "密码";
            // 
            // txtPwd
            // 
            this.txtPwd.Dock = System.Windows.Forms.DockStyle.Top;
            this.txtPwd.Location = new System.Drawing.Point(13, 200);
            this.txtPwd.Name = "txtPwd";
            this.txtPwd.Size = new System.Drawing.Size(298, 21);
            this.txtPwd.TabIndex = 4;
            this.txtPwd.Text = "123456";
            // 
            // btnOk
            // 
            this.btnOk.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnOk.Location = new System.Drawing.Point(13, 322);
            this.btnOk.Margin = new System.Windows.Forms.Padding(3, 20, 3, 3);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(298, 39);
            this.btnOk.TabIndex = 5;
            this.btnOk.Text = "确定";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(13, 71);
            this.label2.Margin = new System.Windows.Forms.Padding(3, 15, 3, 2);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(29, 12);
            this.label2.TabIndex = 1;
            this.label2.Text = "端口";
            // 
            // txtPort
            // 
            this.txtPort.Dock = System.Windows.Forms.DockStyle.Top;
            this.txtPort.Location = new System.Drawing.Point(13, 88);
            this.txtPort.Name = "txtPort";
            this.txtPort.Size = new System.Drawing.Size(298, 21);
            this.txtPort.TabIndex = 2;
            this.txtPort.Text = "3389";
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.rdpClient);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.flowLayoutPanel1);
            this.splitContainer1.Size = new System.Drawing.Size(992, 573);
            this.splitContainer1.SplitterDistance = 665;
            this.splitContainer1.TabIndex = 4;
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Controls.Add(this.label1);
            this.flowLayoutPanel1.Controls.Add(this.txtIp);
            this.flowLayoutPanel1.Controls.Add(this.label2);
            this.flowLayoutPanel1.Controls.Add(this.txtPort);
            this.flowLayoutPanel1.Controls.Add(this.label3);
            this.flowLayoutPanel1.Controls.Add(this.txtUserName);
            this.flowLayoutPanel1.Controls.Add(this.label4);
            this.flowLayoutPanel1.Controls.Add(this.txtPwd);
            this.flowLayoutPanel1.Controls.Add(this.ckEnableCredSspSupport);
            this.flowLayoutPanel1.Controls.Add(this.label5);
            this.flowLayoutPanel1.Controls.Add(this.textLoadBalanceInfo);
            this.flowLayoutPanel1.Controls.Add(this.btnOk);
            this.flowLayoutPanel1.Controls.Add(this.btnMax);
            this.flowLayoutPanel1.Controls.Add(this.btnDisConnect);
            this.flowLayoutPanel1.Controls.Add(this.jingdutiao);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel1.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Padding = new System.Windows.Forms.Padding(10, 0, 10, 0);
            this.flowLayoutPanel1.Size = new System.Drawing.Size(323, 573);
            this.flowLayoutPanel1.TabIndex = 0;
            // 
            // ckEnableCredSspSupport
            // 
            this.ckEnableCredSspSupport.AutoSize = true;
            this.ckEnableCredSspSupport.Location = new System.Drawing.Point(13, 227);
            this.ckEnableCredSspSupport.Name = "ckEnableCredSspSupport";
            this.ckEnableCredSspSupport.Size = new System.Drawing.Size(144, 16);
            this.ckEnableCredSspSupport.TabIndex = 6;
            this.ckEnableCredSspSupport.Text = "EnableCredSspSupport";
            this.ckEnableCredSspSupport.UseVisualStyleBackColor = true;
            // 
            // btnMax
            // 
            this.btnMax.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnMax.Location = new System.Drawing.Point(13, 367);
            this.btnMax.Name = "btnMax";
            this.btnMax.Size = new System.Drawing.Size(298, 39);
            this.btnMax.TabIndex = 7;
            this.btnMax.Text = "最大化";
            this.btnMax.UseVisualStyleBackColor = true;
            this.btnMax.Click += new System.EventHandler(this.btnMax_Click);
            // 
            // btnDisConnect
            // 
            this.btnDisConnect.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnDisConnect.Location = new System.Drawing.Point(13, 412);
            this.btnDisConnect.Name = "btnDisConnect";
            this.btnDisConnect.Size = new System.Drawing.Size(298, 39);
            this.btnDisConnect.TabIndex = 8;
            this.btnDisConnect.Text = "断开";
            this.btnDisConnect.UseVisualStyleBackColor = true;
            this.btnDisConnect.Click += new System.EventHandler(this.btnDisConnect_Click);
            // 
            // jingdutiao
            // 
            this.jingdutiao.Dock = System.Windows.Forms.DockStyle.Top;
            this.jingdutiao.Location = new System.Drawing.Point(13, 457);
            this.jingdutiao.Name = "jingdutiao";
            this.jingdutiao.Size = new System.Drawing.Size(298, 23);
            this.jingdutiao.TabIndex = 9;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(13, 261);
            this.label5.Margin = new System.Windows.Forms.Padding(3, 15, 3, 2);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(95, 12);
            this.label5.TabIndex = 1;
            this.label5.Text = "LoadBalanceInfo";
            // 
            // textLoadBalanceInfo
            // 
            this.textLoadBalanceInfo.Dock = System.Windows.Forms.DockStyle.Top;
            this.textLoadBalanceInfo.Location = new System.Drawing.Point(13, 278);
            this.textLoadBalanceInfo.Name = "textLoadBalanceInfo";
            this.textLoadBalanceInfo.Size = new System.Drawing.Size(298, 21);
            this.textLoadBalanceInfo.TabIndex = 5;
            // 
            // RemoteWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(992, 573);
            this.Controls.Add(this.splitContainer1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "RemoteWindow";
            this.Text = "RemoteWindow";
            this.Load += new System.EventHandler(this.RemoteWindow_Load);
            ((System.ComponentModel.ISupportInitialize)(this.rdpClient)).EndInit();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private AxMSTSCLib.AxMsRdpClient6NotSafeForScripting rdpClient;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtIp;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtUserName;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtPwd;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtPort;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.CheckBox ckEnableCredSspSupport;
        private System.Windows.Forms.Button btnMax;
        private System.Windows.Forms.Button btnDisConnect;
        private System.Windows.Forms.ProgressBar jingdutiao;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox textLoadBalanceInfo;
    }
}

