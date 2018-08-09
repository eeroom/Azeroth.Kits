using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Excel.Extension.Views
{
    public partial class ProcessDashboard : Form
    {
        public ProcessDashboard()
        {
            this.FormBorderStyle = FormBorderStyle.None;
            InitializeComponent();
            
        }
        bool flag = true;

        public Action TableMetaHandler { get; set; }

        private void ProcessDashboard_Load(object sender, EventArgs e)
        {
            System.Threading.ThreadPool.QueueUserWorkItem(x =>
            {
                int value = 0;
                while (flag)
                {
                    System.Threading.Thread.Sleep(100);
                    value += 10;
                    if (value > this.progressBar1.Maximum)
                        value = this.progressBar1.Minimum;
                    this.progressBar1.Invoke(new Action<int>(RefreshProcessValue),value);
                }
                this.progressBar1.Invoke(new Action<int>(RefreshProcessValue), this.progressBar1.Maximum);
                System.Threading.Thread.Sleep(1*1000);
                this.Invoke(new Action(()=>this.Close()));
            });
            System.Threading.ThreadPool.QueueUserWorkItem(x=> { this.TableMetaHandler();
                this.flag = false;
            });
        }

        private void RefreshProcessValue(int value)
        {
            this.progressBar1.Value = value;
        }
    }
}
