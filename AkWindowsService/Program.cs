using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AkWindowsService
{
    class Program
    {
        static void Main(string[] args)
        {
            var p1service = new P1Service.P1ServiceImpl();
            System.ServiceProcess.ServiceBase.Run(p1service);
        }
    }
}
