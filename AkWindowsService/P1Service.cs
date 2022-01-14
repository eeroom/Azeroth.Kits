using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AkWindowsService
{
    /// <summary>
    /// 这里嵌套了class只是为了避免类继承System.ServiceProcess.ServiceBase后，导致vs默认打开设计器
    /// </summary>
    public class P1Service
    {
        public class P1ServiceImpl:System.ServiceProcess.ServiceBase
        {
            System.ServiceModel.Web.WebServiceHost hs { set; get; }
            protected override void OnStart(string[] args)
            {
                this.hs = new System.ServiceModel.Web.WebServiceHost(typeof(Home));
                this.hs.Open();
            }

            protected override void OnStop()
            {
                this.hs.Close();
            }
        }
    }
}
