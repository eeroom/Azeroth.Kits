using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace logUtil
{
    class Program
    {
        static void Main(string[] args)
        {
            log4net.Config.XmlConfigurator.Configure(new System.IO.FileInfo("log4net.config"));
            var logger= log4net.LogManager.GetLogger("abc");
            logger.Error("hello world");
            System.Console.ReadLine();
        }
    }
}
