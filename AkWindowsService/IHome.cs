using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AkWindowsService
{
    [System.ServiceModel.ServiceContract]
    public interface IHome
    {
        [System.ServiceModel.OperationContract]
        [System.ComponentModel.Description("运行对应的winform程序")]
        int RunApp();
    }
}
